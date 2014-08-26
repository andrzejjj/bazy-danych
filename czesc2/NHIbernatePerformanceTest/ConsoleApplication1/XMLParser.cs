using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Globalization;
using System.Text.RegularExpressions;

namespace ConsoleApplication1
{
    public class XMLParser
    {
        private static readonly String patternUserDate = @"Dołączył\(a\): .*";
        private static readonly String patternUserCity = @"Skąd: .*";

        private String xmlPath; 
        private XmlNode rootNode;


        public XMLParser(String xmlPath)
        {
            this.xmlPath = xmlPath;
        }

        public void Parse()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlPath);
            rootNode = xmlDoc.ChildNodes[0];
        }

        public IList<PostHolder> RetrievePosts()
        { 
            List<PostHolder> posts = new List<PostHolder>();

            foreach (XmlNode taskResultNode in rootNode.ChildNodes)
            {
                //task_result
                PostHolder postHolder = new PostHolder();
                foreach (XmlNode node in taskResultNode.SelectNodes("rule"))
                {
                    switch (node.Attributes[0].Value)
                    {
                        case "user-data":
                            FillUserDate(postHolder, node.InnerText);
                            break;
                        case "user-login":
                            postHolder.UserLogin = node.InnerText.Trim();
                            break;
                        case "post-content":
                            postHolder.PostContent = node.InnerText;
                            break;
                        case "post-details":
                            String postDate = node.InnerText.Substring(10, 16);
                            postHolder.PostDate = DateTime.ParseExact(postDate, "dd-MM-yyyy HH:mm", null);
                            break;
                        case "thread-title":
                            postHolder.ThreadTitle = node.InnerText.Substring(8).Trim();
                            break;
                        default:
                            break;
                    }
                }
                posts.Add(postHolder);
            }

            return posts;
        }

        private void FillUserDate(PostHolder postHolder, string content)
        {
            String userDate = null;
            Regex regex = new Regex(patternUserDate);
            Match match = regex.Match(content);
            if (match.Success)
            {
                userDate = match.Value.Substring(13);
                postHolder.UserCreationDate = DateTime.ParseExact(userDate, "dd MMM yyyy", null);
            }

            regex = new Regex(patternUserCity);
            match = regex.Match(content);
            if (match.Success)
                postHolder.UserCity = match.Value.Substring(6).Trim();
        }
    }
}
