using MS.RestApi.Abstractions;

namespace contract.Account
{
    /// <summary>
    /// Sign out user 
    /// </summary>
    [EndPoint("account/sign-out", "Account")]
    public class SignOut : MediatR.IRequest
    {
    }
}