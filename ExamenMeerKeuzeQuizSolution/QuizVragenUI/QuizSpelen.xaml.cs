using MeerKeuzeBL.Domein;
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
    /// Interaction logic for QuizSpelen.xaml
    /// </summary>
    public partial class QuizSpelen : Window
    {
        public Manager manager;
        public Onderwerpen onderwerp;
        public int aantalvragen;
        public QuizSpelen(Manager manager, Onderwerpen onderwerp, int aantalvragen)
        {
            InitializeComponent();
            this.manager = manager;
            this.onderwerp = onderwerp;
            this.aantalvragen = aantalvragen;
        }
    }
}
