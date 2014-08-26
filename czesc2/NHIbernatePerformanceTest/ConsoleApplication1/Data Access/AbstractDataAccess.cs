using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConsoleApplication1.DataAccess;
using ConsoleApplication1.NHIbernateHelper;
using NHibernate;
using ConsoleApplication1.Model.Domain;
using NHibernate.Criterion;
using System.Text.RegularExpressions;

namespace ConsoleApplication1.Data_Access
{
    public abstract class AbstractDataAccess : IDataAccess
    {
        private NHibernateHelper nHibernateHelper;

        public virtual NHibernateHelper NHibernateHelper
        {
            get
            {
                return nHibernateHelper;
            }
            set
            {
                nHibernateHelper = value;
            }
        }

        public abstract void PersisUsers(IList<Model.Domain.User> users);

        public virtual void PrepareDatabase()
        {
            return;
        }

        public virtual void PrepareFullTextSearchIndex()
        {
            return;
        }

        public int CreatedThreads2013Count()
        {
            int count = 0;

            using (ISession session = NHibernateHelper.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                count = session.CreateCriteria<Thread>()
                            .SetProjection(Projections.RowCount())
                            .Add(Restrictions.Between("CreationDate", DateTime.Parse("2013-01-01"), DateTime.Parse("2013-12-31")))
                            .UniqueResult<int>();
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

            return count;
        }

        public String MostPopularThread2013()
        {
            String thread = null;

            using (ISession session = NHibernateHelper.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                object[] returned = (object[])session.CreateCriteria<Thread>("t")
                            .SetMaxResults(1)
                            .SetFetchMode("t.Posts", FetchMode.Join)
                            .CreateAlias("t.Posts", "p")
                            .Add(Restrictions.Between("p.CreationDate", DateTime.Parse("2013-01-01"), DateTime.Parse("2013-12-31")))
                            .SetProjection(Projections.ProjectionList()
                                                .Add(Projections.GroupProperty("t.Title"))
                                                .Add(Projections.RowCount(), "count"))
                            .AddOrder(Order.Desc("count"))
                            .List()[0];
                thread = (String)returned[0];
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

            return thread;
        }

        public Double AveragePostLength()
        {
            Double length = 0;

            using (ISession session = NHibernateHelper.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                length = session.CreateCriteria<Post>()
                            .SetProjection(Projections.Avg(Projections.SqlFunction("Length", NHibernateUtil.Int32, Projections.Property("Content"))))
                            .UniqueResult<Double>();
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

            return length;
        }

        public String MostActiveByThreadsUser()
        {
            String login = null;

            using (ISession session = NHibernateHelper.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                IQuery criteria = session.CreateQuery("select u.Login " +
                                                        "from User u, Thread t " +
                                                        "where exists(" +
                                                            "from Post p " +
                                                            "where p.User = u and p.Thread = t" +
                                                            ")" +
                                                        "group by u.Login " +
                                                        "order by count(u) desc")
                                            .SetMaxResults(1);

                login = criteria.UniqueResult<string>();
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

            return login;
        }

        public String MostActiveByCommentingOtherUsers()
        {
            String login = null;

            using (ISession session = NHibernateHelper.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                IQuery criteria = session.CreateQuery("select u.Login " +
                                                        "from User u, Thread t " +
                                                        "where exists(" +
                                                            "from Post p " +
                                                            "where p.User = u and p.Thread = t and u != t.User" +
                                                            ")" +
                                                        "group by u.Login " +
                                                        "order by count(u) desc")
                                            .SetMaxResults(1);

                login = criteria.UniqueResult<string>();
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

            return login;
        }

        public virtual int PostConatinsFrodoCount()
        {
            int count = 0;

            using (ISession session = NHibernateHelper.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                count = session.CreateCriteria<Post>()
                            .SetProjection(Projections.RowCount())
                            .Add(Restrictions.Like("Content", "%Frodo%"))
                            .UniqueResult<int>();
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

            return count;
        }

        public int PostByCityLikeKCount()
        {
            int count = 0;

            using (ISession session = NHibernateHelper.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                count = session.CreateCriteria<User>("u")
                            .CreateAlias("u.Posts", "p")
                            .SetProjection(Projections.RowCount())
                            .Add(Restrictions.Like("u.City", "K%"))
                            .UniqueResult<int>();
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

            return count;
        }

        public virtual string MostFrequentWord35th()
        {
            String word35th = null;
            IList<Post> posts = null;
            using (ISession session = NHibernateHelper.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                posts = session.CreateCriteria<Post>().List<Post>();
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

            word35th = posts.SelectMany(p => Regex.Split(p.Content.ToLower(), @"\W+"))
                    .Where(s => s.Length > 0)
                    .GroupBy(s => s)
                    .OrderByDescending(g => g.Count())
                    .Select(x => x.Key)
                    .Skip(34)
                    .First();

            return word35th;
        }
    }
}
