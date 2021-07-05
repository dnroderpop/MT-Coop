using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace MT_Coop.Database_Connection
{
    public class ValueToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double input;
            try
            {
                DataGridCell dgc = (DataGridCell)value;
                System.Data.DataRowView rowView = (System.Data.DataRowView)dgc.DataContext;
                input = (double)rowView.Row.ItemArray[dgc.Column.DisplayIndex = 6];
            }
            catch (InvalidCastException e)
            {
                return DependencyProperty.UnsetValue;
            }
            if(input < 0)
            {
                return Brushes.HotPink;
            }
            else
            {
                return Brushes.Transparent;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
