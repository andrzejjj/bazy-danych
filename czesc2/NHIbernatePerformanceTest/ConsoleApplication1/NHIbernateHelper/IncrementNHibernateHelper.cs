using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Cfg;
using ConsoleApplication1.Model.Domain;

namespace ConsoleApplication1.NHIbernateHelper
{
    class IncrementNHibernateHelper : NHibernateHelper
    {
        protected override void AddMappings(Configuration configuration)
        {
            configuration
                .AddResource("ConsoleApplication1.Model.Mappings.IncrementGenerator.Post.hbm.xml", typeof(Post).Assembly)
                .AddResource("ConsoleApplication1.Model.Mappings.IncrementGenerator.Thread.hbm.xml", typeof(Post).Assembly)
                .AddResource("ConsoleApplication1.Model.Mappings.IncrementGenerator.User.hbm.xml", typeof(Post).Assembly);
        }
    }
}
