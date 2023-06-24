using AutoFixture;
using FluentAssertions;
using KfnApi.Models.Settings;
using UnitTests.Helpers;

namespace UnitTests.ModelTests.Settings;

public class RedisOptionsTests
{
    private readonly Fixture _fixture = new();

    [Fact]
    public void RedisOptionsSuccess()
    {
        var options = _fixture.Build<RedisOptions>()
            .With(x => x.ConnectionString, "Host=localhost;Port=5430")
            .Create();

        var result = ValidationHelper.ValidateObject(options);

        result.Should().BeTrue();
    }

    [Fact]
    public void ConnectionStringIsRequired()
    {
        var options = _fixture.Build<RedisOptions>()
            .Without(o => o.ConnectionString)
            .Create();

        var result = ValidationHelper.ValidateObject(options);

        result.Should().BeFalse();
    }
}
