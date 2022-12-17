using CSWBManagementApplication.Service;

namespace TestProject
{
    public class UnitTest3
    {
        [Theory]
        [InlineData("abc", false)]
        [InlineData(";.'.@", false)]
        [InlineData("abc@abc", false)]
        [InlineData("abc@-=.com", false)]
        [InlineData("abc@gmail.c",false)]
        [InlineData("abc@gmail.c0m", false)]
        [InlineData("abc@gmail.com", true)]
        public void Test1(string email, bool expect)
        {
            Assert.Equal(expect, MiscFunctions.IsEmailValid(email));
        }
    }
}
