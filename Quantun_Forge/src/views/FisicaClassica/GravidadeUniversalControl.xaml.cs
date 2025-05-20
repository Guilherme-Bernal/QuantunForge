using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Quantun_Forge.src.views.FisicaClassica
{
    public partial class GravidadeUniversalControl : UserControl
    {
        private const double G = 6.674e-11;
        private bool desafioAtivo = false;
        private double ultimaForcaCalculada = 0;

        public GravidadeUniversalControl()
        {
            InitializeComponent();
        }

        private void BtnCalcular_Click(object sender, RoutedEventArgs e)
        {
            double m1 = SliderMassa1.Value;
            double m2 = SliderMassa2.Value;
            double r = SliderDistancia.Value;

            double F = G * (m1 * m2) / (r * r);
            ultimaForcaCalculada = F;

            // Mostra passo a passo da fórmula
            TxtFormula.Text =
                $"F = G × (m₁ × m₂) / r²\n" +
                $"F = 6,674×10⁻¹¹ × ({m1:E2} × {m2:E2}) / ({r:E2})²\n";

            // Resultado
            TxtResultado.Text = $"🌌 Força Gravitacional: {F:E2} N";

            // Explicação com base na relação
            if (m1 > 1e23 || m2 > 1e23)
                TxtExplicacao.Text = "Corpos com grande massa produzem uma força intensa.";
            else if (r > 1e8)
                TxtExplicacao.Text = "A distância elevada enfraquece consideravelmente a atração.";
            else
                TxtExplicacao.Text = "A força é moderada devido às massas e proximidade.";

            // Atualiza linha visual
            AtualizarLinha(F);

            // Se for modo desafio
            if (desafioAtivo)
            {
                MessageBox.Show($"Resposta do desafio: {F:E2} N", "Resultado");
                desafioAtivo = false;
            }
        }

        private void AtualizarLinha(double forca)
        {
            // Define a espessura da linha com base na força
            double grossura = Math.Clamp(forca / 1e20, 1, 10);
            LinhaGravidade.StrokeThickness = grossura;

            // Cor mais intensa para forças maiores
            if (forca > 1e20)
                LinhaGravidade.Stroke = Brushes.Red;
            else if (forca > 1e18)
                LinhaGravidade.Stroke = Brushes.Orange;
            else
                LinhaGravidade.Stroke = Brushes.LightGray;
        }

        private void BtnResetar_Click(object sender, RoutedEventArgs e)
        {
            SliderMassa1.Value = 5.97e24;
            SliderMassa2.Value = 7.35e22;
            SliderDistancia.Value = 3.84e8;

            LinhaGravidade.Stroke = Brushes.LightGray;
            LinhaGravidade.StrokeThickness = 2;

            TxtResultado.Text = "";
            TxtFormula.Text = "";
            TxtExplicacao.Text = "";
            desafioAtivo = false;
        }

        private void BtnTerraLua_Click(object sender, RoutedEventArgs e)
        {
            SliderMassa1.Value = 5.97e24;
            SliderMassa2.Value = 7.35e22;
            SliderDistancia.Value = 3.84e8;
        }

        private void BtnTerraMaca_Click(object sender, RoutedEventArgs e)
        {
            SliderMassa1.Value = 5.97e24;     // Terra
            SliderMassa2.Value = 0.15;        // Maçã ~150g
            SliderDistancia.Value = 1.0;      // Aproximadamente 1 metro
        }

        private void BtnDesafio_Click(object sender, RoutedEventArgs e)
        {
            desafioAtivo = true;
            TxtResultado.Text = "";
            TxtFormula.Text = "";
            TxtExplicacao.Text = "Desafio ativo: calcule mentalmente a força gravitacional antes de clicar em 'Calcular'.";
        }
    }
}
