using Microsoft.EntityFrameworkCore;
using MoslemToolkit.Data;
using MoslemToolkit.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoslemToolkit.Data
{
    public class ProgramRamadanService : ICrud<ProgramRamadan>
    {
        NgajiDB db;

        public ProgramRamadanService()
        {
            if (db == null) db = new NgajiDB();

        }
        public bool DeleteData(object Id)
        {
            var selData = (db.ProgramRamadans.Include(t=>t.TargetPrograms).Where(x => x.Id == (long)Id).FirstOrDefault());
            foreach(var item in selData.TargetPrograms)
            {
                db.TargetPrograms.Remove(item);
            }
            db.ProgramRamadans.Remove(selData);
            db.SaveChanges();
            return true;
        }

        public List<ProgramRamadan> FindByKeyword(string Keyword)
        {
            var data = from x in db.ProgramRamadans.Include(x => x.TargetPrograms)
                       where x.Nama.Contains(Keyword) 
                       select x;
            return data.ToList();
        }

        public List<ProgramRamadan> GetAllData()
        {
            return db.ProgramRamadans.Include(x=>x.TargetPrograms).ToList();
        }

        public ProgramRamadan GetDataById(object Id)
        {
            return db.ProgramRamadans.Include(x => x.TargetPrograms).Where(x => x.Id == (long)Id).FirstOrDefault();
        }
        public bool IsUserExist(string Username)
        {
            return db.ProgramRamadans.Any(x => x.Nama.ToLower() == Username.ToLower());
        }


        public bool InsertData(ProgramRamadan data)
        {
            try
            {
                db.ProgramRamadans.Add(data);
                db.SaveChanges();
                return true;
            }
            catch
            {

            }
            return false;
        }



        public bool UpdateData(ProgramRamadan data)
        {
            try
            {
                db.Entry(data).State = EntityState.Modified;
                foreach (var item in data.TargetPrograms)
                {
                    db.Entry(item).State = EntityState.Modified;
                }
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
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }

        public long GetLastId()
        {
            return db.ProgramRamadans.Max(x => x.Id);
        }
    }
}
