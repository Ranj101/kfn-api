using AutoFixture;
using FluentAssertions;
using KfnApi.Models.Settings;
using UnitTests.Helpers;

namespace UnitTests.ModelTests.Settings;

public class DatabaseOptionsTests
{
    private readonly Fixture _fixture = new();

    [Fact]
    public void DatabaseOptionsSuccess()
    {
        var options = _fixture.Build<PostgresOptions>()
            .With(x => x.ConnectionString, "Host=localhost;Port=5430")
            .Create();

        var result = ValidationHelper.ValidateObject(options);

        result.Should().BeTrue();
    }

    [Fact]
    public void ConnectionStringIsRequired()
    {
        var options = _fixture.Build<PostgresOptions>()
            .Without(o => o.ConnectionString)
            .Create();

        var result = ValidationHelper.ValidateObject(options);

        result.Should().BeFalse();
    }
}
