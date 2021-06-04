using FluentAssertions;
using PactNet.Matchers;
using Xunit;

namespace PactNet.Tests.Matchers
{
    public class MatchTests
    {
        [Fact]
        public void Regex_WhenCalled_ReturnsMatcher()
        {
            const string example = "hello@tester.com";
            const string regex = "/\\A([\\w+\\-].?)+@[a-z\\d\\-]+(\\.[a-z]+)*\\.[a-z]+\\z/i";

            var matcher = Match.Regex(example, regex);

            matcher.Should().BeEquivalentTo(new RegexMatcher(example, regex));
        }

        [Fact]
        public void Type_WhenCalled_ReturnsMatcher()
        {
            const int example = 22;

            var matcher = Match.Type(example);

            matcher.Should().BeEquivalentTo(new TypeMatcher(example));
        }

        [Fact]
        public void MinType_WhenCalled_ReturnseMatcher()
        {
            var example = new[] { 22, 23, 56 };

            var matcher = Match.MinType(example, 2);

            matcher.Should().BeEquivalentTo(new MinMaxTypeMatcher(example, min: 2));
        }

        [Fact]
        public void MaxType_WhenCalled_ReturnsMatcher()
        {
            var example = new[] { 22, 23, 56 };

            var matcher = Match.MaxType(example, 2);

            matcher.Should().BeEquivalentTo(new MinMaxTypeMatcher(example, max: 2));
        }

        [Fact]
        public void MinMaxType_WhenCalled_ReturnsMatcher()
        {
            var example = new[] { 22, 23, 56 };

            var matcher = Match.MinMaxType(example, 2, 3);

            matcher.Should().BeEquivalentTo(new MinMaxTypeMatcher(example, min: 2, max: 3));
        }

        [Fact]
        public void Integer_WhenCalled_ReturnsMatcher()
        {
            const int example = 42;

            var matcher = Match.Integer(example);

            matcher.Should().BeEquivalentTo(new IntegerMatcher(example));
        }

        [Fact]
        public void Decimal_Float_ReturnsMatcher()
        {
            const float example = 3.14f;

            var matcher = Match.Decimal(example);

            matcher.Should().BeEquivalentTo(new DecimalMatcher(example));
        }

        [Fact]
        public void Decimal_Double_ReturnsMatcher()
        {
            const double example = 3.14;

            var matcher = Match.Decimal(example);

            matcher.Should().BeEquivalentTo(new DecimalMatcher(example));
        }

        [Fact]
        public void Decimal_Decimal_ReturnsMatcher()
        {
            const decimal example = 3.14m;

            var matcher = Match.Decimal(example);

            matcher.Should().BeEquivalentTo(new DecimalMatcher(example));
        }

        [Fact]
        public void Number_Int_ReturnsMatcher()
        {
            const int example = 42;

            var matcher = Match.Number(example);

            matcher.Should().BeEquivalentTo(new NumericMatcher(example));
        }

        [Fact]
        public void Number_Float_ReturnsMatcher()
        {
            const float example = 3.14f;

            var matcher = Match.Number(example);

            matcher.Should().BeEquivalentTo(new NumericMatcher(example));
        }

        [Fact]
        public void Number_Double_ReturnsMatcher()
        {
            const double example = 3.14;

            var matcher = Match.Number(example);

            matcher.Should().BeEquivalentTo(new NumericMatcher(example));
        }

        [Fact]
        public void Number_Decimal_ReturnsMatcher()
        {
            const decimal example = 3.14m;

            var matcher = Match.Number(example);

            matcher.Should().BeEquivalentTo(new NumericMatcher(example));
        }

        [Fact]
        public void Null_WhenCalled_ReturnsMatcher()
        {
            var matcher = Match.Null();

            matcher.Should().BeEquivalentTo(new NullMatcher());
        }

        [Fact]
        public void Equality_WhenCalled_ReturnsMatcher()
        {
            const string example = "test";

            var matcher = Match.Equality(example);

            matcher.Should().BeEquivalentTo(new EqualityMatcher(example));
        }

        [Fact]
        public void Include_WhenCalled_ReturnsMatcher()
        {
            const string example = "test";

            var matcher = Match.Include(example);

            matcher.Should().BeEquivalentTo(new IncludeMatcher(example));
        }

        [Fact]
        public void ComposingTwoTypeMatchers_WhenCalled_ReturnsAllMatchers()
        {
            const int example = 22;

            var matcher = Match.Type(Match.Type(example));

            matcher.Should().BeEquivalentTo(Match.Type(Match.Type(example)));
        }

        [Fact]
        public void ComposingATypeMatcherAndARegexMatcher_WhenCalled_ReturnsAllMatchers()
        {
            const string example = "hello@tester.com";
            const string regex = "/\\A([\\w+\\-].?)+@[a-z\\d\\-]+(\\.[a-z]+)*\\.[a-z]+\\z/i";

            var matcher = Match.Type(Match.Regex(example, regex));

            matcher.Should().BeOfType<TypeMatcher>();

            Assert.IsType<TypeMatcher>(matcher);
            Assert.IsType<RegexMatcher>(matcher.Value);
        }

        [Fact]
        public void ComposingAMinTypeMatcherAndATypeMatcher_WhenCalled_ReturnsAllMatchers()
        {
            var example = new[] { Match.Type(22), Match.Type(23), Match.Type(56) };

            var matcher = Match.MinType(example, 2);

            Assert.IsType<MinMaxTypeMatcher>(matcher);
            var actualExample = matcher.Value;
            Assert.IsType<TypeMatcher>(actualExample[0]);
            Assert.IsType<TypeMatcher>(actualExample[1]);
            Assert.IsType<TypeMatcher>(actualExample[2]);
        }
    }
}