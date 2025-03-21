using DemoProject.Data;
using DemoProject.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoProject.Service
{
    public class UserExamRecordService
    {
        public readonly UserExamRecordProvider _userExamRecordProvider;
        public UserExamRecordService()
        {
            _userExamRecordProvider = new UserExamRecordProvider();
        }

        public int CreateUserExamRecord(UserExamRecord userExamRecord)
        {
            return _userExamRecordProvider.CreateUserExamRecord(userExamRecord);
        }

        public int UpdateUserExamRecord(UserExamRecord userExamRecord)
        {
            return _userExamRecordProvider.UpdateUserExamRecord(userExamRecord);
        }

        public IQueryable<UserExamRecordGridModel> GetAllUserExamRecordGrid()
        {
            return _userExamRecordProvider.GetAllUserExamRecordGrid();
        }

        public UserExamRecord GetRecordByPaperSetIdAndUserId(int paperSetId, int userId)
        {
            return _userExamRecordProvider.GetRecordByPaperSetIdAndUserId(paperSetId, userId);
        }
    }
}
