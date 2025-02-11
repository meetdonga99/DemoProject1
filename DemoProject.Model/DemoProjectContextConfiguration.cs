using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Infrastructure.DependencyResolution;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoProject.Model
{
    public class DemoProjectContextConfiguration : DbConfiguration
    {
        private static bool useCachedDbModelStore;

        public static void Configure(bool useCachedDbModelStore)
        {
            DemoProjectContextConfiguration.useCachedDbModelStore = useCachedDbModelStore;
        }

        public DemoProjectContextConfiguration()
        {
            //string cachePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\TTS\EFCache\";
            string cachePath = AppDomain.CurrentDomain.BaseDirectory + @"EFCache\";
            if (!Directory.Exists(cachePath))
            {
                Directory.CreateDirectory(cachePath);
            }
            MyDbModelStore cachedDbModelStore = new MyDbModelStore(cachePath);
            IDbDependencyResolver dependencyResolver = new SingletonDependencyResolver<DbModelStore>(cachedDbModelStore);
            AddDependencyResolver(dependencyResolver);
        }

        private class MyDbModelStore : DefaultDbModelStore
        {
            public MyDbModelStore(string location)
                : base(location)
            { }

            public override DbCompiledModel TryLoad(Type contextType)
            {
                string path = GetFilePath(contextType);

                if (File.Exists(path))
                {
                    DateTime lastWriteTime = File.GetLastWriteTimeUtc(path);
                    DateTime lastWriteTimeDomainAssembly = File.GetLastWriteTimeUtc(typeof(DemoProject.Model.DemoProjectEntities).Assembly.Location);
                    if (lastWriteTimeDomainAssembly > lastWriteTime)
                    {
                        File.Delete(path);
                    }
                }
                else
                {

                }

                return base.TryLoad(contextType);
            }
        }
    }

}
