using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConfSpider.INL.ProcessComunicator
{
    public static class ProcessComunicatorSimple
    {
        public static void ExecCommandSimple(string command, string arguments)
        {
            var _proc = new Process();
            _proc.StartInfo.FileName = command;
            _proc.StartInfo.Arguments = arguments;

            _proc.Start();

            try
            {
                _proc.WaitForExit();
            }
            catch (ThreadAbortException)
            {
                _proc.Kill();
            }
        }
    }
}
