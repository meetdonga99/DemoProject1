using DemoProject.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoProject.Data
{
    public class UserExamRecordProvider : BaseProvider
    {
        public UserExamRecordProvider()
        {

        }
        public int CreateUserExamRecord(UserExamRecord userExamRecord)
        {
            try
            {
                _db.UserExamRecord.Add(userExamRecord);
                _db.SaveChanges();
                return userExamRecord.Id;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public int UpdateUserExamRecord(UserExamRecord userExamRecord)
        {
            try
            {
                _db.Entry(userExamRecord).State = System.Data.Entity.EntityState.Modified;
                _db.SaveChanges();
                return userExamRecord.Id;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public IQueryable<UserExamRecordGridModel> GetAllUserExamRecordGrid()
        {
            return (from userExamRecord in _db.UserExamRecord
                        //where paperSet.IsDeleted == false
                    select new UserExamRecordGridModel()
                    {
                        Id = userExamRecord.Id,
                        PaperSetName = userExamRecord.PaperSet.PaperSetName,
                        UserEmail = userExamRecord.User.Email,
                        ExamStatus = userExamRecord.ExamStatus,
                        ExpiryDate = userExamRecord.ExpiryDate
                    }).AsQueryable();
        }
    }
}

