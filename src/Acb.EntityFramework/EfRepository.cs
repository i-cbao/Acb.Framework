using Acb.Core.Domain;
using Acb.Core.Domain.Entities;
using Acb.Core.Domain.Repositories;
using Acb.Core.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Acb.EntityFramework
{
    public class EfRepository<TDbContext, TEntity, TKey>
        : BaseRepository<TEntity, TKey>
        where TEntity : BaseEntity<TKey>
        where TDbContext : DbContext, IUnitOfWork
    {
        public EfRepository(TDbContext unitOfWork) : base(unitOfWork)
        {
        }

        public virtual TDbContext Context => (TDbContext)UnitOfWork;


        public virtual DbSet<TEntity> Table => ((DbContext)UnitOfWork).Set<TEntity>();

        public override IQueryable<TEntity> GetAll() => Table.AsNoTracking();

        public override TKey Insert(TEntity entity)
        {
            var item = Table.Add(entity);
            return SaveChanges() > 0 ? item.Entity.Id : default(TKey);
        }

        public override int Insert(IEnumerable<TEntity> entities)
        {
            Table.AddRange(entities);
            return SaveChanges();
        }

        public override int Delete(TEntity entity)
        {
            AttachIfNot(entity);
            Table.Remove(entity);
            return SaveChanges();
        }

        public override int Delete(TKey key)
        {
            var entity = Table.Local.FirstOrDefault(t => EqualityComparer<TKey>.Default.Equals(t.Id, key));
            if (entity == null)
            {
                entity = Load(key);
                if (entity == null)
                    return 0;
            }
            AttachIfNot(entity);
            Table.Remove(entity);
            return SaveChanges();
        }

        public override int Delete(Expression<Func<TEntity, bool>> expression)
        {
            var entities = Where(expression);
            foreach (var entity in entities)
            {
                AttachIfNot(entity);
            }

            Table.RemoveRange(entities);
            return SaveChanges();
        }

        public override int Update(TEntity entity)
        {
            AttachIfNot(entity);
            Context.Entry(entity).State = EntityState.Modified;
            return SaveChanges();
        }

        private void Update(IEnumerable<TEntity> entities, ICollection<string> props)
        {
            entities.Foreach(t =>
            {
                AttachIfNot(t);
                var entry = Context.Entry(t);
                entry.State = EntityState.Unchanged;
                foreach (var member in props)
                {
                    entry.Property(member).IsModified = true;
                }
            });
        }

        public override int Update(TEntity entity, params string[] parms)
        {
            if (entity == null || parms == null || parms.Length == 0)
                return 0;
            Update(new[] { entity }, parms);
            return SaveChanges();
        }

        public override int Update(TEntity entity, IQueryable<TEntity> entities, params string[] parms)
        {
            if (parms == null || parms.Length == 0)
                return 0;
            var props = entity.GetType().GetProperties().Where(p => parms.Contains(p.Name)).ToList();
            if (!props.Any())
                return 0;
            var list = entities.ToList();
            list.Foreach(t =>
            {
                foreach (var prop in props)
                {
                    prop.SetValue(t, prop.GetValue(entity, null));
                }
            });
            Update(list, parms);
            return SaveChanges();
        }

        public override int Update(Expression<Func<TEntity, dynamic>> propExpression, params TEntity[] entities)
        {
            if (entities == null || entities.Length == 0)
                return 0;
            ReadOnlyCollection<MemberInfo> memberInfos = ((dynamic)propExpression.Body).Members;
            if (memberInfos == null || !memberInfos.Any())
                return 0;
            Update(entities, memberInfos.Select(p => p.Name).ToList());
            return SaveChanges();
        }

        protected virtual void AttachIfNot(TEntity entity)
        {
            var entry = Context.ChangeTracker.Entries().FirstOrDefault(ent => ent.Entity == entity);
            if (entry != null)
            {
                return;
            }

            Table.Attach(entity);
        }


        private int SaveChanges()
        {
            return UnitOfWork.IsTransaction ? 0 : ((DbContext)UnitOfWork).SaveChanges();
        }
    }
}
