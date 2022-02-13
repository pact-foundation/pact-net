using System;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;
using PactNet.Verifier.Messaging;
using Xunit;

namespace PactNet.Abstractions.Tests.Verifier.Messaging
{
    /// <summary>
    /// Defines the scenarios tests
    /// </summary>
    public class MessageScenarioContentBuilderTests
    {
        /// <summary>
        /// The builder under test
        /// </summary>
        private readonly MessageScenarioBuilder builder;

        public MessageScenarioContentBuilderTests()
        {
            this.builder = new MessageScenarioBuilder("a good description");
        }

        [Fact]
        public void WithMetadata_WhenCalled_SetsMetadata()
        {
            object expected = new { Foo = "Bar" };

            this.builder.WithMetadata(expected).WithContent(() => "foo");
            object actual = this.builder.Build().Metadata;

            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void WithMetadata_NullMetadata_ThrowsArgumentNullException()
        {
            this.builder
                .Invoking(b => b.WithMetadata(null))
                .Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void WithContent_WhenCalled_SetsContent()
        {
            object expected = new { Foo = 42 };

            this.builder.WithContent(() => expected);
            object actual = this.builder.Build().InvokeScenario();

            actual.Should().Be(expected);
        }

        [Fact]
        public void WithContent_WithCustomSettings_SetsSettings()
        {
            var expected = new JsonSerializerSettings();

            this.builder.WithContent(() => "foo", expected);
            var actual = this.builder.Build().JsonSettings;

            actual.Should().Be(expected);
        }

        [Fact]
        public async Task WithContentAsync_WhenCalled_SetsContent()
        {
            dynamic expected = new { Foo = 42 };

            await this.builder.WithContentAsync(() => Task.FromResult<dynamic>(expected));
            object actual = this.builder.Build().InvokeScenario();

            actual.Should().Be(expected);
        }

        [Fact]
        public async Task WithContentAsync_WithCustomSettings_SetsSettings()
        {
            var expected = new JsonSerializerSettings();

            await this.builder.WithContentAsync(() => Task.FromResult<dynamic>(new { Foo = "Bar" }), expected);
            var actual = this.builder.Build().JsonSettings;

            actual.Should().Be(expected);
        }

        [Fact]
        public void Build_ContentNotSet_ThrowsInvalidOperationException()
        {
            builder.Invoking(b => b.Build())
                   .Should().Throw<InvalidOperationException>();
        }
    }
}
