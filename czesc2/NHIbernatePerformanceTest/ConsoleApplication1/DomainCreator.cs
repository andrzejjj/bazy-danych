using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConsoleApplication1.Model.Domain;

namespace ConsoleApplication1
{
    public class DomainCreator
    {
        private IList<PostHolder> postHolders;
        private IDictionary<String, User> usersMap;
        private IDictionary<String, Thread> threadsMap;
        private IList<Post> posts;
        

        public DomainCreator(IList<PostHolder> postHolders)
        {
            this.postHolders = postHolders;
        }

        public IList<User> GetUsersFromPostHolders()
        {
            this.usersMap = new Dictionary<String, User>();
            this.threadsMap = new Dictionary<String, Thread>();
            this.posts = new List<Post>();

            foreach (PostHolder postHolder in postHolders)
            {
                Post post = CreatePost(postHolder);

                if (posts.Where(p => p.Content.Equals(post.Content) && p.CreationDate.Equals(post.CreationDate)).Count() > 0 /*|| post.Content.Length > 8000*/)
                    continue;

                if (!usersMap.ContainsKey(postHolder.UserLogin))
                    usersMap.Add(postHolder.UserLogin, CreateUser(postHolder));
                User user = usersMap[postHolder.UserLogin];
                post.User = user;
                user.Posts.Add(post);

                if (!threadsMap.ContainsKey(postHolder.ThreadTitle))
                {
                    Thread newThread = CreateThread(postHolder);
                    threadsMap.Add(postHolder.ThreadTitle, newThread);
                }
                Thread thread = threadsMap[postHolder.ThreadTitle];
                post.Thread = thread;
                thread.Posts.Add(post);

                posts.Add(post);
            }

            foreach (Thread thread in threadsMap.Values)
            {
                DateTime minDate = DateTime.MaxValue;
                Post firstPost = null;
                foreach (Post threadPost in thread.Posts)
                {
                    if (threadPost.CreationDate < minDate)
                    {
                        minDate = threadPost.CreationDate;
                        firstPost = threadPost;
                    }
                }
                thread.CreationDate = firstPost.CreationDate;
                thread.User = firstPost.User;
                thread.User.Threads.Add(thread);
            }

            return usersMap.Values.ToList<User>();
        }

        private Post CreatePost(PostHolder postHolder)
        {
            Post post = new Post();
            post.Content = postHolder.PostContent;
            post.CreationDate = postHolder.PostDate;

            return post;
        }

        private User CreateUser(PostHolder postHolder)
        {
            User user = new User();
            user.City = postHolder.UserCity ?? "";
            user.CreationDate = postHolder.UserCreationDate;
            user.Login = postHolder.UserLogin;

            return user;
        }

        private Thread CreateThread(PostHolder postHolder)
        {
            Thread thread = new Thread();
            thread.Title = postHolder.ThreadTitle;

            return thread;
        }
    }
}
