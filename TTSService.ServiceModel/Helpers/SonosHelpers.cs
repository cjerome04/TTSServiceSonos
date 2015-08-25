using ServiceStack.Common.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Speech.AudioFormat;
using System.Speech.Synthesis;
using System.Text;
using System.Threading;

namespace TTSService.ServiceModel.Helpers
{
    public class SonosHelpers
    {
        private string _sonosIp;
        private int _sonosPort;
        public SpeechSynthesizer Voice;

        public SonosHelpers(String sonosIp, int sonosPort = 1400)
        {
            _sonosIp = sonosIp;
            _sonosPort = sonosPort;
            Voice = new SpeechSynthesizer();

        }

        private string Upnp(string url, string soapService, string soapAction, string soapArguments = "", string soapFilter = "")
        {
            var post = "<s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\" s:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\">";
            post += "<s:Body>";
            post += string.Format("<u:{0} xmlns:u=\"{1}\">", soapAction, soapService);
            post += soapArguments;
            post += string.Format("</u:{0}>", soapAction);
            post += "</s:Body>";
            post += "</s:Envelope>";

            var postUrl = string.Format("http://{0}:{1}{2}", _sonosIp, _sonosPort, url);

            var request = (HttpWebRequest)WebRequest.Create(postUrl);
            request.Method = "POST";
            request.ContentType = "text/xml; charset=utf-8";
            request.Headers.Add("SOAPAction", String.Format("{0}#{1}", soapService, soapAction));
            request.ProtocolVersion = HttpVersion.Version11;
    		request.Credentials = CredentialCache.DefaultCredentials;
    		
    		Stream requestStream = request.GetRequestStream(); 
    		StreamWriter streamWriter = new StreamWriter(requestStream, Encoding.UTF8);
    		streamWriter.Write(post);             
    		streamWriter.Close();
    		
            /*byte[] buffer = Encoding.GetEncoding("UTF-8").GetBytes(post);
            Stream reqstr = request.GetRequestStream();
            reqstr.Write(buffer, 0, buffer.Length);
            reqstr.Close();*/
            
            var response = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            string content = reader.ReadToEnd();

            if (!string.IsNullOrEmpty(soapFilter))
                return Filter(content, soapFilter);
            else
                return content;
        }

        public string Filter(string subject, string pattern)
        {
            IList<String> tab = subject.Split(new string[] { string.Format("<{0}>", pattern) }, StringSplitOptions.RemoveEmptyEntries);
            if (tab.Count >= 2)
            {
                IList<String> tab2 = tab[1].Split(new string[] { string.Format("</{0}>", pattern) }, StringSplitOptions.RemoveEmptyEntries);
                if (tab2.Count >= 1)
                {
                    return tab2[0];
                }
            }
            return string.Empty;
        }

        public string Play()
        {
            var url = "/MediaRenderer/AVTransport/Control";
            var action = "Play";
            var service = "urn:schemas-upnp-org:service:AVTransport:1";
            var args = "<InstanceID>0</InstanceID><Speed>1</Speed>";
            return Upnp(url, service, action, args);
        }

        /**
	    * Pause
	    */
        public string Pause()
        {
            var url = "/MediaRenderer/AVTransport/Control";
            var action = "Pause";
            var service = "urn:schemas-upnp-org:service:AVTransport:1";
            var args = "<InstanceID>0</InstanceID>";
            return Upnp(url, service, action, args);
        }

        /**
        * Stop
        */
        public string Stop()
        {
            var url = "/MediaRenderer/AVTransport/Control";
            var action = "Stop";
            var service = "urn:schemas-upnp-org:service:AVTransport:1";
            var args = "<InstanceID>0</InstanceID>";
            return Upnp(url, service, action, args);
        }

        public Dictionary<string, string> GetPositionInfo()
        {
            var datas = new Dictionary<string, string>();
            var url = "/MediaRenderer/AVTransport/Control";
            var action = "GetPositionInfo";
            var service = "urn:schemas-upnp-org:service:AVTransport:1";
            var args = "<InstanceID>0</InstanceID>";
            var response = Upnp(url, service, action, args);


            datas["TrackNumberInQueue"] = Filter(response, "Track");
            datas["TrackURI"] = Filter(response, "TrackURI");
            datas["TrackDuration"] = Filter(response, "TrackDuration");
            datas["RelTime"] = Filter(response, "RelTime");
            var TrackMetaData = Filter(response, "TrackMetaData");

            /*$xml = substr($xml, stripos($TrackMetaData, '&lt;'));
		    $xml = substr($xml, 0, strrpos($xml, '&gt;') + 4);*/

            TrackMetaData = TrackMetaData.Replace("&lt;", "<");
            TrackMetaData = TrackMetaData.Replace("&gt;", ">");
            TrackMetaData = TrackMetaData.Replace("&quot;", "\\");
            TrackMetaData = TrackMetaData.Replace("&amp;", "&");
            TrackMetaData = TrackMetaData.Replace("%3a", ":");
            TrackMetaData = TrackMetaData.Replace("%2f", "/");
            TrackMetaData = TrackMetaData.Replace("%25", "%");

            datas["Title"] = Filter(TrackMetaData, "dc:title");  // Track Title
            datas["AlbumArtist"] = Filter(TrackMetaData, "r:albumArtist");       // Album Artist
            datas["Album"] = Filter(TrackMetaData, "upnp:album");        // Album Title
            datas["TitleArtist"] = Filter(TrackMetaData, "dc:creator");   // Track Artist*/
            return datas;

        }

        /**
	    * Get volume value (0-100)
	    */
        public string GetVolume()
        {
            var url = "/MediaRenderer/RenderingControl/Control";
            var action = "GetVolume";
            var service = "urn:schemas-upnp-org:service:RenderingControl:1";
            var args = "<InstanceID>0</InstanceID><Channel>Master</Channel>";
            var filter = "CurrentVolume";
            return Upnp(url, service, action, args, filter);
        }

        /**
        * Set volume value (0-100)
        */
        public string SetVolume(int volume)
        {
            var url = "/MediaRenderer/RenderingControl/Control";
            var action = "SetVolume";
            var service = "urn:schemas-upnp-org:service:RenderingControl:1";
            var args = string.Format("<InstanceID>0</InstanceID><Channel>Master</Channel><DesiredVolume>{0}</DesiredVolume>", volume);
            return Upnp(url, service, action, args);
        }

        /**
	    * Get mute status
	    */
        public string GetMute()
        {
            var url = "/MediaRenderer/RenderingControl/Control";
            var action = "GetMute";
            var service = "urn:schemas-upnp-org:service:RenderingControl:1";
            var args = "<InstanceID>0</InstanceID><Channel>Master</Channel>";
            var filter = "CurrentMute";
            return Upnp(url, service, action, args, filter);
        }

        /**
        * Set mute
        * @param integer mute active=1
        */
        public string SetMute(string mute = "0")
        {
            var url = "/MediaRenderer/RenderingControl/Control";
            var action = "SetMute";
            var service = "urn:schemas-upnp-org:service:RenderingControl:1";
            var args = string.Format("<InstanceID>0</InstanceID><Channel>Master</Channel><DesiredMute>{0}</DesiredMute>", mute);
            return Upnp(url, service, action, args);
        }

        /**
	    * Get Transport Info : get status about player
	    */
        public string GetTransportInfo()
        {
            var url = "/MediaRenderer/AVTransport/Control";
            var action = "GetTransportInfo";
            var service = "urn:schemas-upnp-org:service:AVTransport:1";
            var args = "<InstanceID>0</InstanceID>";
            var filter = "CurrentTransportState";
            return Upnp(url, service, action, args, filter);
        }

        public string TTSToMp3(string text)
        {
            var fileName = string.Format("{0}.mp3", Md5Helpers.CalculateMD5Hash(text));
            MemoryStream ms = new MemoryStream();
            Voice.Volume = 100;
            Voice.Rate = 0;

            Voice.SetOutputToWaveStream(ms);
            //Voice.SetOutputToWaveFile(Path.Combine(string.Format("\\{0}", Settings.DirectoryFile), fileName), new SpeechAudioFormatInfo(32000, AudioBitsPerSample.Sixteen, AudioChannel.Stereo));
            Voice.Speak(text);
            var file = Path.Combine(string.Format("\\\\{0}", Settings.DirectoryFile), fileName);
            Mp3Helpers.ConvertWavStreamToMp3File(ref ms, file);
            return fileName;

        }

        /**
	    * Add URI to Queue
	    * @param string track/radio URI
	    * @param bool added next (=1) or end queue (=0)
	    */
        public String AddURIToQueue(string uri, int next = 0)
        {
            var url = "/MediaRenderer/AVTransport/Control";
            var action = "AddURIToQueue";
            var service = "urn:schemas-upnp-org:service:AVTransport:1";
            var args = string.Format("<InstanceID>0</InstanceID><EnqueuedURI>{0}</EnqueuedURI><EnqueuedURIMetaData></EnqueuedURIMetaData><DesiredFirstTrackNumberEnqueued>0</DesiredFirstTrackNumberEnqueued><EnqueueAsNext>{1}</EnqueueAsNext>", uri, next);
            var filter = "FirstTrackNumberEnqueued";
            return Upnp(url, service, action, args, filter);
        }

        /**
	    * Seek to position xx:xx:xx or track number x
	    * @param string 'REL_TIME' for time position (xx:xx:xx) or 'TRACK_NR' for track in actual queue
	    * @param string
	    */
        public String Seek(string type, String position)
        {
            var url = "/MediaRenderer/AVTransport/Control";
            var action = "Seek";
            var service = "urn:schemas-upnp-org:service:AVTransport:1";
            var args = string.Format("<InstanceID>0</InstanceID><Unit>{0}</Unit><Target>{1}</Target>", type, position);
            return Upnp(url, service, action, args);
        }

        /**
	    * Seek to time xx:xx:xx
	    */
        public string SeekTime(string time)
        {
            return Seek("REL_TIME", time);
        }

        /**
	    * Change to track number
	    */
        public string ChangeTrack(string number)
        {
            return Seek("TRACK_NR", number);
        }

        /**
	    * Remove a track from Queue
	    *
	    */
        public string RemoveTrackFromQueue(string tracknumber)
        {
            var url = "/MediaRenderer/AVTransport/Control";
            var action = "RemoveTrackFromQueue";
            var service = "urn:schemas-upnp-org:service:AVTransport:1";
            var args = string.Format("<InstanceID>0</InstanceID><ObjectID>Q:0/{0}</ObjectID>", tracknumber);
            return Upnp(url, service, action, args);
        }

        /**
	    * Set Queue
	    * @param string URI of new track
	    */
        public string SetQueue(string uri)
        {
            var url = "/MediaRenderer/AVTransport/Control";
            var action = "SetAVTransportURI";
            var service = "urn:schemas-upnp-org:service:AVTransport:1";
            var args = string.Format("<InstanceID>0</InstanceID><CurrentURI>{0}</CurrentURI><CurrentURIMetaData></CurrentURIMetaData>", uri);
            return Upnp(url, service, action, args);
        }

        /**
	    * Say song name via TTS message
	    * @param string message
	    * @param string radio name display on sonos controller
	    * @param int volume
	    * @param string language
	    */
        public void SongNameTTS(int volume = 0, bool unmute = false)
        {
            var ThisSong = "Cette chanson s'appelle ";
            var By = ", elle est interprétée par ";

            var track = GetPositionInfo();

            var SongName = track["Title"];
            var Artist = track["TitleArtist"];

            var message = string.Format("{0}{1}{2}{3}", ThisSong, SongName, By, Artist);

            PlayTTS(message, volume, unmute);

        }

        public void PlayTTS(string text, int volume = 0, Boolean unmute = false)
        {

            var actual = new Dictionary<string, object>();
            actual["track"] = GetPositionInfo();
            actual["volume"] = GetVolume();
            actual["mute"] = GetMute();
            actual["status"] = GetTransportInfo();
            if (actual["status"].ToString() == "PLAYING")
            {
                Pause();
            }

            if (unmute)
                SetMute("0");
            if (volume != 0)
                SetVolume(volume);

            var file = string.Format("x-file-cifs://{0}/{1}", Settings.DirectoryFile, TTSToMp3(text));
            file = file.Replace("\\", "/");

            var track = actual["track"] as Dictionary<string, string>;
            if (track["TrackURI"].StartsWith("x-file-cifs:") || track["TrackURI"].EndsWith(".mp3"))
            {
                // It's a MP3 file
                var TrackNumber = AddURIToQueue(file);
                ChangeTrack(TrackNumber);
			    Play();
                while (true)
                {
				    var ttsFile = GetPositionInfo();
                    if (ttsFile["TrackNumberInQueue"] !=TrackNumber)
					    break;
                    Thread.Sleep(10);
                }
                if (GetTransportInfo().ToString() == "PLAYING")
                {
                    Pause();
                }

                RemoveTrackFromQueue(TrackNumber);

                SetVolume(Convert.ToInt32(actual["volume"]));
                SetMute(actual["mute"].ToString());
                ChangeTrack(track["TrackNumberInQueue"].ToString());
                SeekTime(track["RelTime"].ToString());
            }
            else
            {
                //It's a radio / or TV (playbar) / or nothing
                /*SetQueue(file);
			    Play();
                Thread.Sleep(2000);
                while (GetTransportInfo() == "PLAYING") {  }
                if (GetTransportInfo() == "PLAYING")
                {
                    Pause();
                }*/
                // It's a MP3 file
                var TrackNumber = AddURIToQueue(file);
                ChangeTrack(TrackNumber);
                Play();
                while (true)
                {
                    var ttsFile = GetPositionInfo();
                    if (ttsFile["TrackNumberInQueue"] != TrackNumber)
                        break;
                    Thread.Sleep(10);
                }
                if (GetTransportInfo().ToString() == "PLAYING")
                {
                    Pause();
                }

                RemoveTrackFromQueue(TrackNumber);

                SetVolume(Convert.ToInt32(actual["volume"]));
                SetMute(actual["mute"].ToString());
                ChangeTrack(track["TrackNumberInQueue"].ToString());
                SeekTime(track["RelTime"].ToString());
            }

            if (actual["status"].ToString().Contains("PLAYING") )
			    Play();

        }

        
    }
}
