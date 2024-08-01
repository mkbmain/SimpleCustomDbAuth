using AuthCustomDb.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace AuthCustomDb.Db;

public class UserDb : DbContext
{
    public DbSet<User> Users { get; set; }

    public UserDb() : this((string?)null)
    {
    }

    public UserDb(string? connectionString) : base(GetOptions(connectionString))
    {
    }

    private static DbContextOptions GetOptions(string? connectionString)
    {
        // Example only
        return connectionString is null ? UserDbFactor.GetOptions() : UserDbFactor.GetOptions(connectionString);
    }

    public UserDb(DbContextOptions<UserDb> options)
        : base(options)
    {
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(w =>
        {
            w.Property(w => w.Password).HasMaxLength(150);
            w.Property(e => e.Email).HasMaxLength(150);
        });

        modelBuilder.Entity<Role>(w =>
        {
            w.Property(w => w.Name).HasMaxLength(35);
            w.HasData(Enum.GetValues<Roles>().Select(q => new Role { Id = (int)q, Name = q.ToString() }));
        });

        modelBuilder.Entity<UserRole>(q =>
            {
                q.HasOne<User>(w => w.User).WithMany(q => q.UserRoles);
                q.HasOne<Role>(w => w.Role).WithMany(q => q.UserRoles);
            }
        );
    }
}