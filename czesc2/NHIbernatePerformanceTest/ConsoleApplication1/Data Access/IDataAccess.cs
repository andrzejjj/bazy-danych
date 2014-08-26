using System;
using ConsoleApplication1.NHIbernateHelper;
using ConsoleApplication1.Model.Domain;
namespace ConsoleApplication1.DataAccess
{
    interface IDataAccess
    {
        NHibernateHelper NHibernateHelper
        {
            get;
            set;
        }

        void PrepareDatabase();

        void PrepareFullTextSearchIndex();

        void PersisUsers(System.Collections.Generic.IList<ConsoleApplication1.Model.Domain.User> users);

        int CreatedThreads2013Count();

        String MostPopularThread2013();

        Double AveragePostLength();

        String MostActiveByThreadsUser();

        String MostActiveByCommentingOtherUsers();

        int PostConatinsFrodoCount();

        int PostByCityLikeKCount();

        String MostFrequentWord35th();
    }
}
