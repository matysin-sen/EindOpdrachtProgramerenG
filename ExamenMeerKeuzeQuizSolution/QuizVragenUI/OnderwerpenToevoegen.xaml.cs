using MeerKeuzeBL.Domein;
using MeerKeuzeBL.Interface;
using MeerKeuzeBL.Managers;
using Microsoft.Win32;
using QuizUtil;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for OnderwerpenToevoegen.xaml
    /// </summary>
    /// 
    public partial class OnderwerpenToevoegen : Window
    {
        string filepath;
        private ImportManager importManager;
        private Manager manager;
        public int PrimairKey { get;  set; }
        public string name { get; set; }
        public OnderwerpenToevoegen(ImportManager importManager , Manager manager)
        {
            InitializeComponent();
            this.importManager = importManager;
            this.manager = manager;
            cmbBoxOnderwerp.ItemsSource = manager.GeefAlleOnderwerpen();
        }

        private void btnFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.ShowDialog();
            filepath = ofd.FileName;
            filePathTxt.Text = filepath;    
              
        }

        private void cmbBoxOnderwerp_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void btnStartQuiz_Click(object sender, RoutedEventArgs e)
        {
            string fileType;
            if (cmbBoxOnderwerp.SelectedItem == null)
            {
                MessageBox.Show("Selecteer eerst een onderwerp.");
                return;
            }
            // 1. Haal het gekozen onderwerp op uit de ComboBox
            Onderwerpen gekozenOnderwerp = (Onderwerpen)cmbBoxOnderwerp.SelectedItem;
            if (rbtnTxtAchter.IsChecked == true) 
            {
                 fileType = "TXT_ACHTER";

            }
            else
            {
                fileType = "TXT_ONDER";
            }
            IFileReader Dejuistereader = FileReaderFactory.CreateFileReader(filepath, fileType, "");

            // 2. Importeer het bestand
            // OPMERKING: Je moet je ImporteerBestand methode in ImportManager.cs eventueel 
            // aanpassen zodat deze het gekozenOnderwerp als argument accepteert, 
            // in plaats van het zelf te proberen te 'bepalen' via de bestandsnaam.
            importManager.ImporteerBestand(filepath,Dejuistereader, gekozenOnderwerp);

            MessageBox.Show("Bestand succesvol geïmporteerd!");


        }

        private void btnOnderwerpToevoegen_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
