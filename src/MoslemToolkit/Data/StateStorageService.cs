using GemBox.Spreadsheet;
using Microsoft.EntityFrameworkCore;
using MoslemToolkit.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace MoslemToolkit.Data
{
    public class StateStorageService : ICrud<StateStorage>
    {
        NgajiDB db;

        public StateStorageService()
        {
            if (db == null) db = new NgajiDB();

        }
        public bool DeleteData(object Id)
        {
            var selData = (db.StateStorages.Where(x => x.Id == (long)Id).FirstOrDefault());
            db.StateStorages.Remove(selData);
            db.SaveChanges();
            return true;
        }

        public List<StateStorage> FindByKeyword(string Keyword)
        {
            var data = from x in db.StateStorages
                       where x.NameKey.Contains(Keyword)
                       select x;
            return data.ToList();
        }

        public List<StateStorage> GetAllData()
        {
            return db.StateStorages.ToList();
        }

        public bool IsKeyExist(string NameKey)
        {
            var exist = db.StateStorages.Any(x => x.NameKey == NameKey);
            return exist;
        }
        public StateStorage GetDataByKey(string NameKey)
        {
            var exist = db.StateStorages.Any(x => x.NameKey == NameKey);
            if (!exist) return null;
            return db.StateStorages.Where(x => x.NameKey == NameKey).FirstOrDefault();
        }

        public bool SetData(StateStorage data)
        {
            var hasil = false;
            var exist = db.StateStorages.Any(x => x.NameKey == data.NameKey);
            if (exist)
            {
                var item = db.StateStorages.Where(x => x.NameKey == data.NameKey).FirstOrDefault();
                item.ValueString = data.ValueString;
                hasil=UpdateData(item);
            }
            else
            {
                hasil=InsertData(data);
            }
            return hasil;
        }

        public StateStorage GetDataById(object Id)
        {
            return db.StateStorages.Where(x => x.Id == (long)Id).FirstOrDefault();
        }


        public bool InsertData(StateStorage data)
        {
            try
            {
                db.StateStorages.Add(data);
                db.SaveChanges();
                return true;
            }
            catch
            {

            }
            return false;
        }


        public bool UpdateData(StateStorage data)
        {
            try
            {
                db.Entry(data).State = EntityState.Modified;
                db.SaveChanges();

                /*
                if (sel != null)
                {
                    sel.Nama = data.Nama;
                    sel.Keterangan = data.Keterangan;
                    sel.Tanggal = data.Tanggal;
                    sel.DocumentUrl = data.DocumentUrl;
                    sel.StreamUrl = data.StreamUrl;
                    return true;

                }*/
                return true;
            }
            catch
            {

            }
            return false;
        }

        public long GetLastId()
        {
            return db.StateStorages.Max(x => x.Id);
        }

    
        
    }
}
