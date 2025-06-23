namespace Core.Models;
public interface ITokenService
{
    string GenerateToken(TokenGenerationRequest request);
}