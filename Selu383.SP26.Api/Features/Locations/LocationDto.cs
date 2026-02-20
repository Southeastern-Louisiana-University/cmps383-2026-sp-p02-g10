using System.ComponentModel.DataAnnotations;

namespace Selu383.SP26.Api.Features.Locations;
public class LocationDto
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Address { get; set; } = string.Empty;

    [Required]
    public int TableCount { get; set; }

    public int ManagerId { get; set; }

}