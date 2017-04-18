using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer3.AccessTokenValidation;
using Moq;
using Owin;
using Xunit;

namespace AccessTokenValidation.Tests
{
    public class TestIdentityServerBearerTokenValidationMiddleware
    {
        [Fact]
        public void Construct_GivenNullLoggerFactory_ShouldNotThrow_NPE()
        {
            // Arrange
            var options = new IdentityServerOAuthBearerAuthenticationOptions();
            Func<IDictionary<string, object>, Task> appFunc =
                d => Task.FromResult(0);
            var appBuilder = Mock.Of<IAppBuilder>();

            // Act
            var sut = new IdentityServerBearerTokenValidationMiddleware(
                appFunc,
                appBuilder,
                options,
                null
             );
            
            // Assert
            var fieldInfo = sut.GetType().GetField("_logger", BindingFlags.NonPublic | BindingFlags.Instance);
            fieldInfo.Should().NotBe(null, $"Expected to find private field _logger on {nameof(IdentityServerBearerTokenValidationMiddleware)}");
            var fieldValue = fieldInfo.GetValue(sut);
            fieldValue.Should().NotBe(null, "Expected _logger field to have been set during construction");
            fieldValue.Should().BeOfType<TraceLogger>();
        }
    }
}
