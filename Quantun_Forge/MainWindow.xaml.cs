using System.Windows;
using System.Windows.Controls;
using Quantun_Forge.src.views;
using Quantun_Forge.src.views.ComputacaoQuantica;

namespace Quantun_Forge
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnLogicaBooleana_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new LogicaBooleanaControl();
        }

        private void BtnVonNeumann_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new VonNeumannControl();
        }
        private void BtnPortasLogicas_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new src.views.PortasLogicasControl(); // Você criará este UserControl
        }

        private void BtnSistemaBinario_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new src.views.SistemaBinarioControl();
        }

        private void BtnAlu_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new src.views.AluControl();
        }

        private void BtnCicloInstrucao_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new src.views.CicloInstrucaoControl();
        }
        private void Btn_LeiNewton1_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new src.views.FisicaClassica.LeiNewton1Control();
        }

        private void Btn_LeiNewton2_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new src.views.FisicaClassica.LeiNewton2Control();
        }

        private void Btn_LeiNewton3_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new src.views.FisicaClassica.LeiNewton3Control();
        }

        private void Btn_Gravitacao_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new src.views.FisicaClassica.GravidadeUniversalControl();
        }

        private void Btn_Termo1_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new src.views.FisicaClassica.Termo1Control();
        }

        private void Btn_Termo2_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new src.views.FisicaClassica.Termo2Control();
        }

        private void Btn_Termo3_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new src.views.FisicaClassica.Termo3Control();
        }

        private void Btn_Coulomb_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new src.views.FisicaClassica.CoulombControl();
        }

        private void Btn_Maxwell_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new src.views.FisicaClassica.MaxwellControl();
        }

        private void Btn_Pascal_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new src.views.FisicaClassica.PascalControl();
        }

        private void Btn_Arquimedes_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new src.views.FisicaClassica.ArquimedesControl();
        }

        private void Btn_Torricelli_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new src.views.FisicaClassica.TorricelliControl();
        }
        private void BtnSuperposicao_Click(object sender, RoutedEventArgs e)
        {
            
            MainFrame.Content = new src.views.ComputacaoQuantica.SuperposicaoControl();

        }
        private void BtnEmaranhamento_Click(object sender, RoutedEventArgs e)
        {

            MainFrame.Content = new src.views.ComputacaoQuantica.EmaranhamentoControl();

        }
       


    }
}
