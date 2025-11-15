using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Council.Core.Interfaces
{
    public interface IRepository<T>
    {
        // Methods
        IQueryable<T> All();
        void Delete(T item);
        T Get<Type>(Type id);
        T GetAsNoTracking(string id);
        void Insert(T item);
        void Save();
        void Update(T item);
        void Detach(T item);
    }
}
