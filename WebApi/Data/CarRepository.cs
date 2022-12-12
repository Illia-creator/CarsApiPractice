namespace WebApi.Data
{
    public class CarRepository : ICarRepository
    {
        private readonly CarDb _context;
        public CarRepository(CarDb context)
        {
            _context = context;
        }

        public Task<List<Car>> GetCarsAsync() => _context.Cars.ToListAsync();

        public async Task<Car> GetCarAsync(int carId) => await _context.Cars.FindAsync(new object[] { carId });

        public async Task<List<Car>> GetCarAsync(string name) =>
             await _context.Cars.Where(h => h.Name.ToLower().Contains(name.ToLower())).ToListAsync();

        public async Task<List<Car>> GetCarAsync(SearchParameters searchParameters) => 
            await _context.Cars.Where(car => 
                car.Price == searchParameters.price &&
                car.Name == searchParameters.name
                ).ToListAsync();
       

        public async Task InsertCarAsync(Car car) => await _context.AddAsync(car);

        public async Task ApdateCarAsync(Car car)
        {
            var carFromDb = await _context.Cars.FindAsync(new object[] { car.Id });
            if (carFromDb == null) return;
            carFromDb.Price = car.Price;
            carFromDb.Name = car.Name;
        }

        public async Task DeleteCarAsync(int carId)
        {
            var carFromDb = await _context.Cars.FindAsync(new object[] { carId });
            if (carFromDb == null) return;
            _context.Cars.Remove(carFromDb);
        }

        public async Task SaveAsync() => await _context.SaveChangesAsync();

        private bool _disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

       
    }
}
