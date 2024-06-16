using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Security.Cryptography;
using System.Text;

namespace VeritradeTests
{
    [TestClass]
    public class SecurityHotspotTest
    {
        [TestMethod]
        public void CheckReturnPassword()
        {
            var randomGenerator = RandomNumberGenerator.Create();
            byte[] data = new byte[6];
            randomGenerator.GetBytes(data);
            string password = BitConverter.ToString(data);

            password = password.Replace("-", "");

            long passInt = Convert.ToInt64(password, 16);

            password = passInt.ToString().Substring(0, 6);

            int passLength = password.Length;

            Assert.AreEqual(6, passLength);
        }
    }
}
