namespace LibA2;

public class AdvancedCalculator
{
    private readonly LibA.Calculator _calculator = new();

    public int Multiply(int a, int b)
    {
        var result = 0;
        for (var i = 0; i < b; i++)
        {
            result = _calculator.Add(result, a);
        }
        return result;
    }

    public int Square(int a) => Multiply(a, a);
}
