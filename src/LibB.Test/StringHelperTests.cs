using NUnit.Framework;

namespace LibB.Test;

[TestFixture]
public class StringHelperTests
{
    private StringHelper _helper = null!;

    [SetUp]
    public void SetUp()
    {
        _helper = new StringHelper();
    }

    [Test]
    public void Reverse_ReturnsReversedString()
    {
        Assert.That(_helper.Reverse("hello"), Is.EqualTo("olleh"));
    }

    [Test]
    public void IsPalindrome_ReturnsTrueForPalindrome()
    {
        Assert.That(_helper.IsPalindrome("racecar"), Is.True);
    }

    [Test]
    public void IsPalindrome_ReturnsFalseForNonPalindrome()
    {
        Assert.That(_helper.IsPalindrome("hello"), Is.False);
    }
}
