namespace PactNet.Remote.Mappers
{
    internal interface IMapper<in TFrom, out TTo>
    {
        TTo Convert(TFrom from);
    }
}