using Microsoft.EntityFrameworkCore;
using Wallet.Application.Services.Concrete;
using Wallet.Persistence.Context;

namespace Wallet.Tests;

public class TransferServiceTests
{
    private TransferService _transferService;
    private WalletDbContext _context;

    [OneTimeSetUp]
    public void CreateTestEnvironment()
    {
        var options = new DbContextOptionsBuilder<WalletDbContext>()
            .UseInMemoryDatabase("WalletDb")
            .Options;

        _context = new WalletDbContext(options);

        _transferService = new TransferService(_context);
    }

    [OneTimeTearDown]
    public void ResetTestEnvironment()
    {
        _context.Dispose();
    }
}