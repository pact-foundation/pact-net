namespace PactNet.Comparers
{
    public interface IComparer<in T>
    {
        ComparisonResult Compare(T expected, T actual);
    }
}
