using AblilityStoneLoger;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbilityStoneLoger
{
    internal class ProcessDetector
    {
        public ProcessDetector(Form1 form1)
        {
            Form1 = form1;
        }

        public Form1 Form1 { get; }

        private void ProcessDetection()
        {
            while (true)
            {
                Process[] processList = Process.GetProcessesByName("LostArk");
                if (processList.Length > 0)
                {
                    Form1.SetLostArkState(true);
                    break;
                }
                else
                {
                    Form1.SetLostArkState(false);
                }
                Thread.Sleep(1000);
            }
        }

        public void Run()
        {
            Thread thread = new Thread(ProcessDetection);
            thread.Start();
        }
    }

}
