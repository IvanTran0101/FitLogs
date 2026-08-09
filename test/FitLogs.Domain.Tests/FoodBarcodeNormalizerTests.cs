using Shouldly;
using Xunit;

namespace FitLogs.Foods;

public class FoodBarcodeNormalizerTests
{
    [Theory]
    [InlineData(" 4006381333931 ", "4006381333931")]
    [InlineData("03600029145-2", "036000291452")]
    public void Supported_gtin_formats_are_normalized(string input, string expected)
    {
        FoodBarcodeNormalizer.TryNormalize(input, out var normalized).ShouldBeTrue();
        normalized.ShouldBe(expected);
    }

    [Theory]
    [InlineData("4006381333932")]
    [InlineData("12345")]
    [InlineData("ABC4006381333931")]
    public void Unsupported_or_invalid_barcodes_are_rejected(string input)
    {
        FoodBarcodeNormalizer.TryNormalize(input, out _).ShouldBeFalse();
    }
}
