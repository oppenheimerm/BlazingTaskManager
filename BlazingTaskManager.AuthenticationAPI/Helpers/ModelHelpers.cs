using BlazingTaskManager.Shared.Domain;
using BlazingTaskManager.Shared.Domain.DTO.Authentication;

namespace BlazingTaskManager.AuthenticationAPI.Helpers
{
    public static class ModelHelpers
    {
        public static BTUser ToEntity(this RegisterRequestDTO dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            else
            {
                return new BTUser()
                {
                    Id = Guid.NewGuid(),
                    FirstName = dto.FirstName,
                    LasttName = dto.LasttName,
                    Email = dto.Email.ToLower(),
                    //PasswordHash
                    //UserPhoto
                    JoinDate = DateTime.UtcNow,
                    Updated = DateTime.UtcNow,
                    AcceptTerms = dto.AcceptTerms,
                };
            }
        }
    }
}
