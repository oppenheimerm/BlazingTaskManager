
using BlazingTaskManager.Shared.Domain.DTO.Role;

namespace BlazingTaskManager.Shared.Domain.DTO.User
{
    /// <summary>
    /// A <see cref="BTUser"/> Claim with <see cref="BlazingTaskManager.Shared.Domain.Role"/> ('s) collection.
    /// </summary>
    public class BTUserClaimsDTO
    {
        public Guid? Id { get; set; }
        public string? FirstName { get; set; }
        public string? Email { get; set; }
        public List<RoleDTO>? Roles { get; set; }
    }
}
