using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using ConsoleApplication1.Model.Domain;
using ConsoleApplication1.NHIbernateHelper;
using ConsoleApplication1.Data_Access;
using System.Data;

namespace ConsoleApplication1.DataAccess
{
    public class SimpleDataAccess : AbstractDataAccess, IDataAccess
    {
        private NHibernateHelper nHibernateHelper;

        public override NHibernateHelper NHibernateHelper
        {
            get
            {
                return nHibernateHelper;
            }
            set
            {
                nHibernateHelper = value;
                base.NHibernateHelper = value;
            }
        }

        public override void PersisUsers(IList<User> users)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                int i = 0;
                foreach (User user in users)
                {
                    session.Save(user);
                    //i++;
                    //if (i % 100 == 0)
                    //{
                    //    session.Flush();
                    //    session.Clear();
                    //}
                }

                try
                {
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    if (transaction != null)
                        transaction.Rollback();
                    throw e;
                }
            }
        }
    }
} 
