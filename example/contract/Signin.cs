using System.ComponentModel.DataAnnotations;
using MS.RestApi.Abstractions;

namespace contract
{
    /// <summary>
    /// Signin user and generate access token 
    /// </summary>
    [EndPoint(Method.Post, "account/signin/local", "Account")]
    public class Signin : Request<SigninResponse>
    {
        /// <summary>
        /// The user's account email 
        /// </summary>
        [Required, EmailAddress]
        public string Username { get; set; }
        
        /// <summary>
        /// Secret
        /// </summary>
        [Required, MinLength(6), MaxLength(50)]
        public string Password { get; set; }
    }

    /// <summary/>
    public class SigninResponse
    {
        /// <summary>
        /// The user's access token
        /// </summary>
        public string AccessToken { get; set; }
    }
}