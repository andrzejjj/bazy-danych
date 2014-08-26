using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConsoleApplication1.Model.Domain;
using NHibernate;
using ConsoleApplication1.NHIbernateHelper;
using ConsoleApplication1.Data_Access;
using System.Reflection;
using System.IO;
using System.Data;

namespace ConsoleApplication1.DataAccess
{
    class ImprovedDataAccess : AbstractDataAccess, IDataAccess
    {
        private static readonly String POSTS_TABLE_INMEMORY;

        private static readonly String THREADS_TABLE_INMEMORY;

        private static readonly String USERS_TABLE_INMEMORY;

        private static readonly String FULL_TEXT_SEARCH_INDEX_CREATE;

        private static readonly String FULL_TEXT_SEARCH_INDEX_POPULATION_STATUS;

        private static readonly String DROP_FOREING_KEYS;

        private static readonly String DROP_OLD_TABLES;

        private static readonly String POSTS_CONTAINS_FRODO;

        private static readonly String WORD35TH_MOST_COMMON_QUERY;

        private NHibernateHelper nHibernateHelper;

        static ImprovedDataAccess()
        {
            StreamReader SQLReader = null; 
            Assembly assembly = Assembly.GetExecutingAssembly();
            
            SQLReader = new StreamReader(assembly.GetManifestResourceStream("ConsoleApplication1.SQLCommands.PostsTableInMemory.txt"));
            POSTS_TABLE_INMEMORY = SQLReader.ReadToEnd();

            SQLReader = new StreamReader(assembly.GetManifestResourceStream("ConsoleApplication1.SQLCommands.ThreadsTableInMemory.txt"));
            THREADS_TABLE_INMEMORY = SQLReader.ReadToEnd();

            SQLReader = new StreamReader(assembly.GetManifestResourceStream("ConsoleApplication1.SQLCommands.UsersTableInMemory.txt"));
            USERS_TABLE_INMEMORY = SQLReader.ReadToEnd();

            SQLReader = new StreamReader(assembly.GetManifestResourceStream("ConsoleApplication1.SQLCommands.FullTextSearchIndexCreate.txt"));
            FULL_TEXT_SEARCH_INDEX_CREATE = SQLReader.ReadToEnd();

            SQLReader = new StreamReader(assembly.GetManifestResourceStream("ConsoleApplication1.SQLCommands.FullTextSearchIndexPopulationStatus.txt"));
            FULL_TEXT_SEARCH_INDEX_POPULATION_STATUS = SQLReader.ReadToEnd();

            SQLReader = new StreamReader(assembly.GetManifestResourceStream("ConsoleApplication1.SQLCommands.DropForeingKeys.txt"));
            DROP_FOREING_KEYS = SQLReader.ReadToEnd();

            SQLReader = new StreamReader(assembly.GetManifestResourceStream("ConsoleApplication1.SQLCommands.DropOldTables.txt"));
            DROP_OLD_TABLES = SQLReader.ReadToEnd();

            SQLReader = new StreamReader(assembly.GetManifestResourceStream("ConsoleApplication1.SQLCommands.PostsContainsFrodo.txt"));
            POSTS_CONTAINS_FRODO = SQLReader.ReadToEnd();

            SQLReader = new StreamReader(assembly.GetManifestResourceStream("ConsoleApplication1.SQLCommands.Word35thMostCommon.txt"));
            WORD35TH_MOST_COMMON_QUERY = SQLReader.ReadToEnd();

        }

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

        //public override void PrepareDatabase()
        //{
        //    using (ISession session = NHibernateHelper.OpenSession())
        //    {
        //        session.CreateSQLQuery(DROP_FOREING_KEYS)
        //                .ExecuteUpdate();
        //        session.CreateSQLQuery(DROP_OLD_TABLES)
        //                .ExecuteUpdate();
        //        session.CreateSQLQuery(POSTS_TABLE_INMEMORY)
        //                .ExecuteUpdate();
        //        session.CreateSQLQuery(THREADS_TABLE_INMEMORY)
        //                .ExecuteUpdate();
        //        session.CreateSQLQuery(USERS_TABLE_INMEMORY)
        //                .ExecuteUpdate();
        //    }
        //}

        public override void PrepareFullTextSearchIndex()
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                session.CreateSQLQuery(FULL_TEXT_SEARCH_INDEX_CREATE)
                        .ExecuteUpdate();

                IQuery checkPopulationStatus = session.CreateSQLQuery(FULL_TEXT_SEARCH_INDEX_POPULATION_STATUS);

                //wait for end of population
                while (checkPopulationStatus.UniqueResult<int>() != 0)
                {
                    System.Threading.Thread.Sleep(500);
                }
            }
        }

        public override void PersisUsers(IList<User> users)
        {
            using (IStatelessSession session = NHibernateHelper.OpenStatelessSession())
            using (ITransaction transaction = session.BeginTransaction())
            {

                foreach (User user in users)
                {
                    session.Insert(user);
                }

                foreach (User user in users)
                {
                    foreach (Thread thread in user.Threads)
                        session.Insert(thread);
                }

                foreach (User user in users)
                {
                    foreach (Thread thread in user.Threads)
                        foreach (Post post in thread.Posts)
                            session.Insert(post);
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

        public override int PostConatinsFrodoCount()
        {
            int count = 0;

            using (ISession session = NHibernateHelper.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                count = session.CreateSQLQuery(POSTS_CONTAINS_FRODO)
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

        public override string MostFrequentWord35th()
        {
            String word35th = null;
            using (ISession session = NHibernateHelper.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                word35th = session.CreateSQLQuery(WORD35TH_MOST_COMMON_QUERY)
                            .UniqueResult<string>();
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

            return word35th;
        }
    }
}
