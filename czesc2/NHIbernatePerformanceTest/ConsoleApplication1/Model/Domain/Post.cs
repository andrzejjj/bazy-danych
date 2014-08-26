using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication1.Model.Domain
{
    public class Post
    {
        public virtual int ID {get; set;}
        public virtual String Content { get; set; }
        public virtual DateTime CreationDate { get; set; }
        public virtual User User { get; set; }
        public virtual Thread Thread { get; set; }
    }
}
