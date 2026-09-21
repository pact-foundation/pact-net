using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using PactNet.Matchers;
using Xunit;

namespace PactNet.Tests.Matchers;

public class ArrayContainsMatcherTests
{
    private readonly IPactBuilderV4 pactBuilder;

    public ArrayContainsMatcherTests()
    {
        pactBuilder = Pact.V4("Some Consumer", "Some Producer")
            .WithHttpInteractions();
    }

    [Fact]
    public async Task ConsumerTests_Simple_StringList()
    {
        // Arrange
        this.pactBuilder
            .UponReceiving("A request for strings")
            .WithRequest(HttpMethod.Get, "/api/list/string")
            .WillRespond()
            .WithStatus(HttpStatusCode.OK)
            .WithJsonBody(Match.ArrayContains([
                "Thing 1",
                "Thing 2"
            ]));

        await this.pactBuilder.VerifyAsync(async ctx =>
        {
            using var service = Service(ctx.MockServerUri);

            // Act
            var strings = await service.GetStringList();

            // Assert
            strings.Should().Contain("Thing 1").And.Contain("Thing 2");
        });
    }

    [Fact]
    public async Task ConsumerTests_Simple_StringList_RegexMatch()
    {
        // Arrange
        this.pactBuilder
            .UponReceiving("A request for strings")
            .WithRequest(HttpMethod.Get, "/api/list/string")
            .WillRespond()
            .WithStatus(HttpStatusCode.OK)
            .WithJsonBody(Match.ArrayContains([
                Match.Regex("Thing 1", "Thing [0-9]+"),
                Match.Regex("Thing X", "Thing [A-Z]+"),
            ]));

        await this.pactBuilder.VerifyAsync(async ctx =>
        {
            using var service = Service(ctx.MockServerUri);

            // Act
            var strings = await service.GetStringList();

            // Assert
            strings.Should().Contain("Thing 1").And.Contain("Thing X");
        });
    }

    [Fact]
    public async Task ConsumerTests_Simple_ObjectList()
    {
        // Arrange
        this.pactBuilder
            .UponReceiving("A request for strings")
            .WithRequest(HttpMethod.Get, "/api/list/insert-or-update")
            .WillRespond()
            .WithStatus(HttpStatusCode.OK)
            .WithJsonBody(Match.ArrayContains([
                new InsertOrUpdate<ContentDto>
                {
                    Type = ActionEnum.Insert,
                    Content = new ContentDto
                    {
                        Name = "Name 1",
                    }
                },
                new InsertOrUpdate<ContentDto>
                {
                    Type = ActionEnum.Update,
                    Content = new ContentDto
                    {
                        Name = "Name 2",
                    }
                },
            ]));

        await this.pactBuilder.VerifyAsync(async ctx =>
        {
            using var service = Service(ctx.MockServerUri);

            // Act
            var strings = await service.GetInsertOrUpdateList();

            // Assert
            strings.Should()
                .Contain(new InsertOrUpdate<ContentDto>
                {
                    Type = ActionEnum.Insert,
                    Content = new ContentDto
                    {
                        Name = "Name 1",
                    }
                })
                .And
                .Contain(new InsertOrUpdate<ContentDto>
                {
                    Type = ActionEnum.Update,
                    Content = new ContentDto
                    {
                        Name = "Name 2",
                    }
                });
        });
    }

    [Fact]
    public async Task ConsumerTests_Simple_StringListList()
    {
        // Arrange
        this.pactBuilder
            .UponReceiving("A request for strings")
            .WithRequest(HttpMethod.Get, "/api/list/list/string")
            .WillRespond()
            .WithStatus(HttpStatusCode.OK)
            .WithJsonBody(Match.ArrayContains([
                new[] { "A", "B" },
                new[] { "C", "D" },
            ]));

        await this.pactBuilder.VerifyAsync(async ctx =>
        {
            using var service = Service(ctx.MockServerUri);

            // Act
            var strings = await service.GetStringListList();

            // Assert
            strings.Should()
                .Contain(xs => xs.Count == 2 && xs[0] == "A" && xs[1] == "B")
                .And
                .Contain(xs => xs.Count == 2 && xs[0] == "C" && xs[1] == "D");
        });
    }

    [Fact]
    public async Task ConsumerTests_Nested_StringListList()
    {
        // Arrange
        this.pactBuilder
            .UponReceiving("A request for strings")
            .WithRequest(HttpMethod.Get, "/api/list/list/string")
            .WillRespond()
            .WithStatus(HttpStatusCode.OK)
            .WithJsonBody(Match.ArrayContains([
                Match.ArrayContains(["A", "B"]),
                Match.ArrayContains(["C", "D"]),
            ]));

        await this.pactBuilder.VerifyAsync(async ctx =>
        {
            using var service = Service(ctx.MockServerUri);

            // Act
            var strings = await service.GetStringListList();

            // Assert
            strings.Should()
                .Contain(xs => xs.Count == 2 && xs[0] == "A" && xs[1] == "B")
                .And
                .Contain(xs => xs.Count == 2 && xs[0] == "C" && xs[1] == "D");
        });
    }

    private static ArrayIncludeTestService Service(Uri uri)
        => new(uri);

    private enum ActionEnum
    {
        Insert,
        Update,
    }

    private sealed record ContentDto
    {
        public string Name { get; set; }
    }

    private sealed record InsertOrUpdate<T> where T : IEquatable<T>
    {
        public ActionEnum Type { get; set; }
        public T Content { get; set; }
    }

    private sealed class ArrayIncludeTestService : IDisposable
    {
        private readonly Uri baseUri;
        private readonly HttpClient client;

        public ArrayIncludeTestService(Uri baseUri)
        {
            this.baseUri = baseUri;
            this.client = new HttpClient();
        }

        private string BaseUri => this.baseUri.ToString().TrimEnd('/');

        public Task<List<string>> GetStringList()
        {
            return Get<List<string>>("/api/list/string");
        }

        public Task<List<List<string>>> GetStringListList()
        {
            return Get<List<List<string>>>("/api/list/list/string");
        }

        public Task<List<InsertOrUpdate<ContentDto>>> GetInsertOrUpdateList()
        {
            return Get<List<InsertOrUpdate<ContentDto>>>("/api/list/insert-or-update");
        }

        private async Task<T> Get<T>(string url)
        {
            using var response = await this.client.GetAsync($"{BaseUri}{url}");
            response.EnsureSuccessStatusCode();

            using var stream = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<T>(stream);
        }

        public void Dispose()
        {
            this.client.Dispose();
        }
    }
}
