using System;

namespace MoneyTracker.WinUI.Utils
{
    public static class UnitConverter
    {
        // --- Decimal <-> Double (NumberBox) ---
        public static double DecimalToDouble(decimal value) => (double)value;
        // overloads that match possible generated calls
        public static double DecimalToDouble(object sender, decimal value) => (double)value;
        public static double DecimalToDouble(decimal value, object extra) => (double)value;

        public static decimal DoubleToDecimal(double value) => (decimal)value;
        public static decimal DoubleToDecimal(object sender, double value) => (decimal)value;
        public static decimal DoubleToDecimal(double value, object extra) => (decimal)value;

        // --- DateTime <-> DateTimeOffset? (CalendarDatePicker) ---
        public static DateTimeOffset? DateTimeToOffset(DateTime value)
            => value == default ? (DateTimeOffset?)null : new DateTimeOffset(value);
        public static DateTimeOffset? DateTimeToOffset(object sender, DateTime value)
            => DateTimeToOffset(value);
        public static DateTimeOffset? DateTimeToOffset(DateTime value, object extra)
            => DateTimeToOffset(value);

        public static DateTime OffsetToDateTime(DateTimeOffset? value)
            => value?.DateTime ?? default;
        // IMPORTANT: generated code may call (sender, value) — so provide overload with sender first
        public static DateTime OffsetToDateTime(object sender, DateTimeOffset? value)
            => OffsetToDateTime(value);
        public static DateTime OffsetToDateTime(DateTimeOffset? value, object extra)
            => OffsetToDateTime(value);
    }
}
