using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Quantun_Forge.src.views.ModuloEnsino.Computacao.ComputacaoClassica;
using Quantun_Forge.src.views.ModuloEnsino.Matematica;

namespace Quantun_Forge.src.views.ModuloEnsino
{
    public partial class ExerciciosClassicaControl : Page
    {
        public ExerciciosClassicaControl()
        {
            InitializeComponent();
        }

        private void BitsCard_Click(object sender, MouseButtonEventArgs e)
        {
            var win = new bitsistemabinariosWindow();

            win.Show();      
        }

        private void PortasLogicasCard_Click(object sender, MouseButtonEventArgs e)
        {
            var win = new PLABWindow();

            win.Show();
        }

        private void VonNeumannCard_Click(object sender, MouseButtonEventArgs e)
        {
            var win = new VonNeumann();

            win.Show();
        }

        private void TuringCard_Click(object sender, MouseButtonEventArgs e)
        {
            var win = new Turing();

            win.Show();
        }

        private void AlgoritmosCard_Click(object sender, MouseButtonEventArgs e)
        {
            var win = new ALES();
            win.Show();
        }

        private void SistemaOperacionalCard_Click(object sender, MouseButtonEventArgs e)
        {
            var win = new SistemaOperacional();
            win.Show();

        }
    }
}