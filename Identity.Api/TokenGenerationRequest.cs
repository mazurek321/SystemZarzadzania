using Database.Data.Models;

namespace Identity.Api;

public class TokenGenerationRequest{
    public Guid UserId { get; set; }
    public String Email { get; set; }
    public User.UserRole Role { get; set; }

}