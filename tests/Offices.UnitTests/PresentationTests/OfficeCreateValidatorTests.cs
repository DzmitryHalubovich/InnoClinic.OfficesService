using AutoFixture;
using FluentValidation.TestHelper;
using Offices.Contracts.DTOs;
using Offices.Contracts.Enums;
using Offices.Presentation.Validators;
using Offices.UnitTests.FixtureCustomization;

namespace Offices.UnitTests.Presentation;

public class OfficeCreateValidatorTests
{
    private const string StringWith101Symbols = "SM0t9vW61PepITq61TS4GPMsIpMpzR12FxSgT1StDsAzGVR2tBHOCwSdaDebzUnib7QeMRp2W4gTTgNwVhDC3nwpCR521OQuIXgx1";
    private const string StringWith21Symbols = "67uLGP6pBcUm6kOro4CYX";

    private readonly OfficeCreateValidator _sut;
    private readonly Fixture _fixture;

    public OfficeCreateValidatorTests()
    {
        _sut = new OfficeCreateValidator();
        _fixture = new Fixture();
        _fixture.Customize(new OfficeCreateDtoCustomization());
        
    }

    [Fact]
    public void CreateNewOffice_PassValidData_SuccessValidation()
    {
        //Arrange
        var validOfficeDto = _fixture.Create<OfficeCreateDTO>();

        //Act
        var result = _sut.TestValidate(validOfficeDto);

        //Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("", false)]
    [InlineData(null, true)]
    public void CreateNewOffice_TryPhotoIdValidation_FailValidationIfPhotoIdIsInvalid(string? photoId,
        bool expectedValidationResult)
    {
        //Arrange
        var fakeOfficeModel = new OfficeCreateDTO(
                PhotoId: photoId,
                City: "Test city",
                Street: "Test street",
                HouseNumber: "11",
                OfficeNumber: "22",
                RegistryPhoneNumber: "+375112223344",
                IsActive: Status.Active);

        //Act
        var result = _sut.TestValidate(fakeOfficeModel);

        //Assert
        Assert.Equal(expectedValidationResult, result.IsValid);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData(StringWith101Symbols)]
    public void CreateNewOffice_TryPassInvalidCity_FailValidationForCity(string city)
    {
        //Arrange
        var fakeOfficeModel = new OfficeCreateDTO(
               PhotoId: "Test photo id",
               City: city,
               Street: "Test street",
               HouseNumber: "11",
               OfficeNumber: "22",
               RegistryPhoneNumber: "+375112223344",
               IsActive: Status.Active);

        //Act
        var result = _sut.TestValidate(fakeOfficeModel);

        //Assert
        result.ShouldHaveValidationErrorFor(x => x.City);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData(StringWith101Symbols)]
    public void CreateNewOffice_TryPassInvalidStreet_FailValidationForStreet(string street)
    {
        //Arrange
        var fakeOfficeCreateModel = new OfficeCreateDTO(
               PhotoId: "Test photo id",
               City: "Test city",
               Street: street,
               HouseNumber: "11",
               OfficeNumber: "22",
               RegistryPhoneNumber: "+375112223344",
               IsActive: Status.Active);

        //Act
        var result = _sut.TestValidate(fakeOfficeCreateModel);

        //Assert
        result.ShouldHaveValidationErrorFor(x => x.Street);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData(StringWith21Symbols)]
    public void CreateNewOffice_TryPassInvalidHouseNumber_FailValidationForHouseNumber(string houseNumber)
    {
        //Arrange
        var fakeOfficeCreateModel = new OfficeCreateDTO(
               PhotoId: "Test photo id",
               City: "Test city",
               Street: "Test street",
               HouseNumber: houseNumber,
               OfficeNumber: "22",
               RegistryPhoneNumber: "+375112223344",
               IsActive: Status.Active);

        //Act
        var result = _sut.TestValidate(fakeOfficeCreateModel);

        //Assert
        result.ShouldHaveValidationErrorFor(x => x.HouseNumber);
    }

    [Theory]
    [InlineData("", false)]
    [InlineData(null, true)]
    [InlineData(StringWith21Symbols, false)]
    public void CreateNewOffice_PassingOfficeNumber_FailIfOfficeNumberIsInvalid(string officeNumber, bool expectedValidationResult)
    {
        //Arrange
        var fakeOfficeCreateModel = new OfficeCreateDTO(
               PhotoId: "Test photo id",
               City: "Test city",
               Street: "Test street",
               HouseNumber: "11",
               OfficeNumber: officeNumber,
               RegistryPhoneNumber: "+375112223344",
               IsActive: Status.Active);

        //Act
        var result = _sut.TestValidate(fakeOfficeCreateModel);

        //Assert
        Assert.Equal(expectedValidationResult, result.IsValid);
    }

    [Theory]
    [InlineData("+375147895412", true)]
    [InlineData("+37514261", false)]
    [InlineData("abc-123-4567", false)]
    [InlineData("123 456 7890", true)]
    public void CreateNewOffice_PassingOffcieRegistryPhoneNumber_FailIfOffcieRegistryPhoneNumberIsInvalid(string registryPhoneNumber, 
        bool expectedValidationResult)
    {
        //Arrange
        var fakeOfficeCreateModel = new OfficeCreateDTO(
               PhotoId: "Test photo id",
               City: "Test city",
               Street: "Test street",
               HouseNumber: "11",
               OfficeNumber: "22",
               RegistryPhoneNumber: registryPhoneNumber,
               IsActive: Status.Active);

        //Act
        var result = _sut.TestValidate(fakeOfficeCreateModel);

        //Assert
        Assert.Equal(expectedValidationResult, result.IsValid);
    }
}
