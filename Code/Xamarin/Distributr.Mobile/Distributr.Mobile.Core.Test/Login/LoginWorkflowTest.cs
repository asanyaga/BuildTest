using System;
using System.Net;
using System.Threading.Tasks;
using Distributr.Mobile.Core.Util;
using Distributr.Mobile.Login;
using Moq;
using NUnit.Framework;

namespace Distributr.Mobile.Core.Login.Test
{
    [TestFixture]
    public class LoginWorkflowTest
    {
        //MD5 Hash of string "password"
        private readonly string hashedPassword = "5f4dcc3b5aa765d61d8327deb882cf99";
        private readonly string password = "password";
        private readonly string user1 = "user1";
        private Mock<ILoginClient> loginClientMock;
        private Mock<ILoginRepository> loginRepositoryMock;

        [SetUp]
        public void Setup()
        {
            loginRepositoryMock = new Mock<ILoginRepository>();
            loginClientMock = new Mock<ILoginClient>();
        }

        [Test]
        public void CallsRemoteLoginAPIWhenLocalUserIsNotFound()
        {
            loginRepositoryMock.Setup(loginRepository => loginRepository.FindUser(user1)).Returns((User) null);

            var task = Task.FromResult(Result<User>.Success(new User()));

            loginClientMock.Setup(loginClient => loginClient.Login(user1, hashedPassword)).Returns(task);

            var _loginRepository = loginRepositoryMock.Object;
            var _loginClient = loginClientMock.Object;

            var loginWorkflow = new LoginWorkflow(_loginRepository, _loginClient);

            var result = loginWorkflow.Login(user1, password).Result;

            Assert.AreEqual(task.Result.value, result.value);
        }

        [Test]
        public void LoginWithValidPasswordReturnsLocalUser()
        {
            var user = new User
            {
                Username = user1,
                Password = hashedPassword
            };

            loginRepositoryMock.Setup(loginRepository => loginRepository.FindUser(user1)).Returns(user);

            var _loginRepository = loginRepositoryMock.Object;
            var _loginClient = loginClientMock.Object;

            var loginWorkflow = new LoginWorkflow(_loginRepository, _loginClient);
            var result = loginWorkflow.Login(user1, password);

            Assert.AreEqual(user, result.Result.value);
            Assert.IsTrue(result.Result.WasSuccessful());
        }

        [Test]
        public void LoginWithInvalidPasswordReturnsErrorMessage()
        {
            var user = new User
            {
                Username = user1,
                Password = String.Empty
            };

            loginRepositoryMock.Setup(loginRepository => loginRepository.FindUser(user1)).Returns(user);

            var loginWorkflow = new LoginWorkflow(loginRepositoryMock.Object, loginClientMock.Object);

            var result = loginWorkflow.Login(user1, password);

            Assert.AreEqual(user, result.Result.value);
            Assert.AreEqual("Invalid Username or Password", result.Result.message);
        }

        [Test]
        public void LoginWhenNetworkUnavailableReturnsError()
        {
            loginRepositoryMock.Setup(loginRepository => loginRepository.FindUser(user1)).Returns((User) null);

            loginClientMock.Setup(loginClient => loginClient.Login(user1, hashedPassword)).Throws(new WebException());

            var _loginRepository = loginRepositoryMock.Object;
            var _loginClient = loginClientMock.Object;

            var loginWorkflow = new LoginWorkflow(_loginRepository, _loginClient);
            var result = loginWorkflow.Login(user1, password);

            Assert.AreEqual("Network Error", result.Result.message);
        }

        [Test]
        public void UpdatesLastLoggedInUserWhenNewUserLogsIn()
        {
            var user = new User
            {
                Username = user1,
                Password = hashedPassword
            };

            loginRepositoryMock.Setup(loginRepository => loginRepository.FindUser(user1)).Returns(user);
            loginRepositoryMock.Setup(loginRepository => loginRepository.GetLastUser()).Returns("different user");

            var _loginRepository = loginRepositoryMock.Object;
            var _loginClient = loginClientMock.Object;

            var loginWorkflow = new LoginWorkflow(_loginRepository, _loginClient);

            var result = loginWorkflow.Login(user1, password).Result;

            loginRepositoryMock.Verify(loginRepository => loginRepository.SetLastUser(String.Empty), Times.Once());
        }
    }
}