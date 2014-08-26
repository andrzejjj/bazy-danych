using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication1
{
    public class PostHolder
    {
        private DateTime postDate;

        public DateTime PostDate
        {
            get { return postDate; }
            set { postDate = value; }
        }
        private String threadTitle;

        public String ThreadTitle
        {
            get { return threadTitle; }
            set { threadTitle = value; }
        }
        private String userLogin;

        public String UserLogin
        {
            get { return userLogin; }
            set { userLogin = value; }
        }
        private String userCity;

        public String UserCity
        {
            get { return userCity; }
            set { userCity = value; }
        }
        private DateTime? userCreationDate;

        public DateTime? UserCreationDate
        {
            get { return userCreationDate; }
            set { userCreationDate = value; }
        }
        private String postContent;

        public String PostContent
        {
            get { return postContent; }
            set { postContent = value; }
        }
    }
}
