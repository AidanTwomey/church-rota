using Microsoft.EntityFrameworkCore;

namespace ChurchRota.Library.Data;

public class ChurchRotaContext : DbContext
{
    public ChurchRotaContext(DbContextOptions<ChurchRotaContext> options)
        : base(options)
    {
    }

    public DbSet<Person> People { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<PeopleRole> PeopleRoles { get; set; }
    public DbSet<Schedule> Schedules { get; set; }
    public DbSet<Solemnity> Solemnities { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Person>(entity =>
        {
            entity.HasKey(e => e.PersonId);
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.Phone).HasMaxLength(20);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId);
            entity.Property(e => e.RoleName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
        });

        modelBuilder.Entity<Schedule>(entity =>
        {
            entity.HasKey(e => e.ScheduleId);
            entity.Property(e => e.Notes).HasMaxLength(500);
            
            entity.HasOne(d => d.Person)
                .WithMany(p => p.Schedules)
                .HasForeignKey(d => d.PersonId);

            entity.HasOne(d => d.Role)
                .WithMany(p => p.Schedules)
                .HasForeignKey(d => d.RoleId);
        });

        modelBuilder.Entity<PeopleRole>(entity =>
        {
            // Composite primary key
            entity.HasKey(e => new { e.PersonId, e.RoleId });

            entity.HasOne(e => e.Person)
                .WithMany(p => p.PeopleRoles)
                .HasForeignKey(e => e.PersonId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Role)
                .WithMany(r => r.PeopleRoles)
                .HasForeignKey(e => e.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Solemnity>(entity =>
        {
            entity.HasKey(e => e.SolemnityId);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Date).IsRequired();
            entity.Property(e => e.Season).IsRequired().HasMaxLength(50);
        });
    }
}