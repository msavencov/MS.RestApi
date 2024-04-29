using System.Threading;
using System.Threading.Tasks;
using contract.Account;
using MediatR;
using My.App.MyApi.Controllers;

namespace server.Services;

internal class Account(ISender mediator) : 
    IRequestHandler<contract.Account.SignInLocal, contract.Account.SignInResponse>,
    IRequestHandler<contract.Account.SignOut>,
    IRequestHandler<Profile, ProfileDto>
{
    public Task<SignInResponse> Handle(SignInLocal request, CancellationToken cancellationToken)
    {
        throw new System.NotImplementedException();
    }

    public Task Handle(SignOut request, CancellationToken cancellationToken)
    {
        return Task.FromResult(Unit.Value);
    }

    public async Task<ProfileDto> Handle(Profile request, CancellationToken cancellationToken)
    {
        request = request with { Name = "sadasd" };

        return await mediator.Send<Profile>(new ProfileGenerated
        {
            Id = 0
        });
    }
}