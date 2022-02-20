using System;
using System.Threading;
using System.Threading.Tasks;
using contract;
using server.Generated.Services.Abstractions;

namespace server.Services
{
    internal class AccountService : IAccountService
    {
        public Task<SigninResponse> SigninAsync(Signin model, CancellationToken ct = default)
        {
            return Task.FromResult(new SigninResponse
            {
                AccessToken = Guid.NewGuid().ToString()
            });
        }
    }
}