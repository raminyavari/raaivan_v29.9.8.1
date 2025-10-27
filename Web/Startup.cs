using System;
using Microsoft.Owin;
using Owin;
using Microsoft.AspNet.SignalR;
using RaaiVan.Web.API;
using RaaiVan.Modules.GlobalUtilities;
using System.Collections.Generic;
using System.Web;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using RaaiVan.Web;

[assembly: OwinStartup(typeof(Startup))]

namespace RaaiVan.Web
{
    /// <summary>
    /// The server needs to know which URL to intercept and direct to SignalR. To do that we add an OWIN startup class.
    /// </summary>
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            RVScheduler.run_jobs();

            // Any connection or hub wire up and configuration should go here
            if (RaaiVanSettings.RealTime(null))
            {
                string redisConnectionString = RedisAPI.Enabled ? 
                    RedisSessionStateProviderSettings.getConnectionString() : string.Empty;

                if (!string.IsNullOrEmpty(redisConnectionString)) {
                    RedisScaleoutConfiguration redisConf = new RedisScaleoutConfiguration(redisConnectionString, "rv_teal_time");
                    GlobalHost.DependencyResolver.UseStackExchangeRedis(redisConf);
                }

                app.MapSignalR("/signalr", new HubConfiguration());
            }
            else
            {
                app.Map("/signalr", conf =>
                {
                    conf.Use((context, next) =>
                    {
                        ParamsContainer paramsContainer = new ParamsContainer(HttpContext.Current);
                        paramsContainer.file_response("var messageFromRamin = {\"Message\":\"-)\"};",
                            "empty.js", "text/javascript", isAttachment: false);
                        return next();
                    });
                });
            }
            
            //Ignore SSL certificate check for web requests
            ServicePointManager.ServerCertificateValidationCallback = delegate (object s,
                X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
            {
                return true;
            };
            //end of Ignore SSL certificate check for web requests

            app.Map("/ui", spa => {
                spa.Use((context, next) => {
                    context.Request.Path = new PathString("/ui/build/index.html");
                    return next();
                });

                spa.UseStaticFiles();
            });

            ConfigureAuth(app);
        }

        public void ConfigureAuth(IAppBuilder app)
        {
            app.UseTenantCore(new HostNameTenantResolver(RaaiVanSettings.Tenants));
        }
    }
}
