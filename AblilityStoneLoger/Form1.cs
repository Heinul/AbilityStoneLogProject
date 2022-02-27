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
        }


        private Process LostarkProess = null;
        
        private void StartLogger()
        {
            //해상도 확인
            if(Screen.PrimaryScreen.Bounds.Width / 16 != Screen.PrimaryScreen.Bounds.Height / 9 && Screen.PrimaryScreen.Bounds.Width / 21 != Screen.PrimaryScreen.Bounds.Height / 9)
            {
                // 지원하지 않는 해상도 모니터
                label2.Text = "지원하지 않는 해상도입니다.\n해당 프로그램은 21:9 혹은 16:9의 해상도만 지원합니다.";
                label2.Visible = true;
            }
            else
            {
                //로스트아크 프로세스 확인
                ProcessDetector processDetector = new ProcessDetector(this);
                DisplayCapture displayCapture = new DisplayCapture(this);

                processDetector.Run();
                displayCapture.Run();
            }
            

        }

        public void SetProcess(Process process)
        {
            if(process == null)
            {
                LostarkProess = null;
                this.Invoke(new Action(delegate ()
                {
                    label1.Text ="프로세스 탐지 중";
                }));
            }
            else
            {
                LostarkProess = process;
                this.Invoke(new Action(delegate ()
                {
                    label1.Text = LostarkProess.ProcessName;
                }));
            }
            
        }

        public void SetImage(Bitmap bmp)
        {
            this.Invoke(new Action(delegate ()
            {
                pictureBox1.Image = bmp;
            }));
        }



        public Process GetProcess()
        {
            return LostarkProess;
        }

        private void Capt()
        {
            // 어빌리티 스톤창 확인
            Bitmap bmp = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            Graphics gr = Graphics.FromImage(bmp);
            gr.CopyFromScreen(0, 0, 0, 0, bmp.Size);
            bmp = new Bitmap(bmp, new System.Drawing.Size(576, 324));
            pictureBox1.Image = bmp;

        }
    }
}