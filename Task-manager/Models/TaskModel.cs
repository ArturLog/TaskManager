namespace Task_manager.Models
{
    public class TaskModel
    {
        public string TaskName { get; set; }
        public DateTime ExecutionTime { get; set; }
        public string Command { get; set; }
        public int ExecutionCount { get; set; }
        public TimeSpan TimeToNextExecution { get; set; }
    }
}
