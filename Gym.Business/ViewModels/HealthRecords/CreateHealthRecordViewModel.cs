using Gym.DataAccess.Enums;
using System.ComponentModel.DataAnnotations;

namespace Gymy.BusinessLogic.ViewModels.HealthRecords;

public class CreateHealthRecordViewModel
{
    [Range(0.1, 300, ErrorMessage = "Height must be greater than 0")]
    public decimal Height { get; set; }

    [Range(0.1, 500, ErrorMessage = "Weight must be greater than 0")]
    public decimal Weight { get; set; }

    [Required(ErrorMessage = "Blood Type Is Required")]
    [EnumDataType(typeof(BloodType), ErrorMessage = "Invalid blood type")]

    public BloodType BloodType { get; set; } 

    public string? Note { get; set; } = default!;
}