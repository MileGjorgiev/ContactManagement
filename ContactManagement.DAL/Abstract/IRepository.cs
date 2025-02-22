namespace ContactManagement.DAL.Abstract
{
    public interface IRepository<T> where T : class
    {
        Task<List<T>> GetAllAsync();
        Task<T> GetAsync(int id);
        Task<int> SaveAsync(T entity);
        Task DeleteAsync(int id);
    }
}
