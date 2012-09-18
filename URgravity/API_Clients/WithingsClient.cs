using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OAuth.Net.Common;
using OAuth.Net.Components;
using OAuth.Net.Consumer;
using EndPoint = OAuth.Net.Consumer.EndPoint;
using System.IO;


namespace URgravity
{
    public class WithingsClient
    {
        private const string ConsumerKey = "8d4cc5b4189774239427ce19e18f352c747831f01ddd7565ee2f19d5fb9850";
        private const string ConsumerSecret = "296f1d37366242c754bc2e67e4499308f5f1db9503c207c08343f7d86c1b9";

        private OAuthService service;




        private System.Uri callbackurl;
        private string sessionId;



        private Boolean m_bAuthorized;
        //Access token
        private OAuth.Net.Common.IToken access_token;

        private IToken requestToken;
        private IToken accessToken;
        // API call path to get temporary credentials (request token and secret)
        private const string RequestTokenUrl = "https://oauth.withings.com/account/request_token";

        // Base path of URL where the user will authorize this application
        private const string AuthorizationUrl = "https://oauth.withings.com/account/authorize";

        // API call path to get token credentials (access token and secret)
        private const string AccessTokenUrl = "https://oauth.withings.com/account/access_token";



        public WithingsClient(System.Uri callbackurl, string sessionId)
        {
            m_bAuthorized = false;
            this.callbackurl = callbackurl;
            this.sessionId = sessionId;
        }

        public WithingsClient(System.Uri callbackurl, string sessionId, IToken requestToken, IToken accessToken)
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


        public string GetMeasures(string userid)
        {
            //string url = "http://wbsapi.withings.net/measure?action=getmeas&userid=" + userid;

            string url = " http://wbsapi.withings.net/user?action=getbyuserid&userid=1093152";
            return Access(url);

        }


        public string GetProfile()
        {
            string ApiCallUrl = " http://wbsapi.withings.net/once?action=probe";

            return Access(ApiCallUrl);
        }





        public string Access(string url)
        {


            // Create OAuthService object, containing oauth consumer configuration
            service = OAuthService.Create(
                new EndPoint(RequestTokenUrl, "POST"),         // requestTokenEndPoint
                new Uri(AuthorizationUrl),                     // authorizationUri
                new EndPoint(AccessTokenUrl, "POST"),          // accessTokenEndPoint
                true,                                          // useAuthorizationHeader
                "http://wbsapi.withings.net",                       // realm
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
