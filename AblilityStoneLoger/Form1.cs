using AbilityStoneLoger;
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

        private ImageAnalysis imageAnalysis;
        private Process LostarkProess = null;
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
                AbilityItem a = new AbilityItem(0, "", false, false);


                ProcessDetector processDetector = new ProcessDetector(this);
                processDetector.Run();

                imageAnalysis = new ImageAnalysis(this);
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

        public void SetLostArkState(bool check)
        {
            lostarkProcessState = check;
        }

        public bool GetLostArkState()
        {
            return lostarkProcessState;
        }

        public Process GetProcess()
        {
            return LostarkProess;
        }

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        private void Form_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
        }
    }
}