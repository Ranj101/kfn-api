namespace KfnApi.Models.Common;

public class FirebaseUser
{
    public string Id { get; }
    public string Email { get; }
    public Guid? Picture { get; }
    public string FirstName { get; }
    public string LastName { get; }
    public bool EmailVerified { get; }

    public FirebaseUser(string id, string email, string firstName, string lastName, Guid? picture, bool emailVerified)
    {
        Id = id;
        Email = email;
        Picture = picture;
        FirstName = firstName;
        LastName = lastName;
        EmailVerified = emailVerified;
    }
}
