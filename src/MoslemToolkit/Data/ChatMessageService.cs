using Microsoft.EntityFrameworkCore;
using MoslemToolkit.Data;
using MoslemToolkit.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoslemToolkit.Data
{
    public class ChatMessageService : ICrud<ChatMessage>
    {
        NgajiDB db;

        public ChatMessageService()
        {
            if (db == null) db = new NgajiDB();

        }
        public void ClearChat(long ThreadId)
        {
            var datas = db.ChatMessages.Where(x => x.ThreadId == ThreadId).ToList();
            foreach (var item in datas)
            {
                DeleteData(item.Id);
            }
        }
        public List<ChatMessage> GetByThreadId(long ThreadId)
        {
            return (from x in db.ChatMessages
                    where x.ThreadId == ThreadId
                    orderby x.Tanggal descending
                    select x).ToList();
        }

        public bool DeleteData(object Id)
        {
            var selData = (db.ChatMessages.Where(x => x.Id == (long)Id).FirstOrDefault());
            db.ChatMessages.Remove(selData);
            db.SaveChanges();
            return true;
        }

        public List<ChatMessage> FindByKeyword(string Keyword)
        {
            var data = from x in db.ChatMessages
                       where x.Message.Contains(Keyword)
                       select x;
            return data.ToList();
        }

        public List<ChatMessage> GetAllData()
        {
            return db.ChatMessages.ToList();
        }

        public ChatMessage GetDataById(object Id)
        {
            return db.ChatMessages.Where(x => x.Id == (long)Id).FirstOrDefault();
        }


        public bool InsertData(ChatMessage data)
        {
            try
            {
                db.ChatMessages.Add(data);
                db.SaveChanges();
                return true;
            }
            catch
            {

            }
            return false;
        }

       

        public bool UpdateData(ChatMessage data)
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
            return db.ChatMessages.Max(x => x.Id);
        }
    }
}
