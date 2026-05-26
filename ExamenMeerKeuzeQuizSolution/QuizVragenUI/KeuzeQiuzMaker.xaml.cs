using MeerKeuzeBL.Domein;
using MeerKeuzeBL.Interface;
using MeerKeuzeBL.Managers;
using MeerKeuzeDL.Repository;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace QuizVragenUI
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class KeuzeQuizMaker : Window
    {
        private Manager manager;
        private ImportManager importManager;
        private int _userID;
        public KeuzeQuizMaker(Manager manager, int userID)
        {
            InitializeComponent();
            this.manager = manager;
            this._userID = userID;
        }

        private void btnNieuweVraag_Click(object sender, RoutedEventArgs e)
        {
            VraagToevoegen vraagToevoegen = new VraagToevoegen(manager);
            vraagToevoegen.Show();
        }

        private void btnQuizSpelen_Click(object sender, RoutedEventArgs e)
        {
            

            QuizOptiesxaml quizOptiesxaml = new QuizOptiesxaml(manager, _userID);
            quizOptiesxaml.Show();
        }

        private void btnScore_Click(object sender, RoutedEventArgs e)
        {
            ScoreBekijken scoreBekijken = new ScoreBekijken(manager, _userID);
            scoreBekijken.Show();
        }

        private void btnBestandenImporteren_Click(object sender, RoutedEventArgs e)
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
             .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            var config = builder.Build();
            string connectionString = config.GetConnectionString("SQLServerConnection");

            IVragenRepository vragenRepository = new VragenRepository(connectionString);
            importManager = new ImportManager(vragenRepository, null);
            manager = new Manager(vragenRepository);

            OnderwerpenToevoegen onderwerpenToevoegen = new OnderwerpenToevoegen(importManager, manager);
            onderwerpenToevoegen.Show();
        }

        private void btnOnderwerpToevoegen_Click(object sender, RoutedEventArgs e)
        {
            ExtraOnderwerpenToevoegen extraOnderwerpenToevoegen = new ExtraOnderwerpenToevoegen(importManager, manager);
            extraOnderwerpenToevoegen.Show();
        }
    }
}
