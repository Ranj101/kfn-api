using AutoFixture;
using FluentAssertions;
using KfnApi.Models.Settings;
using UnitTests.Helpers;

namespace UnitTests.ModelTests.Settings;

public class FirebaseOptionsTests
{
    private readonly Fixture _fixture = new();

    [Fact]
    public void FirebaseOptionsSuccess()
    {
        var options = _fixture.Build<FirebaseOptions>()
            .With(x => x.AuthUri, "https://www.googleapis.com")
            .With(x => x.TokenUri, "https://www.googleapis.com")
            .With(x => x.AuthProviderX509CertUrl, "https://www.googleapis.com")
            .With(x => x.ClientX509CertUrl, "https://www.googleapis.com")
            .Create();

        var result = ValidationHelper.ValidateObject(options);

        result.Should().BeTrue();
    }

    [Fact]
    public void TypeRequired()
    {
        var options = _fixture.Build<FirebaseOptions>()
            .With(x => x.AuthUri, "https://www.googleapis.com")
            .With(x => x.TokenUri, "https://www.googleapis.com")
            .With(x => x.AuthProviderX509CertUrl, "https://www.googleapis.com")
            .With(x => x.ClientX509CertUrl, "https://www.googleapis.com")
            .Without(x => x.Type)
            .Create();

        var result = ValidationHelper.ValidateObject(options);

        result.Should().BeFalse();
    }

    [Fact]
    public void ProjectIdRequired()
    {
        var options = _fixture.Build<FirebaseOptions>()
            .With(x => x.AuthUri, "https://www.googleapis.com")
            .With(x => x.TokenUri, "https://www.googleapis.com")
            .With(x => x.AuthProviderX509CertUrl, "https://www.googleapis.com")
            .With(x => x.ClientX509CertUrl, "https://www.googleapis.com")
            .Without(x => x.ProjectId)
            .Create();

        var result = ValidationHelper.ValidateObject(options);

        result.Should().BeFalse();
    }

    [Fact]
    public void PrivateKeyIdRequired()
    {
        var options = _fixture.Build<FirebaseOptions>()
            .With(x => x.AuthUri, "https://www.googleapis.com")
            .With(x => x.TokenUri, "https://www.googleapis.com")
            .With(x => x.AuthProviderX509CertUrl, "https://www.googleapis.com")
            .With(x => x.ClientX509CertUrl, "https://www.googleapis.com")
            .Without(x => x.PrivateKeyId)
            .Create();

        var result = ValidationHelper.ValidateObject(options);

        result.Should().BeFalse();
    }

    [Fact]
    public void PrivateKeyRequired()
    {
        var options = _fixture.Build<FirebaseOptions>()
            .With(x => x.AuthUri, "https://www.googleapis.com")
            .With(x => x.TokenUri, "https://www.googleapis.com")
            .With(x => x.AuthProviderX509CertUrl, "https://www.googleapis.com")
            .With(x => x.ClientX509CertUrl, "https://www.googleapis.com")
            .Without(x => x.PrivateKey)
            .Create();

        var result = ValidationHelper.ValidateObject(options);

        result.Should().BeFalse();
    }

    [Fact]
    public void ClientEmailRequired()
    {
        var options = _fixture.Build<FirebaseOptions>()
            .With(x => x.AuthUri, "https://www.googleapis.com")
            .With(x => x.TokenUri, "https://www.googleapis.com")
            .With(x => x.AuthProviderX509CertUrl, "https://www.googleapis.com")
            .With(x => x.ClientX509CertUrl, "https://www.googleapis.com")
            .Without(x => x.ClientEmail)
            .Create();

        var result = ValidationHelper.ValidateObject(options);

        result.Should().BeFalse();
    }

    [Fact]
    public void ClientIdRequired()
    {
        var options = _fixture.Build<FirebaseOptions>()
            .With(x => x.AuthUri, "https://www.googleapis.com")
            .With(x => x.TokenUri, "https://www.googleapis.com")
            .With(x => x.AuthProviderX509CertUrl, "https://www.googleapis.com")
            .With(x => x.ClientX509CertUrl, "https://www.googleapis.com")
            .Without(x => x.ClientId)
            .Create();

        var result = ValidationHelper.ValidateObject(options);

        result.Should().BeFalse();
    }

    [Fact]
    public void AuthUriRequired()
    {
        var options = _fixture.Build<FirebaseOptions>()
            .With(x => x.TokenUri, "https://www.googleapis.com")
            .With(x => x.AuthProviderX509CertUrl, "https://www.googleapis.com")
            .With(x => x.ClientX509CertUrl, "https://www.googleapis.com")
            .Without(x => x.AuthUri)
            .Create();

        var result = ValidationHelper.ValidateObject(options);

        result.Should().BeFalse();
    }

    [Fact]
    public void AuthUriMustBeUrl()
    {
        var options = _fixture.Build<FirebaseOptions>()
            .With(x => x.AuthUri, "not-url")
            .With(x => x.TokenUri, "https://www.googleapis.com")
            .With(x => x.AuthProviderX509CertUrl, "https://www.googleapis.com")
            .With(x => x.ClientX509CertUrl, "https://www.googleapis.com")
            .Create();

        var result = ValidationHelper.ValidateObject(options);

        result.Should().BeFalse();
    }

    [Fact]
    public void TokenUriRequired()
    {
        var options = _fixture.Build<FirebaseOptions>()
            .With(x => x.AuthUri, "https://www.googleapis.com")
            .With(x => x.AuthProviderX509CertUrl, "https://www.googleapis.com")
            .With(x => x.ClientX509CertUrl, "https://www.googleapis.com")
            .Without(x => x.TokenUri)
            .Create();

        var result = ValidationHelper.ValidateObject(options);

        result.Should().BeFalse();
    }

    [Fact]
    public void TokenUriMustBeUrl()
    {
        var options = _fixture.Build<FirebaseOptions>()
            .With(x => x.AuthUri, "https://www.googleapis.com")
            .With(x => x.TokenUri, "not-url")
            .With(x => x.AuthProviderX509CertUrl, "https://www.googleapis.com")
            .With(x => x.ClientX509CertUrl, "https://www.googleapis.com")
            .Create();

        var result = ValidationHelper.ValidateObject(options);

        result.Should().BeFalse();
    }

    [Fact]
    public void AuthProviderX509CertUrlRequired()
    {
        var options = _fixture.Build<FirebaseOptions>()
            .With(x => x.AuthUri, "https://www.googleapis.com")
            .With(x => x.TokenUri, "https://www.googleapis.com")
            .With(x => x.ClientX509CertUrl, "https://www.googleapis.com")
            .Without(x => x.AuthProviderX509CertUrl)
            .Create();

        var result = ValidationHelper.ValidateObject(options);

        result.Should().BeFalse();
    }

    [Fact]
    public void AuthProviderX509CertUrlMustBeUrl()
    {
        var options = _fixture.Build<FirebaseOptions>()
            .With(x => x.AuthUri, "https://www.googleapis.com")
            .With(x => x.TokenUri, "https://www.googleapis.com")
            .With(x => x.AuthProviderX509CertUrl, "not-url")
            .With(x => x.ClientX509CertUrl, "https://www.googleapis.com")
            .Create();

        var result = ValidationHelper.ValidateObject(options);

        result.Should().BeFalse();
    }

    [Fact]
    public void ClientX509CertUrlRequired()
    {
        var options = _fixture.Build<FirebaseOptions>()
            .With(x => x.AuthUri, "https://www.googleapis.com")
            .With(x => x.TokenUri, "https://www.googleapis.com")
            .With(x => x.AuthProviderX509CertUrl, "https://www.googleapis.com")
            .Without(x => x.ClientX509CertUrl)
            .Create();

        var result = ValidationHelper.ValidateObject(options);

        result.Should().BeFalse();
    }

    [Fact]
    public void ClientX509CertUrlMustBeUrl()
    {
        var options = _fixture.Build<FirebaseOptions>()
            .With(x => x.AuthUri, "https://www.googleapis.com")
            .With(x => x.TokenUri, "https://www.googleapis.com")
            .With(x => x.AuthProviderX509CertUrl, "https://www.googleapis.com")
            .With(x => x.ClientX509CertUrl, "not-url")
            .Create();

        var result = ValidationHelper.ValidateObject(options);

        result.Should().BeFalse();
    }

    [Fact]
    public void UniverseDomainRequired()
    {
        var options = _fixture.Build<FirebaseOptions>()
            .With(x => x.AuthUri, "https://www.googleapis.com")
            .With(x => x.TokenUri, "https://www.googleapis.com")
            .With(x => x.AuthProviderX509CertUrl, "https://www.googleapis.com")
            .With(x => x.ClientX509CertUrl, "https://www.googleapis.com")
            .Without(x => x.UniverseDomain)
            .Create();

        var result = ValidationHelper.ValidateObject(options);

        result.Should().BeFalse();
    }
}
