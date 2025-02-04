using CleanArhictecture_2025.Domain.Users;

namespace CleanArchictecture.Application.Services;

public interface IJwtProvider
{
    public Task<string> CreateTokenAsync(AppUser user, CancellationToken cancellationToken = default);
}
