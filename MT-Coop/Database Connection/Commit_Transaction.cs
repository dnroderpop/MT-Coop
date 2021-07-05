using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MT_Coop.Database_Connection;

namespace MT_Coop.Database_Connection
{
    class Commit_Transaction
    {
        connn Connection = new connn();
        inventoryManager inv = new inventoryManager();

        //transaction Number
        //"C#" + date + clientid + transation number
        // C#2012300101 <- sample
        public String transaction_number(String Date, int Client)
        {
            //20[yyyy]12[mm]30[dd]01[client]01[number of client's  transaction on that day]
            String TransactionNumber = "C#0000000000";
            String year = DateTime.Parse(Date).ToString("yyyy").Substring(2);
            String month = DateTime.Parse(Date).ToString("MM");
            String day = DateTime.Parse(Date).ToString("dd");

            String num = "";
            num = Connection.sql_gettabledata("`transactions`", "Count(`ID`)", " where `Transaction Number` like '%C#" + year + month + day + Client + "%'").Rows[0][0].ToString();

            TransactionNumber = "C#" + year + month + day + Client + num;
            return TransactionNumber;
        }


        //transaction table insert
        public bool Commit_Transaction_Main(string transNumber, int Client, double balance, string date, string duedate, double amount)
        {
            bool result = false;
            date = Connection.Date_to_Mysql(date);
            duedate = Connection.Date_to_Mysql(duedate);
            String values = "'[value-1]',[value-2],[value-6],'[value-3]','[value-4]',[value-5]";
            values = values.Replace("[value-1]", transNumber);
            values = values.Replace("[value-2]", Client + "");
            values = values.Replace("[value-3]", date);
            values = values.Replace("[value-4]", duedate);
            values = values.Replace("[value-5]", amount + "");
            values = values.Replace("[value-6]", balance + "");
            if (Connection.sql_insertdatatable("Transactions", "`Transaction Number`, `Client`,`Balance`, `Date`, `Date due`, `Total Amount`", values))
                result = true;

            return result;
        }

        public bool Commit_Transaction_Details(String transNumber, String Product, String Uom, Double qty, Double Price, DateTime date)
        {
            bool result = false;
            Double amount = qty * Price;
            String values = "'[value-1]','[value-2]','[value-3]',[value-4],[value-5],[value-6]";
            values = values.Replace("[value-1]", transNumber);
            values = values.Replace("[value-2]", Product);
            values = values.Replace("[value-3]", Uom);
            values = values.Replace("[value-4]", qty + "");
            values = values.Replace("[value-5]", Price + "");
            values = values.Replace("[value-6]", amount + "");
            if (Connection.sql_insertdatatable("transaction_details", "`Transaction Number`, `Product`, `Uom`, `Quantity`, `Price`, `Amount`", values))
            {
                result = true;
                String Productid = Connection.Sql_getstringofCustomSQL("products", "id", " where `Product Name` = '" + Product + "'");
                try
                {
                    inv.insertInventory(int.Parse(Productid), qty * -1, date, "Transaction", transNumber);
                }
                catch (Exception)
                {
                }
            }
 
            return result;

        }

        /// <summary>
        /// connection to database that will track the records of payments and deductions
        /// Please Use specific types
        /// </summary>
        /// <param name="Type">"Deduction" if they employee balance is deducted, "Expense" if the employee had another transaction</param>
        /// <param name="Client">Int for client ID</param>
        /// <param name="AddInfo">this may be transaction Numbe or reason of payment</param>
        /// <param name="date"></param>
        /// <param name="amount">Add Negative if its a Deduction</param>
        /// <returns></returns>
        public bool Commit_Balance_logs(String Type, int Client, String AddInfo, String date, Double amount)
        {
            bool result = false;
            date = Connection.Date_to_Mysql(date);
            String values = "'[value-1]',[value-0],'[value-2]','[value-3]',[value-4]";
            values = values.Replace("[value-1]", Type);
            values = values.Replace("[value-0]", Client + "");
            values = values.Replace("[value-2]", AddInfo);
            values = values.Replace("[value-3]", date);
            values = values.Replace("[value-4]", amount + "");

            if (Connection.sql_insertdatatable("balance_logs", "`Type`,`Client`, `Add_Info`, `Date`, `Amount Total`", values))
                result = true;

            if (result)
            {
                double balance;
                balance = double.Parse(Connection.Sql_getstringofCustomSQL("client", "Balance", " where ID = " + Client));

                balance = balance + amount;

                Connection.sql_updatedatafromtable("client", "`Balance`= " + balance, " WHERE ID = " + Client);
            }
            return result;
        }
    }
}
