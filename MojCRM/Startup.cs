using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Owin;
using Owin;
using MojCRM.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;

[assembly: OwinStartup(typeof(MojCRM.Startup))]

namespace MojCRM
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }


    }
}
