namespace PactNet.Matchers
{
    internal enum MatcherCheckFailureType
    {
        AdditionalItemInArray,
        AdditionalPropertyInObject,
        NotEnoughValuesInArray,
        ValueDoesNotMatch,
        ValueNotEqual,
        ValueNotInteger,
        ValueNotDecimal,
        ValueDoesNotMatchDateFormat,
        ValueDoesNotMatchTimestamp
    }
}