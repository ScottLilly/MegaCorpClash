using MegaCorpClash.Core;

namespace Test.MegaCorpClash.Core;

public class TestExtensionMethods
{
    [Fact]
    public void Test_IsSafeText()
    {
        Assert.True("asd".IsSafeText());
        Assert.True("a s d".IsSafeText());
        Assert.True("a-s,d".IsSafeText());
        Assert.True("a,   s --- d".IsSafeText());
        Assert.True("a, 123  s --- d".IsSafeText());
        
        Assert.False("a!   s --- d".IsSafeText());
        Assert.False("a!".IsSafeText());
        Assert.False("{a!".IsSafeText());
        Assert.False("}a!".IsSafeText());
    }
}