using System.Windows.Controls;
using Task_manager.ViewsModels.Classes;

namespace Task_manager.Views
{
    public partial class TaskSchedulerView : UserControl
    {
        public TaskSchedulerView()
        {
            InitializeComponent();
            DataContext = new TaskSchedulerViewModel();
        }
    }
}
