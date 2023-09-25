﻿using FindingPets.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace FindingPets.Data.Repositories.BaseRepositories
{
    public class BaseRepo<T> : IBaseRepo<T> where T : class
    {
        protected readonly FindingPetsDbContext context;
        private DbSet<T> _entities;

        public BaseRepo(FindingPetsDbContext context)
        {
            this.context = context;
            _entities = context.Set<T>();
        }

        public Task<T> Delete(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<T?> FindByID(Guid id)
        {
            return await _entities.FindAsync(id);
        }

        public async Task Insert(T entity)
        {
            await _entities.AddAsync(entity);
            await Update();
        }

        public async Task Update()
        {
            await context.SaveChangesAsync();
        }
    }
}