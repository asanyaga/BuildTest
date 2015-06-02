using Distributr.Mobile.Core.Net;
using NUnit.Framework;

namespace Distributr.Mobile.Core.Test.Net
{
    [TestFixture]
    public class HttpParamsTest
    {
        [Test]
        public void CanGenerateMultipleUrlParams()
        {
            var httpParams = new HttpParams {{"param1", "param1Value"}, {"param2", "param2Value"}};

            Assert.AreEqual("?param1=param1Value&param2=param2Value", httpParams.ToString());
        }

        [Test]
        public void AppliesUrlEncodingWhenRequired()
        {
            var httpParams = new HttpParams {{"param1", "param1Value@"}, {"param2", "param2 Value"}};

            Assert.AreEqual("?param1=param1Value%40&param2=param2+Value", httpParams.ToString());            
        }

        [Test]
        public void CanBuildASinglePostParam()
        {
            Assert.AreEqual("param1=$Value", HttpParams.CreatePostParam("param1", "$Value"));            
        }

        [Test]
        public void RetunsEmptyStringWhenNoParamsAreAdded()
        {
            Assert.AreEqual(string.Empty, new HttpParams().ToString());
        }
    }
}