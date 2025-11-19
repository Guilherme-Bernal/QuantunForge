using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Quantun_Forge.src.views
{
    /// <summary>
    /// Módulo interativo sobre Dualidade Onda-Partícula
    /// Permite comparação visual entre comportamento clássico e quântico
    /// </summary>
    public partial class DualidadeOndaFisicaCQ : UserControl
    {
        public DualidadeOndaFisicaCQ()
        {
            InitializeComponent();
            InicializarAnimacoes();

            // Mostra a visão clássica por padrão
            MostrarPainelClassico();
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
        /// Mostra apenas a visão clássica
        /// </summary>
        private void BtnMostrarClassico_Click(object sender, RoutedEventArgs e)
        {
            MostrarPainelClassico();
        }

        /// <summary>
        /// Mostra apenas a visão quântica
        /// </summary>
        private void BtnMostrarQuantico_Click(object sender, RoutedEventArgs e)
        {
            MostrarPainelQuantico();
        }

        /// <summary>
        /// Mostra comparação lado a lado
        /// </summary>
        private void BtnComparacao_Click(object sender, RoutedEventArgs e)
        {
            MostrarPainelComparacao();
        }

        /// <summary>
        /// Exibe o painel de visualização clássica
        /// </summary>
        private void MostrarPainelClassico()
        {
            PainelClassico.Visibility = Visibility.Visible;
            PainelQuantico.Visibility = Visibility.Collapsed;
            PainelComparacao.Visibility = Visibility.Collapsed;

            AnimarOndas();
        }

        /// <summary>
        /// Exibe o painel de visualização quântica
        /// </summary>
        private void MostrarPainelQuantico()
        {
            PainelClassico.Visibility = Visibility.Collapsed;
            PainelQuantico.Visibility = Visibility.Visible;
            PainelComparacao.Visibility = Visibility.Collapsed;

            AnimarFotons();
        }

        /// <summary>
        /// Exibe o painel de comparação lado a lado
        /// </summary>
        private void MostrarPainelComparacao()
        {
            PainelClassico.Visibility = Visibility.Collapsed;
            PainelQuantico.Visibility = Visibility.Collapsed;
            PainelComparacao.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Anima as ondas na visualização clássica
        /// </summary>
        private void AnimarOndas()
        {
            // Animação de translação horizontal para simular propagação
            var animacao1 = new DoubleAnimation
            {
                From = -100,
                To = 400,
                Duration = new Duration(System.TimeSpan.FromSeconds(3)),
                RepeatBehavior = RepeatBehavior.Forever
            };

            var animacao2 = new DoubleAnimation
            {
                From = -100,
                To = 400,
                Duration = new Duration(System.TimeSpan.FromSeconds(3.5)),
                RepeatBehavior = RepeatBehavior.Forever
            };

            var animacao3 = new DoubleAnimation
            {
                From = -100,
                To = 400,
                Duration = new Duration(System.TimeSpan.FromSeconds(4)),
                RepeatBehavior = RepeatBehavior.Forever
            };

            if (WaveClassical1 != null)
            {
                var transform1 = new System.Windows.Media.TranslateTransform();
                WaveClassical1.RenderTransform = transform1;
                transform1.BeginAnimation(System.Windows.Media.TranslateTransform.XProperty, animacao1);
            }

            if (WaveClassical2 != null)
            {
                var transform2 = new System.Windows.Media.TranslateTransform();
                WaveClassical2.RenderTransform = transform2;
                transform2.BeginAnimation(System.Windows.Media.TranslateTransform.XProperty, animacao2);
            }

            if (WaveClassical3 != null)
            {
                var transform3 = new System.Windows.Media.TranslateTransform();
                WaveClassical3.RenderTransform = transform3;
                transform3.BeginAnimation(System.Windows.Media.TranslateTransform.XProperty, animacao3);
            }
        }

        /// <summary>
        /// Anima os fótons na visualização quântica
        /// </summary>
        private void AnimarFotons()
        {
            AnimarFoton(Photon1, 1.5, 0);
            AnimarFoton(Photon2, 2.0, 0.3);
            AnimarFoton(Photon3, 1.8, 0.6);
            AnimarFoton(Photon4, 2.2, 0.9);
        }

        /// <summary>
        /// Anima um fóton individual
        /// </summary>
        private void AnimarFoton(System.Windows.Shapes.Ellipse foton, double duracao, double delay)
        {
            if (foton == null) return;

            var animacaoX = new DoubleAnimation
            {
                From = -50,
                To = 350,
                Duration = new Duration(System.TimeSpan.FromSeconds(duracao)),
                BeginTime = System.TimeSpan.FromSeconds(delay),
                RepeatBehavior = RepeatBehavior.Forever
            };

            // Adiciona variação vertical aleatória
            var random = new System.Random();
            var variacaoY = random.Next(-20, 20);

            var animacaoY = new DoubleAnimation
            {
                From = 0,
                To = variacaoY,
                Duration = new Duration(System.TimeSpan.FromSeconds(duracao / 2)),
                AutoReverse = true,
                BeginTime = System.TimeSpan.FromSeconds(delay),
                RepeatBehavior = RepeatBehavior.Forever
            };

            var transform = new System.Windows.Media.TranslateTransform();
            foton.RenderTransform = transform;

            transform.BeginAnimation(System.Windows.Media.TranslateTransform.XProperty, animacaoX);
            transform.BeginAnimation(System.Windows.Media.TranslateTransform.YProperty, animacaoY);

            // Efeito de fade in/out para simular natureza quântica
            var animacaoOpacidade = new DoubleAnimation
            {
                From = 0.3,
                To = 1.0,
                Duration = new Duration(System.TimeSpan.FromSeconds(duracao / 4)),
                AutoReverse = true,
                BeginTime = System.TimeSpan.FromSeconds(delay),
                RepeatBehavior = RepeatBehavior.Forever
            };

            foton.BeginAnimation(UIElement.OpacityProperty, animacaoOpacidade);
        }
    }
}