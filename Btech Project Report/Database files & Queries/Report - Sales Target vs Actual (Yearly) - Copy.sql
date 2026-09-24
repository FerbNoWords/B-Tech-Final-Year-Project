SELECT 
	SM.ID AS 'ID',
    SM.Name AS 'State Name',
    SUM(ST.TargetAmount) AS 'Target Sales',
    SUM(STD.Amount) AS 'Actual Sales',
    (SUM(STD.Amount) - SUM(ST.TargetAmount)) AS 'Difference'
FROM state_master SM
	INNER JOIN sales_target ST ON ST.StateID = SM.ID
    INNER JOIN sales_transaction STR ON STR.StateID = SM.ID
    INNER JOIN sales_transaction_details STD ON STD.SalesID = STR.ID
WHERE ST.Year = 2020
    AND year(STR.SalesDate) = 2020
	GROUP BY SM.ID, SM.Name;

