1. PROJECT DESCRIPTION
Satora Caffe is an automation system developed with ASP.NET Core MVC architecture that manages table management, takeaway service, inventory tracking, and personnel productivity. The project manages part of its business logic with database-level (SQL Server) objects to ensure data consistency.

2. TECHNOLOGIES USED
Backend: ASP.NET Core MVC (v6.0/7.0)

Database: Microsoft SQL Server (T-SQL)

ORM: Entity Framework Core (Database First)

Frontend: Bootstrap, CSS3, HTML5, JavaScript/AJAX

3. INSTALLATION AND OPERATION INSTRUCTIONS
For the project to run locally, the following 3 main steps must be followed in order:

Step 1: Restoring the Database
Open SQL Server Management Studio (SSMS).

Right-click on the Databases folder on the left and select Restore Database...

In the Source section, select Device and click the button (...) next to it.

In the window that opens, click Add and select the SatoraCaffeRestaurant_Backup.bak file located in the project folder.

Click OK on all windows to load the database, along with all its tables, triggers, views, and sample data.

Step 2: Configuring the Database Connection
Open the project (via the .sln file) with Visual Studio.

Open the appsettings.json file in the main directory.

In the ConnectionStrings -> DefaultConnection line, replace Server=... with your local SQL Server name (e.g., . , localhost, or COMPUTER-NAME).

Ensure the database name is SatoraCaffeRestaurantDB and that Trusted_Connection=True is set.

Step 3: Compiling and Running the Project
Compile the project using the Build > Build Solution option from the Visual Studio top menu.

Run the project by pressing F5.

The application will automatically start from the Login page due to the route settings in Program.cs.

4. DATABASE FEATURES
The system not only stores data but also provides automation with the following SQL objects:

Triggers: Automatic stock reduction when orders are added/updated, and real-time updating of table "Full/Empty" status.

Stored Procedures: Quick generation of daily turnover, order statistics, and personnel productivity reports on the dashboard.

Views: Simplifying complex JOIN structures and presenting them on the interface.


5. PANEL LOGIN INFORMATIONS

admin:admin@cafe.com	 		password:123
customer:musteri@deneme.com 		password:1234
personnel(waiter):tahsin@satora.com	password:123
Owner:owner@satora.com			password:123

6. DEVELOPER INFORMATION
Name and Surname: [BÜŞRA DÜZGÜN]




