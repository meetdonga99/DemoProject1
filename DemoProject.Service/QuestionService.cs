using DemoProject.Data;
using DemoProject.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoProject.Service
{
    public class QuestionService
    {
        public readonly QuestionProvider _questionProvider;
        public QuestionService()
        {
            _questionProvider = new QuestionProvider();
        }

        public List<Question> GetAllQuestions()
        {
            return _questionProvider.GetAllQuestions();
        }
        public IQueryable<QuestionGridModel> GetAllQuestionsGrid()
        {
            return _questionProvider.GetAllQuestionsGrid();
        }

        public int CreateQuestion(Question question)
        {
            return _questionProvider.CreateQuestion(question);
        }

        public int UpdateQuestion(Question question)
        {
            return _questionProvider.UpdateQuestion(question);
        }

        public Question GetQuestionById(int id)
        {
            return _questionProvider.GetQuestionById(id);
        }

        public bool DeleteQuestion(int questionId)
        {
            return _questionProvider.DeleteQuestion(questionId);
        }
    }
}
