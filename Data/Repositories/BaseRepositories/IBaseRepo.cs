namespace FindingPets.Data.Repositories.BaseRepositories
{
    public interface IBaseRepo<T> where T : class
    {
        Task<T?> FindByID(Guid id);
        Task<int> Insert(T entity);
        Task<bool> Delete(Guid id);
        Task<int> Update();

        void Add(T entity);
        void Remove(T entity);
        void Edit(T entity);
    }
}
