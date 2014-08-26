using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Cfg;
using ConsoleApplication1.Model.Domain;

namespace ConsoleApplication1.NHIbernateHelper
{
    class IdentityNHibernateHelper : NHibernateHelper
    {
        protected override void AddMappings(Configuration configuration)
        {
            configuration
                .AddResource("ConsoleApplication1.Model.Mappings.IdentityGenerator.Post.hbm.xml", typeof(Post).Assembly)
                .AddResource("ConsoleApplication1.Model.Mappings.IdentityGenerator.Thread.hbm.xml", typeof(Post).Assembly)
                .AddResource("ConsoleApplication1.Model.Mappings.IdentityGenerator.User.hbm.xml", typeof(Post).Assembly);
        }
    }
}
