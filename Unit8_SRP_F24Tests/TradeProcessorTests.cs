using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SingleResponsibilityPrinciple;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SingleResponsibilityPrinciple.Tests
{
    [TestClass()]
    public class TradeProcessorTests
    {
        [TestMethod()]
        public void ProcessTradesTest()
        {
            Assert.Fail();
        }

        private int CountDbRecords()
        {
            string azureConnectString = @"Server=tcp:cis3285-sql-server.database.windows.net,1433; Initial Catalog = Unit8_TradesDatabase; Persist Security Info=False; User ID=cis3285;Password=Saints4SQL; MultipleActiveResultSets = False; Encrypt = True; TrustServerCertificate = False; Connection Timeout = 60;";
            // Change the connection string used to match the one you want
            using (var connection = new Microsoft.Data.SqlClient.SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Normies\OneDrive\Documents\tradedatabase.mdf;Integrated Security=True;Connect Timeout=30"))
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                string myScalarQuery = "SELECT COUNT(*) FROM trade";
                SqlCommand myCommand = new SqlCommand(myScalarQuery, connection);
                //myCommand.Connection.Open();
                int count = (int)myCommand.ExecuteScalar();
                connection.Close();
                return count;
            }
        }
        [TestMethod()]
        public void TestNormalFile()
        {
            //Arrange
            var tradeStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Unit8_SRP_F24Tests.goodtrades.txt");
            var tradeProcessor = new TradeProcessor();

            //Act
            int countBefore = CountDbRecords();
            tradeProcessor.ProcessTrades(tradeStream);
            //Assert
            int countAfter = CountDbRecords();
            Assert.AreEqual(countBefore + 4, countAfter);
        }
    }
}