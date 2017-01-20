namespace PactNet.Matchers.Integer
{
    public class IntegerMatchDefinition : MatchDefinition
    {
        public const string Name = "integer";

        public IntegerMatchDefinition(object example) :
            base(Name, example)
        {
        }
    }
}