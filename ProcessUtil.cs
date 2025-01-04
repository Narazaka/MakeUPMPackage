using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Narazaka.MakeUPMPackage
{
    public static class ProcessUtil
    {
        internal static ProcessStartInfo CreateProcessStartInfo(IList<string> args, string cwd = null)
        {
            var info = new ProcessStartInfo(args[0])
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
            };
            if (cwd != null) info.WorkingDirectory = cwd;
            foreach (var arg in args.Skip(1)) info.ArgumentList.Add(arg);
            return info;
        }

        internal static Process Call(IList<string> args, string cwd = null)
        {
            return Call(CreateProcessStartInfo(args, cwd), string.Join(" ", args));
        }

        internal static Process Call(ProcessStartInfo info, string error)
        {
            var p = Process.Start(info);
            if (p == null)
            {
                throw new System.InvalidOperationException(error);
            }
            p.WaitForExit();
            return p;
        }

        internal static void PrintResult(this Process p)
        {
            if (p == null) return;
            UnityEngine.Debug.Log($"{p.StartInfo.FileName} {string.Join(" ", p.StartInfo.ArgumentList)}");
            var stdout = p.StandardOutput.ReadToEnd();
            if (!string.IsNullOrEmpty(stdout)) UnityEngine.Debug.Log("[STDOUT] " + stdout);
            var stderr = p.StandardError.ReadToEnd();
            if (!string.IsNullOrEmpty(stderr)) UnityEngine.Debug.Log("[STDERR] " + stderr);
        }
    }
}
