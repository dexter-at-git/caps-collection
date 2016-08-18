using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace CapsCollection.Data.Repositories
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        IEnumerable<TEntity> GetWithRawSql(string query, params object[] parameters);

        IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "");

        TEntity GetByID(object id);

        void Insert(TEntity entity);

        void Delete(object id);

        void Delete(TEntity entityToDelete);

        void Update(TEntity entityToUpdate);

        IEnumerable<TEntity> GetAll();

        TEntity Get(int id);

        void Remove(TEntity item);

        void Add(TEntity item);

        void Merge(TEntity persisted, TEntity current);

        void Save();
    }
}