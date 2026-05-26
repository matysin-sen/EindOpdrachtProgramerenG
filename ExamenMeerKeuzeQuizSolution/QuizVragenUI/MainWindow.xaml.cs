using MeerKeuzeBL.Interface;
using MeerKeuzeBL.Managers;
using MeerKeuzeDL.FileReader;
using MeerKeuzeDL.Repository;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using QuizUtil;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace QuizVragenUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ImportManager importManager;
        private Manager manager;
        public MainWindow()
        {
            InitializeComponent();

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
            string TxtAchter = "TXT_ACHTER";
            string TxtOnder = "TXT_ONDER";
            importManager = new ImportManager(new VragenRepository(connectionString), new FileReaderAntwoordOnder());
            manager = new Manager(new VragenRepository(connectionString));
           
            InitializeerDatabase();
        }

      

        private void btnLogIn_Click(object sender, RoutedEventArgs e)
        {
            string naam = txtBoxNaam.Text;
            
            string voornaam = naam.Split(' ')[0].Trim();   
            string achternaam = naam.Split(' ')[1].Trim();

            manager.VoegUserToe(voornaam, achternaam);
            KeuzeQuizMaker keuzeQuizMaker = new KeuzeQuizMaker(manager);
            keuzeQuizMaker.Show();
            this.Close();

        }

        private void InitializeerDatabase()
        {
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
            string TxtAchter = "TXT_ACHTER";
            string TxtOnder = "TXT_ONDER";


            var databaseConnection = new SqlConnection(connectionString);
            IVragenRepository vragenRepository = RepositoryFactory.CreateVragenRepository(databaseType, connectionString);

            IFileReader fileReader = FileReaderFactory.CreateFileReader(sourceFilePathc_1, TxtAchter, errorLogPath);
            IFileReader fileReaderGeo1 = FileReaderFactory.CreateFileReader(sourceFilePathGeo1, TxtOnder, errorLogPath);
            IFileReader fileReaderGeo2 = FileReaderFactory.CreateFileReader(sourceFilePathGeo2, TxtOnder, errorLogPath);
            IFileReader fileReaderMuziek1 = FileReaderFactory.CreateFileReader(sourceFilePathMuziek1, TxtOnder, errorLogPath);
            IFileReader fileReaderMuziek80s1 = FileReaderFactory.CreateFileReader(sourceFilePathMuziek80s1, TxtOnder, errorLogPath);
            IFileReader fileReaderMuziek80s2 = FileReaderFactory.CreateFileReader(sourceFilePathMuziek80s2, TxtOnder, errorLogPath);
            IFileReader fileReaderSQL_Beg = FileReaderFactory.CreateFileReader(sourceFilePathSQL_Beg, TxtOnder, errorLogPath);
            IFileReader fileReaderSQL_Beg2 = FileReaderFactory.CreateFileReader(sourceFilePathSQL_Beg2, TxtOnder, errorLogPath);
            IFileReader fileReaderSQL_Ex = FileReaderFactory.CreateFileReader(sourceFilePathSQL_Ex, TxtOnder, errorLogPath);

            ImportManager importManager = new ImportManager(vragenRepository, fileReader);
            ImportManager importManagerGeo1 = new ImportManager(vragenRepository, fileReaderGeo1);
            ImportManager importManagerGeo2 = new ImportManager(vragenRepository, fileReaderGeo2);
            ImportManager importManagerMuziek1 = new ImportManager(vragenRepository, fileReaderMuziek1);
            ImportManager importManagerMuziek80s1 = new ImportManager(vragenRepository, fileReaderMuziek80s1);
            ImportManager importManagerMuziek80s2 = new ImportManager(vragenRepository, fileReaderMuziek80s2);
            ImportManager importManagerSQL_Beg = new ImportManager(vragenRepository, fileReaderSQL_Beg);
            ImportManager importManagerSQL_Beg2 = new ImportManager(vragenRepository, fileReaderSQL_Beg2);
            ImportManager importManagerSQL_Ex = new ImportManager(vragenRepository, fileReaderSQL_Ex);
           // importManager.voegonderwerpToe("Aardrijkskunde");
           // importManager.voegonderwerpToe("Muziek");
            //importManager.voegonderwerpToe("SQL");
            //importManager.voegonderwerpToe("algemeen");
  

      

        }
    }
}