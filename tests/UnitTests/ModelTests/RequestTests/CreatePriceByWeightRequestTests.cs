using AutoFixture;
using FluentAssertions;
using KfnApi.DTOs.Requests;
using UnitTests.Helpers;

namespace UnitTests.ModelTests.RequestTests;

public class CreatePriceByWeightRequestTests
{
    private readonly Fixture _fixture = new();

    [Fact]
    public void CreatePriceByWeightRequestSuccess()
    {
        var options = _fixture.Create<CreatePriceByWeightRequest>();

        var result = ValidationHelper.ValidateObject(options);

        result.Should().BeTrue();
    }

    [Fact]
    public void ValueRequired()
    {
        var options = _fixture.Build<CreatePriceByWeightRequest>()
            .Without(x => x.Value)
            .Create();

        var result = ValidationHelper.ValidateObject(options);

        result.Should().BeFalse();
    }

    [Fact]
    public void ValueMustBeMoreThanOrEqualToOne()
    {
        var options = _fixture.Build<CreatePriceByWeightRequest>()
            .With(x => x.Value, -1)
            .Create();

        var result = ValidationHelper.ValidateObject(options);

        result.Should().BeFalse();
    }


    [Fact]
    public void WeightRequired()
    {
        var options = _fixture.Build<CreatePriceByWeightRequest>()
            .Without(x => x.Weight)
            .Create();

        var result = ValidationHelper.ValidateObject(options);

        result.Should().BeFalse();
    }

    [Fact]
    public void WeightMustBeMoreThanOrEqualToOne()
    {
        var options = _fixture.Build<CreatePriceByWeightRequest>()
            .With(x => x.Weight, -1)
            .Create();

        var result = ValidationHelper.ValidateObject(options);

        result.Should().BeFalse();
    }
}
