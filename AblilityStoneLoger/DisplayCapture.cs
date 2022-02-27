using OpenCvSharp;
using OpenCvSharp.XFeatures2D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AblilityStoneLoger
{
    internal class DisplayCapture
    {
        private Mat abilityStoneTextImage = new Mat();
        
        
        public DisplayCapture(Form1 form1)
        {
            Form1 = form1;
            abilityStoneTextImage = OpenCvSharp.Extensions.BitmapConverter.ToMat(ImageResource.Ability_Stone_Text);
        }

        public Form1 Form1 { get; }

        private void Capture()
        {
            /* 캡쳐 공식
             * d = (가로해상도 - 1920) / 2
             * CopyFromScreen(d,0,d,0,bmp.size) 하면 1920으로 캡쳐되는걸로 추측됨
             */
            int difference = (Screen.PrimaryScreen.Bounds.Width - 1920) / 2;
            Bitmap bmp = new Bitmap(1920, 1080);
            while (true)
            {
                if (/*Form1.GetProcess() != null*/ true) {
                    
                    Graphics gr = Graphics.FromImage(bmp);
                    //Screen.PrimaryScreen.Bounds.Width
                    gr.CopyFromScreen(difference, 0, difference, 0, bmp.Size);

                    SerchAbilityStoneText(bmp);

                    /*
                     * TODO 이미지 분석기 생성해야함
                     */

                    Form1.SetImage(new Bitmap(bmp, new System.Drawing.Size(960, 540)));

                    System.GC.Collect();
                    System.GC.WaitForPendingFinalizers();
                }
                Thread.Sleep(100);
            }

        }

        public void Run()
        {
            Thread thread = new Thread(Capture);
            thread.Start();
        }

        private void SerchAbilityStoneText(Bitmap bitmap)
        {
            
            Mat img = OpenCvSharp.Extensions.BitmapConverter.ToMat(bitmap);

            var Detector = SURF.Create(hessianThreshold: 400);
            //var key1 = Detector.Detect(img, abilityStoneTextImage);
            //var key2 = Detector.Detect(img, abilityStoneTextImage);
            
        }
    }
}
