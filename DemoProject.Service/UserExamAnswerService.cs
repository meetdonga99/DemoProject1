using DemoProject.Data;
using DemoProject.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoProject.Service
{
    public class UserExamAnswerService
    {
        public readonly UserExamAnswerProvider _userExamAnswerProvider;
        public UserExamAnswerService()
        {
            _userExamAnswerProvider = new UserExamAnswerProvider();
        }

        public int CreateUserExamAnswer(UserExamAnswer userExamAnswer)
        {
            return _userExamAnswerProvider.CreateUserExamAnswer(userExamAnswer);
        }

        public int UpdateUserExamAnswer(UserExamAnswer userExamAnswer)
        {
            return _userExamAnswerProvider.UpdateUserExamAnswer(userExamAnswer);
        }

        public UserExamAnswer GetAnswerByExamIdAndQuestionId(int userExamId, int questionId)
        {
            return _userExamAnswerProvider.GetAnswerByExamIdAndQuestionId(userExamId, questionId);
        }

        public List<UserExamAnswer> GetAnswersByExamId(int userExamId)
        {
            return _userExamAnswerProvider.GetAnswersByExamId(userExamId);
        }
    }
}
