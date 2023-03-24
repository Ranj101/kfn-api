using KfnApi.Models.Entities;

namespace KfnApi.Abstractions;

public interface IProducerService
{
    Task<Producer?> GetProducerByIdAsync(Guid id);
}
