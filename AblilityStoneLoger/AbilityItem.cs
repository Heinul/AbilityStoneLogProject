using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;

namespace AbilityStoneLoger
{
    internal class AbilityItem
    {
        private int percentage = 0;
        private string[] engravingName = new string[3];
        private int[][] engravingSuccessData = new int[3][];
        private string path = @"./Data.xlsx";

        private Microsoft.Office.Interop.Excel.Application excel = null;
        private Microsoft.Office.Interop.Excel.Workbook wb = null;
        private Microsoft.Office.Interop.Excel.Worksheet ws = null;

        public AbilityItem(int percentage, string[] engravingName, int[][] engravingSuccessData)
        {
            this.percentage = percentage;
            this.engravingName = engravingName;
            this.engravingSuccessData = engravingSuccessData;
            //Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();
            //wb = excel.Workbooks.Open(path);

        }

        public void SaveData()
        {

        }

        public void SendData()
        {

        }
    }
}
