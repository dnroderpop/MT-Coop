using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using MySql.Data.MySqlClient;

namespace MT_Coop.Database_Connection
{
    public class connn
    {
        getinfo getinfo = new getinfo();
        MySqlConnection mysqlcon;
        MySqlCommand command;
        MySqlDataAdapter dataadapter;
        MySqlDataReader datareader;
        String MysqlConnectionString = "server=" + Properties.Settings.Default.serverdb.ToString() +
                    ";uid=" + Properties.Settings.Default.userdb +
                    ";pwd=" + Properties.Settings.Default.passdb +
                    ";database=" + Properties.Settings.Default.namedb;
        private bool resultbol = false;


        public bool check()
        {
            resultbol = false;
            DataTable data = sql_gettabledata("checking", "*", "");
            if (data != null)
            {
                foreach (DataRow row in data.Rows)
                {
                    if (getinfo.GetMacAddress().ToLower().Equals(row[0].ToString(), StringComparison.CurrentCultureIgnoreCase) && Properties.Settings.Default.licenseKey.Equals(row[1].ToString()))
                    {
                        resultbol = true;
                    }
                }
            }
            else
            {
                resultbol = false;
            }
            return resultbol;
        }
        public string get_user_name()
        {
            String id = Properties.Settings.Default.userid;
            DataTable data = sql_gettabledata("user", "fn,ln,utype", "where able = 1 and uname = '" + id + "'");
            return data.Rows[0][1] + ", " + data.Rows[0][0] + " (" + data.Rows[0][2] + ")";
        }

        public Boolean showmessage(String message, MessageBoxButton btntype)
        {
            bool bol = false;

            var result = MessageBox.Show(message, "Hoop Coop System", btntype);

            switch (result)
            {
                case MessageBoxResult.OK:
                    bol = true;
                    break;

                case MessageBoxResult.Yes:
                    bol = true;
                    break;
            }

            return bol;
        }


        //refresh mysql Connection String
        private void refreshconstring()
        {
            MysqlConnectionString = "server=" + Properties.Settings.Default.serverdb.ToString() +
                    ";uid=" + Properties.Settings.Default.userdb +
                    ";pwd=" + Properties.Settings.Default.passdb +
                    ";database=" + Properties.Settings.Default.namedb;
        }

        //Mysql Command that retrieve data from table
        public DataTable sql_gettabledata(String tablename, String tableColumn, String whereStatement)
        {
            refreshconstring();
            DataTable returndata = new DataTable();
            mysqlcon = new MySqlConnection(this.MysqlConnectionString);

            try
            {
                mysqlcon.Open();
                String querystring = "SELECT @tablecolumn FROM @tablename @statement";

                querystring = querystring.Replace("@tablecolumn", tableColumn);
                querystring = querystring.Replace("@tablename", tablename);
                querystring = querystring.Replace("@statement", whereStatement);

                command = new MySqlCommand(querystring, mysqlcon);
                dataadapter = new MySqlDataAdapter(command);
                dataadapter.Fill(returndata);

                mysqlcon.Close();

            }
            catch (Exception ex)
            {
                Debug.Write(ex.Message);
                mysqlcon.Close();
                return null;
            }
            return returndata;
        }

        public DataTable sql_Customgettabledata(String sqlstate)
        {
            refreshconstring();
            DataTable returndata = new DataTable();
            mysqlcon = new MySqlConnection(this.MysqlConnectionString);

            try
            {
                mysqlcon.Open();
                String querystring = sqlstate;

                command = new MySqlCommand(querystring, mysqlcon);
                dataadapter = new MySqlDataAdapter(command);
                dataadapter.Fill(returndata);

                mysqlcon.Close();

            }
            catch (Exception ex)
            {
                Debug.Write(ex.Message);
                mysqlcon.Close();
                return null;
            }
            return returndata;
        }

        //Mysql Command that insert data to table
        public bool sql_insertdatatable(string tablename, string tableColumn, string values)
        {
            refreshconstring();
            bool returndata = false;
            mysqlcon = new MySqlConnection(this.MysqlConnectionString);

            try
            {
                mysqlcon.Open();
                String querystring = "INSERT INTO  @tablename (@tableColumn) VALUES (@values)";


                querystring = querystring.Replace("@tablename", tablename);
                querystring = querystring.Replace("@tableColumn", tableColumn);
                querystring = querystring.Replace("@values", values);

                command = new MySqlCommand(querystring, mysqlcon);
                int result = command.ExecuteNonQuery();
                if (result != 0)
                {
                    returndata = true;
                }

                mysqlcon.Close();
            }
            catch (Exception ex)
            {
                Debug.Write(ex.Message);
                mysqlcon.Close();
                return false;
            }
            return returndata;

        }

        //Mysql Command that removes items from table
        public bool sql_removeitemfromtable(string tablename, string where)
        {
            refreshconstring();
            bool returndata = false;
            mysqlcon = new MySqlConnection(this.MysqlConnectionString);

            try
            {
                mysqlcon.Open();
                String querystring = "update `" + tablename + "` set able = 0 " + where;

                command = new MySqlCommand(querystring, mysqlcon);
                int result = command.ExecuteNonQuery();
                if (result != 0)
                {
                    returndata = true;
                }
                mysqlcon.Close();

            }
            catch (Exception ex)
            {
                Debug.Write(ex.Message);
                mysqlcon.Close();
                return false;
            }
            return returndata;

        }
        //Mysql Command that permanently_delete
        public bool sql_superdelete(string tablename, string where)
        {
            refreshconstring();
            bool returndata = false;
            mysqlcon = new MySqlConnection(this.MysqlConnectionString);

            try
            {
                mysqlcon.Open();
                String querystring = "delete from `" + tablename + "` " + where;

                command = new MySqlCommand(querystring, mysqlcon);
                int result = command.ExecuteNonQuery();
                if (result != 0)
                {
                    returndata = true;
                }

                mysqlcon.Close();
            }
            catch (Exception ex)
            {
                Debug.Write(ex.Message);
                mysqlcon.Close();
                return false;
            }
            return returndata;

        }
        //Mysql Command that update data
        public bool sql_updatedatafromtable(string tablename, string updatesetval, string where)
        {
            refreshconstring();
            bool returndata = false;
            mysqlcon = new MySqlConnection(this.MysqlConnectionString);

            try
            {
                mysqlcon.Open();
                String querystring = "update `" + tablename + "` set " + updatesetval + " " + where;

                command = new MySqlCommand(querystring, mysqlcon);

                int result = command.ExecuteNonQuery();
                if (result != 0)
                {
                    returndata = true;
                }

                mysqlcon.Close();
            }
            catch (Exception ex)
            {
                Debug.Write(ex.Message);
                mysqlcon.Close();
                return false;
            }
            return returndata;

        }
        //Mysql Command custom no data
        public bool sql_CustomQuery_Nodata(string querystrings)
        {
            refreshconstring();
            bool returndata = false;
            mysqlcon = new MySqlConnection(this.MysqlConnectionString);

            try
            {
                mysqlcon.Open();
                String querystring = querystrings;

                command = new MySqlCommand(querystring, mysqlcon);
                int result = command.ExecuteNonQuery();
                if (result != 0)
                {
                    returndata = true;
                }

                mysqlcon.Close();
            }
            catch (Exception ex)
            {
                Debug.Write(ex.Message);
                mysqlcon.Close();
                return false;
            }
            return returndata;

        }
        //Mysql Command custom with data (datatable)
        //Mysql Command Get table with data (dataset)
        public DataSet Sql_gettabledataset(String tablename, String tableColumn, String whereStatement)
        {
            refreshconstring();
            DataSet returndata = new DataSet();
            mysqlcon = new MySqlConnection(this.MysqlConnectionString);

            try
            {
                mysqlcon.Open();
                String querystring = "SELECT @tablecolumn FROM @tablename @statement";

                querystring = querystring.Replace("@tablecolumn", tableColumn);
                querystring = querystring.Replace("@tablename", tablename);
                querystring = querystring.Replace("@statement", whereStatement);

                command = new MySqlCommand(querystring, mysqlcon);
                dataadapter = new MySqlDataAdapter(command);
                dataadapter.Fill(returndata, "Table");

                mysqlcon.Close();

            }
            catch (Exception ex)
            {
                Debug.Write(ex.Message);
                mysqlcon.Close();
                return null;
            }
            return returndata;
        }
        //Mysql Command custom with data [single item (string)]
        public String Sql_getstringofCustomSQL(String tablename, String tableColumn, String whereStatement)
        {
            string returndata = "";
            DataTable data = sql_gettabledata(tablename, tableColumn, whereStatement);
            try
            {
                returndata = data.Rows[0][0].ToString();
            }
            catch (Exception)
            {
                returndata = "";
            }

            return returndata;
        }
        //Mysql Command count datatable
        private int checkRowCount(string tbname, string statement)
        {
            int row_count = 0;

            DataTable data = sql_gettabledata(tbname, "*", statement);

            row_count = data.Rows.Count;

            return row_count;
        }

        //encripty
        public string MD5Hash(string text)
        {
            MD5 md5 = new MD5CryptoServiceProvider();

            //compute hash from the bytes of text  
            md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(text));

            //get hash result after compute it  
            byte[] result = md5.Hash;

            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                //change it into 2 hexadecimal digits  
                //for each byte  
                strBuilder.Append(result[i].ToString("x2"));
            }

            return strBuilder.ToString();
        }

        public string Date_to_Mysql(String validdate)
        {
            string date = "";
            try
            {
                date = DateTime.Parse(validdate).ToString("yyyy-MM-dd");
            }
            catch (Exception)
            {
                showmessage("Invalid Date " + date, MessageBoxButton.OK);
                date = null;
            }

            return date;
        }

        public bool sql_CheckStringInTable(string StringToFind, String Table, string column)
        {
            bool result = false;

            if(Sql_getstringofCustomSQL(Table, column, " where " + column + " = '" + StringToFind + "'") != "") {
                result = true;
            }

            return result;
        }
    }
}
public class getinfo
{

    public static String GetMacAddress()
    {
        foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
        {
            // Only consider Ethernet network interfaces
            if (nic.NetworkInterfaceType == NetworkInterfaceType.Ethernet &&
                nic.OperationalStatus == OperationalStatus.Up)
            {
                return nic.GetPhysicalAddress().ToString();
            }
        }
        return null;
    }

    public static String GetComputerName()
    {
        return Environment.MachineName;
    }

}
