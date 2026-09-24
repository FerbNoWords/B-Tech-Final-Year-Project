using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Tls.Crypto;
using System.Data;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Reporting_App_API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MonthlySalesController : ControllerBase
    {
       
        [HttpPost(Name = "GetMonthlySalesGrowthKPI")]
        public MonthlySalesGrowthKPI Get(ReportParameter param)
        {
            string date = GetFormattedDate(param);

            string myConnectionString;

            myConnectionString = "server=127.0.0.1;uid=root;" +
                    "pwd=MySql@12345678;database=reporting_web_app";
            string sql = "SELECT " +
             " SUM(STD.Amount) AS 'Current Month Sales', " +
             " SUM(STDP.Amount) AS 'Previous Month Sales' " +
             " FROM state_master SM " +
             " INNER JOIN sales_transaction STR ON STR.StateID = SM.ID " +
             " INNER JOIN sales_transaction_details STD ON STD.SalesID = STR.ID " +
             " INNER JOIN sales_transaction STRP ON STRP.StateID = SM.ID " +
             " INNER JOIN sales_transaction_details STDP ON STDP.SalesID = STRP.ID " +
             " WHERE MONTH(STR.SalesDate) = MONTH('" + date + "') " +
             " AND YEAR(STR.SalesDate) = YEAR('" + date + "') " +
             " AND (MONTH(STRP.SalesDate) = IF(MONTH('" + date + "') = 1, 12, MONTH('" + date + "') - 1)) " +
             " AND (YEAR(STRP.SalesDate) = IF(MONTH('" + date + "') = 1, YEAR('" + date + "') - 1, YEAR('" + date + "')));";

            MySqlConnection conn = new MySqlConnection(myConnectionString);
            conn.Open();
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            DataTable dataTable = new DataTable();
            using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
            {
                da.Fill(dataTable);
            }

            conn.Close();
            int CurrentMonthSales = 0;
            int PreviousMonthSales = 0;
            foreach (DataRow row in dataTable.Rows)
            {
                CurrentMonthSales = Convert.ToInt32(row["Current Month Sales"]);
                PreviousMonthSales = Convert.ToInt32(row["Previous Month Sales"]);
            }
            return new MonthlySalesGrowthKPI
            {
                CurrentMonthSales = CurrentMonthSales,
                PreviousMonthSales = PreviousMonthSales,
            };

        }

        private static string GetFormattedDate(ReportParameter param)
        {
            var input = new { monthNumber = param.monthNumber, year = param.year };
            int month = input.monthNumber;
            int year = input.year;
            string date = string.Empty;
            if (month < 1 || month > 12 || year <= 0)
            {
                throw new Exception("Invalid Input");
            }
            else
            {
                date = new DateTime(year, month, 01).ToString("yyyy-MM-dd");
            }

            return date;
        }

        [HttpPost("GetAverageProfitMarginKPI")]
        public AverageProfitMarginKPI GetMargin(ReportParameter param)
        {
            string date = GetFormattedDate(param);

            string myConnectionString;
            myConnectionString = "server=127.0.0.1;uid=root;" +
                   "pwd=MySql@12345678;database=reporting_web_app";
            string sql = "SELECT " +
              " AVG(STD.Amount - (STD.Quantity * PC.cost)) AS 'Current Month Profit Margin', " +
              " AVG(STDP.Amount - (STDP.Quantity * PC.cost)) AS 'Previous Month Profit Margin' " +
              " FROM state_master SM " +
              " INNER JOIN sales_transaction STR ON STR.StateID = SM.ID " +
              " INNER JOIN sales_transaction_details STD ON STD.SalesID = STR.ID " +
              " INNER JOIN sales_transaction STRP ON STRP.StateID = SM.ID " +
              " INNER JOIN sales_transaction_details STDP ON STDP.SalesID = STRP.ID " +
              " INNER JOIN production_cost PC ON PC.ProductID = STD.ProductID " +
              " WHERE MONTH(STR.SalesDate) = MONTH('" + date + "') " +
              " AND YEAR(STR.SalesDate) = YEAR('" + date + "') " +
              " AND (MONTH(STRP.SalesDate) = IF(MONTH('" + date + "') = 1, 12, MONTH('" + date + "') - 1)) " +
              " AND (YEAR(STRP.SalesDate) = IF(MONTH('" + date + "') = 1, YEAR('" + date + "') - 1, YEAR('" + date + "'))) " + 
              " AND PC.MONTH = MONTH('" + date + "') " +
              " AND PC.YEAR = YEAR('" + date + "');";

            MySqlConnection conn = new MySqlConnection(myConnectionString);
            conn.Open();
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            DataTable dataTable = new DataTable();
            using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
            {
                da.Fill(dataTable);
            }

            conn.Close();
            double CurrentMonthProfitMargin = 0;
            double PreviousMonthProfitMargin = 0;
            foreach (DataRow row in dataTable.Rows)
            {
                CurrentMonthProfitMargin = Math.Round(Convert.ToDouble(row["Current Month Profit Margin"]),2);
                PreviousMonthProfitMargin = Math.Round(Convert.ToDouble(row["Previous Month Profit Margin"]),2);
            }
            return new AverageProfitMarginKPI
            {
                CurrentMonthProfitMargin = CurrentMonthProfitMargin,
                PreviousMonthProfitMargin = PreviousMonthProfitMargin,
            };

        }

        [HttpPost("MonthlySalesBookingsKPI")]
        public MonthlySalesBookingsKPI GetBookings(ReportParameter param)
        {
            string date = GetFormattedDate(param);

            string myConnectionString;

            myConnectionString = "server=127.0.0.1;uid=root;" +
                    "pwd=MySql@12345678;database=reporting_web_app";
            string sqlcmb = "SELECT " +
                                " COUNT(STR.ID) AS 'Current Month Bookings' " +
                            " FROM  sales_transaction STR " +
                            " WHERE MONTH(STR.SalesDate) = MONTH('"+date+"') " +
                                " AND YEAR(STR.SalesDate) = YEAR('"+date+"');";
            string sqlpmb = "SELECT " +
                " COUNT(STR.ID) AS 'Previous Month Bookings' " +
                " FROM sales_transaction STR " +
                " WHERE (MONTH(STR.SalesDate) = IF(MONTH('" + date + "') = 1, 12, MONTH('" + date + "') - 1)) " +
                " AND (YEAR(STR.SalesDate) = IF(MONTH('" + date + "') = 1, YEAR('" + date + "') - 1, YEAR('" + date + "')));";


            MySqlConnection conn = new MySqlConnection(myConnectionString);
            conn.Open();
            MySqlCommand cmdcmb = new MySqlCommand(sqlcmb, conn);
            DataTable dataTableCmb = new DataTable();
            using (MySqlDataAdapter da = new MySqlDataAdapter(cmdcmb))
            {
                da.Fill(dataTableCmb);
            }
            MySqlCommand cmdpmb = new MySqlCommand(sqlpmb, conn);
            DataTable dataTablePmb = new DataTable();
            using (MySqlDataAdapter da = new MySqlDataAdapter(cmdpmb))
            {
                da.Fill(dataTablePmb);
            }
            conn.Close();
            int CurrentMonthBookings = 0;
            int PreviousMonthBookings = 0;
            foreach (DataRow row in dataTableCmb.Rows)
            {
                CurrentMonthBookings = Convert.ToInt32(row["Current Month Bookings"]);
                 
            }
            foreach (DataRow row in dataTablePmb.Rows)
            {
                PreviousMonthBookings = Convert.ToInt32(row["Previous Month Bookings"]);
            }
            return new MonthlySalesBookingsKPI
            {
                CurrentMonthBookings = CurrentMonthBookings,
                PreviousMonthBookings = PreviousMonthBookings,
            };

        }

        [HttpPost("GetMonthlySalesStatistics")]
        public IEnumerable<MonthlySalesStatistics> GetStatistics(ReportParameter param)
        {
            string date = GetFormattedDate(param);

            string myConnectionString;
            myConnectionString = "server=127.0.0.1;uid=root;" +
                   "pwd=MySql@12345678;database=reporting_web_app";
            string sql = "SELECT " +
             " SM.Name AS 'State Name', " +
             " SUM(STD.Amount) AS 'Current Month Sales', " +
             " SUM(STDP.Amount) AS 'Previous Month Sales' " +
             " FROM state_master SM " +
             " INNER JOIN sales_transaction STR ON STR.StateID = SM.ID " +
             " INNER JOIN sales_transaction_details STD ON STD.SalesID = STR.ID " +
             " INNER JOIN sales_transaction STRP ON STRP.StateID = SM.ID " +
             " INNER JOIN sales_transaction_details STDP ON STDP.SalesID = STRP.ID " +
             " WHERE MONTH(STR.SalesDate) = MONTH('" + date + "') " +
             " AND YEAR(STR.SalesDate) = YEAR('" + date + "') " +
             " AND (MONTH(STRP.SalesDate) = IF(MONTH('" + date + "') = 1, 12, MONTH('" + date + "') - 1)) " +
             " AND (YEAR(STRP.SalesDate) = IF(MONTH('" + date + "') = 1, YEAR('" + date + "') - 1, YEAR('" + date + "'))) " +
             " GROUP BY SM.Name;";

            MySqlConnection conn = new MySqlConnection(myConnectionString);
            conn.Open();
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            DataTable dataTable = new DataTable();
            using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
            {
                da.Fill(dataTable);
            }

            conn.Close();
            string StateName = "";
            int CurrentMonthSales = 0;
            int PreviousMonthSales = 0;

            List<MonthlySalesStatistics> salesStatistics = new List<MonthlySalesStatistics>();

            foreach (DataRow row in dataTable.Rows)
            {
                MonthlySalesStatistics a = new MonthlySalesStatistics
                {
                    StateName = Convert.ToString(row["State Name"]),
                    CurrentMonthSales = Convert.ToInt32(row["Current Month Sales"]),
                    PreviousMonthSales = Convert.ToInt32(row["Previous Month Sales"])
                };
                salesStatistics.Add(a);
            }
            return salesStatistics;

        }

        [HttpPost("GetAverageTransactionValueStatistics")]
        public IEnumerable<AverageTransactionValueStatistics> GetAverageTransactionValueStatistics(ReportParameter param)
        {
            string date = GetFormattedDate(param);

            string myConnectionString;
            myConnectionString = "server=127.0.0.1;uid=root;" +
                   "pwd=MySql@12345678;database=reporting_web_app";
            string sql = "SELECT " +
              " SM.Name AS 'State Name', " +
              " AVG(STD.Amount) AS 'C.M. AVG. Transaction Value', " +
              " AVG(STDP.Amount) AS 'P.M. AVG. Transaction Value' " +
              " FROM state_master SM " +
              " INNER JOIN sales_transaction STR ON STR.StateID = SM.ID " +
              " INNER JOIN sales_transaction_details STD ON STD.SalesID = STR.ID " +
              " INNER JOIN sales_transaction STRP ON STRP.StateID = SM.ID " +
              " INNER JOIN sales_transaction_details STDP ON STDP.SalesID = STRP.ID " +
              " WHERE MONTH(STR.SalesDate) = MONTH('" + date + "') " +
              " AND YEAR(STR.SalesDate) = YEAR('" + date + "') " +
              " AND (MONTH(STRP.SalesDate) = IF(MONTH('" + date + "') = 1, 12, MONTH('" + date + "') - 1)) " +
              " AND (YEAR(STRP.SalesDate) = IF(MONTH('" + date + "') = 1, YEAR('" + date + "') - 1, YEAR('" + date + "'))) " +
              " GROUP BY SM.Name;";

            MySqlConnection conn = new MySqlConnection(myConnectionString);
            conn.Open();
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            DataTable dataTable = new DataTable();
            using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
            {
                da.Fill(dataTable);
            }

            conn.Close();

            string StateName = string.Empty;
            double CurrMonthAvgTransactionValue = 0;
            double PrevMonthAvgTransactionValue = 0;

            List<AverageTransactionValueStatistics> averageTransactionValue = new List<AverageTransactionValueStatistics>();
            foreach (DataRow row in dataTable.Rows)
            {
                AverageTransactionValueStatistics atv = new AverageTransactionValueStatistics
                {
                    StateName = Convert.ToString(row["State Name"]),
                    CurrMonthAvgTransactionValue = Math.Round(Convert.ToDouble(row["C.M. AVG. Transaction Value"]), 2),
                    PrevMonthAvgTransactionValue = Math.Round(Convert.ToDouble(row["P.M. AVG. Transaction Value"]), 2),
                };
                averageTransactionValue.Add(atv);
            }
            return averageTransactionValue;

        }

        [HttpPost("GetTotalValueOfSalesStatistics")]
        public IEnumerable<TotalValueOfSalesStatistics> GetTotalValueOfSalesStatistics(ReportParameter param)
        {
            string date = GetFormattedDate(param);

            string myConnectionString;
            myConnectionString = "server=127.0.0.1;uid=root;" +
                   "pwd=MySql@12345678;database=reporting_web_app";
            string sql = "SELECT " +
             " SM.Name AS 'State Name', " +
             " SUM(STD.Amount) AS 'Current Month Sales Count', " +
             " SUM(STDP.Amount) AS 'Previous Month Sales Count' " +
             " FROM state_master SM " +
             " INNER JOIN sales_transaction STR ON STR.StateID = SM.ID " +
             " INNER JOIN sales_transaction_details STD ON STD.SalesID = STR.ID " +
             " INNER JOIN sales_transaction STRP ON STRP.StateID = SM.ID " +
             " INNER JOIN sales_transaction_details STDP ON STDP.SalesID = STRP.ID " +
             " WHERE MONTH(STR.SalesDate) = MONTH('" + date + "') " +
             " AND YEAR(STR.SalesDate) = YEAR('" + date + "') " +
             " AND (MONTH(STRP.SalesDate) = IF(MONTH('" + date + "') = 1, 12, MONTH('" + date + "') - 1)) " +
             " AND (YEAR(STRP.SalesDate) = IF(MONTH('" + date + "') = 1, YEAR('" + date + "') - 1, YEAR('" + date + "'))) " +
             " GROUP BY SM.Name;";

            MySqlConnection conn = new MySqlConnection(myConnectionString);
            conn.Open();
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            DataTable dataTable = new DataTable();
            using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
            {
                da.Fill(dataTable);
            }

            conn.Close();

            string StateName = string.Empty;
            int CurrMonthSalesCount = 0;
            int PrevMonthSalesCount = 0;

        List<TotalValueOfSalesStatistics> totalValueOfSales = new List<TotalValueOfSalesStatistics>();
        foreach (DataRow row in dataTable.Rows)
        {
            TotalValueOfSalesStatistics tvos = new TotalValueOfSalesStatistics
            {
                StateName = Convert.ToString(row["State Name"]),
                CurrMonthSalesCount = Convert.ToInt32(row["Current Month Sales Count"]),
                PrevMonthSalesCount = Convert.ToInt32(row["Previous Month Sales Count"]),
            };
            totalValueOfSales.Add(tvos);
        }
            return totalValueOfSales;

        }
        
        [HttpPost("GetSalesAsPerProductQuantity")]
        public IEnumerable<MonthlySalesReport> GetDetailedReport(ReportParameter param)
        {
            string monthName, yearno;
            GetMonthAndYear(param, out monthName, out yearno);

            string myConnectionString;
            myConnectionString = "server=127.0.0.1;uid=root;" +
                   "pwd=MySql@12345678;database=reporting_web_app ";
            string sql = "SELECT " +
             " PM.Code AS 'Product Code', " +
             " PM.Name AS 'Product Name', " +
             " SM.Name AS 'State Name', " +
             " STD.UnitPrice AS 'Unit Price', " +
             " STD.Quantity AS 'Sales Quantity', " +
             " STD.Amount AS 'Total Amount' " +
             " FROM sales_transaction ST " +
             " INNER JOIN sales_transaction_details STD ON ST.ID = STD.SalesID " +
             " INNER JOIN product_master PM ON PM.ID = STD.ProductID " +
             " INNER JOIN state_master SM ON SM.ID = ST.StateID " +
             " WHERE MONTHNAME(ST.SalesDate) = '" + monthName + "' " +
             " AND YEAR(ST.SalesDate) = " + yearno + ";";

            MySqlConnection conn = new MySqlConnection(myConnectionString);
            conn.Open();
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            DataTable dataTable = new DataTable();
            using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
            {
                da.Fill(dataTable);
            }

            conn.Close();

            string productCode = string.Empty;
            string productName = string.Empty;
            string stateName = string.Empty;
            int unitPrice = 0;
            int salesQuantity = 0;
            int totalAmount = 0;

            List<MonthlySalesReport> monthlySalesReports = new List<MonthlySalesReport>();
            foreach (DataRow row in dataTable.Rows)
            {
                MonthlySalesReport b = new MonthlySalesReport
                {
                    productCode = Convert.ToString(row["Product Code"]),
                    productName = Convert.ToString(row["Product Name"]),
                    stateName = Convert.ToString(row["State Name"]),
                    unitPrice = Convert.ToInt32(row["Unit Price"]),
                    salesQuantity = Convert.ToInt32(row["Sales Quantity"]),
                    totalAmount = Convert.ToInt32(row["Total Amount"]),
                };
                monthlySalesReports.Add(b);
            }
            return monthlySalesReports;
        }

        private static void GetMonthAndYear(ReportParameter param, out string monthName, out string yearno)
        {
            var input = new { monthNumber = param.monthNumber, year = param.year };
            int month = input.monthNumber;
            int year = input.year;
            monthName = string.Empty;
            yearno = string.Empty;
            if (month < 1 || month > 12 || year <= 0)
            {
                throw new Exception("Invalid Input");
            }
            else
            {
                monthName = new DateTime(1, month, 1).ToString("MMMM");
                yearno = new DateTime(year, month, 01).ToString("yyyy");
            }
        }

        [HttpPost("GetSalesRegisterReport")]
        public IEnumerable<SalesRegisterReport> GetRegisterDetailedReport(ReportParameter param)
        {
            string monthName, yearno;
            GetMonthAndYear(param, out monthName, out yearno);

            string myConnectionString;
            myConnectionString = "server=127.0.0.1;uid=root;" +
                   "pwd=MySql@12345678;database=reporting_web_app ";
            string sql = "SELECT " +
             " ST.ID AS 'Sales ID', " +
             " CM.Name AS 'Customer Name', " +
             " SM.Name AS 'State Name', " +
             " SUM(STD.Amount) AS 'Sales Amount' " +
             " FROM sales_transaction ST " +
             " INNER JOIN sales_transaction_details STD ON ST.ID = STD.SalesID " +
             " INNER JOIN customer_master CM ON CM.ID = ST.CustomerID " +
             " INNER JOIN state_master SM ON SM.ID = ST.StateID " +
             " WHERE MONTHNAME(ST.SalesDate) = '" + monthName + "' " +
             " AND YEAR(ST.SalesDate) = " + yearno + " " +
             " GROUP BY CM.Name, SM.Name, ST.ID;";

            MySqlConnection conn = new MySqlConnection(myConnectionString);
            conn.Open();
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            DataTable dataTable = new DataTable();
            using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
            {
                da.Fill(dataTable);
            }

            conn.Close();

            int SalesID = 0;
            string CustomerName = string.Empty;
            string StateName = string.Empty;
            int SalesAmount = 0;

            List<SalesRegisterReport> salesRegisterReports = new List<SalesRegisterReport>();
            foreach (DataRow row in dataTable.Rows)
            {
                SalesRegisterReport sr = new SalesRegisterReport
                {
                    SalesID = Convert.ToInt32(row["Sales ID"]),
                    CustomerName = Convert.ToString(row["Customer Name"]),
                    StateName = Convert.ToString(row["State Name"]),
                    SalesAmount = Convert.ToInt32(row["Sales Amount"]),
                };
                salesRegisterReports.Add(sr);
            }
            return salesRegisterReports;
        }

        [HttpPost("GetSalesTargetVSActualMonthly")]
        public IEnumerable<SalesTargetVSActualMonthlyReport> GetSalesTargetVSActualMonthlyReport(ReportParameter param)
        {
            string monthName, yearno;
            GetMonthAndYear(param, out monthName, out yearno);

            string myConnectionString;
            myConnectionString = "server=127.0.0.1;uid=root;" +
                   "pwd=MySql@12345678;database=reporting_web_app ";
            string sql = "SELECT "+
                        " SM.ID AS 'ID', "+
                        " SM.Name AS 'State Name', "+
                        " ST.TargetAmount AS 'Target Sales', "+
                        " SUM(STD.Amount) AS 'Actual Sales', "+
                        " (SUM(STD.Amount) - ST.TargetAmount) AS 'Difference' "+
                    " FROM state_master SM "+
                        " INNER JOIN sales_target ST ON ST.StateID = SM.ID "+
                        " INNER JOIN sales_transaction STR ON STR.StateID = SM.ID "+
                        " INNER JOIN sales_transaction_details STD ON STD.SalesID = STR.ID "+
                    " WHERE ST.MonthName = '" + monthName + "' "+
                        " AND ST.Year = " + yearno + " "+
                        " AND monthname(STR.SalesDate) = '" + monthName + "' "+
                        " AND year(STR.SalesDate) = " + yearno + " "+
                        " GROUP BY SM.ID, SM.Name, ST.TargetAmount;";
            MySqlConnection conn = new MySqlConnection(myConnectionString);
            conn.Open();
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            DataTable dataTable = new DataTable();
            using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
            {
                da.Fill(dataTable);
            }

            conn.Close();

            int ID = 0;
            string StateName = string.Empty;
            int TargetSales = 0;
            int ActualSales = 0;
            int Difference = 0;

            List<SalesTargetVSActualMonthlyReport> salesTargetVSActualMonthlyReports = new List<SalesTargetVSActualMonthlyReport>();
            foreach (DataRow row in dataTable.Rows)
            {
                SalesTargetVSActualMonthlyReport stvam = new SalesTargetVSActualMonthlyReport
                {
                    ID = Convert.ToInt32(row["ID"]),
                    StateName = Convert.ToString(row["State Name"]),
                    TargetSales = Convert.ToInt32(row["Target Sales"]),
                    ActualSales = Convert.ToInt32(row["Actual Sales"]),
                    Difference = Convert.ToInt32(row["Difference"]),
                };
                salesTargetVSActualMonthlyReports.Add(stvam);
            }
            return salesTargetVSActualMonthlyReports;
        }

        [HttpGet("GetSalesTargetVSActualYearly")]
        public IEnumerable<SalesTargetVSActualMonthlyReport> GetSalesTargetVSActualYearlyReport(int Year)
        {
            var input = new {  year = Year };
            int year = input.year;
            
            if (year <= 0)
            {
                throw new Exception("Invalid Input");
            }
            else
            {
                year = Year;
            }

            string myConnectionString;
            myConnectionString = "server=127.0.0.1;uid=root;" +
                   "pwd=MySql@12345678;database=reporting_web_app ";
            string sql = "SELECT " +
                        " SM.ID AS 'ID', " +
                        " SM.Name AS 'State Name', " +
                        " SUM(ST.TargetAmount) AS 'Target Sales', " +
                        " SUM(STD.Amount) AS 'Actual Sales', " +
                        " (SUM(STD.Amount) - SUM(ST.TargetAmount)) AS 'Difference' " +
                    " FROM state_master SM " +
                        " INNER JOIN sales_target ST ON ST.StateID = SM.ID " +
                        " INNER JOIN sales_transaction STR ON STR.StateID = SM.ID " +
                        " INNER JOIN sales_transaction_details STD ON STD.SalesID = STR.ID " +
                    " WHERE ST.Year = " + year + " " +
                        " AND year(STR.SalesDate) = " + year + " " +
                        " GROUP BY SM.ID, SM.Name;";
            MySqlConnection conn = new MySqlConnection(myConnectionString);
            conn.Open();
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            DataTable dataTable = new DataTable();
            using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
            {
                da.Fill(dataTable);
            }

            conn.Close();

            int ID = 0;
            string StateName = string.Empty;
            int TargetSales = 0;
            int ActualSales = 0;
            int Difference = 0;

            List<SalesTargetVSActualMonthlyReport> salesTargetVSActualYearlyReports = new List<SalesTargetVSActualMonthlyReport>();
            foreach (DataRow row in dataTable.Rows)
            {
                SalesTargetVSActualMonthlyReport stvay = new SalesTargetVSActualMonthlyReport
                {

                    ID = Convert.ToInt32(row["ID"]),
                    StateName = Convert.ToString(row["State Name"]),
                    TargetSales = Convert.ToInt32(row["Target Sales"]),
                    ActualSales = Convert.ToInt32(row["Actual Sales"]),
                    Difference = Convert.ToInt32(row["Difference"]),
                };
                salesTargetVSActualYearlyReports.Add(stvay);
            }
            return salesTargetVSActualYearlyReports;
        }

        [HttpPost("GetUserLogin")]
        public List<UserLogin> Getlogin(ReportParameter param)
        {

            string myConnectionString;

            myConnectionString = "server=127.0.0.1;uid=root;" +
                    "pwd=MySql@12345678;database=reporting_web_app";
            string sql = "SELECT " +
                            "um.Email AS 'Email'," +
                            "um.Password AS 'Password'" +
                         "FROM user_master um;";

            MySqlConnection conn = new MySqlConnection(myConnectionString);
            conn.Open();
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            DataTable dataTable = new DataTable();
            using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
            {
                da.Fill(dataTable);
            }

            conn.Close();
            string email = string.Empty;
            string password = string.Empty;

            List<UserLogin> userLogins = new List<UserLogin>();

            foreach (DataRow row in dataTable.Rows)
            {
                UserLogin u = new UserLogin
                {
                    email = Convert.ToString(row["Email"]),
                    password = Convert.ToString(row["Password"]),
                };
                userLogins.Add(u);
            }
            return userLogins;

        }

        [HttpPost("RegisterUser")]
        public IActionResult RegisterUser([FromBody] UserSignup user)
        {
            string myConnectionString = "server=127.0.0.1;uid=root;" +
                                         "pwd=MySql@12345678;database=reporting_web_app";

            using (MySqlConnection conn = new MySqlConnection(myConnectionString))
            {
                conn.Open();

        
                string checkSql = "SELECT COUNT(*) FROM user_master WHERE Email = @Email";
                MySqlCommand checkCmd = new MySqlCommand(checkSql, conn);
                checkCmd.Parameters.AddWithValue("@Email", user.Email);

                int exists = Convert.ToInt32(checkCmd.ExecuteScalar());
                if (exists > 0)
                {
                    return Conflict(new { message = "Email already registered" });
                }

                int newId = 1;
                int newUserRoleId = 1;

                string getMaxSql = "SELECT MAX(ID) AS MaxID, MAX(UserRoleID) AS MaxRoleID FROM user_master";
                MySqlCommand maxCmd = new MySqlCommand(getMaxSql, conn);

                using (MySqlDataReader reader = maxCmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        if (!reader.IsDBNull(0))
                            newId = reader.GetInt32("MaxID") + 1;

                        if (!reader.IsDBNull(1))
                            newUserRoleId = reader.GetInt32("MaxRoleID") + 1;
                    }
                }

                // 3. Insert new user
                string insertSql = @"
            INSERT INTO user_master 
            (ID, Name, Email, Phone, Password, UserRoleID, CreatedBy, CreatedOn, ModifiedBy, ModifiedOn, IsActive)
            VALUES
            (@ID, @Name, @Email, @Phone, @Password, @UserRoleID, @CreatedBy, @CreatedOn, @ModifiedBy, @ModifiedOn, @IsActive)";

                MySqlCommand insertCmd = new MySqlCommand(insertSql, conn);
                insertCmd.Parameters.AddWithValue("@ID", newId);
                insertCmd.Parameters.AddWithValue("@Name", user.Name);
                insertCmd.Parameters.AddWithValue("@Email", user.Email);
                insertCmd.Parameters.AddWithValue("@Phone", user.Phone);
                insertCmd.Parameters.AddWithValue("@Password", user.Password); // 🔐 hash later
                insertCmd.Parameters.AddWithValue("@UserRoleID", newUserRoleId);
                insertCmd.Parameters.AddWithValue("@CreatedBy", 1);
                insertCmd.Parameters.AddWithValue("@CreatedOn", DateTime.Parse("2024-10-14 11:15:45"));
                insertCmd.Parameters.AddWithValue("@ModifiedBy", 1);
                insertCmd.Parameters.AddWithValue("@ModifiedOn", DateTime.Parse("2024-10-14 11:15:45"));
                insertCmd.Parameters.AddWithValue("@IsActive", 1);

                int rowsAffected = insertCmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    return Ok(new { message = "User registered successfully", userId = newId });
                }
                else
                {
                    return StatusCode(500, new { message = "Failed to register user" });
                }
            }
        }




    }
}
