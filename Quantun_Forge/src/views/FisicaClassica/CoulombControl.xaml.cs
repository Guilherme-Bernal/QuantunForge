using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Quantun_Forge.src.views.FisicaClassica
{
    public partial class CoulombControl : UserControl
    {
        const double k = 8.99e9; // Constante eletrostática de Coulomb

        public CoulombControl()
        {
            InitializeComponent();
            Loaded += CoulombControl_Loaded;
        }

        private void CoulombControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Aguarda todos os elementos visuais estarem disponíveis
            Dispatcher.BeginInvoke(new Action(() =>
            {
                AtualizarSimulacao(null, null);
            }), System.Windows.Threading.DispatcherPriority.Loaded);
        }

        private void AtualizarSimulacao(object? sender, RoutedPropertyChangedEventArgs<double>? e)
        {
            // Proteção contra controles não carregados
            if (SliderQ1 == null || SliderQ2 == null || SliderDist == null ||
                TxtForcaValor == null || LinhaForca == null || Carga1 == null || Carga2 == null || TxtInterpretacao == null)
                return;

            double q1 = SliderQ1.Value;
            double q2 = SliderQ2.Value;
            double r = SliderDist.Value;

            if (r <= 0.01) return;

            double forca = k * Math.Abs(q1 * q2) / (r * r);
            string tipo = (q1 * q2 < 0) ? "Atração" : "Repulsão";

            TxtForcaValor.Text = $"F = {forca:E2} N ({tipo})";

            // Interpretação didática
            if (forca < 1e3)
                TxtInterpretacao.Text = $"🟢 Força fraca ({tipo})";
            else if (forca < 1e6)
                TxtInterpretacao.Text = $"🟡 Força moderada ({tipo})";
            else
                TxtInterpretacao.Text = $"🔴 Força intensa ({tipo})";

            AtualizarVisual(q1, q2, tipo);
        }

        private void AtualizarVisual(double q1, double q2, string tipo)
        {
            if (Carga1 == null || Carga2 == null || LinhaForca == null) return;

            // Carga 1
            Carga1.Foreground = q1 >= 0 ? Brushes.Red : Brushes.Blue;
            Carga1.Text = q1 >= 0 ? "🔴 +q₁" : "🔵 -q₁";

            // Carga 2
            Carga2.Foreground = q2 >= 0 ? Brushes.Red : Brushes.Blue;
            Carga2.Text = q2 >= 0 ? "🔴 +q₂" : "🔵 -q₂";

            // Linha entre as cargas
            LinhaForca.Stroke = (tipo == "Atração") ? Brushes.Cyan : Brushes.Orange;
            LinhaForca.StrokeDashArray = tipo == "Atração" ? null : new DoubleCollection() { 5, 2 };
        }

        private void BtnResetar_Click(object sender, RoutedEventArgs e)
        {
            if (SliderQ1 == null || SliderQ2 == null || SliderDist == null) return;

            SliderQ1.Value = 5;
            SliderQ2.Value = -5;
            SliderDist.Value = 2;
        }
    }
}
