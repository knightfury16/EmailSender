namespace EmailSenderTests;

public class SmtpEmailSenderTests
{
    //So this class is not testable because SmtpClient is a concreate
    //class. It does not implement an interface as such I can not Moq
    //SmtpClient. For that I would need a concreate SMTP class.
    //Another way of testing this class would be Wrapping SmtpClient with
    //my own interface. That would require the entire code design to change.
    //if testing is a priority then during the design face we need to take this
    //fact into consideration
}
