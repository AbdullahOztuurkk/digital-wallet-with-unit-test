using Microsoft.EntityFrameworkCore;
using Wallet.Domain.Entities;
using WalletEntity = Wallet.Domain.Entities.Wallet;

namespace Wallet.Persistence.Context;
public interface IBaseDbContext
{
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<WalletEntity> Wallets { get; set; }
    int SaveChanges();
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

}
