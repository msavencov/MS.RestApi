using System;
using System.Threading;
using System.Threading.Tasks;
using contract.Account;
using MediatR;

namespace server.Services;

internal class Account(ISender mediator) : 
    IRequestHandler<contract.Account.SignInLocal, contract.Account.SignInResponse>,
    IRequestHandler<contract.Account.SignOut>,
    IRequestHandler<Profile, ProfileDto>
{
    public Task<SignInResponse> Handle(SignInLocal request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task Handle(SignOut request, CancellationToken cancellationToken)
    {
        return Task.FromResult(Unit.Value);
    }

    public Task<ProfileDto> Handle(Profile request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}