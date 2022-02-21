using MediatR;
using MS.RestApi.Abstractions;

namespace contract.Account
{
    /// <summary>
    /// Sign out user 
    /// </summary>
    [EndPoint(Method.Post, "account/sign-out", "Account")]
    public class SignOut : Request, IRequest
    {
    }
}