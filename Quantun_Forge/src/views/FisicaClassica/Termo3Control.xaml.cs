using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Quantun_Forge.src.views.FisicaClassica
{
    public partial class Termo3Control : UserControl
    {
        private List<TextBlock> particulas;

        public Termo3Control()
        {
            InitializeComponent();
            Loaded += Termo3Control_Loaded;
        }

        private void Termo3Control_Loaded(object sender, RoutedEventArgs e)
        {
            // Captura as partículas uma vez após a interface estar carregada
            particulas = GridParticulas.Children.OfType<TextBlock>().ToList();

            // Aguarda a renderização completa antes de atualizar os controles
            Dispatcher.BeginInvoke(new Action(() =>
            {
                AtualizarVisual(SliderTemp.Value);
            }), System.Windows.Threading.DispatcherPriority.Loaded);
        }

        private void SliderTemp_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            AtualizarVisual(e.NewValue);
        }

        private void BtnResetar_Click(object sender, RoutedEventArgs e)
        {
            SliderTemp.Value = 300;
        }

        private void AtualizarVisual(double temperatura)
        {
            // Verificações de segurança
            if (TxtTemp == null || TxtInterpretacao == null || BarraEntropia == null)
                return;

            TxtTemp.Text = $"Temperatura: {temperatura:F0} K";

            double proporcao = temperatura / 300.0;
            AtualizarBarraEntropia(proporcao);
            AtualizarParticulas(proporcao);

            // Texto interpretativo baseado na temperatura
            if (temperatura <= 20)
                TxtInterpretacao.Text = "🧊 Sistema extremamente ordenado. Entropia quase nula.";
            else if (temperatura <= 100)
                TxtInterpretacao.Text = "🟦 Baixa temperatura: partículas organizadas com pouca entropia.";
            else if (temperatura <= 200)
                TxtInterpretacao.Text = "🟨 Temperatura moderada: entropia intermediária.";
            else
                TxtInterpretacao.Text = "🔴 Alta temperatura: sistema desordenado e entropia elevada.";
        }

        private void AtualizarBarraEntropia(double proporcao)
        {
            if (BarraEntropia == null) return;

            double altura = Math.Clamp(proporcao * 150, 5, 150);
            var anim = new DoubleAnimation
            {
                To = altura,
                Duration = TimeSpan.FromMilliseconds(400),
                EasingFunction = new QuadraticEase()
            };

            BarraEntropia.BeginAnimation(HeightProperty, anim);
        }

        private void AtualizarParticulas(double proporcao)
        {
            if (particulas == null) return;

            // Por padrão, todas como ❄️
            var emojis = Enumerable.Repeat("❄️", particulas.Count).ToList();

            // Se temperatura alta, embaralha com partículas agitadas
            if (proporcao > 0.7)
            {
                var rnd = new Random();
                for (int i = 0; i < emojis.Count; i++)
                {
                    int r = rnd.Next(3);
                    emojis[i] = r switch
                    {
                        0 => "💨",
                        1 => "💧",
                        _ => "❄️"
                    };
                }

                // Embaralha a ordem visual
                emojis = emojis.OrderBy(x => Guid.NewGuid()).ToList();
            }

            // Aplica visual aos TextBlocks
            for (int i = 0; i < particulas.Count; i++)
            {
                particulas[i].Text = emojis[i];
            }
        }
    }
}
