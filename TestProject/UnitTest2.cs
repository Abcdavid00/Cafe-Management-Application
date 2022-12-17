using CSWBManagementApplication.Service;

namespace TestProject
{
    public class UnitTest2
    {
        [Theory]
        [InlineData("abc", "abc", "abc", false)]
        [InlineData("-1", "abc", "abc", false)]
        [InlineData("0", "abc", "abc", false)]
        [InlineData("0", "-1", "abc", false)]
        [InlineData("0", "0", "abc", false)]
        [InlineData("0", "0", "-1", false)]
        [InlineData("0", "0", "0", true)]
        public void Test1(string sPrice, string mPrice, string lPrice, bool expect)
        {
            Assert.Equal(expect, MiscFunctions.IsProductPricesValid(sPrice, mPrice, lPrice));
        }
    }
}
