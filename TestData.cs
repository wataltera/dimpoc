namespace Dimpoc;

public class DemoData
{
    [TIN]
    public string SSN = "  123-45-6789<script>alert(1)</script>"; // valid SSN with attack characters

    [PAN]
    public string Card = "4111 1111 1111 1111"; // valid Visa test number (Luhn OK)

    [Tel]
    public string Phone = "(415) 555-2671 OR '1'='1'"; // US phone with attack suffix

    [USNUM]
    public string Number = " +12.34e-1; DROP TABLE users;";

    [PAN]
    public string CardInvalid = "1234-5678-9012-3456"; // invalid Luhn

    [TIN]
    public string TinNull = null!; // intentionally null to test null->empty handling
}
