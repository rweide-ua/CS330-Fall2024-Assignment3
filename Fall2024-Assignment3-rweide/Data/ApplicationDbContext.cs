using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Fall2024_Assignment3_rweide.Models;

namespace Fall2024_Assignment3_rweide.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    public DbSet<Fall2024_Assignment3_rweide.Models.Movie> Movie { get; set; } = default!;
    public DbSet<Fall2024_Assignment3_rweide.Models.Actor> Actor { get; set; } = default!;
}

