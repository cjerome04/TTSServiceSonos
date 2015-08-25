using ServiceStack.ServiceInterface;
using System.IO;
using System.Speech.AudioFormat;
using System.Speech.Synthesis;
using TTSService.ServiceInterface;
using TTSService.ServiceModel.Helpers;

namespace TTSService.ServiceModel
{
    public class TTSServices : Service
    {
       
        public SonosHelpers sonos;

        public TTSServices()
        {
            
            sonos = new SonosHelpers(Settings.SonosIp, Settings.SonosPort);
        }

        public TextToVoiceResponse Any(TextToVoice request)
        {
            var response = new TextToVoiceResponse();
            response.Text = request.Text;

            sonos.PlayTTS(request.Text, 50);
            return response;
        }

        public SelectVoiceResponse Any(SelectVoice request)
        {
            var response = new SelectVoiceResponse();


            sonos.Voice.SelectVoice(request.VoiceName);
            response.Voice = sonos.Voice.Voice;
            return response;
        }

        public GetCurrentVoiceResponse Any(GetCurrentVoice request)
        {
            var response = new GetCurrentVoiceResponse();
            response.Voice = sonos.Voice.Voice;
            return response;
        }

        public GetVoicesResponse Any(GetVoices request)
        {
            var response = new GetVoicesResponse();

            var voices = sonos.Voice.GetInstalledVoices();
            response.Voices = voices.GetEnumerator().SaveRest();

            return response;
        }

        public CurrentSongNameResponse Any(CurrentSongName request)
        {
            var response = new CurrentSongNameResponse();
            sonos.SongNameTTS(50);
            return response;
        }


    }

   
}
