namespace BlazingTaskManager.Client.ViewModels
{
    public interface IAccountLoginVM
    {
        void OnSubmit();
    }

    public class AccountLoginVM :IAccountLoginVM
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? LoginError { get; set; } 

        public void OnSubmit()
        {

        }
    }
}
