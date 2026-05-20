using MeerKeuzeBL.Domein;
using MeerKeuzeDL.FileReader;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace ExamenMeerKeuzeQuiz
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            var builder = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            var config = builder.Build();
            string connectionString = config.GetConnectionString("SQLServerConnection");
            string sourceFilePathc_1 = config.GetSection("FileSettings")["sourceFilePathC_1"];
            string sourceFilePathGeo1 = config.GetSection("FileSettings")["sourceFilePathGeo1"];
            string sourceFilePathGeo2 = config.GetSection("FileSettings")["sourceFilePathGeo2"];
            string sourceFilePathMuziek1 = config.GetSection("FileSettings")["sourceFilePathMuziek1"];
            string sourceFilePathMuziek80s1 = config.GetSection("FileSettings")["sourceFilePathMuziek80s1"];
            string sourceFilePathMuziek80s2 = config.GetSection("FileSettings")["sourceFilePathMuziek80s2"];
            string sourceFilePathSQL_Beg = config.GetSection("FileSettings")["sourceFilePathSQL_Beg"];
            string sourceFilePathSQL_Beg2 = config.GetSection("FileSettings")["sourceFilePathSQL_Beg2"];
            string sourceFilePathSQL_Ex = config.GetSection("FileSettings")["sourceFilePathSQL_Ex"];
            string errorLogPath = config.GetSection("FileSettings")["errorLogPath"];
            string sourceFileType = config.GetSection("FileSettings")["sourceFileType"];
            string databaseType = config.GetSection("FileSettings")["databaseType"];


            //var loginconnection = new SqlConnection(loginconnectionString);
        }
        
    }
    
}

