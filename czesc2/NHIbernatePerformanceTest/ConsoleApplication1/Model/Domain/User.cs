using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iesi.Collections;

namespace ConsoleApplication1.Model.Domain
{
    public class User
    {
        public User()
        {
            Threads = new ListSet();
            Posts = new ListSet();
        }

        public virtual int ID { get; set; }
        public virtual String Login { get; set; }
        public virtual String City { get; set; }
        public virtual DateTime? CreationDate { get; set; }
        public virtual ISet Threads { get; set; }
        public virtual ISet Posts { get; set; }
    }
}
