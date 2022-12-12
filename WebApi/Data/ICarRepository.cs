namespace WebApi.Data
{
    public interface ICarRepository : IDisposable
    {
        Task<List<Car>> GetCarsAsync();
        Task<Car> GetCarAsync(int carId);
        Task<List<Car>> GetCarAsync(string carName);
        Task<List<Car>> GetCarAsync(SearchParameters searchParameters);
        Task InsertCarAsync(Car car);
        Task ApdateCarAsync(Car car);
        Task DeleteCarAsync(int carId);
        Task SaveAsync();            
    }
}
