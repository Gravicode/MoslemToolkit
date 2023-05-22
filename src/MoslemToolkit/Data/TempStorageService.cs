using System;
using System.Collections.Generic;

namespace MoslemToolkit.Data
{
    public class TempStorageService:IDisposable
    {
        public Dictionary<string,byte[]> DataStorage { get; set; }
        public TempStorageService()
        {
            DataStorage = new Dictionary<string,byte[]>();  
        }

        public byte[] GetItem(string Key)
        {
            if(DataStorage.ContainsKey(Key))
                return DataStorage[Key];
            else
                return null; 
        }
        
        public bool SetItem(string Key,byte[] Data)
        {
            try
            {
                DataStorage.Add(Key, Data);
                return true;
            }
            catch (System.Exception ex)
            {
                return false;
            }
        }

        public void Dispose()
        {
            DataStorage.Clear();
        }
    }
}
