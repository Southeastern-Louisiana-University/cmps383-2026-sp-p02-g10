namespace Selu383.SP26.Api.Features.Users;

public class User
{
    public int Id { get; set; }

    public string UserName { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public List<Role> Roles { get; set; } = new();
}
