using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface.ServiceModel;
using System.Speech.Synthesis;

namespace TTSService.ServiceInterface
{
    public class GetCurrentVoice : IReturn<GetCurrentVoiceResponse>
    {
    }

    public class GetCurrentVoiceResponse : IHasResponseStatus
    {
        public GetCurrentVoiceResponse()
        {
            ResponseStatus = new ResponseStatus();
            
        }
        public ResponseStatus ResponseStatus { get; set; }
        public VoiceInfo Voice { get; set; }
    }
}
