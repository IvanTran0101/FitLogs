using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FitLogs.ExternalServices.OpenFoodFacts;
using FitLogs.Foods;
using Shouldly;
using Xunit;

namespace FitLogs;

public class OpenFoodFactsClientTests
{
    [Fact]
    public async Task Missing_nutrition_is_partial_instead_of_zero()
    {
        var handler = new StubHandler(_ => Json("""{"status":1,"product":{"product_name":"Partial","nutriments":{"proteins_100g":2}}}"""));
        var client = CreateClient(handler);

        var result = await client.GetByBarcodeAsync("4006381333931");

        result!.CaloriesPer100g.ShouldBeNull();
        result.DataQuality.ShouldBe(FoodProductDataQuality.Partial);
    }

    [Fact]
    public async Task Transient_failure_is_retried_twice_then_classified()
    {
        var handler = new StubHandler(_ => new HttpResponseMessage(HttpStatusCode.ServiceUnavailable));
        var client = CreateClient(handler);

        var exception = await Should.ThrowAsync<OpenFoodFactsUpstreamException>(
            () => client.GetByBarcodeAsync("4006381333931"));

        exception.Kind.ShouldBe(OpenFoodFactsFailureKind.Unavailable);
        handler.CallCount.ShouldBe(2);
    }

    [Fact]
    public async Task Malformed_product_is_rejected_as_invalid_data()
    {
        var handler = new StubHandler(_ => Json("""{"status":1,"product":{"brands":"No name"}}"""));
        var client = CreateClient(handler);

        var exception = await Should.ThrowAsync<OpenFoodFactsUpstreamException>(
            () => client.GetByBarcodeAsync("4006381333931"));

        exception.Kind.ShouldBe(OpenFoodFactsFailureKind.InvalidData);
    }

    private static OpenFoodFactsClient CreateClient(StubHandler handler)
    {
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://world.openfoodfacts.org/")
        };
        return new OpenFoodFactsClient(httpClient, new OpenFoodFactsCircuitBreaker());
    }

    private static HttpResponseMessage Json(string content) => new(HttpStatusCode.OK)
    {
        Content = new StringContent(content, Encoding.UTF8, "application/json")
    };

    private sealed class StubHandler(Func<HttpRequestMessage, HttpResponseMessage> responseFactory) : HttpMessageHandler
    {
        public int CallCount { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            CallCount++;
            return Task.FromResult(responseFactory(request));
        }
    }
}
