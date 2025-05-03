
using BlazingTaskManager.Shared.Domain.DTO.Role;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BlazingTaskManager.Shared.Domain.DTO.User
{
    /// <summary>
    /// Authenticated user response.  ***Does not include authentication / password info.
    /// </summary>
    public class BTUserDTO
    {
        public Guid Id { get; set; }
        public string? FirstName { get; set; }
        public string? LasttName { get; set; }
        public string? Email { get; set; }
        public string? UserPhoto { get; set; }
        public List<RoleDTO>? Roles { get; set; }
        public DateTime? JoinDate { get; set; }
        public DateTime? Updated { get; set; }
        public bool IsVerified { get; set; }
        public bool AccountLockedOut { get; set; }
        public string? JwtToken { get; set; }

        [JsonIgnore] // refresh token is returned in http only cookie
        public string? RefreshToken { get; set; }
    }
}
