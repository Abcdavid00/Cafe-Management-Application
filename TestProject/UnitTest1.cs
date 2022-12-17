using CSWBManagementApplication.Service;

namespace TestProject
{
    public class UnitTest1
    {
        [Theory]
        [InlineData("abc", false)]
        [InlineData("abcde12345", false)]
        [InlineData("0123456789", false)]
        [InlineData("0373256789", true)]
        public void Test1(string phoneNumber, bool expect)
        {
            Assert.Equal(expect, MiscFunctions.IsPhoneNumberValid(phoneNumber));
        }
    }
}