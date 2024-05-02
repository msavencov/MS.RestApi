using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MediatR;
using MS.RestApi.Abstractions;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

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
        /// User name
        /// </summary>
        public string Name { get; set; }
        
        /// <summary/>
        public IAttachment Avatar { get; set; }

        /// <summary/>
        public List<IAttachment> Documents { get; set; } = new();
        
        /// <summary/>
        public ProfileData Inner { get; set; }
    }

    public class ProfileData
    {
        public string Name { get; set; }
        public IAttachment Doc { get; set; }
        
        [Required]
        public List<IAttachment> AttachmentsCollection { get; set; } = new();
    }

    ///
    public class ProfileDto {}
}