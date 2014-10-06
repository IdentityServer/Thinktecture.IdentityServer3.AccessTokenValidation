﻿using Microsoft.IdentityModel.Protocols;
using Microsoft.Owin.Security.Jwt;
using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Thinktecture.IdentityServer.v3.AccessTokenValidation;

namespace Owin
{
    public static class SelfContainedTokenValidationExtensions
    {
        public static IAppBuilder UseIdentityServerJwt(this IAppBuilder app, JwtTokenValidationOptions options)
        {
            if (!string.IsNullOrWhiteSpace(options.Authority))
            {
                return app.UseDiscovery(options);
            }
            else
            {
                return app.ConfigureMiddleware(options.IssuerName, options.SigningCertificate, options.AuthenticationType);
            }
        }

        private static IAppBuilder UseDiscovery(this IAppBuilder app, JwtTokenValidationOptions options)
        {
            var authority = options.Authority;

            if (!authority.EndsWith("/"))
            {
                authority += "/";
            }

            authority += ".well-known/openid-configuration";
            var configuration = new ConfigurationManager<OpenIdConnectConfiguration>(authority);

            var result = configuration.GetConfigurationAsync().Result;
            var x5c = result.JsonWebKeySet.Keys.First().X5c.First();

            return app.ConfigureMiddleware(result.Issuer, new X509Certificate2(Convert.FromBase64String(x5c)), options.AuthenticationType);
        }

        private static IAppBuilder ConfigureMiddleware(this IAppBuilder app, string issuerName, X509Certificate2 signingCertificate, string authenticationType)
        {
            if (string.IsNullOrWhiteSpace(issuerName)) throw new ArgumentNullException("issuerName");
            if (signingCertificate == null) throw new ArgumentNullException("signingCertificate");

            var audience = issuerName;

            if (!audience.EndsWith("/"))
            {
                audience += "/";
            }

            audience += "resources";

            app.UseJwtBearerAuthentication(new Microsoft.Owin.Security.Jwt.JwtBearerAuthenticationOptions
            {
                AuthenticationType = authenticationType,

                AllowedAudiences = new[] { audience },
                IssuerSecurityTokenProviders = new[] 
                        {
                            new X509CertificateSecurityTokenProvider(
                                issuerName,
                                signingCertificate)
                        }
            });

            return app;
        }
    }
}
