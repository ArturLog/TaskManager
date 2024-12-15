using System.ComponentModel;
using System.Runtime.CompilerServices;
using Task_manager.ViewsModels.Interfaces;

namespace Task_manager.ViewsModels.Classes
{
    public class BaseViewModel : IViewModel
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
