using AblilityStoneLoger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbilityStoneLoger
{
    internal class DetailPage
    {
        private Form1 form1;
        private bool pageState = false;
        private DateTimePicker startDateTimePicker, endDateTimePicker;

        Label[] detailPercentage;
        TransparentPanel dotGraph;
        public DetailPage(Form1 form1, DateTimePicker startDateTimePicker, DateTimePicker endDateTimePicker, Label[] detailPercentage, TransparentPanel dotGraph)
        {
            this.form1 = form1;
            
            this.startDateTimePicker = startDateTimePicker;
            endDateTimePicker.Value = DateTime.Now;
            this.endDateTimePicker = endDateTimePicker;

            this.detailPercentage = detailPercentage;

            this.dotGraph = dotGraph;

            UpdateDetailPage();
        }

        public void SetPageState(bool state)
        {
            pageState = state;
        }

        int[] dotPosition = { 475, 389, 305, 221, 137, 47 }, ePosition = { 505, 419, 335, 251, 167, 77 }, rPosition = { 537, 451, 367, 283, 199, 110 };

        public void UpdateDetailPage()
        {
            new Thread(() => {
                SQLite db = new SQLite();
                while (pageState)
                {
                    form1.Invoke(new Action(delegate ()
                    {
                        endDateTimePicker.Value = DateTime.Now;
                    }));
                    try
                    {
                        int[] totalDot = new int[6];
                        int[] heightEData = new int[6];
                        int[] heightRData = new int[6];
                        for (int i = 0; i < 6; i++)
                        {
                            var perECount = db.Select(startDateTimePicker.Value, endDateTimePicker.Value, 25 + (10 * i), true).Count;
                            var scsECount = db.Select(startDateTimePicker.Value, endDateTimePicker.Value, 25 + (10 * i), true, true).Count;

                            var perRCount = db.Select(startDateTimePicker.Value, endDateTimePicker.Value, 25 + (10 * i), false).Count;
                            var scsRCount = db.Select(startDateTimePicker.Value, endDateTimePicker.Value, 25 + (10 * i), false, true).Count;

                            var totalTryCount = db.Select(startDateTimePicker.Value, endDateTimePicker.Value, 25 + (10 * i)).Count;
                            var scsTCount = scsECount+ scsRCount;

                            totalDot[i] = totalTryCount > 0 ? (400 * scsTCount / totalTryCount) : 0;
                            heightEData[i] = perECount > 0 ? (400 * scsECount / perECount) : 0;
                            heightRData[i] = perRCount > 0 ? (400 * scsRCount / perRCount) : 0;

                            form1.Invoke(new Action(delegate ()
                            {
                                detailPercentage[i].Text = totalTryCount != 0 ? (100 * scsTCount / (double)totalTryCount).ToString("0.0") + "%" : "0%";
                            }));
                            
                        }

                        Graphics g = dotGraph.CreateGraphics();
                        g.Clear(Color.White);
                        g.Dispose();
                        for (int i = 0; i < 6; ++i)
                        {
                            form1.Invoke(new Action(delegate ()
                            {
                                Graphics g = dotGraph.CreateGraphics();
                                Rectangle eGrp = new Rectangle(ePosition[i] - 56, 415 - heightEData[i], 20, heightEData[i]);
                                g.FillRectangle(Brushes.CornflowerBlue, eGrp);

                                Rectangle rGrp = new Rectangle(rPosition[i] - 56, 415 - heightRData[i], 20, heightRData[i]);
                                g.FillRectangle(Brushes.Tomato, rGrp);

                                Rectangle totalDotRect = new Rectangle(dotPosition[i], 415 - totalDot[i], 5, 5);
                                g.FillRectangle(Brushes.MediumOrchid, totalDotRect);

                                if (i < 5)
                                {
                                    Point s = new Point(dotPosition[i], 415 - totalDot[i]);
                                    Point e = new Point(dotPosition[i + 1], 415 - totalDot[i + 1]);
                                    g.DrawLine(Pens.MediumOrchid, s, e);
                                }

                                g.Dispose();
                            }));
                        }
                        Thread.Sleep(1000);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        pageState = false;
                    }
                }
            }).Start();
        }
    }
}
