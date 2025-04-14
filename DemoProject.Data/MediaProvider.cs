using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DemoProject.Model;


namespace DemoProject.Data
{
    public class MediaProvider : BaseProvider
    {
        public MediaProvider()
        {

        }

        public bool CreateMultiMedia(List<Media> records)
        {
            _db.Media.AddRange(records);
            _db.SaveChanges();

            return true;
        }

        public bool UpdateMultiMedia(IEnumerable<Media> records)
        {
            try
            {
                foreach (var record in records)
                {
                    _db.Entry(record).State = System.Data.Entity.EntityState.Modified;
                }

                _db.SaveChanges();

                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public bool RemoveMultiMedia(List<Media> records)
        {
            _db.Media.RemoveRange(records);
            _db.SaveChanges();

            return true;
        }

        public List<Media> GetMediaByQuestionId(int questionId)
        {
            return _db.Media.Where(m => m.QuestionId == questionId).ToList();
        }

        public Media GetMediaById(int id)
        {
            return _db.Media.Where(m => m.Id == id).FirstOrDefault();
        }
    }
}
