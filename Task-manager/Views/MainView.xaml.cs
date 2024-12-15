using System.Windows;
using Task_manager.ViewsModels.Classes;

namespace Task_manager.Views
{
    public partial class MainView : Window
    {
        public MainView()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }
    }
}