using System.ComponentModel.DataAnnotations;

namespace UnitTests.Helpers;

public static class ValidationHelper
{
    public static bool ValidateObject(object someObject)
    {
        var validationResults = new List<ValidationResult>();
        return Validator.TryValidateObject(someObject, new ValidationContext(someObject), validationResults, true);
    }
}
