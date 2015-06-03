using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Distributr.Mobile.Core.Net;
using Distributr.Mobile.Login;
using Distributr.Mobile.Login.Settings;
using Newtonsoft.Json;
using NHttp;
using NUnit.Framework;

namespace Distributr.Mobile.Core.Test.Support
{
    
    public class WithFakeServerTest : WithFullDatabaseTest
    {
        private HttpServer server;
        private readonly List<FakeResponse> responses = new List<FakeResponse>();
        private readonly List<string> unhandledResponses = new List<string>();

        private LoginSettingsRepository loginSettingsRepository;

        private const string FaviconPath = "/favicon.ico";

        [TestFixtureSetUp]
        public void SetupFakeServer()
        {
            loginSettingsRepository = Resolve<LoginSettingsRepository>();

            server = new HttpServer();
            server.RequestReceived += (s, e) =>
            {
                using (var writer = new StreamWriter(e.Response.OutputStream))
                {
                    var path = e.Request.RawUrl;
                    var response = responses.First(r => r.Path == path);
                    
                    Console.WriteLine("Received request {0}", path);

                    if (response == null)
                    {
                        if (path != FaviconPath)
                            unhandledResponses.Add(path);
                    }
                    else
                    {
                        if (response.IsSatisfied(e))
                        {
                            responses.Remove(response);
                            Console.WriteLine(response.ResponseText);
                            writer.Write(response.ResponseText);
                        }                        
                    }                    
                }
            };

            //Use port zero - this way the OS will assign a free port and we can avoid collisions
            server.EndPoint = new IPEndPoint(IPAddress.Loopback, 0);
            server.Start();

            Console.WriteLine("Starting server at {0}", server.EndPoint);

            //Update login settings so that they match the server URL
            UpdateLoginSettings(server.EndPoint.ToString());
        }

        private void UpdateLoginSettings(string serverUrl)
        {
            var loginSettings = loginSettingsRepository.GetSettings();
            loginSettings.ServerUrl = string.Format("http://{0}", serverUrl);
            loginSettingsRepository.Save(loginSettings);
        }

        [SetUp]
        //Some tests stop the server to verify error conditions
        public void RestartFakeServerIfRequired()
        {
            if (server.State.HasFlag(HttpServerState.Stopped))
            {
                SetupFakeServer();
            }                
        }

        [TearDown]
        public void ClearPreviousResponses()
        {
            responses.Clear();            
        }

        [TestFixtureTearDown]
        public void ShutdownFakeServer()
        {
            if(server.State.HasFlag(HttpServerState.Started))
            {
                server.Stop();
            }            
        }

        public void AssertFakeServerIsSatisfied()
        {
            var happy = true;

            if (unhandledResponses.Any())
            {
                happy = false;
                Console.WriteLine("Unhandled requests at path(s):");
                unhandledResponses.ForEach(Console.WriteLine);
            }

            if (responses.Any())
            {
                happy = false;
                Console.WriteLine("The following response(s) were not consumed:");
                responses.ForEach(Console.WriteLine);
            }

            Assert.True(happy);
        }

        public void AddFakeGetResponse(string path, HttpParams httpParams, string responseText )
        {
            var fullPath = Path.Combine("/", path + httpParams);

            responses.Add(new FakeGetResponse(fullPath, responseText));
        }

        public void AddFakePostResponse(string path, HttpParams httpParams, string responseText, string expectedBody)
        {
            var fullPath = Path.Combine("/", path + httpParams);

            responses.Add(new FakePostResponse(fullPath, responseText, expectedBody));
        }

        public HttpParams CreateLoginParams(User user)
        {
            return new HttpParams
            {
                {"Username", user.Username},
                {"Password", user.Password},
                {"usertype", "DistributorSalesman"}
            };            
        }

        public HttpParams CreateCostCentreApplicationIdParams(User user)
        {
            return new HttpParams
            {
                {"costCentreId", user.CostCentre.ToString()},
                {"applicationDescription", "Android_Application"}
            };            
        }

        public string CreateLoginEndpointResponse(string costCentreId, string errorInfo = "Success")
        {
            return JsonConvert.SerializeObject(new FakeLoginEndpointResponse(costCentreId, errorInfo));
        }

        public string CreateCostCentreApplicationIdtResponse(string costCentreApplicationId, string errorInfo = "Success")
        {
            return JsonConvert.SerializeObject(new FakeCostCentreApplicationIdResponse(costCentreApplicationId, errorInfo));
        }
    }

    public class FakeLoginEndpointResponse
    {
        public FakeLoginEndpointResponse(string costCentreId, string errorInfo = "")
        {
            CostCentreId = costCentreId;
            ErrorInfo = errorInfo;
        }

        public string CostCentreId { get; set; }
        public string ErrorInfo { get; set; }
    }

    public class FakeCostCentreApplicationIdResponse
    {
        public FakeCostCentreApplicationIdResponse(string costCentreApplicationId, string errorInfo = "Success")
        {
            CostCentreApplicationId = costCentreApplicationId;
            ErrorInfo = errorInfo;
        }

        public string CostCentreApplicationId { get; set; }
        public string ErrorInfo { get; set; }
    }

    public abstract class FakeResponse
    {
        protected FakeResponse(string path, string responseText, string httpMethod)
        {
            Path = path;
            ResponseText = responseText;
            HttpHttpMethod = httpMethod;
        }

        public string HttpHttpMethod { get; set; }
        public string Path { get; set; }
        public string ResponseText { get; set; }

        public abstract bool IsSatisfied(HttpRequestEventArgs request);

        protected bool CheckHttpMethod(HttpRequestEventArgs request)
        {
            var actualMethod = request.Request.HttpMethod.ToUpper();
            var result = actualMethod.Equals(HttpHttpMethod);

            if (!result) Console.WriteLine("Expected GET method but was {0}", actualMethod);
            
            return result;
        }

        public override string ToString()
        {
            return string.Format("Mehtod {0}\nPath {1}\nResponseText {2}", HttpHttpMethod, Path, ResponseText);
        }
    }

    public class FakeGetResponse : FakeResponse
    {
        public FakeGetResponse(string path, string responseText) : base(path, responseText, "GET")
        {
        }

        public override bool IsSatisfied(HttpRequestEventArgs request)
        {
            return CheckHttpMethod(request);
        }
    }

    public class FakePostResponse : FakeResponse
    {
        private string ExpectedBody { get; set; }

        public FakePostResponse(string path, string responseText, string expectedBody) : base(path, responseText, "POST")
        {
            ExpectedBody = expectedBody;
        }

        public override bool IsSatisfied(HttpRequestEventArgs request)
        {
            if (!CheckHttpMethod(request)) return false;
            var actualBody = new StreamReader(request.Request.InputStream).ReadToEnd();
            var decodedBody = WebUtility.UrlDecode(actualBody);
            Console.WriteLine(decodedBody);
            return ExpectedBody.Equals(decodedBody);
        }

        public override string ToString()
        {
            return base.ToString() + string.Format("\nExpectedBody {0}", ExpectedBody);
        }
    }
}