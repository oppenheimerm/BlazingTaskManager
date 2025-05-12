namespace BlazingTaskManager.Client.Services
{
    public interface IPageUIService
    {
        event Action? OnChange;
        bool MobileWindowOpen { get; set; }

        /// <summary>
        /// Handle mobile menu state toggle(bool).
        /// </summary>
        void OnClickToggleMobileMenu();
    }

    /// <summary>
    /// Manages the UI state of the page.
    /// </summary>
    public class PageUIService : IPageUIService
    {
        public event Action? OnChange;
        public bool MobileWindowOpen { get; set; } = false;

        /// <summary>
        /// Handle mobile menu state toggle(bool).
        /// </summary>
        public void OnClickToggleMobileMenu()
        {
            if (MobileWindowOpen)
            {
                MobileWindowOpen = false;
                OnChange?.Invoke();
            }
            else
            {
                MobileWindowOpen = true;
                OnChange?.Invoke();
            }
        }
    }

}
