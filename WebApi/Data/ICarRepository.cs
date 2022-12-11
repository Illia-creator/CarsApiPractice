namespace WebApi.Data
{
    public interface ICarRepository : IDisposable
    {
        Task<List<Car>> GetCarsAsync();
        Task<Car> GetCarAsync(int carId);
        Task InsertCarAsync(Car car);
        Task ApdateCarAsync(Car car);
        Task DeleteCarAsync(int carId);
        Task SaveAsync();            
    }
}
