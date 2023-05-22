using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoslemToolkit.Data
{
    public interface ICrud<T> where T : class
    {
        bool InsertData(T data);
        bool UpdateData(T data);
        List<T> GetAllData();
        T GetDataById(object Id);
        bool DeleteData(object Id);
        long GetLastId();
        List<T> FindByKeyword(string Keyword);
    }
}
