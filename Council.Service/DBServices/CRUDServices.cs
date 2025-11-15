using Council.Core.Entities;
using Council.Core.Interfaces;
using Council.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Council.Service.DBServices
{
    public class CRUDServices<T> : ICRUD<T> where T : BaseEntity, new()
    {
        Repository<T> repository;
        public CRUDServices()
        {
            this.repository = new Repository<T>("MainContext");
        }
        public CRUDServices(string contextName)
        {
            this.repository = new Repository<T>(contextName);
        }

        public IQueryable<T> AllAsReport()
        {
            return this.repository.AllAsReport();
        }

        public IQueryable<T> All()
        {          
             var result= this.repository.All();
            return result;
        }

        public T Create(T item, bool validate = true, bool saveChanges = true)
        {
            this.repository.Insert(item);
            if (saveChanges)
            {
                this.repository.Save();
            }
            return item;
        }

        public void Delete(string id, bool saveChanges = true)
        {
            var item = this.repository.Get<string>(id);
            this.repository.Delete(item);            
        }
        public void Remove(T item, bool saveChanges = true)
        {
            this.repository.Remove(item);
            if (saveChanges)
            {
                this.repository.Save();
            }
        }

        public T Get<Type>(Type id)
        {
            return this.repository.Get<Type>(id);
        }

        public void Save()
        {
            this.repository.Save();
        }

        public T Update(T item, bool validate = true, bool saveChanges = true)
        {
            this.repository.Update(item);
            if (saveChanges)
            {
                this.repository.Save();
            }
            return item;
        }


        public IList<T> Select(string query)
        {
            return this.repository.Select(query);
        }

        public T GetAsNoTracking(string id)
        {
            return this.repository.GetAsNoTracking(id);
        }

        public void Detach(T item)
        {
            this.repository.Detach(item);
        }
    }
}
