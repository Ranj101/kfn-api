using AutoFixture;
using FluentAssertions;
using KfnApi.DTOs.Requests;
using UnitTests.Helpers;

namespace UnitTests.ModelTests.RequestTests;

public class SubmitFormRequestTests
{
    private readonly Fixture _fixture = new();

    [Fact]
    public void SubmitFormRequestSuccess()
    {
        var options = _fixture.Create<SubmitFormRequest>();

        var result = ValidationHelper.ValidateObject(options);

        result.Should().BeTrue();
    }

    [Fact]
    public void ProducerNameRequired()
    {
        var options = _fixture
            .Build<SubmitFormRequest>()
            .Without(x => x.ProducerName)
            .Create();

        var result = ValidationHelper.ValidateObject(options);

        result.Should().BeFalse();
    }

    [Fact]
    public void ProducerNameShouldBeLessThan351()
    {
        var options = _fixture
            .Build<SubmitFormRequest>()
            .With(x => x.ProducerName, "a".PadLeft(351, 'a'))
            .Create();

        var result = ValidationHelper.ValidateObject(options);

        result.Should().BeFalse();
    }

    [Fact]
    public void LocationsRequired()
    {
        var options = _fixture
            .Build<SubmitFormRequest>()
            .Without(x => x.Locations)
            .Create();

        var result = ValidationHelper.ValidateObject(options);

        result.Should().BeFalse();
    }

    [Fact]
    public void OpeningTimeRequired()
    {
        var options = _fixture
            .Build<SubmitFormRequest>()
            .Without(x => x.OpeningTime)
            .Create();

        var result = ValidationHelper.ValidateObject(options);

        result.Should().BeFalse();
    }

    [Fact]
    public void ClosingTimeRequired()
    {
        var options = _fixture
            .Build<SubmitFormRequest>()
            .Without(x => x.ClosingTime)
            .Create();

        var result = ValidationHelper.ValidateObject(options);

        result.Should().BeFalse();
    }

    [Fact]
    public void UploadsRequired()
    {
        var options = _fixture
            .Build<SubmitFormRequest>()
            .Without(x => x.Uploads)
            .Create();

        var result = ValidationHelper.ValidateObject(options);

        result.Should().BeFalse();
    }

    [Fact]
    public void UploadsShouldBeLessThanFour()
    {
        var options = _fixture
            .Build<SubmitFormRequest>()
            .With(x => x.Uploads, _fixture.CreateMany<Guid>(4).ToList())
            .Create();

        var result = ValidationHelper.ValidateObject(options);

        result.Should().BeFalse();
    }
}
