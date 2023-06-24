using AutoFixture;
using FluentAssertions;
using KfnApi.DTOs.Requests;
using UnitTests.Helpers;

namespace UnitTests.ModelTests.RequestTests;

public class CreateProductRequestTests
{
    private readonly Fixture _fixture = new();

    [Fact]
    public void CreateProductRequestSuccess()
    {
        var options = _fixture.Create<CreateProductRequest>();

        var result = ValidationHelper.ValidateObject(options);

        result.Should().BeTrue();
    }

    [Fact]
    public void NameRequired()
    {
        var options = _fixture.Build<CreateProductRequest>()
            .Without(x => x.Name)
            .Create();

        var result = ValidationHelper.ValidateObject(options);

        result.Should().BeFalse();
    }

    [Fact]
    public void NameShouldBeLessThan351()
    {
        var options = _fixture.Build<CreateProductRequest>()
            .With(x => x.Name, "a".PadLeft(351, 'a'))
            .Create();

        var result = ValidationHelper.ValidateObject(options);

        result.Should().BeFalse();
    }

    [Fact]
    public void PricesRequired()
    {
        var options = _fixture.Build<CreateProductRequest>()
            .Without(x => x.Prices)
            .Create();

        var result = ValidationHelper.ValidateObject(options);

        result.Should().BeFalse();
    }

    [Fact]
    public void PricesShouldBeLessThanEleven()
    {
        var options = _fixture.Build<CreateProductRequest>()
            .With(x => x.Prices, _fixture.CreateMany<CreatePriceByWeightRequest>(11).ToList())
            .Create();

        var result = ValidationHelper.ValidateObject(options);

        result.Should().BeFalse();
    }
}
