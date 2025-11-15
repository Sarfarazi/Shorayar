using Council.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Council.Core.Interfaces
{
    public interface ICRUD<T> where T : BaseEntity, new()
    {
        IQueryable<T> All();
        T Create(T item, bool validate = true, bool saveChanges = true);
        void Delete(string id, bool saveChanges = true);
        T Get<Type>(Type id);
        T GetAsNoTracking(string id);
        void Save();
        T Update(T item, bool validate = true, bool saveChanges = true);
        IList<T> Select(string query);
        void Detach(T item);
    }
}
