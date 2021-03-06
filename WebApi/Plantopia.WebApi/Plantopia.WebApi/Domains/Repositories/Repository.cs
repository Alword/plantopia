﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Plantopia.WebApi.Interfaces;

namespace Plantopia.WebApi.Domains.Repositories
{
    public class Repository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        private readonly DbContext context;
        protected readonly DbSet<TEntity> DbSet;

        public Repository(DbContext context)
        {
            this.context = context;
            DbSet = context.Set<TEntity>();
        }

        public async Task<TEntity> Create(TEntity item)
        {
            DbSet.Add(item);
            await context.SaveChangesAsync();
            return item;
        }

        public async Task<TEntity> FindById(int id)
        {
            return await DbSet.FindAsync(id);
        }

        public IEnumerable<TEntity> Get()
        {
            return DbSet.AsNoTracking().AsEnumerable();
        }

        public IEnumerable<TEntity> Get(Func<TEntity, bool> predicate)
        {
            return DbSet.AsNoTracking().AsEnumerable().Where(predicate).ToList();
        }

        /// <summary>
        ///     Async remove from DataBase
        /// </summary>
        /// <param name="item"></param>
        public async Task<int> Remove(params TEntity[] item)
        {
            foreach (TEntity currentItem in item) context.Entry(currentItem).State = EntityState.Deleted;
            return await context.SaveChangesAsync();
        }

        public async Task<TEntity> Update(TEntity item)
        {
            context.Entry(item).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return item;
        }

        /// <summary>
        ///     Creates an entity or returns if it exists
        /// </summary>
        /// <param name="item"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public async Task<TEntity> Stack(TEntity item, Func<TEntity, bool> predicate)
        {
            TEntity entity = Get(predicate).SingleOrDefault();

            entity = entity ?? await Create(item);

            return entity;
        }

        public bool ExistAny(Expression<Func<TEntity, bool>> predicate)
        {
            return DbSet.Any(predicate);
        }

        #region Include

        /// <summary>
        ///     Exapmle: IEnumerable<Phone> phones = phoneRepo.GetWithInclude(x=>x.Company.Name.StartsWith("S"), p=>p.Company);
        /// </summary>
        /// <param name="includeProperties"></param>
        /// <returns></returns>
        public IEnumerable<TEntity> GetWithInclude(params Expression<Func<TEntity, object>>[] includeProperties)
        {
            return Include(includeProperties).ToList();
        }

        public IEnumerable<TEntity> GetWithInclude(Func<TEntity, bool> predicate,
            params Expression<Func<TEntity, object>>[] includeProperties)
        {
            IQueryable<TEntity> query = Include(includeProperties);
            return query.Where(predicate).ToList();
        }

        private IQueryable<TEntity> Include(params Expression<Func<TEntity, object>>[] includeProperties)
        {
            IQueryable<TEntity> query = DbSet.AsNoTracking();
            return includeProperties
                .Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
        }

        #endregion
    }
}