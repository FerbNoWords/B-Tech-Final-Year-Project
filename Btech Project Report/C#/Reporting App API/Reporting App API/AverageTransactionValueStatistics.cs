namespace Reporting_App_API
{
    public class AverageTransactionValueStatistics
    {
        public String? StateName { get; set; }
        public double CurrMonthAvgTransactionValue { get; set; }
        public double PrevMonthAvgTransactionValue { get; set; }
    }
}
