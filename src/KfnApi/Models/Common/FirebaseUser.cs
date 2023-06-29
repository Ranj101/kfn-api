namespace KfnApi.Models.Common;

public class FirebaseUser
{
    public string Id { get; }
    public string Email { get; }
    public Guid? ProfilePicture { get; }
    public string FirstName { get; }
    public string LastName { get; }
    public bool EmailVerified { get; }

    public FirebaseUser(string id, string email, string firstName, string lastName, Guid? profilePicture, bool emailVerified)
    {
        Id = id;
        Email = email;
        ProfilePicture = profilePicture;
        FirstName = firstName;
        LastName = lastName;
        EmailVerified = emailVerified;
    }
}
