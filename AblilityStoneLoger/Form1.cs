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

        private void Form1_Load(object sender, EventArgs e)
        {
            Init();
            StartLogger();
            UpdateDashboard();
            new Thread(MousePosition).Start();
        }

        ImageAnalysis imageAnalysis;
        Process LostarkProess = null;
        bool lostarkProcessState = false;

        ResourceLoader resourceLoader;
        PictureBox[] itemImages;
        Label[] imageNames;
        Label[] successText;
        int itemCount = 0;

        private void Init()
        {
            resourceLoader = new ResourceLoader();
            itemImages = new PictureBox[]{ ItemImage1, ItemImage2, ItemImage3, ItemImage4, ItemImage5, ItemImage6, ItemImage7};
            imageNames = new Label[] { ImageName1, ImageName2, ImageName3, ImageName4, ImageName5, ImageName6, ImageName7 };
            successText = new Label[] { SuccessText1, SuccessText2, SuccessText3, SuccessText4, SuccessText5, SuccessText6, SuccessText7 };
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
                    imageAnalysis = new ImageAnalysis(this, resourceLoader);
                    ProcessDetector processDetector = new ProcessDetector(this);
                    processDetector.Run();
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
            imageAnalysis.StopTread();
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

        //public void SetLostArkState(bool check)
        //{
        //    lostarkProcessState = check;
        //}

        //public bool GetLostArkState()
        //{
        //    return lostarkProcessState;
        //}

        

        public void AddItemToListBox(string engravingName, int percentage, bool success)
        {
            this.Invoke(new Action(delegate ()
            {
                var image = resourceLoader.GetImageToName(engravingName);
                image = image.Resize(new OpenCvSharp.Size(image.Width / 2, image.Height / 2));
                Bitmap bitmap = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(image);
                
                if (itemCount < 7)
                    itemCount += 1;

                for(int i = itemCount; i > 1; --i)
                {
                    itemImages[i - 1].Image = itemImages[i - 2].Image;
                    imageNames[i - 1].Text = imageNames[i - 2].Text;
                    successText[i - 1].Text = successText[i - 2].Text;
                }

                string successString = success ? "성공" : "실패";

                itemImages[0].Image = bitmap;
                imageNames[0].Text = engravingName;
                successText[0].Text = $"{percentage}% {successString}";



            }));
            
        }
        
        #region Form 관련 함수
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

        private void UpdateDashboard()
        {
            new Thread(() => {
                PictureBox[] enhanceGraph = { EnhanceGraph25, EnhanceGraph35, EnhanceGraph45, EnhanceGraph55, EnhanceGraph65, EnhanceGraph75 };
                PictureBox[] reductionGraph = { ReductionGraph25, ReductionGraph35, ReductionGraph45, ReductionGraph55, ReductionGraph65, ReductionGraph75};
                SQLite db = new SQLite();
                while (true)
                {
                    try
                    {
                        var tryCount = db.SelectAll().Count;
                        var successCount = db.Select(true).Count;
                        this.Invoke(new Action(delegate ()
                        {
                            TryLabel.Text = (tryCount > 1000) ? String.Format("{0:#,0}", ((double)tryCount / 1000)) + "K" : String.Format("{0:#,0}", tryCount);
                            SuccessLabel.Text = (successCount > 1000) ? String.Format("{0:#,0}", ((double)successCount / 1000)) + "K" : String.Format("{0:#,0}", successCount);
                            FailLabel.Text = (tryCount - successCount > 1000) ? String.Format("{0:#,0}", ((double)tryCount - successCount / 1000)) : String.Format("{0:#,0}", (tryCount - successCount));
                            CoinLabel.Text = (tryCount * 1.68 > 1000) ? String.Format("{0:##,##0}", tryCount * 1.68) + "K" : String.Format("{0:##,##0.00}", tryCount * 1.68) + "K";
                        }));

                        for (int i = 0; i < 6; i++)
                        {
                            if (db.Select(25 + (10 * i), true).Count != 0)
                            {
                                var perCount = db.Select(25 + (10 * i), true).Count;
                                var scsCount = db.Select(25 + (10 * i), true, true).Count;
                                var height = 250 * scsCount / perCount;

                                this.Invoke(new Action(delegate ()
                                {
                                    enhanceGraph[i].Height = height;
                                    enhanceGraph[i].Location = new System.Drawing.Point(enhanceGraph[i].Location.X, 456 - height);
                                    tooltipEText[i] = ($"시행횟수 : {perCount}\n성공횟수 : {scsCount}\n실패횟수 : {perCount - scsCount}");
                                }));
                            }

                            if (db.Select(25 + (10 * i), false).Count != 0)
                            {
                                var height = 250 * db.Select(25 + (10 * i), false, true).Count / db.Select(25 + (10 * i), false).Count;

                                this.Invoke(new Action(delegate ()
                                {
                                    var perCount = db.Select(25 + (10 * i), false).Count;
                                    var scsCount = db.Select(25 + (10 * i), false, true).Count;
                                    reductionGraph[i].Height = height;
                                    reductionGraph[i].Location = new System.Drawing.Point(reductionGraph[i].Location.X, 456 - height);
                                    tooltipRText[i] = ($"시행횟수 : {perCount}\n성공횟수 : {scsCount}\n실패횟수 : {perCount - scsCount}");
                                }));
                            }
                        }
                        Thread.Sleep(1000);
                    }catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }).Start();
        }

        string[] tooltipEText = new string[6];
        string[] tooltipRText = new string[6];
        private void Home_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 0;
        }

        private void LogDetail_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 1;
        }

        private void Tendency_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 2;
        }

        private void Option_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 3;
        }

        private void GraphMouseMove(object sender, EventArgs e)
        {
            if (((PictureBox)sender).Name == "EnhanceGraph75")
                SetTooltip(5, true, sender);
            else if (((PictureBox)sender).Name == "EnhanceGraph65")
                SetTooltip(4, true, sender);
            else if (((PictureBox)sender).Name == "EnhanceGraph55")
                SetTooltip(3, true, sender);
            else if (((PictureBox)sender).Name == "EnhanceGraph45")
                SetTooltip(2, true, sender);
            else if (((PictureBox)sender).Name == "EnhanceGraph35")
                SetTooltip(1, true, sender);
            else if (((PictureBox)sender).Name == "EnhanceGraph25")
                SetTooltip(0, true, sender);
            else if (((PictureBox)sender).Name == "ReductionGraph75")
                SetTooltip(5, false, sender);
            else if (((PictureBox)sender).Name == "ReductionGraph65")
                SetTooltip(4, false, sender);
            else if (((PictureBox)sender).Name == "ReductionGraph55")
                SetTooltip(3, false, sender);
            else if (((PictureBox)sender).Name == "ReductionGraph45")
                SetTooltip(2, false, sender);
            else if (((PictureBox)sender).Name == "ReductionGraph35")
                SetTooltip(1, false, sender);
            else if (((PictureBox)sender).Name == "ReductionGraph25")
                SetTooltip(0, false, sender);
        }

        private void SetTooltip(int num, bool adjustment, object sender)
        {
            toolTip1.ToolTipTitle = "상세정보";
            if (adjustment)
                toolTip1.SetToolTip((PictureBox)sender, $"{tooltipEText[num]}");
            else
                toolTip1.SetToolTip((PictureBox)sender, $"{tooltipRText[num]}");
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.GetCurrentProcess().Kill();
            Application.Exit();
        }

        #endregion

        #region 트레이아이콘 관련

        private void LoadTrayIcon(object? sender, EventArgs e)
        {
            TrayIcon.Visible = true;
            TrayIcon.ContextMenuStrip = TrayMenu;
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
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