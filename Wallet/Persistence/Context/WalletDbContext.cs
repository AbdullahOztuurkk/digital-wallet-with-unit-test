using WalletEntity = Wallet.Domain.Entities.Wallet;

namespace Wallet.Persistence.Context;
public class WalletDbContext : DbContext, IBaseDbContext
{
    public WalletDbContext(DbContextOptions<WalletDbContext> options) : base(options)
    {
        
    }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<WalletEntity> Wallets { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseInMemoryDatabase("walletDb");
    }

}
