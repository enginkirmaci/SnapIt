using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;

namespace SnapIt.Common.Extensions;

public static class ProcessExtensions
{
    public static IList<Process> GetChildProcesses(this Process process)
        => new ManagementObjectSearcher(
                $"Select * From Win32_Process Where ParentProcessID={process.Id}")
            .Get()
            .Cast<ManagementObject>()
            .Select(mo =>
                Process.GetProcessById(Convert.ToInt32(mo["ProcessID"])))
            .ToList();

    public static string ProcessExecutablePath(this Process process)
    {
        try
        {
            return process.MainModule.FileName;
        }
        catch
        {
            string query = "SELECT ExecutablePath, ProcessID FROM Win32_Process";
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);

            foreach (ManagementObject item in searcher.Get())
            {
                object id = item["ProcessID"];
                object path = item["ExecutablePath"];

                if (path != null && id.ToString() == process.Id.ToString())
                {
                    return path.ToString();
                }
            }
        }

        return "";
    }
}