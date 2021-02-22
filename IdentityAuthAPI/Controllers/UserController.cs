using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace IdentityAuthAPI.Controllers
{

    public class LoginDataModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class PasswordResetTokenModel
    {
        public string Email { get; set; }
    }


    public class ElevateDataModel
    {
        public string Email { get; set; }

        public int RoleType { get; set; }
    }
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IConfiguration _configeration;

        public UserController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<IdentityUser> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _configeration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] LoginDataModel loginDataModel)
        {

            IdentityUser identityUser = new IdentityUser();
            identityUser.Email = loginDataModel.Email;
            identityUser.UserName = loginDataModel.Email;

            var result = await _userManager.CreateAsync(identityUser, loginDataModel.Password);

            if (result.Succeeded)
            {
                // generate the email confirmation token
                string emailConfirmToken = await _userManager.GenerateEmailConfirmationTokenAsync(identityUser);
                await _userManager.SetAuthenticationTokenAsync(identityUser, "TestApp", "EmailConfirmation", emailConfirmToken);


                // a default role is assigned to the user
                await _userManager.AddToRoleAsync(identityUser, "visitor");
                return Ok(emailConfirmToken);
            }
            else
            {
                return NotFound();
            }



            //var rng = new Random();
            //return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            //{
            //    Date = DateTime.Now.AddDays(index),
            //    TemperatureC = rng.Next(-20, 55),
            //    Summary = Summaries[rng.Next(Summaries.Length)]
            //})
            //.ToArray();


            // return NotFound();

        }


        // basic login user call 
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDataModel loginData)
        {

            var result = await _signInManager.PasswordSignInAsync(loginData.Email, loginData.Password, false, false);

            if (result.Succeeded)
            {
                IdentityUser user = await _userManager.FindByEmailAsync(loginData.Email);
                AuthenticateService authenticateService = new AuthenticateService(_userManager, _roleManager, _configeration);
                string tokenStr = authenticateService.AuthenticateUser(user);



                return Ok(new LoginResponseDto()
                {
                    authToken = tokenStr,
                    stastusCode = 200,
                    status = "Success"
                });
            }
            else
            {
                return BadRequest();
            }

        }


        [HttpPost("elevateUser")]
        public async Task<IActionResult> ElevateUserRole([FromBody] ElevateDataModel elevateDataModel)
        {
            if (elevateDataModel.RoleType == 1)
            {
                IdentityUser identityUser = await _userManager.FindByEmailAsync(elevateDataModel.Email);
                if (identityUser != null)
                {
                    await _userManager.RemoveFromRoleAsync(identityUser, "visitor");
                    await _userManager.AddToRoleAsync(identityUser, "Buyer");
                    return Ok();
                }
                else
                {
                    return BadRequest();
                }
            }
            else
            {
                return BadRequest();
            }
        }
        [HttpPost("resetPassword")]
        public async Task<IActionResult> GeneratePasswordResetToken([FromBody] PasswordResetTokenModel resetTokenModel)
        {
            var user = await _userManager.FindByEmailAsync(resetTokenModel.Email);
            if (user == null)
                return BadRequest();
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            await _userManager.SetAuthenticationTokenAsync(user, "TestApp", "ResetPassword", token);
            return Ok(token);
        }


        [HttpGet("userInfo")]
        [Authorize(Roles = RolesEnum.BUYER)]
        public async Task<ActionResult<IdentityUser>> GetUserData()
        {
            IdentityUser identityUser = await _userManager.FindByIdAsync("44bcf427-11fc-49ad-bdbd-42923d9182f9");

            return identityUser;
        }
    }
}
