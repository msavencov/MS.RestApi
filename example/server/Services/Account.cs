using System.Threading;
using System.Threading.Tasks;
using contract.Account;
using MediatR;

namespace server.Services;

internal class Account : 
    IRequestHandler<contract.Account.SignInLocal, contract.Account.SignInResponse>,
    IRequestHandler<contract.Account.SignOut>
    
{
    public Task<SignInResponse> Handle(SignInLocal request, CancellationToken cancellationToken)
    {
        throw new System.NotImplementedException();
    }

    public Task<Unit> Handle(SignOut request, CancellationToken cancellationToken)
    {
        return Task.FromResult(Unit.Value);
    }
}