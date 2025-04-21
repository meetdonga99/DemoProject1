using DemoProject.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoProject.Data
{
    public class ConfigurationProvider : BaseProvider
    {
        public ConfigurationProvider()
        {

        }

        public int CreateConfiguration(Configuration configuration)
        {
            try
            {
                _db.Configurations.Add(configuration);
                _db.SaveChanges();
                return configuration.Id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int UpdateConfiguration(Configuration configuration)
        {
            try
            {
                _db.Entry(configuration).State = System.Data.Entity.EntityState.Modified;
                _db.SaveChanges();
                return configuration.Id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string GetConfigurationValueByKey(string key)
        {
            var data = _db.Configurations
              .FirstOrDefault(i => i.ConfigurationKey.ToUpper() == key.ToUpper());
            return data.Value;
        }

        public IQueryable<ConfigurationGridModel> GetAllConfigurations()
        {
            return (from c in _db.Configurations
                    select new ConfigurationGridModel()
                    {
                        Id = c.Id,
                        ConfigurationKey = c.ConfigurationKey,
                        Value = c.Value,
                        Comment = c.Comment
                    }).AsQueryable();
        }

        public Configuration GetConfigurationById(int id)
        {
            return _db.Configurations.Find(id);
        }
    }
}
