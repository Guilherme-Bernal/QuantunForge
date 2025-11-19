// LeiNewton1Control.xaml.cs
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Quantun_Forge.src.views.FisicaClassica
{
    public partial class LeiNewton1Control : UserControl
    {
        // Timer de animação
        private DispatcherTimer timer = null!;

        // Variáveis da física
        private double velocidade = 0;          // m/s
        private double posicao = 0;             // m
        private double massa = 10;              // kg
        private double coeficienteAtrito = 0.1; // adimensional
        private double tempo = 0;               // s
        private double forcaExterna = 0;        // N
        private bool simulacaoAtiva = false;
        private bool pausado = false;

        // Constantes
        private const double DT = 0.05;         // 20 FPS
        private const double ESCALA_PIXELS = 5; // 1 metro = 5 pixels

        // Configurações de ambiente
        private readonly (string Nome, double Atrito)[] ambientes = new[]
        {
            ("🌍 Terra", 0.15),
            ("🚀 Espaço", 0.0),
            ("💧 Água", 0.35),
            ("🧊 Gelo", 0.02)
        };

        public LeiNewton1Control()
        {
            InitializeComponent();
            InicializarTimer();
            InicializarEventos();
            AtualizarDisplays();
        }

        private void InicializarTimer()
        {
            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(DT)
            };
            timer.Tick += Timer_Tick;
        }

        private void InicializarEventos()
        {
            // Atualiza os displays dos sliders
            SliderVelocidade.ValueChanged += (s, e) =>
            {
                TxtVelocidadeSlider.Text = $"{e.NewValue:F0} m/s";
            };

            SliderMassa.ValueChanged += (s, e) =>
            {
                TxtMassaSlider.Text = $"{e.NewValue:F0} kg";
            };

            // Atualiza tamanho do objeto baseado na massa
            SliderMassa.ValueChanged += (s, e) =>
            {
                double novaMassa = e.NewValue;
                double novoTamanho = 30 + (novaMassa / 100.0) * 30; // 30 a 60 pixels
                ObjectShape.Width = novoTamanho;
                ObjectShape.Height = novoTamanho;
            };
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (!simulacaoAtiva || pausado) return;

            // Incrementa tempo
            tempo += DT;

            // Calcula força de atrito
            double forcaAtrito = 0;
            if (Math.Abs(velocidade) > 0.01)
            {
                double sinalVelocidade = Math.Sign(velocidade);
                forcaAtrito = -sinalVelocidade * coeficienteAtrito * massa * 9.81; // F_atrito = μ * m * g
            }

            // Força resultante
            double forcaResultante = forcaExterna + forcaAtrito;

            // Aceleração (2ª Lei de Newton: F = m*a)
            double aceleracao = forcaResultante / massa;

            // Atualiza velocidade
            velocidade += aceleracao * DT;

            // Se velocidade muito pequena e sem força externa, para
            if (Math.Abs(velocidade) < 0.1 && forcaExterna == 0)
            {
                velocidade = 0;
            }

            // Atualiza posição
            posicao += velocidade * DT;

            // Limites do canvas
            double larguraCanvas = SimulationCanvas.ActualWidth;
            double margemDireita = larguraCanvas - ObjectShape.Width - 10;

            if (posicao < 0)
            {
                posicao = 0;
                velocidade = 0; // Para ao bater na borda
            }
            else if (posicao * ESCALA_PIXELS > margemDireita)
            {
                posicao = margemDireita / ESCALA_PIXELS;
                velocidade = 0; // Para ao bater na borda
            }

            // Atualiza posição visual
            Canvas.SetLeft(MovingObject, posicao * ESCALA_PIXELS);

            // Atualiza vetor de velocidade
            AtualizarVetorVelocidade();

            // Atualiza displays
            AtualizarDisplays();

            // Atualiza estado
            AtualizarEstado();
        }

        private void AtualizarVetorVelocidade()
        {
            if (Math.Abs(velocidade) > 0.5)
            {
                VelocityVector.Visibility = Visibility.Visible;

                double comprimento = Math.Min(Math.Abs(velocidade) * 5, 100);
                double posX = posicao * ESCALA_PIXELS + ObjectShape.Width;
                double posY = SimulationCanvas.ActualHeight - ObjectShape.Height / 2 - 5;

                VelocityVector.X1 = posX;
                VelocityVector.Y1 = posY;
                VelocityVector.X2 = posX + (velocidade > 0 ? comprimento : -comprimento);
                VelocityVector.Y2 = posY;
            }
            else
            {
                VelocityVector.Visibility = Visibility.Collapsed;
            }
        }

        private void AtualizarDisplays()
        {
            TxtVelocidade.Text = $"{velocidade:F2} m/s";
            TxtPosicao.Text = $"{posicao:F1} m";

            // Aceleração
            double forcaAtrito = 0;
            if (Math.Abs(velocidade) > 0.01)
            {
                forcaAtrito = -Math.Sign(velocidade) * coeficienteAtrito * massa * 9.81;
            }
            double aceleracao = (forcaExterna + forcaAtrito) / massa;
            TxtAceleracao.Text = $"{aceleracao:F2} m/s²";

            TxtTempo.Text = $"{tempo:F2} s";

            // Força resultante
            double forcaResultante = forcaExterna + forcaAtrito;
            TxtForcaResultante.Text = $"Força resultante: {forcaResultante:F1} N";

            // Quantidade de movimento
            double quantidadeMovimento = massa * velocidade;
            TxtQuantidadeMovimento.Text = $"Quantidade de movimento: {quantidadeMovimento:F1} kg⋅m/s";
        }

        private void AtualizarEstado()
        {
            if (Math.Abs(velocidade) < 0.1 && forcaExterna == 0)
            {
                TxtEstadoMovimento.Text = "Em repouso (Inércia)";
                TxtEstadoMovimento.Foreground = new SolidColorBrush(Color.FromRgb(149, 165, 166));
            }
            else if (Math.Abs(forcaExterna) < 0.1)
            {
                TxtEstadoMovimento.Text = "Movimento uniforme (MRU)";
                TxtEstadoMovimento.Foreground = new SolidColorBrush(Color.FromRgb(26, 188, 156));
            }
            else
            {
                TxtEstadoMovimento.Text = "Movimento com aceleração";
                TxtEstadoMovimento.Foreground = new SolidColorBrush(Color.FromRgb(247, 37, 133));
            }
        }

        // Eventos de botões
        private void IniciarSimulacao_Click(object sender, RoutedEventArgs e)
        {
            if (simulacaoAtiva)
            {
                // Se já está rodando, para
                PararSimulacao();
                return;
            }

            // Reseta valores
            tempo = 0;
            posicao = 0;
            forcaExterna = 0;

            // Pega valores dos sliders
            velocidade = SliderVelocidade.Value;
            massa = SliderMassa.Value;

            // Ajusta tamanho do objeto
            double tamanho = 30 + (massa / 100.0) * 30;
            ObjectShape.Width = tamanho;
            ObjectShape.Height = tamanho;

            // Posição inicial
            Canvas.SetLeft(MovingObject, 50);
            Canvas.SetBottom(MovingObject, 5);

            // Inicia timer
            simulacaoAtiva = true;
            pausado = false;
            timer.Start();

            AnimarElemento(MovingObject);
        }

        private void PausarSimulacao_Click(object sender, RoutedEventArgs e)
        {
            if (!simulacaoAtiva) return;

            pausado = !pausado;

            if (sender is Button btn)
            {
                btn.Content = pausado ? "▶️ Continuar" : "⏸️ Pausar";
            }
        }

        private void ResetarSimulacao_Click(object sender, RoutedEventArgs e)
        {
            PararSimulacao();

            tempo = 0;
            posicao = 0;
            velocidade = 0;
            forcaExterna = 0;
            pausado = false;

            Canvas.SetLeft(MovingObject, 50);
            VelocityVector.Visibility = Visibility.Collapsed;

            AtualizarDisplays();
            AtualizarEstado();
        }

        private void PararSimulacao()
        {
            timer.Stop();
            simulacaoAtiva = false;
            pausado = false;
        }

        private void AplicarForcaDireita_Click(object sender, RoutedEventArgs e)
        {
            if (!simulacaoAtiva)
            {
                MessageBox.Show("Inicie a simulação primeiro!", "Aviso", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            forcaExterna += 50; // Adiciona 50N para a direita
            AnimarForca(ObjectShape, 1.2);
        }

        private void AplicarForcaEsquerda_Click(object sender, RoutedEventArgs e)
        {
            if (!simulacaoAtiva)
            {
                MessageBox.Show("Inicie a simulação primeiro!", "Aviso", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            forcaExterna -= 50; // Adiciona 50N para a esquerda
            AnimarForca(ObjectShape, 0.8);
        }

        private void RemoverForcas_Click(object sender, RoutedEventArgs e)
        {
            forcaExterna = 0;
            AnimarElemento(ObjectShape);
        }

        private void ComboAmbiente_Changed(object sender, SelectionChangedEventArgs e)
        {
            int index = ComboAmbiente.SelectedIndex;
            if (index >= 0 && index < ambientes.Length)
            {
                var ambiente = ambientes[index];
                coeficienteAtrito = ambiente.Atrito;
                EnvironmentLabel.Text = ambiente.Nome;
            }
        }

        // Animações
        private void AnimarElemento(UIElement elemento)
        {
            var fade = new DoubleAnimation
            {
                From = 0.5,
                To = 1.0,
                Duration = TimeSpan.FromSeconds(0.3)
            };
            elemento.BeginAnimation(OpacityProperty, fade);
        }

        private void AnimarForca(Ellipse objeto, double escala)
        {
            var scaleTransform = new ScaleTransform(1, 1);
            objeto.RenderTransform = scaleTransform;
            objeto.RenderTransformOrigin = new Point(0.5, 0.5);

            var scaleAnimation = new DoubleAnimation
            {
                From = 1.0,
                To = escala,
                Duration = TimeSpan.FromSeconds(0.1),
                AutoReverse = true
            };

            scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, scaleAnimation);
            scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, scaleAnimation);
        }
    }
}