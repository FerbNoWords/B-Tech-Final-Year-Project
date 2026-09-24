SELECT 
	ST.ID AS 'Sales ID',
    CM.Name AS 'Customer Name',
    SM.Name AS 'State Name',
    SUM(STD.Amount) AS 'Sales Amount'
FROM sales_transaction ST 
	INNER JOIN sales_transaction_details STD ON ST.ID = STD.SalesID
    INNER JOIN customer_master CM ON CM.ID = ST.CustomerID
	INNER JOIN state_master SM ON SM.ID = ST.StateID
WHERE monthname(SalesDate) = 'january' 
	AND year(SalesDate) = 2019
    GROUP BY CM.Name,SM.Name,ST.ID;
    
