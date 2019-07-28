using System;
using System.Collections.Generic;
using System.Text;

namespace ProcessMonitor
{
    public struct AgentPerfData
    {
        public string Host { get; set; }
        public DateTime SystemTime { get; set; }
        public string ProcessName { get; set; }
        public int NumberOfProcesses { get; set; }
        public int ProcessId { get; set; }
        public double CpuPercentage { get; set; }
        public double Memory { get; set; }
        public double MemoryPercentage { get; set; }
        public string AdditionalData { get; set; }
    }
}
