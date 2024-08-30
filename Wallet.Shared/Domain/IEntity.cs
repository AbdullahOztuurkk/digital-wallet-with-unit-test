using Wallet.Shared.Domain.Enums;

namespace Wallet.Shared.Domain;
public interface IEntity
{
    StatusType Status { get; set; }
    int Id { get; set; }
}

public interface IEntity<T> : IEntity
{
    public new T Id { get; set; }
}