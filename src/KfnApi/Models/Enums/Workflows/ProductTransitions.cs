namespace KfnApi.Models.Enums.Workflows;

public enum ExposedProductTrigger
{
    MakeAvailable,
    MakeUnavailable,
}

public enum ProductTrigger
{
    MakeAvailable,
    MakeUnavailable,
    MarkAsModified
}

public enum ProductState
{
    Available,
    Unavailable,
    Modified
}
