using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace zym_api
{
    public class OptionsMethodModule : IHttpModule
    {
        public OptionsMethodModule() { }
        public void Dispose() { }
        public void Init(HttpApplication app)
        {
            app.BeginRequest += new EventHandler(this.BeginRequest);
        }

        public void BeginRequest(object resource, EventArgs e) 
        { 
            HttpApplication app = resource as HttpApplication;
            HttpContext context = app.Context;
            if(context.Request.HttpMethod.ToUpper() == "OPTIONS")
            {
                context.Response.StatusCode = 200;
                context.Response.End();
            }
        }
    }
}