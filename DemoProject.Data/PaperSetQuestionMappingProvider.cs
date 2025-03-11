using DemoProject.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoProject.Data
{
    public class PaperSetQuestionMappingProvider : BaseProvider
    {
        public PaperSetQuestionMappingProvider()
        {

        }

        public int AddQuestionInPaper(PaperSetQuestionMapping rec)
        {
            _db.PaperSetQuestionMapping.Add(rec);
            _db.SaveChanges();
            return rec.Id;
        }

        public int UpdateMapping(PaperSetQuestionMapping mapping)
        {
            try
            {
                _db.Entry(mapping).State = System.Data.Entity.EntityState.Modified;
                _db.SaveChanges();
                return mapping.Id;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public bool RemoveQuestionFromPaper(int recId)
        {
            var rec = _db.PaperSetQuestionMapping.Find(recId);
            if (rec == null)
            {
                return false;
            }
            _db.PaperSetQuestionMapping.Remove(rec);
            _db.SaveChanges();

            return true;
        }

        public bool RemoveQuestionsFromPaper(List<PaperSetQuestionMapping> mappings)
        {
            //var rec = _db.PaperSetQuestionMapping.Find(recId);
            //if (rec == null)
            //{
            //    return false;
            //}
            _db.PaperSetQuestionMapping.RemoveRange(mappings);
            _db.SaveChanges();

            return true;
        }

        public List<PaperSetQuestionMapping> GetMappingsByPaperSetId(int paperSetId)
        {
            var data = (from a in _db.PaperSetQuestionMapping where a.PaperSetId == paperSetId select a).ToList();
            return data;
        }

    }
}
