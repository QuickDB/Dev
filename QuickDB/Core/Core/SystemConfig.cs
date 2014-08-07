using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuickDB.Core.Session;

namespace QuickDB.Core.Core
{
   public  class SystemConfig
    {
       public SystemConfig()
       {
           using (var session=new QuickDBSessionFor<SystemConfig>())
           {
            var systemConfig=   session.LoadAndCreateIfItDoesntExist();
               if (systemConfig.SystemID == null)
               {
                   systemConfig.SystemID = Guid.NewGuid();
                  session.SaveChanges(true);
               }
           }
       }

       public Guid? SystemID { set; get; }

    }
}
