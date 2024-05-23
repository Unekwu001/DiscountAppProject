using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Data.DatabaseContext
{
    public class DiscountDbContext : DbContext
    {
        public DiscountDbContext(DbContextOptions<DiscountDbContext> options) : base(options) { }



        public DbSet<DiscountCode> DiscountCodes { get; set; }
    }
}
