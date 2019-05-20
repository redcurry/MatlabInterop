using BioDose;
using MathWorks.MATLAB.NET.Arrays;

namespace MatlabInterop
{
public class BioDoseService
{
    private static readonly BioDoseMatlab BioDoseMatlab = new BioDoseMatlab();

    public double[,] Calculate(double[,] dose, int fractions, double alphaBeta)
    {
        var doseArray = new MWNumericArray(dose);
        var nArray = new MWNumericArray(fractions);
        var alphaBetaArray = new MWNumericArray(alphaBeta);

        var result = BioDoseMatlab.biodose(doseArray, nArray, alphaBetaArray);
        return (double[,])result.ToArray();
    }
}
}