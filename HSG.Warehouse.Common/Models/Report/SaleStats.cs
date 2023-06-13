namespace HSG.Warehouse.Common.Models.Report
{
    public class SaleStats
    {
        public int Year { get; set; }
        public byte Month { get; set; }
        public int SaleCount { get; set; }
        public double SaleSum { get; set; }
        public int CurrencyId { get; set; }
        public string? CurrencyShortName { get; set; }

    }
}
