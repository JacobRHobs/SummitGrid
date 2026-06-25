using Microsoft.EntityFrameworkCore;
using SummitGrid.Core.Entities;

namespace SummitGrid.Infrastructure;

public class SummitGridDbContext: DbContext{

    public DbSet<ClimbingArea> ClimbingAreas {get; set; }

    public DbSet<Route> Routes {get; set; }

    public DbSet<Incident> Incidents {get; set; }

    public DbSet<User> Users {get; set; }

    public DbSet<AuditLog> AuditLogs {get; set; }
    

    public SummitGridDbContext(DbContextOptions<SummitGridDbContext> options) : base(options) {}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ClimbingArea>()
            .Property(a => a.Location)
            .HasColumnType("geography");
    }
}