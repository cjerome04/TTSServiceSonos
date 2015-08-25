using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface.ServiceModel;
using System;

namespace TTSService.ServiceInterface
{
    public class CurrentSongName : IReturn<CurrentSongNameResponse>
    {
    }

    public class CurrentSongNameResponse : IHasResponseStatus
    {
        public CurrentSongNameResponse()
        {
            ResponseStatus = new ResponseStatus();
        }
        public ResponseStatus ResponseStatus { get; set; }
        public String Text { get; set; }
    }
}

