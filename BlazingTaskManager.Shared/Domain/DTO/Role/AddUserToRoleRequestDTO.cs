
using System.ComponentModel.DataAnnotations;

namespace BlazingTaskManager.Shared.Domain.DTO.Role
{
    public class AddUserToRoleRequestDTO
    {
        [Required]
        public string? RoleCode { get; set; }

        [Required]
        public Guid? UserId { get; set; }
    }
}
