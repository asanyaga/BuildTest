using System;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Distributr.Mobile.Core.Util;

namespace Distributr.Mobile.Login
{
    public class LoginWorkflow
    {
        private readonly ILoginClient loginClient;
        private readonly ILoginRepository loginRepository;

        public LoginWorkflow(ILoginRepository loginRepository, ILoginClient loginClient)
        {
            this.loginRepository = loginRepository;
            this.loginClient = loginClient;
        }

        public async Task<Result<User>> Login(string username, string password)
        {
            var hashedPassword = Hash(password);
            var result = LoginLocally(username, hashedPassword);

            if (result.Value != null)
            {
                return result;
            }
            return await LoginRemotely(username, hashedPassword);
        }

        private Result<User> LoginLocally(string username, string password)
        {
            var user = loginRepository.FindUser(username);
            if (user != null)
            {
                if (user.Password == password)
                {
                    user.IsNewUser = IsNewUser(username);
                    return Result<User>.Success(user);
                }
                return Result<User>.Failure(user, "Invalid Username or Password");
            }
            return Result<User>.Failure("Local user not found");
        }

        private async Task<Result<User>> LoginRemotely(string username, string password)
        {
            try
            {
                return await loginClient.Login(username, password);
            }
            catch (WebException e)
            {
                return Result<User>.Failure(e, "Network Error");
            }
            catch (TaskCanceledException e)
            {
                return Result<User>.Failure(e, "Connection Timeout");
            }
            catch (Exception e)
            {
                return Result<User>.Failure(e, "An unexpected error occured");
            }
        }

        private bool IsNewUser(string username)
        {
            var lastUser = loginRepository.GetLastUser();
            if (lastUser.LastUser != username)
            {
                loginRepository.SetLastUser(String.Empty, String.Empty);
                return true;
            }
            return false;
        }

        private string Hash(string password)
        {
            var md5 = MD5.Create();
            var encoding = new UTF8Encoding();
            var bytes = encoding.GetBytes(password);
            var hash = md5.ComputeHash(bytes);

            var buffer = new StringBuilder();

            for (var i = 0; i < hash.Length; i++)
            {
                buffer.Append(hash[i].ToString("x2"));
            }
            return buffer.ToString();
        }
    }
}