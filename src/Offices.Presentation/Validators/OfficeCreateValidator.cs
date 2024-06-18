﻿using FluentValidation;
using Offices.Contracts.DTOs;
using Offices.Contracts.Enums;

namespace Offices.Presentation.Validators;

public class OfficeCreateValidator : AbstractValidator<OfficeCreateDTO>
{
    public OfficeCreateValidator()
    {
        RuleFor(o => o.IsActive)
            .Must((status) => (status == Status.Active || status == Status.Inactive));
        RuleFor(o => o.City)
            .NotEmpty()
            .MaximumLength(100);
        RuleFor(o => o.Street)
            .NotEmpty()
            .MaximumLength(100);
        RuleFor(o => o.HouseNumber)
            .NotEmpty()
            .MaximumLength(50);
        RuleFor(o => o.OfficeNumber)
            .MaximumLength(50);
        RuleFor(o => o.RegistryPhoneNumber)
            .Matches("^[\\+]?[(]?[0-9]{3}[)]?[-\\s\\.]?[0-9]{3}[-\\s\\.]?[0-9]{4,6}$")
            .WithMessage("Wrong phone format.")
            .NotEmpty().WithMessage("Phone number field can not be empty.");
    }
}
