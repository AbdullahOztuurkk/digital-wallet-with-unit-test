using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Wallet.Application.Services.Concrete;
using Wallet.Domain.Dtos.Wallet;
using Wallet.Persistence.Context;
using Wallet.Shared.Domain.Enums;

namespace Wallet.Tests.Services.Wallet;

public class WalletServiceTests
{
    private WalletService _walletService;
    private WalletDbContext _context;
    private WalletInsertDtoValidator insertDtoValidator;
    /*
     [Setup] -> Her test öncesi baþlýyor.
     [TearDown] -> Her test sonrasý baþlýyor. Dispose()
     [OneTimeSetUp] -> Tüm testlerden önce bi kez çalýþýyor.
     [OneTimeTearDown] -> Tüm testler bittiðinde çalýþtýran bir metot.
     */

    /*
     -> NUnit
     XUnit
     MSTest
     */

    [OneTimeSetUp]
    public void CreateTestEnvironment()
    {
        var options = new DbContextOptionsBuilder<WalletDbContext>()
            .UseInMemoryDatabase("WalletDb")
            .Options;

        _context = new WalletDbContext(options);
        insertDtoValidator = new WalletInsertDtoValidator();
        _walletService = new WalletService(_context);
    }

    [Test]
    public void CreateWallet_WhenValuesAreCorrect_ValidationResultMustBeValid()
    {
        //Arrange
        var insertDto = new WalletInsertDto("abdullah@gmail.com", "1234567890");

        //Act
        var result = insertDtoValidator.Validate(insertDto);

        //Assert
        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    public void CreateWallet_WhenEmailAreCorrect_ValidationResultMustBeNotValid()
    {
        //Arrange
        var insertDto = new WalletInsertDto("abdullah@gmail.com", "123457890");

        //Act
        var result = insertDtoValidator.Validate(insertDto);

        //Assert
        Assert.That(result.IsValid, Is.False);
    }

    [Test]
    public void CreateWallet_WhenMsisdnAreCorrect_ValidationResultMustBeNotValid()
    {
        //Arrange
        var insertDto = new WalletInsertDto("abdullahgmail.com", "1234567890");

        //Act
        var result = insertDtoValidator.Validate(insertDto);

        //Assert
        Assert.That(result.IsValid, Is.False);
    }

    [Test]
    public void CreateWallet_WhenValuesAreNotCorrect_ValidationResultMustBeNotValid()
    {
        //Arrange
        var insertDto = new WalletInsertDto("abdullahgmail.com", "123456789a");

        //Act
        var result = insertDtoValidator.Validate(insertDto);

        //Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Count, Is.EqualTo(2));
    }

    [Test]
    public async Task CreateWallet_WhenValuesAreCorrect_WalletPropertiesShouldBeOkay()
    {
        //Arrange
        var insertDto = new WalletInsertDto("abdullah@gmail.com", "1234567890");

        //Act
        var result = insertDtoValidator.Validate(insertDto);
        var wallet = await _walletService.CreateWalletAsync(insertDto);

        //Assert
        Assert.That(result.IsValid, Is.True);
        wallet.IsSuccess.Should().BeTrue();
        wallet.Data.Email.Should().BeEquivalentTo(insertDto.Email);
        wallet.Data.Status.Should().Be(StatusType.Active);
        wallet.Data.Msisdn.Should().BeEquivalentTo(insertDto.Msisdn);
    }

    [OneTimeTearDown]
    public void ResetTestEnvironment()
    {
        _context.Dispose();
        _walletService = null;
    }
}