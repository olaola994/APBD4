using LegacyApp;

namespace LegacyAppTest;

public class UserServiceTests
{
    [Fact]
    public void ValidateData_Should_Return_False_When_Email_Without_At_And_Dot()
    {
        string email = "doe";
        var service = new UserService();

        bool result = service.ValidateEmail(email);
        
        Assert.Equal(false, result);
    }
    [Fact]
    public void Add_User_Should_Return_False_When_User_Younger_Than_21()
    {
        var firstName = "John";
        var lastName = "Doe";
        string email = "johndoe@gmail.com";
        DateTime dateOfBirth = new DateTime(2015, 12, 4);
        var now = DateTime.Now;
        int age = now.Year - dateOfBirth.Year;
        if (now.Month < dateOfBirth.Month || (now.Month == dateOfBirth.Month && now.Day < dateOfBirth.Day)) age--;
        var service = new UserService();
        int clientId = 100;
        var result = service.AddUser(firstName, lastName, email, dateOfBirth, clientId);
        Assert.Equal(false, result);
    }
}