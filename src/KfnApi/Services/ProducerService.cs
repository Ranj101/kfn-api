using KfnApi.Abstractions;
using KfnApi.Helpers.Database;
using KfnApi.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace KfnApi.Services;

public class ProducerService : IProducerService
{
    private readonly DatabaseContext _databaseContext;

    public ProducerService(DatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
    }

    public async Task<Producer?> GetProducerByIdAsync(Guid id)
    {
        return await _databaseContext.Producers
            .Include(p => p.User)
            .Include(p => p.Products)
            .FirstOrDefaultAsync(p => p.Id == id);
    }
}
