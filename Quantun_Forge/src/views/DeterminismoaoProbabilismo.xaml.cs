using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Animation;

namespace Quantun_Forge.src.views
{
    /// <summary>
    /// Módulo interativo sobre a transição do Determinismo ao Probabilismo
    /// Demonstra o experimento do Gato de Schrödinger e o colapso da função de onda
    /// </summary>
    public partial class DeterminismoaoProbabilismo : UserControl
    {
        private Random _random;
        private bool _estadoColapsado;

        public DeterminismoaoProbabilismo()
        {
            InitializeComponent();
            _random = new Random();
            _estadoColapsado = false;

            InicializarAnimacoes();
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
        /// Evento do slider de observação
        /// </summary>
        private void SliderObservacao_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (TxtEstadoSistema == null || CaixaFechada == null || CaixaAberta == null) return;

            double nivelObservacao = e.NewValue;

            if (nivelObservacao < 50)
            {
                // Sistema não observado - superposição
                MostrarEstadoSuperposicao();
            }
            else if (!_estadoColapsado)
            {
                // Observação iniciada - colapsar estado
                ColapsarEstadoQuantico();
            }
        }

        /// <summary>
        /// Mostra o estado de superposição (caixa fechada)
        /// </summary>
        private void MostrarEstadoSuperposicao()
        {
            if (TxtEstadoSistema == null || CaixaFechada == null || CaixaAberta == null) return;

            TxtEstadoSistema.Text = "Sistema Não Observado | Estado: Superposição Quântica";
            TxtEstadoSistema.Foreground = FindResource("QuantumColor") as System.Windows.Media.Brush;

            CaixaFechada.Visibility = Visibility.Visible;
            CaixaAberta.Visibility = Visibility.Collapsed;

            _estadoColapsado = false;
        }

        /// <summary>
        /// Colapsa o estado quântico e determina o resultado
        /// </summary>
        private void ColapsarEstadoQuantico()
        {
            if (TxtEstadoSistema == null || CaixaFechada == null || CaixaAberta == null) return;
            if (GatoVivo == null || GatoMorto == null) return;

            // Determina aleatoriamente o resultado (50% cada)
            bool gatoVivo = _random.Next(0, 2) == 0;

            // Anima a transição
            var fadeOut = new DoubleAnimation
            {
                From = 1.0,
                To = 0.0,
                Duration = TimeSpan.FromSeconds(0.4)
            };

            fadeOut.Completed += (s, e) =>
            {
                CaixaFechada.Visibility = Visibility.Collapsed;
                CaixaAberta.Visibility = Visibility.Visible;

                if (gatoVivo)
                {
                    GatoVivo.Visibility = Visibility.Visible;
                    GatoMorto.Visibility = Visibility.Collapsed;
                    TxtEstadoSistema.Text = "Sistema Observado | Estado Colapsado: GATO VIVO 😺";
                    TxtEstadoSistema.Foreground = FindResource("SuccessColor") as System.Windows.Media.Brush;
                }
                else
                {
                    GatoVivo.Visibility = Visibility.Collapsed;
                    GatoMorto.Visibility = Visibility.Visible;
                    TxtEstadoSistema.Text = "Sistema Observado | Estado Colapsado: GATO MORTO 😿";
                    TxtEstadoSistema.Foreground = FindResource("AccentColor") as System.Windows.Media.Brush;
                }

                // Anima o fade in do resultado
                var fadeIn = new DoubleAnimation
                {
                    From = 0.0,
                    To = 1.0,
                    Duration = TimeSpan.FromSeconds(0.5)
                };

                CaixaAberta.BeginAnimation(OpacityProperty, fadeIn);
            };

            CaixaFechada.BeginAnimation(OpacityProperty, fadeOut);
            _estadoColapsado = true;
        }

        /// <summary>
        /// Reinicia o experimento
        /// </summary>
        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            ResetarExperimento();
        }

        /// <summary>
        /// Reseta o experimento para o estado inicial
        /// </summary>
        private void ResetarExperimento()
        {
            if (SliderObservacao == null) return;

            // Anima o slider de volta para 0
            var animacao = new DoubleAnimation
            {
                To = 0,
                Duration = TimeSpan.FromSeconds(0.5),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };

            animacao.Completed += (s, e) =>
            {
                MostrarEstadoSuperposicao();
            };

            SliderObservacao.BeginAnimation(RangeBase.ValueProperty, animacao);
            _estadoColapsado = false;
        }
    }
}