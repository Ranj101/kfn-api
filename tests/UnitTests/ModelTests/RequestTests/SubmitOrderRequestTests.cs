using AutoFixture;
using FluentAssertions;
using KfnApi.DTOs.Requests;
using UnitTests.Helpers;

namespace UnitTests.ModelTests.RequestTests;

public class SubmitOrderRequestTests
{
    private readonly Fixture _fixture = new();

    [Fact]
    public void SubmitFormRequestSuccess()
    {
        var options = _fixture.Create<SubmitOrderRequest>();

        var result = ValidationHelper.ValidateObject(options);

        result.Should().BeTrue();
    }

    [Fact]
    public void ProducerIdRequired()
    {
        var options = _fixture
            .Build<SubmitOrderRequest>()
            .Without(x => x.ProducerId)
            .Create();

        var result = ValidationHelper.ValidateObject(options);

        result.Should().BeFalse();
    }

    [Fact]
    public void LocationRequired()
    {
        var options = _fixture
            .Build<SubmitOrderRequest>()
            .Without(x => x.Location)
            .Create();

        var result = ValidationHelper.ValidateObject(options);

        result.Should().BeFalse();
    }

    [Fact]
    public void PickupTimeRequired()
    {
        var options = _fixture
            .Build<SubmitOrderRequest>()
            .Without(x => x.PickupTime)
            .Create();

        var result = ValidationHelper.ValidateObject(options);

        result.Should().BeFalse();
    }

    [Fact]
    public void ProductsRequired()
    {
        var options = _fixture
            .Build<SubmitOrderRequest>()
            .Without(x => x.Products)
            .Create();

        var result = ValidationHelper.ValidateObject(options);

        result.Should().BeFalse();
    }
}
