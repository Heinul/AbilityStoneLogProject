using AblilityStoneLoger;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbilityStoneLoger
{
    internal class ImageAnalysis
    {
        private DisplayCapture displayCapture;

        public Form1 Form1 { get; }

        private ResourceLoader resourceLoader = new ResourceLoader();
        private bool abilityWindowState = false;

        private Mat result = new Mat();

        private int previousPercentage = 0;
        private string[] previousEngravingName = new string[3] { "", "", "" };
        private int[][] previousEngravingSuccessData = new int[3][];

        public ImageAnalysis(Form1 form1)
        {
            Form1 = form1;

            displayCapture = new DisplayCapture();
            for (int i = 0; i < 3; i++)
                previousEngravingSuccessData[i] = new int[10];
        }

        public void Run()
        {
            Thread thread = new Thread(ImageAnalysisThread);
            thread.Start();
            SaveData();
        }

        private void ImageAnalysisThread()
        {
            string[] engravingName = new string[3];
            int percentage = 0;
            int[][] engravingSuccessData = new int[3][];
            for (int i = 0; i < 3; i++)
                engravingSuccessData[i] = new int[10] { 3, 3, 3, 3, 3, 3, 3, 3, 3, 3 };

            
            while (true /*Form1.GetLostArkState()*/)
            {
                Mat display = displayCapture.GetMatCapture();

                //for(int i = 0; i < 3; i++)
                //{
                //    for (int j = 0; j < 10; j++)
                //    {
                //        Cv2.Rectangle(display, new Rect(posX[j], posY[i], 1, 1) , Scalar.Green, 3);
                //    }
                //}

                //display.SaveImage("image.png");

                SerchAbilityStoneText(display);
                if (abilityWindowState)
                {
                    percentage = PercentageCheck(display, percentage);

                    for (int i = 0; i < 3; i++)
                    {
                        engravingName[i] = EngravingImageCheck(display, i);
                        engravingSuccessData[i] = EngravingSuccessCheck(display, i);
                    }

                    ComparisonData(percentage, engravingName, engravingSuccessData);
                }
                else
                {
                    percentage = 0;
                    for (int i = 0; i < 3; i++)
                    {
                        engravingName[i] = "인식실패";
                        engravingSuccessData[i] = new int[10] { 3, 3, 3, 3, 3, 3, 3, 3, 3, 3 };
                    }
                }
            }
        }

        private void ComparisonData(int percentage, string[] engravingName, int[][] engravingSuccessData)
        {
            // 첫입력
            if (previousEngravingName[0] == "")
            {
                previousPercentage = percentage;
                for (int i = 0; i < 3; i++)
                {
                    previousEngravingName[i] = engravingName[i];
                    for (int j = 0; j < 10; j++)
                    {
                        previousEngravingSuccessData[i][j] = engravingSuccessData[i][j];
                    }
                }
                return;
            }

            if (engravingName[0] == "인식실패" || engravingName[1] == "인식실패" || engravingName[2] == "인식실패")
            {
                return;
            }

            if (previousPercentage == 0)
            {
                previousPercentage = percentage;
                return;
            }

            
            var distance1 = GetEngravingDistance(previousEngravingSuccessData, engravingSuccessData, 0);
            var distance2 = GetEngravingDistance(previousEngravingSuccessData, engravingSuccessData, 1);
            var distance3 = GetEngravingDistance(previousEngravingSuccessData, engravingSuccessData, 2);
            int percentageDistace = previousPercentage - percentage;
            // 어빌리티 스톤 변경
            if (previousEngravingName[0] != engravingName[0] || previousEngravingName[1] != engravingName[1] || previousEngravingName[2] != engravingName[2])
            {
                //각인이름이 달라진 경우 갱신
                previousPercentage = percentage;
                for (int i = 0; i < 3; i++)
                {
                    previousEngravingName[i] = engravingName[i];
                    for (int j = 0; j < 10; j++)
                    {
                        previousEngravingSuccessData[i][j] = engravingSuccessData[i][j];
                    }
                }
                return;
            }
            else if (distance1[0] == 0 && distance2[0] == 0 && distance3[0] == 0)
            {
                return;
            }
            else if (distance1[0] < 0 || distance1[0] > 2 || distance2[0] < 0 || distance2[0] > 2 || distance3[0] < 0 || distance3[0] > 2)
            {
                // 각인은 같으나 돌을 바꾼경우 (값의 차가 +1~+2가 아닌경우) 갱신
                previousPercentage = percentage;
                for (int i = 0; i < 3; i++)
                {
                    previousEngravingName[i] = engravingName[i];
                    for (int j = 0; j < 10; j++)
                    {
                        previousEngravingSuccessData[i][j] = engravingSuccessData[i][j];
                    }
                }
                return;
            }
            else if (Math.Abs(percentageDistace) > 10)
            {
                return;
            }
            else if( distance1[0] == 1 || distance1[0] == 2 || distance2[0] == 1 || distance2[0] == 2 || distance3[0] == 1 || distance3[0] == 2)
            {
                //값이 범위 내로 증가하면 강화를 했다는거니까 저장하고 갱신하면됨
                if (distance1[0] == 1 )
                {
                    PushData(previousPercentage, previousEngravingName[0], false, true, distance1[1]);
                }
                else if(distance1[0] == 2)
                {
                    PushData(previousPercentage, previousEngravingName[0], true, true, distance1[1]);
                }
               else if(distance2[0] == 1)
                {
                    PushData(previousPercentage, previousEngravingName[1], false, true, distance1[1]);
                }
                else if (distance2[0] == 2)
                {
                    PushData(previousPercentage, previousEngravingName[1], true, true, distance1[1]);
                }
                else if (distance3[0] == 1)
                {
                    PushData(previousPercentage, previousEngravingName[2], false, false, distance1[1]);
                }
                else if (distance3[0] == 2)
                {
                    PushData(previousPercentage, previousEngravingName[2], true, false, distance1[1]);
                }

                previousPercentage = percentage;
                for (int i = 0; i < 3; i++)
                {
                    previousEngravingName[i] = engravingName[i];
                    for (int j = 0; j < 10; j++)
                    {
                        previousEngravingSuccessData[i][j] = engravingSuccessData[i][j];
                    }
                }
                return;
            }
        }

        Queue<AbilityItem> queue = new Queue<AbilityItem>();
        private void PushData(int percentage, string engravingName, bool success, bool adjustment, int digit)
        {
            //큐에 데이터 올리고 다른 스레드로 저장 작업 처리
            AbilityItem data = new AbilityItem(percentage, engravingName, success, adjustment, digit);
            queue.Enqueue(data);
        }

        private void SaveData()
        {
            //데이터 저장할 때 서버로 데이터 전송
            new Thread(() =>
            {
                while (true)
                {
                    if (queue.Count != 0)
                    {
                        var item = queue.Dequeue();
                        item.SendData();
                        item.SaveData();
                    }
                }
            }).Start();

        }

        private int[] GetEngravingDistance(int[][] previousData, int[][] engravingData, int num)
        {
            int[] result = new int[2];
            var a = ArrayToLong(engravingData[num]);
            var b = ArrayToLong(previousData[num]);
            double distance = a - b;
            double digits = Math.Truncate(Math.Log10(Math.Abs(distance)));

            if (distance != 0)
                 distance = distance / Math.Pow(10,digits);

            result[0] = (int) distance;
            result[1] = (int) (11 - digits);
            return (int[])result.Clone();
        }

        private long ArrayToLong(int[] data)
        {
            string str = "";
            for (int i = 0; i < data.Length; i++)
            {
                str += data[i].ToString();
            }

            return long.Parse(str);
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

        private int PercentageCheck(Mat display, int percentage)
        {
            Mat percentageSerchResult = new Mat();
            Mat percentageArea = display.SubMat(new Rect(1100, 200, 200, 200));
            OpenCvSharp.Point minloc, maxloc;
            double minval, maxval;

            double[] val = new double[6];
            // 퍼센트 확인

            for (int i = 0; i < 6; i++)
            {
                Mat gray = new Mat();
                Cv2.CvtColor(percentageArea, gray, ColorConversionCodes.BGR2GRAY);

                Cv2.MatchTemplate(gray, resourceLoader.GetPercentageGrayImage(i), percentageSerchResult, TemplateMatchModes.CCoeffNormed);
                Cv2.MinMaxLoc(percentageSerchResult, out minval, out maxval, out minloc, out maxloc);
                val[i] = maxval;
            }


            var maxVal = val.Max();
            var maxIndex = val.ToList().IndexOf(maxVal);

            if (maxVal < 0.5)
            {
                return 0;
            }
            else
            {
                return percentageList[maxIndex];
            }
        }

        private int[] EngravingSuccessCheck(Mat display, int num)
        {
            /*
             * 0 : 아직 안누름, 1 : 실패, 2 : 성공, 3 : 인식오류
             */
            int[] data = new int[10];
            if (num != 2)
            {
                for (int i = 0; i < 10; i++)
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
                for (int i = 0; i < 10; i++)
                {
                    var b = display.Get<Vec3b>(posY[num], posX_Reduction[i])[0];
                    var g = display.At<Vec3b>(posY[num], posX_Reduction[i])[1];
                    var r = display.At<Vec3b>(posY[num], posX_Reduction[i])[2];
                    if (r < 30 && g < 30 && b < 30)
                        data[i] = 0;
                    else if (r < 150 && g < 150 && b < 150)
                        data[i] = 1;
                    else if (r > 200)
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
                        return resourceLoader.GetReductionName(i);
                    }
                }
            }
            return "인식실패";
        }

        private Mat GetEngravingImageArea(Mat display, int num)
        {
            if (num == 0)
            {
                return display.SubMat(new Rect(620, 332, 96, 96));
            }
            else if (num == 1)
            {
                return display.SubMat(new Rect(620, 426, 96, 96));
            }
            else if (num == 2)
            {
                return display.SubMat(new Rect(620, 552, 96, 96));
            }
            else
            {
                return null;
            }

        }

    }
}
