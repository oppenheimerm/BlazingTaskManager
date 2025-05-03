using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace BlazingTaskManager.Shared.Domain
{
    /// <summary>
    /// Represents a user in the Blazing Task Manager application.
    /// </summary>
    public class BTUser
    {
        public BTUser()
        {
            this.Id = Guid.NewGuid();
            this.JoinDate = DateTime.UtcNow;
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public Guid Id { get; set; }

        [Required]
        [StringLength(30)]
        [PersonalData]
        public string? FirstName { get; set; }

        [Required]
        [StringLength(30)]
        [PersonalData]
        public string? LasttName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string? Email { get; set; }
        public string? PasswordHash { get; set; }

        [MaxLength(100, ErrorMessage = "Filename has a maximum of 100 characters.")]
        public string? UserPhoto { get; set; }

        public DateTime? JoinDate { get; set; } = DateTime.Now;

        public DateTime? Updated { get; set; } = DateTime.Now;

        public List<Role>? Roles { get; set; }

        public string? VerificationToken { get; set; }

        public DateTime? Verified { get; set; }

        public bool IsVerified => Verified.HasValue;

        public bool AccountLockedOut { get; set; } = false;

        public string? ResetToken { get; set; }
        public DateTime? ResetTokenExpires { get; set; }
        public DateTime? PasswordReset { get; set; }
        public List<RefreshToken>? RefreshTokens { get; set; }

        [MaxLength(500, ErrorMessage = "Bio has a maximum size 500 characters.")]
        public string? Bio { get; set; }

        [Range(typeof(bool), "true", "true")]
        public bool AcceptTerms { get; set; }

        public bool OwnsToken(string token)
        {
            return this.RefreshTokens?.Find(x => x.Token == token) != null;
        }
    }
}
