using DemoProject.Helper;
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

        public IQueryable<UserExamRecord> GetAllRecords()
        {
            return from i in _db.UserExamRecord select i;
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
                    join commonLookUp in _db.CommonLookup on userExamRecord.ExamStatus equals commonLookUp.Code
                    where commonLookUp.Type == LookupType.ExamStatus
                        //where paperSet.IsDeleted == false
                    select new UserExamRecordGridModel()
                    {
                        Id = userExamRecord.Id,
                        Token = userExamRecord.Token,
                        PaperSetName = userExamRecord.PaperSet.PaperSetName,
                        StartTime = userExamRecord.StartTime,
                        EndTime = userExamRecord.EndTime,
                        UserEmail = userExamRecord.User.Email,
                        ExamStatus = userExamRecord.ExamStatus,
                        ExpiryDate = userExamRecord.ExpiryDate.Value,
                        Score = userExamRecord.Score,
                        BadgeCode = commonLookUp.BadgeCode
                    }).AsQueryable();
        }

        public UserExamRecord GetRecordByPaperSetIdAndUserId(int paperSetId, int userId)
        {
            var record = (from a in _db.UserExamRecord where a.PaperSetId == paperSetId && a.UserId == userId select a).FirstOrDefault();
            return record;
        }

        public UserExamRecord GetRecordByToken(string token)
        {
            var record = (from a in _db.UserExamRecord where a.Token == token select a).FirstOrDefault();
            return record;
        }

        public UserExamRecord GetRecordByUserExamRecordId(int userExamRecordId)
        {
            var record = (from a in _db.UserExamRecord where a.Id == userExamRecordId select a).FirstOrDefault();
            return record;
        }

        public List<UserExamRecord> GetAllInprogressRecords()
        {
            return (from a in _db.UserExamRecord where a.ExamStatus == "INPROGRESS" select a).ToList();
        }
    }
}

