using Microsoft.AspNetCore.Identity;

namespace CleanArhictecture_2025.Domain.Users;

public sealed class AppUser : IdentityUser<Guid>
{
    public AppUser()
    {
        Id = Guid.CreateVersion7();
    }
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string FullName => $"{FirstName} {LastName}";

    #region Audit Log
    public DateTimeOffset CreateAt { get; set; }
    public string CreateUserId { get; set; } = default!;
    public DateTimeOffset? UpdateAt { get; set; }
    public string? UpdateUserId { get; set; }
    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeleteAt { get; set; }
    public string? DeleteUserId { get; set; }
    #endregion
}
