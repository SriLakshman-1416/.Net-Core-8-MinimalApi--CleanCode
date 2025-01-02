using AutoFixture;
using Coding_Clean_Safe_REST_API_V_8_MinimalApi.Endpoints;
using Coding_Clean_Safe_REST_API_V_8_MinimalApi.Models;
using Domain.Dtos;
using Domain.MappingInterface;
using ExpectedObjects;
using Microsoft.AspNetCore.Http.HttpResults;
using NSubstitute;

namespace CountryUnitTest.Countries;

public class GetCountriesTests
{
    private readonly ICountryMapper _countryMapper;
    private readonly ICountryService_V2 _countryService;
    private readonly Fixture _fixture;


    public GetCountriesTests()
    {
        _countryMapper = Substitute.For<ICountryMapper>();
        _countryService = Substitute.For<ICountryService_V2>();
        _fixture = new Fixture();
    }


    [Fact]
    public async Task WhenGetCountriesReceivesNullPagingParametersAndGetAllAsyncMethodReturnsCountries_ShouldFillUpDefaultPagingParametersAndReturnCountries()
    {
        // Arrange
        int? pageIndex = null;
        int? pageSize = null;
        var expectedPaging = new PagingDto
        {
            PageIndex = 1,
            PageSize = 10
        }.ToExpectedObject();


        var countries = _fixture.CreateMany<CountryDto>(2).ToList();
        var expectedCountries = countries.ToExpectedObject();


        var mappedCountries = _fixture.CreateMany<Country>(2).ToList();
        var expectedMappedCountries = mappedCountries.ToExpectedObject();


        _countryService.GetAllAsync().Returns(x => countries);
        _countryMapper.MapListOfDtoToModel(Arg.Any<List<CountryDto>>()).Returns(x => mappedCountries);


        // Act
        var result = (await CountryCurdOperationEndPoints.GetCountries(_countryMapper, _countryService)) as Ok<List<Country>>;


        // Assert
        expectedMappedCountries.ShouldEqual(result?.Value);
        await _countryService.Received(1).GetAllAsync();
        _countryMapper.Received(1).MapListOfDtoToModel(Arg.Is<List<CountryDto>>(x => expectedCountries.Matches(x)));
    }

    private class PagingDto
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}
