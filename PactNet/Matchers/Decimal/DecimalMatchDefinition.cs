namespace PactNet.Matchers.Decimal
{
    public class DecimalMatchDefinition : MatchDefinition
    {
        public const string Name = "decimal";

        public DecimalMatchDefinition(object example) :
            base(Name, example)
        {
        }
    }
}