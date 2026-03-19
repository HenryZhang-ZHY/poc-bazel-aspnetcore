using NUnit.Framework;

namespace LibA.Test;

[TestFixture]
public class CalculatorTests
{
    private Calculator _calculator = null!;

    [SetUp]
    public void SetUp()
    {
        _calculator = new Calculator();
    }

    [Test]
    public void Add_ReturnsCorrectSum()
    {
        Assert.That(_calculator.Add(2, 3), Is.EqualTo(5));
    }

    [Test]
    public void Subtract_ReturnsCorrectDifference()
    {
        Assert.That(_calculator.Subtract(5, 3), Is.EqualTo(2));
    }
}
