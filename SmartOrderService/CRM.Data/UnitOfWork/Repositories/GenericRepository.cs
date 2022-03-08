using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace SmartOrderService.UnitOfWork.Repositories
{
    public class GenericRepository<TEntity> where TEntity : class
    {
        /// <summary>
        /// instancia de la clase
        /// </summary>
        /// </summary>
        /// <param name="Context"></param>
        /// <returns></returns>
        public static GenericRepository<TEntity> Create(DbContext Context) => new GenericRepository<TEntity>(Context);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Context"></param>
        public GenericRepository(DbContext Context)
        {
            this.Context = Context;
            this.dbSet = Context.Set<TEntity>();
        }

        /// <summary>
        /// Retorna toda la instancia de obtejos
        /// </summary>
        /// <returns></returns>
        public IQueryable<TEntity> GetAll()
        {
            return this.dbSet;
        }

        /// <summary>
        /// 
        /// </summary>
        internal DbContext Context;

        /// <summary>
        /// 
        /// </summary>
        internal DbSet<TEntity> dbSet;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="orderBy"></param>
        /// <param name="includeProperties"></param>
        /// <returns></returns>
        public virtual IQueryable<TEntity> Get(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "")
        {
            IQueryable<TEntity> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return orderBy(query);
            }
            else
            {
                return query;
            }
        }

        /// <summary>
        /// Retorna un TEntity unsando un object id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual TEntity GetByID(object id)
        {
            return dbSet.Find(id);
        }

        /// <summary>
        /// Añade una nueva eTEntity
        /// </summary>
        /// <param name="entity"></param>
        public virtual void Insert(TEntity entity)
        {
            dbSet.Add(entity);
        }

        /// <summary>
        /// Añade un enumerable entidades.
        /// </summary>
        /// <param name="entities"></param>
        public virtual void InsertByRange(IEnumerable<TEntity> entities)
        {
            dbSet.AddRange(entities);
        }

        /// <summary>
        /// Elimina un TEntity por objeto id
        /// </summary>
        /// <param name="id"></param>
        public virtual void Delete(object id)
        {
            TEntity entityToDelete = dbSet.Find(id);
            Delete(entityToDelete);
        }

        /// <summary>
        /// Borra entidades por usando una lista de object id.
        /// </summary>
        /// <param name="Ids"></param>
        public virtual void DeleteByRange<T>(IEnumerable<T> Ids)
        {
            foreach (var id in Ids)
            {
                Delete(id);
            }
        }

        /// <summary>
        /// Elimina un TEntity
        /// </summary>
        /// <param name="entityToDelete"></param>
        public virtual void Delete(TEntity entityToDelete)
        {
            if (Context.Entry(entityToDelete).State == EntityState.Detached)
            {
                dbSet.Attach(entityToDelete);
            }
            dbSet.Remove(entityToDelete);
        }

        /// <summary>
        /// Borra una lista de entidades.
        /// </summary>
        /// <param name="entities"></param>
        public virtual void DeleteByRange(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                Delete(entity);
            }
        }

        /// <summary>
        /// Actualiza un TEntity
        /// </summary>
        /// <param name="entityToUpdate"></param>
        public virtual void Update(TEntity entityToUpdate)
        {
            dbSet.Attach(entityToUpdate);
            Context.Entry(entityToUpdate).State = EntityState.Modified;
        }

        /// <summary>
        /// Actualiza una lista de entidades
        /// </summary>
        /// <param name="entities"></param>
        public virtual void UpdateByRange(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                Update(entity);
            }
        }
    }
}