using IdentityModel;
using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LaundryBooker.Sts
{
    public static class Config
    {
        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId = "laundrybooker-spa",
                    ClientName = "MrWhite LaundryBooker SPA Client",
                    IncludeJwtId = true,
                    ClientClaimsPrefix = "client_",
                    AccessTokenType = AccessTokenType.Jwt,
                    AlwaysSendClientClaims = true,
                    RequireClientSecret = false,
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    AllowedScopes = new List<string>
                    {
                        "openid",
                        "profile",
                        "biography",
                        "profile_picture_url",
                        "bookings.write",
                        "bookings.read",
                    }
                }
            };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource
                {
                    Description = "MrWhite LaundryBooker Resources",
                    DisplayName = "LaundryBooker Resources",
                    Enabled = true,
                    UserClaims = new List<string>
                    {
                        JwtClaimTypes.Subject,
                        JwtClaimTypes.Name,
                    },
                    Scopes = new List<Scope>
                    {
                        new Scope
                        {
                            Name = "bookings.read"
                        },
                        new Scope
                        {
                            Name = "bookings.write"
                        }
                    }
                }
            };
        }

        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email()
            };
        }
    }
}
