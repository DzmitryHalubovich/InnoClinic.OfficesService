using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Offices.Contracts.DTOs;
using Offices.Presentation.ModelBinders;
using Offices.Services.Abstractions;
using System.Net.Mime;

namespace Offices.Presentation.Controllers;

[ApiController]
[Route("api/offices")]
public class OfficesController : ControllerBase
{
    private readonly IOfficesService _officesService;

    public OfficesController(IOfficesService officesService) =>
        _officesService = officesService;

    [AllowAnonymous]
    [HttpGet]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<OfficeDetailsDTO>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAllOffices()
    {
        var getAllOfficesResult = await _officesService.GetAllOfficesAsync();

        return getAllOfficesResult.Match<IActionResult>(Ok, notFound => NotFound());
    }

    [HttpGet("collection/({officesIds})")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OfficeDetailsDTO))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOfficesByIds(
        [ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<string> officesIds)
    {
        var getOfficesResult = await _officesService.GetOfficesByIdsAsync(officesIds);

        return getOfficesResult.Match<IActionResult>(Ok, notFound => NotFound());
    }

    [HttpGet("{officeId}", Name = "GetOfficeById")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OfficeDetailsDTO))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOfficeById([FromRoute] string officeId)
    {
        var getOfficeResult = await _officesService.GetOfficeByIdAsync(officeId);

        return getOfficeResult.Match<IActionResult>(Ok, notFound => NotFound());
    }

    [HttpPost]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddOffice(IValidator<OfficeCreateDTO> validator, [FromForm] OfficeCreateDTO newOffice)
    {
        var validationResult = validator.Validate(newOffice);

        if (validationResult.IsValid)
        {
            var createdOfficeId = await _officesService.AddNewOfficeAsync(newOffice);

            return CreatedAtRoute("GetOfficeById", new { officeId = createdOfficeId }, createdOfficeId);
        }

        return BadRequest(validationResult.ToDictionary());
    }

    [HttpDelete("{officeId}")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteOfficeById([FromRoute] string officeId)
    {
        var deleteOfficeResult = await _officesService.DeleteOfficeAsync(officeId);

        return deleteOfficeResult.Match<IActionResult>(success => NoContent(), notFound => NotFound());
    }

    [HttpPut("{officeId}")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateOfficeById(IValidator<OfficeUpdateDTO> validator,
        [FromBody] OfficeUpdateDTO editedOffice, [FromRoute] string officeId)
    {
        var validationResult = validator.Validate(editedOffice);

        if (validationResult.IsValid)
        {
            var updateOfficeResult = await _officesService.UpdateOfficeAsync(officeId, editedOffice);

            return updateOfficeResult.Match<IActionResult>(success => NoContent(), notFound => NotFound());
        }

        return BadRequest(validationResult.ToDictionary());
    }
}