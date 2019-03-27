using IdentityModel;
using IdentityServer4;
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
                    ClientId = "laundrybooker-postman",
                    ClientName = "MrWhite LaundryBooker Postman Client",
                    IncludeJwtId = true,
                    ClientClaimsPrefix = "client_",
                    AccessTokenType = AccessTokenType.Jwt,
                    AlwaysSendClientClaims = true,
                    RequireClientSecret = false,
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    AllowedScopes = new List<string>
                    {
                        "bookings.write",
                        "bookings.read",
                        "bookings.delete",
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile
                    }
                },
                new Client
                {
                    ClientId = "laundrybooker-spa",
                    ClientName = "MrWhite LaundryBooker SPA Client",
                    IncludeJwtId = true,
                    ClientClaimsPrefix = "client_",
                    AccessTokenType = AccessTokenType.Jwt,
                    AlwaysSendClientClaims = true,
                    RequireClientSecret = false,
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,
                    RequireConsent = false,
                    AllowedCorsOrigins = new List<string>
                    {
                        "http://localhost:3000",
                        "http://192.168.99.100:30505"
                    },
                    AllowedScopes = new List<string>
                    {
                        "bookings.write",
                        "bookings.read",
                        "bookings.delete",
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile
                    },
                    RedirectUris = new List<string>
                    {
                        "http://localhost:3000/authentication/callback",
                        "http://192.168.99.100:30400/authentication/callback"
                    },
                    PostLogoutRedirectUris = new List<string>
                    {
                        "http://localhost:3000",
                        "http://192.168.99.100:30505"
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
                        },
                        new Scope
                        {
                            Name = "bookings.delete"
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
