using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using ConsoleApplication1.Model.Domain;
using ConsoleApplication1.NHIbernateHelper;
using ConsoleApplication1.DataAccess;
using ConsoleApplication1.Data_Access;
using NHibernate.Engine;
using System.Data;

namespace ConsoleApplication1
{
    public class Program
    {
        public static readonly String xmlPath = AppDomain.CurrentDomain.BaseDirectory + @"\..\..\..\data\tolkien.xml";
        public static int ROWS_NUMBER = 10380;

        public static void Main(string[] args)
        {
            Test(ParseData());
            Console.ReadKey();
        }

        public static DomainCreator ParseData()
        { 
            XMLParser xmlParser = new XMLParser(xmlPath);
            xmlParser.Parse();
            IList<PostHolder> postHolders =  xmlParser.RetrievePosts();

            return new DomainCreator(postHolders);
        }

        public static void Test(DomainCreator domainCreator)
        { 
            Stopwatch sw = new Stopwatch();
            IList<NHibernateHelper> nHibernateHelpers = new List<NHibernateHelper> {new IdentityNHibernateHelper(), new HiloNHibernateHelper()/*, new IncrementNHibernateHelper()*/};
            IList<IDataAccess> dataAccesses = new List<IDataAccess> { new SimpleDataAccess(), new ImprovedDataAccess()};

            foreach (NHibernateHelper nHibernateHelper in nHibernateHelpers)
            {
                Console.WriteLine(nHibernateHelper.GetType().Name);
                foreach (IDataAccess dataAccess in dataAccesses)
                {
                    nHibernateHelper.Init();
                    Console.WriteLine("\t" + dataAccess.GetType().Name);

                    dataAccess.NHibernateHelper = nHibernateHelper;

                    sw.Start();
                    dataAccess.PrepareDatabase();
                    sw.Stop();
                    Console.WriteLine("\t\tPrepareDatabase: " + sw.ElapsedMilliseconds / 1000.0);
                    Console.WriteLine("");
                    sw.Reset();

                    IList<User> users = domainCreator.GetUsersFromPostHolders();
                    sw.Start();
                    dataAccess.PersisUsers(users);
                    sw.Stop();
                    Console.WriteLine("\t\tPersist Data: " + sw.ElapsedMilliseconds / 1000.0 + " IpS: " + ROWS_NUMBER / (sw.ElapsedMilliseconds / 1000.0));
                    Console.WriteLine("");
                    sw.Reset();

                    sw.Start();
                    dataAccess.PrepareFullTextSearchIndex();
                    sw.Stop();
                    Console.WriteLine("\t\tPrepareFullTextSearchIndex: " + sw.ElapsedMilliseconds / 1000.0);
                    Console.WriteLine("");
                    sw.Reset();

                    sw.Start();
                    int cretated2013 = dataAccess.CreatedThreads2013Count();
                    sw.Stop();
                    Console.WriteLine("\t\tCreatedThreads2013Count: " + sw.ElapsedMilliseconds / 1000.0 + " Result: " + cretated2013);
                    Console.WriteLine("");
                    sw.Reset();

                    sw.Start();
                    String mostPopularThread = dataAccess.MostPopularThread2013();
                    sw.Stop();
                    Console.WriteLine("\t\tMostPopularThread2013: " + sw.ElapsedMilliseconds / 1000.0 + " Result: " + mostPopularThread);
                    Console.WriteLine("");
                    sw.Reset();

                    sw.Start();
                    Double avgPostLength = dataAccess.AveragePostLength();
                    sw.Stop();
                    Console.WriteLine("\t\tAveragePostLength: " + sw.ElapsedMilliseconds / 1000.0 + " Result: " + avgPostLength);
                    Console.WriteLine("");
                    sw.Reset();

                    sw.Start();
                    String mostActiveByThreadsUser = dataAccess.MostActiveByThreadsUser();
                    sw.Stop();
                    Console.WriteLine("\t\tMostActiveByThreadsUser: " + sw.ElapsedMilliseconds / 1000.0 + " Result: " + mostActiveByThreadsUser);
                    Console.WriteLine("");
                    sw.Reset();

                    sw.Start();
                    String mostActiveByCommentingOtherUsers = dataAccess.MostActiveByCommentingOtherUsers();
                    sw.Stop();
                    Console.WriteLine("\t\tMostActiveByCommentingOtherUsers: " + sw.ElapsedMilliseconds / 1000.0 + " Result: " + mostActiveByCommentingOtherUsers);
                    Console.WriteLine("");
                    sw.Reset();

                    sw.Start();
                    int postConatinsFrodoCount = dataAccess.PostConatinsFrodoCount();
                    sw.Stop();
                    Console.WriteLine("\t\tPostConatinsFrodoCount: " + sw.ElapsedMilliseconds / 1000.0 + " Result: " + postConatinsFrodoCount);
                    Console.WriteLine("");
                    sw.Reset();

                    sw.Start();
                    int postByCityLikeKCount = dataAccess.PostByCityLikeKCount();
                    sw.Stop();
                    Console.WriteLine("\t\tPostByCityLikeKCount: " + sw.ElapsedMilliseconds / 1000.0 + " Result: " + postByCityLikeKCount);
                    Console.WriteLine("");
                    sw.Reset();

                    sw.Start();
                    String mostFrequentWord35th = dataAccess.MostFrequentWord35th();
                    sw.Stop();
                    Console.WriteLine("\t\tMostFrequentWord35th: " + sw.ElapsedMilliseconds / 1000.0 + " Result: " + mostFrequentWord35th);
                    Console.WriteLine("");
                    sw.Reset();

                    nHibernateHelper.Close();
                }
            }

            for (int i = 1; i <= 2; i++)
            {
                SQLBulkCopyDataAccess bulkCopyDataAccess = new SQLBulkCopyDataAccess(domainCreator.GetUsersFromPostHolders());
                Console.WriteLine(bulkCopyDataAccess.GetType().Name + " " + i);

                NHibernateHelper nHelper = nHibernateHelpers[1];
                nHelper.Init();
                IDataAccess dataAccess = dataAccesses[1];
                dataAccess.NHibernateHelper = nHelper;
                dataAccess.PrepareDatabase();

                IDbConnection connection = ((ISessionFactoryImplementor)nHelper.SessionFactory).ConnectionProvider.GetConnection();
                if (i == 1)
                {
                    bulkCopyDataAccess.Prepare();
                    sw.Start();
                    bulkCopyDataAccess.Persist(connection);
                    sw.Stop();
                }
                else
                {
                    sw.Start();
                    bulkCopyDataAccess.Prepare();
                    bulkCopyDataAccess.Persist(connection);
                    sw.Stop();
                }

                Console.WriteLine("\t\tPersist Data: " + sw.ElapsedMilliseconds / 1000.0 + " IpS: " + ROWS_NUMBER / (sw.ElapsedMilliseconds / 1000.0));
                Console.WriteLine("");
                sw.Reset();
                nHelper.Close();
            }

        }
    }
}
