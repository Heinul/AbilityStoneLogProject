using AbilityStoneLoger;
using OpenCvSharp;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Google.Cloud.Firestore;

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
            //new Thread(MousePosition).Start();
        }

        DashBoardPage dashboard;
        ImageAnalysis imageAnalysis;
        ResourceLoader resourceLoader;
        DetailPage detailPage;
        FirestoreDb firestoreDb;

        private void Init()
        {
            resourceLoader = new ResourceLoader();
            
            PictureBox[] itemImages = new PictureBox[] { ItemImage1, ItemImage2, ItemImage3, ItemImage4, ItemImage5, ItemImage6, ItemImage7 }; ;
            Label[] imageNames = new Label[] { ImageName1, ImageName2, ImageName3, ImageName4, ImageName5, ImageName6, ImageName7 }; ;
            Label[] successText = new Label[] { SuccessText1, SuccessText2, SuccessText3, SuccessText4, SuccessText5, SuccessText6, SuccessText7 }; ;
            PictureBox[] dashboardEnhanceGraph = { EnhanceGraph25, EnhanceGraph35, EnhanceGraph45, EnhanceGraph55, EnhanceGraph65, EnhanceGraph75 };
            PictureBox[] dashboardReductionGraph = { ReductionGraph25, ReductionGraph35, ReductionGraph45, ReductionGraph55, ReductionGraph65, ReductionGraph75 };
            dashboard = new DashBoardPage(this, resourceLoader, dashboardEnhanceGraph, dashboardReductionGraph, TryLabel, SuccessLabel, FailLabel, CoinLabel, itemImages, imageNames, successText);

            Label[] successPercentage = { Detail25, Detail35, Detail45, Detail55, Detail65, Detail75 };
            detailPage = new DetailPage(this, StartDateTimePicker, EndDateTimePicker, successPercentage, DetailGraphPictureBox);

            string path = AppDomain.CurrentDomain.BaseDirectory + @"asl-project-80aca-7ea4b7df82f1.json";
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);
            firestoreDb = FirestoreDb.Create("asl-project-80aca");
        }


        private async void StartLogger()
        {
            // ???? ????
            var version = firestoreDb.Collection("Version").Document("VersionCheck");
            var snap = await version.GetSnapshotAsync();

            if (snap.Exists)
            {
                var systemVer = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                var firebaseVersion = snap.ToDictionary();
                var value = firebaseVersion.Values;
                foreach (var item in value)
                {
                    if (!item.ToString().Equals(systemVer.ToString())) {
                        MessageBox.Show("?????????? ????????. ?????????? ????????????!");
                        System.Diagnostics.Process.Start(new ProcessStartInfo("https://github.com/Heinul/AbilityStoneLogProject/releases") { UseShellExecute = true });
                        return;
                    }
                }
            }

            //?????? ????
            if (Screen.PrimaryScreen.Bounds.Height != 1080 && Screen.PrimaryScreen.Bounds.Height != 1440 && Screen.PrimaryScreen.Bounds.Height != 2160 && Screen.PrimaryScreen.Bounds.Height != 2880)
            {
                MessageBox.Show($"?????? ???????? ???????? 1080, 1440, 2160, 2880?? ???????? ??????????.\n???? ???????? ???????? ???? {Screen.PrimaryScreen.Bounds.Height} ??????.");
                ImageAnalysisState1.Text = "Error : ???????? ???? ????????????!";
            }
            else
            {
                try
                {
                    imageAnalysis = new ImageAnalysis(this, resourceLoader, firestoreDb);
                    ProcessDetector processDetector = new ProcessDetector(this);
                    processDetector.Run();

                    //Test
                    //imageAnalysis = new ImageAnalysis(this, resourceLoader, firestoreDb);
                    //imageAnalysis.Run();

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

        #region ?????? ???? ??????
        public void AddItemToListBox(string engravingNAme, int percentage, bool success)
        {
            dashboard.AddItemToListBox(engravingNAme, percentage, success);
        }
        #endregion

        #region Form ?????? ??????
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        // ?? ????
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
            //TrayIcon.Dispose();
            //System.Diagnostics.Process.GetCurrentProcess().Kill();
            //Application.Exit();
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
            MessageBox.Show("???????? ??????????.");
        }

        private void GraphMouseMove(object sender, EventArgs e)
        {
            dashboard.GraphMouseMove(sender, e, DashboardGraphToolTip);
        }

        public void SetImageAnalysisStateText(string str)
        {
            this.Invoke(new Action(delegate ()
            {
                ImageAnalysisState1.Text = str;
                ImageAnalysisState2.Text = str;
            }));
        }
        #endregion

        #region ????????????

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