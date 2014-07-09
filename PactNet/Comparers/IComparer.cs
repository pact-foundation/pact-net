namespace PactNet.Comparers
{
    public interface IComparer<in T>
    {
        void Compare(T item1, T item2);
    }
}
