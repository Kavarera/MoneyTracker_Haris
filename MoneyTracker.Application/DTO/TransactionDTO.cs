
namespace MoneyTracker.Application.DTO
{
    public sealed class TransactionDTO
    {
        public int Id { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Description { get; set; }
        public decimal Kredit { get; set; }
        public decimal Debit { get; set; }
        public decimal LastBalance { get; set; }
        public string CategoryName { get; set; }
        public int CategoryId { get; set; }
        public string Status { get; set; }

        // KONVERSI TANGGAL
        // CalendarDatePicker.Date adalah DateTimeOffset?, maka input/output harus konsisten
        //public DateTimeOffset? DateTimeToOffset(DateTime dt)
        //{
        //    return new DateTimeOffset(dt);
        //}

        //public DateTime OffsetToDateTime(DateTimeOffset? dto)
        //{
        //    // Jika user mengosongkan tanggal, kita beri default DateTime.Now atau dt asal
        //    return dto.HasValue ? dto.Value.DateTime : DateTime.Now;
        //}

        //// KONVERSI ANGKA
        //// NumberBox.Value adalah double (bukan double?), maka parameter harus double
        //public double DecimalToDouble(decimal val) => (double)val;

        //public decimal DoubleToDecimal(double val) => (decimal)val;
    }
}
