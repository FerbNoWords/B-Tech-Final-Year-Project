SELECT 
	PM.Code AS 'Product Code',
    PM.Name AS 'Product Name',
    SM.Name AS 'State Name',
	STD.UnitPrice AS 'Unit Price',
    STD.Quantity AS 'Sales Quantity',
    STD.Amount AS 'Total Amount'
FROM sales_transaction ST
	INNER JOIN sales_transaction_details STD ON ST.ID = STD.SalesID
	INNER JOIN product_master PM ON PM.ID = STD.ProductID
	INNER JOIN state_master SM ON SM.ID = ST.StateID
WHERE monthname(SalesDate) = 'january'
	AND year(SalesDate) = 2019;
    




