namespace PactNet.Matchers.Equality
{
    public class EqualityMatchDefinition : MatchDefinition
    {
        public const string Name = "equality";

        public EqualityMatchDefinition(object example) :
            base(Name, example)
        {
        }
    }
}