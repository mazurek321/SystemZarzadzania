namespace Core.Models.Tokens;
public interface ITokenService
{
    string GenerateToken(TokenGenerationRequest request);
}