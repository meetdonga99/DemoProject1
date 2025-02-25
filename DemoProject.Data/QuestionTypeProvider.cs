using DemoProject.Model;
using DemoProject.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoProject.Data
{
    public class QuestionTypeProvider : BaseProvider
    {
        public QuestionTypeProvider()
        {

        }
        public List<QuestionType> GetAllQuestionTypes()
        {
            var data = (from a in _db.QuestionType select a).OrderByDescending(a => a.Id).ToList();
            return data;
        }
        public IQueryable<QuestionTypeGridModel> GetAllQuestionTypesGrid()
        {
            return (from questionType in _db.QuestionType
                    select new QuestionTypeGridModel()
                    {
                        Id = questionType.Id,
                        TypeName = questionType.TypeName,
                        TypeCode = questionType.TypeCode,
                        IsActive = questionType.IsActive
                    }).AsQueryable();
        }
        public int CreateQuestionType(QuestionType questionType)
        {
            try
            {
                _db.QuestionType.Add(questionType);
                _db.SaveChanges();
                return questionType.Id;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public int UpdateQuestionType(QuestionType questionType)
        {
            try
            {
                _db.Entry(questionType).State = System.Data.Entity.EntityState.Modified;
                _db.SaveChanges();
                return questionType.Id;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public QuestionType GetQuestionTypeById(int id)
        {
            return _db.QuestionType.Find(id);
        }

        public QuestionType GetQuestionTypeByName(string questionTypeName)
        {
            return _db.QuestionType.Where(x => x.TypeName == questionTypeName).FirstOrDefault();
        }
        public List<QuestionType> CheckDuplicateQuestionTypeCode(string QuestionTypeCode)
        {
            var getQuestionTypeDetails = (from questionType in _db.QuestionType
                                          where questionType.TypeCode.ToUpper().Trim() == QuestionTypeCode.ToUpper().Trim()
                                     select questionType).ToList();
            return getQuestionTypeDetails;
        }
    }
}
