using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Acb.Core.Domain.Entities;

namespace Acb.Core.Domain.Repositories
{
    /// <summary> 基础仓储 </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public abstract class BaseRepository<TEntity, TKey> : IRepository<TEntity, TKey>
        where TEntity : class, IEntity<TKey>
    {
        private const string KeyField = "Id";
        private readonly IUnitOfWork _unitOfWork;
        public BaseRepository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IUnitOfWork UnitOfWork
        {
            get { return _unitOfWork; }
        }

        public abstract IQueryable<TEntity> Table { get; }
        public abstract TKey Insert(TEntity entity);

        public virtual int Insert(IEnumerable<TEntity> entities)
        {
            return entities.Select(Insert).Count(key => key != null);
        }

        public abstract int Delete(TEntity entity);

        public abstract int Delete(TKey key);

        public abstract int Delete(Expression<Func<TEntity, bool>> expression);

        public abstract int Update(TEntity entity);
        public abstract int Update(TEntity entity, params string[] parms);
        public int Update(TEntity entity, Expression<Func<TEntity, bool>> expression, params string[] parms)
        {
            return Update(entity, Where(expression), parms);
        }

        public abstract int Update(TEntity entity, IQueryable<TEntity> entities, params string[] parms);
        public abstract int Update(Expression<Func<TEntity, dynamic>> propExpression, params TEntity[] entities);

        public virtual bool Exists(Expression<Func<TEntity, bool>> expression)
        {
            return Table.Any(expression);
        }

        public virtual TEntity Load(TKey key)
        {
            var express = CreateEqualityExpressionForId(key);
            return Table.SingleOrDefault(express);
        }
        public TEntity First(Expression<Func<TEntity, bool>> expression)
        {
            return Table.First(expression);
        }

        public TEntity FirstOrDefault(Expression<Func<TEntity, bool>> expression)
        {
            return Table.FirstOrDefault(expression);
        }

        public TEntity Single(Expression<Func<TEntity, bool>> expression)
        {
            return Table.Single(expression);
        }

        public TEntity SingleOrDefault(Expression<Func<TEntity, bool>> expression)
        {
            return Table.SingleOrDefault(expression);
        }

        public IQueryable<TEntity> List(IEnumerable<TKey> keys)
        {
            return Table.Where(t => keys.Contains(t.Id));
        }

        public IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> expression)
        {
            return Table.Where(expression);
        }

        public DResults<TEntity> PageList(IOrderedQueryable<TEntity> ordered, DPage page)
        {
            if (ordered == null)
                return DResult.Errors<TEntity>("数据查询异常！");
            var result = ordered.Skip(page.Page * page.Size).Take(page.Size).ToList();
            var total = ordered.Count();
            return DResult.Succ(result, total);
        }

        public int Count()
        {
            return Table.Count();
        }

        public int Count(Expression<Func<TEntity, bool>> predicate)
        {
            return Table.Count(predicate);
        }

        public long LongCount()
        {
            return Table.LongCount();
        }

        public long LongCount(Expression<Func<TEntity, bool>> predicate)
        {
            return Table.LongCount(predicate);
        }

        public TValue Max<TValue>(Func<TEntity, TValue> perdicate, Expression<Func<TEntity, bool>> condition = null)
        {
            var data = condition == null ? Table : Table.Where(condition);
            if (data == null || !data.Any())
                return default(TValue);
            return data.Max(perdicate);
        }

        public TValue Min<TValue>(Func<TEntity, TValue> perdicate, Expression<Func<TEntity, bool>> condition = null)
        {
            var data = condition == null ? Table : Table.Where(condition);
            if (data == null || !data.Any())
                return default(TValue);
            return data.Min(perdicate);
        }

        public double Average(Func<TEntity, int> perdicate, Expression<Func<TEntity, bool>> condition = null)
        {
            var data = condition == null ? Table : Table.Where(condition);
            if (data == null || !data.Any())
                return 0;
            return data.Average(perdicate);
        }

        public double Average(Func<TEntity, long> perdicate, Expression<Func<TEntity, bool>> condition = null)
        {
            var data = condition == null ? Table : Table.Where(condition);
            if (data == null || !data.Any())
                return 0;
            return data.Average(perdicate);
        }

        public double Average(Func<TEntity, double> perdicate, Expression<Func<TEntity, bool>> condition = null)
        {
            var data = condition == null ? Table : Table.Where(condition);
            if (data == null || !data.Any())
                return 0;
            return data.Average(perdicate);
        }

        public decimal Average(Func<TEntity, decimal> perdicate, Expression<Func<TEntity, bool>> condition = null)
        {
            var data = condition == null ? Table : Table.Where(condition);
            if (data == null || !data.Any())
                return 0;
            return data.Average(perdicate);
        }

        public float Average(Func<TEntity, float> perdicate, Expression<Func<TEntity, bool>> condition = null)
        {
            var data = condition == null ? Table : Table.Where(condition);
            if (data == null || !data.Any())
                return 0;
            return data.Average(perdicate);
        }

#if NET45

        public Task<TKey> InsertAsync(TEntity entity)
        {
            return Task.FromResult(Insert(entity));
        }

        public Task<int> InsertAsync(IEnumerable<TEntity> entities)
        {
            return Task.FromResult(Insert(entities));
        }

        public Task<int> DeleteAsync(TEntity entity)
        {
            return Task.FromResult(Delete(entity));
        }

        public Task<int> DeleteAsync(TKey key)
        {
            return Task.FromResult(Delete(key));
        }

        public Task<int> DeleteAsync(Expression<Func<TEntity, bool>> expression)
        {
            return Task.FromResult(Delete(expression));
        }

        public Task<int> UpdateAsync(TEntity entity)
        {
            return Task.FromResult(Update(entity));
        }

        public Task<int> UpdateAsync(TEntity entity, params string[] parms)
        {
            return Task.FromResult(Update(entity, parms));
        }

        public Task<int> UpdateAsync(TEntity entity, Expression<Func<TEntity, bool>> expression, params string[] parms)
        {
            return Task.FromResult(Update(entity, expression, parms));
        }

        public Task<int> UpdateAsync(TEntity entity, IQueryable<TEntity> entities, params string[] parms)
        {
            return Task.FromResult(Update(entity, entities, parms));
        }

        public Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> expression)
        {
            return Task.FromResult(Exists(expression));
        }

        public Task<TEntity> LoadAsync(TKey key)
        {
            return Task.FromResult(Load(key));
        }

        public Task<IQueryable<TEntity>> ListAsync(IEnumerable<TKey> keys)
        {
            return Task.FromResult(List(keys));
        }

        public Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> expression)
        {
            return Task.FromResult(FirstOrDefault(expression));
        }

        public Task<TEntity> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> expression)
        {
            return Task.FromResult(SingleOrDefault(expression));
        }

        public Task<IQueryable<TEntity>> WhereAsync(Expression<Func<TEntity, bool>> expression)
        {
            return Task.FromResult(Where(expression));
        }

        public Task<DResults<TEntity>> PageListAsync(IOrderedQueryable<TEntity> ordered, DPage page)
        {
            return Task.FromResult(PageList(ordered, page));
        }

        public Task<int> CountAsync()
        {
            return Task.FromResult(Count());
        }

        public Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return Task.FromResult(Count(predicate));
        }

        public Task<long> LongCountAsync()
        {
            return Task.FromResult(LongCount());
        }

        public Task<long> LongCountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return Task.FromResult(LongCount(predicate));
        }

#endif

        protected virtual Expression<Func<TEntity, bool>> CreateEqualityExpressionForId(TKey id,
            string keyColumn = KeyField)
        {
            var lambdaParam = Expression.Parameter(typeof(TEntity));

            var lambdaBody = Expression.Equal(
                Expression.PropertyOrField(lambdaParam, keyColumn),
                Expression.Constant(id, typeof(TKey))
                );

            return Expression.Lambda<Func<TEntity, bool>>(lambdaBody, lambdaParam);
        }

    }
}
