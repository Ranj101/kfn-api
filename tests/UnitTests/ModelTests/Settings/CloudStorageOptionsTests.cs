using AutoFixture;
using FluentAssertions;
using KfnApi.Models.Settings;
using UnitTests.Helpers;

namespace UnitTests.ModelTests.Settings;

public class CloudStorageOptionsTests
{
    private readonly Fixture _fixture = new();

    [Fact]
    public void CloudStorageOptionsSuccess()
    {
        var options = _fixture.Build<CloudStorageOptions>()
            .With(x => x.AuthUri, "https://www.googleapis.com")
            .With(x => x.TokenUri, "https://www.googleapis.com")
            .With(x => x.AuthProviderCertUrl, "https://www.googleapis.com")
            .With(x => x.ClientCertUrl, "https://www.googleapis.com")
            .Create();

        var result = ValidationHelper.ValidateObject(options);

        result.Should().BeTrue();
    }

    [Fact]
    public void TypeRequired()
    {
        var options = _fixture.Build<CloudStorageOptions>()
            .With(x => x.AuthUri, "https://www.googleapis.com")
            .With(x => x.TokenUri, "https://www.googleapis.com")
            .With(x => x.AuthProviderCertUrl, "https://www.googleapis.com")
            .With(x => x.ClientCertUrl, "https://www.googleapis.com")
            .Without(x => x.Type)
            .Create();

        var result = ValidationHelper.ValidateObject(options);

        result.Should().BeFalse();
    }

    [Fact]
    public void BucketRequired()
    {
        var options = _fixture.Build<CloudStorageOptions>()
            .With(x => x.AuthUri, "https://www.googleapis.com")
            .With(x => x.TokenUri, "https://www.googleapis.com")
            .With(x => x.AuthProviderCertUrl, "https://www.googleapis.com")
            .With(x => x.ClientCertUrl, "https://www.googleapis.com")
            .Without(x => x.Bucket)
            .Create();

        var result = ValidationHelper.ValidateObject(options);

        result.Should().BeFalse();
    }

    [Fact]
    public void ProjectIdRequired()
    {
        var options = _fixture.Build<CloudStorageOptions>()
            .With(x => x.AuthUri, "https://www.googleapis.com")
            .With(x => x.TokenUri, "https://www.googleapis.com")
            .With(x => x.AuthProviderCertUrl, "https://www.googleapis.com")
            .With(x => x.ClientCertUrl, "https://www.googleapis.com")
            .Without(x => x.ProjectId)
            .Create();

        var result = ValidationHelper.ValidateObject(options);

        result.Should().BeFalse();
    }

    [Fact]
    public void PrivateKeyIdRequired()
    {
        var options = _fixture.Build<CloudStorageOptions>()
            .With(x => x.AuthUri, "https://www.googleapis.com")
            .With(x => x.TokenUri, "https://www.googleapis.com")
            .With(x => x.AuthProviderCertUrl, "https://www.googleapis.com")
            .With(x => x.ClientCertUrl, "https://www.googleapis.com")
            .Without(x => x.PrivateKeyId)
            .Create();

        var result = ValidationHelper.ValidateObject(options);

        result.Should().BeFalse();
    }

    [Fact]
    public void PrivateKeyRequired()
    {
        var options = _fixture.Build<CloudStorageOptions>()
            .With(x => x.AuthUri, "https://www.googleapis.com")
            .With(x => x.TokenUri, "https://www.googleapis.com")
            .With(x => x.AuthProviderCertUrl, "https://www.googleapis.com")
            .With(x => x.ClientCertUrl, "https://www.googleapis.com")
            .Without(x => x.PrivateKey)
            .Create();

        var result = ValidationHelper.ValidateObject(options);

        result.Should().BeFalse();
    }

    [Fact]
    public void ClientEmailRequired()
    {
        var options = _fixture.Build<CloudStorageOptions>()
            .With(x => x.AuthUri, "https://www.googleapis.com")
            .With(x => x.TokenUri, "https://www.googleapis.com")
            .With(x => x.AuthProviderCertUrl, "https://www.googleapis.com")
            .With(x => x.ClientCertUrl, "https://www.googleapis.com")
            .Without(x => x.ClientEmail)
            .Create();

        var result = ValidationHelper.ValidateObject(options);

        result.Should().BeFalse();
    }

    [Fact]
    public void ClientIdRequired()
    {
        var options = _fixture.Build<CloudStorageOptions>()
            .With(x => x.AuthUri, "https://www.googleapis.com")
            .With(x => x.TokenUri, "https://www.googleapis.com")
            .With(x => x.AuthProviderCertUrl, "https://www.googleapis.com")
            .With(x => x.ClientCertUrl, "https://www.googleapis.com")
            .Without(x => x.ClientId)
            .Create();

        var result = ValidationHelper.ValidateObject(options);

        result.Should().BeFalse();
    }

    [Fact]
    public void AuthUriRequired()
    {
        var options = _fixture.Build<CloudStorageOptions>()
            .With(x => x.TokenUri, "https://www.googleapis.com")
            .With(x => x.AuthProviderCertUrl, "https://www.googleapis.com")
            .With(x => x.ClientCertUrl, "https://www.googleapis.com")
            .Without(x => x.AuthUri)
            .Create();

        var result = ValidationHelper.ValidateObject(options);

        result.Should().BeFalse();
    }

    [Fact]
    public void AuthUriMustBeUrl()
    {
        var options = _fixture.Build<CloudStorageOptions>()
            .With(x => x.AuthUri, "not-url")
            .With(x => x.TokenUri, "https://www.googleapis.com")
            .With(x => x.AuthProviderCertUrl, "https://www.googleapis.com")
            .With(x => x.ClientCertUrl, "https://www.googleapis.com")
            .Create();

        var result = ValidationHelper.ValidateObject(options);

        result.Should().BeFalse();
    }

    [Fact]
    public void TokenUriRequired()
    {
        var options = _fixture.Build<CloudStorageOptions>()
            .With(x => x.AuthUri, "https://www.googleapis.com")
            .With(x => x.AuthProviderCertUrl, "https://www.googleapis.com")
            .With(x => x.ClientCertUrl, "https://www.googleapis.com")
            .Without(x => x.TokenUri)
            .Create();

        var result = ValidationHelper.ValidateObject(options);

        result.Should().BeFalse();
    }

    [Fact]
    public void TokenUriMustBeUrl()
    {
        var options = _fixture.Build<CloudStorageOptions>()
            .With(x => x.AuthUri, "https://www.googleapis.com")
            .With(x => x.TokenUri, "not-url")
            .With(x => x.AuthProviderCertUrl, "https://www.googleapis.com")
            .With(x => x.ClientCertUrl, "https://www.googleapis.com")
            .Create();

        var result = ValidationHelper.ValidateObject(options);

        result.Should().BeFalse();
    }

    [Fact]
    public void AuthProviderCertUrlRequired()
    {
        var options = _fixture.Build<CloudStorageOptions>()
            .With(x => x.AuthUri, "https://www.googleapis.com")
            .With(x => x.TokenUri, "https://www.googleapis.com")
            .With(x => x.ClientCertUrl, "https://www.googleapis.com")
            .Without(x => x.AuthProviderCertUrl)
            .Create();

        var result = ValidationHelper.ValidateObject(options);

        result.Should().BeFalse();
    }

    [Fact]
    public void AuthProviderCertUrlMustBeUrl()
    {
        var options = _fixture.Build<CloudStorageOptions>()
            .With(x => x.AuthUri, "https://www.googleapis.com")
            .With(x => x.TokenUri, "https://www.googleapis.com")
            .With(x => x.AuthProviderCertUrl, "not-url")
            .With(x => x.ClientCertUrl, "https://www.googleapis.com")
            .Create();

        var result = ValidationHelper.ValidateObject(options);

        result.Should().BeFalse();
    }

    [Fact]
    public void ClientCertUrlRequired()
    {
        var options = _fixture.Build<CloudStorageOptions>()
            .With(x => x.AuthUri, "https://www.googleapis.com")
            .With(x => x.TokenUri, "https://www.googleapis.com")
            .With(x => x.AuthProviderCertUrl, "https://www.googleapis.com")
            .Without(x => x.ClientCertUrl)
            .Create();

        var result = ValidationHelper.ValidateObject(options);

        result.Should().BeFalse();
    }

    [Fact]
    public void ClientCertUrlMustBeUrl()
    {
        var options = _fixture.Build<CloudStorageOptions>()
            .With(x => x.AuthUri, "https://www.googleapis.com")
            .With(x => x.TokenUri, "https://www.googleapis.com")
            .With(x => x.AuthProviderCertUrl, "https://www.googleapis.com")
            .With(x => x.ClientCertUrl, "not-url")
            .Create();

        var result = ValidationHelper.ValidateObject(options);

        result.Should().BeFalse();
    }
}
