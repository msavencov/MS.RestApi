using MediatR;
using MS.RestApi.Abstractions;

namespace contract.Account
{
    /// <summary>
    /// Returns account profile by account id
    /// </summary>
    [EndPoint("account/profile/{" + nameof(Id) + "}", "Account")]
    public record Profile : IRequest<ProfileDto>
    {
        /// <summary>
        /// The account identifier
        /// </summary>
        public virtual required int Id { get; init; }

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }
    }
    
    ///
    public class ProfileDto {}
}