namespace PactNet.Mappers
{
    public interface IMapper<in TFrom, out TTo>
    {
        TTo Convert(TFrom from);
    }
}
