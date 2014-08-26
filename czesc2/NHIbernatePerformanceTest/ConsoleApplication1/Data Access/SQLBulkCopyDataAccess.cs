using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ConsoleApplication1.Model.Domain;
using System.Data.SqlClient;

namespace ConsoleApplication1.Data_Access
{
    public class SQLBulkCopyDataAccess
    {
        private IList<User> userList;
        private DataTable users;
        private DataTable threads;
        private DataTable posts;

        public SQLBulkCopyDataAccess(IList<User> userList)
        {
            this.userList = userList;
            
        }

        public void Prepare()
        {
            users = new DataTable("Users");
            users.Columns.Add(new DataColumn("USR_ID", typeof(int)));
            users.Columns.Add(new DataColumn("USR_Login", typeof(string)));
            users.Columns.Add(new DataColumn("USR_City", typeof(string)));
            users.Columns.Add(new DataColumn("USR_CreationDate", typeof(DateTime)));

            threads = new DataTable("Threads");
            threads.Columns.Add(new DataColumn("THR_ID", typeof(int)));
            threads.Columns.Add(new DataColumn("THR_Title", typeof(string)));
            threads.Columns.Add(new DataColumn("THR_CreationDate", typeof(DateTime)));
            threads.Columns.Add(new DataColumn("THR_USRID", typeof(int)));

            posts = new DataTable("Posts");
            posts.Columns.Add(new DataColumn("PST_ID", typeof(int)));
            posts.Columns.Add(new DataColumn("PST_Content", typeof(string)));
            posts.Columns.Add(new DataColumn("PST_CreationDate", typeof(DateTime)));
            posts.Columns.Add(new DataColumn("PST_USRID", typeof(int)));
            posts.Columns.Add(new DataColumn("PST_THRID", typeof(int)));

            int counter = 0;

            foreach (User user in userList)
            {
                user.ID = ++counter;

                var row = users.NewRow();
                row["USR_ID"] = user.ID;
                row["USR_Login"] = user.Login;
                row["USR_City"] = user.City;
                row["USR_CreationDate"] = user.CreationDate != null ? user.CreationDate : Convert.DBNull;
                users.Rows.Add(row);
            }

            foreach (User user in userList)
            {
                foreach (Thread thread in user.Threads)
                {
                    thread.ID = ++counter;

                    var row = threads.NewRow();
                    row["THR_ID"] = thread.ID;
                    row["THR_Title"] = thread.Title;
                    row["THR_CreationDate"] = thread.CreationDate;
                    row["THR_USRID"] = thread.User.ID;
                    threads.Rows.Add(row);
                }
            }

            foreach (User user in userList)
            {
                foreach (Thread thread in user.Threads)
                {
                    foreach (Post post in thread.Posts)
                    {
                        post.ID = ++counter;

                        var row = posts.NewRow();
                        row["PST_ID"] = post.ID;
                        row["PST_Content"] = post.Content;
                        row["PST_CreationDate"] = post.CreationDate;
                        row["PST_USRID"] = post.User.ID;
                        row["PST_THRID"] = post.Thread.ID;
                        posts.Rows.Add(row);
                    }
                }
            }
        }

        public void Persist(IDbConnection connection)
        {
            using (connection)
            using (SqlTransaction transaction = ((SqlConnection)connection).BeginTransaction())
            {
                var s = (SqlConnection)connection;
                var copyUsers = new SqlBulkCopy(s, SqlBulkCopyOptions.Default, transaction);
                //copyUsers.BatchSize = 5000;
                copyUsers.BulkCopyTimeout = 10000;
                copyUsers.DestinationTableName = "Users";
                foreach (DataColumn column in users.Columns)
                {
                    copyUsers.ColumnMappings.Add(column.ColumnName, column.ColumnName);
                }
                copyUsers.WriteToServer(users);

                var copyThreads = new SqlBulkCopy(s, SqlBulkCopyOptions.Default, transaction);
                //copyThreads.BatchSize = 5000;
                copyThreads.BulkCopyTimeout = 10000;
                copyThreads.DestinationTableName = "Threads";
                foreach (DataColumn column in threads.Columns)
                {
                    copyThreads.ColumnMappings.Add(column.ColumnName, column.ColumnName);
                }
                copyThreads.WriteToServer(threads);

                var copyPosts = new SqlBulkCopy(s, SqlBulkCopyOptions.Default, transaction);
                //copyPosts.BatchSize = 5000;
                copyPosts.BulkCopyTimeout = 10000;
                copyPosts.DestinationTableName = "Posts";
                foreach (DataColumn column in posts.Columns)
                {
                    copyPosts.ColumnMappings.Add(column.ColumnName, column.ColumnName);
                }
                copyPosts.WriteToServer(posts);

                try
                {
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }
    }
}
