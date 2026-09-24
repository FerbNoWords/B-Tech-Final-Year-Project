namespace Reporting_App_API
{
    public class SalesTargetVSActualMonthlyReport
    {
        public int ID { get; set; }
        public string StateName { get; set; }
        public int TargetSales { get; set; }
        public int ActualSales { get; set; }
        public int Difference { get; set; }
    }
}
