using Microsoft.UI.Xaml.Data;
using System;

namespace MoneyTracker.WinUI.Utils
{
    public class InverseBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool booleanValue)
            {
                return !booleanValue; // Membalikkan nilai (True jadi False, dst)
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is bool booleanValue)
            {
                return !booleanValue;
            }
            return false;
        }
    }
}
