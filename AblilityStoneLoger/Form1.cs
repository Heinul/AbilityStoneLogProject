using OpenCvSharp;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace AblilityStoneLoger
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            StartLogger();
            new Thread(MousePosition).Start();
        }


        private Process LostarkProess = null;
        private Mat display;
        ImageAnalysis imageAnalysis;
        private bool lostarkProcessState = false;

        private void StartLogger()
        {
            //해상도 확인
            if(Screen.PrimaryScreen.Bounds.Width / 16 != Screen.PrimaryScreen.Bounds.Height / 9 && Screen.PrimaryScreen.Bounds.Width / 21 != Screen.PrimaryScreen.Bounds.Height / 9)
            {
                not_supported_text.Visible = true;
            }
            else
            {
                ProcessDetector processDetector = new ProcessDetector(this);
                //DisplayCapture displayCapture = new DisplayCapture(this);
                imageAnalysis = new ImageAnalysis(this);

                processDetector.Run();
                //displayCapture.Run();
                imageAnalysis.Run();
            }

           
        }



        public void MousePosition()
        {
            try
            {
                while (true)
                {
                    this.Invoke(new Action(delegate ()
                    {
                        MousePos.Text = Control.MousePosition.ToString();

                    }));
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
        }

        public void SetEngravingData(string[] engravingName, int[] engravingSuccessData1, int[] engravingSuccessData2, int[] engravingSuccessData3, int percentage)
        {
            this.Invoke(new Action(delegate ()
            {
                label2.Text = percentage.ToString();
                engraving1.Text = engravingName[0] + "\n" + ArrayToStr(engravingSuccessData1);
                engraving2.Text = engravingName[1] + "\n" + ArrayToStr(engravingSuccessData2);
                engraving3.Text = engravingName[2] + "\n" + ArrayToStr(engravingSuccessData3);
            }));
        }

        private string ArrayToStr(int[] data)
        {
            string str = "";
            for(int i = 0; i < data.Length; i++)
            {
                str += data[i].ToString();
            }
            return str;
        }

        public void SetLostArkState(bool check)
        {
            lostarkProcessState = check;
        }

        public bool GetLostArkState()
        {
            return lostarkProcessState;
        }

        public void SetDisplay(Mat display)
        {
            this.display = display;
        }

        public Mat GetDisplay()
        {
            return display;
        }

        public void SetImage(Bitmap bmp)
        {
            this.Invoke(new Action(delegate ()
            {
                pictureBox1.Image = bmp;
            }));
        }

        public void SetPercentage(string str)
        {
            this.Invoke(new Action(delegate ()
            {
                label2.Text = str;
            }));
        }

        public Process GetProcess()
        {
            return LostarkProess;
        }
    }
}