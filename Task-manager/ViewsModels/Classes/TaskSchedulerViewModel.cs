
using Microsoft.Win32.TaskScheduler;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Task_manager.Commands;
using Task_manager.Models;

namespace Task_manager.ViewsModels.Classes
{
    public class TaskSchedulerViewModel : BaseViewModel
    {
        #region Attributes
        private Collection<TaskModel> _tasks;
        #endregion
        #region Properties
        private ObservableCollection<TaskModel> _filteredTasks;
        public ObservableCollection<TaskModel> FilteredTasks
        {
            get => _filteredTasks;
            set
            {
                _filteredTasks = value;
                OnPropertyChanged(nameof(FilteredTasks));
            }
        }
        private TaskModel _selectedTask;
        public TaskModel SelectedTask
        {
            get => _selectedTask;
            set
            {
                _selectedTask = value;
                OnPropertyChanged(nameof(SelectedTask));
            }
        }
        private string _firstFilter;
        public string FirstFilter
        {
            get => _firstFilter;
            set
            {
                _firstFilter = value;
                OnPropertyChanged(nameof(FirstFilter));
            }
        }
        private string _secondFilter;
        public string SecondFilter
        {
            get => _secondFilter;
            set
            {
                _secondFilter = value;
                OnPropertyChanged(nameof(SecondFilter));
            }
        }
        private string _thirdFilter;
        public string ThirdFilter
        {
            get => _thirdFilter;
            set
            {
                _thirdFilter = value;
                OnPropertyChanged(nameof(ThirdFilter));
            }
        }
        private string _fourthFilter;
        public string FourthFilter
        {
            get => _fourthFilter;
            set
            {
                _fourthFilter = value;
                OnPropertyChanged(nameof(FourthFilter));
            }
        }
        #endregion
        #region Commands
        private RelayCommand _addTaskCommand;
        public RelayCommand AddTaskCommand
        {
            get
            {
                return _addTaskCommand;
            }
        }
        private RelayCommand _deleteTaskCommand;
        public RelayCommand DeleteTaskCommand
        {
            get
            {
                return _deleteTaskCommand;
            }
        }
        #endregion
        public TaskSchedulerViewModel()
        {
            AssignCommands();
            FilteredTasks = new ObservableCollection<TaskModel>();
            _tasks = new Collection<TaskModel>();
            RefreshTasks();
        }
        private void AssignCommands()
        {
            _addTaskCommand = new RelayCommand(AddTask);
            _deleteTaskCommand = new RelayCommand(DeleteTask);
        }
        public void AddTask()
        {
            var newTask = new TaskModel
            {
                TaskName = "Sample Task",
                ExecutionTime = DateTime.Now.AddMinutes(1),
                Command = "notepad.exe",
                ExecutionCount = 0
            };
            Tasks.Add(newTask);
        }
        public void DeleteTask()
        {

        }
        private void RefreshTasks()
        {
            using (var taskService = new TaskService())
            {
                foreach (var task in taskService.AllTasks)
                {
                    var nextRunTime = task.NextRunTime == DateTime.MinValue
                        ? TimeSpan.Zero
                        : task.NextRunTime - DateTime.Now;

                    var taskModel = new TaskModel
                    {
                        TaskName = task.Name,
                        ExecutionTime = task.NextRunTime,
                        Command = task.Definition.Actions.OfType<ExecAction>().FirstOrDefault()?.Path ?? "N/A",
                        ExecutionCount = task.LastTaskResult, // Example: This can be replaced with a custom counter
                        TimeToNextExecution = nextRunTime
                    };

                    _tasks.Add(taskModel);
                }
            }
            CopyCollections();
        }
        private void FilterProcesses()
        {
            CopyCollections();
            var filteredProcesses = FilteredTasks.Where(p =>
            {
                var isNameValid = string.IsNullOrEmpty(FirstFilter) || p.TaskName.Contains(SecondFilter);
                var isExecutionTimeValid = string.IsNullOrEmpty(SecondFilter) || p.ExecutionTime.Minute >= int.Parse(SecondFilter);
                var isCountValid = string.IsNullOrEmpty(ThirdFilter) || p.ExecutionCount >= int.Parse(ThirdFilter);
                var isTimeToNextExecutionValid = string.IsNullOrEmpty(FourthFilter) || p.TimeToNextExecution.Minutes >= int.Parse(FourthFilter);
                return isNameValid && isExecutionTimeValid && isCountValid && isTimeToNextExecutionValid;
            }).ToList();
            FilteredTasks.Clear();
            foreach (var process in filteredProcesses)
            {
                FilteredTasks.Add(process);
            }
        }
        private void CopyCollections()
        {
            FilteredTasks.Clear();

            foreach (var process in _tasks)
            {
                FilteredTasks.Add(process);
            }
        }
        //private async Task ScheduleTasks()
        //{
        //    while (true)
        //    {
        //        foreach (var task in Tasks.ToList())
        //        {
        //            if (DateTime.Now >= task.ExecutionTime)
        //            {
        //                // Execute the command
        //                try
        //                {
        //                    Process.Start(task.Command);
        //                    task.ExecutionCount++;
        //                    task.ExecutionTime = task.ExecutionTime.AddMinutes(1); // Update next execution
        //                }
        //                catch
        //                {
        //                    // Handle execution errors
        //                }
        //            }

        //            // Update time to next execution
        //            task.TimeToNextExecution = task.ExecutionTime - DateTime.Now;
        //        }

        //        await Task.Delay(1000); // Refresh every second
        //    }
        //}
    }
}
