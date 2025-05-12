using BlazingTaskManager.Shared.Domain;
using BlazingTaskManager.Shared.Domain.DTO.Role;
using BlazingTaskManager.Shared.Domain.DTO.User;

namespace BlazingTaskManager.AuthenticationAPI.Data
{
    public static class ModelHelpers
    {
        public static BTUserDTO ToDto(this BTUser entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            else
            {
                var _user = new BTUserDTO()
                {
                    Id = entity.Id,
                    FirstName = entity.FirstName,
                    LasttName = entity.LasttName,
                    Email = entity.Email,
                    UserPhoto = entity.UserPhoto,
                    //  Roles
                    JoinDate = entity.JoinDate,
                    Updated = entity.Updated,
                    IsVerified = entity.IsVerified,
                    AccountLockedOut = entity.AccountLockedOut
                };

                List<RoleDTO>? _userRoles;

                if (entity.Roles is not null)
                {
                    if (entity.Roles.Count >= 1)
                    {
                        _userRoles = entity.Roles!.Select(_ => new RoleDTO()
                        {
                            RoleCode = _.RoleCode,
                            RoleName = _.RoleName,
                            Description = _.Description
                        }).ToList();
                        _user.Roles = _userRoles;
                    }
                }

                return _user;
            }
        }
    }
}
