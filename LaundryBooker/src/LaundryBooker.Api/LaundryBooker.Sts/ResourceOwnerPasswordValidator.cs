using IdentityModel;
using IdentityServer4.Validation;
using LaundryBooker.Domain.Model.User;
using LaundryBooker.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace LaundryBooker.Sts
{
    public class ResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        private IUserRepository _userRepository  { get; }
        public ResourceOwnerPasswordValidator(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            var user = await _userRepository.Find(context.UserName);

            if (user != null)
            {
                var claims = GetClaims(user);
                context.Result = new GrantValidationResult(user.Id.ToString(), "password", claims);
            }
        }

        public IReadOnlyList<Claim> GetClaims(User user)
        {
            return new List<Claim>
            {
                new Claim(JwtClaimTypes.Subject, user.Id.Value),
                new Claim(JwtClaimTypes.PreferredUserName, user.Name),
                new Claim(JwtClaimTypes.IdentityProvider, "idsvr"),
            };
            
        }
    }
}
