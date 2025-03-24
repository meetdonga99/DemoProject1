using DemoProject.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoProject.Data
{
    public class UserExamAnswerProvider : BaseProvider
    {
        public UserExamAnswerProvider()
        {

        }

        public int CreateUserExamAnswer(UserExamAnswer userExamAnswer)
        {
            try
            {
                _db.UserExamAnswer.Add(userExamAnswer);
                _db.SaveChanges();
                return userExamAnswer.Id;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public int UpdateUserExamAnswer(UserExamAnswer userExamAnswer)
        {
            try
            {
                _db.Entry(userExamAnswer).State = System.Data.Entity.EntityState.Modified;
                _db.SaveChanges();
                return userExamAnswer.Id;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public UserExamAnswer GetAnswerByExamIdAndQuestionId(int userExamId, int questionId)
        {
            var ans = (from a in _db.UserExamAnswer where a.UserExamRecordId == userExamId && a.QuestionId == questionId select a).FirstOrDefault();
            return ans;
        }

        public List<UserExamAnswer> GetAnswersByExamId(int userExamId)
        {
            var data = (from a in _db.UserExamAnswer where a.UserExamRecordId == userExamId select a).ToList();
            return data;
        }
    }
}
