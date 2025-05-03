using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BlazingTaskManager.Shared.Domain
{
    /// <summary>
    /// Represents a role in the Blazing Task Manager application.
    /// </summary>
    public class Role
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [MaxLength(4, ErrorMessage = "Role code must be 4 characters long"), MinLength(4)]
        public string? RoleCode { get; set; }

        [Required]
        public string? RoleName { get; set; }

        [Required]
        [StringLength(50)]
        public string? Description { get; set; }
    }
}
