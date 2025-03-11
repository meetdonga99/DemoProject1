using DemoProject.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoProject.Data
{
    public class PaperSetProvider : BaseProvider
    {
        public PaperSetProvider()
        {

        }

        public List<PaperSet> GetAllPaperSets()
        {
            var data = (from a in _db.PaperSet where a.IsDeleted == false select a).OrderByDescending(a => a.Id).ToList();
            return data;
        }

        public IQueryable<PaperSetGridModel> GetAllPaperSetsGrid()
        {
            return (from paperSet in _db.PaperSet
                    where paperSet.IsDeleted == false
                    select new PaperSetGridModel()
                    {
                        Id = paperSet.Id,
                        PaperSetName = paperSet.PaperSetName,
                        TotalMarks = paperSet.TotalMarks,
                        DurationInMinutes = paperSet.DurationInMinutes,
                        Status = paperSet.Status,
                        IsActive = paperSet.IsActive,
                        BadgeCode = (from c in _db.CommonLookup where c.Name == paperSet.Status select c.BadgeCode).FirstOrDefault(),
                    }).AsQueryable();
        }

        public int CreatePaperSet(PaperSet paperSet)
        {
            try
            {
                _db.PaperSet.Add(paperSet);
                _db.SaveChanges();
                return paperSet.Id;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public int UpdatePaperSet(PaperSet paperSet)
        {
            try
            {
                _db.Entry(paperSet).State = System.Data.Entity.EntityState.Modified;
                _db.SaveChanges();
                return paperSet.Id;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public PaperSet GetPaperSetById(int id)
        {
            return _db.PaperSet.Find(id);
        }

        public bool DeletePaperSet(int paperSetId)
        {
            try
            {
                var paperSet = _db.PaperSet.Find(paperSetId);
                if (paperSet == null)
                {
                    return false;
                }

                _db.PaperSet.Remove(paperSet);
                _db.SaveChanges();

                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        public void SaveChanges()
        {
            _db.SaveChanges();
        }
    }
}
