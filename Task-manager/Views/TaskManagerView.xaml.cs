using System.Windows.Controls;
using Task_manager.ViewsModels.Classes;

namespace Task_manager.Views
{
    /// <summary>
    /// Interaction logic for TaskManagerView.xaml
    /// </summary>
    public partial class TaskManagerView : UserControl
    {
        public TaskManagerView()
        {
            InitializeComponent();
            DataContext = new TaskManagerViewModel();
        }
    }
}
