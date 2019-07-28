using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ProcessMonitor
{
    public class DataCollector
    {
        private const string TopCommand = @"top -b -n 1 -p {0} | tail -1";

        public static ProcessInfo GetProcessInfo(string imageName)
        {
            Console.WriteLine($"retrieving process info of: {imageName} ");

            int procId = GetProcessId(imageName);
            string command = String.Format(TopCommand, procId);
            string topOutput = ExecuteProcess(command);

            Console.WriteLine($"process perf data: {topOutput} ");

            return ProcessInfo.Parse(topOutput);
        }

        private static int GetProcessId(string imageName)
        {
            string output = ExecuteProcess($"pgrep -f {imageName}");
            Console.WriteLine($"process IDs: {output} ");
            IEnumerable<string> processIds = output.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries).Distinct();

            if (processIds.Count() != 1)
                throw new ProcessCannotBeDeterminedException(processIds.Count(), output);

            return Int32.Parse(processIds.First());
        }

        private static string ExecuteProcess(string command)
        {
            string args = $"-c \"{command}\"";

            using (Process processToExecute = new Process())
            {
                processToExecute.StartInfo.FileName = "/bin/bash";
                processToExecute.StartInfo.Arguments = args;
                processToExecute.StartInfo.RedirectStandardOutput = true;
                processToExecute.StartInfo.RedirectStandardError = true;
                processToExecute.StartInfo.UseShellExecute = false;
                processToExecute.StartInfo.CreateNoWindow = true;
                processToExecute.Start();

                string result = processToExecute.StandardOutput.ReadToEnd();
                string error = processToExecute.StandardError.ReadToEnd();

                processToExecute.WaitForExit();

                int exitCode = processToExecute.ExitCode;

                return result;
            }
        }
    }
}
