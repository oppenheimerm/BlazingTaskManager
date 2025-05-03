
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BlazingTaskManager.Shared.Domain
{
    /// <summary>
    /// Instance of a user in specific roles in the Blazing Task Manager application.
    /// </summary>
    public class UserRole
    {
        [Key]
        public int? Id { get; set; }

        [Required]
        [ForeignKey(nameof(Role))]
        public string? RoleCode { get; set; }

        public Role? Role { get; set; }

        [Required]
        [ForeignKey(nameof(User))]
        public Guid? UserId { get; set; }

        public BTUser? User { get; set; }

        [Required]
        public DateTime? AddedDate { get; set; }
    }
}
