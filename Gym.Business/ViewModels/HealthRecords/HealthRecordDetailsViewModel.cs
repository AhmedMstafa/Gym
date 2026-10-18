namespace Gymy.BusinessLogic.ViewModels.HealthRecords;

public class HealthRecordDetailsViewModel
{
    public decimal Height { get; set; }

    public decimal Weight { get; set; }


    public string BloodType { get; set; } = default!;

    public string? Note { get; set; } = default!;
}