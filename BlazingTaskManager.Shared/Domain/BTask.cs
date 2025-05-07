
using System.ComponentModel.DataAnnotations;

namespace BlazingTaskManager.Shared.Domain
{
    public enum BTaskStatus
    {
        NotStarted,
        InProgress,
        Completed,
        OnHold
    }

    public enum BPriority
    {
        High = 1,
        Medium = 2,
        Low = 3
    }

    /// <summary>
    /// Represents a task in the task manager application. Using the prefix 'B' to avoid name 
    /// collision with other Task classes.
    /// </summary>
    public class BTask
    {
        public BTask()
        {
            CreatedAt = DateTime.Now;
        }
        [Required]
        public int? Id { get; set; }
        [Required]
        [MaxLength(20, ErrorMessage = "The title field has a max length of 25 characters.")]
        public string? Title { get; set; } = string.Empty;
        [MaxLength(20, ErrorMessage = "Description has a maxium of 200 characters.")]
        public string? Description { get; set; } = string.Empty;
        public DateTime? CreatedAt { get; set; }
        [Required]
        public DateTime? DueDate { get; set; }
        public BTaskStatus TaskStatus { get; set; } = BTaskStatus.NotStarted;

        [Required]
        public BPriority Priority { get; set; } = BPriority.Low;

        [Required]
        public Guid? UserId { get; set; }
    }
}
