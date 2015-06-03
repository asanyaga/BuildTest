using System;
using Distributr.Mobile.Core.Util;
using NUnit.Framework;

namespace Distributr.Mobile.Core.Test.Util
{
    [TestFixture]
    public class ResultTest
    {
        [Test]
        public void SuccessfulResult()
        {
            var result = Result<object>.Success(new object());

            Assert.IsTrue(result.WasSuccessful(), "was successful");
            Assert.IsNull(result.Exception, "was successful");
            Assert.IsNotNull(result.Value, "was successful");
        }

        [Test]
        public void FailureWithExceptionAndMessage()
        {
            var result = Result<object>.Failure(new Exception("something bad happened"), "the message");

            Assert.IsFalse(result.WasSuccessful(), "was successful");
            Assert.IsNotNull(result.Exception, "exception");
            Assert.IsNull(result.Value, "value");
            Assert.AreEqual(result.Message, "the message", "message");
        }

        [Test]
        public void FailureWithoutException()
        {
            var result = Result<object>.Failure("the message");

            Assert.IsFalse(result.WasSuccessful(), "was successful");
            Assert.IsNull(result.Exception, "exception");
            Assert.IsNull(result.Value, "value");
            Assert.AreEqual(result.Message, "the message", "message");
        }

        [Test]
        public void FailureWithValue()
        {
            var result = Result<object>.Failure(new object(), "the message");

            Assert.IsFalse(result.WasSuccessful(), "was successful");
            Assert.IsNull(result.Exception, "exception");
            Assert.IsNotNull(result.Value, "value");
            Assert.AreEqual(result.Message, "the message", "message");
        }

        [Test]
        public void FailureWithOnlyMessage()
        {
            var result = Result<object>.Failure("the message");

            Assert.IsFalse(result.WasSuccessful(), "was successful");
            Assert.IsNull(result.Exception, "exception");
            Assert.IsNull(result.Value, "value");
            Assert.AreEqual(result.Message, "the message", "message");
        }
    }
}