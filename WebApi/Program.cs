var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<CarDb>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("Sqlite"));
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<CarDb>();
    db.Database.EnsureCreated();
}


app.MapGet("/cars", async (CarDb db) => await db.Cars.ToListAsync());
app.MapGet("/cars/{id}", async (int id, CarDb db) =>
    await db.Cars.FirstOrDefaultAsync(c => c.Id == id) is Car car
    ? Results.Ok(car)
    : Results.NotFound());

app.MapPost("/cars", async ([FromBody] Car car, [FromServices] CarDb db,
    HttpResponse response) =>
{
    db.Cars.Add(car);
    await db.SaveChangesAsync();
    response.StatusCode = 200;
    response.Headers.Location = $"/cars/{car.Id}";
});

app.MapPut("/cars", async ([FromBody] Car car, CarDb db) =>
{
    var carFromDb = await db.Cars.FindAsync(new object[] { car.Id });
    if (carFromDb == null) return Results.NotFound();
    carFromDb.Price = car.Price;
    carFromDb.Name = car.Name;
    await db.SaveChangesAsync();
    return Results.NoContent();
});
app.MapDelete("/cars/{id}", async (int id, CarDb db) =>
{
    var carFromDb = await db.Cars.FindAsync(new object[] {id});
    if (carFromDb == null) return Results.NotFound();
    db.Cars.Remove(carFromDb);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.UseHttpsRedirection();


app.Run();

public class CarDb : DbContext
{
    public CarDb(DbContextOptions<CarDb> options) : base(options) { }
    public DbSet<Car> Cars => Set<Car>();

}


public class Car
{
    public int Id { get; set; }
    public double Price { get; set; }
    public string Name { get; set; } = string.Empty;

}