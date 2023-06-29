using System.Security.Claims;
using KfnApi.Models.Common;

namespace KfnApi.Helpers.Extensions;

public static class FirebaseUserHttpContextExtensions
{
    public static FirebaseUser GetFirebaseUser(this HttpContext httpContext)
    {
        var claimsPrincipal = httpContext.User;

        var id = claimsPrincipal.FindFirstValue(Constants.FirebaseUserClaimType.Id);
        var email = claimsPrincipal.FindFirstValue(Constants.FirebaseUserClaimType.Email);
        var profilePicture = claimsPrincipal.FindFirstValue(Constants.FirebaseUserClaimType.ProfilePicture);
        var username = claimsPrincipal.FindFirstValue(Constants.FirebaseUserClaimType.Username);
        var conversionResult = bool.TryParse(claimsPrincipal.FindFirstValue(Constants.FirebaseUserClaimType.EmailVerified), out var emailVerified);

        if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(username) || !conversionResult)
            throw new Exception("Some firebase user claims are missing.");

        Guid? profilePictureId = null;

        if (!string.IsNullOrWhiteSpace(profilePicture))
            try
            {
                profilePictureId = Guid.Parse(profilePicture);
            }
            catch (Exception)
            {
                profilePictureId = null;
            }

        var names = username.Split("_+_");

        return new FirebaseUser(id, email, names[0], names[1], profilePictureId, emailVerified);
    }
}
