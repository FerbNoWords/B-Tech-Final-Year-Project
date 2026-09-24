SELECT month(curdate())-1;
SELECT curdate() AS 'Current Date',
	month(curdate()) AS 'Current Month',
    month(curdate())-1 AS 'Previous Month',
    year(curdate()) AS 'Current Year',
    year(curdate()) AS 'Previous Months Year',
    IF(month('2024-01-11') = 1, year(curdate())-1, year(curdate())) AS 'test'