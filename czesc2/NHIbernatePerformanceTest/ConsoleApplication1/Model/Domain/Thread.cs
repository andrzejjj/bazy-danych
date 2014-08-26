using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iesi.Collections;

namespace ConsoleApplication1.Model.Domain
{
    public class Thread
    {
        public Thread()
        {
            Posts = new ListSet();
        }

        public virtual int ID { get; set; }
        public virtual String Title { get; set; }
        public virtual DateTime CreationDate { get; set; }
        public virtual User User { get; set; }
        public virtual ISet Posts { get; set; }
    }
}
