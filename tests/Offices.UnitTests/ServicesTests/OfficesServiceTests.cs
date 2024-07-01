using AutoFixture;
using AutoMapper;
using Moq;
using Offices.Contracts.DTOs;
using Offices.Domain.Entities;
using Offices.Domain.Interfaces;
using Offices.Services.Services;
using OneOf;
using OneOf.Types;

namespace Offices.UnitTests.ServicesTests;

public class OfficesServiceTests
{
    private readonly OfficesService _sut;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IOfficesRepository> _officesRepositoryMock;

    public OfficesServiceTests()
    {
        _mapperMock = new Mock<IMapper>();
        _officesRepositoryMock = new Mock<IOfficesRepository>();
        _sut = new OfficesService(_officesRepositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task GetAllOfficesAsync_GetAllOffices_ReturnsMappedListOfOffices()
    {
        //Arrange
        var officesListMock = new Fixture().Create<List<Office>>();
        var mappedOfficesListMock = new Fixture().Create<List<OfficeShortInfoDTO>>();

        _officesRepositoryMock.Setup(x => x.GetAllAsync())
            .ReturnsAsync(officesListMock);

        _mapperMock.Setup(x => x.Map<List<OfficeShortInfoDTO>>(officesListMock))
            .Returns(mappedOfficesListMock);

        //Act
        var result = await _sut.GetAllOfficesAsync();

        //Assert
        Assert.IsType<List<OfficeShortInfoDTO>>(result.Value);
        _officesRepositoryMock.Verify(x => x.GetAllAsync(), Times.Once);
        _mapperMock.Verify(x => x.Map<List<OfficeShortInfoDTO>>(officesListMock), Times.Once);
    }

    [Fact]
    public async Task GetAllOfficesAsync_RepositoryReturnsNothinFromDatabase_ReturnsNotFoundResult()
    {
        //Arrange
        var emptyOfficesListMock = new List<Office>();
        var mappedOfficesListMock = new Fixture().Create<List<OfficeShortInfoDTO>>();

        _officesRepositoryMock.Setup(x => x.GetAllAsync())
            .ReturnsAsync(emptyOfficesListMock);

        _mapperMock.Setup(x => x.Map<List<OfficeShortInfoDTO>>(emptyOfficesListMock))
            .Returns(mappedOfficesListMock);

        //Act
        var result = await _sut.GetAllOfficesAsync();

        //Assert
        Assert.IsType<NotFound>(result.Value);
        _officesRepositoryMock.Verify(x => x.GetAllAsync(), Times.Once);
        _mapperMock.Verify(x => x.Map<List<OfficeShortInfoDTO>>(emptyOfficesListMock), Times.Never);
    }

    [Fact]
    public async Task GetOfficesByIdsAsync_RepositoryReturnsCollectionOfOffices_ReturnsMappedListOfOffices()
    {
        //Arrange
        var officesIdsCollection = new Fixture().Create<IEnumerable<string>>();
        var officesListMock = new Fixture().Create<List<Office>>();
        var mappedOfficesListMock = new Fixture().Create<List<OfficeDetailsDTO>>();

        _officesRepositoryMock.Setup(x => x.GetCollectionByIdsAsync(officesIdsCollection))
            .ReturnsAsync(officesListMock);

        _mapperMock.Setup(x => x.Map<List<OfficeDetailsDTO>>(officesListMock))
            .Returns(mappedOfficesListMock);

        //Act
        var result = await _sut.GetOfficesByIdsAsync(officesIdsCollection);

        //Assert
        Assert.IsType<List<OfficeDetailsDTO>>(result.Value);
        _officesRepositoryMock.Verify(x => x.GetCollectionByIdsAsync(officesIdsCollection), Times.Once);
        _mapperMock.Verify(x => x.Map<List<OfficeDetailsDTO>>(officesListMock), Times.Once);
    }

    [Fact]
    public async Task GetOfficesByIdsAsync_RepositoryReturnsNothing_ReturnsNotFoundResult()
    {
        //Arrange
        var officesIdsCollection = new Fixture().Create<IEnumerable<string>>();
        var officesListMock = new List<Office>();
        var mappedOfficesListMock = new Fixture().Create<List<OfficeDetailsDTO>>();

        _officesRepositoryMock.Setup(x => x.GetCollectionByIdsAsync(officesIdsCollection))
            .ReturnsAsync(officesListMock);

        _mapperMock.Setup(x => x.Map<List<OfficeDetailsDTO>>(officesListMock))
            .Returns(mappedOfficesListMock);

        //Act
        var result = await _sut.GetOfficesByIdsAsync(officesIdsCollection);

        //Assert
        Assert.IsType<NotFound>(result.Value);
        _officesRepositoryMock.Verify(x => x.GetCollectionByIdsAsync(officesIdsCollection), Times.Once);
        _mapperMock.Verify(x => x.Map<List<OfficeDetailsDTO>>(officesListMock), Times.Never);
    }

    [Fact]
    public async Task GetOfficeByIdAsync_OfficeWasFound_ReturnsMappedOffice()
    {
        //Arrange
        var fakeOfficeId = new Fixture().Create<string>().Substring(0, 24);
        var officeMock = new Fixture().Create<Office>();
        var mappedOfficeMock = new Fixture().Create<OfficeDetailsDTO>();

        _officesRepositoryMock.Setup(x => x.GetByIdAsync(fakeOfficeId))
           .ReturnsAsync(officeMock);

        _mapperMock.Setup(x => x.Map<OfficeDetailsDTO>(officeMock))
            .Returns(mappedOfficeMock);

        //Act
        var result = await _sut.GetOfficeByIdAsync(fakeOfficeId);

        //Assert
        Assert.IsType<OfficeDetailsDTO>(result.Value);
        _officesRepositoryMock.Verify(x => x.GetByIdAsync(fakeOfficeId), Times.Once);
        _mapperMock.Verify(x => x.Map<OfficeDetailsDTO>(officeMock), Times.Once);
    }

    [Fact]
    public async Task GetOfficeByIdAsync_OfficeWasNotFount_ReturnsNotFound()
    {
        //Arrange
        var fakeOfficeId = new Fixture().Create<string>().Substring(0, 24);

        _officesRepositoryMock.Setup(x => x.GetByIdAsync(fakeOfficeId))
           .ReturnsAsync(It.IsAny<Office>());

        _mapperMock.Setup(x => x.Map<OfficeDetailsDTO>(It.IsAny<Office>()))
            .Returns(It.IsAny<OfficeDetailsDTO>());

        //Act
        var result = await _sut.GetOfficeByIdAsync(fakeOfficeId);

        //Assert
        Assert.IsType<NotFound>(result.Value);
        _officesRepositoryMock.Verify(x => x.GetByIdAsync(fakeOfficeId), Times.Once);
        _mapperMock.Verify(x => x.Map<OfficeDetailsDTO>(It.IsAny<Office>()), Times.Never);
    }

    [Fact]
    public async Task AddNewOfficeAsync_AddValidOffice_ReturnsIdOfCreatedOffice()
    {
        //Arrange
        var fakeOfficeId = new Fixture().Create<string>().Substring(0, 24);
        var newOfficeDTOMock = new Fixture().Create<OfficeCreateDTO>();
        var officeMock = new Fixture().Create<Office>();

        _officesRepositoryMock.Setup(x => x.AddNewAsync(officeMock));

        _mapperMock.Setup(x => x.Map<Office>(newOfficeDTOMock))
            .Returns(officeMock);

        //Act
        var result = await _sut.AddNewOfficeAsync(newOfficeDTOMock);

        //Assert
        Assert.IsType<string>(result);
        _officesRepositoryMock.Verify(x => x.AddNewAsync(officeMock), Times.Once);
        _mapperMock.Verify(x => x.Map<Office>(newOfficeDTOMock), Times.Once);
    }

    [Fact]
    public async Task DeleteOfficeAsync_PassIdOfExistedOffice_SuccessResult()
    {
        //Arrange
        var fakeOfficeId = new Fixture().Create<string>().Substring(0, 24);
        var fakeOffice = new Fixture().Create<Office>();

        _officesRepositoryMock.Setup(x => x.GetByIdAsync(fakeOfficeId))
            .ReturnsAsync(fakeOffice);

        _officesRepositoryMock.Setup(x => x.DeleteAsync(fakeOfficeId));

        //Act
        var result = await _sut.DeleteOfficeAsync(fakeOfficeId);

        //Assert
        Assert.IsType<Success>(result.Value);
        _officesRepositoryMock.Verify(x => x.GetByIdAsync(fakeOfficeId), Times.Once);
        _officesRepositoryMock.Verify(x => x.DeleteAsync(fakeOfficeId), Times.Once);
    }

    [Fact]
    public async Task DeleteOfficeAsync_PassIdOfNotExistedOffice_SuccessResult()
    {
        //Arrange
        var fakeOfficeId = new Fixture().Create<string>().Substring(0, 24);
        var fakeOffice = new Fixture().Create<Office>();

        _officesRepositoryMock.Setup(x => x.GetByIdAsync(fakeOfficeId))
            .ReturnsAsync(It.IsAny<Office>());

        _officesRepositoryMock.Setup(x => x.DeleteAsync(fakeOfficeId));

        //Act
        var result = await _sut.DeleteOfficeAsync(fakeOfficeId);

        //Assert
        Assert.IsType<NotFound>(result.Value);
        _officesRepositoryMock.Verify(x => x.GetByIdAsync(fakeOfficeId), Times.Once);
        _officesRepositoryMock.Verify(x => x.DeleteAsync(fakeOfficeId), Times.Never);
    }

    [Fact]
    public async Task UpdateOfficeAsync_PassIdOfExistedOffice_SuccessResult()
    {
        //Arrange
        var fakeOfficeId = new Fixture().Create<string>().Substring(0, 24);
        var fakeOffice = new Fixture().Create<Office>();
        var fakeOfficeUpdateModel = new Fixture().Create<OfficeUpdateDTO>();

        _officesRepositoryMock.Setup(x => x.GetByIdAsync(fakeOfficeId))
            .ReturnsAsync(fakeOffice);

        _officesRepositoryMock.Setup(x => x.UpdateAsync(fakeOfficeId, fakeOffice));

        _mapperMock.Setup(x => x.Map<Office>(fakeOfficeUpdateModel))
            .Returns(fakeOffice);

        //Act
        var result = await _sut.UpdateOfficeAsync(fakeOfficeId, fakeOfficeUpdateModel);

        //Assert
        Assert.IsType<Success>(result.Value);
        _officesRepositoryMock.Verify(x => x.GetByIdAsync(fakeOfficeId), Times.Once);
        _mapperMock.Verify(x => x.Map<Office>(fakeOfficeUpdateModel), Times.Once);
        _officesRepositoryMock.Verify(x => x.UpdateAsync(fakeOfficeId, fakeOffice), Times.Once);
    }

    [Fact]
    public async Task UpdateOfficeAsync_PassIdOfOfficeThatDoesntExist_NotFoundResult()
    {
        //Arrange
        var fakeOfficeId = new Fixture().Create<string>().Substring(0, 24);
        var fakeOffice = new Fixture().Create<Office>();
        var fakeOfficeUpdateModel = new Fixture().Create<OfficeUpdateDTO>();

        _officesRepositoryMock.Setup(x => x.GetByIdAsync(fakeOfficeId))
            .ReturnsAsync(It.IsAny<Office>());

        _officesRepositoryMock.Setup(x => x.UpdateAsync(fakeOfficeId, fakeOffice));

        _mapperMock.Setup(x => x.Map<Office>(fakeOfficeUpdateModel))
            .Returns(fakeOffice);

        //Act
        var result = await _sut.UpdateOfficeAsync(fakeOfficeId, fakeOfficeUpdateModel);

        //Assert
        Assert.IsType<NotFound>(result.Value);
        _officesRepositoryMock.Verify(x => x.GetByIdAsync(fakeOfficeId), Times.Once);
        _mapperMock.Verify(x => x.Map<Office>(fakeOfficeUpdateModel), Times.Never);
        _officesRepositoryMock.Verify(x => x.UpdateAsync(fakeOfficeId, fakeOffice), Times.Never);
    }
}