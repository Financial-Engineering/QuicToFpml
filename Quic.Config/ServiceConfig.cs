using System;
using System.IO;
using System.Web.Hosting;

namespace Quic.Config
{

    /// <summary>
    /// Static configuration
    /// </summary>
    public static class ServiceConfig
    {
        public static DataMap<string, ProductMap.PricingSupport> Products;
        public static DataMap<ObservablesMap.Key, ObservablesMap.ReferenceIndex> Observables;
        public static PricingContext Context;

        static ServiceConfig()
        {
            var path = HostingEnvironment.ApplicationPhysicalPath ?? @"D:\QuIC\etl\code\Quic.WebService\Quic.Config";

            Products = ProductMap.FromXml(File.ReadAllText(Path.Combine(path,@"bin\config\Products.xml")));
            Observables = ObservablesMap.FromXml(File.ReadAllText(Path.Combine(path,@"bin\config\Observables.xml")));
            Context = Utilities.Deserialize<PricingContext>(File.ReadAllText(Path.Combine(path,@"bin\config\PricingContext.xml")));
        }
     
    }
}