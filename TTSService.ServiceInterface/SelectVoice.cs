using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface.ServiceModel;
using System.Speech.Synthesis;

namespace TTSService.ServiceInterface
{
    public class SelectVoice : IReturn<SelectVoiceResponse>
    {
        public string VoiceName { get; set; }
    }

    public class SelectVoiceResponse : IHasResponseStatus
    {
        public SelectVoiceResponse()
        {
            ResponseStatus = new ResponseStatus();
        }
        public ResponseStatus ResponseStatus { get; set; }
        public VoiceInfo Voice { get; set; }
    }
}
