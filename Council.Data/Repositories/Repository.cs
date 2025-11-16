using Council.Core.Entities;
using Council.Core.Interfaces;
using Council.Data.Contexts;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Council.Data.Repositories
{
    public class Repository<T> : IRepository<T> where T : BaseEntity, new()
    {
        // Fields
        protected readonly DbContext dbContext;
        

        public Repository(string contextName)
        {
            switch (contextName)
            {
                case "MainContext":
                    dbContext = ContextPerRequest.MainContext;
                    break;
            }
        }
        public List<T> Select(string query)
        {
            return dbContext.Database.SqlQuery<T>(query).ToList();
        }
        public IQueryable<T> All()
        {
            var tt= dbContext.Set<T>();
            return tt;
        }
        public T FirstOrDefault(Expression<Func<T, bool>> predicate)
        {
            return dbContext.Set<T>().FirstOrDefault(predicate);
        }
        public IQueryable<T> Where(Expression<Func<T, bool>> predicate)
        {
            return dbContext.Set<T>().Where(predicate);
        }

        public IEnumerable<T> All(int skip,int take)
        {
            var yy= dbContext.Set<T>();
            var uu = yy.OrderBy(m=>m.CreatedOn).Skip(skip).Take(take);
            return uu;
        }

        public IQueryable<T> AllAsReport()
        {
            return dbContext.Set<T>().AsNoTracking();
        }
        public void Delete(T item)
        {
            var _item = Get<string>(item.ID);
            _item.Deleted = true;
            
            Update(_item);
            Save();
        }
        public void Remove(T item)
        {
            var errorMessage="";

            try
            {
                if (item == null)
                {
                    throw new ArgumentNullException("entity");
                }

                this.dbContext.Set<T>().Remove(item);
                this.dbContext.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {

                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        errorMessage += Environment.NewLine + string.Format("Property: {0} Error: {1}",
                        validationError.PropertyName, validationError.ErrorMessage);
                    }
                }
                throw new Exception(errorMessage, dbEx);
            }
        }

        public T Get<Type>(Type id)
        {
            return dbContext.Set<T>().Find(new object[] { id });
        }

        public void Insert(T item)
        {
            dbContext.Set<T>().Add(item);
        }

        public void Save()
        {
            try
            {
                dbContext.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    string a = String.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        string b = String.Format("- Property: \"{0}\", Error: \"{1}\"",
                           ve.PropertyName, ve.ErrorMessage);
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
        }

        public void Update(T item)
        {
            dbContext.Entry(item).State = EntityState.Modified;
            //dbContext.Set<T>().Attach(item);           
        }

        public T GetAsNoTracking(string id)
        {
            return dbContext.Set<T>().AsNoTracking().FirstOrDefault(t => t.ID == id);
        }

        public void Detach(T item)
        {
            dbContext.Entry(item).State = EntityState.Detached;
        }
        
    }
}
