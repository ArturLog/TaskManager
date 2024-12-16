using System.Collections.ObjectModel;
using System.Globalization;
using Microsoft.Win32.TaskScheduler;
using Task_manager.Commands;
using Task_manager.Models;
using Task = System.Threading.Tasks.Task;

namespace Task_manager.ViewsModels.Classes
{
    public class TaskSchedulerViewModel : BaseViewModel
    {
        #region Attributes
        private bool _isRefreshing;
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
        private int _refreshDelay;
        public int RefreshDelay
        {
            get => _refreshDelay;
            set
            {
                _refreshDelay = value;
                OnPropertyChanged(nameof(RefreshDelay));
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

        private string _newTaskName;
        public string NewTaskName
        {
            get => _newTaskName;
            set
            {
                _newTaskName = value;
                OnPropertyChanged(nameof(NewTaskName));
            }
        }
        private string _newTaskCommand;
        public string NewTaskCommand
        {
            get => _newTaskCommand;
            set
            {
                _newTaskCommand = value;
                OnPropertyChanged(nameof(NewTaskCommand));
            }
        }
        private DateTime? _newTaskDate;
        public DateTime? NewTaskDate
        {
            get => _newTaskDate;
            set
            {
                _newTaskDate = value;
                OnPropertyChanged(nameof(NewTaskDate));
            }
        }
        private string _newTaskCount;
        public string NewTaskCount
        {
            get => _newTaskCount;
            set
            {
                _newTaskCount = value;
                OnPropertyChanged(nameof(NewTaskCount));
            }
        }
        private string _newTaskHour;
        public string NewTaskHour
        {
            get => _newTaskHour;
            set
            {
                _newTaskHour = value;
                OnPropertyChanged(nameof(NewTaskHour));
            }
        }

        private bool _newTaskIsCyclic;
        public bool NewTaskIsCyclic
        {
            get => _newTaskIsCyclic;
            set
            {
                _newTaskIsCyclic = value;
                OnPropertyChanged(nameof(NewTaskIsCyclic));
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
        private RelayCommand _refreshCommand;
        public RelayCommand RefreshCommand
        {
            get
            {
                return _refreshCommand;
            }
        }

        private RelayCommand _startRefreshCommand;
        public RelayCommand StartRefreshCommand
        {
            get
            {
                return _startRefreshCommand;
            }
        }
        private RelayCommand _stopRefreshCommand;
        public RelayCommand StopRefreshCommand
        {
            get
            {
                return _stopRefreshCommand;
            }
        }
        private RelayCommand _filterCommand;
        public RelayCommand FilterCommand
        {
            get
            {
                return _filterCommand;
            }
        }

        private RelayCommand _executeTaskCommand;
        public RelayCommand ExecuteTaskCommand
        {
            get
            {
                return _executeTaskCommand;
            }
        }
        #endregion
        public TaskSchedulerViewModel()
        {
            AssignCommands();
            FilteredTasks = new ObservableCollection<TaskModel>();
            _tasks = new Collection<TaskModel>();
            _isRefreshing = true;
            _refreshDelay = 1;
            _newTaskHour = "HH:mm";
        }
        private void AssignCommands()
        {
            _refreshCommand = new RelayCommand(async () => await RefreshTasksAsync());
            _startRefreshCommand = new RelayCommand(() =>
            {
                _isRefreshing = true;
                StartRefreshTask();
            });
            _stopRefreshCommand = new RelayCommand(() =>
            {
                _isRefreshing = false;
            });
            _addTaskCommand = new RelayCommand(AddTask);
            _deleteTaskCommand = new RelayCommand(DeleteTask);
            _filterCommand = new RelayCommand(FilterProcesses);
            _executeTaskCommand = new RelayCommand(async () => await ExecuteTaskAsync());
        }
        private void ClearNewFields()
        {
            NewTaskName = string.Empty;
            NewTaskCommand = string.Empty;
            NewTaskHour = string.Empty;
            NewTaskIsCyclic = false;

            OnPropertyChanged(nameof(NewTaskName));
            OnPropertyChanged(nameof(NewTaskCommand));
            OnPropertyChanged(nameof(NewTaskHour));
            OnPropertyChanged(nameof(NewTaskIsCyclic));
        }
        public void AddTask()
        {
            try
            {
                if (!DateTime.TryParseExact(NewTaskHour, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedTime))
                {
                    Console.WriteLine("Invalid time format. Use HH:mm.");
                    return;
                }

                using (var taskService = new TaskService())
                {
                    var taskDefinition = taskService.NewTask();
                    taskDefinition.RegistrationInfo.Description = NewTaskName;

                    // Determine the trigger
                    if (NewTaskIsCyclic)
                    {
                        var dailyTrigger = new DailyTrigger
                        {
                            StartBoundary = DateTime.Today.Add(parsedTime.TimeOfDay),
                            DaysInterval = 1
                        };
                        taskDefinition.Triggers.Add(dailyTrigger);
                    }
                    else
                    {
                        var oneTimeTrigger = new TimeTrigger
                        {
                            StartBoundary = NewTaskDate.Value.Add(parsedTime.TimeOfDay)
                        };
                        taskDefinition.Triggers.Add(oneTimeTrigger);
                    }
                    taskDefinition.Actions.Add(new ExecAction(NewTaskCommand, null, null));

                    taskService.RootFolder.RegisterTaskDefinition(NewTaskName, taskDefinition);

                    RefreshTasksAsync();
                }
                ClearNewFields();
            }
            catch
            {
                // Handle exception
            }
        }
        public void DeleteTask()
        {
            try
            {
                using (var taskService = new TaskService())
                {
                   taskService.RootFolder.DeleteTask(SelectedTask.TaskName, false);
                   FilteredTasks.Remove(SelectedTask);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting task: {ex.Message}");
            }
        }
        public async Task ExecuteTaskAsync()
        {
            try
            {
                await Task.Run(() =>
                {
                    using (var taskService = new TaskService())
                    {
                        var task = taskService.FindTask(SelectedTask.TaskName);
                        if (task != null)
                        {
                            task.Run(); 
                        }
                        else
                        {
                            throw new Exception($"Task '{SelectedTask.TaskName}' not found.");
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing task: {ex.Message}");
            }
        }
        private void StartRefreshTask()
        {
            _isRefreshing = true;
            Task.Run(async () =>
            {
                while (_isRefreshing)
                {
                    await RefreshTasksAsync();
                   await Task.Delay(RefreshDelay * 1000);
                }
            });
        }
        private async Task RefreshTasksAsync()
        {
            await Task.Run(() =>
            {
                using (var taskService = new TaskService())
                {
                    var tasks = new List<TaskModel>();

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
                            ExecutionCount = task.NumberOfMissedRuns,
                            TimeToNextExecution = nextRunTime
                        };

                        tasks.Add(taskModel);
                    }
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        _tasks.Clear();
                        foreach (var taskModel in tasks)
                        {
                            _tasks.Add(taskModel);
                        }
                        CopyCollections();
                    });
                }
            });
        }
        private void FilterProcesses()
        {
            CopyCollections();
            var filteredProcesses = FilteredTasks.Where(p =>
            {
                var isNameValid = string.IsNullOrEmpty(FirstFilter) || p.TaskName.Contains(FirstFilter);
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
    }
}
