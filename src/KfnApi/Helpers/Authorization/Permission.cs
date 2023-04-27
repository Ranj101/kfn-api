namespace KfnApi.Helpers.Authorization;

public enum Permission
{
    None,

    GetForms,               // customer
    GetOrders,              // customer
    GetBasicOrders,         // customer
    GetProducerPages,       // customer
    GetProducts,            // customer
    GetUserProfiles,        // customer
    GetSelf,                // customer

    GetProducers,           // admin
    GetReports,             // admin
    GetUsers,               // admin

    GetFormById,            // customer
    GetOrderById,           // customer
    GetProducerPageById,    // customer
    GetProductById,         // customer
    GetUserProfileById,     // customer

    GetProducerById,        // admin
    GetReportById,          // admin
    GetUserById,            // admin

    SubmitForm,             // customer
    SubmitOrder,            // customer
    SubmitReport,           // customer
    UploadFile,             // customer

    CreateProduct,          // producer

    UpdateOrder,            // customer
    UpdateSelf,             // customer
    UpdateForm,             // customer

    UpdateProduct,          // producer
    UpdateProducer,         // producer

    UpdateOrderState,       // customer

    UpdateProductState,     // producer

    UpdateFormState,        // admin
    UpdateProducerState,    // admin
    UpdateUserState,        // admin

    DeleteProduct           // producer
}
