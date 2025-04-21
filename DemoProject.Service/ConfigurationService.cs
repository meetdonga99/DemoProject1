using DemoProject.Data;
using DemoProject.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoProject.Service
{
    public class ConfigurationService
    {
        public readonly ConfigurationProvider _configurationProvider;
        public ConfigurationService()
        {
            _configurationProvider = new ConfigurationProvider();
        }

        public int CreateConfiguration(Configuration configuration)
        {
            return _configurationProvider.CreateConfiguration(configuration);
        }

        public int UpdateConfiguration(Configuration configuration)
        {
            return _configurationProvider.UpdateConfiguration(configuration);
        }

        public string GetConfigurationValueByKey(string key)
        {
            return _configurationProvider.GetConfigurationValueByKey(key);
        }

        public IQueryable<ConfigurationGridModel> GetAllConfigurations()
        {
            return _configurationProvider.GetAllConfigurations();
        }

        public Configuration GetConfigurationById(int id)
        {
            return _configurationProvider.GetConfigurationById(id);
        }
    }
}
