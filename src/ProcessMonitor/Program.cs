using System;
using System.Threading;
using static ProcessMonitor.MessageSender;

namespace ProcessMonitor
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("monitoring agent starting");
            WorkLoop();
        }

        static void WorkLoop()
        {
            while (true)
            {
                try
                {
                    SendProcessData();
                    Thread.Sleep(Configuration.SamplingInterval);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"an error occured: {ex.ToString()} ");
                }
            }
        }

        static void SendProcessData()
        {
            AgentPerfData perfData;
            try
            {
                var procInfo = DataCollector.GetProcessInfo(Configuration.ProcessNameToMonitor);
                perfData = new AgentPerfData()
                {
                    Host = Environment.MachineName,
                    SystemTime = DateTime.Now,
                    ProcessName = Configuration.ProcessNameToMonitor,
                    NumberOfProcesses = 1,
                    ProcessId = procInfo.ProcessId,
                    CpuPercentage = procInfo.CpuPercentage,
                    Memory = procInfo.VirtualMemory + procInfo.ResidentMemory + procInfo.SharedMemory,
                    MemoryPercentage = procInfo.PhysicalMemoryPercentage
                };
            }
            catch (ProcessCannotBeDeterminedException ex)
            {
                perfData = new AgentPerfData()
                {
                    Host = Environment.MachineName,
                    SystemTime = DateTime.Now,
                    ProcessName = Configuration.ProcessNameToMonitor,
                    NumberOfProcesses = ex.NumberOfProcesses,
                    ProcessId = 0,
                    CpuPercentage = 0,
                    Memory = 0,
                    MemoryPercentage = 0,
                    AdditionalData = ex.AdditionalData
                };
            }

            SendMessage(perfData);
        }
    }
}
