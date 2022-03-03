using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AblilityStoneLoger
{
    internal class ImageAnalysis
    {
        private DisplayCapture displayCapture;

        public Form1 Form1 { get; }

        private ResourceLoader resourceLoader = new ResourceLoader();
        private bool abilityWindowState = false;

        private Mat result = new Mat();

        public ImageAnalysis(Form1 form1)
        {
            Form1 = form1;

            displayCapture = new DisplayCapture();
        }

        public void Run()
        {
            Thread thread = new Thread(ImageAnalysisThread);
            thread.Start();
        }

        private void ImageAnalysisThread()
        {
            string[] engravingName = new string[3];
            int[][] engravingSuccessData = new int[3][];
            engravingSuccessData[0] = new int[10];
            engravingSuccessData[1] = new int[10];
            engravingSuccessData[2] = new int[10];
            int percentage = 0;

            while (true /*Form1.GetLostArkState()*/)
            {
                Mat display = displayCapture.GetMatCapture();
                SerchAbilityStoneText(display);
                if (abilityWindowState)
                {
                    percentage = PercentageCheck(display);

                    for (int i = 0; i < 3; i++)
                    {
                        engravingName[i] = EngravingImageCheck(display, i);
                        engravingSuccessData[i] = EngravingSuccessCheck(display, i);
                    }

                    //ComparisonData();


                    Form1.SetEngravingData(engravingName, engravingSuccessData, percentage);
                }
                else
                {
                    for (int i = 0; i < 3; i++)
                    {
                        engravingName[i] = "인식실패";
                        engravingSuccessData[i] = new int[10] { 3, 3, 3, 3, 3, 3, 3, 3, 3, 3 };
                    }
                    Form1.SetEngravingData(engravingName, engravingSuccessData, percentage);
                }
            }
        }

        private void SerchAbilityStoneText(Mat display)
        {

            Cv2.MatchTemplate(display, resourceLoader.GetAbilityStoneText(), result, TemplateMatchModes.CCoeffNormed);

            OpenCvSharp.Point minloc, maxloc;
            double minval, maxval;
            
            Cv2.MinMaxLoc(result, out minval, out maxval, out minloc, out maxloc);
            
            if (maxval > 0.8 && maxloc.X > 800 && maxloc.X < 1050 && maxloc.Y > 80 && maxloc.Y < 130)
            {
                abilityWindowState = true;
            }
            else
            {
                abilityWindowState = false;
            }
        }

        int[] posX = { 745, 783, 822, 862, 900, 939, 978, 1018, 1057, 1096 };
        int[] posX_Reduction = { 742, 781, 820, 859, 898, 937, 976, 1015, 1054, 1093 };
        int[] posY = { 388, 481, 607 };
        int[] percentageList = { 75, 65, 55, 45, 35, 25 };

        private int PercentageCheck(Mat display)
        {
            Mat percentageSerchResult = new Mat();
            Mat percentageArea = display.SubMat(new OpenCvSharp.Rect(1100, 200, 200, 200));
            OpenCvSharp.Point minloc, maxloc;
            double minval, maxval;

            // 퍼센트 확인
            for (int i = 0; i< 6; i++)
            {
                Cv2.MatchTemplate(percentageArea, resourceLoader.GetPersentageImage(i), percentageSerchResult, TemplateMatchModes.CCoeffNormed);
                Cv2.MinMaxLoc(percentageSerchResult, out minval, out maxval, out minloc, out maxloc);
                if (maxval > 0.95)
                {
                    Form1.SetPercentage(percentageList[i].ToString());
                    return percentageList[i];
                }
            }
            return 0;
        }

        private int[] EngravingSuccessCheck(Mat display, int num)
        {
            /*
             * 0 : 아직 안누름, 1 : 실패, 2 : 성공, 3 : 인식오류
             */
            int[] data = new int[10];
            if (num != 2)
            {
                for(int i = 0; i< 10; i++)
                {
                    var b = display.At<Vec3b>(posY[num], posX[i] - num)[0];
                    var g = display.At<Vec3b>(posY[num], posX[i] - num)[1];
                    var r = display.At<Vec3b>(posY[num], posX[i] - num)[2];
                    if (r < 30 && g < 30 && b < 30)
                        data[i] = 0;
                    else if (r < 150 && g < 150 && b < 150)
                        data[i] = 1;
                    else if (b > 180)
                        data[i] = 2;
                    else
                        data[i] = 3;
                }
            }
            else
            {
                for(int i = 0; i < 10; i++)
                {
                    var b = display.Get<Vec3b>(posY[num], posX_Reduction[i] )[0];
                    var g = display.At<Vec3b>(posY[num], posX_Reduction[i])[1];
                    var r = display.At<Vec3b>(posY[num], posX_Reduction[i])[2];
                    if (r < 30 && g < 30 && b < 30)
                        data[i] = 0;
                    else if (r < 150 && g < 150 && b < 150)
                        data[i] = 1;
                    else if( r > 200)
                        data[i] = 2;
                    else
                        data[i] = 3;
                }
            }

            return data;
        }

        private string EngravingImageCheck(Mat image, int num)
        {
            Mat engravingSerchResult = new Mat();
            Mat engravingArea = GetEngravingImageArea(image, num);
            OpenCvSharp.Point minloc, maxloc;
            double minval, maxval;
            if (num != 2)
            {
                for (int i = 0; i < 43; ++i)
                {
                    Mat img = resourceLoader.GetEnhanceImage(i);
                    Cv2.MatchTemplate(engravingArea, img, engravingSerchResult, TemplateMatchModes.CCoeffNormed);
                    Cv2.MinMaxLoc(engravingSerchResult, out minval, out maxval, out minloc, out maxloc);
                    if (maxval > 0.95)
                    {
                        return resourceLoader.GetEnhanceName(i);
                    }
                }
            }
            else
            {
                for (int i = 0; i < 4; ++i)
                {
                    Mat img = resourceLoader.GetReductionImage(i);
                    Cv2.MatchTemplate(engravingArea, img, engravingSerchResult, TemplateMatchModes.CCoeffNormed);
                    Cv2.MinMaxLoc(engravingSerchResult, out minval, out maxval, out minloc, out maxloc);
                    if (maxval > 0.95)
                    {
                        return resourceLoader.GetEnhanceName(i);
                    }
                }
            }
            return "인식실패";
        }

        private Mat GetEngravingImageArea(Mat display, int num)
        {
            if (num == 0)
            {
                return display.SubMat(new OpenCvSharp.Rect(620, 332, 96, 96));
            }
            else if (num == 1)
            {
                return display.SubMat(new OpenCvSharp.Rect(620, 426, 96, 96));
            }
            else if (num == 2)
            {
                return display.SubMat(new OpenCvSharp.Rect(620, 552, 96, 96));
            }
            else
            {
                return null;
            }

        }

    }
}
