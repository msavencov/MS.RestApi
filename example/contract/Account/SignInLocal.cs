using System.ComponentModel.DataAnnotations;
using MS.RestApi.Abstractions;

namespace contract.Account
{
    /// <summary>
    /// Signin user and generate access token 
    /// </summary>
    [EndPoint("account/sign-in/local", "Account")]
    public class SignInLocal : MediatR.IRequest<SignInResponse>
    {
        /// <summary>
        /// The user's account email 
        /// </summary>
        [Required, EmailAddress, MaxLength(320)]
        public string Username { get; set; }
        
        /// <summary>
        /// Secret
        /// </summary>
        [Required, MinLength(6), MaxLength(50)]
        public string Password { get; set; }
    }
}