using ServiceStack.Common.Web;
using ServiceStack.Logging;
using ServiceStack.Logging.Support.Logging;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceModel.Serialization;
using ServiceStack.WebHost.Endpoints;
using TTSService.ServiceInterface;
using TTSService.ServiceModel;

namespace TTSService
{
    public class AppHost : AppHostHttpListenerBase
    {
        private static ILog _log;


        public AppHost()
            : base(Settings.ServiceName, typeof(TTSServices).Assembly)
        {
            
            LogManager.LogFactory = new DebugLogFactory();
            _log = LogManager.GetLogger(typeof(AppHost));
        }

        public override void Configure(Funq.Container container)
        {
            JsonDataContractSerializer.UseSerializer(new JsonNetSerializer());
            //Signal advanced web browsers what HTTP Methods you accept
            base.SetConfig(new EndpointHostConfig
            {
                GlobalResponseHeaders =
                {
                    { "Access-Control-Allow-Origin", "*" },
					//{ "Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS" },
					{ "Access-Control-Allow-Methods", "GET, PUT" },
                },
                WsdlServiceNamespace = "http://schemas.servicestack.net/types",

            });
        }
    }
}
