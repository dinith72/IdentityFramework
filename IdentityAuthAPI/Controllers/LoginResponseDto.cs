using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityAuthAPI.Controllers
{
    public class LoginResponseDto
    {
        public string status { get; set; }

        public int stastusCode { get; set; }

        public string authToken { get; set; }

      

    }
}
