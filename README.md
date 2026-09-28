You cannot run this project Directly
To run this project follow below steps
Major Steps :
1. Set Up MySQL workbench
2. Set Up Visual Studio 2022

================================================
1. Set Up MySQL workbench
- create schema "reporting_web_app" in mySQL workbench
- You have given .csv files in report folder > databsae files & queries
- create all the required tables and import all the data
- create required primary keys & forgien keys

2. Set Up Visual Studio 2022
- copy "C#/Reporting App API" folder in VS 2022 repos folder
- open project in VS 2022 
- in monthlySalesController.cs file check and update below code
- myConnectionString = "server=127.0.0.1;uid=root;" +
        "pwd=MySql@12345678;database=reporting_web_app";
================================================
Important things before running project
1. Check database is connected to VS 2022 project correctly
2. Test APIs are working properly in swagger
3. Check API URLs in index.html are same in swagger

now you are ready to execute project
