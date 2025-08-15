using OrderService.Infrastructure.NamingPolicy;

namespace OrderService.Infrastructure.UnitTests;

public class SnakeCaseNamingPolicyTests
{
    private readonly SnakeCaseNamingPolicy _policy = new();

    [Theory]
    [InlineData("MyProperty", "my_property")]
    [InlineData("IPAddress", "i_p_address")]
    [InlineData("URL", "u_r_l")]
    [InlineData("simple", "simple")]
    [InlineData("", "")]
    [InlineData(null, null)]
    [InlineData("Already_Snake", "already__snake")]
    public void ConvertName_ShouldConvertToSnakeCase(string input, string expected)
    {
        // Act
        var result = _policy.ConvertName(input);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void ConvertName_WithSingleChar_ShouldLowercase()
    {
        var result = _policy.ConvertName("A");
        Assert.Equal("a", result);
    }

    [Fact]
    public void ConvertName_WithAllLowercase_ShouldRemainSame()
    {
        var result = _policy.ConvertName("lowercase");
        Assert.Equal("lowercase", result);
    }

    [Fact]
    public void ConvertName_WithConsecutiveUppercase_ShouldAddUnderscoresBetween()
    {
        var result = _policy.ConvertName("TestABCName");
        Assert.Equal("test_a_b_c_name", result);
    }
}

