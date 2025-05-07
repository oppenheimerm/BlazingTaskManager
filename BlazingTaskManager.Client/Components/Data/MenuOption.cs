using System.ComponentModel.DataAnnotations;

namespace BlazingTaskManager.Client.Components.Data
{
    public class MenuOption
    {
        [Required]
        public int? Id { get; set; }
        [Required]
        public string? Title { get; set; }
        [Required]
        public string? IconName { get; set; }
        public bool AdminOnly { get; set; } = false;
    }
}
