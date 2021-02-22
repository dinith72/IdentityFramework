using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityAuthAPI
{
    public class TestIdentityDbContext : IdentityDbContext<IdentityUser>
    {
        public TestIdentityDbContext(DbContextOptions<TestIdentityDbContext> options)
            : base(options)
        {
        }
    }
}
