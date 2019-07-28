using System;
using System.Collections.Generic;
using System.Text;

namespace ProcessMonitor
{
    public class ProcessCannotBeDeterminedException : Exception
    {
        public int NumberOfProcesses { get; }
        public string AdditionalData { get; }

        public ProcessCannotBeDeterminedException(int numOfProcesses, string additionalData = null) : base($"The number of processes is not supported, expected: 1, actual: {numOfProcesses}")
        {
            NumberOfProcesses = numOfProcesses;
            AdditionalData = additionalData;
        }
    }
}
