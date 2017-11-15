using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using UnityEngine;
using System.Text;
using UnityEngine.Networking;

namespace YoutubeLight
{
    public class RequestResolver : MonoBehaviour
    {
        private const string RateBypassFlag = "ratebypass";
        private const string SignatureQuery = "signature";
        public IEnumerator DecryptDownloadUrl(Action<string> callback, VideoInfo videoInfo)
        {
            IDictionary<string, string> queries = HTTPHelperYoutube.ParseQueryString(videoInfo.DownloadUrl);
            if (queries.ContainsKey(SignatureQuery))
            {
                string encryptedSignature = queries[SignatureQuery];

                //decrypted = GetDecipheredSignature( encryptedSignature);
                //MagicHands.DecipherWithVersion(encryptedSignature, videoInfo.HtmlPlayerVersion);
                string jsUrl = string.Format("http://s.ytimg.com/yts/jsbin/player-{0}.js", videoInfo.HtmlPlayerVersion);
                yield return StartCoroutine(DownloadUrl(jsUrl));
                string js = urlResult;
                //Find "C" in this: var A = B.sig||C (B.s)
                string functNamePattern = @"\""signature"",\s?([a-zA-Z0-9\$]+)\("; //Regex Formed To Find Word or DollarSign

                var funcName = Regex.Match(js, functNamePattern).Groups[1].Value;

                if (funcName.Contains("$"))
                {
                    funcName = "\\" + funcName; //Due To Dollar Sign Introduction, Need To Escape
                }

                string funcPattern = @"(?!h\.)" + @funcName + @"=function\(\w+\)\{.*?\}"; //Escape funcName string
                var funcBody = Regex.Match(js, funcPattern, RegexOptions.Singleline).Value; //Entire sig function
                var lines = funcBody.Split(';'); //Each line in sig function

                string idReverse = "", idSlice = "", idCharSwap = ""; //Hold name for each cipher method
                string functionIdentifier = "";
                string operations = "";

                foreach (var line in lines.Skip(1).Take(lines.Length - 2)) //Matches the funcBody with each cipher method. Only runs till all three are defined.
                {
                    if (!string.IsNullOrEmpty(idReverse) && !string.IsNullOrEmpty(idSlice) &&
                        !string.IsNullOrEmpty(idCharSwap))
                    {
                        break; //Break loop if all three cipher methods are defined
                    }

                    functionIdentifier = GetFunctionFromLine(line);
                    string reReverse = string.Format(@"{0}:\bfunction\b\(\w+\)", functionIdentifier); //Regex for reverse (one parameter)
                    string reSlice = string.Format(@"{0}:\bfunction\b\([a],b\).(\breturn\b)?.?\w+\.", functionIdentifier); //Regex for slice (return or not)
                    string reSwap = string.Format(@"{0}:\bfunction\b\(\w+\,\w\).\bvar\b.\bc=a\b", functionIdentifier); //Regex for the char swap.

                    if (Regex.Match(js, reReverse).Success)
                    {
                        idReverse = functionIdentifier; //If def matched the regex for reverse then the current function is a defined as the reverse
                    }

                    if (Regex.Match(js, reSlice).Success)
                    {
                        idSlice = functionIdentifier; //If def matched the regex for slice then the current function is defined as the slice.
                    }

                    if (Regex.Match(js, reSwap).Success)
                    {
                        idCharSwap = functionIdentifier; //If def matched the regex for charSwap then the current function is defined as swap.
                    }
                }

                foreach (var line in lines.Skip(1).Take(lines.Length - 2))
                {
                    Match m;
                    functionIdentifier = GetFunctionFromLine(line);

                    if ((m = Regex.Match(line, @"\(\w+,(?<index>\d+)\)")).Success && functionIdentifier == idCharSwap)
                    {
                        operations += "w" + m.Groups["index"].Value + " "; //operation is a swap (w)
                    }

                    if ((m = Regex.Match(line, @"\(\w+,(?<index>\d+)\)")).Success && functionIdentifier == idSlice)
                    {
                        operations += "s" + m.Groups["index"].Value + " "; //operation is a slice
                    }

                    if (functionIdentifier == idReverse) //No regex required for reverse (reverse method has no parameters)
                    {
                        operations += "r "; //operation is a reverse
                    }
                }

                operations = operations.Trim();

                string magicResult = MagicHands.DecipherWithOperations(encryptedSignature, operations);

                videoInfo.DownloadUrl = HTTPHelperYoutube.ReplaceQueryStringParameter(videoInfo.DownloadUrl, SignatureQuery, magicResult);
                videoInfo.RequiresDecryption = false;
                callback.Invoke(videoInfo.DownloadUrl);
            }
            else
                yield return null;
        }

        private static string GetFunctionFromLine(string currentLine)
        {
            Regex matchFunctionReg = new Regex(@"\w+\.(?<functionID>\w+)\("); //lc.ac(b,c) want the ac part.
            Match rgMatch = matchFunctionReg.Match(currentLine);
            string matchedFunction = rgMatch.Groups["functionID"].Value;
            return matchedFunction; //return 'ac'
        }

        public IEnumerator WebGlRequest(Action<string> callback, string id, string host)
        {
            Debug.Log(host + "getvideo.php?videoid=" + id + "&type=Download");
            WWW request = new WWW(host + "getvideo.php?videoid=" + id + "&type=Download");
            yield return request;
            callback.Invoke(request.text);
        }

        public List<VideoInfo> videoInfos;
		public IEnumerator GetDownloadUrls(Action<List<VideoInfo>> callback, string videoUrl, bool decryptSignature = true)
		{
			yield return StartCoroutine( ExecGetDownloadUrls( videoUrl, decryptSignature ) );

			callback.Invoke( videoInfos );

		}
		public IEnumerator GetDownloadUrls(Action callback, string videoUrl, bool decryptSignature = true)
        {
			yield return StartCoroutine( ExecGetDownloadUrls( videoUrl, decryptSignature ) );
			callback.Invoke();

		}
		public IEnumerator ExecGetDownloadUrls(string videoUrl, bool decryptSignature = true)
			{
            if (videoUrl == null)
                throw new ArgumentNullException("videoUrl");
            ValidateCertificate vl = gameObject.AddComponent<ValidateCertificate>();
            ServicePointManager.ServerCertificateValidationCallback = vl.MyRemoteCertificateValidationCallback;
            Uri uriResult;
            bool result = Uri.TryCreate(videoUrl, UriKind.Absolute, out uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
            if (!result)
                videoUrl = "https://youtube.com/watch?v=" + videoUrl;

            bool isYoutubeUrl = TryNormalizeYoutubeUrl(videoUrl, out videoUrl);
            if (!isYoutubeUrl)
            {
                throw new ArgumentException("URL is not a valid youtube URL!");
            }
            yield return StartCoroutine(DownloadUrl(videoUrl));
            string pageSource = urlResult;
            if (IsVideoUnavailable(pageSource))
            {
                throw new VideoNotAvailableException();
            }
            var dataRegex = new Regex(@"ytplayer\.config\s*=\s*(\{.+?\});", RegexOptions.Multiline);
            string extractedJson = dataRegex.Match(pageSource).Result("$1");
            JObject json = JObject.Parse(extractedJson);
            string videoTitle = GetVideoTitle(json);
            IEnumerable<ExtractionInfo> downloadUrls = ExtractDownloadUrls(json);
            List<VideoInfo> infos = GetVideoInfos(downloadUrls, videoTitle).ToList();
            string htmlPlayerVersion = GetHtml5PlayerVersion(json);
            foreach (VideoInfo info in infos)
            {
                info.HtmlPlayerVersion = htmlPlayerVersion;
                if (decryptSignature && info.RequiresDecryption)
                {
                    Debug.LogWarning("TO DECRYPT I RECOMMEND TO USE THE VIDEO INFO, not by here, made this to work with WebGl");
                    //DecryptDownloadUrl(info);
                }
            }
			videoInfos = infos;
        }

        public static bool TryNormalizeYoutubeUrl(string url, out string normalizedUrl)
        {
            url = url.Trim();

            url = url.Replace("youtu.be/", "youtube.com/watch?v=");
            url = url.Replace("www.youtube", "youtube");
            url = url.Replace("youtube.com/embed/", "youtube.com/watch?v=");

            if (url.Contains("/v/"))
            {
                url = "https://youtube.com" + new Uri(url).AbsolutePath.Replace("/v/", "/watch?v=");
            }

            url = url.Replace("/watch#", "/watch?");

            IDictionary<string, string> query = HTTPHelperYoutube.ParseQueryString(url);

            string v;

            if (!query.TryGetValue("v", out v))
            {
                normalizedUrl = null;
                return false;
            }

            normalizedUrl = "https://youtube.com/watch?v=" + v;

            return true;
        }

        private static IEnumerable<ExtractionInfo> ExtractDownloadUrls(JObject json)
        {
            string[] splitByUrls = GetStreamMap(json).Split(',');
            string[] adaptiveFmtSplitByUrls = GetAdaptiveStreamMap(json).Split(',');
            splitByUrls = splitByUrls.Concat(adaptiveFmtSplitByUrls).ToArray();

            foreach (string s in splitByUrls)
            {
                IDictionary<string, string> queries = HTTPHelperYoutube.ParseQueryString(s);
                string url;

                bool requiresDecryption = false;

                if (queries.ContainsKey("s") || queries.ContainsKey("sig"))
                {
                    requiresDecryption = queries.ContainsKey("s");
                    string signature = queries.ContainsKey("s") ? queries["s"] : queries["sig"];

                    url = string.Format("{0}&{1}={2}", queries["url"], SignatureQuery, signature);

                    string fallbackHost = queries.ContainsKey("fallback_host") ? "&fallback_host=" + queries["fallback_host"] : String.Empty;

                    url += fallbackHost;
                }

                else
                {
                    url = queries["url"];
                }

                url = HTTPHelperYoutube.UrlDecode(url);
                url = HTTPHelperYoutube.UrlDecode(url);

                IDictionary<string, string> parameters = HTTPHelperYoutube.ParseQueryString(url);
                if (!parameters.ContainsKey(RateBypassFlag))
                    url += string.Format("&{0}={1}", RateBypassFlag, "yes");

                yield return new ExtractionInfo { RequiresDecryption = requiresDecryption, Uri = new Uri(url) };
            }
        }

        private static string GetAdaptiveStreamMap(JObject json)
        {
            JToken streamMap = json["args"]["adaptive_fmts"];

            // bugfix: adaptive_fmts is missing in some videos, use url_encoded_fmt_stream_map instead
            if (streamMap == null)
            {
                streamMap = json["args"]["url_encoded_fmt_stream_map"];
            }

            return streamMap.ToString();
        }

        private static string GetHtml5PlayerVersion(JObject json)
        {
            var regex = new Regex(@"player-(.+?).js");

            string js = json["assets"]["js"].ToString();

            return regex.Match(js).Result("$1");
        }

        private static string GetStreamMap(JObject json)
        {
            JToken streamMap = json["args"]["url_encoded_fmt_stream_map"];

            string streamMapString = streamMap == null ? null : streamMap.ToString();

            if (streamMapString == null || streamMapString.Contains("been+removed"))
            {
                throw new VideoNotAvailableException("Video is removed or has an age restriction.");
            }

            return streamMapString;
        }

        private static IEnumerable<VideoInfo> GetVideoInfos(IEnumerable<ExtractionInfo> extractionInfos, string videoTitle)
        {
            var downLoadInfos = new List<VideoInfo>();

            foreach (ExtractionInfo extractionInfo in extractionInfos)
            {
                string itag = HTTPHelperYoutube.ParseQueryString(extractionInfo.Uri.Query)["itag"];

                int formatCode = int.Parse(itag);

                VideoInfo info = VideoInfo.Defaults.SingleOrDefault(videoInfo => videoInfo.FormatCode == formatCode);

                if (info != null)
                {
                    info = new VideoInfo(info)
                    {
                        DownloadUrl = extractionInfo.Uri.ToString(),
                        Title = videoTitle,
                        RequiresDecryption = extractionInfo.RequiresDecryption
                    };
                }

                else
                {
                    info = new VideoInfo(formatCode)
                    {
                        DownloadUrl = extractionInfo.Uri.ToString()
                    };
                }

                downLoadInfos.Add(info);
            }

            return downLoadInfos;
        }

        private static string GetVideoTitle(JObject json)
        {
            JToken title = json["args"]["title"];

            return title == null ? String.Empty : title.ToString();
        }

        private static bool IsVideoUnavailable(string pageSource)
        {
            const string unavailableContainer = "<div id=\"watch-player-unavailable\">";

            return pageSource.Contains(unavailableContainer);
        }

        private string urlResult;
        void StringDownloadComplete(object sender, DownloadStringCompletedEventArgs e)
        {
            urlResult = e.Result;
            Debug.Log("result ok");
            downloadString = true;
        }

        bool downloadString = false;

        IEnumerator DownloadUrl(string url)
        {
            using (var client = new WebClient())
            {
                downloadString = false;
                client.Encoding = System.Text.Encoding.UTF8;
                Uri nuri = new Uri(url);
                client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(StringDownloadComplete);
                client.DownloadStringAsync(nuri);
                while (!downloadString)
                {
                    yield return null;
                }
                yield return true;
            }
        }

        private static void ThrowYoutubeParseException(Exception innerException, string videoUrl)
        {
            throw new YoutubeParseException("Could not parse the Youtube page for URL " + videoUrl + "\n" +
                                            "This may be due to a change of the Youtube page structure.\n" +
                                            "Please report this bug at kelvinparkour@gmail.com with a subject message 'Parse Error' ", innerException);
        }

        private class ExtractionInfo
        {
            public bool RequiresDecryption { get; set; }

            public Uri Uri { get; set; }
        }
    }

    public class ValidateCertificate : MonoBehaviour
    {
        public bool MyRemoteCertificateValidationCallback(System.Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            bool isOk = true;
            // If there are errors in the certificate chain, look at each error to determine the cause.
            if (sslPolicyErrors != SslPolicyErrors.None)
            {
                for (int i = 0; i < chain.ChainStatus.Length; i++)
                {
                    if (chain.ChainStatus[i].Status != X509ChainStatusFlags.RevocationStatusUnknown)
                    {
                        chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
                        chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
                        chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
                        chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
                        bool chainIsValid = chain.Build((X509Certificate2)certificate);
                        if (!chainIsValid)
                        {
                            isOk = false;
                        }
                    }
                }
            }
            return isOk;
        }
    }
}
