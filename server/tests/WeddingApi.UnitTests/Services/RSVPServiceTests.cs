using Microsoft.Extensions.Logging;
using Moq;
using WeddingApi.Models;
using WeddingApi.Repository;
using WeddingApi.Repository.Interfaces;
using WeddingApi.Services;
using Xunit;

namespace WeddingApi.UnitTests;

public class RSVPServiceTests
{
  private readonly Mock<IUnitOfWork<ApplicationDbContext>> _mockUnitOfWork;
  private readonly Mock<IGenericAsyncRepository<RSVP>> _mockRepository;
  private readonly Mock<ILogger<GenericAsyncDataService<RSVP, ApplicationDbContext>>> _mockLogger;

  public RSVPServiceTests()
  {
    _mockUnitOfWork = new Mock<IUnitOfWork<ApplicationDbContext>>();
    _mockRepository = new Mock<IGenericAsyncRepository<RSVP>>();
    _mockLogger = new Mock<ILogger<GenericAsyncDataService<RSVP, ApplicationDbContext>>>();

    _mockUnitOfWork
        .Setup(uow => uow.GetGenericAsyncRepository<RSVP>())
        .Returns(_mockRepository.Object);
  }

  [Fact]
  public async Task AddAndSaveAsync_CallsRepositoryAndSavesChanges()
  {
    var service = new GenericAsyncDataService<RSVP, ApplicationDbContext>(
        _mockUnitOfWork.Object,
        _mockLogger.Object);

    var rsvp = new RSVP
    {
      FirstName = "Test",
      LastName = "User",
      Email = "test@example.com",
      IsAttending = true
    };

    _mockRepository
        .Setup(r => r.AddAsync(It.IsAny<RSVP>(), It.IsAny<CancellationToken>()))
        .Returns((RSVP entity, CancellationToken token) => default);

    await service.AddAndSaveAsync(rsvp);

    _mockRepository.Verify(r => r.AddAsync(It.Is<RSVP>(e => e.Email == "test@example.com"), It.IsAny<CancellationToken>()), Times.Once);
    _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
  }

  [Fact]
  public async Task GetByIdAsync_ReturnsRSVP_WhenFound()
  {
    var id = Guid.NewGuid();
    var rsvp = new RSVP
    {
      Id = id,
      FirstName = "Test",
      LastName = "User",
      Email = "test@example.com"
    };

    _mockRepository
        .Setup(r => r.GetByIdAsync(id, It.IsAny<object>()))
        .ReturnsAsync(rsvp);

    var service = new GenericAsyncDataService<RSVP, ApplicationDbContext>(
        _mockUnitOfWork.Object,
        _mockLogger.Object);

    var result = await service.GetByIdAsync(id);

    Assert.Equal(id, result.Id);
    Assert.Equal("test@example.com", result.Email);
  }

  [Fact]
  public async Task GetAllAsync_LogsError_WhenExceptionOccurs()
  {
    _mockRepository
        .Setup(r => r.GetListAsync(null, null, null, It.IsAny<CancellationToken>(), false, false))
        .ThrowsAsync(new Exception("Test exception"));

    var service = new GenericAsyncDataService<RSVP, ApplicationDbContext>(
        _mockUnitOfWork.Object,
        _mockLogger.Object);

    await Assert.ThrowsAsync<Exception>(() => service.GetAllAsync());

    _mockLogger.Verify(
        x => x.Log(
            LogLevel.Error,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => true),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception, string>>()),
        Times.Once);
  }
}
