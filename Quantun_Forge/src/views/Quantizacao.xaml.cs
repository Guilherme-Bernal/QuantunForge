using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Quantun_Forge.src.views
{
    /// <summary>
    /// Módulo interativo sobre Quantização da Energia
    /// Demonstra a transição do contínuo (clássico) para o discreto (quântico)
    /// </summary>
    public partial class Quantizacao : UserControl
    {
        public Quantizacao()
        {
            InitializeComponent();
            InicializarAnimacoes();

            // Mostra a visão contínua por padrão
            MostrarPainelContinuo();
        }

        /// <summary>
        /// Inicializa as animações de entrada
        /// </summary>
        private void InicializarAnimacoes()
        {
            Loaded += (sender, e) =>
            {
                var fadeIn = FindResource("FadeInAnimation") as Storyboard;
                fadeIn?.Begin(this);
            };
        }

        /// <summary>
        /// Mostra a visão de energia contínua (clássica)
        /// </summary>
        private void BtnMostrarContinuo_Click(object sender, RoutedEventArgs e)
        {
            MostrarPainelContinuo();
        }

        /// <summary>
        /// Mostra a visão de energia discreta (quântica)
        /// </summary>
        private void BtnMostrarDiscreto_Click(object sender, RoutedEventArgs e)
        {
            MostrarPainelDiscreto();
        }

        /// <summary>
        /// Mostra a transição entre os dois modelos
        /// </summary>
        private void BtnMostrarTransicao_Click(object sender, RoutedEventArgs e)
        {
            MostrarPainelTransicao();
        }

        /// <summary>
        /// Exibe o painel de visualização contínua
        /// </summary>
        private void MostrarPainelContinuo()
        {
            PainelContinuo.Visibility = Visibility.Visible;
            PainelDiscreto.Visibility = Visibility.Collapsed;
            PainelTransicao.Visibility = Visibility.Collapsed;

            AtualizarParticlaContinua(SliderContinuo.Value);
        }

        /// <summary>
        /// Exibe o painel de visualização discreta
        /// </summary>
        private void MostrarPainelDiscreto()
        {
            PainelContinuo.Visibility = Visibility.Collapsed;
            PainelDiscreto.Visibility = Visibility.Visible;
            PainelTransicao.Visibility = Visibility.Collapsed;

            AtualizarParticleDiscreta((int)SliderDiscreto.Value);
        }

        /// <summary>
        /// Exibe o painel de transição
        /// </summary>
        private void MostrarPainelTransicao()
        {
            PainelContinuo.Visibility = Visibility.Collapsed;
            PainelDiscreto.Visibility = Visibility.Collapsed;
            PainelTransicao.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Atualiza a posição da partícula no modelo contínuo
        /// </summary>
        private void SliderContinuo_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (TxtFreqContinuo != null && TxtEnergiaContinua != null && ParticleContinua != null)
            {
                double valor = e.NewValue;
                TxtFreqContinuo.Text = $"Frequência: {valor:F0}%";
                TxtEnergiaContinua.Text = $"E = {valor:F1} (valor contínuo)";

                AtualizarParticlaContinua(valor);
            }
        }

        /// <summary>
        /// Atualiza a posição da partícula na rampa contínua
        /// </summary>
        private void AtualizarParticlaContinua(double valor)
        {
            if (ParticleContinua == null) return;

            // Calcula posição na rampa (X aumenta, Y diminui)
            double x = 50 + (valor / 100.0) * 500;
            double y = 200 - (valor / 100.0) * 170;

            // Anima a transição suave
            AnimarParticula(ParticleContinua, x - 8, y - 8, 0.5);
        }

        /// <summary>
        /// Atualiza o nível quântico selecionado
        /// </summary>
        private void SliderDiscreto_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (TxtNivelQuantico != null && ParticleDiscreta != null)
            {
                int nivel = (int)e.NewValue;
                string[] niveis = { "E₀ = 0", "E₁ = h·f", "E₂ = 2h·f", "E₃ = 3h·f", "E₄ = 4h·f" };

                if (nivel < niveis.Length)
                {
                    TxtNivelQuantico.Text = $"Nível: n = {nivel} | Energia: {niveis[nivel]}";
                }

                AtualizarParticleDiscreta(nivel);
            }
        }

        /// <summary>
        /// Atualiza a posição da partícula nos níveis discretos
        /// </summary>
        private void AtualizarParticleDiscreta(int nivel)
        {
            if (ParticleDiscreta == null) return;

            // Posições discretas dos níveis (escada)
            double[] posX = { 100, 200, 300, 400, 500 };
            double[] posY = { 180, 140, 100, 60, 30 };

            if (nivel >= 0 && nivel < posX.Length)
            {
                // Anima o salto quântico
                AnimarParticula(ParticleDiscreta, posX[nivel] - 9, posY[nivel] - 9, 0.3);

                // Emite fóton quando muda de nível (exceto nível 0)
                if (nivel > 0)
                {
                    EmitirFoton();
                }
            }
        }

        /// <summary>
        /// Anima o movimento de uma partícula
        /// </summary>
        private void AnimarParticula(UIElement elemento, double novoX, double novoY, double duracao)
        {
            var storyboard = new Storyboard();

            // Animação X
            var animacaoX = new DoubleAnimation
            {
                To = novoX,
                Duration = TimeSpan.FromSeconds(duracao),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };

            Storyboard.SetTarget(animacaoX, elemento);
            Storyboard.SetTargetProperty(animacaoX, new PropertyPath("(Canvas.Left)"));
            storyboard.Children.Add(animacaoX);

            // Animação Y
            var animacaoY = new DoubleAnimation
            {
                To = novoY,
                Duration = TimeSpan.FromSeconds(duracao),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };

            Storyboard.SetTarget(animacaoY, elemento);
            Storyboard.SetTargetProperty(animacaoY, new PropertyPath("(Canvas.Top)"));
            storyboard.Children.Add(animacaoY);

            storyboard.Begin();
        }

        /// <summary>
        /// Anima a emissão de um fóton durante transição quântica
        /// </summary>
        private void EmitirFoton()
        {
            if (FotonEmitido == null) return;

            // Torna o fóton visível
            FotonEmitido.Visibility = Visibility.Visible;

            var storyboard = new Storyboard();

            // Animação de movimento do fóton
            var animacaoX = new DoubleAnimation
            {
                From = 110,
                To = 200,
                Duration = TimeSpan.FromSeconds(0.8),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };

            Storyboard.SetTarget(animacaoX, FotonEmitido);
            Storyboard.SetTargetProperty(animacaoX, new PropertyPath("(Canvas.Left)"));
            storyboard.Children.Add(animacaoX);

            // Animação de fade out
            var animacaoOpacidade = new DoubleAnimation
            {
                From = 1.0,
                To = 0.0,
                BeginTime = TimeSpan.FromSeconds(0.4),
                Duration = TimeSpan.FromSeconds(0.4)
            };

            Storyboard.SetTarget(animacaoOpacidade, FotonEmitido);
            Storyboard.SetTargetProperty(animacaoOpacidade, new PropertyPath("Opacity"));
            storyboard.Children.Add(animacaoOpacidade);

            // Esconde o fóton ao final
            storyboard.Completed += (s, e) =>
            {
                FotonEmitido.Visibility = Visibility.Collapsed;
                FotonEmitido.Opacity = 1.0;
            };

            storyboard.Begin();
        }
    }
}