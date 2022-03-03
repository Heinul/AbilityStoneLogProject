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
        private string[] engravingName = new string[3];
        private int[] engravingSuccessData1 = new int[10];
        private int[] engravingSuccessData2 = new int[10];
        private int[] engravingSuccessData3 = new int[10];

        private int percentage = 0;


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
            while (true /*Form1.GetLostArkState()*/)
            {
                Mat display = displayCapture.GetMatCapture();
                SerchAbilityStoneText(display);
                if (abilityWindowState)
                {
                    PercentageCheck(display);

                    EngravingImageCheck(display, 0);
                    EngravingImageCheck(display, 1);
                    EngravingImageCheck(display, 2);

                    EngravingSuccessCheck(display, 0);
                    EngravingSuccessCheck(display, 1);
                    EngravingSuccessCheck(display, 2);

                    EngravingSuccessCheck(display, 0);
                    EngravingSuccessCheck(display, 1);
                    EngravingSuccessCheck(display, 2);
                

                    Form1.SetEngravingData(engravingName, engravingSuccessData1, engravingSuccessData2, engravingSuccessData3, percentage);
                }
                else
                {
                    ClearData();
                    Form1.SetEngravingData(engravingName, engravingSuccessData1, engravingSuccessData2, engravingSuccessData3, percentage);
                }
            }
        }
        private void ClearData()
        {
            for(int i = 0; i < engravingName.Length; i++)
            {
                engravingName[i] = "인식실패";
            }

            for (int i = 0; i < 10; i++)
            {
                engravingSuccessData1[i] = 0;
                engravingSuccessData2[i] = 0;
                engravingSuccessData3[i] = 0;
            }

            percentage = 0;
        }
        private void SerchAbilityStoneText(Mat display)
        {

            Cv2.MatchTemplate(display, resourceLoader.GetAbilityStoneText(), result, TemplateMatchModes.CCoeffNormed);

            OpenCvSharp.Point minloc, maxloc;
            double minval, maxval;
            Cv2.MinMaxLoc(result, out minval, out maxval, out minloc, out maxloc);
            //어빌리티 강화 텍스트 좌표 834,85 / 1030, 115
            if (maxval > 0.8 && maxloc.X > 800 && maxloc.X < 1050 && maxloc.Y > 80 && maxloc.Y < 130)
            {
                abilityWindowState = true;
                //Form1.SetImage(new Bitmap(OpenCvSharp.Extensions.BitmapConverter.ToBitmap(display), new System.Drawing.Size(960, 540)));
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

        private void PercentageCheck(Mat display)
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
                    percentage = percentageList[i];
                    break;
                }
                else
                {
                    percentage = 0;
                }
}
        }

        private void EngravingSuccessCheck(Mat display, int num)
        {
            /*
             * 0 : 아직 안누름, 1 : 실패, 2 : 성공
             */

            if(num == 0)
            {
                for(int i = 0; i< 10; i++)
                {
                    var b = display.At<Vec3b>(posY[num], posX[i])[0];
                    var g = display.At<Vec3b>(posY[num], posX[i])[1];
                    var r = display.At<Vec3b>(posY[num], posX[i])[2];
                    if (r < 30 && g < 30 && b < 30)
                        engravingSuccessData1[i] = 0;
                    else if (r < 150 && g < 150 && b < 150)
                        engravingSuccessData1[i] = 1;
                    else if (b > 180)
                        engravingSuccessData1[i] = 2;
                    else
                        engravingSuccessData1[i] = 3;
                }
            }
            else if(num == 1)
            {
                for (int i = 0; i < 10; i++)
                {
                    var b = display.At<Vec3b>(posY[num], posX[i] - 1)[0];
                    var g = display.At<Vec3b>(posY[num], posX[i] - 1)[1];
                    var r = display.At<Vec3b>(posY[num], posX[i] - 1)[2];
                    if (r < 30 && g < 30 && b < 30)
                        engravingSuccessData2[i] = 0;
                    else if (r < 150 && g < 150 && b < 150)
                        engravingSuccessData2[i] = 1;
                    else if( b > 180)
                        engravingSuccessData2[i] = 2;
                    else
                        engravingSuccessData3[i] = 3;
                }
            }
            else if(num == 2)
            {
                for(int i = 0; i < 10; i++)
                {
                    var b = display.Get<Vec3b>(posY[num], posX_Reduction[i] )[0];
                    var g = display.At<Vec3b>(posY[num], posX_Reduction[i])[1];
                    var r = display.At<Vec3b>(posY[num], posX_Reduction[i])[2];
                    if (r < 30 && g < 30 && b < 30)
                        engravingSuccessData3[i] = 0;
                    else if (r < 150 && g < 150 && b < 150)
                        engravingSuccessData3[i] = 1;
                    else if( r > 200)
                        engravingSuccessData3[i] = 2;
                    else
                    {
                        engravingSuccessData3[i] = 3;
                    }
                }
            }
            else
            {
                MessageBox.Show("뭔상태냐");
            }
        }

        private void EngravingImageCheck(Mat image, int num)
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
                        engravingName[num] = resourceLoader.GetEnhanceName(i);
                        break;
                    }
                    else
                    {
                        engravingName[num] = "인식실패";
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
                        engravingName[num] = resourceLoader.GetReductionName(i);
                        break;
                    }
                    else
                    {
                        engravingName[num] = "인식실패";
                    }
                }
            }
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
