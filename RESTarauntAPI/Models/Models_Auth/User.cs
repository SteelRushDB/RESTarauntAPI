namespace RESTarauntAPI.Models;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string Role { get; set; } = "User";
    
    public DateTime CreatedAt { get;}   
    public DateTime UpdatedAt { get; set; }

    public User()
    {
        CreatedAt = DateTime.Now;
        UpdatedAt = DateTime.Now;
    }
}
