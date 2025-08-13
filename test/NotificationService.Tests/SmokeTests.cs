using Xunit;

public class SmokeTests
{
    [Fact]
    public void Simple_Pass()
    {
        Assert.True(1 + 1 == 2);
    }
}
