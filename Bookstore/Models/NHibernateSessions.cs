using NHibernate;
using NHibernate.Cfg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bookstore.Models
{
    public class NHibernateSessions
    {
        public static ISession OpenSession()
        {
            var configuration = new Configuration();
            var configurationPath = HttpContext.Current.Server.MapPath(@"~\Models\hibernate.cfg.xml");
            configuration.Configure(configurationPath);

            var bookConfigFile = HttpContext.Current.Server.MapPath(@"~\Models\Mappings\Book.hbm.xml");
            configuration.AddFile(bookConfigFile);

            ISessionFactory sessionFactory = configuration.BuildSessionFactory();
            return sessionFactory.OpenSession();
        }
    }
}