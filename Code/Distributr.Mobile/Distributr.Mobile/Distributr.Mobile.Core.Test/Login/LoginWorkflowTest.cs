using Distributr.Mobile.Core.Test.Support;
using Distributr.Mobile.Login;
using NUnit.Framework;

namespace Distributr.Mobile.Core.Test.Login
{
    [TestFixture]
    public class LoginWorkflowTest : WithFakeServerTest
    {
        
        private const string Password = "12345678";        

        [Test]
        // ReSharper disable once InconsistentNaming
        public void CallsRemoteLoginAPIWhenLocalUserIsNotFound()
        {
            //Given
            var user = AnUnknownUser();
            var loginParams = CreateLoginParams(user);
            var costCentreApplicationIdParams = CreateCostCentreApplicationIdParams(user);

            AddFakeGetResponse(LoginClient.LoginEndpoint, 
                loginParams, 
                CreateLoginEndpointResponse(user.CostCentre.ToString()));           

            AddFakeGetResponse(
                LoginClient.CostCentreApplicationIdEndpoint, 
                costCentreApplicationIdParams, 
                CreateCostCentreApplicationIdtResponse(user.CostCentreApplicationId));

            var loginWorkflow = Resolve<LoginWorkflow>();

            //When
            var result = loginWorkflow.Login(user.Username, Password).Result;

            //Then
            AssertFakeServerIsSatisfied();

            Assert.IsTrue(result.WasSuccessful(), "successful result");
            Assert.IsNotNull(result.Value, "user");
        }

        [Test]
        public void LoginWithValidPasswordReturnsLocalUser()
        {
            //Given
            var user = AUser();

            var loginWorkflow = Resolve<LoginWorkflow>();

            //When
            var result = loginWorkflow.Login(user.Username, Password);

            //Then
            AssertFakeServerIsSatisfied();

            Assert.AreEqual(user.Id, result.Result.Value.Id);
            Assert.IsTrue(result.Result.WasSuccessful());
        }

        [Test]
        public void LoginWithInvalidPasswordReturnsErrorMessage()
        {
            //Given
            var user = AUser();
            var loginWorkflow = Resolve<LoginWorkflow>();

            //When
            var result = loginWorkflow.Login(user.Username, "incorrect password");

            //Then
            AssertFakeServerIsSatisfied();
            Assert.AreEqual(user.Id, result.Result.Value.Id);
            Assert.AreEqual("Invalid Username or Password", result.Result.Message);
        }

        [Test]
        public void LoginWhenNetworkUnavailableReturnsError()
        {
            //Given
            ShutdownFakeServer();

            var user = AnUnknownUser();
            var loginWorkflow = Resolve<LoginWorkflow>();

            //When
            var result = loginWorkflow.Login(user.Username, Password);

            //Then
            Assert.AreEqual("An unexpected error occured", result.Result.Message);
        }

        [Test]
        public void ResetsLastLoggedInUserWhenNewUserLogsIn()
        {
            //Given
            var user = AUser();
            var loginWorkflow = Resolve<LoginWorkflow>();
            var loginRepository = Resolve<LoginRepository>();
            loginRepository.SetLastUser("last user", "cost centre application id");
            Assert.AreEqual("last user", loginRepository.GetLastUser().LastUser, "last user");

            //When
            var result = loginWorkflow.Login(user.Username, Password).Result;

            //Then
            Assert.AreEqual(string.Empty, loginRepository.GetLastUser().LastUser, "last user");
        }

    }
}