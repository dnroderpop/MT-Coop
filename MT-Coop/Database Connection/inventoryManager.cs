using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MT_Coop.Database_Connection;

namespace MT_Coop.Database_Connection
{     
    class inventoryManager
    {
        connn connection = new connn();

        /// <summary>
        /// Insert on Inventory
        /// </summary>
        /// <param name="productid"></param>
        /// <param name="qty"></param>
        /// <param name="date"></param>
        /// <param name="from">Transaction or Delivery or Adjustment</param>
        /// <param name="Details">Control Number or drnumber or reason</param>
        public bool insertInventory(int productid, double qty, DateTime date, String from, String Details)
        {
            bool result = false;
            string column = "`Product`, `Flow`, `Date`, `Type`, `Details`";
            string value = "[value-1],[value-2],'[value-3]','[value-4]','[value-5]'";
            value = value.Replace("[value-1]", productid.ToString());
            value = value.Replace("[value-2]", qty.ToString());
            value = value.Replace("[value-3]", connection.Date_to_Mysql(date.ToString()));
            value = value.Replace("[value-4]", from);
            value = value.Replace("[value-5]", Details);
            if (connection.sql_insertdatatable("inventory_logs",column,value))
            {
                result = true;
            }
            else
            {
                connection.showmessage("Something went wrong",System.Windows.MessageBoxButton.OK);
            }

            return result;
        }
    }
}
