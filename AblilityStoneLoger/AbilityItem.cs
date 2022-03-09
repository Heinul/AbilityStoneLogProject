using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Cloud.Firestore;


namespace AbilityStoneLoger
{
    internal class AbilityItem
    {
        private int percentage = 0;
        private string engravingName = "";
        private bool success = false;
        private bool adjustment = false; //true 강화, false 감소
        private int digit = 0;
        SQLite database = null;
        FirestoreDb firestoreDb;


        public AbilityItem(int percentage, string engravingName, bool success, bool adjustment, int digit)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + @"asl-project-80aca-firebase-adminsdk-5wshs-63c1bb844a.json";
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);
            firestoreDb = FirestoreDb.Create("asl-project-80aca");
            this.database = new SQLite();

            this.percentage = percentage;
            this.engravingName = engravingName;
            this.success = success;
            this.adjustment = adjustment;
            this.digit = digit;
        }

        public void SaveData()
        {
            database.Insert(percentage, engravingName, success, adjustment, digit);
        }

        public void SendData()
        {
            
            CollectionReference coll = firestoreDb.Collection("EngravingDataBase");
            Dictionary<string, object> data = new Dictionary<string, object>()
            {
                {"Percentage", percentage},
                {"EngravingName", engravingName},
                {"Success", success },
                {"Adjusment", adjustment},
                {"Digit", digit},
                {"MAC", GetMacAddress() },
                {"Timestamp", Timestamp.FromDateTime(DateTime.UtcNow) }
            };
            coll.AddAsync(data);
            Console.WriteLine("Send To Server");
        }

        private string GetMacAddress()
        {
            return System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces()[0].GetPhysicalAddress().ToString();
        }
    }
}
