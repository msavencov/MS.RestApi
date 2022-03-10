﻿using System.ComponentModel.DataAnnotations;
using MediatR;
using MS.RestApi.Abstractions;

namespace contract.Account
{
    /// <summary>
    /// Signin user and generate access token 
    /// </summary>
    [EndPoint(Method.Get, "account/sign-in/local", "Account")]
    public class SignInLocal : Request<SignInResponse>, IRequest<SignInResponse>
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
}