using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Vitrine.Models;

namespace VitrineApi.Data
{
    public class UserAuthDbContext : IdentityDbContext<Lojista>
    {
        public UserAuthDbContext(DbContextOptions options) : base(options)
        {
        }
    }
}
