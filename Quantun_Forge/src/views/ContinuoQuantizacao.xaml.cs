using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Shapes;

namespace Quantun_Forge.src
{
    /// <summary>
    /// Módulo interativo sobre a transição do Campo Contínuo à Quantização
    /// Demonstra visualmente a diferença entre ondas clássicas e fótons quânticos
    /// </summary>
    public partial class ContinuoQuantizacao : UserControl
    {
        private int _frequencia = 5;
        private int _intensidade = 5;

        public ContinuoQuantizacao()
        {
            InitializeComponent();
            InicializarAnimacoes();
            AtualizarVisualizacoes();
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
        /// Evento de mudança da frequência
        /// </summary>
        private void SliderFrequencia_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _frequencia = (int)e.NewValue;

            if (TxtFrequencia != null)
            {
                TxtFrequencia.Text = $"f = {_frequencia} | E(fóton) = {_frequencia}hf";
            }

            AtualizarVisualizacoes();
        }

        /// <summary>
        /// Evento de mudança da intensidade
        /// </summary>
        private void SliderIntensidade_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _intensidade = (int)e.NewValue;

            if (TxtIntensidade != null)
            {
                TxtIntensidade.Text = $"I = {_intensidade} | Nº de fótons = {_intensidade}";
            }

            AtualizarVisualizacoes();
        }

        /// <summary>
        /// Atualiza ambas as visualizações (clássica e quântica)
        /// </summary>
        private void AtualizarVisualizacoes()
        {
            GerarOndaClassica();
            GerarFotonsQuanticos();
        }

        /// <summary>
        /// Gera a onda senoidal contínua (visão clássica)
        /// </summary>
        private void GerarOndaClassica()
        {
            if (OndaClassica == null || CanvasClassico == null) return;

            double largura = 400;
            double altura = 180;
            double centroY = altura / 2;

            // Amplitude proporcional à intensidade
            double amplitude = 30 + (_intensidade * 3);

            // Número de ciclos proporcional à frequência
            double ciclos = _frequencia * 0.8;

            // Criar geometria da onda senoidal
            PathGeometry pathGeometry = new PathGeometry();
            PathFigure pathFigure = new PathFigure();
            pathFigure.StartPoint = new Point(20, centroY);

            // Gerar pontos da senoide
            int pontos = 200;
            for (int i = 0; i <= pontos; i++)
            {
                double x = 20 + (largura / pontos) * i;
                double radianos = (i / (double)pontos) * ciclos * 2 * Math.PI;
                double y = centroY - (amplitude * Math.Sin(radianos));

                LineSegment segment = new LineSegment(new Point(x, y), true);
                pathFigure.Segments.Add(segment);
            }

            pathGeometry.Figures.Add(pathFigure);
            OndaClassica.Data = pathGeometry;

            // Animar a onda
            AnimarOnda();
        }

        /// <summary>
        /// Anima a propagação da onda clássica
        /// </summary>
        private void AnimarOnda()
        {
            if (OndaClassica == null) return;

            var storyboard = new Storyboard { RepeatBehavior = RepeatBehavior.Forever };

            var animacaoOpacidade = new DoubleAnimation
            {
                From = 0.6,
                To = 1.0,
                Duration = TimeSpan.FromSeconds(1.0 / _frequencia),
                AutoReverse = true
            };

            Storyboard.SetTarget(animacaoOpacidade, OndaClassica);
            Storyboard.SetTargetProperty(animacaoOpacidade, new PropertyPath("Opacity"));
            storyboard.Children.Add(animacaoOpacidade);

            storyboard.Begin();
        }

        /// <summary>
        /// Gera os fótons discretos (visão quântica)
        /// </summary>
        private void GerarFotonsQuanticos()
        {
            if (ContainerFotons == null || CanvasQuantico == null) return;

            ContainerFotons.Items.Clear();

            double largura = 400;
            double altura = 180;
            double centroY = altura / 2;

            // Número de fótons baseado na intensidade
            int numeroFotons = _intensidade;

            // Tamanho dos fótons baseado na frequência (energia)
            double tamanhoFoton = 8 + (_frequencia * 1.5);

            // Distribuir fótons ao longo do canvas
            double espacamento = largura / (numeroFotons + 1);

            for (int i = 0; i < numeroFotons; i++)
            {
                double x = 30 + (i * espacamento);

                // Posição Y varia senoidalmente para manter padrão visual
                double fase = (i / (double)numeroFotons) * _frequencia * 2 * Math.PI;
                double variacaoY = 25 * Math.Sin(fase);
                double y = centroY + variacaoY;

                // Criar fóton
                Ellipse foton = new Ellipse
                {
                    Width = tamanhoFoton,
                    Height = tamanhoFoton,
                    Fill = new SolidColorBrush(Color.FromRgb(255, 215, 0)), // Dourado
                    Stroke = new SolidColorBrush(Color.FromRgb(255, 193, 7)),
                    StrokeThickness = 2
                };

                // Efeito de brilho
                foton.Effect = new DropShadowEffect
                {
                    Color = Color.FromRgb(255, 215, 0),
                    BlurRadius = 15,
                    ShadowDepth = 0,
                    Opacity = 0.8
                };

                // Posicionar no canvas
                Canvas.SetLeft(foton, x - tamanhoFoton / 2);
                Canvas.SetTop(foton, y - tamanhoFoton / 2);

                ContainerFotons.Items.Add(foton);

                // Animar fóton individual
                AnimarFoton(foton, i);
            }
        }

        /// <summary>
        /// Anima um fóton individual
        /// </summary>
        private void AnimarFoton(Ellipse foton, int indice)
        {
            var storyboard = new Storyboard { RepeatBehavior = RepeatBehavior.Forever };

            // Animação de pulsação
            var animacaoEscala = new DoubleAnimation
            {
                From = 1.0,
                To = 1.3,
                Duration = TimeSpan.FromSeconds(0.5),
                AutoReverse = true,
                BeginTime = TimeSpan.FromSeconds(indice * 0.1) // Defasagem
            };

            var scaleTransform = new ScaleTransform(1.0, 1.0, foton.Width / 2, foton.Height / 2);
            foton.RenderTransform = scaleTransform;

            Storyboard.SetTarget(animacaoEscala, scaleTransform);
            Storyboard.SetTargetProperty(animacaoEscala, new PropertyPath("ScaleX"));
            storyboard.Children.Add(animacaoEscala);

            var animacaoEscalaY = new DoubleAnimation
            {
                From = 1.0,
                To = 1.3,
                Duration = TimeSpan.FromSeconds(0.5),
                AutoReverse = true,
                BeginTime = TimeSpan.FromSeconds(indice * 0.1)
            };

            Storyboard.SetTarget(animacaoEscalaY, scaleTransform);
            Storyboard.SetTargetProperty(animacaoEscalaY, new PropertyPath("ScaleY"));
            storyboard.Children.Add(animacaoEscalaY);

            // Animação de opacidade (piscando)
            var animacaoOpacidade = new DoubleAnimation
            {
                From = 0.7,
                To = 1.0,
                Duration = TimeSpan.FromSeconds(0.6),
                AutoReverse = true,
                BeginTime = TimeSpan.FromSeconds(indice * 0.1)
            };

            Storyboard.SetTarget(animacaoOpacidade, foton);
            Storyboard.SetTargetProperty(animacaoOpacidade, new PropertyPath("Opacity"));
            storyboard.Children.Add(animacaoOpacidade);

            storyboard.Begin();
        }
    }
}