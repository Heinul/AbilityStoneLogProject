﻿using System;
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
                    sql.AppendLine(@" ""DIGIT"" INTAGER,");
                    sql.AppendLine(@" ""TIMESTAMP"" INTAGER NOT NULL");
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
            }
        }

        public DataRowCollection SelectAll()
        {
            try
            {
                DataSet ds = new DataSet();

                string sql = $"SELECT * FROM ENGRAVINGDATA";
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

        public DataRowCollection Select(string engravingName)
        {
            try
            {
                DataSet ds = new DataSet();

                string sql = $"SELECT * FROM ENGRAVINGDATA WHERE ENGRAVINGNAME = '{engravingName}'";
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

        public DataRowCollection Select(int percentage)
        {
            try
            {
                DataSet ds = new DataSet();

                string sql = $"SELECT * FROM ENGRAVINGDATA WHERE PERCENTAGE = {percentage}";
                adapter = new SQLiteDataAdapter(sql, DBpath);
                adapter.Fill(ds);

                if (ds.Tables.Count > 0) return ds.Tables[0].Rows;
                else return null;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                throw;
            }
        }


        public DataRowCollection Select(DateTime time1, DateTime time2 , int percentage)
        {
            try
            {
                DataSet ds = new DataSet();

                string sql = $"SELECT * FROM ENGRAVINGDATA WHERE TIMESTAMP BETWEEN {time1.Ticks} AND {time2.Ticks} AND PERCENTAGE = {percentage}";
                adapter = new SQLiteDataAdapter(sql, DBpath);
                adapter.Fill(ds);

                if (ds.Tables.Count > 0) return ds.Tables[0].Rows;
                else return null;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                throw;
            }
        }

        public DataRowCollection Select(DateTime time1, DateTime time2, int percentage, bool adjustment)
        {
            try
            {
                DataSet ds = new DataSet();

                string sql = $"SELECT * FROM ENGRAVINGDATA WHERE TIMESTAMP BETWEEN {time1.Ticks} AND {time2.Ticks} AND PERCENTAGE = {percentage} AND ADJUSTMENT = {adjustment}";
                adapter = new SQLiteDataAdapter(sql, DBpath);
                adapter.Fill(ds);

                if (ds.Tables.Count > 0) return ds.Tables[0].Rows;
                else return null;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                throw;
            }
        }

        public DataRowCollection Select(DateTime time1, DateTime time2, int percentage, bool adjustment, bool success)
        {
            try
            {
                DataSet ds = new DataSet();

                string sql = $"SELECT * FROM ENGRAVINGDATA WHERE TIMESTAMP BETWEEN {time1.Ticks} AND {time2.Ticks} AND PERCENTAGE = {percentage} AND ADJUSTMENT = {adjustment} AND SUCCESS = {success}";
                adapter = new SQLiteDataAdapter(sql, DBpath);
                adapter.Fill(ds);

                if (ds.Tables.Count > 0) return ds.Tables[0].Rows;
                else return null;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                throw;
            }
        }

        public DataRowCollection Select(bool success)
        {
            try
            {
                DataSet ds = new DataSet();

                string sql = $"SELECT * FROM ENGRAVINGDATA WHERE SUCCESS = {success}";
                adapter = new SQLiteDataAdapter(sql, DBpath);
                adapter.Fill(ds);

                if (ds.Tables.Count > 0) return ds.Tables[0].Rows;
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

        public void Insert(int percentage, string engravingName, bool success, bool adjustment, int digit)
        {
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(DBpath))
                {
                    conn.Open();
                    string sql = $"INSERT INTO ENGRAVINGDATA('PERCENTAGE', 'ENGRAVINGNAME', 'SUCCESS', 'ADJUSTMENT', 'DIGIT', 'TIMESTAMP') VALUES ({percentage}, '{engravingName}', {success}, {adjustment}, {digit}, {DateTime.Now.Ticks})";
                    SQLiteCommand cmd = new SQLiteCommand(sql, conn);
                    Console.WriteLine(cmd.CommandText);
                    cmd.ExecuteNonQuery();
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
