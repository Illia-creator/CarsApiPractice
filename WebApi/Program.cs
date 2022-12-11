using WebApi.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<CarDb>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("Sqlite"));
});

builder.Services.AddScoped<ICarRepository, CarRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<CarDb>();
    db.Database.EnsureCreated();
}

app.MapGet("/cars", async (ICarRepository repository) =>
    Results.Ok(await repository.GetCarsAsync()));

app.MapGet("/cars/{id}", async (int id, ICarRepository repository) =>
    await repository.GetCarAsync(id) is Car car
    ? Results.Ok(car)
    : Results.NotFound());

app.MapPost("/cars", async ([FromBody] Car car, ICarRepository repository) =>
{
    await repository.InsertCarAsync(car);
    await repository.SaveAsync();
    return Results.Created($"/cars/{car.Id}", car);
});

app.MapPut("/cars", async ([FromBody] Car car, ICarRepository repository) =>
{
    await repository.ApdateCarAsync(car);
    await repository.SaveAsync();
    return Results.Ok();
});

app.MapDelete("/cars/{id}", async (int id, ICarRepository repository) =>
{
    await repository.DeleteCarAsync(id);
    await repository.SaveAsync();
    return Results.NoContent();   
});

app.UseHttpsRedirection();

app.Run();



