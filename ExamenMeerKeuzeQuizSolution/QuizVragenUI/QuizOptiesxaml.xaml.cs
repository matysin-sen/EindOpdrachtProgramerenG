using MeerKeuzeBL.Domein;
using MeerKeuzeBL.Interface;
using MeerKeuzeBL.Managers;
using MeerKeuzeDL.Repository;
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
    /// Interaction logic for QuizOptiesxaml.xaml
    /// </summary>
    public partial class QuizOptiesxaml : Window
    {
        public Manager manager;
        private IVragenRepository repository;
        
        public QuizOptiesxaml(Manager manager)
        {
            InitializeComponent();
            this.manager = manager;
            cmbBoxOnderwerp.ItemsSource = manager.GeefAlleOnderwerpen();
        }

        private void btnStartQuiz_Click(object sender, RoutedEventArgs e)
        {
            string onderwerp = cmbBoxOnderwerp.SelectedItem.ToString();
           int aantalvragen = AantalVragen.Value.HasValue ? (int)AantalVragen.Value.Value : 0;
            if (string.IsNullOrEmpty(onderwerp))
            {
                MessageBox.Show("Selecteer een onderwerp.");
                return;
            }
            if (aantalvragen <= 0)
            {
                MessageBox.Show("Voer een geldig aantal vragen in.");
                return;
            }
            QuizSpelen quizSpelen = new QuizSpelen(manager, onderwerp, aantalvragen);
            quizSpelen.Show();
        }

        private void cmbBoxOnderwerp_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }
    }
}
