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
        Thread thread;
        bool lostarkState = false;
        public ProcessDetector(Form1 form1)
        {
            Form1 = form1;
            thread = new Thread(ProcessDetection);
        }

        public Form1 Form1 { get; }

        private void ProcessDetection()
        {
            while (true)
            {
                Process[] processList = Process.GetProcessesByName("LostArk");
                if (processList.Length > 0)
                {
                    if (lostarkState == false)
                    {
                        lostarkState = true;
                        Form1.SetImageAnalysisStateText("이미지탐지 대기중");
                        Form1.StartImageAnalysis();
                    }
                    
                }
                else
                {
                    if (lostarkState == true)
                    {
                        lostarkState = false;
                        Form1.SetImageAnalysisStateText("로스트아크 실행 대기중");
                        Form1.StopImageAnalysis();
                    }
                }
                GC.Collect();
                GC.WaitForPendingFinalizers();
                Thread.Sleep(5000);
            }
        }

        public void Run()
        {
            Thread thread = new Thread(ProcessDetection);
            thread.Start();
        }
    }

}
