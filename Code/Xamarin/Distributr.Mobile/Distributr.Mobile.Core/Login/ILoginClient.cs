using System;
using System.Threading.Tasks;
using Distributr.Mobile.Core.Util;

namespace Distributr.Mobile.Login
{
    public interface ILoginClient
    {
        Task<Result<User>> Login(string username, String password);
    }
}