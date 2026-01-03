using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI;
using System;

namespace MoneyTracker.WinUI.Utils
{
    public class TransactionUiConverter : IValueConverter
    {
        public object Convert(object v, Type t, object p, string l)
        {
            decimal val = (decimal)v;
            string type = p as string; // Parameter: "Icon" atau "Color"

            if (type == "Icon")
                return val > 0 ? "\uE70D" : "\uE70E"; // Panah serong atas (Kredit) vs bawah (Debit)

            return val > 0 ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Red);
        }
        public object ConvertBack(object v, Type t, object p, string l) => throw new NotImplementedException();
    }
}
