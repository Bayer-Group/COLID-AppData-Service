using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using COLID.AppDataService.Common.DataModel;
using COLID.AppDataService.Common.Extensions;
using COLID.AppDataService.Repositories.Interfaces;
using COLID.AppDataService.Services.Interfaces;
using COLID.Exception.Models.Business;

namespace COLID.AppDataService.Services.Implementation
{
    public abstract class ServiceBase<TEntity> : IServiceBase<TEntity> where TEntity : class, IEntity
    {
        protected readonly IGenericRepository _repo;

        protected ServiceBase(IGenericRepository repo)
        {
            _repo = repo;
        }

        public IEnumerable<TEntity> GetAll(
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = null,
            int? skip = null,
            int? take = null,
            bool isReadOnly = false)
        {
            return _repo.GetAll(orderBy, includeProperties, skip, take, isReadOnly);
        }

        public Task<IEnumerable<TEntity>> GetAllAsync(
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = null,
            int? skip = null,
            int? take = null,
            bool isReadOnly = false)
        {
            return _repo.GetAllAsync(orderBy, includeProperties, skip, take, isReadOnly);
        }

        public IEnumerable<TEntity> GetEntities(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = null,
            int? skip = null,
            int? take = null,
            bool isReadOnly = false)
        {
            return _repo.GetEntities(filter, orderBy, includeProperties, skip, take, isReadOnly);
        }

        public Task<IEnumerable<TEntity>> GetAsync(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = null,
            int? skip = null,
            int? take = null,
            bool isReadOnly = false)
        {
            return _repo.GetAsync(filter, orderBy, includeProperties, skip, take, isReadOnly);
        }

        public TEntity GetOne(
            Expression<Func<TEntity, bool>> filter = null,
            string includeProperties = null,
            bool isReadOnly = false)
        {
            var entity = _repo.GetOne(filter, includeProperties, isReadOnly);
            if (entity.IsEmpty())
            {
                throw new EntityNotFoundException();
            }

            return entity;
        }

        public async Task<TEntity> GetOneAsync(
            Expression<Func<TEntity, bool>> filter = null,
            string includeProperties = null,
            bool isReadOnly = false)
        {
            var entity = await _repo.GetOneAsync(filter, includeProperties, isReadOnly);
            if (entity.IsEmpty())
            {
                throw new EntityNotFoundException($"Unable to find a {typeof(TEntity).FullName} for the given values");
            }

            return entity;
        }

        public bool TryGetOne(out TEntity entity,
            Expression<Func<TEntity, bool>> filter = null,
            string includeProperties = null,
            bool isReadOnly = false)
        {
            return _repo.TryGetOne(out entity, filter, includeProperties, isReadOnly);
        }

        public TEntity GetById(object id)
        {
            return _repo.GetById<TEntity>(id);
        }

        public ValueTask<TEntity> GetByIdAsync(object id)
        {
            return _repo.GetByIdAsync<TEntity>(id);
        }

        public int GetCount(Expression<Func<TEntity, bool>> filter = null)
        {
            return _repo.GetCount(filter);
        }

        public Task<int> GetCountAsync(Expression<Func<TEntity, bool>> filter = null)
        {
            return _repo.GetCountAsync(filter);
        }

        public bool GetExists(Expression<Func<TEntity, bool>> filter = null)
        {
            return _repo.GetExists(filter);
        }

        public Task<bool> GetExistsAsync(Expression<Func<TEntity, bool>> filter = null)
        {
            return _repo.GetExistsAsync(filter);
        }

        public void Create(TEntity entity)
        {
            _repo.Create(entity);
        }

        public void Update(TEntity entity)
        {
            _repo.Update(entity);
        }

        public void Delete(object id)
        {
            _repo.Delete<TEntity>(id);
        }

        public void Delete(TEntity entity)
        {
            _repo.Delete(entity);
        }

        public void DeleteRange(IEnumerable<TEntity> entities)
        {
            _repo.DeleteRange(entities);
        }

        public void Save()
        {
            _repo.Save();
        }

        public Task SaveAsync()
        {
            return _repo.SaveAsync();
        }
    }
}
