
namespace Task_manager.Models
{
    public class ProcessModel
    {
        public int ProcessId { get; set; }
        public string Name { get; set; }
        public double MemoryUsage { get; set; }
        public string Status { get; set; }
        public List<int> Threads { get; set; }
        public List<string> Modules { get; set; }
    }
}
