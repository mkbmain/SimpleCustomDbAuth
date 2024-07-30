using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AuthCustomDb.Db;

public class UserDbFactor : IDesignTimeDbContextFactory<UserDb>
{
    public static DbContextOptions<UserDb> GetOptions(string connectionString ="Data Source=Application.db;" )
    {
        var optionsBuilder = new DbContextOptionsBuilder<UserDb>();
        optionsBuilder.UseSqlite(connectionString);
        return optionsBuilder.Options;
    }   
    public static UserDb CreateDbContext()
        => new UserDb(GetOptions());

    public UserDb CreateDbContext(string[] args)
        => CreateDbContext();
}