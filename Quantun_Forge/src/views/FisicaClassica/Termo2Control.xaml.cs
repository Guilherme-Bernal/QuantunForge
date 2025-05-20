using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Quantun_Forge.src.views.FisicaClassica
{
    public partial class Termo2Control : UserControl
    {
        public Termo2Control()
        {
            InitializeComponent();
            this.Loaded += Termo2Control_Loaded; // Corrigido para evitar NullReferenceException
        }

        private void Termo2Control_Loaded(object sender, RoutedEventArgs e)
        {
            AtualizarTudo(null, null); // Somente após os controles estarem carregados
        }

        private void AtualizarTudo(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (SliderTq == null || SliderTf == null || LblTq == null || LblTf == null || TxtEficiência == null)
                return;

            double Tq = SliderTq.Value;
            double Tf = SliderTf.Value;

            // Atualiza rótulos das temperaturas
            LblTq.Text = $"Tq = {Tq:F0} K";
            LblTf.Text = $"Tf = {Tf:F0} K";

            // Validação
            if (Tq <= 0 || Tf <= 0 || Tf >= Tq)
            {
                TxtEficiência.Text = "⚠️ Tq deve ser maior que Tf e ambos maiores que zero.";
                TxtInterpretacao.Text = "";
                AtualizarEntropia(0);
                return;
            }

            // Cálculo da eficiência de Carnot
            double eficiencia = 1 - (Tf / Tq);
            TxtEficiência.Text = $"Eficiência máxima (Carnot): η = 1 - Tf / Tq = 1 - {Tf:F0}/{Tq:F0} = {(eficiencia * 100):F1}%";

            // Interpretação
            if (eficiencia >= 0.7)
                TxtInterpretacao.Text = "🟢 Alta eficiência teórica — boa conversão de energia.";
            else if (eficiencia >= 0.4)
                TxtInterpretacao.Text = "🟡 Eficiência moderada — perdas consideráveis.";
            else
                TxtInterpretacao.Text = "🔴 Baixa eficiência — grande parte do calor se perde.";

            AtualizarEntropia(1 - eficiencia); // entropia proporcional à perda
        }

        private void AtualizarEntropia(double proporcao)
        {
            if (BarraEntropia == null)
                return;

            double alturaFinal = Math.Clamp(proporcao * 150, 10, 150);
            double topFinal = 250 - alturaFinal;

            var alturaAnim = new DoubleAnimation
            {
                To = alturaFinal,
                Duration = TimeSpan.FromMilliseconds(400),
                EasingFunction = new QuadraticEase()
            };

            var topAnim = new DoubleAnimation
            {
                To = topFinal,
                Duration = TimeSpan.FromMilliseconds(400),
                EasingFunction = new QuadraticEase()
            };

            BarraEntropia.BeginAnimation(HeightProperty, alturaAnim);
            BarraEntropia.BeginAnimation(Canvas.TopProperty, topAnim);
        }

        private void BtnResetar_Click(object sender, RoutedEventArgs e)
        {
            if (SliderTq == null || SliderTf == null) return;

            SliderTq.Value = 500;
            SliderTf.Value = 300;

            TxtEficiência.Text = "";
            TxtInterpretacao.Text = "";

            AtualizarEntropia(0);
        }
    }
}
