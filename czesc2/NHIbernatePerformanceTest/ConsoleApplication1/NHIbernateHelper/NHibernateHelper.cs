using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Cfg;
using ConsoleApplication1.Model.Domain;
using System.Data;

namespace ConsoleApplication1.NHIbernateHelper
{
    public abstract class NHibernateHelper
    {
        private static readonly String CONNECTION_STRING = @"Data Source=KOMP\SQL2014;Initial Catalog=TolkienForum;Integrated Security=True";
        private ISessionFactory sessionFactory = null;

        public ISessionFactory SessionFactory
        {
            get { return sessionFactory; }
        }

        protected NHibernateHelper()
        {
        }

        public ISession OpenSession()
        {
            return sessionFactory.OpenSession();
        }

        public IStatelessSession OpenStatelessSession()
        {
            return sessionFactory.OpenStatelessSession();
        }

        public void Close()
        {
            sessionFactory.Close();
            sessionFactory = null;
        }

        protected abstract void AddMappings(Configuration configuration);

        public void Init()
        {
            if (sessionFactory == null)
            {
                Configuration configuration = new Configuration();
                configuration
                    .SetProperty(NHibernate.Cfg.Environment.Dialect, typeof(NHibernate.Dialect.MsSql2012Dialect).AssemblyQualifiedName)
                    .SetProperty(NHibernate.Cfg.Environment.ConnectionProvider, typeof(NHibernate.Connection.DriverConnectionProvider).AssemblyQualifiedName)
                    .SetProperty(NHibernate.Cfg.Environment.ConnectionString, CONNECTION_STRING)
                    .SetProperty(NHibernate.Cfg.Environment.Hbm2ddlAuto, "create-drop")
                    .SetProperty(NHibernate.Cfg.Environment.BatchSize, "200")
                    //.SetProperty(NHibernate.Cfg.Environment.Isolation, IsolationLevel.Snapshot.ToString())
                    .SetProperty(NHibernate.Cfg.Environment.ShowSql, "false")
                    //.SetProperty(NHibernate.Cfg.Environment.FormatSql, "false")
                    ;
                AddMappings(configuration);

                sessionFactory = configuration.BuildSessionFactory();
            }
        }
    }
}
