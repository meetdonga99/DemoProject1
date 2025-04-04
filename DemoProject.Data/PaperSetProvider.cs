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

        public bool IsUsedForUserExam(int paperSetId)
        {
            var record = _db.UserExamRecord.Where(o => o.PaperSetId == paperSetId).FirstOrDefault();
            return record != null ? true : false;
        }

        public bool ClonePaperSet(int paperSetId)
        {
            try
            {
                int userId = SessionHelper.UserId;
                var record = _db.PaperSet.Find(paperSetId);
                var total = _db.PaperSet.Where(o => o.PaperSetName.Contains(record.PaperSetName)).Select(o => o.Id).Count();
                var clonedPaperSet = new PaperSet
                {
                    PaperSetName = record.PaperSetName + "(Copy " + (total - 1).ToString() + ")",
                    TotalMarks = record.TotalMarks,
                    DurationInMinutes = record.DurationInMinutes,
                    IsActive = record.IsActive,
                    Status = record.Status,
                    CreatedOn = record.CreatedOn,
                    CreatedBy = userId,
                };
                _db.PaperSet.Add(clonedPaperSet);
                _db.SaveChanges();
                int newPaperSetId = clonedPaperSet.Id;

                var newMappings = _db.PaperSetQuestionMapping
    .Where(o => o.PaperSetId == paperSetId)
    .Select(o => new
    {
        o.QuestionId,
        o.CustomMarks
    })
    .ToList();
                var newMappingEntities = newMappings.Select(o => new PaperSetQuestionMapping
                {
                    PaperSetId = newPaperSetId,
                    QuestionId = o.QuestionId,
                    CustomMarks = o.CustomMarks
                }).ToList();

                _db.PaperSetQuestionMapping.AddRange(newMappingEntities);
                _db.SaveChanges();
                return true;
            }
            catch(Exception e)
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
