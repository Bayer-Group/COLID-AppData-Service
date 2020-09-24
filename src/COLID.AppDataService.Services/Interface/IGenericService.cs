using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using COLID.AppDataService.Common.Exceptions;

namespace COLID.AppDataService.Services.Interface
{
    /// <summary>
    /// Service including base operations for all implementing classes.
    /// </summary>
    /// <typeparam name="TEntityType">the generic entity type</typeparam>
    /// <typeparam name="TIdType">the generic id type</typeparam>
    public interface IGenericService<TEntityType, in TIdType>
    {
        /// <summary>
        /// Read all entries and return them as an IEnumerable.
        /// To fetch subclasses of an entity, the Includes member variable has to be used.
        /// </summary>
        IEnumerable<TEntityType> GetAll();

        /// <summary>
        /// Find one entity by a given id and returns it.
        /// </summary>
        /// <param name="id">the entity id to search for</param>
        /// <exception cref="EntityNotFoundException">In case that no entity was found for the given id</exception>
        TEntityType GetOne(TIdType id);

        /// <summary>
        /// Check if a entity exists for the given id and writes in into the out param.
        /// If no entity was found, it will be empty.
        /// </summary>
        /// <param name="id">the entity id to search for</param>
        /// <param name="entity">the found entity otherwise null</param>
        /// <return>true if entity was found, otherwise false</return>
        bool TryGetOne(TIdType id, out TEntityType entity);

        /// <summary>
        /// Check if a entity exists for the given id.
        /// </summary>
        /// <param name="id">the entity id to search for</param>
        /// <return>true if entity exists, otherwise false</return>
        bool Exists(TIdType id);

        /// <summary>
        /// Create a new entity from the given parameter.
        /// </summary>
        /// <param name="entity">the entity to create</param>
        TEntityType Create(TEntityType entity);

        /// <summary>
        /// Updates a given entity.
        /// </summary>
        /// <param name="entity">the entity to update</param>
        TEntityType Update(TEntityType entity);

        /// <summary>
        /// Deletes an entity.
        /// </summary>
        /// <param name="entity">the entity to delete</param>
        /// <exception cref="EntityNotFoundException">In case that no entity was found</exception>
        void Delete(TEntityType entity);

        /// <summary>
        /// Deletes a range of entities.
        /// </summary>
        /// <param name="entities">the entities to delete</param>
        /// <exception cref="EntityNotFoundException">In case that no entity was found</exception>
        void DeleteRange(ICollection<TEntityType> entities);

        /// <summary>
        /// Deletes an entity, that matches to the the given id.
        /// </summary>
        /// <param name="id">the id of the entity to delete</param>
        /// <exception cref="EntityNotFoundException">In case that no entity was found for the given id</exception>
        /// <exception cref="ArgumentNullException">if the argument is null</exception>
        void Delete(TIdType id);

        /// <summary>
        /// Read all entries asynchronously from the repo and return them.
        /// </summary>
        Task<IEnumerable<TEntityType>> GetAllAsync();

        /// <summary>
        /// Find one entity asynchronously by a given id and returns it.
        /// </summary>
        /// <param name="id">the entity id to search for</param>
        /// <exception cref="EntityNotFoundException">In case that no entity was found for the given id</exception>
        Task<TEntityType> GetOneAsync(TIdType id);

        /// <summary>
        /// Create a new entity asynchronously from the given parameter.
        /// </summary>
        /// <param name="entity">the entity to create</param>
        Task<TEntityType> CreateAsync(TEntityType entity);

        /// <summary>
        /// Check if a entity exists asynchronously for the given id.
        /// </summary>
        /// <param name="id">the entity id to search for</param>
        /// <return>true if entity exists, otherwise false</return>
        Task<bool> ExistsAsync(TIdType id);
    }
}
