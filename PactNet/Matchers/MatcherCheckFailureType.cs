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
        ValueDoesNotMatchValidDate,
        ValueDoesNotMatchTimestamp,
        ValueDoesNotExist
    }
}