using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using COLID.AppDataService.Common.DataModel;
using COLID.AppDataService.Common.Exceptions;

namespace COLID.AppDataService.Repositories.Interface
{
    /// <summary>
    /// Repository including base operations for all implementing classes.
    /// </summary>
    /// <typeparam name="TEntityType">the generic entity type</typeparam>
    /// <typeparam name="TIdType">the generic id type</typeparam>
    public interface IGenericRepository<TEntityType, in TIdType>
    {
        /// <summary>
        /// Fetches all entries from the database and returns them as a queryable result.
        /// <b>NOTE:</b> Find all doesn't include the <code>Includes</code>
        /// <b>WARNING!</b> The tracked field is required to get modifiable queries,
        ///                 because changes aren't tracked due to performance by default.
        /// </summary>
        IQueryable<TEntityType> FindAll(bool tracked = false);

        /// <summary>
        /// Find all entries from the database, filtered by a given expression and return them as a queryable result.
        /// <b>WARNING!</b> The tracked field is required to get modifiable queries,
        ///                 because changes aren't tracked due to performance by default.
        /// </summary>
        /// <param name="expression">the expression to filter by</param>
        /// <param name="tracked">param to get a modifiable result instead of read only</param>
        /// <exception cref="ArgumentNullException">if expression is null</exception>
        IQueryable<TEntityType> FindByCondition(Expression<Func<TEntityType, bool>> expression, bool tracked = false);

        /// <summary>
        /// Read all entries from the database and return them as an IEnumerable.
        /// To fetch subclasses of an entity, the Includes member variable has to be used.
        /// </summary>
        /// <param name="tracked">param to get a modifiable result instead of read only</param>
        IEnumerable<TEntityType> GetAll(bool tracked = false);

        /// <summary>
        /// Find one entity by a given id and returns it.
        /// <b>WARNING!</b> The tracked field is required to get modifiable queries,
        ///                 because changes aren't tracked due to performance by default.
        /// </summary>
        /// <param name="id">the entity id to search for</param>
        /// <param name="tracked">param to get a modifiable result instead of read only</param>
        /// <exception cref="ArgumentNullException">if the argument is null</exception>
        /// <exception cref="EntityNotFoundException">in case that no entity was found for the given id</exception>
        TEntityType GetOne(TIdType id, bool tracked = false);

        /// <summary>
        /// Check if a entity exists for the given id and writes in into the out param.
        /// If no entity was found, it will be empty.
        /// <b>WARNING!</b> The tracked field is required to get modifiable queries,
        ///                 because changes aren't tracked due to performance by default.
        /// </summary>
        /// <param name="id">the entity id to search for</param>
        /// <param name="entity">the found entity otherwise null</param>
        /// <param name="tracked">param to get a modifiable result instead of read only</param>
        /// <return>true if entity was found, otherwise false</return>
        /// <exception cref="ArgumentNullException">if the argument is null</exception>
        /// <exception cref="InvalidOperationException">if more than one result was found</exception>
        bool TryGetOne(TIdType id, out TEntityType entity, bool tracked = false);

        /// <summary>
        /// Check if a entity exists for the given id.
        /// </summary>
        /// <param name="id">the entity id to search for</param>
        /// <return>true if entity exists, otherwise false</return>
        /// <exception cref="ArgumentNullException">if the argument is null</exception>
        /// <exception cref="InvalidOperationException">if more than one result was found</exception>
        bool Exists(TIdType id);

        /// <summary>
        /// Create a new entity from the given parameter.
        /// </summary>
        /// <param name="entity">the entity to create</param>
        /// <exception cref="ArgumentNullException">if the argument is null</exception>
        TEntityType Create(TEntityType entity);

        /// <summary>
        /// Updates a given entity.<br />
        /// <b>WARNING:</b> Updating relations to null-values will be ignored! To use this, use UpdateReference
        /// </summary>
        /// <param name="entity">the entity to update</param>
        /// <exception cref="ArgumentNullException">if the argument is null</exception>
        TEntityType Update(TEntityType entity);

        /// <summary>
        /// Updates the reference to the entity, identified by the given expression.<br />
        /// <b>NOTE:</b> The referenced value can be set to null.
        /// </summary>
        /// <param name="entity">The entity to update</param>
        /// <param name="referenceToUpdate">The reference to update</param>
        TEntityType UpdateReference(TEntityType entity, Expression<Func<TEntityType, EntityBase>> referenceToUpdate);

        /// <summary>
        /// Updates the collection reference to the entity, identified by the given expression.<br />
        /// <b>NOTE:</b> The referenced value can be set to null.
        /// </summary>
        /// <param name="entity">The entity to update</param>
        /// <param name="collectionReferenceToUpdate">The collection reference to update</param>
        TEntityType UpdateCollectionReference(TEntityType entity, Expression<Func<TEntityType, IEnumerable<EntityBase>>> collectionReferenceToUpdate);

        /// <summary>
        /// Deletes an entity.
        /// </summary>
        /// <param name="entity">the entity to delete</param>
        /// <exception cref="ArgumentNullException">if the argument is null</exception>
        void Delete(TEntityType entity);

        /// <summary>
        /// Deletes a range of entities.
        /// </summary>
        /// <param name="entities">the entities to delete</param>
        /// <exception cref="ArgumentNullException">if the argument is null or empty</exception>
        void DeleteRange(ICollection<TEntityType> entities);

        /// <summary>
        /// Save (commit) all operations to the database.
        /// </summary>
        void Save();

        /// <summary>
        /// Read all entries asynchronously from the database and return them as an IEnumerable.
        /// <b>WARNING!</b> The tracked field is required to get modifiable queries,
        ///                 because changes aren't tracked due to performance by default.
        /// <param name="tracked">param to get a modifiable result instead of read only</param>
        /// </summary>
        Task<IEnumerable<TEntityType>> GetAllAsync(bool tracked = false);

        /// <summary>
        /// Find one entity asynchronously by a given id and returns it.
        /// <b>WARNING!</b> The tracked field is required to get modifiable queries,
        ///                 because changes aren't tracked due to performance by default.
        /// </summary>
        /// <param name="id">the entity id to search for</param>
        /// <param name="tracked">param to get a modifiable result instead of read only</param>
        /// <exception cref="ArgumentNullException">if the argument is null</exception>
        /// <exception cref="EntityNotFoundException">in case that no entity was found for the given id</exception>
        Task<TEntityType> GetOneAsync(TIdType id, bool tracked = false);

        /// <summary>
        /// Check if a entity exists asynchronously for the given id.
        /// </summary>
        /// <param name="id">the entity id to search for</param>
        /// <return>true if entity exists, otherwise false</return>
        /// <exception cref="ArgumentNullException">if the argument is null</exception>
        Task<bool> ExistsAsync(TIdType id);

        /// <summary>
        /// Create a new entity asynchronously from the given parameter.
        /// </summary>
        /// <param name="entity">the entity to create</param>
        /// <exception cref="ArgumentNullException">if the argument is null</exception>
        Task<TEntityType> CreateAsync(TEntityType entity);

        /// <summary>
        /// Save (commit) all operations asynchronously to the database.
        /// </summary>
        Task SaveAsync();
    }
}
