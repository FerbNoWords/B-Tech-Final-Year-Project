namespace Reporting_App_API
{
    public class MonthlySalesReport
    {
        public String? productCode {  get; set; }
        public String? productName { get; set; }
        public String? stateName { get; set; }
        public int unitPrice { get; set; }
        public int salesQuantity { get; set; }
        public int totalAmount { get; set; }
    }
}
