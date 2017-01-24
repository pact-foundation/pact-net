using PactNet.Matchers;

namespace PactNet.Mocks.MockHttpService.Matchers.Type
{
    public class TypeMatchDefinition : MatchDefinition
    {
        public const string Name = "type";

        public TypeMatchDefinition(object example) :
            base(Name, example)
        {
        }
    }
}