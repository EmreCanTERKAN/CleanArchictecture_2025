﻿namespace CleanArchictecture.Domain.Abstractions;

public abstract class Entity
{
    public Entity()
    {
        Id = Guid.CreateVersion7();
    }
    public Guid Id { get; set; }
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
