using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using COLID.AppDataService.Common.DataModel;
using COLID.AppDataService.Common.Extensions;
using COLID.AppDataService.Common.Utilities;
using COLID.AppDataService.Repositories.Context;
using COLID.AppDataService.Repositories.Interfaces;
using COLID.Exception.Models.Business;
using Microsoft.EntityFrameworkCore;

namespace COLID.AppDataService.Repositories.Implementation
{
    public class GenericRepository : IGenericRepository
    {
        private readonly AppDataContext _context;

        public GenericRepository(AppDataContext context)
        {
            _context = context;
        }

        protected virtual IQueryable<TEntity> GetQueryable<TEntity>(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = null,
            int? skip = null,
            int? take = null,
            bool isReadOnly = false) where TEntity : class, IEntity
        {
            includeProperties ??= string.Empty;

            IQueryable<TEntity> query = _context.Set<TEntity>();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split(',', StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            if (skip.HasValue)
            {
                query = query.Skip(skip.Value);
            }

            if (take.HasValue)
            {
                query = query.Take(take.Value);
            }

            if (isReadOnly)
                query = query.AsNoTracking();

            return query;
        }

        public virtual IEnumerable<TEntity> GetAll<TEntity>(
          Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
          string includeProperties = null,
          int? skip = null,
          int? take = null,
          bool IsReadOnly = false) where TEntity : class, IEntity
        {
            return GetQueryable(null, orderBy, includeProperties, skip, take, IsReadOnly).ToList();
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync<TEntity>(
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = null,
            int? skip = null,
            int? take = null,
            bool IsReadOnly = false) where TEntity : class, IEntity
        {
            return await GetQueryable(null, orderBy, includeProperties, skip, take, IsReadOnly).ToListAsync();
        }

        public virtual IEnumerable<TEntity> GetEntities<TEntity>(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = null,
            int? skip = null,
            int? take = null,
            bool IsReadOnly = false) where TEntity : class, IEntity
        {
            var entities = GetQueryable(filter, orderBy, includeProperties, skip, take, IsReadOnly).ToList();
            if (entities.IsNullOrEmpty())
            {
                throw new EntityNotFoundException($"Unable to find {typeof(TEntity).Name}s to the given parameters", string.Empty);
            }

            return entities;
        }

        public virtual async Task<IEnumerable<TEntity>> GetAsync<TEntity>(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = null,
            int? skip = null,
            int? take = null,
            bool IsReadOnly = false) where TEntity : class, IEntity
        {
            var entities = await GetQueryable(filter, orderBy, includeProperties, skip, take, IsReadOnly).ToListAsync();
            if (entities.IsNullOrEmpty())
            {
                throw new EntityNotFoundException($"Unable to find {typeof(TEntity).Name}s to the given parameters", string.Empty);
            }

            return entities;
        }

        public virtual TEntity GetOne<TEntity>(
            Expression<Func<TEntity, bool>> filter = null,
            string includeProperties = "",
            bool IsReadOnly = false) where TEntity : class, IEntity
        {
            var entity = GetQueryable(filter, null, includeProperties, null, null, IsReadOnly).SingleOrDefault();
            if (entity.IsEmpty())
            {
                throw new EntityNotFoundException($"Unable to find a {typeof(TEntity).Name} to the given parameters", string.Empty);
            }

            return entity;
        }

        public virtual async Task<TEntity> GetOneAsync<TEntity>(
            Expression<Func<TEntity, bool>> filter = null,
            string includeProperties = null,
            bool IsReadOnly = false) where TEntity : class, IEntity
        {
            var entity = await GetQueryable(filter, null, includeProperties, null, null, IsReadOnly).SingleOrDefaultAsync();
            if (entity.IsEmpty())
            {
                throw new EntityNotFoundException($"Unable to find a {typeof(TEntity).Name} to the given parameters", string.Empty);
            }

            return entity;
        }

        public virtual bool TryGetOne<TEntity>(
            out TEntity entity,
            Expression<Func<TEntity, bool>> filter = null,
            string includeProperties = null,
            bool IsReadOnly = false) where TEntity : class, IEntity
        {
            entity = null;
            try
            {
                entity = GetOne(filter, includeProperties, IsReadOnly);
            }
            catch (EntityNotFoundException)
            {
                // ingore this exception
            }

            return entity.IsNotEmpty();
        }

        public virtual TEntity GetById<TEntity>(object id) where TEntity : class, IEntity
        {
            Guard.IsNotNull(id);
            var entity = _context.Set<TEntity>().Find(id);
            if (entity.IsEmpty())
            {
                throw new EntityNotFoundException($"Unable to find a {typeof(TEntity).Name} to the given id", id.ToString());
            }

            return entity;
        }

        public virtual async ValueTask<TEntity> GetByIdAsync<TEntity>(object id) where TEntity : class, IEntity
        {
            Guard.IsNotNull(id);
            var entity = await _context.Set<TEntity>().FindAsync(id);
            if (entity.IsEmpty())
            {
                throw new EntityNotFoundException($"Unable to find a {typeof(TEntity).Name} to the given id", id.ToString());
            }

            return entity;
        }

        public virtual int GetCount<TEntity>(Expression<Func<TEntity, bool>> filter = null) where TEntity : class, IEntity
        {
            const bool readOnly = true;
            return GetQueryable(filter,null, null, null, null, readOnly).Count();
        }

        public virtual Task<int> GetCountAsync<TEntity>(Expression<Func<TEntity, bool>> filter = null) where TEntity : class, IEntity
        {
            const bool readOnly = true;
            return GetQueryable(filter, null, null, null, null, readOnly).CountAsync();
        }

        public virtual bool GetExists<TEntity>(Expression<Func<TEntity, bool>> filter = null) where TEntity : class, IEntity
        {
            const bool readOnly = true;
            return GetQueryable(filter, null, null, null, null, readOnly).Any();
        }

        public virtual Task<bool> GetExistsAsync<TEntity>(Expression<Func<TEntity, bool>> filter = null) where TEntity : class, IEntity
        {
            const bool readOnly = true;
            return GetQueryable(filter, null, null, null, null, readOnly).AnyAsync();
        }

        public virtual void Create<TEntity>(TEntity entity) where TEntity : class, IEntity
        {
            Guard.IsNotNull(entity);
            _context.Set<TEntity>().Add(entity);
        }

        public virtual void Update<TEntity>(TEntity entity) where TEntity : class, IEntity
        {
            Guard.IsNotNull(entity);
            _context.Set<TEntity>().Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }

        public virtual void Delete<TEntity>(object id) where TEntity : class, IEntity
        {
            Guard.IsNotNull(id);
            var result = _context.Set<TEntity>().Find(id);
            if (result.IsEmpty())
            {
                throw new EntityNotFoundException($"Unable to delete a {typeof(TEntity).Name} to the given id, because it doesn't exist", id.ToString());
            }

            Delete(result);
        }

        public virtual void Delete<TEntity>(TEntity entity) where TEntity : class, IEntity
        {
            Guard.IsNotNull(entity);
            var dbSet = _context.Set<TEntity>();
            if (_context.Entry(entity).State == EntityState.Detached)
            {
                dbSet.Attach(entity);
            }
            dbSet.Remove(entity);
        }

        public virtual void DeleteRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class, IEntity
        {
            Guard.IsNotNullOrEmpty(entities);
            var dbSet = _context.Set<TEntity>();
            var enumerable = entities.ToList();
            foreach (var entity in enumerable
                .Where(entity => _context.Entry(entity).State == EntityState.Detached))
            {
                dbSet.Attach(entity);
            }

            dbSet.RemoveRange(enumerable);
        }

        public virtual void Save()
        {
            _context.SaveChanges();
        }

        public virtual Task SaveAsync()
        {
            return _context.SaveChangesAsync();
        }
    }
}
