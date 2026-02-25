using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Selu383.SP26.Api.Data;

namespace Selu383.SP26.Api.Features.Locations;

[ApiController]
[Route("api/locations")]
public class LocationsController : ControllerBase
{
    private readonly DataContext _dataContext;

    public LocationsController(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    // GET /api/locations
    [HttpGet]
    public async Task<ActionResult<List<LocationDto>>> ListAll()
    {
        var locations = await _dataContext.Locations
            .Select(x => new LocationDto
            {
                Id = x.Id,
                Name = x.Name,
                Address = x.Address,
                TableCount = x.TableCount,
                ManagerId = x.ManagerId
            })
            .ToListAsync();

        return Ok(locations);
    }

    // GET /api/locations/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<LocationDto>> GetById(int id)
    {
        var location = await _dataContext.Locations
            .Where(x => x.Id == id)
            .Select(x => new LocationDto
            {
                Id = x.Id,
                Name = x.Name,
                Address = x.Address,
                TableCount = x.TableCount,
                ManagerId = x.ManagerId
            })
            .FirstOrDefaultAsync();

        if (location is null)
            return NotFound();

        return Ok(location);
    }

    // POST /api/locations (Admins only)
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<LocationDto>> Create(LocationDto dto)
    {
        var validation = await ValidateLocationDto(dto, validateId: false, existingLocation: null);
        if (validation is not null)
            return validation;

        var location = new Location
        {
            Name = dto.Name.Trim(),
            Address = dto.Address.Trim(),
            TableCount = dto.TableCount,
            ManagerId = dto.ManagerId
        };

        _dataContext.Locations.Add(location);
        await _dataContext.SaveChangesAsync();

        var createdDto = new LocationDto
        {
            Id = location.Id,
            Name = location.Name,
            Address = location.Address,
            TableCount = location.TableCount,
            ManagerId = location.ManagerId
        };

        return CreatedAtAction(nameof(GetById), new { id = location.Id }, createdDto);
    }

    // PUT /api/locations/{id}
    [HttpPut("{id:int}")]
    [Authorize]
    public async Task<ActionResult<LocationDto>> Update(int id, LocationDto dto)
    {
        var location = await _dataContext.Locations.FirstOrDefaultAsync(x => x.Id == id);
        if (location is null)
            return NotFound();

        var isAdmin = User.IsInRole("Admin");
        var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "-1");

        // Users may update only if they are the manager; admins always allowed
        if (!isAdmin)
        {
            if (location.ManagerId is null || location.ManagerId.Value != currentUserId)
                return Forbid();

            // Only admins can change ManagerId
            if (dto.ManagerId != location.ManagerId)
                return Forbid();
        }

        var validation = await ValidateLocationDto(dto, validateId: false, existingLocation: location);
        if (validation is not null)
            return validation;

        location.Name = dto.Name.Trim();
        location.Address = dto.Address.Trim();
        location.TableCount = dto.TableCount;

        // Admins can change ManagerId
        if (isAdmin)
            location.ManagerId = dto.ManagerId;

        await _dataContext.SaveChangesAsync();

        var updatedDto = new LocationDto
        {
            Id = location.Id,
            Name = location.Name,
            Address = location.Address,
            TableCount = location.TableCount,
            ManagerId = location.ManagerId
        };

        return Ok(updatedDto);
    }

    // DELETE /api/locations/{id}
    [HttpDelete("{id:int}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var location = await _dataContext.Locations.FirstOrDefaultAsync(x => x.Id == id);
        if (location is null)
            return NotFound();

        var isAdmin = User.IsInRole("Admin");
        var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "-1");

        // Users may delete only if they are the manager; admins always allowed
        if (!isAdmin)
        {
            if (location.ManagerId is null || location.ManagerId.Value != currentUserId)
                return Forbid();
        }

        _dataContext.Locations.Remove(location);
        await _dataContext.SaveChangesAsync();

        return Ok();
    }

    private async Task<ActionResult?> ValidateLocationDto(
        LocationDto dto,
        bool validateId,
        Location? existingLocation)
    {
        if (dto is null)
            return BadRequest();

        if (validateId && dto.Id <= 0)
            return BadRequest();

        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest();

        if (dto.Name.Length > 120)
            return BadRequest();

        if (string.IsNullOrWhiteSpace(dto.Address))
            return BadRequest();

        if (dto.TableCount < 1)
            return BadRequest();

        // ManagerId can be null. If not null, it must be a real user.
        if (dto.ManagerId is not null)
        {
            var managerExists = await _dataContext.Users.AnyAsync(u => u.Id == dto.ManagerId.Value);
            if (!managerExists)
                return BadRequest();
        }

        return null;
    }
}