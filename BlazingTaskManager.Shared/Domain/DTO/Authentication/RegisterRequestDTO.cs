using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BlazingTaskManager.Shared.Domain.DTO.Authentication
{
    public class RegisterRequestDTO
    {

        [Required]
        [StringLength(30)]
        [MinLength(3, ErrorMessage = "First name is reqired.")]
        [PersonalData]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(30)]
        [MinLength(3, ErrorMessage = "Last name is reqired.")]
        [PersonalData]
        public string LasttName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = "";

        [Required]
        [MinLength(6)]
        public string? Password { get; set; }

        [Required]
        [Compare("Password")]
        public string? ConfirmPassword { get; set; }

        [Range(typeof(bool), "true", "true")]
        public bool AcceptTerms { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is RegisterRequestDTO dTO &&
                   FirstName == dTO.FirstName &&
                   LasttName == dTO.LasttName &&
                   Email == dTO.Email;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(FirstName, LasttName, Email);
        }
    }
}
