using LMS.Domain.Entities;

namespace LMS.Infrastructure.Services;

public interface ITokenService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
}
