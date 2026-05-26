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
        public KeuzeQuizMaker(Manager manager)
        {
            InitializeComponent();
            this.manager = manager;
        }

        private void btnNieuweVraag_Click(object sender, RoutedEventArgs e)
        {
            VraagToevoegen vraagToevoegen = new VraagToevoegen(manager);
            vraagToevoegen.Show();
        }

        private void btnQuizSpelen_Click(object sender, RoutedEventArgs e)
        {
            

            QuizOptiesxaml quizOptiesxaml = new QuizOptiesxaml(manager);
            quizOptiesxaml.Show();
        }

        private void btnScore_Click(object sender, RoutedEventArgs e)
        {
            ScoreBekijken scoreBekijken = new ScoreBekijken(manager);
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
    }
}
