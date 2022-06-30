using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using COLID.AppDataService.Common.DataModel;

namespace COLID.AppDataService.Services.Interface
{
    public interface IServiceBase<TEntity> where TEntity : class, IEntity
    {
        IEnumerable<TEntity> GetAll(Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = null,
            int? skip = null,
            int? take = null,
            bool readOnly = false);

        Task<IEnumerable<TEntity>> GetAllAsync(
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = null,
            int? skip = null,
            int? take = null,
            bool readOnly = false);

        IEnumerable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = null,
            int? skip = null,
            int? take = null,
            bool readOnly = false);

        Task<IEnumerable<TEntity>> GetAsync(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = null,
            int? skip = null,
            int? take = null,
            bool readOnly = false);

        TEntity GetOne(
            Expression<Func<TEntity, bool>> filter = null,
            string includeProperties = null,
            bool readOnly = false);

        Task<TEntity> GetOneAsync(
            Expression<Func<TEntity, bool>> filter = null,
            string includeProperties = null,
            bool readOnly = false);

        bool TryGetOne(
            out TEntity entity,
            Expression<Func<TEntity, bool>> filter = null,
            string includeProperties = null,
            bool readOnly = false);

        TEntity GetById(object id);

        ValueTask<TEntity> GetByIdAsync(object id);

        int GetCount(Expression<Func<TEntity, bool>> filter = null);

        Task<int> GetCountAsync(Expression<Func<TEntity, bool>> filter = null);

        bool GetExists(Expression<Func<TEntity, bool>> filter = null);

        Task<bool> GetExistsAsync(Expression<Func<TEntity, bool>> filter = null);

        void Create(TEntity entity);

        void Update(TEntity entity);

        void Delete(object id);

        void Delete(TEntity entity);

        void DeleteRange(IEnumerable<TEntity> entities);

        void Save();

        Task SaveAsync();
    }
}
