using KfnApi.Helpers.Database;
using Microsoft.EntityFrameworkCore;

namespace IntegrationTests.Extensions;

public static class DbContextExtensions
{
    public static void ClearData(this DatabaseContext dbContext)
    {
        ReloadDatabase(dbContext);
        dbContext.Orders.RemoveRange(dbContext.Orders.ToList());
        dbContext.Products.RemoveRange(dbContext.Products.ToList());
        dbContext.Producers.RemoveRange(dbContext.Producers.ToList());
        dbContext.Users.RemoveRange(dbContext.Users.ToList());
        dbContext.Uploads.RemoveRange(dbContext.Uploads.ToList());
        dbContext.ProducerReports.RemoveRange(dbContext.ProducerReports.ToList());
        dbContext.UserReports.RemoveRange(dbContext.UserReports.ToList());
        dbContext.Prices.RemoveRange(dbContext.Prices.ToList());
        dbContext.ApprovalForms.RemoveRange(dbContext.ApprovalForms.ToList());
        dbContext.SaveChanges();
        dbContext.ChangeTracker.Clear();
    }

    private static void ReloadDatabase(DbContext dbContext)
    {
        foreach (var item in dbContext.ChangeTracker.Entries())
        {
            item.State = EntityState.Detached;
        }
    }
}
