using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Quantun_Forge.src.views.FisicaQuantica
{
    /// <summary>
    /// Simulador Visual do Princípio da Incerteza de Heisenberg
    /// </summary>
    public partial class Heisenberg : UserControl
    {
        #region Constantes Físicas

        private const double PLANCK_CONSTANT = 6.62607015e-34;      // J·s
        private const double REDUCED_PLANCK = 1.054571817e-34;      // ℏ = h/2π (J·s)
        private const double ELECTRON_MASS = 9.10938356e-31;        // kg

        #endregion

        #region Variáveis de Estado

        private double deltaX = 1.0e-10;  // Incerteza em posição (m)
        private double deltaP = 5.3e-25;  // Incerteza em momento (kg·m/s)

        private DispatcherTimer waveTimer;
        private double wavePhase = 0;

        #endregion

        public Heisenberg()
        {
            InitializeComponent();
            Loaded += Heisenberg_Loaded;
        }

        private void Heisenberg_Loaded(object sender, RoutedEventArgs e)
        {
            InicializarSimulador();
        }

        #region Inicialização

        private void InicializarSimulador()
        {
            // Timer para animação de ondas
            waveTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(50)
            };
            waveTimer.Tick += WaveTimer_Tick;
            waveTimer.Start();

            // Desenhar visualizações iniciais
            AtualizarVisualizacao();
            DesenharPacotesDeOnda();
        }

        #endregion

        #region Cálculos

        /// <summary>
        /// Calcula a incerteza mínima em momento dado Δx
        /// Δp ≥ ℏ/(2Δx)
        /// </summary>
        private double CalcularDeltaPMinimo(double deltaX)
        {
            return REDUCED_PLANCK / (2.0 * deltaX);
        }

        /// <summary>
        /// Calcula o produto das incertezas
        /// </summary>
        private double CalcularProduto(double deltaX, double deltaP)
        {
            return deltaX * deltaP;
        }

        /// <summary>
        /// Verifica se satisfaz o princípio
        /// Δx·Δp ≥ ℏ/2
        /// </summary>
        private bool VerificaPrincipio(double deltaX, double deltaP)
        {
            double produto = CalcularProduto(deltaX, deltaP);
            double limite = REDUCED_PLANCK / 2.0;
            return produto >= limite;
        }

        #endregion

        #region Atualização de Visualização

        private void AtualizarVisualizacao()
        {
            // Calcular valores
            double produto = CalcularProduto(deltaX, deltaP);
            double limite = REDUCED_PLANCK / 2.0;
            bool satisfaz = VerificaPrincipio(deltaX, deltaP);

            // Atualizar displays de valores
            TxtDeltaXValue.Text = $"{deltaX:E2} m";
            TxtDeltaPValue.Text = $"{deltaP:E2} kg·m/s";
            TxtProducto.Text = $"{produto:E2} J·s";

            // Atualizar labels nos indicadores
            TxtDeltaX.Text = $"Δx = {deltaX:E1} m";
            TxtDeltaP.Text = $"Δp = {deltaP:E1}";

            // Atualizar largura dos indicadores visuais
            AtualizarIndicadoresVisuais();

            // Verificação do princípio
            if (satisfaz)
            {
                BorderVerificacao.Background = new SolidColorBrush(Color.FromRgb(213, 244, 230));
                BorderVerificacao.BorderBrush = new SolidColorBrush(Color.FromRgb(39, 174, 96));
                TxtVerificacao.Text = "✓ SATISFEITO";
                TxtVerificacao.Foreground = new SolidColorBrush(Color.FromRgb(39, 174, 96));
            }
            else
            {
                BorderVerificacao.Background = new SolidColorBrush(Color.FromRgb(250, 219, 216));
                BorderVerificacao.BorderBrush = new SolidColorBrush(Color.FromRgb(231, 76, 60));
                TxtVerificacao.Text = "✗ NÃO SATISFEITO";
                TxtVerificacao.Foreground = new SolidColorBrush(Color.FromRgb(231, 76, 60));
            }

            TxtComparacao.Text = $"{produto:E2} ≥ {limite:E2}";
        }

        private void AtualizarIndicadoresVisuais()
        {
            // Normalizar valores para visualização (escala logarítmica)
            double logDeltaX = Math.Log10(deltaX);
            double logDeltaP = Math.Log10(deltaP);

            // Mapear para larguras (inversamente proporcional)
            // Δx: quanto menor, mais estreito o indicador
            double larguraX = MapearValor(logDeltaX, -11, -9, 30, 250);
            DeltaXIndicator.Width = larguraX;
            Canvas.SetLeft(DeltaXIndicator, 200 - larguraX / 2);

            // Δp: quanto maior, mais largo o indicador
            double larguraP = MapearValor(logDeltaP, -25, -23, 30, 250);
            DeltaPIndicator.Width = larguraP;
            Canvas.SetLeft(DeltaPIndicator, 200 - larguraP / 2);
        }

        private double MapearValor(double valor, double minEntrada, double maxEntrada, double minSaida, double maxSaida)
        {
            valor = Math.Max(minEntrada, Math.Min(maxEntrada, valor));
            return minSaida + (valor - minEntrada) * (maxSaida - minSaida) / (maxEntrada - minEntrada);
        }

        #endregion

        #region Desenho de Ondas

        private void WaveTimer_Tick(object sender, EventArgs e)
        {
            wavePhase += 0.15;
            if (wavePhase > 2 * Math.PI) wavePhase = 0;

            DesenharFuncaoOnda();
        }

        private void DesenharFuncaoOnda()
        {
            PositionWaveCanvas.Children.Clear();
            MomentumWaveCanvas.Children.Clear();

            // Função de onda de posição (Gaussiana)
            DesenharGaussiana(PositionWaveCanvas, deltaX, Color.FromRgb(231, 76, 60));

            // Função de onda de momento (Gaussiana inversa)
            DesenharGaussiana(MomentumWaveCanvas, deltaP, Color.FromRgb(52, 152, 219));
        }

        private void DesenharGaussiana(Canvas canvas, double sigma, Color cor)
        {
            var polyline = new Polyline
            {
                Stroke = new SolidColorBrush(cor),
                StrokeThickness = 3,
                Opacity = 0.8
            };

            double canvasWidth = canvas.ActualWidth > 0 ? canvas.ActualWidth : 400;

            // Normalizar sigma para visualização
            double logSigma = Math.Log10(sigma);
            double largura = MapearValor(logSigma, -11, -23, 20, 150);

            for (int i = 0; i <= 100; i++)
            {
                double x = (i / 100.0) * canvasWidth;
                double xNorm = (x - canvasWidth / 2) / largura;

                // Gaussiana: exp(-x²/2)
                double y = 175 - 50 * Math.Exp(-xNorm * xNorm / 2) * Math.Cos(wavePhase);

                polyline.Points.Add(new Point(x, y));
            }

            canvas.Children.Add(polyline);

            // Área sombreada (envelope)
            var area = new Polygon
            {
                Fill = new SolidColorBrush(Color.FromArgb(50, cor.R, cor.G, cor.B))
            };

            for (int i = 0; i <= 100; i++)
            {
                double x = (i / 100.0) * canvasWidth;
                double xNorm = (x - canvasWidth / 2) / largura;
                double y = 175 - 50 * Math.Exp(-xNorm * xNorm / 2);
                area.Points.Add(new Point(x, y));
            }

            for (int i = 100; i >= 0; i--)
            {
                double x = (i / 100.0) * canvasWidth;
                area.Points.Add(new Point(x, 175));
            }

            canvas.Children.Insert(0, area);
        }

        private void DesenharPacotesDeOnda()
        {
            // Pacote de onda localizado (Δx pequeno, Δp grande)
            DesenharPacote(LocalizedWaveCanvas, 30, 10, Color.FromRgb(231, 76, 60));

            // Pacote de onda espalhado (Δx grande, Δp pequeno)
            DesenharPacote(SpreadWaveCanvas, 150, 3, Color.FromRgb(52, 152, 219));
        }

        private void DesenharPacote(Canvas canvas, double larguraEnvelope, int numOndas, Color cor)
        {
            if (canvas == null) return;

            canvas.Children.Clear();

            double canvasWidth = canvas.ActualWidth > 0 ? canvas.ActualWidth : 350;
            double canvasHeight = canvas.ActualHeight > 0 ? canvas.ActualHeight : 120;

            // Envelope Gaussiano
            var envelope = new Polygon
            {
                Fill = new SolidColorBrush(Color.FromArgb(50, cor.R, cor.G, cor.B)),
                Stroke = new SolidColorBrush(Color.FromArgb(100, cor.R, cor.G, cor.B)),
                StrokeThickness = 1
            };

            // Parte superior do envelope
            for (int i = 0; i <= 100; i++)
            {
                double x = (i / 100.0) * canvasWidth;
                double xNorm = (x - canvasWidth / 2) / larguraEnvelope;
                double amplitude = 40 * Math.Exp(-xNorm * xNorm);
                double y = canvasHeight / 2 - amplitude;
                envelope.Points.Add(new Point(x, y));
            }

            // Parte inferior do envelope
            for (int i = 100; i >= 0; i--)
            {
                double x = (i / 100.0) * canvasWidth;
                double xNorm = (x - canvasWidth / 2) / larguraEnvelope;
                double amplitude = 40 * Math.Exp(-xNorm * xNorm);
                double y = canvasHeight / 2 + amplitude;
                envelope.Points.Add(new Point(x, y));
            }

            canvas.Children.Add(envelope);

            // Onda portadora
            var wave = new Polyline
            {
                Stroke = new SolidColorBrush(cor),
                StrokeThickness = 2
            };

            for (int i = 0; i <= 200; i++)
            {
                double x = (i / 200.0) * canvasWidth;
                double xNorm = (x - canvasWidth / 2) / larguraEnvelope;
                double amplitude = 40 * Math.Exp(-xNorm * xNorm);
                double y = canvasHeight / 2 + amplitude * Math.Sin(numOndas * (i / 200.0) * 2 * Math.PI);
                wave.Points.Add(new Point(x, y));
            }

            canvas.Children.Add(wave);

            // Linha central
            var centerLine = new Line
            {
                X1 = 0,
                Y1 = canvasHeight / 2,
                X2 = canvasWidth,
                Y2 = canvasHeight / 2,
                Stroke = Brushes.Gray,
                StrokeThickness = 1,
                StrokeDashArray = new DoubleCollection { 2, 2 }
            };
            canvas.Children.Insert(0, centerLine);
        }

        #endregion

        #region Eventos dos Controles

        private void SliderDeltaX_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!IsLoaded) return;

            // Mapear slider (1-10) para valores reais de Δx
            // Escala logarítmica: 10^-11 até 10^-9 metros
            double expoente = -11 + (e.NewValue - 1) * 0.2;
            deltaX = Math.Pow(10, expoente);

            // Calcular Δp mínimo necessário
            deltaP = CalcularDeltaPMinimo(deltaX);

            // Adicionar uma margem para garantir satisfação
            deltaP *= 1.1;

            // Atualizar label do slider
            string[] labels = { "Muito Pequeno", "Pequeno", "Médio-Baixo", "Médio", "Médio-Alto",
                              "Alto", "Muito Alto", "Grande", "Muito Grande", "Enorme" };
            int index = Math.Max(0, Math.Min(9, (int)(e.NewValue - 1)));
            TxtSliderDeltaX.Text = $"{labels[index]} ({e.NewValue:F0})";

            AtualizarVisualizacao();
        }

        private void Resetar_Click(object sender, RoutedEventArgs e)
        {
            SliderDeltaX.Value = 5;
            deltaX = 1.0e-10;
            deltaP = CalcularDeltaPMinimo(deltaX) * 1.1;
            AtualizarVisualizacao();
        }

        private void Aleatorio_Click(object sender, RoutedEventArgs e)
        {
            Random rand = new Random();
            SliderDeltaX.Value = rand.Next(1, 11);
        }

        #endregion

        #region Exemplos Práticos

        private void ExemploEletron_Click(object sender, RoutedEventArgs e)
        {
            // Elétron em átomo de hidrogênio
            deltaX = 5.3e-11;  // Raio de Bohr
            deltaP = CalcularDeltaPMinimo(deltaX) * 1.1;

            MessageBox.Show(
                "Elétron no Átomo de Hidrogênio\n\n" +
                "Se confinamos um elétron em ~0.5 Å:\n" +
                $"Δx ≈ {deltaX:E2} m (Raio de Bohr)\n" +
                $"Δp ≥ {deltaP:E2} kg·m/s\n\n" +
                "Isso significa que o elétron tem uma\n" +
                "velocidade mínima de ~2.2×10⁶ m/s!\n\n" +
                "Por isso os elétrons não 'caem' no núcleo!",
                "Exemplo: Elétron em Átomo",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            AtualizarVisualizacao();
        }

        private void ExemploBola_Click(object sender, RoutedEventArgs e)
        {
            // Bola de tênis macroscópica
            double deltaXBola = 0.001;  // 1 mm
            double massa = 0.058;  // 58g
            double deltaPBola = CalcularDeltaPMinimo(deltaXBola) * 1.1;
            double velocidadeMin = deltaPBola / massa;

            MessageBox.Show(
                "Bola de Tênis (objeto macroscópico)\n\n" +
                "Se medirmos a posição com precisão de 1 mm:\n" +
                $"Δx = {deltaXBola} m\n" +
                $"Δp ≥ {deltaPBola:E2} kg·m/s\n\n" +
                $"Incerteza na velocidade: {velocidadeMin:E2} m/s\n\n" +
                "Isso é EXTREMAMENTE pequeno!\n" +
                "Por isso não notamos efeitos quânticos\n" +
                "em objetos do dia a dia.",
                "Exemplo: Bola de Tênis",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void ExemploMicroscopio_Click(object sender, RoutedEventArgs e)
        {
            // Microscópio eletrônico
            deltaX = 1.0e-10;  // Resolução típica
            deltaP = CalcularDeltaPMinimo(deltaX) * 1.1;

            MessageBox.Show(
                "Microscópio Eletrônico\n\n" +
                "Para 'ver' algo, a luz/elétron deve\n" +
                "interagir com o objeto.\n\n" +
                "Resolução λ ~ 1 Å:\n" +
                $"Δx ≈ {deltaX:E2} m\n" +
                $"Δp ≥ {deltaP:E2} kg·m/s\n\n" +
                "O ato de medir PERTURBA o sistema!\n" +
                "Quanto melhor a resolução, maior\n" +
                "a perturbação no momento.",
                "Exemplo: Microscópio",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            AtualizarVisualizacao();
        }

        private void ExemploFenda_Click(object sender, RoutedEventArgs e)
        {
            // Experimento da dupla fenda
            deltaX = 1.0e-7;  // Largura da fenda (~100 nm)
            deltaP = CalcularDeltaPMinimo(deltaX) * 1.1;
            double velocidade = deltaP / ELECTRON_MASS;

            MessageBox.Show(
                "Experimento da Dupla Fenda\n\n" +
                "Fenda com largura de ~100 nm:\n" +
                $"Δx = {deltaX:E2} m\n" +
                $"Δp ≥ {deltaP:E2} kg·m/s\n\n" +
                $"Velocidade mínima: {velocidade:E2} m/s\n\n" +
                "A partícula 'não sabe' exatamente seu\n" +
                "momento ao passar pela fenda!\n\n" +
                "Isso causa o padrão de difração!",
                "Exemplo: Fenda",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            AtualizarVisualizacao();
        }

        #endregion
    }
}