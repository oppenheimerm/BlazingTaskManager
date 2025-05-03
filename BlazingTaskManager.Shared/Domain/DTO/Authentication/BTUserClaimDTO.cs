
using BlazingTaskManager.Shared.Domain.DTO.Role;

namespace BlazingTaskManager.Shared.Domain.DTO.Authentication
{
    /// <summary>
    /// Represents the claims of a user in the Blazing Task Manager application.
    /// </summary>
    public class BTUserClaimDTO
    {
        public Guid? Id { get; set; }
        public string? FirstName { get; set; }
        public string? Email{ get; set; }
        public List<RoleDTO>? Roles { get; set; }
    }
}
