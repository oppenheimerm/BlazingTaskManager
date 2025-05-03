using System.Text.RegularExpressions;

namespace BlazingTaskManager.AuthenticationAPI.Helpers
{
    public static class AccountHelpers
    {
        /// <summary>
        /// Validates the password based on the specified criteria.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="minPasswordLength"></param>
        /// <returns></returns>
        public static bool ValidatePassword(string text, int minPasswordLength)
        {
            // Check if the length of the string is >= minpasswordlength
            // Check if the string contains at least one uppercase letter, one lowercase letter, one digit, and one special character
            // Ensure that the string doesn't contain any character that doesn't belong to the allowed set
            bool result = text.Length >= minPasswordLength
            && Regex.IsMatch(text, "[A-Z]")
            && Regex.IsMatch(text, "[a-z]")
            && Regex.IsMatch(text, @"\d")
            && Regex.IsMatch(text, @"[!-/:-@\[-_{-~]")
            && !Regex.IsMatch(text, @"[^\dA-Za-z!-/:-@\[-_{-~]");

            // Return the result indicating whether the string is a valid password
            return result;
        }
    }
}
