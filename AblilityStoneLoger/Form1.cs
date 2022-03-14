using AbilityStoneLoger;
using OpenCvSharp;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace AblilityStoneLoger
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.Load += LoadTrayIcon;
        }

        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(int nLeftRect
                                                      , int nTopRect
                                                      , int nRightRect
                                                      , int nBottomRect
                                                      , int nWidthEllipse
                                                      , int nHeightEllipse);

        private void Form1_Load(object sender, EventArgs e)
        {
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, this.Width, this.Height, 20, 20));
            Init();
            StartLogger();
            new Thread(MousePosition).Start();
        }

        DashBoardPage dashboard;
        ImageAnalysis imageAnalysis;
        ResourceLoader resourceLoader;

        DetailPage detailPage;

        private void Init()
        {
            resourceLoader = new ResourceLoader();
            
            PictureBox[] itemImages = new PictureBox[] { ItemImage1, ItemImage2, ItemImage3, ItemImage4, ItemImage5, ItemImage6, ItemImage7 }; ;
            Label[] imageNames = new Label[] { ImageName1, ImageName2, ImageName3, ImageName4, ImageName5, ImageName6, ImageName7 }; ;
            Label[] successText = new Label[] { SuccessText1, SuccessText2, SuccessText3, SuccessText4, SuccessText5, SuccessText6, SuccessText7 }; ;
            PictureBox[] dashboardEnhanceGraph = { EnhanceGraph25, EnhanceGraph35, EnhanceGraph45, EnhanceGraph55, EnhanceGraph65, EnhanceGraph75 };
            PictureBox[] dashboardReductionGraph = { ReductionGraph25, ReductionGraph35, ReductionGraph45, ReductionGraph55, ReductionGraph65, ReductionGraph75 };
            dashboard = new DashBoardPage(this, resourceLoader, dashboardEnhanceGraph, dashboardReductionGraph, TryLabel, SuccessLabel, FailLabel, CoinLabel, itemImages, imageNames, successText);

            PictureBox[] detailEnhanceGraph = { EnhanceGraph25_Detail, EnhanceGraph35_Detail, EnhanceGraph45_Detail, EnhanceGraph55_Detail, EnhanceGraph65_Detail, EnhanceGraph75_Detail };
            PictureBox[] detailReductionGraph = { ReductionGraph25_Detail, ReductionGraph35_Detail, ReductionGraph45_Detail, ReductionGraph55_Detail, ReductionGraph65, ReductionGraph75_Detail };
            TransparentPanel dotGraph = DotGraphPanel;
            detailPage = new DetailPage(this, StartDateTimePicker, EndDateTimePicker, detailEnhanceGraph, detailReductionGraph, dotGraph);
        }

       
        private void StartLogger()
        {
            //해상도 확인
            if (Screen.PrimaryScreen.Bounds.Height != 1080 && Screen.PrimaryScreen.Bounds.Height != 1440 && Screen.PrimaryScreen.Bounds.Height != 2160)
            {
                MessageBox.Show("FHD 이상의 해상도만을 지원합니다.");
            }
            else
            {
                try
                {
                    //imageAnalysis = new ImageAnalysis(this, resourceLoader);
                    //ProcessDetector processDetector = new ProcessDetector(this);
                    //processDetector.Run();

                    //Test 코드
                    imageAnalysis = new ImageAnalysis(this, resourceLoader);
                    imageAnalysis.Run();

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        public void StartImageAnalysis()
        {
            imageAnalysis.Run();
        }

        public void StopImageAnalysis()
        {
            imageAnalysis.Stop();
        }

        public void MousePosition()
        {
            try
            {
                while (true)
                {
                    this.Invoke(new Action(delegate ()
                    {
                        label1.Text = Screen.PrimaryScreen.Bounds.Width.ToString();
                        MousePos.Text = Control.MousePosition.ToString();
                    }));
                    Thread.Sleep(100);
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }

        }

        #region 이벤트 전달 메서드
        public void AddItemToListBox(string engravingNAme, int percentage, bool success)
        {
            dashboard.AddItemToListBox(engravingNAme, percentage, success);
        }
        #endregion

        #region Form 이벤트 메서드
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        // 폼 이동
        private void Form_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
        }

        private void Home_Click(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex != 0)
            {
                dashboard.SetPageState(true);
                dashboard.UpdateDashboard();

                detailPage.SetPageState(false);


                tabControl1.SelectedIndex = 0;
            }
        }

        private void LogDetail_Click(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex != 1)
            {
                detailPage.SetPageState(true);
                detailPage.UpdateDetailPage();

                dashboard.SetPageState(false);

                tabControl1.SelectedIndex = 1;
            }
        }

        private void Tendency_Click(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex != 2)
            {
                dashboard.SetPageState(false);
                detailPage.SetPageState(false);

                tabControl1.SelectedIndex = 2;
            }
        }

        private void Option_Click(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex != 3)
            {
                dashboard.SetPageState(false);
                detailPage.SetPageState(false);

                tabControl1.SelectedIndex = 3;
            }
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            TrayIcon.Dispose();
            System.Diagnostics.Process.GetCurrentProcess().Kill();
            Application.Exit();
        }

        private void GraphMouseMove(object sender, EventArgs e)
        {
            dashboard.GraphMouseMove(sender, e, DashboardGraphToolTip);
        }

        #endregion

        #region 트레이아이콘

        private void LoadTrayIcon(object? sender, EventArgs e)
        {
            TrayIcon.Visible = true;
            TrayIcon.ContextMenuStrip = TrayMenu;
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TrayIcon.Dispose();
            System.Diagnostics.Process.GetCurrentProcess().Kill();
            Application.Exit();
        }

        private void ShowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ShowInTaskbar = true;
            this.Visible = true;
            this.WindowState = FormWindowState.Normal;
        }

        private void TrayClick(object sender, EventArgs e)
        {
            this.WindowState |= FormWindowState.Minimized;
            this.ShowInTaskbar = false;
            this.Visible = false;
        }

        private void TrayIconMouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.ShowInTaskbar = true;
            this.Visible = true;
            this.WindowState = FormWindowState.Normal;
        }

        #endregion
    }

   
}