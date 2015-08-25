using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface.ServiceModel;
using System;
using System.Collections.Generic;
using System.Speech.Synthesis;

namespace TTSService.ServiceInterface
{
    public class GetVoices : IReturn<GetVoicesResponse>
    {
    }

    public class GetVoicesResponse : IHasResponseStatus
    {
        public GetVoicesResponse()
        {
            ResponseStatus = new ResponseStatus();
            Voices = new List<InstalledVoice>();
        }
        public ResponseStatus ResponseStatus { get; set; }
        public List<InstalledVoice> Voices { get; set; }
    }
}
