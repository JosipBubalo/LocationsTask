using LocationsAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LocationsAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<LocationRequest> Requests { get; set; }
        public DbSet<LocationResponse> Responses { get; set; }
    }
}
