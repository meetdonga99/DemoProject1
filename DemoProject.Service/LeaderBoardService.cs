using DemoProject.Data;
using DemoProject.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoProject.Service
{
    public class LeaderBoardService
    {
        public readonly LeaderBoardProvider _leaderBoardProvider;

        public LeaderBoardService()
        {
            _leaderBoardProvider = new LeaderBoardProvider();
        }

        public IQueryable<LeaderBoardGridModel> GetLeaderBoardGrid()
        {
            return _leaderBoardProvider.GetLeaderBoardGrid();
        }
    }
}
