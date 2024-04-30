using MediatR;
using MS.RestApi.Abstractions;

namespace contract.Account
{
    /// <summary>
    /// Returns account profile by account id
    /// </summary>
    [EndPoint("account/profile/{" + nameof(Id) + "}", "Account")]
    public class Profile : IRequest<ProfileDto>
    {
        /// <summary>
        /// The account identifier
        /// </summary>
        public required int Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }
    }
    
    ///
    public class ProfileDto {}
}