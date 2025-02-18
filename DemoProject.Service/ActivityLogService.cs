using DemoProject.Data;
using DemoProject.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoProject.Service
{
    public class ActivityLogService
    {
        private readonly ActivityLogProvider _activityLogProvider;
        public ActivityLogService()
        {
            _activityLogProvider = new ActivityLogProvider();
        }
        public int CreateActivityLog(ActivityLog activitylog)
        {
            return _activityLogProvider.CreateActivityLog(activitylog);
        }
        public IQueryable<ActivityLogGridModel> GetAllActivityLogs()
        {
            return _activityLogProvider.GetAllActivityLogs();
        }
        public ActivityLog GetActivityLogById(int id)
        {
            return _activityLogProvider.GetActivityLogById(id);
        }
    }
}
