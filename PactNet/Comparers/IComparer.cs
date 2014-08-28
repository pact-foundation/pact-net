namespace PactNet.Comparers
{
    public interface IComparer<in T>
    {
        void Compare(T expected, T actual);
    }
}
