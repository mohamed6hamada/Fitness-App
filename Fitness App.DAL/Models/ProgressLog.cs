using Fitness_App.DAL.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class ProgressLog
{
    [Key]
    public int ProgressLogId { get; set; }
    [Required]
    public DateTime Date { get; set; }
    [Required]
    [Range(0, 500, ErrorMessage = "Please enter a valid weight")]
    public double Weight { get; set; }
    [Range(0, 500, ErrorMessage = "Please enter a valid goal weight")] // Updated to 500
    public double GoalWeight { get; set; }

    // Body measurements
    [Range(0, 300)]
    public double Chest { get; set; }
    [Range(0, 300)]
    public double Waist { get; set; }
    [Range(0, 300)]
    public double Arms { get; set; }
    [Range(0, 300)]
    public double Thighs { get; set; }
    [MaxLength(3000)]
    public string Notes { get; set; }
    
    // Calorie calculation
    [MaxLength(50)]
    public string ActivityLevel { get; set; } = "Moderate";
    public int? DailyCalorieNeeds { get; set; }

    // Foreign key to ApplicationUser
    public string UserId { get; set; }

    [ForeignKey("UserId")]
    public virtual ApplicationUser User { get; set; }
}