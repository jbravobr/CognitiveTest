using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using SQLiteNetExtensions.Extensions;

namespace Poc.Luis.Xamarin
{
    public class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity, new()
    {
        object _lock = new object();

        public BaseRepository()
        {
            if (App.AppSQLiteConnection == null)
            { 
                App.AppSQLiteConnection = DBContext.Instance;
                CreateDB();
            }
        }

        void CreateDB()
        {
            App.AppSQLiteConnection.CreateTable<Image>(SQLite.CreateFlags.None);
            App.AppSQLiteConnection.CreateTable<Product>(SQLite.CreateFlags.None);
        }

        /// <summary>
        /// Add the specified TEntity.
        /// </summary>
        /// <returns>The add.</returns>
        /// <param name="TEntity">TE ntity.</param>
        public void Add(T TEntity)
        {
            try
            {
                lock (_lock)
                    App.AppSQLiteConnection.InsertOrReplaceWithChildren(TEntity, true);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Delete the specified TEntity.
        /// </summary>
        /// <returns>The delete.</returns>
        /// <param name="TEntity">TE ntity.</param>
        public void Delete(T TEntity)
        {
            try
            {
                lock (_lock)
                    App.AppSQLiteConnection.Delete(TEntity, true);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Get this instance.
        /// </summary>
        /// <returns>The get.</returns>
        public T Get(int pkId)
        {
            try
            {
                lock (_lock)
                    return App.AppSQLiteConnection.GetWithChildren<T>(pkId, true);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <returns>The all.</returns>
        public List<T> GetAll()
        {
            try
            {
                lock (_lock)
                    return App.AppSQLiteConnection.GetAllWithChildren<T>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Gets all with predicate.
        /// </summary>
        /// <returns>The all with predicate.</returns>
        /// <param name="predicate">Predicate.</param>
        public List<T> GetAllWithPredicate(Expression<Func<T, bool>> predicate)
        {
            try
            {
                lock(_lock)
                    return App.AppSQLiteConnection.GetAllWithChildren(predicate, true);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Gets the with predicate.
        /// </summary>
        /// <returns>The with predicate.</returns>
        /// <param name="predicate">Predicate.</param>
        public T GetWithPredicate(Expression<Func<T, bool>> predicate)
        {
            try
            {
                lock(_lock)
                    return App.AppSQLiteConnection.GetAllWithChildren(predicate, true).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Update the specified TEntity.
        /// </summary>
        /// <returns>The update.</returns>
        /// <param name="TEntity">TE ntity.</param>
        public void Update(T TEntity)
        {
            try
            {
                lock(_lock)
                    App.AppSQLiteConnection.Update(TEntity);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}