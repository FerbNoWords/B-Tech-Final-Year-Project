SELECT 
    AVG(STD.Amount - (STD.Quantity * PC.cost)) AS 'Current Month Profit Margin',
    AVG(STDP.Amount - (STDP.Quantity * PC.cost)) AS 'Previous Month Profit Margin'
FROM state_master SM
	INNER JOIN sales_transaction STR ON STR.StateID = SM.ID
	INNER JOIN sales_transaction_details STD ON STD.SalesID = STR.ID
    INNER JOIN sales_transaction STRP ON STRP.StateID = SM.ID
    INNER JOIN sales_transaction_details STDP ON STDP.SalesID = STRP.ID
    INNER JOIN production_cost PC ON PC.ProductID = STD.ProductID
WHERE MONTH(STR.SalesDate) = MONTH('2023-10-01')
and YEAR(STR.SalesDate) = YEAR('2023-10-01')
and MONTH(STRP.SalesDate) = MONTH('2023-10-01')-1
and YEAR(STRP.SalesDate) = IF(month('2023-10-01') = 1, year('2023-10-01')-1, year('2023-10-01'))
and PC.MONTH = MONTH('2023-10-01')
and PC.YEAR = YEAR('2023-10-01');