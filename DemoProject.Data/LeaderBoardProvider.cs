using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DemoProject.Model;

namespace DemoProject.Data
{
    public class LeaderBoardProvider : BaseProvider
    {
        public LeaderBoardProvider()
        {

        }

        public IQueryable<LeaderBoardGridModel> GetLeaderBoardGrid()
        {
            var data = (from user in _db.UserProfile.AsNoTracking()
                        join examRecord in _db.UserExamRecord.AsNoTracking() on user.UserId equals examRecord.UserId
                        join paperSet in _db.PaperSet.AsNoTracking() on examRecord.PaperSetId equals paperSet.Id
                        where examRecord.ExamStatus == "RESULT_PUBLISHED"
                        orderby examRecord.Score descending
                        select new LeaderBoardGridModel
                        {
                            UserExamRecordId = examRecord.Id,
                            Email = user.Email,
                            PaperSetName = paperSet.PaperSetName,
                            Score = examRecord.Score,
                            Date = examRecord.StartTime ?? DateTime.MinValue
                        }).AsQueryable();

            return data;

        }
    }
}
