using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Poc.Luis.Xamarin
{
    public interface IBaseRepository<T>
    {
        void Add(T TEntity);
        void Delete(T TEntity);
        void Update(T TEntity);

        T Get(int pkId);
        T GetWithPredicate(Expression<Func<T, bool>> predicate);

        List<T> GetAll();
        List<T> GetAllWithPredicate(Expression<Func<T, bool>> predicate);
    }
}
