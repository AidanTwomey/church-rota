namespace ChurchRota.Api.Utilities;

public class PhoneNumberSanitiser
{
    public string Sanitize(string phoneNumber)
    {
        // Remove any non-numeric characters
        return new string(phoneNumber.Where(char.IsDigit).ToArray());
    }
}
