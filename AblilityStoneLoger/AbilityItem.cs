using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbilityStoneLoger
{
    internal class AbilityItem
    {
        private int percentage = 0;
        private string engravingName = "";
        private bool success = false;
        private bool adjustment = false; //true 강화, false 감소
        SQLite database = null;

        public AbilityItem(int percentage, string engravingName, bool success, bool adjustment)
        {
            this.percentage = percentage;
            this.engravingName = engravingName;
            this.success = success;
            this.adjustment = adjustment;
            this.database = new SQLite();
        }

        public void SaveData()
        {
            database.Insert(percentage, engravingName, success, adjustment);
        }

        public void SendData()
        {

        }
    }
}
