# RestAPISkeleton

RestAPISkeleton is a skeleton that was stripped from a .NET Core REST API project built using Visual Studio. 

* Targetted framework:  >= .NET Framework 4.5 

## Installation

There are currently no way to "install" the project per-say, as this is not a complete project. This is more of an architectural solution to creating REST APIs. 

You can clone/download it and build on top of it using Controllers or whatever your preference is for HTTP communication.

## How-to-use
This skeleton exists to eliminate the need for boilerplate code and/or creating multiple layers that satisfies modern day business application architecture. 
Ideally, this project has a 5 step process to go from Database to Endpoint. As this was an MVC application at its birth, the steps had controllers and models: 
#### 1 - Controllers: HTTP Methods for endpoints, ideal approach was to have GET/ GET(id)/ POST/ PUT/ DELETE on each controller. Example: 
 * **GET EmployeeV1** - returns all employees. 
 * **GET(id) EmployeeV1/1** - returns specified employee by ID. 
 * **POST EmployeeV1** - Creates new employee based on the content found in [FromBody] attribute.
 * **PUT EmpooyeeV1/1** - Updates the specified employee by ID. This is the same as POST but exists to please our OCD.
 * **DELETE EmployeeV1/1** - Deletes the employee by ID

#### 2 - Model: Models for this project are simple POCO (Plain Old C# Objects).
 * Example: ```
            public string FirstName
            {
               get;
               set;
            }```
#### 3 - Business layer: Controllers use model to communicate with the business layer to send/retrieve data from/to the database.
* Business layer also handles all sorts of business logic that gets applied to the data retrieved from the db. Queries, filtering, adding, calculating are few of many manipulations that the business layer handles.
#### 4 - DataEntities: DataEntities is a class library that contains exactly the same POCO as models.
 * Reason for having this redundancy is for security. Only the model is exposed to the world, whereas DataEntities are only exposed UP TO the business layer. Plus, data in properties from DataEntities class are manipulated, whereas the POCO in model exists just to show the finalized data.
#### 5 - Data Access Layer: Contains all SQL statements, SQLClient methods, and all things to do with DB. 
 * Data access layer is another Class Library project that lives to handle all things related to DB. 
 * Includes but not limited to: Creation of connection to databases, methods to execute SQL queries/stored procedures/functions, and disposal of said connection. 
 * Our approach was to initiate a connection based on the request coming from the Controller, and using that same connection throughout the endpoint call lifespan, and send it to garbage collection after its done doing its job.
 * This way we prevented having multiple DB connections open at the same time, which helped the application's scalability.

## Usage
In order to use this skeleton, you would need to decide on a SQL Client. 

* For us, we used MySQL/SQLClient/AccessODBC/AccessOLE clients. You can use whichever client you fancy, given that it is supported by the .NET framework.
## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.
## License
[MIT](https://choosealicense.com/licenses/mit/)
