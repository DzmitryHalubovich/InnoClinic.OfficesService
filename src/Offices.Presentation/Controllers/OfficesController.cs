using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Offices.Contracts.DTOs;
using Offices.Presentation.ModelBinders;
using Offices.Services.Abstractions;
using Serilog;
using System.Net.Mime;

namespace Offices.Presentation.Controllers;

[ApiController]
[Route("api/offices")]
public class OfficesController : ControllerBase
{
    private readonly IOfficesService _officesService;
    private readonly IRedisCahceService _cacheService;

    public OfficesController(IOfficesService officesService, IRedisCahceService cahceService)
    {
        _officesService = officesService;
        _cacheService = cahceService;
    }
        
    [AllowAnonymous]
    [HttpGet]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<OfficeDetailsDTO>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAllOffices()
    {
        var instanceId = GetInstanceId();
        var cacheKey = $"Offices_Cache_{instanceId}";

        var offices = _cacheService.GetCachedData<List<OfficeDetailsDTO>>(cacheKey);

        if (offices is null)
        {
            var getAllOfficesResult = await _officesService.GetAllOfficesAsync();

            _cacheService.SetCachedData(cacheKey, getAllOfficesResult.Value, TimeSpan.FromMinutes(5));

            return getAllOfficesResult.Match<IActionResult>(Ok, notFound => NotFound());
        }

        Log.Information($"Return data from cache, type: {offices.First().GetType()}, count: {offices.Count}");

        return Ok(offices);
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

    [AllowAnonymous]
    [HttpPost]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddOffice(IValidator<OfficeCreateDTO> validator, [FromBody] OfficeCreateDTO newOffice)
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


    private string GetInstanceId()
    {
        var instanceId = HttpContext.Session.GetString("InstanceId");

        if (string.IsNullOrEmpty(instanceId))
        {
            instanceId = Guid.NewGuid().ToString();

            HttpContext.Session.SetString("InstanceId", instanceId);
        }

        return instanceId;
    }
}