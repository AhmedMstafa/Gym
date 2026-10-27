using System.ComponentModel.DataAnnotations;

namespace Gym.Business.ViewModels.Sessions;

public class EditSessionViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Category is required")]
    [Display(Name = "Category")]
    [Range(1, int.MaxValue, ErrorMessage = "Please select a category")]
    public int CategoryId { get; set; }

    [Required(ErrorMessage = "Trainer is required")]
    [Display(Name = "Trainer")]
    [Range(1, int.MaxValue, ErrorMessage = "Please select a trainer")]
    public int TrainerId { get; set; }

    [Required(ErrorMessage = "Description is required")]
    [StringLength(500, MinimumLength = 3, ErrorMessage = "Description must be between 3 and 500 characters")]
    [Display(Name = "Description")]
    public string Description { get; set; } = default!;

    [Required(ErrorMessage = "Capacity is required")]
    [Range(1, 25, ErrorMessage = "Capacity must be between 1 and 25")]
    [Display(Name = "Capacity")]
    public int Capacity { get; set; }

    [Required(ErrorMessage = "Start Date & Time is required")]
    [Display(Name = "Start Time")]
    public DateTime StartDate { get; set; }

    [Required(ErrorMessage = "End Date & Time is required")]
    [Display(Name = "End Time")]
    public DateTime EndDate { get; set; }
}
