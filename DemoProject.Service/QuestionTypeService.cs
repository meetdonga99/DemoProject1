using DemoProject.Data;
using DemoProject.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoProject.Service
{
    public class QuestionTypeService
    {
        public readonly QuestionTypeProvider _questionTypeProvider;
        public QuestionTypeService()
        {
            _questionTypeProvider = new QuestionTypeProvider();
        }

        public List<QuestionType> GetAllQuestionTypes()
        {
            return _questionTypeProvider.GetAllQuestionTypes();
        }
        public IQueryable<QuestionTypeGridModel> GetAllQuestionTypesGrid()
        {
            return _questionTypeProvider.GetAllQuestionTypesGrid();
        }

        public int CreateQuestionType(QuestionType questionType)
        {
            return _questionTypeProvider.CreateQuestionType(questionType);
        }

        public int UpdateQuestionType(QuestionType questionType)
        {
            return _questionTypeProvider.UpdateQuestionType(questionType);
        }

        public QuestionType GetQuestionTypeById(int id)
        {
            return _questionTypeProvider.GetQuestionTypeById(id);
        }

        public QuestionType GetQuestionTypeByName(string questionTypeName)
        {
            return _questionTypeProvider.GetQuestionTypeByName(questionTypeName);
        }
        public List<QuestionType> CheckDuplicateQuestionTypeCode(string QuestionTypeCode)
        {
            return _questionTypeProvider.CheckDuplicateQuestionTypeCode(QuestionTypeCode);
        }
    }
}
