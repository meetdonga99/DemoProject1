using DemoProject.Model;
using DemoProject.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace DemoProject.Data
{
    public class QuestionProvider : BaseProvider
    {
        public QuestionProvider()
        {

        }
        public List<Question> GetAllQuestions()
        {
            var data = (from a in _db.Question where a.IsDeleted == false select a).OrderByDescending(a => a.Id).ToList();
            return data;
        }
        public IQueryable<QuestionGridModel> GetAllQuestionsGrid()
        {
            return (from question in _db.Question
                        //join subject in _db.Subject on question.SubjectId equals subject.Id
                        //join questionType in _db.QuestionType on question.QuestionTypeId equals questionType.Id
                    where question.IsDeleted == false
                    select new QuestionGridModel()
                    {
                        Id = question.Id,
                        SubjectName = question.Subjects.Name,
                        QuestionType = question.QuestionTypes.TypeName,
                        QuestionText = question.QuestionText,
                        DefaultMarks = question.DefaultMarks,
                        DifficultyLevel = question.DifficultyLevel,
                        IsActive = question.IsActive,
                        BadgeCode = (from c in _db.CommonLookup where c.Name == question.DifficultyLevel select c.BadgeCode).FirstOrDefault(),
                    }).AsQueryable();
        }
        public int CreateQuestion(Question question)
        {
            try
            {
                _db.Question.Add(question);
                _db.SaveChanges();
                return question.Id;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public int UpdateQuestion(Question question)
        {
            try
            {
                _db.Entry(question).State = System.Data.Entity.EntityState.Modified;
                _db.SaveChanges(); 
                return question.Id;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Question GetQuestionById(int id)
        {
            return _db.Question.Find(id);
        }

        public bool DeleteQuestion(int questionId)
        {
            try
            {
                var question = _db.Question.Find(questionId);
                if (question == null)
                {
                    return false;
                }

                var questionOptions = (from a in _db.Option where a.QuestionId == questionId select a).ToList();
                _db.Option.RemoveRange(questionOptions);

                var mediaToBeDelete = (from a in _db.Media where a.QuestionId == questionId select a).ToList();
                _db.Media.RemoveRange(mediaToBeDelete);

                _db.Question.Remove(question);
                _db.SaveChanges();

                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<int> BulkCreateQuestions(List<Question> questions)
        {
            List<int> questionIds = new List<int>();

            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    _db.Question.AddRange(questions);
                    _db.SaveChanges();

                    
                    foreach (var question in questions)
                    {
                        questionIds.Add(question.Id);
                    }

                    transaction.Commit();
                    return questionIds;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception("Failed to bulk create questions: " + ex.Message, ex);
                }
            }
        }

        public List<Question> GetQuestionsByIds(List<int> questionIds)
        {
            return _db.Question
                .Include(q => q.Subjects)
                .Include(q => q.QuestionTypes)
                .Where(q => questionIds.Contains(q.Id))
                .ToList();
        }

    }
}
