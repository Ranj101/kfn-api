using AutoFixture;
using FluentAssertions;
using KfnApi.DTOs.Requests;
using UnitTests.Helpers;

namespace UnitTests.ModelTests.RequestTests;

public class SubmitReportRequestTests
{
    private readonly Fixture _fixture = new();

    [Fact]
    public void SubmitReportRequestSuccess()
    {
        var options = _fixture.Create<SubmitReportRequest>();

        var result = ValidationHelper.ValidateObject(options);

        result.Should().BeTrue();
    }

    [Fact]
    public void TitleRequired()
    {
        var options = _fixture
            .Build<SubmitReportRequest>()
            .Without(x => x.Title)
            .Create();

        var result = ValidationHelper.ValidateObject(options);

        result.Should().BeFalse();
    }

    [Fact]
    public void TitleShouldBeLessThan351()
    {
        var options = _fixture.Build<SubmitReportRequest>()
            .With(x => x.Title, "a".PadLeft(351, 'a'))
            .Create();

        var result = ValidationHelper.ValidateObject(options);

        result.Should().BeFalse();
    }

    [Fact]
    public void SummaryRequired()
    {
        var options = _fixture
            .Build<SubmitReportRequest>()
            .Without(x => x.Summary)
            .Create();

        var result = ValidationHelper.ValidateObject(options);

        result.Should().BeFalse();
    }
}
