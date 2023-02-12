using IdentityServer4;
using IdentityServer4.Models;
using System;
using System.Collections;
using System.Collections.Generic;

namespace NetCoreIdentityExample
{
    public static class Config
    {
        public static IEnumerable<ApiResource> ApiResources => new ApiResource[]
        {
            new ApiResource("resource_catalog"){Scopes = {"catalog_fullpermission"} },
            new ApiResource("photostock_catalog"){Scopes = {"photostock_fullpermission"} },
            new ApiResource(IdentityServerConstants.LocalApi.ScopeName)
        };
        public static IEnumerable<IdentityResource> IdentityResources => new IdentityResource[]
        {
            new IdentityResources.Email(),
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResource() { Name = "roles", DisplayName = "Roles", Description = "Kullanıcı rolleri", UserClaims = new []{"role"} }
        };
        public static IEnumerable<ApiScope> ApiScopes => new ApiScope[]
        {
        new ApiScope("catalog_fullpermission","Catalog API'si için full erişim"),
        new ApiScope("photostock_fullpermission","Photostock API'si için full erişim"),
        new ApiScope(IdentityServerConstants.LocalApi.ScopeName)
        };
        public static IEnumerable<Client> Clients => new Client[]
        {
            new Client()
            {
                ClientId = "WebMvcClient",
                ClientSecrets = {new Secret("secret".Sha256()) },
                ClientName = "Asp.Net Core MVC",
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                AllowedScopes = {
                    "catalog_fullpermission",
                    "photostock_fullpermission",
                    IdentityServerConstants.LocalApi.ScopeName
                }
            },
            new Client()
            {
                ClientId = "WebMvcClientForUser",
                ClientSecrets = {new Secret("secret".Sha256()) },
                ClientName = "Asp.Net Core MVC",
                AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                AllowedScopes = {
                    IdentityServerConstants.StandardScopes.Email,
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.OfflineAccess,
                    "roles"
                },
                AccessTokenLifetime = 3600,
                RefreshTokenExpiration = TokenExpiration.Absolute,
                AbsoluteRefreshTokenLifetime = (int)(DateTime.Now.AddDays(60) - DateTime.Now).TotalSeconds,
                RefreshTokenUsage = TokenUsage.ReUse
            }
        };

    }
}
