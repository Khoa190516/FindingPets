namespace FindingPets.Data.Repositories.BaseRepositories
{
    public interface IBaseRepo<T> where T : class
    {
        Task<T?> FindByID(Guid id);
        Task Insert(T entity);
        Task<T> Delete(Guid id);
        Task Update();
    }
}
