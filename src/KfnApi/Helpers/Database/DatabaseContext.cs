using Microsoft.EntityFrameworkCore;

namespace KfnApi.Helpers.Database;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }
}


