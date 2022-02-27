using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AblilityStoneLoger
{
    internal class ProcessDetector
    {
        public ProcessDetector(Form1 form1)
        {
            Form1 = form1;
        }

        public Form1 Form1 { get; }

        public void ProcessDetection()
        {
            while (true)
            {
                Process[] processList = Process.GetProcessesByName("LostArk");
                if(processList.Length > 0)
                {
                    Form1.SetProcess(processList[0]);
                }
                else
                {
                    Form1.SetProcess(null);
                }
                Thread.Sleep(100);
            }
        }

        public void Run()
        {
            Thread thread = new Thread(ProcessDetection);
            thread.Start();
        }
    }

}
