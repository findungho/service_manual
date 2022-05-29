### Author: Dung Ho
### Email: fin.dungho@gmail.com
### Phone number: +358 449865555


## Overview
_This is a basic service manual application that manages the factory devices for the maintenance._


## General
- The Service manual implemented by using .Net 6.0. The application was using "UseInMemoryDatabase" from EntityFrameworkCore for development purpose. In production, it can be used different database engines, here I used Mysql Server configuration.
The application includes serveral services:

    + Get all factory devices and sort by the Severity and CreatedDate.
	+ Get specific factry device by its Id.
	+ Add new factory device.
	+ Update an existing device by its Id.
	+ Delete a factory device based on its Id.
	+ Search all factory devices that based on their Severity.
    

## Requirements
- Docker installed and running.
- .Net v6.0 installed.


## How to use
1. Clone the project 

2. Navigate to project root folder.

3. Run the application: 
    + For development: simply run following command
	`docker-compose up`

	+ For production: need to run migration database
		+ Uncomment following lines from Program.cs (ServiceManual.Web)
		```
		builder.Services.AddDbContext<FactoryDeviceContext>
				(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
		```
		+ Uncomment following lines from appsetting.json and edit correct information: port, user ID, Password.
		```
		  ,
		  "ConnectionStrings": {
			"DefaultConnection": "Data Source=localhost,<port>;Initial Catalog=ServiceManual;Persist Security Info=True;User ID=<ID>;Password=<pwd>"
		  }
		```
		+ Run migration: 
			- Open application by Vistual Studio -> Tools -> Nuget Package Manager -> Package Manager Console
			- Set Default project: ServiceManual.ApplicationCore
			- Commands:
				```
				Add-Migration InitialMigration
				update-database
				```
		+ Run application:
			```
			cd ServiceManual.Web
			dotnet run
			```


4. Test the API:
	The application can be tested by using Swagger. After run the application, open browser, navigate to https://localhost:5001/swagger/index.html
	+ POST:
		![POST request body](/screenshots/postrq.png)
		![POST request response](/screenshots/postrq_rp.png)
	+ GET all:
		![GET all](/screenshots/getall_rq.png)
	+ GET by Id:
		![GET by Id](/screenshots/get_byid.png)
	+ SEARCH filter by Severity
		![Search body](/screenshots/searchrq.png)
		![Search response](/screenshots/searchrq_rp.png)
	+ DELETE
		![DELETE](/screenshots/deleterq_rp.png)