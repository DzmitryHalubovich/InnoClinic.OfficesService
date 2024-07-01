using AutoFixture;
using Offices.Contracts.DTOs;
using Offices.Contracts.Enums;

namespace Offices.UnitTests.FixtureCustomization;

public class OfficeCreateDtoCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Customize<OfficeCreateDTO>(composer => composer
            .With(dto => dto.PhotoId, fixture.Create<string>())
            .With(dto => dto.City, TruncateString(fixture.Create<string>(), 100))
            .With(dto => dto.Street, TruncateString(fixture.Create<string>(), 100))
            .With(dto => dto.HouseNumber, TruncateString(fixture.Create<string>(), 20))
            .With(dto => dto.OfficeNumber, TruncateString(null, 20))
            .With(dto => dto.RegistryPhoneNumber, "+375211112233")
            .With(dto => dto.IsActive, (Status)0));
    }

    private string TruncateString(string input, int maxLength)
    {
        if (input is null)
        {
            return null;
        }

        return input.Length <= maxLength ? input : input.Substring(0, maxLength);
    }
}
