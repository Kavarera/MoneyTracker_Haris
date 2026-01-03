using Microsoft.UI.Xaml.Data;
using System;
namespace MoneyTracker.WinUI.Utils
{
    public class DateTimeToFriendlyStr : IValueConverter
    {
        public object Convert(object v, Type t, object p, string l) =>
            v is DateTime dt ? dt.ToString("MMM dd, yyyy") : "";
        public object ConvertBack(object v, Type t, object p, string l) => throw new NotImplementedException();
    }
}
