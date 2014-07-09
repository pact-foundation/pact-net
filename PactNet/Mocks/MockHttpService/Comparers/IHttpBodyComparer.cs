namespace PactNet.Mocks.MockHttpService.Comparers
{
    public interface IHttpBodyComparer
    {
        void Validate(dynamic body1, dynamic body2);
    }
}