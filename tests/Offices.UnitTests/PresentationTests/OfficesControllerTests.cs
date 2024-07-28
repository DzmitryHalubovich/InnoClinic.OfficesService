using AutoFixture;
using AutoFixture.Xunit2;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Offices.Contracts.DTOs;
using Offices.Presentation.Controllers;
using Offices.Services.Abstractions;
using OneOf.Types;

namespace Offices.UnitTests.PresentationTests;

public class OfficesControllerTests
{
    private readonly Mock<IOfficesService> _officesServiceMock;
    private readonly Mock<IValidator<OfficeCreateDTO>> _officeCreateValidator;
    private readonly Mock<IValidator<OfficeUpdateDTO>> _officeUpdateValidator;
    private readonly OfficesController _officesController;

    public OfficesControllerTests()
    {
        _officesServiceMock = new Mock<IOfficesService>();
        _officeCreateValidator = new Mock<IValidator<OfficeCreateDTO>>();
        _officeUpdateValidator = new Mock<IValidator<OfficeUpdateDTO>>();
        _officesController = new OfficesController(_officesServiceMock.Object);
    }

    [Fact]
    public async Task GetAllOffices_GetOfficesWhenServiceReturnsListOfOffices_OkResult()
    {
        //Arrange
        _officesServiceMock.Setup(x => x.GetAllOfficesAsync())
            .ReturnsAsync(It.IsAny<List<OfficeDetailsDTO>>());

        //Act
        var result = await _officesController.GetAllOffices();

        //Assert
        Assert.IsType<OkObjectResult>(result);
        _officesServiceMock.Verify(x => x.GetAllOfficesAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllOffices_GetOfficesWhenServiceReturnsNothing_NotFoundResult()
    {
        //Arrange
        _officesServiceMock.Setup(x => x.GetAllOfficesAsync())
            .ReturnsAsync(new NotFound());

        //Act
        var result = await _officesController.GetAllOffices();

        //Assert
        Assert.IsType<NotFoundResult>(result);
        _officesServiceMock.Verify(x => x.GetAllOfficesAsync(), Times.Once);
    }

    [Theory, AutoData]
    public async Task GetOfficeById_GetOfficeWhenItExists_OkResult(string fakeOfficeId)
    {
        //Arrange 
        _officesServiceMock.Setup(x => x.GetOfficeByIdAsync(fakeOfficeId))
            .ReturnsAsync(new Fixture().Create<OfficeDetailsDTO>());

        //Act
        var result = await _officesController.GetOfficeById(fakeOfficeId);

        //Assert
        Assert.IsType<OkObjectResult>(result);
        _officesServiceMock.Verify(x => x.GetOfficeByIdAsync(fakeOfficeId),Times.Once);
    }

    [Theory, AutoData]
    public async Task GetOfficeById_GetOfficeWhenItDoesntExist_NotFoundResult(string fakeOfficeId)
    {
        //Arrange 
        _officesServiceMock.Setup(x => x.GetOfficeByIdAsync(fakeOfficeId))
            .ReturnsAsync(new NotFound());

        //Act
        var result = await _officesController.GetOfficeById(fakeOfficeId);

        //Assert
        Assert.IsType<NotFoundResult>(result);
        _officesServiceMock.Verify(x => x.GetOfficeByIdAsync(fakeOfficeId), Times.Once);
    }

    [Theory, AutoData]
    public async Task AddNewOffice_TryAddValidOffice_CreatedResult(OfficeCreateDTO fakeOfficeModel)
    {
        //Arrange
        _officesServiceMock.Setup(x => x.AddNewOfficeAsync(fakeOfficeModel))
            .ReturnsAsync("newOfficeId");
        _officeCreateValidator.Setup(x => x.Validate(fakeOfficeModel))
            .Returns(new ValidationResult());

        //Act
        var result = await _officesController.AddOffice(_officeCreateValidator.Object, fakeOfficeModel);

        //Assert
        Assert.IsType<CreatedAtRouteResult>(result);
        _officesServiceMock.Verify(x => x.AddNewOfficeAsync(fakeOfficeModel), Times.Once);
        _officeCreateValidator.Verify(x => x.Validate(fakeOfficeModel), Times.Once);
    }

    [Theory, AutoData]
    public async Task AddNewOffice_TryAddInvalidOffice_BadRequest(OfficeCreateDTO newOfficeDto)
    {
        //Arrange
        _officesServiceMock.Setup(x => x.AddNewOfficeAsync(newOfficeDto))
            .ReturnsAsync("newOfficeId");
        _officeCreateValidator.Setup(x => x.Validate(newOfficeDto))
            .Returns(new ValidationResult() { Errors = { new Fixture().Create<ValidationFailure>() } });

        //Act
        var result = await _officesController.AddOffice(_officeCreateValidator.Object, newOfficeDto);

        //Assert
        Assert.IsType<BadRequestObjectResult>(result);
        _officesServiceMock.Verify(x => x.AddNewOfficeAsync(newOfficeDto), Times.Never);
        _officeCreateValidator.Verify(x => x.Validate(newOfficeDto), Times.Once);
    }

    [Theory, AutoData]
    public async Task DeleteOffice_TryDeleteExistedOffice_NoContentResult(string fakeOfficeId)
    {
        //Arrange 
        _officesServiceMock.Setup(x => x.DeleteOfficeAsync(fakeOfficeId))
            .ReturnsAsync(new Success());

        //Act
        var result = await _officesController.DeleteOfficeById(fakeOfficeId);

        //Assert
        Assert.IsType<NoContentResult>(result);
        _officesServiceMock.Verify(x => x.DeleteOfficeAsync(fakeOfficeId), Times.Once);
    }

    [Theory, AutoData]
    public async Task DeleteOffice_TryDeleteOfficeThatDoesntExist_NotFoundResult(string fakeOfficeId)
    {
        //Arrange
        _officesServiceMock.Setup(x => x.DeleteOfficeAsync(fakeOfficeId))
            .ReturnsAsync(new NotFound());

        //Act
        var result = await _officesController.DeleteOfficeById(fakeOfficeId);

        //Assert
        Assert.IsType<NotFoundResult>(result);
        _officesServiceMock.Verify(x => x.DeleteOfficeAsync(fakeOfficeId), Times.Once);
    }

    [Theory, AutoData]
    public async Task UpdateOffice_TryUpdateOfficeThatExistsWithValidData_NoContentResult(OfficeUpdateDTO fakeOfficeModel, string fakeOfficeId)
    {
        //Arrange
        _officesServiceMock.Setup(x => x.UpdateOfficeAsync(fakeOfficeId,  fakeOfficeModel))
            .ReturnsAsync(new Success());
        _officeUpdateValidator.Setup(x => x.Validate(fakeOfficeModel))
            .Returns(new ValidationResult());

        //Act
        var result = await _officesController.UpdateOfficeById(_officeUpdateValidator.Object, fakeOfficeModel, fakeOfficeId);

        //Assert
        Assert.IsType<NoContentResult>(result);
        _officeUpdateValidator.Verify(x => x.Validate(fakeOfficeModel), Times.Once);
        _officesServiceMock.Verify(x => x.UpdateOfficeAsync(fakeOfficeId, fakeOfficeModel), Times.Once);
    }

    [Theory, AutoData]
    public async Task UpdateOffice_TryUpdateOfficeThatExistsWithInvalidData_BadRequestResult(OfficeUpdateDTO fakeOfficeModel, string fakeOfficeId)
    {
        //Arrange
        _officesServiceMock.Setup(x => x.UpdateOfficeAsync(fakeOfficeId, fakeOfficeModel))
            .ReturnsAsync(new Success());
        _officeUpdateValidator.Setup(x => x.Validate(fakeOfficeModel))
            .Returns(new ValidationResult() { Errors = { new Fixture().Create<ValidationFailure>() } });

        //Act
        var result = await _officesController.UpdateOfficeById(_officeUpdateValidator.Object, fakeOfficeModel, fakeOfficeId);

        //Assert
        Assert.IsType<BadRequestObjectResult>(result);
        _officeUpdateValidator.Verify(x => x.Validate(fakeOfficeModel), Times.Once);
        _officesServiceMock.Verify(x => x.UpdateOfficeAsync(fakeOfficeId, fakeOfficeModel), Times.Never);
    }

    [Theory, AutoData]
    public async Task UpdateOffice_TryUpdateOfficeThatDoesntExist_NotFoundResult(OfficeUpdateDTO fakeOfficeModel, string fakeOfficeId)
    {
        //Arrange
        _officesServiceMock.Setup(x => x.UpdateOfficeAsync(fakeOfficeId, fakeOfficeModel))
            .ReturnsAsync(new NotFound());
        _officeUpdateValidator.Setup(x => x.Validate(fakeOfficeModel))
            .Returns(new ValidationResult());

        //Act
        var result = await _officesController.UpdateOfficeById(_officeUpdateValidator.Object, fakeOfficeModel, fakeOfficeId);

        //Assert
        Assert.IsType<NotFoundResult>(result);
        _officesServiceMock.Verify(x => x.UpdateOfficeAsync(fakeOfficeId, fakeOfficeModel), Times.Once);
        _officeUpdateValidator.Verify(x => x.Validate(fakeOfficeModel), Times.Once);
    }
}
