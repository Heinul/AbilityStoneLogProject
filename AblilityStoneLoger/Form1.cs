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
            //�ػ� Ȯ��
            if(Screen.PrimaryScreen.Bounds.Width / 16 != Screen.PrimaryScreen.Bounds.Height / 9 && Screen.PrimaryScreen.Bounds.Width / 21 != Screen.PrimaryScreen.Bounds.Height / 9)
            {
                // �������� �ʴ� �ػ� �����
                label2.Text = "�������� �ʴ� �ػ��Դϴ�.\n�ش� ���α׷��� 21:9 Ȥ�� 16:9�� �ػ󵵸� �����մϴ�.";
                label2.Visible = true;
            }
            else
            {
                //�ν�Ʈ��ũ ���μ��� Ȯ��
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
                    label1.Text ="���μ��� Ž�� ��";
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
            // �����Ƽ ����â Ȯ��
            Bitmap bmp = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            Graphics gr = Graphics.FromImage(bmp);
            gr.CopyFromScreen(0, 0, 0, 0, bmp.Size);
            bmp = new Bitmap(bmp, new System.Drawing.Size(576, 324));
            pictureBox1.Image = bmp;

        }
    }
}