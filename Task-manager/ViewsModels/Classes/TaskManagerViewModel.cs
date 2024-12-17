using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Task_manager.Commands;
using Task_manager.Models;
using Task_manager.Views;

namespace Task_manager.ViewsModels.Classes
{
    public class TaskManagerViewModel : BaseViewModel
    {
        #region Attributes
        private bool _isRefreshing;
        private Collection<ProcessModel> _processes;
        #endregion
        #region Properties
        private ObservableCollection<ProcessModel> _filteredProcesses;
        public ObservableCollection<ProcessModel> FilteredProcesses
        {
            get => _filteredProcesses;
            set
            {
                _filteredProcesses = value;
                OnPropertyChanged(nameof(FilteredProcesses));
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
        private ProcessModel _selectedProcess;
        public ProcessModel SelectedProcess
        {
            get => _selectedProcess;
            set
            {
                _selectedProcess = value;
                OnPropertyChanged(nameof(SelectedProcess));
            }
        }
        #endregion
        #region Commands

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

        private RelayCommand _killProcessCommand;
        public RelayCommand KillProcessCommand
        {
            get
            {
                return _killProcessCommand;
            }
        }

        private RelayCommand _changePriorityCommand;
        public RelayCommand ChangePriorityCommand
        {
            get
            {
                return _changePriorityCommand;
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
        #endregion

        public TaskManagerViewModel()
        {
            AssignCommands();
            _processes = new Collection<ProcessModel>();
            FilteredProcesses = new ObservableCollection<ProcessModel>();
            _isRefreshing = true;
            _refreshDelay = 1;
        }
        private void KillProcess()
        {
            try
            {
                Process.GetProcessById(SelectedProcess.ProcessId).Kill();
                _processes.Remove(SelectedProcess);
                FilteredProcesses.Remove(SelectedProcess);
            }
            catch
            {
                // Handle errors
            }
        }
        private void ChangePriority(string? priority)
        {
            try
            {
                var process = Process.GetProcessById(SelectedProcess.ProcessId);
                process.PriorityClass = Enum.Parse<ProcessPriorityClass>(priority);
            }
            catch
            {
                // Handle errors
            }
        }
        private void AssignCommands()
        {
            _refreshCommand = new RelayCommand(async () => await RefreshProcessesAsync());
            _startRefreshCommand = new RelayCommand(() =>
            {
                _isRefreshing = true;
                StartRefreshTask();
            });
            _stopRefreshCommand = new RelayCommand(() =>
            {
                _isRefreshing = false;
            });
            _killProcessCommand = new RelayCommand(KillProcess);
            _changePriorityCommand = new RelayCommand(param => ChangePriority(param as string));
            _filterCommand = new RelayCommand(FilterProcesses);
        }
        private void StartRefreshTask()
        {
            _isRefreshing = true;
            Task.Run(async () =>
            {
                while (_isRefreshing)
                {
                    await RefreshProcessesAsync();
                    await Task.Delay(RefreshDelay * 2000);
                }
            });
        }
        private void FilterProcesses()
        {
            CopyCollections();
            var filteredProcesses = FilteredProcesses.Where(p =>
            {
                var isPidValid = string.IsNullOrEmpty(FirstFilter) || p.ProcessId == int.Parse(FirstFilter);
                var isNameValid = string.IsNullOrEmpty(SecondFilter) || p.Name.Contains(SecondFilter);
                var isStatusValid = string.IsNullOrEmpty(ThirdFilter) || p.Status.Contains(ThirdFilter);
                var isMemoryValid = string.IsNullOrEmpty(FourthFilter) || p.MemoryUsage >= double.Parse(FourthFilter);
                return isPidValid && isNameValid && isStatusValid && isMemoryValid;
            }).ToList();
            FilteredProcesses.Clear();
            foreach (var process in filteredProcesses)
            {
                FilteredProcesses.Add(process);
            }
        }
        private void CopyCollections()
        {
            FilteredProcesses.Clear();

            foreach (var process in _processes)
            {
                FilteredProcesses.Add(process);
            }
        }
        private async Task RefreshProcessesAsync()
        {
            var processList = Process.GetProcesses();

            await Task.Run(() =>
            {
                var updatedProcesses = processList.Select(p => new ProcessModel
                {
                    ProcessId = p.Id,
                    Name = p.ProcessName,
                    MemoryUsage = p.WorkingSet64 / 1024.0 / 1024.0, // Convert bytes to MB
                    Status = p.Responding ? "Running" : "Not Responding",
                    Threads = p.Threads.Cast<ProcessThread>().Select(t => t.Id).ToList(),
                    Modules = GetModules(p)
                }).ToList();

                App.Current.Dispatcher.Invoke(() =>
                {
                    _processes.Clear();
                    foreach (var process in updatedProcesses)
                    {
                        _processes.Add(process);
                    }
                    CopyCollections();
                });
            });
        }
        private List<string> GetModules(Process process)
        {
            try
            {
                return process.Modules.Cast<ProcessModule>().Select(m => m.ModuleName).ToList();
            }
            catch
            {
                return new List<string> { "Access Denied" };
            }
        }
    }
}
