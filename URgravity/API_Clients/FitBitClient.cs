using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OAuth.Net.Common;
using OAuth.Net.Components;
using OAuth.Net.Consumer;
using EndPoint = OAuth.Net.Consumer.EndPoint;
using System.IO;
using System.Xml.Linq;


namespace URgravity
{
    public class FitBitClient
    {
       private const string ConsumerKey = "2ad1be9d760f47e2b9a60829e9299810";
       private const string ConsumerSecret = "4a60b11b45774e9cb3a1dcbfc4863cd7";

       private OAuthService service;




       private System.Uri callbackurl;
        private string sessionId;



       private Boolean m_bAuthorized;
        //Access token
        private OAuth.Net.Common.IToken access_token;

        private IToken requestToken;
        private IToken accessToken;
       // API call path to get temporary credentials (request token and secret)
       private const string RequestTokenUrl = "https://api.fitbit.com/oauth/request_token";

       // Base path of URL where the user will authorize this application
       private const string AuthorizationUrl = "https://www.fitbit.com/oauth/authorize";

       // API call path to get token credentials (access token and secret)
       private const string AccessTokenUrl = "https://api.fitbit.com/oauth/access_token";

       // API call path to protected resource
      // private const string ApiCallUrl = "http://api.fitbit.com/1/user/-/activities/date/2012-09-13.xml";


        public FitBitClient(System.Uri callbackurl, string sessionId)
        {
            m_bAuthorized = false;
            this.callbackurl = callbackurl;
            this.sessionId = sessionId;
        }

        public FitBitClient(System.Uri callbackurl, string sessionId,IToken requestToken, IToken accessToken)
        {
            m_bAuthorized = true;
            this.requestToken = requestToken;
            this.accessToken = accessToken;
            this.callbackurl = callbackurl;
            this.sessionId = sessionId;
        }


        public Boolean isAuthorizated()
        {

            return m_bAuthorized;
        }

        public IToken getRequestToken()
        {
            return requestToken;
        }

        public IToken getAccessToken()
        {
            return accessToken;
        }



        public string GetProfile()
        {
            string ApiCallUrl = "http://api.fitbit.com/1/user/-/profile.xml";

            return Access(ApiCallUrl);
        }


        public SortedList<string, int> GetCalories()
        {

            SortedList<string, int> CaloriesList = new SortedList<string, int>();

            var data = Access("http://api.fitbit.com/1/user/-/activities/calories/date/today/1m.xml");


            var feedDocument = XDocument.Parse(data);
            var userXml = feedDocument.Element("result").Element("activities-calories").Elements("data");
            
            foreach (XElement elem in userXml)
            {
                string date = elem.Element("dateTime").Value;
                int value = Convert.ToInt32(elem.Element("value").Value);

                CaloriesList.Add(date, value);
            }


            return CaloriesList;
        }

        public SortedList<string, int> GetMinutesAsSleep()
        {
            SortedList<string, int> MinutesAsSleepList = new SortedList<string, int>();

            var data = Access("http://api.fitbit.com/1/user/-/sleep/minutesAsleep/date/today/1m.xml");


            var feedDocument = XDocument.Parse(data);
            var userXml = feedDocument.Element("result").Element("sleep-minutesAsleep").Elements("data");

            foreach (XElement elem in userXml)
            {
                string date = elem.Element("dateTime").Value;
                int value = Convert.ToInt32(elem.Element("value").Value);

                MinutesAsSleepList.Add(date, value);
            }

            return MinutesAsSleepList;

        }


        public SortedList<string, int> GetTotalNumberOfSteps()
        {
            SortedList<string, int> TotalNumberOfStepsList = new SortedList<string, int>();

            var data = Access("http://api.fitbit.com/1/user/-/activities/tracker/steps/date/today/1m.xml");


            var feedDocument = XDocument.Parse(data);
            var userXml = feedDocument.Element("result").Element("activities-tracker-steps").Elements("data");

            foreach (XElement elem in userXml)
            {
                string date = elem.Element("dateTime").Value;
                int value = Convert.ToInt32(elem.Element("value").Value);

                TotalNumberOfStepsList.Add(date, value);
            }

            return TotalNumberOfStepsList;

        }

        public SortedList<string, int> GetTotalFloorsClimbled()
        {
            SortedList<string, int> TotalFloorsClimbledList = new SortedList<string, int>();

            var data = Access("http://api.fitbit.com/1/user/-/activities/tracker/floors/date/today/1m.xml");


            var feedDocument = XDocument.Parse(data);
            var userXml = feedDocument.Element("result").Element("activities-tracker-floors").Elements("data");

            foreach (XElement elem in userXml)
            {
                string date = elem.Element("dateTime").Value;
                int value = Convert.ToInt32(elem.Element("value").Value);

                TotalFloorsClimbledList.Add(date, value);
            }

            return TotalFloorsClimbledList;

        }
        
        public string Access(string url)
        {


            // Create OAuthService object, containing oauth consumer configuration
            service = OAuthService.Create(
                new EndPoint(RequestTokenUrl, "POST"),         // requestTokenEndPoint
                new Uri(AuthorizationUrl),                     // authorizationUri
                new EndPoint(AccessTokenUrl, "POST"),          // accessTokenEndPoint
                true,                                          // useAuthorizationHeader
                "http://api.fitbit.com",                       // realm
                "HMAC-SHA1",                                   // signatureMethod
                "1.0",                                         // oauthVersion
                new OAuthConsumer(ConsumerKey, ConsumerSecret) // consumer
                );
            string content;
            try
            {
                // Create OAuthRequest object, providing protected resource URL

                OAuthRequest request;
                if (m_bAuthorized)
                {
                    request = OAuthRequest.Create(
                        new EndPoint(url, "GET"),
                        service,
                        callbackurl,
                        requestToken, accessToken);

                }
                else
                {


                    request = OAuthRequest.Create(
                        new EndPoint(url, "GET"),
                        service,
                        callbackurl,
                        sessionId);

                }
                

                // Assign verification handler delegate
                request.VerificationHandler = AspNetOAuthRequest.HandleVerification;

                // Call OAuthRequest object GetResource method, which returns OAuthResponse object
                OAuthResponse response = request.GetResource();

                // Check if OAuthResponse object has protected resource
                if (!response.HasProtectedResource)
                {
                    m_bAuthorized = false;
                    // If not we are not authorized yet, build authorization URL and redirect to it
                    string authorizationUrl = service.BuildAuthorizationUrl(response.Token).AbsoluteUri;
                    return authorizationUrl;
                }
                else
                {
                    //Save our data
                    requestToken = request.RequestToken;
                    accessToken = request.AccessToken;


                }

                // Store the access token in session variable
                access_token = response.Token;
                

                // Initialize the XmlDocument object and OAuthResponse object's protected resource to it
                m_bAuthorized = true;


                Stream ms = response.ProtectedResource.GetResponseStream();
                // Jump to the start position of the stream
                ms.Seek(0, SeekOrigin.Begin);

                StreamReader rdr = new StreamReader(ms);
                string anwser_content = rdr.ReadToEnd();


                return anwser_content;
            }

            catch (OAuthRequestException ex)
            {
                return ex.Message;
            }
        }




    }
}
