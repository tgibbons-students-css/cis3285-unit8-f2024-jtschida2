using Microsoft.Data.SqlClient;
using System.Data;
using System.Reflection;
using System.Text;

namespace SingleResponsibilityPrinciple.Tests
{
    [TestClass()]
    public class TradeProcessorTests
    {
        private int CountDbRecords()
        {
            string azureConnectString = @"Server=tcp:cis3285-sql-server.database.windows.net,1433; Initial Catalog = Unit8_TradesDatabase; Persist Security Info=False; User ID=cis3285;Password=Saints4SQL; MultipleActiveResultSets = False; Encrypt = True; TrustServerCertificate = False; Connection Timeout = 60;";
            // Change the connection string used to match the one you want
            using (var connection = new SqlConnection(azureConnectString))
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
        [TestMethod]
        public void TestTradeWithTooFewValues()
        {
            var tradeStream = new MemoryStream(Encoding.UTF8.GetBytes("GBPUSD,1000"));
            var tradeProcessor = new TradeProcessor();

            int countBefore = CountDbRecords();
            tradeProcessor.ProcessTrades(tradeStream);
            int countAfter = CountDbRecords();

            Assert.AreEqual(countBefore, countAfter);  // No trade should be added to the database
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

        [TestMethod]
        public void TestSingleGoodTrade()
        {
            var tradeStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Unit8_SRP_F24Tests.goodtrades.txt");
            var tradeProcessor = new TradeProcessor();

            int countBefore = CountDbRecords();
            tradeProcessor.ProcessTrades(tradeStream);
            int countAfter = CountDbRecords();

            Assert.AreEqual(countBefore + 1, countAfter);
        }

        [TestMethod]
        public void TestMultipleGoodTrades()
        {
            var tradeStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Unit8_SRP_F24Tests.trades.txt");
            var tradeProcessor = new TradeProcessor();

            int countBefore = CountDbRecords();
            tradeProcessor.ProcessTrades(tradeStream);
            int countAfter = CountDbRecords();

            Assert.AreEqual(countBefore + 10, countAfter);
        }

        [TestMethod]
        public void TestNoTrades()
        {
            var tradeStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Unit8_SRP_F24Tests.empty.txt");
            var tradeProcessor = new TradeProcessor();

            int countBefore = CountDbRecords();
            tradeProcessor.ProcessTrades(tradeStream);
            int countAfter = CountDbRecords();

            Assert.AreEqual(countBefore, countAfter);
        }

        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void TestFileNotFound()
        {
            var tradeStream = File.OpenRead("non_existent_file.txt");
            var tradeProcessor = new TradeProcessor();

            tradeProcessor.ProcessTrades(tradeStream);
        }

        [TestMethod]
        public void TestReadValidTradeData()
        {
            var tradeStream = new MemoryStream(Encoding.UTF8.GetBytes("GBPUSD,1000,1.51\nEURUSD,2000,1.52"));
            var tradeProcessor = new TradeProcessor();

            var result = tradeProcessor.ReadTradeData(tradeStream);

            Assert.AreEqual(2, result.Count());
        }

        [TestMethod]
        public void TestReadEmptyFile()
        {
            var tradeStream = new MemoryStream(Encoding.UTF8.GetBytes(""));
            var tradeProcessor = new TradeProcessor();

            var result = tradeProcessor.ReadTradeData(tradeStream);

            Assert.AreEqual(0, result.Count());
        }

    }
}