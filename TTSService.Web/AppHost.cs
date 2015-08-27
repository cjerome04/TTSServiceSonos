using ServiceStack.Common.Web;
using ServiceStack.ServiceHost;
using ServiceStack.WebHost.Endpoints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTSService.ServiceModel;

namespace TTSService.Web
{
    public class AppHost : AppHostBase
    {
   

        public AppHost()
            : base(Settings.ServiceName, typeof(TTSServices).Assembly)
        {

        }

        public override void Configure(Funq.Container container)
        {
            // Limite à certains formats
            SetConfig(new EndpointHostConfig
            {
                DebugMode = true,
                DefaultContentType = ContentType.Json,
                EnableFeatures = Feature.All,
            });

           
        }
    }
}
