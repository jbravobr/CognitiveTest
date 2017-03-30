using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Poc.Luis.Xamarin
{
    public class ApplicationServices<T> : IApplicationServices<T> where T: BaseEntity, new()
    {
        readonly IBaseRepository<T> _baseRepository;

        public ApplicationServices(IBaseRepository<T> baseRepository)
        {
            _baseRepository = baseRepository;
        }

        /// <summary>
        /// Add the specified TEntity.
        /// </summary>
        /// <returns>The add.</returns>
        /// <param name="TEntity">TE ntity.</param>
        public void Add(T TEntity)
        {
            _baseRepository.Add(TEntity);
        }

        /// <summary>
        /// Delete the specified TEntity.
        /// </summary>
        /// <returns>The delete.</returns>
        /// <param name="TEntity">TE ntity.</param>
        public void Delete(T TEntity)
        {
            _baseRepository.Delete(TEntity);
        }

        /// <summary>
        /// Get the specified pkId.
        /// </summary>
        /// <returns>The get.</returns>
        /// <param name="pkId">Pk identifier.</param>
        public T Get(int pkId)
        {
            return _baseRepository.Get(pkId);
        }

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <returns>The all.</returns>
        public List<T> GetAll()
        {
            return _baseRepository.GetAll();
        }

        /// <summary>
        /// Gets all with predicate.
        /// </summary>
        /// <returns>The all with predicate.</returns>
        /// <param name="predicate">Predicate.</param>
        public List<T> GetAllWithPredicate(Expression<Func<T, bool>> predicate)
        {
            return _baseRepository.GetAllWithPredicate(predicate);
        }

        /// <summary>
        /// Gets the with predicate.
        /// </summary>
        /// <returns>The with predicate.</returns>
        /// <param name="predicate">Predicate.</param>
        public T GetWithPredicate(Expression<Func<T, bool>> predicate)
        {
            return _baseRepository.GetWithPredicate(predicate);
        }

        /// <summary>
        /// Update the specified TEntity.
        /// </summary>
        /// <returns>The update.</returns>
        /// <param name="TEntity">TE ntity.</param>
        public void Update(T TEntity)
        {
            _baseRepository.Update(TEntity);
        }
    }
}