using DemoProject.Model;
using System;

namespace DemoProject.Data
{
    public class BaseProvider : IDisposable
    {
        public DemoProjectEntities _db;
        public BaseProvider()
        {
            _db = new DemoProjectEntities();
        }
        public void Dispose()
        {
            _db.Dispose();
        }
    }
}