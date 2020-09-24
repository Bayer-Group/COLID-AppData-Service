using System.Collections.Generic;
using System.Threading.Tasks;
using COLID.AppDataService.Common.DataModel;
using COLID.AppDataService.Common.Exceptions;
using COLID.AppDataService.Common.Utilities;
using COLID.AppDataService.Repositories.Interface;
using COLID.AppDataService.Services.Interface;
using COLID.Exception.Models.Business;

namespace COLID.AppDataService.Services.Implementation
{
    public class GenericService<TEntityType, TIdType> : IGenericService<TEntityType, TIdType> where TEntityType : Entity<TIdType>
    {
        private readonly IGenericRepository<TEntityType, TIdType> _repo;

        public GenericService(IGenericRepository<TEntityType, TIdType> genericRepo)
        {
            _repo = genericRepo;
        }

        #region [Synchronous calls]

        public IEnumerable<TEntityType> GetAll()
        {
            return _repo.GetAll();
        }

        public TEntityType GetOne(TIdType id)
        {
            Guard.IsNotNull(id);
            return _repo.GetOne(id);
        }

        public bool TryGetOne(TIdType id, out TEntityType entity)
        {
            Guard.IsNotNull(id);
            return _repo.TryGetOne(id, out entity);
        }

        public bool Exists(TIdType id)
        {
            Guard.IsNotNull(id);
            return _repo.Exists(id);
        }

        public TEntityType Create(TEntityType entity)
        {
            Guard.IsNotNull(entity);
            if (TryGetOne(entity.Id, out var dbEntity))
            {
                throw new EntityAlreadyExistsException($"Couldn't create a new {typeof(TEntityType).Name}, because a similar entry already exists", dbEntity);
            }

            return _repo.Create(entity);
        }

        public TEntityType Update(TEntityType entity)
        {
            Guard.IsNotNull(entity);
            if (!Exists(entity.Id))
            {
                throw new EntityNotFoundException($"Unable to find {typeof(TEntityType).Name} with id {entity.Id}", entity.Id.ToString());
            }

            return _repo.Update(entity);
        }

        public void Delete(TEntityType entity)
        {
            Guard.IsNotNull(entity);
            _repo.Delete(entity);
        }

        public void DeleteRange(ICollection<TEntityType> entities)
        {
            Guard.IsNotNullOrEmpty(entities);
            _repo.DeleteRange(entities);
        }

        public void Delete(TIdType id)
        {
            Guard.IsNotNull(id);
            var entity = GetOne(id);
            _repo.Delete(entity);
        }

        #endregion [Synchronous calls]

        #region [Asynchron calls]

        public virtual async Task<IEnumerable<TEntityType>> GetAllAsync()
        {
            return await _repo.GetAllAsync();
        }

        public virtual async Task<TEntityType> GetOneAsync(TIdType id)
        {
            Guard.IsNotNull(id);
            return await _repo.GetOneAsync(id);
        }

        public virtual Task<bool> ExistsAsync(TIdType id)
        {
            Guard.IsNotNull(id);
            return _repo.ExistsAsync(id);
        }

        public virtual async Task<TEntityType> CreateAsync(TEntityType entity)
        {
            Guard.IsNotNull(entity);
            if (TryGetOne(entity.Id, out var dbEntity))
            {
                throw new EntityAlreadyExistsException($"Couldn't create a new {typeof(TEntityType).Name}, because a similar entry already exists", dbEntity);
            }

            return await _repo.CreateAsync(entity);
        }

        #endregion [Asynchron calls]
    }
}
