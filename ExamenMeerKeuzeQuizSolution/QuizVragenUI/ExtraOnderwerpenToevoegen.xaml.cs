using MeerKeuzeBL.Managers;
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
    /// Interaction logic for ExtraOnderwerpenToevoegen.xaml
    /// </summary>
    public partial class ExtraOnderwerpenToevoegen : Window
    {
        private Manager _manager;
        private ImportManager _importManager;
        public ExtraOnderwerpenToevoegen(ImportManager importManager, Manager manager)
        {
            InitializeComponent();
            _importManager = importManager;
            _manager = manager;
        }
        // todo:chekken voor dubbele onderwerpen, als er al een onderwerp is met dezelfde naam, dan mag je die niet toevoegen
        private void btnOpslaan_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtOnderwerpNaam.Text))
            {
                MessageBox.Show("Vul een onderwerpnaam in!");
                return;
            }
            try
            {
                _manager.voegOnderwerpToe(txtOnderwerpNaam.Text.Trim());
                MessageBox.Show($"Onderwerp '{txtOnderwerpNaam.Text.Trim()}' succesvol toegevoegd!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fout bij het toevoegen van onderwerp:\n{ex.Message}");
            }



            txtOnderwerpNaam.Clear();
        }
    }
}
