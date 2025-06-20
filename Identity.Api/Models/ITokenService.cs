using Database.Data.Models;
using Identity.Api;

public interface ITokenService
{
    string GenerateToken(TokenGenerationRequest request);
}