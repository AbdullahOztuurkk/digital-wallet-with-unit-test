using NSubstitute;
using Wallet.Domain.Entities;

namespace Wallet.Tests.Services.Transfer;

/*
    This file has unit tests of PreTransfer method.
*/

public partial class TransferServiceTests
{
    private TransferService _transferService;
    private WalletDbContext _context;
    private PreTransferRequestDtoValidator preTransferRequestValidator;
    private PreTransferRequestDto preTransferRequest;

    [OneTimeSetUp]
    public void CreateTestEnvironment()
    {
        preTransferRequestValidator = new PreTransferRequestDtoValidator();
        preTransferRequest = new PreTransferRequestDto()
        {
            Amount = 10,
            FromWalletNumber = "1234567890",
        };
    }

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<WalletDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString("N"))
            .Options;

        _context = new WalletDbContext(options);
        _transferService = new TransferService(_context);
    }

    [TearDown]
    public async Task TearDownAsync()
    {
        await _context.Database.EnsureDeletedAsync();
    }

    [Test]
    public void PreTransfer_WhenTransferTypeIsWallet2Wallet_ToWalletNumberShouldNotBeEmpty()
    {
        //Arrange
        preTransferRequest.ToWalletNumber = null;
        preTransferRequest.TransferType = TransferType.Wallet2Wallet;

        //Act
        var result = preTransferRequestValidator.Validate(preTransferRequest);

        //Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Count().Should().BeGreaterThanOrEqualTo(1);
    }

    [Test]
    public void PreTransfer_WhenTransferTypeIsWallet2Wallet_ToWalletNumberLengthShouldBe11()
    {
        //Arrange
        preTransferRequest.ToWalletNumber = "1234567890";
        preTransferRequest.TransferType = TransferType.Wallet2Wallet;

        //Act
        var result = preTransferRequestValidator.Validate(preTransferRequest);

        //Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Count().Should().BeGreaterThanOrEqualTo(1);
    }

    [Test]
    public void PreTransfer_WhenTransferTypeIsWallet2Account_ToAccountNumberShouldNotBeEmpty()
    {
        //Arrange
        preTransferRequest.ToAccountNumber = null;
        preTransferRequest.TransferType = TransferType.Wallet2Account;

        //Act
        var result = preTransferRequestValidator.Validate(preTransferRequest);

        //Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Count().Should().BeGreaterThanOrEqualTo(1);
    }

    [Test]
    public async Task PreTransfer_WhenWallet2WalletAndFromWalletDoesntExist_ShouldFail()
    {
        //Arrange
        preTransferRequest.ToWalletNumber = "12345678901";
        preTransferRequest.TransferType = TransferType.Wallet2Wallet;

        //Act
        var result = await _transferService.PreTransfer(preTransferRequest);

        //Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be(SystemError.E_0005.ErrorCode);
    }

    [Test]
    public async Task PreTransfer_WhenWallet2WalletAndFromWalletIsBlocked_ShouldFail()
    {
        //Arrange
        preTransferRequest.ToWalletNumber = "12345678901";
        preTransferRequest.TransferType = TransferType.Wallet2Wallet;

        var fromWallet = new WalletEntity
        {
            Email = "abc@de.com",
            Msisdn = "1234567890",
            WalletNumber = preTransferRequest.FromWalletNumber,
            Balance = 10,
            Status = StatusType.Blocked
        };

        await _context.Wallets.AddAsync(fromWallet);
        await _context.SaveChangesAsync();

        //Act
        var result = await _transferService.PreTransfer(preTransferRequest);

        //AssertS
        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be(SystemError.E_0009.ErrorCode);
    }

    [Test]
    public async Task PreTransfer_WhenWallet2WalletAndToWalletDoesntExist_ShouldFail()
    {
        //Arrange
        preTransferRequest.ToWalletNumber = "12345678901";
        preTransferRequest.TransferType = TransferType.Wallet2Wallet;

        var fromWallet = new WalletEntity
        {
            Email = "abc@de.com",
            Msisdn = "1234567890",
            WalletNumber = preTransferRequest.FromWalletNumber,
            Balance = 10,
            Status = StatusType.Active
        };

        _context.Wallets.Add(fromWallet);
        _context.SaveChanges();

        //Act
        var result = await _transferService.PreTransfer(preTransferRequest);

        //Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be(SystemError.E_0007.ErrorCode);
    }

    [Test]
    public async Task PreTransfer_WhenWallet2WalletAndFromWalletDoesntHaveEnoughBalance_ShouldFail()
    {
        //Arrange
        preTransferRequest.ToWalletNumber = "12345678901";
        preTransferRequest.TransferType = TransferType.Wallet2Wallet;

        var fromWallet = new WalletEntity
        {
            Email = "abc@de.com",
            Msisdn = "1234567890",
            WalletNumber = preTransferRequest.FromWalletNumber,
            Balance = 7, // Should less than request.Amount property
            Status = StatusType.Active
        };

        var toWallet = new WalletEntity
        {
            Email = "abc@de.com",
            Msisdn = "1234567891",
            WalletNumber = preTransferRequest.ToWalletNumber,
            Balance = 0,
            Status = StatusType.Active
        };

        _context.Wallets.Add(fromWallet);
        _context.Wallets.Add(toWallet);
        _context.SaveChanges();

        //Act
        var result = await _transferService.PreTransfer(preTransferRequest);

        //Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be(SystemError.E_0010.ErrorCode);
    }

    [Test]
    public async Task PreTransfer_WhenAllValidationsAreValid_ShouldReturnSuccessResponse()
    {
        //Arrange
        preTransferRequest.ToWalletNumber = "12345678901";
        preTransferRequest.TransferType = TransferType.Wallet2Wallet;

        var fromWallet = new WalletEntity
        {
            Email = "abc@de.com",
            Msisdn = "1234567890",
            WalletNumber = preTransferRequest.FromWalletNumber,
            Balance = 11, // Should greater than request.Amount property
            Status = StatusType.Active
        };

        var toWallet = new WalletEntity
        {
            Email = "abc@de.com",
            Msisdn = "1234567891",
            WalletNumber = preTransferRequest.ToWalletNumber,
            Balance = 0,
            Status = StatusType.Active
        };

        _context.Wallets.Add(fromWallet);
        _context.Wallets.Add(toWallet);
        _context.SaveChanges();

        //Act
        var result = await _transferService.PreTransfer(preTransferRequest);

        //Assert
        result.IsSuccess.Should().BeTrue();        
    }

    [Test]
    public async Task Wallet2Wallet_WhenProcessGuidIsInValid_ShouldFail()
    {
        //Arrange
        var wallet2WalletRequest = new Wallet2WalletRequestDto(Arg.Any<string>(),0);

        //Act
        var result = await _transferService.Wallet2Wallet(wallet2WalletRequest);

        //Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be(SystemError.E_0014.ErrorCode);
    }

    [Test]
    public async Task Wallet2Wallet_WhenProcessTypeAndProcessSubTypeIsInValid_ShouldFail()
    {
        //Arrange
        var guid = Guid.NewGuid().ToString("N");
        var wallet2WalletRequest = new Wallet2WalletRequestDto(guid,10);
        var transaction = new Transaction
        {
            Amount = 10,
            FromWalletNumber = "12345678901",
            TransactionDate = DateTime.UtcNow.AddHours(3),
            ProcessType = ProcessType.BalanceTransfer,
            ProcessSubType = (int)TransferType.Wallet2Account,
            ProcessGuid = guid,
            Status = StatusType.Pending,
        };

        _context.Transactions.Add(transaction);
        _context.SaveChanges();

        //Act
        var result = await _transferService.Wallet2Wallet(wallet2WalletRequest);

        //Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be(SystemError.E_0015.ErrorCode);
    }

    [Test]
    public async Task Wallet2Wallet_WhenTransactionDateIsInValid_ShouldFail()
    {
        //Arrange
        var guid = Guid.NewGuid().ToString("N");
        var wallet2WalletRequest = new Wallet2WalletRequestDto(guid,10);
        var transaction = new Transaction
        {
            Amount = 10,
            FromWalletNumber = "12345678901",
            TransactionDate = DateTime.UtcNow.AddHours(3).AddMinutes(-3),
            ProcessType = ProcessType.BalanceTransfer,
            ProcessSubType = (int)TransferType.Wallet2Wallet,
            ProcessGuid = guid,
            Status = StatusType.Pending,
        };

        _context.Transactions.Add(transaction);
        _context.SaveChanges();

        //Act
        var result = await _transferService.Wallet2Wallet(wallet2WalletRequest);

        //Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be(SystemError.E_0011.ErrorCode);
    }

    [Test]
    public async Task Wallet2Wallet_WhenTransactionStatusIsNotPending_ShouldFail()
    {
        //Arrange
        var guid = Guid.NewGuid().ToString("N");
        var wallet2WalletRequest = new Wallet2WalletRequestDto(guid,10);
        var transaction = new Transaction
        {
            Amount = 10,
            FromWalletNumber = "12345678901",
            TransactionDate = DateTime.UtcNow.AddHours(3).AddMinutes(1),
            ProcessType = ProcessType.BalanceTransfer,
            ProcessSubType = (int)TransferType.Wallet2Wallet,
            ProcessGuid = guid,
            Status = StatusType.Success,
        };

        _context.Transactions.Add(transaction);
        _context.SaveChanges();

        //Act
        var result = await _transferService.Wallet2Wallet(wallet2WalletRequest);

        //Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be(SystemError.E_0012.ErrorCode);
    }

    [Test]
    public async Task Wallet2Wallet_WhenAmountDoesntMatch_ShouldFail()
    {
        //Arrange
        var guid = Guid.NewGuid().ToString("N");
        var wallet2WalletRequest = new Wallet2WalletRequestDto(guid,10);
        var transaction = new Transaction
        {
            Amount = 11,//Should be different from request.MatchAmount
            FromWalletNumber = "12345678901",
            TransactionDate = DateTime.UtcNow.AddHours(3),
            ProcessType = ProcessType.BalanceTransfer,
            ProcessSubType = (int)TransferType.Wallet2Wallet,
            ProcessGuid = guid,
            Status = StatusType.Pending,
        };

        _context.Transactions.Add(transaction);
        _context.SaveChanges();

        //Act
        var result = await _transferService.Wallet2Wallet(wallet2WalletRequest);

        //Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be(SystemError.E_0013.ErrorCode);
    }

    [Test]
    public async Task Wallet2Wallet_WhenAllValidationsAreValid_ShouldReturnSuccessResponse()
    {
        //Arrange
        var guid = Guid.NewGuid().ToString("N");
        var wallet2WalletRequest = new Wallet2WalletRequestDto(guid,10);
        var transaction = new Transaction
        {
            Amount = wallet2WalletRequest.MatchAmount,
            FromWalletNumber = "12345678901",
            TransactionDate = DateTime.UtcNow.AddHours(3).AddMinutes(1),
            ProcessType = ProcessType.BalanceTransfer,
            ProcessSubType = (int)TransferType.Wallet2Wallet,
            ProcessGuid = guid,
            Status = StatusType.Pending,
        };

        _context.Transactions.Add(transaction);
        _context.SaveChanges();

        //Act
        var result = await _transferService.Wallet2Wallet(wallet2WalletRequest);

        //Assert
        result.IsSuccess.Should().BeTrue();
    }
}