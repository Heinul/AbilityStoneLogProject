using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbilityStoneLoger
{
    internal class SQLite
    {
        string DBpath = "Data Source=" + Application.StartupPath + "engraving_database.db";
        SQLiteDataAdapter adapter = null;
        public SQLite()
        {
            CreateTable();
        }

        private void CreateTable()
        {
            using (SQLiteConnection conn = new SQLiteConnection(DBpath))
            {
                
                string tablecheckQuery = @"SELECT COUNT(*) FROM sqlite_master WHERE Name = 'ENGRAVINGDATA'";
                conn.Open();

                SQLiteCommand cmd1 = new SQLiteCommand(tablecheckQuery, conn);
                int resrult = Convert.ToInt32(cmd1.ExecuteScalar());
                if (resrult < 1)
                {
                    StringBuilder sql = new StringBuilder();
                    sql.AppendLine(@"CREATE TABLE ""ENGRAVINGDATA"" (");
                    sql.AppendLine(@" ""PERCENTAGE"" INTAGER, ");
                    sql.AppendLine(@" ""ENGRAVINGNAME"" TEXT,");
                    sql.AppendLine(@" ""SUCCESS"" BOOLEAN,");
                    sql.AppendLine(@" ""ADJUSTMENT"" BOOLEAN,"); //true 강화효과 / false 감소효과
                    sql.AppendLine(@" ""TIMESTAMP"" DATETIME DEFAULT (datetime('now', 'localtime')) NOT NULL");
                    sql.AppendLine(@" ); ");

                    try
                    {
                        SQLiteCommand cmd = new SQLiteCommand(sql.ToString(), conn);
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }

                //Insert(75,"저주받은인형", true, true);
                //Insert(65, "저주받은인형", true, true);
                //Insert(55, "저주받은인형", false, true);
                //Insert(65, "저주받은인형", true, true);
                //Insert(55, "저주받은인형", true, true);
                //Insert(45, "저주받은인형", false, true);
                //Insert(55, "원한", true, true);
                //Insert(45, "원한", true, true);
                //Insert(35, "공격력감소", true, false);
                //Insert(25, "공격력감소", false, false);

                //var a = Select(75);
                //var b = a.Tables[0].Rows;
                //var c = b[0];
                //Console.WriteLine(c[0]);
                //Console.WriteLine(c[1]);
                //Console.WriteLine(c[2]);
                //Console.WriteLine(c[3]);
                //Console.WriteLine(c[4]);
            }
        }

        public DataSet SelectAll(string table)
        {
            try
            {
                DataSet ds = new DataSet();

                string sql = $"SELECT * FROM {table}";
                adapter = new SQLiteDataAdapter(sql, DBpath);
                adapter.Fill(ds, table);

                if (ds.Tables.Count > 0) return ds;
                else return null;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                throw;
            }
        }

        public DataSet SelectDetail(string table, string condition, string where = "")
        {
            try
            {
                DataSet ds = new DataSet();

                string sql = $"SELECT {condition} FROM {table} {where}";
                adapter = new SQLiteDataAdapter(sql, DBpath);
                adapter.Fill(ds, table);

                if (ds.Tables.Count > 0) return ds;
                else return null;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                throw;
            }
        }

        public DataSet Select(string engravingName)
        {
            try
            {
                DataSet ds = new DataSet();

                string sql = $"SELECT * FROM ENGRAVINGDATA WHERE ENGRAVINGNAME = '{engravingName}'";
                adapter = new SQLiteDataAdapter(sql, DBpath);
                adapter.Fill(ds);

                if (ds.Tables.Count > 0) return ds;
                else return null;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                throw;
            }
        }

        public DataSet Select(int percentage)
        {
            try
            {
                DataSet ds = new DataSet();

                string sql = $"SELECT * FROM ENGRAVINGDATA WHERE PERCENTAGE = {percentage}";
                adapter = new SQLiteDataAdapter(sql, DBpath);
                adapter.Fill(ds);

                if (ds.Tables.Count > 0) return ds;
                else return null;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                throw;
            }
        }

        public DataRowCollection Select(int percentage, bool adjustment)
        {
            try
            {
                DataSet ds = new DataSet();

                string sql = $"SELECT * FROM ENGRAVINGDATA WHERE PERCENTAGE = {percentage} AND ADJUSTMENT = {adjustment}";
                adapter = new SQLiteDataAdapter(sql, DBpath);
                adapter.Fill(ds);

                if (ds.Tables.Count > 0)
                    return ds.Tables[0].Rows;
                else 
                    return null;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                throw;
            }
        }

        public DataRowCollection Select(int percentage, bool adjustment, bool success)
        {
            try
            {
                DataSet ds = new DataSet();

                string sql = $"SELECT * FROM ENGRAVINGDATA WHERE PERCENTAGE = {percentage} AND ADJUSTMENT = {adjustment} AND SUCCESS = {success}";
                adapter = new SQLiteDataAdapter(sql, DBpath);
                adapter.Fill(ds);

                if (ds.Tables.Count > 0)
                    return ds.Tables[0].Rows;
                else
                    return null;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                throw;
            }
        }

        public void Insert(int percentage, string engravingName, bool success, bool adjustment)
        {
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(DBpath))
                {
                    conn.Open();
                    string sql = $"INSERT INTO ENGRAVINGDATA('PERCENTAGE', 'ENGRAVINGNAME', 'SUCCESS', 'ADJUSTMENT') VALUES ({percentage}, '{engravingName}', {success}, {adjustment})";
                    SQLiteCommand cmd = new SQLiteCommand(sql, conn);
                    cmd.ExecuteNonQuery();

                    Console.WriteLine(cmd.CommandText);
                }
            }
            catch(Exception e)
            {
                MessageBox.Show(e.ToString());
                throw;
            }
        }

        public void DeleteAll(string table)
        {
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(DBpath))
                {
                    conn.Open();
                    string sql = $"DELETE FROM {table}";
                    SQLiteCommand cmd = new SQLiteCommand(sql, conn);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                throw;
            }
        }

        public void DeleteDetail(string table, string wherecol, string wherevalue)
        {
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(DBpath))
                {
                    conn.Open();
                    string sql = $"DELETE FROM {table} WHERE {wherecol}='{wherevalue}'";
                    SQLiteCommand cmd = new SQLiteCommand(sql, conn);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                throw;
            }
        }
    }
}
