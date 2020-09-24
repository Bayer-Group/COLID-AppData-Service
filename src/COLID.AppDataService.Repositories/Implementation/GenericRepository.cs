using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using COLID.AppDataService.Common.DataModel;
using COLID.AppDataService.Common.Exceptions;
using COLID.AppDataService.Common.Extensions;
using COLID.AppDataService.Common.Utilities;
using COLID.AppDataService.Repositories.Context;
using COLID.AppDataService.Repositories.Interface;
using COLID.Exception.Models.Business;
using Microsoft.EntityFrameworkCore;

namespace COLID.AppDataService.Repositories.Implementation
{
    public class GenericRepository<TEntityType, TIdType> : IGenericRepository<TEntityType, TIdType>
        where TEntityType : Entity<TIdType>
    {
        protected AppDataContext Context { get; }

        protected List<Expression<Func<TEntityType, object>>> Includes { get; } = new List<Expression<Func<TEntityType, object>>>();

        public GenericRepository(AppDataContext context)
        {
            Guard.IsNotNull(context);
            Context = context;
        }

        protected virtual void AddInclude(Expression<Func<TEntityType, object>> includeExpression)
        {
            Guard.IsNotNull(includeExpression);
            Includes.Add(includeExpression);
        }

        public IQueryable<TEntityType> FindAll(bool tracked = false)
        {
            var queryable = Context.Set<TEntityType>();
            return tracked ? queryable : queryable.AsNoTracking();
        }

        public IQueryable<TEntityType> FindByCondition(Expression<Func<TEntityType, bool>> expression, bool tracked = false)
        {
            Guard.IsNotNull(expression);
            var queryable = Context.Set<TEntityType>().Where(expression);
            return tracked ? queryable : queryable.AsNoTracking();
        }

        #region [Synchronous calls]

        public IEnumerable<TEntityType> GetAll(bool tracked = false)
        {
            return Includes
                .Aggregate(FindAll(tracked), (current, include) => current.Include(include))
                .ToList();
        }

        public TEntityType GetOne(TIdType id, bool tracked = false)
        {
            Guard.IsNotNull(id);
            var resByCondition = FindByCondition(entity => entity.Id.Equals(id), tracked);
            var result = Includes
                .Aggregate(resByCondition, (current, include) => current.Include(include))
                .SingleOrDefault();

            if (result.IsEmpty())
            {
                throw new EntityNotFoundException($"Unable to find {typeof(TEntityType).Name} with id {id}", id.ToString());
            }

            return result;
        }

        public bool TryGetOne(TIdType id, out TEntityType entity, bool tracked = false)
        {
            Guard.IsNotNull(id);
            entity = FindByCondition(e => e.Id.Equals(id), tracked).SingleOrDefault();
            return entity.IsNotEmpty();
        }

        public bool Exists(TIdType id)
        {
            Guard.IsNotNull(id);
            return FindByCondition(e => e.Id.Equals(id))
                .SingleOrDefault()
                .IsNotEmpty();
        }

        public TEntityType Create(TEntityType entity)
        {
            Guard.IsNotNull(entity);
            var res = Context.Set<TEntityType>().Add(entity);
            Save();
            return res.Entity;
        }

        public TEntityType Update(TEntityType entity)
        {
            Guard.IsNotNull(entity);
            var res = Context.Set<TEntityType>().Update(entity);
            Save();
            return res.Entity;
        }

        public TEntityType UpdateReference(TEntityType entity, Expression<Func<TEntityType, EntityBase>> referenceToUpdate)
        {
            Guard.IsNotNull(entity, referenceToUpdate);
            var res = Context.Set<TEntityType>().Update(entity);
            Context.Entry(entity).Reference(referenceToUpdate).IsModified = true;
            Save();
            return res.Entity;
        }

        public TEntityType UpdateCollectionReference(TEntityType entity, Expression<Func<TEntityType, IEnumerable<EntityBase>>> collectionReferenceToUpdate)
        {
            Guard.IsNotNull(entity, collectionReferenceToUpdate);
            var res = Context.Set<TEntityType>().Update(entity);
            Context.Entry(entity).Collection(collectionReferenceToUpdate).IsModified = true;
            Save();
            return res.Entity;
        }

        public void Delete(TEntityType entity)
        {
            Guard.IsNotNull(entity);
            Context.Set<TEntityType>().Remove(entity);
            Save();
        }

        public void DeleteRange(ICollection<TEntityType> entities)
        {
            Guard.IsNotNullOrEmpty(entities);
            Context.Set<TEntityType>().RemoveRange(entities);
            Save();
        }

        public void Save()
        {
            Context.SaveChanges();
        }

        #endregion [Synchronous calls]

        #region [Asynchron calls]

        public async Task<IEnumerable<TEntityType>> GetAllAsync(bool tracked = false)
        {
            return await Includes
                .Aggregate(FindAll(tracked), (current, include) => current.Include(include))
                .ToListAsync();
        }

        public async Task<TEntityType> GetOneAsync(TIdType id, bool tracked = false)
        {
            Guard.IsNotNull(id);
            var resByCondition = FindByCondition(entity => entity.Id.Equals(id), tracked);
            var result = await Includes
                .Aggregate(resByCondition, (current, include) => current.Include(include))
                .SingleOrDefaultAsync(); // To force only one result

            if (result.IsEmpty())
            {
                throw new EntityNotFoundException($"{typeof(TEntityType).Name} with id {id} does not exist.");
            }

            return result;
        }

        public async Task<bool> ExistsAsync(TIdType id)
        {
            Guard.IsNotNull(id);
            var entity = await FindByCondition(e => e.Id.Equals(id))
                .SingleOrDefaultAsync();
            var exists = entity.IsNotEmpty();
            return exists;
        }

        public async Task<TEntityType> CreateAsync(TEntityType entity)
        {
            Guard.IsNotNull(entity);
            await Context.Set<TEntityType>().AddAsync(entity);
            await SaveAsync();
            return entity;
        }

        public async Task SaveAsync()
        {
            await Context.SaveChangesAsync();
        }

        #endregion [Asynchron calls]
    }
}
