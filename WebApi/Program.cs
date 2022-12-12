using WebApi.Data;
using WebApi.Data.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<CarDb>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("Sqlite"));
});

builder.Services.AddScoped<ICarRepository, CarRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<CarDb>();
    db.Database.EnsureCreated();
}

app.MapGet("/cars", async (ICarRepository repository) =>
    Results.Ok(await repository.GetCarsAsync()))
    .Produces<List<Car>>(StatusCodes.Status200OK)
    .WithName("Get All Cars")
    .WithTags("Getters");


app.MapGet("/cars/{id}", async (int id, ICarRepository repository) =>
    await repository.GetCarAsync(id) is Car car
    ? Results.Ok(car)
    : Results.NotFound())
    .Produces<List<Car>>(StatusCodes.Status200OK)
    .WithName("Get Car By Id")
    .WithTags("Getters");

app.MapGet("/cars/search/name/{query}",
    async (string query, ICarRepository repository) =>
        await repository.GetCarAsync(query) is IEnumerable<Car> cars
        ? Results.Ok(cars)
        : Results.NotFound(Array.Empty<Car>()))
    .Produces<List<Car>>(StatusCodes.Status200OK)
    .WithName("Search Car By Name")
    .WithTags("Getters");

app.MapPost("/cars", async ([FromBody] Car car, ICarRepository repository) =>
    {
        await repository.InsertCarAsync(car);
        await repository.SaveAsync();
        return Results.Created($"/cars/{car.Id}", car);
    })
    .Accepts<Car>("application/json")
    .Produces<List<Car>>(StatusCodes.Status201Created)
    .WithName("Create Car")
    .WithTags("Creators");

app.MapPut("/cars", async ([FromBody] Car car, ICarRepository repository) =>
    {
        await repository.ApdateCarAsync(car);
        await repository.SaveAsync();
        return Results.Ok();
    })
    .Accepts<Car>("application/json")
    .WithName("Update Car")
    .WithTags("Updaters");

app.MapDelete("/cars/{id}", async (int id, ICarRepository repository) =>
    {
        await repository.DeleteCarAsync(id);
        await repository.SaveAsync();
        return Results.NoContent();
    })
    .WithName("Delete Car")
    .WithTags("Deleters");

app.MapGet("/cars/search/sort/{query}",
    async (SearchParameters searchParameters, ICarRepository repository) =>
    await repository.GetCarAsync(searchParameters) is IEnumerable<Car> cars
        ? Results.Ok(cars)
        : Results.NotFound(Array.Empty<Car>()))
    .Produces<List<Car>>(StatusCodes.Status200OK)
    .WithName("Search Car Name and Price")
    .WithTags("Getters");

app.UseHttpsRedirection();
app.Run();



