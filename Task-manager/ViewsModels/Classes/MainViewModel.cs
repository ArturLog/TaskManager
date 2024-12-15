
using System.ComponentModel;

namespace Task_manager.ViewsModels.Classes
{
    public class MainViewModel : BaseViewModel
    {
        public TaskManagerViewModel TaskManagerViewModel { get; set; }
        public TaskSchedulerViewModel TaskSchedulerViewModel { get; set; }

        public MainViewModel()
        {
            TaskManagerViewModel = new TaskManagerViewModel();
            TaskSchedulerViewModel = new TaskSchedulerViewModel();
        }
    }
}
