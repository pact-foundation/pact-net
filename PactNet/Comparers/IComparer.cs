namespace PactNet.Comparers
{
    internal interface IComparer<in T>
    {
        ComparisonResult Compare(T expected, T actual);
    }
}
