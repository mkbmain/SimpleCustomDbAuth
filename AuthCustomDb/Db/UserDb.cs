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
        return connectionString is null ?  UserDbFactor.GetOptions() : UserDbFactor.GetOptions(connectionString);
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
        }
    );
}
}