using AblilityStoneLoger;
using Google.Cloud.Firestore;
using OpenCvSharp;

namespace AbilityStoneLoger
{
    internal class ImageAnalysis
    {
        private DisplayCapture displayCapture;
        private Form1 Form1;
        private ResourceLoader resourceLoader;
        private bool abilityWindowState = false;

        private Mat result = new Mat();

        private int previousPercentage = 0;
        private string[] previousEngravingName = new string[3] { "", "", "" };
        private int[][] previousEngravingSuccessData = new int[3][];

        private FirestoreDb firestoreDb;

        public ImageAnalysis(Form1 form1, ResourceLoader resourceLoader, FirestoreDb firestoreDb)
        {
            Form1 = form1;
            this.resourceLoader = resourceLoader; 
            displayCapture = new DisplayCapture();
            for (int i = 0; i < 3; i++)
                previousEngravingSuccessData[i] = new int[10];
            this.firestoreDb = firestoreDb;
        }

        bool threadState = false;
        public void Run()
        {
            if (!threadState) {
                threadState = true;

                new Thread(CaptureDisplay).Start();
                new Thread(ImageAnalysisThread).Start();
                new Thread(SaveData).Start();
            }
        }

        public void Stop()
        {
            threadState = false;
            displayQueue.Clear();
        }

        Queue<Mat> displayQueue = new Queue<Mat>();
        private void CaptureDisplay()
        {
            while (threadState)
            {
                Mat display = displayCapture.GetMatCapture();
                SerchAbilityStoneText(display);

                if (abilityWindowState)
                {
                    displayQueue.Enqueue(display);
                    Form1.SetImageAnalysisStateText("어빌리티스톤 세공 기록중");
                }
                else
                {
                    Form1.SetImageAnalysisStateText("Error : 세공창 인식 불가");
                }
                Thread.Sleep(1);
            }
        }

        private void ImageAnalysisThread()
        {
            string[] engravingName = new string[3];
            int percentage = 0;
            int[][] engravingSuccessData = new int[3][];
            for (int i = 0; i < 3; i++)
                engravingSuccessData[i] = new int[10] { 3, 3, 3, 3, 3, 3, 3, 3, 3, 3 };

            
            while (threadState)
            {
                if (displayQueue.Count > 0)
                {
                    Mat display = displayQueue.Dequeue();
                    percentage = PercentageCheck(display, percentage);

                    for (int i = 0; i < 3; i++)
                    {
                        engravingName[i] = EngravingImageCheck(display, i);
                        engravingSuccessData[i] = EngravingSuccessCheck(display, i);
                    }

                    bool errorCheck = false;
                    for(int i = 0; i < 10; i++)
                    {
                        if (engravingSuccessData[0][i] == 3 || engravingSuccessData[1][i] == 3 || engravingSuccessData[2][i] == 3)
                        {
                            errorCheck = true;
                            break;
                        }
                    }
                    if(!errorCheck)
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
                Thread.Sleep(1);
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
            if (!previousEngravingName[0].Equals(engravingName[0]) || !previousEngravingName[1].Equals(engravingName[1]) || !previousEngravingName[2].Equals(engravingName[2]))
            {
                Console.WriteLine("돌변경1");
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
                Console.WriteLine("돌변경2");
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
            else if ((Math.Abs(percentageDistace) > 10 || previousPercentage == percentage) && !(distance1[1] == 10 || distance2[1] == 10 || distance3[1] == 10))
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
                    PushData(previousPercentage, previousEngravingName[1], false, true, distance2[1]);
                }
                else if (distance2[0] == 2)
                {
                    PushData(previousPercentage, previousEngravingName[1], true, true, distance2[1]);
                }
                else if (distance3[0] == 1)
                {
                    PushData(previousPercentage, previousEngravingName[2], false, false, distance3[1]);
                }
                else if (distance3[0] == 2)
                {
                    PushData(previousPercentage, previousEngravingName[2], true, false, distance3[1]);
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
            AbilityItem data = new AbilityItem(percentage, engravingName, success, adjustment, digit, firestoreDb);
            queue.Enqueue(data);
        }

        private void SaveData()
        {
            //데이터 저장할 때 서버로 데이터 전송
            while (threadState)
            {
                if (queue.Count != 0)
                {
                    var item = queue.Dequeue();
                    item.SendData();
                    item.SaveData();
                    Form1.AddItemToListBox(item.GetEngravingName(), item.GetPercentage(), item.GetSuccess());
                }
                Thread.Sleep(10);
            }
        }

        private int[] GetEngravingDistance(int[][] previousData, int[][] engravingData, int num)
        {
            int[] result = new int[2];
            var a = ArrayToLong(engravingData[num]);
            var b = ArrayToLong(previousData[num]);
            double distance = a - b;
            double digits = 0;

            if (distance != 0)
            {
                digits = Math.Truncate(Math.Log10(Math.Abs(distance)));
                distance = distance / Math.Pow(10, digits);
            }

            result[0] = (int)Math.Ceiling(distance);
            result[1] = (int) (10 - digits);
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

            Cv2.MatchTemplate(display, resourceLoader.GetSuccessTextImage(), result, TemplateMatchModes.CCoeffNormed);
            
            OpenCvSharp.Point minloc, maxloc;
            double minval, maxval;

            Cv2.MinMaxLoc(result, out minval, out maxval, out minloc, out maxloc);

            if (maxval > 0.8)
            {
                abilityWindowState = true;
            }
            else
            {
                abilityWindowState = false;
            }
        }

        int[] posX1 = { 745, 783, 822, 862, 900, 939, 978, 1018, 1056, 1096 };
        int[] posX2 = { 744, 782, 822, 862, 900, 939, 978, 1017, 1054, 1094 };
        int[] posX_Reduction = { 742, 781, 820, 859, 898, 937, 976, 1015, 1054, 1093 };
        int[] posY = { 388, 481, 608 };
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


        private int GetAveragePixel(Mat display, int y, int x, int bgr)
        {
            int result = (display.At<Vec3b>(y, x)[bgr] +
                        display.At<Vec3b>(y, x)[bgr] +
                        display.At<Vec3b>(y, x)[bgr] +
                        display.At<Vec3b>(y, x)[bgr] +
                        display.At<Vec3b>(y, x)[bgr]) / 5;
            return result;
        }

        private int[] EngravingSuccessCheck(Mat display, int num)
        {
            /*
             * 0 : 아직 안누름, 1 : 실패, 2 : 성공, 3 : 인식오류
             */
            int[] data = new int[10];
            int r = 0, g = 0, b = 0;
            if (num == 0)
            {
                for (int i = 0; i < 10; i++)
                {
                    b = GetAveragePixel(display, posY[num], posX1[i], 0);
                    g = GetAveragePixel(display, posY[num], posX1[i], 1);
                    r = GetAveragePixel(display, posY[num], posX1[i], 2);
                    if (r < 50 && g < 50 && b < 50)
                        data[i] = 0;
                    else if (r < 130 && g < 130 && b < 130)
                        data[i] = 1;
                    else if (b > 150)
                        data[i] = 2;
                    else
                        data[i] = 3;
                }
            }
            else if (num == 1)
            {
                for (int i = 0; i < 10; i++)
                {
                    b = GetAveragePixel(display, posY[num], posX2[i], 0);
                    g = GetAveragePixel(display, posY[num], posX2[i], 1);
                    r = GetAveragePixel(display, posY[num], posX2[i], 2);

                    if (r < 50 && g < 50 && b < 50)
                        data[i] = 0;
                    else if (r < 130 && g < 130 && b < 130)
                        data[i] = 1;
                    else if (b > 150)
                        data[i] = 2;
                    else
                        data[i] = 3;
                }
            }
            else if (num == 2)
            {
                for (int i = 0; i < 10; i++)
                {
                    b = GetAveragePixel(display, posY[num], posX_Reduction[i], 0);
                    g = GetAveragePixel(display, posY[num], posX_Reduction[i], 1);
                    r = GetAveragePixel(display, posY[num], posX_Reduction[i], 2);

                    if (r < 50 && g < 50 && b < 50)
                        data[i] = 0;
                    else if (r < 130 && g < 130 && b < 130)
                        data[i] = 1;
                    else if (r > 130)
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
