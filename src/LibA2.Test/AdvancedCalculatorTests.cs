using NUnit.Framework;

namespace LibA2.Test;

[TestFixture]
public class AdvancedCalculatorTests
{
    private AdvancedCalculator _calculator = null!;

    [SetUp]
    public void SetUp()
    {
        _calculator = new AdvancedCalculator();
    }

    [Test]
    public void Multiply_ReturnsCorrectProduct()
    {
        Assert.That(_calculator.Multiply(3, 4), Is.EqualTo(12));
    }

    [Test]
    public void Square_ReturnsCorrectSquare()
    {
        Assert.That(_calculator.Square(5), Is.EqualTo(25));
    }
}
