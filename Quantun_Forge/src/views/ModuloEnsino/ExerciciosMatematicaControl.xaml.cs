using System.Windows.Controls;
using System.Windows.Input;
using Quantun_Forge.src.views.ModuloEnsino.Matematica;
using Quantun_Forge.src.views.ModuloEnsino.Matematica.Testes;

namespace Quantun_Forge.src.views.ModuloEnsino
{
    /// <summary>
    /// Interaction logic for ExerciciosMatematicaControl.xaml
    /// </summary>
    public partial class ExerciciosMatematicaControl : Page
    {
        public ExerciciosMatematicaControl()
        {
            InitializeComponent();
        }

        private void BinomioCard_Click(object sender, MouseButtonEventArgs e)
        {
            var win = new BNewtonWindow();
           
            win.Show();       // abre sem fechar a principal
        }

        private void AlgebraLinearCard_Click(object sender, MouseButtonEventArgs e)
        {
            var win = new AlgebraLinearWindow();

            win.Show();       // abre sem fechar a principal
        }

        private void NumerosComplexosCard_Click(object sender, MouseButtonEventArgs e)
        {
           var win = new NumerosComplexosWindow();
            win.Show();       // abre sem fechar a principal
        }

        private void ProbabilidadeCard_Click(object sender, MouseButtonEventArgs e)
        {
            var win = new TeoriaProbabilidadeWindow();
            win.Show();       // abre sem fechar a principal
        }

        private void EstatisticaCard_Click(object sender, MouseButtonEventArgs e)
        {
            var win = new EstatiscaWindow();
            win.Show();       // abre sem fechar a principal
        }

        private void CalculoCard_Click(object sender, MouseButtonEventArgs e)
        {
            var win = new Calculo();
            win.Show();       // abre sem fechar a principal
        }

        private void LogaritmoCard_Click(object sender, MouseButtonEventArgs e)
        {
            var win = new LogaritmoExponenciaisWindow();
            win.Show();       // abre sem fechar a principal
        }

        private void FuncoesCard_Click(object sender, MouseButtonEventArgs e)
        {
             var win = new  FuncaoWindow();

            win.Show();       // abre sem fechar a principal        
        }
    }
}