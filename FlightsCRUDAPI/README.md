# Create A Simple Flights CRUD API

## Creating the project

    Open VSCode and look for the Create .NET Project

    On the dropdown type ASP.NET Core Web API

    Name the project Flights CRUD API and save it on a a folder of your preference on your computer.

### Adapting the project to use Controllers

    Clean up the program.cs to look like this:

```csharp

    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    // Learn more about configuring Swagger/OpenAPI at https:// aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.Run();
```

    Now add support for the controllers by adding the following two lines on the Program.cs

```csharp

        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();


        builder.Services.AddControllers(); // Add this line

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }


        app.MapControllers(); // Add this line


        app.Run();
```

### Creating project folders

    On the project root create three folders first the Models Folder, second the Services folder, and the Controllers Folder

### Creating the Flight model

    On the Models Folder right click and hit new C# and select class  and name it Flight and it will look like this:

```csharp
        using System;
        using System.Collections.Generic;
        using System.Linq;
        using System.Threading.Tasks;

        namespace FlightsCRUDAPI.Models
        {
            public class Flight
            {

            }
        }
```

    Now we will start by adding some properties on the Flight class
    First we will add an int Id property it will be  used to identify each flight on the database with a unique number
    Then we will add an int FlightNumber property
    Then we will add a string AirlineName propery
    Then we will add a string DepartureAirportCode property
    then we will add a string ArrivalAirportCode property
    then we will add a DateTime DepartureDateTime property
    then we will add a DateTime ArrivalDateTime property
    lastly we will add an int PassengerCapacity property

    The final Flight class will look like this:

```csharp
        namespace FlightsCRUDAPI.Models
        {
            public class Flight
            {
                public int Id { get; set; }

                public int FlightNumber { get; set; }

                public string AirlineName { get; set; } = string.Empty;

                public string DepartureAirportCode { get; set; } = string.Empty;

                public string DestinationAirportCode { get; set; } = string.Empty;

                public DateTime DepartureDataTime { get; set; }

                public DateTime ArrivalDataTime {get; set;}

                public int PassengerCapacity { get; set; }
            }
        }
```

### Adding entity Framework

            Next we will install some packages to use entity framework and sql server
            Microsoft.EntityFrameworkCore.Design
            Microsoft.EntityFrameworkCore.Tools
            Microsoft.EntityFrameworkCore.Sqlite
            Create a connection string
            on appsettings.json add the following

```json
                "ConnectionStrings": {
                    "DefaultConnection": "Data Source=flights.db"

                    },

```

            On Program.cs before the app variable declaration add the following. Make sure DefaultConnection is the same name as the appsettings.json file ConnectionStrings

```csharp

    builder.Services.AddDbContext<ApplicationDbContext>(opt => opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

```

### using the tools to create a migration

    Open the terminal and type
    - dotnet tool install dotnet-ef  --global

    Make sure you are inside of the API folder then create initial migration

    - dotnet ef migrations add InitialCreate -o Data/Migrations

    Update the database with the migration

    - dotnet ef database update

### Installing auto mapper and creating a default mapping configuration

    Search for package AutoMapper and install it

    Create a Mappers Folder

    Create MappingProfile.cs class

```CSharp

        using AutoMapper;
        using FlightsCRUDAPI.Models;
        using FlightsCRUDAPI.Models.Dtos;

        namespace FlightsCRUDAPI.Mappers
        {
            public class MappingProfile : Profile
            {
                public MappingProfile()
                {
                    CreateMap<Flight, FlightApiRequest>();
                    CreateMap<FlightApiRequest, Flight>();
                }
            }
        }
```

    Register the mapper in program.cs

    Add the following line before the app declaration

    -   builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

### Creating ApiResponseDto

```csharp

        namespace FlightsCRUDAPI.Models.Dtos
        {
            public class ApiResponseDto<T>
            {
                public bool RequestFailed { get; set; } = false;

                public string ResponseCode { get; set; } = string.Empty;

                public string ErrorMessage { get; set; } = string.Empty;
                public T? Data { get; set; }
            }
        }
```

### Creating a service

    First we will create an interfece called IFlightService on the Services Folder.

    It should look like this:

```csharp

        using FlightsCRUDAPI.Models;
        using FlightsCRUDAPI.Models.Dtos;

        namespace FlightsCRUDAPI.Services
        {
            public interface IFlightService
            {
                Task<ApiResponseDto<List<Flight>?>> GetAllFlights();
                Task<ApiResponseDto<Flight?>> GetFlightById(int id);
                Task<ApiResponseDto<Flight?>> CreateFlight(FlightApiRequest apiRequestDto);

                Task<ApiResponseDto<Flight?>> UpdateFlight(int id, FlightApiRequest flightToUpdateDto);
                Task<ApiResponseDto<Flight?>> DeleteFlight(int id);
            }
        }

```

    On the Services Folder create a file called FlightsService.cs

    add the following code:

    First we will start by creating a constructor with two properties

    The ApplicationDbContext as dbContext and the IMapper as mapper

    then you will hover over dbContext press cmd + . and choose initialize field from parameter if it does not show the first time try hitting the cmd + . a second or third time till initialize field from parameter appears.

    Your code should look like this:

```csharp
        using AutoMapper;
        using FlightsCRUDAPI.Data;
        using Microsoft.EntityFrameworkCore;
        namespace FlightsCRUDAPI.Services
        {
            public class FlightService
            {
                private readonly ApplicationDbContext _dbContext;
                private readonly IMapper _mapper;

                public FlightService(ApplicationDbContext dbContext, IMapper mapper)
                {
                    _mapper = mapper;

                    _dbContext = dbContext;
                }
            }
        }
```

    Now we will create the GetAllFlightsMethod

```csharp


        public async Task<ApiResponseDto<List<Flight>?>> GetAllFlights()
        {

            try
            {
                var flights = await _dbContext.Flights.ToListAsync();
                return new ApiResponseDto<List<Flight>?>
                {
                    RequestFailed = false,
                    Data = flights,
                    ResponseCode = "200"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<List<Flight>?>
                {
                    RequestFailed = true,
                    Data = null,
                    ResponseCode = "500",
                    ErrorMessage = $"An error occurred retriving the flights from the database: {ex.Message}"
                };

            }

        }



```

        This method, `GetAllFlights`, is an asynchronous method that retrieves all `Flight` entities from the database and wraps the result in an `ApiResponseDto`.

        Here's a breakdown of what it does:

        - `public async Task<ApiResponseDto<List<Flight>?>> GetAllFlights()`: This is the method signature. It's a public method, meaning it can be accessed from outside the class. The `async` keyword indicates that this method is asynchronous, meaning it can run without blocking the calling thread. `Task<ApiResponseDto<List<Flight>?>>` is the return type of the method, indicating it returns a task that, when completed, will produce an `ApiResponseDto` that contains a list of `Flight` objects or null.

        - `try { ... } catch (Exception ex) { ... }`: This is a `try/catch` block. The code inside the `try` block is executed, and if an exception is thrown, the code inside the `catch` block is executed. The `catch` block catches exceptions of type `Exception`, which is the base class for all exceptions in C#. This means it will catch any exception that is thrown.

        - `var flights = await _dbContext.Flights.ToListAsync();`: This line of code starts the asynchronous operation to retrieve all `Flight` entities from the database and waits for it to complete. The `await` keyword is used to suspend the execution of the method until the awaited task completes. This allows the method to return control to its caller until the task is done, preventing the method from blocking the calling thread.

        - `return new ApiResponseDto<List<Flight>?> { ... };`: This returns an instance of `ApiResponseDto` with the `RequestFailed`, `Data`, `ResponseCode`, and `ErrorMessage` properties set. If the `try` block executes successfully, it indicates a successful request with a 200 response code and the `flights` data. If an exception is thrown, the `catch` block returns an `ApiResponseDto` indicating a request failure with a 500 response code and an error message that includes the exception message.

        In summary, this method retrieves all `Flight` entities from the database, wraps them in an `ApiResponseDto`, and returns it. If an error occurs during the retrieval of the flights, it catches the exception and returns an `ApiResponseDto` indicating a request failure.

    Then we will continue with the GetFlightById method

```csharp


        public async Task<ApiResponseDto<Flight?>> GetFlightById(int id)
        {
            try
            {
                var flight = await _dbContext.Flights.FindAsync(id);
                if (flight is null)
                {
                    return new ApiResponseDto<Flight?>
                    {
                        RequestFailed = true,
                        Data = null,
                        ResponseCode = "404",
                        ErrorMessage = $"Flight with id: {id} was not found."

                    };
                }
                return new ApiResponseDto<Flight?>
                {
                    RequestFailed = false,
                    Data = flight,
                    ResponseCode = "200"

                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<Flight?>
                {
                    RequestFailed = true,
                    Data = null,
                    ResponseCode = "500",
                    ErrorMessage = $"An error occurred retriving the flight from the database: {ex.Message}"
                };
            }

        }



```

        This method, `GetFlightById`, is an asynchronous method that retrieves a `Flight` entity with a specific ID from the database and wraps the result in an `ApiResponseDto`.

        Here's a breakdown of what it does:

        - `public async Task<ApiResponseDto<Flight?>> GetFlightById(int id)`: This is the method signature. It's a public method, meaning it can be accessed from outside the class. The `async` keyword indicates that this method is asynchronous, meaning it can run without blocking the calling thread. `Task<ApiResponseDto<Flight?>>` is the return type of the method, indicating it returns a task that, when completed, will produce an `ApiResponseDto` that contains a `Flight` object or null.

        - `try { ... } catch (Exception ex) { ... }`: This is a `try/catch` block. The code inside the `try` block is executed, and if an exception is thrown, the code inside the `catch` block is executed. The `catch` block catches exceptions of type `Exception`, which is the base class for all exceptions in C#. This means it will catch any exception that is thrown.

        - `var flight = await _dbContext.Flights.FindAsync(id);`: This line of code starts the asynchronous operation to retrieve a `Flight` entity with the specified ID from the database and waits for it to complete. The `await` keyword is used to suspend the execution of the method until the awaited task completes. This allows the method to return control to its caller until the task is done, preventing the method from blocking the calling thread.

        - `if (flight is null) { ... }`: This checks if the `flight` is null, which would mean that no `Flight` entity with the specified ID was found in the database.

        - `return new ApiResponseDto<Flight?> { ... };`: This returns an instance of `ApiResponseDto` with the `RequestFailed`, `Data`, `ResponseCode`, and `ErrorMessage` properties set. If `flight` is null, it indicates a request failure with a 404 response code and an error message. If `flight` is not null, it indicates a successful request with a 200 response code and the `flight` data. If an exception is thrown, the `catch` block returns an `ApiResponseDto` indicating a request failure with a 500 response code and an error message that includes the exception message.

        In summary, this method retrieves a `Flight` entity with a specific ID from the database, wraps it in an `ApiResponseDto`, and returns it. If no `Flight` entity with the specified ID is found, or if an error occurs during the retrieval of the flight, it returns an `ApiResponseDto` indicating a request failure.

    Then we will focus on the CreateFlight method

```csharp


        public async Task<ApiResponseDto<Flight?>> CreateFlight(FlightApiRequest apiRequestDto)
        {
            // This mapping will create a new Flight object from the dto
            Flight newFlight = _mapper.Map<Flight>(apiRequestDto);

            try
            {
                await _dbContext.Flights.AddAsync(newFlight);

                await _dbContext.SaveChangesAsync();

                return new ApiResponseDto<Flight?>
                {
                    RequestFailed = false,
                    Data = newFlight,
                    ResponseCode = "201"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<Flight?>
                {
                    RequestFailed = true,
                    Data = null,
                    ResponseCode = "500",
                    ErrorMessage = $"An error occurred saving the flight on the database: {ex.Message}"
                };
            }

        }


```

        This method, `CreateFlight`, is an asynchronous method that creates a new `Flight` entity in the database based on the provided `FlightApiRequest` DTO (Data Transfer Object), and wraps the result in an `ApiResponseDto`.

        Here's a breakdown of what it does:

        - `public async Task<ApiResponseDto<Flight?>> CreateFlight(FlightApiRequest apiRequestDto)`: This is the method signature. It's a public method, meaning it can be accessed from outside the class. The `async` keyword indicates that this method is asynchronous, meaning it can run without blocking the calling thread. `Task<ApiResponseDto<Flight?>>` is the return type of the method, indicating it returns a task that, when completed, will produce an `ApiResponseDto` that contains a `Flight` object or null.

        - `Flight newFlight = _mapper.Map<Flight>(apiRequestDto);`: This line of code uses AutoMapper to map the properties of the `apiRequestDto` to a new `Flight` object. AutoMapper is a library that simplifies object-object mapping, which is useful for mapping DTOs to entities and vice versa.

        - `try { ... } catch (Exception ex) { ... }`: This is a `try/catch` block. The code inside the `try` block is executed, and if an exception is thrown, the code inside the `catch` block is executed. The `catch` block catches exceptions of type `Exception`, which is the base class for all exceptions in C#. This means it will catch any exception that is thrown.

        - `await _dbContext.Flights.AddAsync(newFlight);`: This line of code starts the asynchronous operation to add the new `Flight` entity to the `Flights` DbSet and waits for it to complete.

        - `await _dbContext.SaveChangesAsync();`: This line of code starts the asynchronous operation to save all changes made in the DbContext to the database and waits for it to complete.

        - `return new ApiResponseDto<Flight?> { ... };`: This returns an instance of `ApiResponseDto` with the `RequestFailed`, `Data`, `ResponseCode`, and `ErrorMessage` properties set. If the `try` block executes successfully, it indicates a successful request with a 201 response code and the `newFlight` data. If an exception is thrown, the `catch` block returns an `ApiResponseDto` indicating a request failure with a 500 response code and an error message that includes the exception message.

        In summary, this method creates a new `Flight` entity in the database based on the provided `FlightApiRequest` DTO, saves the changes, wraps the new `Flight` entity in an `ApiResponseDto`, and returns it. If an error occurs during the creation of the flight, it catches the exception and returns an `ApiResponseDto` indicating a request failure.


    Were almost done with the crud methods, now let's work on UpdateFlight

```csharp


        public async Task<ApiResponseDto<Flight?>> UpdateFlight(int id, FlightApiRequest flightToUpdateDto)
        {
            Flight? dbFlight;

            try
            {
                /*
                Here I show that firstordefault async can look for the id as well but
                preferably you will use FindAsync if you will search by primary key
                and use FirstOrDefaultAsync if you will search by other property instead
                of primary key
                */
                dbFlight = await _dbContext.Flights.FirstOrDefaultAsync(f => f.Id == id);

                if (dbFlight == null)
                {
                    return new ApiResponseDto<Flight?>
                    {
                        RequestFailed = true,
                        Data = null,
                        ResponseCode = "404",
                        ErrorMessage = $"Flight with id: {id} was not found."

                    };
                }
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<Flight?>
                {
                    RequestFailed = true,
                    Data = null,
                    ResponseCode = "500",
                    ErrorMessage = $"An error occurred saving the flight on the database: {ex.Message}"
                };
            }

            try
            {
                /*
                This time we don't use the generic as we want to
                make sure we use the dbFlight object and not create
                a new flight object
                */
                _mapper.Map(flightToUpdateDto, dbFlight);

                await _dbContext.SaveChangesAsync();

                return new ApiResponseDto<Flight?>
                {
                    RequestFailed = false,
                    Data = dbFlight,
                    ResponseCode = "200"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<Flight?>
                {
                    RequestFailed = true,
                    Data = null,
                    ResponseCode = "500",
                    ErrorMessage = $"An error occurred updating the flight on the database: {ex.Message}"
                };
            }



        }


```

        This method, `UpdateFlight`, is an asynchronous method that updates a `Flight` entity in the database with the provided `FlightApiRequest` DTO (Data Transfer Object), and wraps the result in an `ApiResponseDto`.

        Here's a breakdown of what it does:

        - `public async Task<ApiResponseDto<Flight?>> UpdateFlight(int id, FlightApiRequest flightToUpdateDto)`: This is the method signature. It's a public method, meaning it can be accessed from outside the class. The `async` keyword indicates that this method is asynchronous, meaning it can run without blocking the calling thread. `Task<ApiResponseDto<Flight?>>` is the return type of the method, indicating it returns a task that, when completed, will produce an `ApiResponseDto` that contains a `Flight` object or null.

        - `dbFlight = await _dbContext.Flights.FirstOrDefaultAsync(f => f.Id == id);`: This line of code starts the asynchronous operation to retrieve a `Flight` entity with the specified ID from the database and waits for it to complete. The `await` keyword is used to suspend the execution of the method until the awaited task completes. This allows the method to return control to its caller until the task is done, preventing the method from blocking the calling thread.

        - `if (dbFlight == null) { ... }`: This checks if the `dbFlight` is null, which would mean that no `Flight` entity with the specified ID was found in the database.

        - `try { ... } catch (Exception ex) { ... }`: These are `try/catch` blocks. The code inside the `try` blocks is executed, and if an exception is thrown, the code inside the `catch` blocks is executed. The `catch` blocks catch exceptions of type `Exception`, which is the base class for all exceptions in C#. This means it will catch any exception that is thrown.

        - `_mapper.Map(flightToUpdateDto, dbFlight);`: This line of code uses AutoMapper to map the properties of the `flightToUpdateDto` to the `dbFlight` object. AutoMapper is a library that simplifies object-object mapping, which is useful for mapping DTOs to entities and vice versa.

        - `await _dbContext.SaveChangesAsync();`: This line of code starts the asynchronous operation to save all changes made in the DbContext to the database and waits for it to complete.

        - `return new ApiResponseDto<Flight?> { ... };`: These return an instance of `ApiResponseDto` with the `RequestFailed`, `Data`, `ResponseCode`, and `ErrorMessage` properties set. If the `try` blocks execute successfully, they indicate a successful request with a 200 or 404 response code and the `dbFlight` data or an error message. If an exception is thrown, the `catch` blocks return an `ApiResponseDto` indicating a request failure with a 500 response code and an error message that includes the exception message.

        In summary, this method retrieves a `Flight` entity with a specific ID from the database, updates it with the provided `FlightApiRequest` DTO, saves the changes, wraps the updated `Flight` entity in an `ApiResponseDto`, and returns it. If no `Flight` entity with the specified ID is found, or if an error occurs during the update of the flight, it returns an `ApiResponseDto` indicating a request failure.

    Finally we will tackle the delete method

```csharp


        public async Task<ApiResponseDto<Flight?>> DeleteFlight(int id)
        {
            Flight? dbFlight;

            try
            {
                dbFlight = await _dbContext.Flights.FirstOrDefaultAsync(f => f.Id == id);

                if (dbFlight is null)
                {
                    return new ApiResponseDto<Flight?>
                    {
                        RequestFailed = true,
                        Data = null,
                        ResponseCode = "404",
                        ErrorMessage = $"Flight with id: {id} was not found."

                    };
                }
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<Flight?>
                {
                    RequestFailed = true,
                    Data = null,
                    ResponseCode = "500",
                    ErrorMessage = $"An error occurred retriving the flight on the database: {ex.Message}"
                };

            }

            try
            {
                _dbContext.Flights.Remove(dbFlight);

                await _dbContext.SaveChangesAsync();

                return new ApiResponseDto<Flight?>
                {
                    RequestFailed = false,
                    Data = dbFlight,
                    ResponseCode = "200"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<Flight?>
                {
                    RequestFailed = true,
                    Data = null,
                    ResponseCode = "500",
                    ErrorMessage = $"An error occurred deleting the flight on the database: {ex.Message}"
                };
            }

        }


```

        This method, `DeleteFlight`, is an asynchronous method that deletes a `Flight` entity from the database based on the provided ID, and wraps the result in an `ApiResponseDto`.

        Here's a breakdown of what it does:

        - `public async Task<ApiResponseDto<Flight?>> DeleteFlight(int id)`: This is the method signature. It's a public method, meaning it can be accessed from outside the class. The `async` keyword indicates that this method is asynchronous, meaning it can run without blocking the calling thread. `Task<ApiResponseDto<Flight?>>` is the return type of the method, indicating it returns a task that, when completed, will produce an `ApiResponseDto` that contains a `Flight` object or null.

        - `dbFlight = await _dbContext.Flights.FirstOrDefaultAsync(f => f.Id == id);`: This line of code starts the asynchronous operation to retrieve a `Flight` entity with the specified ID from the database and waits for it to complete. The `await` keyword is used to suspend the execution of the method until the awaited task completes. This allows the method to return control to its caller until the task is done, preventing the method from blocking the calling thread.

        - `if (dbFlight is null) { ... }`: This checks if the `dbFlight` is null, which would mean that no `Flight` entity with the specified ID was found in the database.

        - `try { ... } catch (Exception ex) { ... }`: These are `try/catch` blocks. The code inside the `try` blocks is executed, and if an exception is thrown, the code inside the `catch` blocks is executed. The `catch` blocks catch exceptions of type `Exception`, which is the base class for all exceptions in C#. This means it will catch any exception that is thrown.

        - `_dbContext.Flights.Remove(dbFlight);`: This line of code removes the `dbFlight` entity from the `Flights` DbSet.

        - `await _dbContext.SaveChangesAsync();`: This line of code starts the asynchronous operation to save all changes made in the DbContext to the database and waits for it to complete.

        - `return new ApiResponseDto<Flight?> { ... };`: These return an instance of `ApiResponseDto` with the `RequestFailed`, `Data`, `ResponseCode`, and `ErrorMessage` properties set. If the `try` blocks execute successfully, they indicate a successful request with a 200 or 404 response code and the `dbFlight` data or an error message. If an exception is thrown, the `catch` blocks return an `ApiResponseDto` indicating a request failure with a 500 response code and an error message that includes the exception message.

        In summary, this method retrieves a `Flight` entity with a specific ID from the database, deletes it, saves the changes, wraps the deleted `Flight` entity in an `ApiResponseDto`, and returns it. If no `Flight` entity with the specified ID is found, or if an error occurs during the deletion of the flight, it returns an `ApiResponseDto` indicating a request failure.

### Register our service on program.cs

```csharp

        using FlightsCRUDAPI.Data;
        using FlightsCRUDAPI.Services;
        using Microsoft.EntityFrameworkCore;

        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();


        builder.Services.AddControllers(); // Add this line
        builder.Services.AddDbContext<ApplicationDbContext>(opt => opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

        // Register the service
        builder.Services.AddScoped<IFlightService, FlightService>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }


        app.MapControllers(); // Add this line


        app.Run();



```

### Creating the controller

    On the controllers folder create a class called FlightsController

    inside we will use dependency injection to inject our service so we can forward our request objects to service for processing

```csharp

    using FlightsCRUDAPI.Services;

    namespace FlightsCRUDAPI.Controllers
    {
        [ApiController]
        [Route("api/[controller]")]
        public class FlightController : ControllerBase
        {
            private readonly IFlightService _flightService;
            public FlightController(IFlightService flightService)
            {
                _flightService = flightService;
            }
        }
    }

```

        This is a controller class in an ASP.NET Core application. Here's a breakdown of what it does:

        - `using FlightsCRUDAPI.Services;`: This line imports the `FlightsCRUDAPI.Services` namespace, which presumably contains the `IFlightService` interface.

        - `namespace FlightsCRUDAPI.Controllers { ... }`: This declares a namespace named `FlightsCRUDAPI.Controllers`. Namespaces are used in C# to organize related classes, interfaces, and other types.

        - `[ApiController]`: This attribute indicates that the `FlightController` class is a controller that uses the Controller Base Class and responds to web API requests. It enables certain features such as attribute routing and automatic HTTP 400 responses.

        - `[Route("api/[controller]")]`: This attribute sets the route template for the controller. `[controller]` is a token that gets replaced with the controller's name. So, for the `FlightController`, the route would be "api/Flight".

        - `public class FlightController : ControllerBase { ... }`: This declares a public class named `FlightController` that inherits from `ControllerBase`. `ControllerBase` is a base class for an MVC controller without view support.

        - `private readonly IFlightService _flightService;`: This declares a private, read-only field of type `IFlightService`. The `readonly` keyword means that the field can only be assigned at declaration or in the constructor.

        - `public FlightController(IFlightService flightService) { ... }`: This is the constructor for the `FlightController` class. It takes an `IFlightService` parameter, which is assigned to the `_flightService` field. This is an example of dependency injection, where the `IFlightService` dependency is "injected" into the `FlightController` through the constructor.

        In summary, this `FlightController` class is a web API controller that handles HTTP requests at the "api/Flight" route. It has a dependency on an `IFlightService`, which is provided through dependency injection.

#### Let's start by creating our GetAll Endpoint

```csharp

        [HttpGet]
        public async Task<ActionResult<ApiResponseDto<List<Flight>>>> GetAllFlights()
        {
            var response = await _flightService.GetAllFlights();

            if (response.Data != null)
            {
                return Ok(response);
            }

            if (response.ResponseCode == "404")
            {
                return NotFound(response);
            }

            return new ObjectResult(response);

        }

```

        This method, `GetAllFlights`, is an asynchronous method that retrieves all `Flight` entities from the database and wraps the result in an `ApiResponseDto`.

        Here's a breakdown of what it does:

        - `[HttpGet]`: This attribute indicates that the method should handle HTTP GET requests.

        - `public async Task<ActionResult<ApiResponseDto<List<Flight>>>> GetAllFlights()`: This is the method signature. It's a public method, meaning it can be accessed from outside the class. The `async` keyword indicates that this method is asynchronous, meaning it can run without blocking the calling thread. `Task<ActionResult<ApiResponseDto<List<Flight>>>>` is the return type of the method, indicating it returns a task that, when completed, will produce an `ActionResult` that contains an `ApiResponseDto` with a list of `Flight` objects.

        - `var response = await _flightService.GetAllFlights();`: This line of code starts the asynchronous operation to retrieve all `Flight` entities from the database and waits for it to complete. The `await` keyword is used to suspend the execution of the method until the awaited task completes. This allows the method to return control to its caller until the task is done, preventing the method from blocking the calling thread.

        - `if (response.Data != null) { ... }`: This checks if the `Data` property of the `response` is not null, which would mean that at least one `Flight` entity was found in the database.

        - `return Ok(response);`: This returns an HTTP 200 OK response with the `response` as the body.

        - `if (response.ResponseCode == "404") { ... }`: This checks if the `ResponseCode` property of the `response` is "404", which would mean that no `Flight` entities were found in the database.

        - `return NotFound(response);`: This returns an HTTP 404 Not Found response with the `response` as the body.

        - `return new ObjectResult(response);`: This returns an HTTP response with the `response` as the body. The status code of the response will be determined by the ASP.NET Core framework based on the `response`.

        In summary, this method retrieves all `Flight` entities from the database, wraps them in an `ApiResponseDto`, and returns an HTTP response with the `ApiResponseDto` as the body. The status code of the response is determined based on the `Data` and `ResponseCode` properties of the `ApiResponseDto`.

#### Creating the GetByID method

```csharp

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponseDto<Flight?>>> GetFlightById(int id)
        {
            var response = await _flightService.GetFlightById(id);

            if (response.Data != null)
            {
                return Ok(response);
            }

            if(response.ResponseCode == "404")
            {
                return NotFound(response);
            }

            return new ObjectResult(response);
        }

```

        This method, `GetFlightById`, is an asynchronous method that retrieves a `Flight` entity from the database based on the provided ID and wraps the result in an `ApiResponseDto`.

        Here's a breakdown of what it does:

        - `[HttpGet("{id}")]`: This attribute indicates that the method should handle HTTP GET requests at a route that includes an `id` parameter.

        - `public async Task<ActionResult<ApiResponseDto<Flight?>>> GetFlightById(int id)`: This is the method signature. It's a public method, meaning it can be accessed from outside the class. The `async` keyword indicates that this method is asynchronous, meaning it can run without blocking the calling thread. `Task<ActionResult<ApiResponseDto<Flight?>>>` is the return type of the method, indicating it returns a task that, when completed, will produce an `ActionResult` that contains an `ApiResponseDto` with a `Flight` object or null.

        - `var response = await _flightService.GetFlightById(id);`: This line of code starts the asynchronous operation to retrieve a `Flight` entity with the specified ID from the database and waits for it to complete. The `await` keyword is used to suspend the execution of the method until the awaited task completes. This allows the method to return control to its caller until the task is done, preventing the method from blocking the calling thread.

        - `if (response.Data != null) { ... }`: This checks if the `Data` property of the `response` is not null, which would mean that a `Flight` entity with the specified ID was found in the database.

        - `return Ok(response);`: This returns an HTTP 200 OK response with the `response` as the body.

        - `if (response.ResponseCode == "404") { ... }`: This checks if the `ResponseCode` property of the `response` is "404", which would mean that no `Flight` entity with the specified ID was found in the database.

        - `return NotFound(response);`: This returns an HTTP 404 Not Found response with the `response` as the body.

        - `return new ObjectResult(response);`: This returns an HTTP response with the `response` as the body. The status code of the response will be determined by the ASP.NET Core framework based on the `response`.

        In summary, this method retrieves a `Flight` entity with a specific ID from the database, wraps it in an `ApiResponseDto`, and returns an HTTP response with the `ApiResponseDto` as the body. The status code of the response is determined based on the `Data` and `ResponseCode` properties of the `ApiResponseDto`.

#### Creating the CreateFlight endpoint

```csharp

        [HttpPost]
        public async Task<ActionResult<ApiResponseDto<Flight?>>> CreateFlight(FlightApiRequest apiRequestDto)
        {
            var response = await _flightService.CreateFlight(apiRequestDto);

            if(response.Data != null)
            {
                return Ok(response);
            }

            return new ObjectResult(response);
        }

```

        This method, `CreateFlight`, is an asynchronous method that creates a new `Flight` entity in the database based on the provided `FlightApiRequest` and wraps the result in an `ApiResponseDto`.

        Here's a breakdown of what it does:

        - `[HttpPost]`: This attribute indicates that the method should handle HTTP POST requests.

        - `public async Task<ActionResult<ApiResponseDto<Flight?>>> CreateFlight(FlightApiRequest apiRequestDto)`: This is the method signature. It's a public method, meaning it can be accessed from outside the class. The `async` keyword indicates that this method is asynchronous, meaning it can run without blocking the calling thread. `Task<ActionResult<ApiResponseDto<Flight?>>>` is the return type of the method, indicating it returns a task that, when completed, will produce an `ActionResult` that contains an `ApiResponseDto` with a `Flight` object or null. `FlightApiRequest apiRequestDto` is the parameter that the method takes, which represents the data for the new `Flight` entity.

        - `var response = await _flightService.CreateFlight(apiRequestDto);`: This line of code starts the asynchronous operation to create a new `Flight` entity in the database based on the `apiRequestDto` and waits for it to complete. The `await` keyword is used to suspend the execution of the method until the awaited task completes. This allows the method to return control to its caller until the task is done, preventing the method from blocking the calling thread.

        - `if (response.Data != null) { ... }`: This checks if the `Data` property of the `response` is not null, which would mean that the `Flight` entity was successfully created in the database.

        - `return Ok(response);`: This returns an HTTP 200 OK response with the `response` as the body.

        - `return new ObjectResult(response);`: This returns an HTTP response with the `response` as the body. The status code of the response will be determined by the ASP.NET Core framework based on the `response`.

        In summary, this method creates a new `Flight` entity in the database based on the `FlightApiRequest`, wraps the result in an `ApiResponseDto`, and returns an HTTP response with the `ApiResponseDto` as the body. The status code of the response is determined based on the `Data` property of the `ApiResponseDto`.

#### Create the UpateFlight method

```csharp

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponseDto<Flight?>>> UpdateFlight(int id, FlightApiRequest apiRequestDto)
        {
            var response = await _flightService.UpdateFlight(id, apiRequestDto);

            if (response.Data != null)
            {
                return Ok(response);
            }

            if(response.ResponseCode.Equals("404"))
            {
                return NotFound(response);
            }

            return new ObjectResult(response);
        }

```

        This method, `UpdateFlight`, is an asynchronous method that updates an existing `Flight` entity in the database based on the provided ID and `FlightApiRequest`, and wraps the result in an `ApiResponseDto`.

        Here's a breakdown of what it does:

        - `[HttpPut("{id}")]`: This attribute indicates that the method should handle HTTP PUT requests at a route that includes an `id` parameter.

        - `public async Task<ActionResult<ApiResponseDto<Flight?>>> UpdateFlight(int id, FlightApiRequest apiRequestDto)`: This is the method signature. It's a public method, meaning it can be accessed from outside the class. The `async` keyword indicates that this method is asynchronous, meaning it can run without blocking the calling thread. `Task<ActionResult<ApiResponseDto<Flight?>>>` is the return type of the method, indicating it returns a task that, when completed, will produce an `ActionResult` that contains an `ApiResponseDto` with a `Flight` object or null. `int id` and `FlightApiRequest apiRequestDto` are the parameters that the method takes, which represent the ID of the `Flight` entity to update and the new data for the `Flight` entity, respectively.

        - `var response = await _flightService.UpdateFlight(id, apiRequestDto);`: This line of code starts the asynchronous operation to update the `Flight` entity with the specified ID in the database based on the `apiRequestDto` and waits for it to complete. The `await` keyword is used to suspend the execution of the method until the awaited task completes. This allows the method to return control to its caller until the task is done, preventing the method from blocking the calling thread.

        - `if (response.Data != null) { ... }`: This checks if the `Data` property of the `response` is not null, which would mean that the `Flight` entity was successfully updated in the database.

        - `return Ok(response);`: This returns an HTTP 200 OK response with the `response` as the body.

        - `if (response.ResponseCode.Equals("404")) { ... }`: This checks if the `ResponseCode` property of the `response` is "404", which would mean that no `Flight` entity with the specified ID was found in the database.

        - `return NotFound(response);`: This returns an HTTP 404 Not Found response with the `response` as the body.

        - `return new ObjectResult(response);`: This returns an HTTP response with the `response` as the body. The status code of the response will be determined by the ASP.NET Core framework based on the `response`.

        In summary, this method updates an existing `Flight` entity in the database based on the provided ID and `FlightApiRequest`, wraps the result in an `ApiResponseDto`, and returns an HTTP response with the `ApiResponseDto` as the body. The status code of the response is determined based on the `Data` and `ResponseCode` properties of the `ApiResponseDto`.

#### Creating the delete method

```csharp

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponseDto<Flight?>>> DeleteFlight(int id)
        {
            var response = await _flightService.DeleteFlight(id);

            if (response.Data != null)
            {
                return Ok(response);
            }

            if(response.ResponseCode.Equals("404"))
            {
                return NotFound(response);
            }

            return new ObjectResult(response);
        }

```

        This method, `DeleteFlight`, is an asynchronous method that deletes an existing `Flight` entity in the database based on the provided ID and wraps the result in an `ApiResponseDto`.

        Here's a breakdown of what it does:

        - `[HttpDelete("{id}")]`: This attribute indicates that the method should handle HTTP DELETE requests at a route that includes an `id` parameter.

        - `public async Task<ActionResult<ApiResponseDto<Flight?>>> DeleteFlight(int id)`: This is the method signature. It's a public method, meaning it can be accessed from outside the class. The `async` keyword indicates that this method is asynchronous, meaning it can run without blocking the calling thread. `Task<ActionResult<ApiResponseDto<Flight?>>>` is the return type of the method, indicating it returns a task that, when completed, will produce an `ActionResult` that contains an `ApiResponseDto` with a `Flight` object or null. `int id` is the parameter that the method takes, which represents the ID of the `Flight` entity to delete.

        - `var response = await _flightService.DeleteFlight(id);`: This line of code starts the asynchronous operation to delete the `Flight` entity with the specified ID from the database and waits for it to complete. The `await` keyword is used to suspend the execution of the method until the awaited task completes. This allows the method to return control to its caller until the task is done, preventing the method from blocking the calling thread.

        - `if (response.Data != null) { ... }`: This checks if the `Data` property of the `response` is not null, which would mean that the `Flight` entity was successfully deleted from the database.

        - `return Ok(response);`: This returns an HTTP 200 OK response with the `response` as the body.

        - `if (response.ResponseCode.Equals("404")) { ... }`: This checks if the `ResponseCode` property of the `response` is "404", which would mean that no `Flight` entity with the specified ID was found in the database.

        - `return NotFound(response);`: This returns an HTTP 404 Not Found response with the `response` as the body.

        - `return new ObjectResult(response);`: This returns an HTTP response with the `response` as the body. The status code of the response will be determined by the ASP.NET Core framework based on the `response`.

        In summary, this method deletes an existing `Flight` entity in the database based on the provided ID, wraps the result in an `ApiResponseDto`, and returns an HTTP response with the `ApiResponseDto` as the body. The status code of the response is determined based on the `Data` and `ResponseCode` properties of the `ApiResponseDto`.

### Test the api using swagger

    Run the application and test the api on the swagger ui
