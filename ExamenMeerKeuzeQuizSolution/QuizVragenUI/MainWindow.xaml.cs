using MeerKeuzeBL.Managers;
using MeerKeuzeDL.FileReader;
using MeerKeuzeDL.Repository;
using Microsoft.Extensions.Configuration;
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
    }
}