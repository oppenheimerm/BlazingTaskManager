using BlazingTaskManager.Client.Components.Data;
using BlazingTaskManager.Shared.Domain;

namespace BlazingTaskManager.Client.UI
{
    public static class UIHelpers
    {
        /// <summary>
        /// Get the menu options for the client application.
        /// </summary>
        /// <returns></returns>
        public static List<MenuOption> GetMenuOptions()
        {
            return new List<MenuOption>
            {
                new MenuOption { Id = 1, IconName = "dashboard", Title = "Dashboard" },
                new MenuOption { Id = 2, IconName = "task", Title = "Manage Task"  },
                new MenuOption { Id = 3, IconName = "add_circle", Title = "Create Task"  },
                new MenuOption { Id = 4, IconName = "diversity_3", Title = "Team Members"  },
                new MenuOption { Id = 5, IconName = "logout", Title = "Sign Out"  }
            };
        }

        /// <summary>
        /// Set the colour of the status badge based on the task status.
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public static string GetStatusBadgeColour(BTaskStatus status)
        {
            return status switch
            {
                BTaskStatus.NotStarted => "bg-gray-500",
                BTaskStatus.InProgress => "bg-blue-500",
                BTaskStatus.Completed => "bg-green-500",
                BTaskStatus.OnHold => "bg-yellow-500",
                _ => "bg-gray-500"
            };
        }

        /// <summary>
        /// Set the colour of the priority badge based on the task priority.
        /// </summary>
        /// <param name="priority"></param>
        /// <returns></returns>
        public static string GetPriorityBadgeColour(BPriority priority)
        {
            return priority switch
            {
                BPriority.Low => "bg-green-300",
                BPriority.Medium => "bg-yellow-300",
                BPriority.High => "bg-red-300",
                _ => "bg-gray-300"
            };
        }
    }
}
