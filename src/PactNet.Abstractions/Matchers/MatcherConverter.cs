using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PactNet.Matchers
{
    /// <summary>
    /// Converter for <see cref="IMatcher"/>
    /// </summary>
    internal class MatcherConverter : JsonConverter<IMatcher>
    {
        /// <summary>Reads and converts the JSON to type <see cref="IMatcher"/>.</summary>
        /// <param name="reader">The reader.</param>
        /// <param name="typeToConvert">The type to convert.</param>
        /// <param name="options">An object that specifies serialization options to use.</param>
        /// <returns>The converted value.</returns>
        public override IMatcher Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => throw new NotSupportedException("Matchers can only be written, not read");

        /// <summary>Writes a specified value as JSON.</summary>
        /// <param name="writer">The writer to write to.</param>
        /// <param name="value">The value to convert to JSON.</param>
        /// <param name="options">An object that specifies serialization options to use.</param>
        public override void Write(Utf8JsonWriter writer, IMatcher value, JsonSerializerOptions options)
        {
            switch (value)
            {
                case DecimalMatcher matcher:
                    JsonSerializer.Serialize(writer, matcher, options);
                    break;
                case EqualityMatcher matcher:
                    JsonSerializer.Serialize(writer, matcher, options);
                    break;
                case IncludeMatcher matcher:
                    JsonSerializer.Serialize(writer, matcher, options);
                    break;
                case IntegerMatcher matcher:
                    JsonSerializer.Serialize(writer, matcher, options);
                    break;
                case MinMaxTypeMatcher matcher:
                    JsonSerializer.Serialize(writer, matcher, options);
                    break;
                case NullMatcher matcher:
                    JsonSerializer.Serialize(writer, matcher, options);
                    break;
                case NumericMatcher matcher:
                    JsonSerializer.Serialize(writer, matcher, options);
                    break;
                case RegexMatcher matcher:
                    JsonSerializer.Serialize(writer, matcher, options);
                    break;
                case TypeMatcher matcher:
                    JsonSerializer.Serialize(writer, matcher, options);
                    break;
                case ArrayContainsMatcher matcher:
                    JsonSerializer.Serialize(writer, matcher, options);
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Unsupported matcher: {value.GetType()}");
            }
        }
    }
}
