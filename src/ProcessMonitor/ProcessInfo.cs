using System;
using System.Text.RegularExpressions;

namespace ProcessMonitor
{
    public class ProcessInfo
    {
        public enum ProcessStatus
        {
            Unknown,
            UninteruptibleSleep,
            Running,
            Sleeping,
            Stopped,
            Zombie
        }

        public int ProcessId { get; set; }
        public string User { get; set; }
        public int Priority { get; set; }
        public int Nice { get; set; }
        public int VirtualMemory { get; set; }
        public int ResidentMemory { get; set; }
        public int SharedMemory { get; set; }
        public ProcessStatus Status { get; set; }
        public double CpuPercentage { get; set; }
        public double PhysicalMemoryPercentage { get; set; }
        public string Command { get; set; }

        public static ProcessInfo Parse(string topLine)
        {
            //the line from top looks like this:
            //topLine = " 1001 root      20   0  553608 170668  48104 S 13.3  4.2  15:01.72 Xorg";

            string[] stuff = Regex.Split(topLine.Trim(), @"\s+");

            return new ProcessInfo()
            {
                ProcessId = Int32.Parse(stuff[0]),
                User = stuff[1],
                Priority = Int32.Parse(stuff[2]),
                Nice = Int32.Parse(stuff[3]),
                VirtualMemory = Int32.Parse(stuff[4]),
                ResidentMemory = Int32.Parse(stuff[5]),
                SharedMemory = Int32.Parse(stuff[6]),
                Status = ParseProcStatus(stuff[7]),
                CpuPercentage = Double.Parse(stuff[8]),
                PhysicalMemoryPercentage = Double.Parse(stuff[9]),
                Command = stuff[11]
            };
        }

        private static ProcessStatus ParseProcStatus(string status)
        {
            switch (status)
            {
                case "D":
                    return ProcessStatus.UninteruptibleSleep;
                case "R":
                    return ProcessStatus.Running;
                case "S":
                    return ProcessStatus.Sleeping;
                case "T":
                    return ProcessStatus.Stopped;
                case "Z":
                    return ProcessStatus.Zombie;
                default:
                    return ProcessStatus.Unknown;

            }
        }
    }
}
