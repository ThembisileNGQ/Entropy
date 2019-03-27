using IdentityModel;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using LaundryBooker.Domain.Model.User;
using LaundryBooker.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace LaundryBooker.Sts.Services
{
    public class ProfileService : IProfileService
    {
        private IUserRepository _userRepository { get; }
        public ProfileService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            Guid subjectId;

            var validSubject = Guid.TryParse(context.Subject.GetSubjectId(), out subjectId);

            if (!validSubject)
            {
                throw new ArgumentException($"{nameof(context.Subject)} is invalid as a subject Id.");
            }

            var userId = UserId.With(subjectId);

            var user = await _userRepository.Find(userId);

            context.IssuedClaims = GetClaims(user).ToList();
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            context.IsActive = true;

            return Task.CompletedTask;
        }

        public static IReadOnlyList<Claim> GetClaims(User user)
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
