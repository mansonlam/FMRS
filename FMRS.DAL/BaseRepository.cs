using FMRS.Model.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace FMRS.DAL
{
    public interface IBaseRepository<T> where T : class
    {
        List<T> FindAll();
        T FindById(params object[] key);
        EntityEntry<T> Create(T entity);
        EntityEntry<T> Delete(T entity);
        EntityEntry<T> DeleteById(params object[] keys);
        void DeleteRange(IEnumerable<T> enities);
        //https://weblogs.asp.net/dixin/entity-framework-core-and-linq-to-entities-7-data-changes-and-transactions
        //https://codingblast.com/entity-framework-core-generic-repository/

    }


    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        private FMRSContext Context;
        public BaseRepository(FMRSContext _context)
        {
            Context = _context;
        }

        /// <summary>
        /// Find all records in an entity
        /// </summary>
        /// <returns>all records in an entity</returns>
        public List<T> FindAll()
        {
            return Context.Set<T>().ToList();
        }

        /// <summary>
        /// Find a user by primary key
        /// </summary>
        /// <param name="key">primary key</param>
        /// <returns>a matched entity</returns>
        public T FindById(params object[] key)
        {
            return Context.Set<T>().Find(key);
        }

        public EntityEntry<T> Create(T entity)
        {
            return Context.Set<T>().Add(entity);
        }

        /// <summary>
        /// Remove the entity from context
        /// </summary>
        /// <param name="entity">the entity to be removed</param>
        /// <returns>the removed entity</returns>
        public EntityEntry<T> Delete(T entity)
        {
            return Context.Set<T>().Remove(entity);
        }

        /// <summary>
        /// Remove the entity from context by entity id
        /// </summary>
        /// <param name="keys">the id of entity to be removed</param>
        /// <returns>the removed entity</returns>
        public EntityEntry<T> DeleteById(params object[] keys)
        {
            var entity = this.FindById(keys);
            return Context.Set<T>().Remove(entity);
        }

        /// <summary>
        /// Remove range of entities from context
        /// </summary>
        /// <param name="enities">the removed entities</param>
        /// <returns>the removed entities</returns>
        public void DeleteRange(IEnumerable<T> enities)
        {
            Context.Set<T>().RemoveRange(enities);
        }


    }
}
