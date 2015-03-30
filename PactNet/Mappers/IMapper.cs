namespace PactNet.Mappers
{
    internal interface IMapper<in TFrom, out TTo>
    {
        TTo Convert(TFrom from);
    }
}
