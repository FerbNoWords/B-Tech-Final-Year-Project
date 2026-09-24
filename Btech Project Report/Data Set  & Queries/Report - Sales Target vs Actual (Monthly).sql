SELECT 
	SM.ID AS 'ID',
    SM.Name AS 'State Name',
    ST.TargetAmount AS 'Target Sales',
    SUM(STD.Amount) AS 'Actual Sales',
    (SUM(STD.Amount) - ST.TargetAmount) AS 'Difference'
FROM state_master SM
	INNER JOIN sales_target ST ON ST.StateID = SM.ID
    INNER JOIN sales_transaction STR ON STR.StateID = SM.ID
    INNER JOIN sales_transaction_details STD ON STD.SalesID = STR.ID
WHERE ST.MonthName = 'December'
	AND ST.Year = 2020
    AND monthname(STR.SalesDate) = 'December'
    AND year(STR.SalesDate) = 2020
	GROUP BY SM.ID, SM.Name, ST.TargetAmount;


