using ChurchRota.Api.Utilities;
using Shouldly;

namespace church_rota.unit.tests;

public class PhoneSanitiserTests
{
    [Theory]
    [InlineData("07700 900123", "07700900123")]
    [InlineData("(07700) 900123", "07700900123")]
    [InlineData("07700-900123", "07700900123")]
    public void WhenPhoneNumberIsSanitized_ThenOnlyDigitsAreReturned(string input, string expected)
    {
        // Arrange
        var sanitiser = new PhoneNumberSanitiser();

        // Act
        sanitiser.Sanitize(input).ShouldBe(expected);
    }
}
