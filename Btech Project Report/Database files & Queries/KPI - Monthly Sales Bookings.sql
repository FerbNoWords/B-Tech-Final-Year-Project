SELECT 
    COUNT(STR.ID) AS 'Current Month Bookings'
FROM  sales_transaction STR
WHERE MONTH(STR.SalesDate) = MONTH('2023-10-01')
	AND YEAR(STR.SalesDate) = YEAR('2023-10-01');

SELECT 
    COUNT(STR.ID) AS 'Previous Month Bookings'
FROM  sales_transaction STR
WHERE MONTH(STR.SalesDate) = MONTH('2023-10-01')-1
	AND YEAR(STR.SalesDate) = IF(MONTH('2023-10-01') = 1, YEAR('2023-10-01')-1, YEAR('2023-10-01'));

