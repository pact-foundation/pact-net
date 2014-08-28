namespace PactNet.Mocks.MockHttpService.Comparers
{
    public interface IHttpBodyComparer
    {
        void Validate(dynamic expected, dynamic actual, bool useStrict = false);
    }
}