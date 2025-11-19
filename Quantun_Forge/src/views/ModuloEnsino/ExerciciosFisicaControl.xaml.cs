using System.Windows.Controls;
using System.Windows.Input;
using Quantun_Forge.src.views.ModuloEnsino.Fisica.FisicaClassica;

namespace Quantun_Forge.src.views.ModuloEnsino
{
    public partial class ExerciciosFisicaControl : Page
    {
        public ExerciciosFisicaControl()
        {
            InitializeComponent();
        }

        // 1. Leis de Newton
        private void EnergiaCard_Click(object sender, MouseButtonEventArgs e)
        {
            var win = new LeisdeNewtonWindow();
            win.Show();
        }

        // 2. Ondas e Eletromagnetismo
        private void OndasCard_Click(object sender, MouseButtonEventArgs e)
        {
            var win = new OndasEletromagenetismoWindow();
            win.Show();
        }

        // 3. Movimento Periódico (Oscilador Harmônico)
        private void OsciladorCard_Click(object sender, MouseButtonEventArgs e)
        {
            var win = new MovimentoPeriodicoWindow();
            win.Show();
        }

        // 4. Momento Angular e Rotação
        private void MomentoAngularCard_Click(object sender, MouseButtonEventArgs e)
        {
            var win = new MomentoAngularWindow();
            win.Show();
        }

        // 5. Termodinâmica e Entropia
        private void TermodinamicaCard_Click(object sender, MouseButtonEventArgs e)
        {
            var win = new TermodinamicaWindow();
            win.Show();
        }

        // 6. Mecânica Lagrangiana e Hamiltoniana
        private void MecanicaAnaliticaCard_Click(object sender, MouseButtonEventArgs e)
        {
            var win = new MecanicaAnaliticaWindow();
            win.Show();
        }

        // 7. Teoria do Caos e Sistemas Dinâmicos
        private void TeoriaCaosCard_Click(object sender, MouseButtonEventArgs e)
        {
            var win = new CaosSistemasDinamicosWindow();
            win.Show();
        }

        // 8. Mecânica Estatística Clássica
        private void MecanicaEstatisticaCard_Click(object sender, MouseButtonEventArgs e)
        {
            var win = new MecanicaEstatisticaWindow();
            win.Show();
        }

        // 9. Relatividade Geral
        private void RelatividadeCard_Click(object sender, MouseButtonEventArgs e)
        {
            var win = new RelatividadeWindow();
            win.Show();
        }
    }
}