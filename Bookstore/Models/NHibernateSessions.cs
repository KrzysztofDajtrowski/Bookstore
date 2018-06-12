using Bookstore.Models.Domain;
using Bookstore.Models.Identity;
using Microsoft.AspNet.Identity;
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

            var usersConfigFile = HttpContext.Current.Server.MapPath(@"~\Models\Mappings\Users.hbm.xml");
            configuration.AddFile(usersConfigFile);

            var ordersConfigFile = HttpContext.Current.Server.MapPath(@"~\Models\Mappings\Orders.hbm.xml");
            configuration.AddFile(ordersConfigFile);

            var ordersBookConfigFile = HttpContext.Current.Server.MapPath(@"~\Models\Mappings\OrdersBook.hbm.xml");
            configuration.AddFile(ordersBookConfigFile);

            ISessionFactory sessionFactory = configuration.BuildSessionFactory();
            return sessionFactory.OpenSession();
        }

        public IUserStore<User, int> Users
        {
            get { return new IdentityStore(OpenSession()); }
        }

    }
}