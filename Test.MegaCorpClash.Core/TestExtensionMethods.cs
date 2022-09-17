using MegaCorpClash.Core;

namespace Test.MegaCorpClash.Core;

public class TestExtensionMethods
{
    [Fact]
    public void Test_IsSafeText()
    {
        Assert.True("asd".IsSafeText());
        Assert.True("asd!".IsSafeText());
        Assert.True("a s d".IsSafeText());
        Assert.True("a-s,d".IsSafeText());
        Assert.True("a,   s --- d".IsSafeText());
        Assert.True("a, 123  s --- d".IsSafeText());
        
        Assert.False("a$   s --- d".IsSafeText());
        Assert.False("a$".IsSafeText());
        Assert.False("{a$".IsSafeText());
        Assert.False("}a$".IsSafeText());
    }

    [Fact]
    public void Test_IsNotSafeText()
    {
        Assert.False("asd".IsNotSafeText());
        Assert.False("asd!".IsNotSafeText());
        Assert.False("a s d".IsNotSafeText());
        Assert.False("a-s,d".IsNotSafeText());
        Assert.False("a,   s --- d".IsNotSafeText());
        Assert.False("a, 123  s --- d".IsNotSafeText());

        Assert.True("a$   s --- d".IsNotSafeText());
        Assert.True("a$".IsNotSafeText());
        Assert.True("{a$".IsNotSafeText());
        Assert.True("}a$".IsNotSafeText());
    }
}