using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface.ServiceModel;
using System;

namespace TTSService.ServiceInterface
{
    public class TextToVoice : IReturn<TextToVoiceResponse>
    {
        public String Text { get; set; }
        public String Language { get; set; }
    }

    public class TextToVoiceResponse : IHasResponseStatus
    {
        public TextToVoiceResponse()
        {
            ResponseStatus = new ResponseStatus();
        }
        public ResponseStatus ResponseStatus { get; set; }
        public String Text { get; set; }
    }
}
