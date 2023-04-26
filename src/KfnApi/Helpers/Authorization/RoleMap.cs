using KfnApi.Abstractions;

namespace KfnApi.Helpers.Authorization;

public class RoleMap : IRoleMap
{
    private readonly IReadOnlyDictionary<string, Permission[]> _roleDefinitions;

    public RoleMap()
    {
        _roleDefinitions = CreateRolePermissionsMap();
    }

    public IReadOnlyDictionary<string, Permission[]> GetRoleDefinitions()
        => _roleDefinitions;

    private static IReadOnlyDictionary<string, Permission[]> CreateRolePermissionsMap()
    {
        var allPermissions = Enum.GetValues<Permission>()[1..].ToArray();

        return new Dictionary<string, Permission[]>
        {
            { Roles.SuperAdmin, allPermissions },
            { Roles.SystemAdmin, allPermissions },
            { Roles.Producer, GetProducerPermissions() },
            { Roles.Customer, GetCustomerPermissions() }
        };
    }

    private static Permission[] GetProducerPermissions()
    {
        return new []
        {
            Permission.CreateProduct,
            Permission.UpdateProduct,
            Permission.UpdateProducer,
            Permission.UpdateProductState,
            Permission.DeleteProduct
        };
    }

    private static Permission[] GetCustomerPermissions()
    {
        return new[]
        {
            Permission.GetSelf,
            Permission.GetForms, // update later
            Permission.GetOrders,
            Permission.GetBasicOrders,
            Permission.GetProducerPages,
            Permission.GetProducts,
            Permission.GetUserProfiles,
            Permission.GetFormById, // update later
            Permission.GetOrderById,
            Permission.GetProducerPageById,
            Permission.GetProductById,
            Permission.GetUserProfileById,
            Permission.SubmitForm,
            Permission.SubmitOrder,
            Permission.SubmitReport,
            Permission.UploadFile,
            Permission.UpdateOrder,
            Permission.UpdateSelf,
            Permission.UpdateForm,
            Permission.UpdateOrderState
        };
    }
}
