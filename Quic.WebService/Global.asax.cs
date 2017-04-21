using System;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using Funq;
using Quic.Config;
using Quic.CSV;
using ServiceStack.WebHost.Endpoints;

namespace Quic.WebService
{
    public class Global : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            new WebAppHost().Init();
        }

        protected void Session_Start(object sender, EventArgs e)
        {
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
        }

        protected void Application_Error(object sender, EventArgs e)
        {
        }

        protected void Session_End(object sender, EventArgs e)
        {
        }

        protected void Application_End(object sender, EventArgs e)
        {
        }

        public class WebAppHost : AppHostBase
        {
            public WebAppHost() : base("QuIC Web Services", typeof (Service).Assembly)
            {
            }

            public override void Configure(Container container)
            {
                try
                {
                    // Preinitialize caches
                    Parallel.Invoke(
                        () => new CounterpartyHierarchy(ServiceConfig.Context.CptyHierarchy).Init(),
                        () => new LegalDocument(ServiceConfig.Context.LegalDocument).Init(),
                        () => new Portfolio(ServiceConfig.Context.Portfolio).Init(),
                        () => new Schedules(ServiceConfig.Context.Schedules).Init(),
                        () => new MarketData(ServiceConfig.Context.MarketDataCsv).Init(),
                        () => new ExportParams(ServiceConfig.Context.ExportParams).Init()
                        );
                }
                catch (Exception e)
                {
                   throw new FileNotFoundException("Error initializing cache, file not found: " + e.Message);
                }
            }
        }
    }
}