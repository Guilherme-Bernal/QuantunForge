// LeiNewton2Control.xaml.cs
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Quantun_Forge.src.views.FisicaClassica
{
    public partial class LeiNewton2Control : UserControl
    {
        // Timer de animação
        private DispatcherTimer timer = null!;

        // Variáveis da física
        private double forca = 50;           // N
        private double massa = 5;            // kg
        private double aceleracao = 0;       // m/s²
        private double velocidade = 0;       // m/s
        private double posicao = 0;          // m
        private double tempo = 0;            // s
        private bool simulacaoAtiva = false;

        // Constantes
        private const double DT = 0.05;      // 20 FPS
        private const double ESCALA_PIXELS = 3; // pixels por metro

        // Dados para tabela comparativa
        private ObservableCollection<ComparativoData> dadosComparativos = new();

        public LeiNewton2Control()
        {
            InitializeComponent();
            InicializarTimer();
            InicializarEventos();
            InicializarTabelaComparativa();
            AtualizarCalculos();
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
            // Atualiza labels dos sliders
            SliderForca.ValueChanged += (s, e) =>
            {
                TxtForcaSlider.Text = $"{e.NewValue:F0} N";
            };

            SliderMassa.ValueChanged += (s, e) =>
            {
                TxtMassaSlider.Text = $"{e.NewValue:F0} kg";
                AjustarTamanhoObjeto(e.NewValue);
            };
        }

        private void InicializarTabelaComparativa()
        {
            dadosComparativos = new ObservableCollection<ComparativoData>
            {
                new ComparativoData { Forca = "10", Massa = "2", Aceleracao = "5.00", Exemplo = "Empurrão leve em cadeira" },
                new ComparativoData { Forca = "50", Massa = "5", Aceleracao = "10.00", Exemplo = "Pessoa empurrando caixa" },
                new ComparativoData { Forca = "100", Massa = "10", Aceleracao = "10.00", Exemplo = "Força moderada em objeto pesado" },
                new ComparativoData { Forca = "100", Massa = "50", Aceleracao = "2.00", Exemplo = "Empurrar móvel pesado" },
                new ComparativoData { Forca = "200", Massa = "100", Aceleracao = "2.00", Exemplo = "Força alta em massa grande" }
            };

            ComparisonTable.ItemsSource = dadosComparativos;
        }

        private void AjustarTamanhoObjeto(double massa)
        {
            // Tamanho proporcional à massa: 30px (min) a 100px (max)
            double tamanho = 30 + (massa / 100.0) * 70;
            ObjectShape.Width = tamanho;
            ObjectShape.Height = tamanho;
            MassValue.Text = $"{massa:F0} kg";
        }

        private void Sliders_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!IsLoaded) return;

            forca = SliderForca.Value;
            massa = SliderMassa.Value;

            AtualizarCalculos();
        }

        private void AtualizarCalculos()
        {
            // Calcula aceleração: a = F / m
            aceleracao = massa > 0 ? forca / massa : 0;

            // Atualiza displays
            TxtForca.Text = $"{forca:F0} N";
            TxtMassa.Text = $"{massa:F0} kg";
            TxtAceleracao.Text = $"{aceleracao:F2} m/s²";

            // Atualiza cálculo completo
            TxtCalculoCompleto.Text = $"F = m × a\n" +
                                     $"{forca:F0} N = {massa:F0} kg × {aceleracao:F2} m/s²\n\n" +
                                     $"Portanto:\n" +
                                     $"a = F ÷ m = {forca:F0} ÷ {massa:F0} = {aceleracao:F2} m/s²";

            // Atualiza tamanho dos vetores (proporcional aos valores)
            AtualizarVetores();
        }

        private void AtualizarVetores()
        {
            // Vetor de força (verde)
            double comprimentoForca = Math.Min(forca * 0.5, 150);
            ForceVectorLine.Point = new Point(110 + comprimentoForca, 150);

            if (forca > 0)
            {
                ForceVector.Visibility = Visibility.Visible;
                ForceLabelCanvas.Visibility = Visibility.Visible;
                Canvas.SetLeft(ForceLabelCanvas, 110 + comprimentoForca / 2);
            }
            else
            {
                ForceVector.Visibility = Visibility.Collapsed;
                ForceLabelCanvas.Visibility = Visibility.Collapsed;
            }

            // Vetor de aceleração (laranja, proporcional à aceleração)
            double comprimentoAcel = Math.Min(aceleracao * 5, 120);
            AccelVectorLine.Point = new Point(110 + comprimentoAcel, 170);

            if (aceleracao > 0)
            {
                AccelerationVector.Visibility = Visibility.Visible;
                AccelLabelCanvas.Visibility = Visibility.Visible;
                Canvas.SetLeft(AccelLabelCanvas, 110 + comprimentoAcel / 2);
            }
            else
            {
                AccelerationVector.Visibility = Visibility.Collapsed;
                AccelLabelCanvas.Visibility = Visibility.Collapsed;
            }
        }

        private void AplicarForca_Click(object sender, RoutedEventArgs e)
        {
            if (simulacaoAtiva)
            {
                MessageBox.Show("Simulação já está ativa! Clique em Resetar primeiro.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // Reset valores
            tempo = 0;
            velocidade = 0;
            posicao = 0;

            // Pega valores atuais
            forca = SliderForca.Value;
            massa = SliderMassa.Value;
            aceleracao = massa > 0 ? forca / massa : 0;

            if (Math.Abs(forca) < 0.1)
            {
                MessageBox.Show("Configure uma força maior que zero!", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Posição inicial
            Canvas.SetLeft(MovingObject, 50);
            Canvas.SetBottom(MovingObject, 5);

            // Inicia simulação
            simulacaoAtiva = true;
            timer.Start();

            AnimarElemento(MovingObject);
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (!simulacaoAtiva) return;

            // Incrementa tempo
            tempo += DT;

            // Atualiza velocidade: v = v₀ + a×t
            velocidade += aceleracao * DT;

            // Atualiza posição: x = x₀ + v×t
            posicao += velocidade * DT;

            // Limites do canvas
            double larguraCanvas = SimulationCanvas.ActualWidth;
            double margemDireita = larguraCanvas - ObjectShape.Width - 10;

            // Verifica limites
            if (posicao * ESCALA_PIXELS >= margemDireita)
            {
                posicao = margemDireita / ESCALA_PIXELS;
                PararSimulacao();
                MessageBox.Show("O objeto chegou ao fim da pista!", "Simulação Concluída", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            // Atualiza posição visual
            Canvas.SetLeft(MovingObject, 50 + (posicao * ESCALA_PIXELS));

            // Atualiza display de velocidade
            TxtVelocidade.Text = $"{velocidade:F2} m/s";

            // Animação de "pulse" no objeto
            if ((int)(tempo * 10) % 5 == 0)
            {
                AnimarPulse(ObjectShape);
            }
        }

        private void PararSimulacao()
        {
            timer.Stop();
            simulacaoAtiva = false;
        }

        private void Resetar_Click(object sender, RoutedEventArgs e)
        {
            PararSimulacao();

            // Reset variáveis
            tempo = 0;
            velocidade = 0;
            posicao = 0;

            // Reset visual
            Canvas.SetLeft(MovingObject, 50);
            Canvas.SetBottom(MovingObject, 5);
            TxtVelocidade.Text = "0 m/s";

            AtualizarCalculos();
        }

        // Cenários pré-definidos
        private void Cenario1_Click(object sender, RoutedEventArgs e)
        {
            SliderForca.Value = 50;
            SliderMassa.Value = 70;
            MessageBox.Show("Cenário configurado!\n\nPessoa de 70 kg sendo empurrada com força de 50 N.\n\nAceleração: ~0.71 m/s²",
                          "Cenário: Pessoa", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Cenario2_Click(object sender, RoutedEventArgs e)
        {
            // Ajusta valores para caber nos sliders (escalados)
            SliderForca.Value = 100; // Representa 10000N (escala 1:100)
            SliderMassa.Value = 12;  // Representa 1200kg (escala 1:100)
            MessageBox.Show("Cenário configurado! (valores escalados)\n\nCarro de 1200 kg com força de 10000 N.\n\nAceleração: ~8.33 m/s²",
                          "Cenário: Carro", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Cenario3_Click(object sender, RoutedEventArgs e)
        {
            SliderForca.Value = 5;
            SliderMassa.Value = 1; // Representa 0.06kg (massa mínima do slider)
            MessageBox.Show("Cenário configurado!\n\nBola de tênis (~0.06 kg) com força de 5 N.\n\nAceleração: muito alta!",
                          "Cenário: Bola de Tênis", MessageBoxButton.OK, MessageBoxImage.Information);
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

        private void AnimarPulse(Shape shape)
        {
            var colorAnimation = new ColorAnimation
            {
                From = Color.FromRgb(247, 37, 133), // Rosa original
                To = Color.FromRgb(255, 100, 180),   // Rosa claro
                Duration = TimeSpan.FromSeconds(0.2),
                AutoReverse = true
            };

            var brush = shape.Fill as SolidColorBrush;
            if (brush != null)
            {
                brush.BeginAnimation(SolidColorBrush.ColorProperty, colorAnimation);
            }
        }

        // Classe auxiliar para tabela comparativa
        public class ComparativoData
        {
            public string Forca { get; set; } = "";
            public string Massa { get; set; } = "";
            public string Aceleracao { get; set; } = "";
            public string Exemplo { get; set; } = "";
        }
    }
}