SELECT 
	SM.Name AS 'State Name',
    SUM(STD.Amount) AS 'Current Month Sales Count',
    SUM(STDP.Amount) AS 'Previous Month Sales Count'
FROM state_master SM
	INNER JOIN sales_transaction STR ON STR.StateID = SM.ID
	INNER JOIN sales_transaction_details STD ON STD.SalesID = STR.ID
    INNER JOIN sales_transaction STRP ON STRP.StateID = SM.ID
    INNER JOIN sales_transaction_details STDP ON STDP.SalesID = STRP.ID
WHERE MONTH(STR.SalesDate) = MONTH('2023-10-01')
and YEAR(STR.SalesDate) = YEAR('2023-10-01')
and MONTH(STRP.SalesDate) = MONTH('2023-10-01')-1
and YEAR(STRP.SalesDate) = IF(month('2023-10-01') = 1, year('2023-10-01')-1, year('2023-10-01'))
GROUP BY SM.Name;