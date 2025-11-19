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
    /// Simulador Visual da Equação de Schrödinger em 1D
    /// </summary>
    public partial class Schrodinger1D : UserControl
    {
        #region Constantes Físicas

        private const double PLANCK_CONSTANT = 6.62607015e-34;      // J·s
        private const double REDUCED_PLANCK = 1.054571817e-34;      // ℏ (J·s)
        private const double ELECTRON_MASS = 9.10938356e-31;        // kg
        private const double ELECTRON_VOLT = 1.602176634e-19;       // J

        #endregion

        #region Enums

        private enum TipoPotencial
        {
            PocoInfinito,
            PocoFinito,
            OsciladorHarmonico,
            Barreira,
            ParticulaLivre
        }

        #endregion

        #region Variáveis de Estado

        private TipoPotencial potencialAtual = TipoPotencial.PocoInfinito;
        private int numeroQuantico = 1;
        private double massa = ELECTRON_MASS;
        private double larguraPoco = 1.0e-10;  // 1 Å

        private DispatcherTimer animationTimer;
        private double tempo = 0;
        private bool animando = false;

        #endregion

        public Schrodinger1D()
        {
            InitializeComponent();
            Loaded += Schrodinger1D_Loaded;
        }

        private void Schrodinger1D_Loaded(object sender, RoutedEventArgs e)
        {
            InicializarSimulador();
        }

        #region Inicialização

        private void InicializarSimulador()
        {
            // Timer para animação
            animationTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(50)
            };
            animationTimer.Tick += AnimationTimer_Tick;

            // Desenhar visualizações iniciais
            DesenharGrid();
            DesenharPocotesEstaticos();
            AtualizarVisualizacao();
        }

        #endregion

        #region Cálculos Físicos

        /// <summary>
        /// Calcula energia do estado n para poço infinito
        /// E_n = n²π²ℏ²/(2mL²)
        /// </summary>
        private double CalcularEnergiaPocoInfinito(int n)
        {
            return (n * n * Math.PI * Math.PI * REDUCED_PLANCK * REDUCED_PLANCK)
                   / (2.0 * massa * larguraPoco * larguraPoco);
        }

        /// <summary>
        /// Calcula energia do oscilador harmônico
        /// E_n = ℏω(n + 1/2)
        /// </summary>
        private double CalcularEnergiaOscilador(int n, double omega)
        {
            return REDUCED_PLANCK * omega * (n + 0.5);
        }

        /// <summary>
        /// Função de onda do poço infinito
        /// ψ_n(x) = √(2/L) sin(nπx/L)
        /// </summary>
        private double PsiPocoInfinito(double x, int n)
        {
            if (x < 0 || x > larguraPoco)
                return 0;

            return Math.Sqrt(2.0 / larguraPoco) * Math.Sin(n * Math.PI * x / larguraPoco);
        }

        /// <summary>
        /// Função de onda do oscilador harmônico (aproximação Gaussiana)
        /// </summary>
        private double PsiOscilador(double x, int n, double omega)
        {
            double x0 = larguraPoco / 2;
            double sigma = Math.Sqrt(REDUCED_PLANCK / (massa * omega));
            double xNorm = (x - x0) / sigma;

            // Hermite polinomials simplificados
            double hermite = 1;
            if (n == 1) hermite = 2 * xNorm;
            else if (n == 2) hermite = 4 * xNorm * xNorm - 2;
            else if (n == 3) hermite = 8 * xNorm * xNorm * xNorm - 12 * xNorm;

            return hermite * Math.Exp(-xNorm * xNorm / 2) / Math.Sqrt(Math.Pow(2, n) * Factorial(n) * sigma * Math.Sqrt(Math.PI));
        }

        private double Factorial(int n)
        {
            if (n <= 1) return 1;
            double result = 1;
            for (int i = 2; i <= n; i++)
                result *= i;
            return result;
        }

        /// <summary>
        /// Comprimento de onda de De Broglie
        /// λ = h/p = h/√(2mE)
        /// </summary>
        private double CalcularComprimentoOnda(double energia)
        {
            if (energia <= 0) return 0;
            double momento = Math.Sqrt(2 * massa * energia);
            return PLANCK_CONSTANT / momento;
        }

        #endregion

        #region Desenho

        private void DesenharGrid()
        {
            GridCanvas.Children.Clear();

            double canvasWidth = WaveFunctionCanvas.ActualWidth > 0 ? WaveFunctionCanvas.ActualWidth : 800;
            double canvasHeight = WaveFunctionCanvas.ActualHeight > 0 ? WaveFunctionCanvas.ActualHeight : 400;

            // Linhas horizontais
            for (int i = 0; i <= 4; i++)
            {
                double y = i * canvasHeight / 4;
                var line = new Line
                {
                    X1 = 0,
                    Y1 = y,
                    X2 = canvasWidth,
                    Y2 = y,
                    Stroke = new SolidColorBrush(Color.FromArgb(30, 255, 255, 255)),
                    StrokeThickness = 1
                };
                GridCanvas.Children.Add(line);
            }

            // Linhas verticais
            for (int i = 0; i <= 8; i++)
            {
                double x = i * canvasWidth / 8;
                var line = new Line
                {
                    X1 = x,
                    Y1 = 0,
                    X2 = x,
                    Y2 = canvasHeight,
                    Stroke = new SolidColorBrush(Color.FromArgb(30, 255, 255, 255)),
                    StrokeThickness = 1
                };
                GridCanvas.Children.Add(line);
            }

            // Eixo X
            var axisX = new Line
            {
                X1 = 50,
                Y1 = canvasHeight - 20,
                X2 = canvasWidth - 50,
                Y2 = canvasHeight - 20,
                Stroke = Brushes.White,
                StrokeThickness = 2
            };
            GridCanvas.Children.Add(axisX);
        }

        private void AtualizarVisualizacao()
        {
            // Limpar canvas
            PotentialCanvas.Children.Clear();
            ProbabilityCanvas.Children.Clear();
            RealPartCanvas.Children.Clear();
            ImaginaryPartCanvas.Children.Clear();

            double canvasWidth = WaveFunctionCanvas.ActualWidth > 0 ? WaveFunctionCanvas.ActualWidth : 800;
            double canvasHeight = WaveFunctionCanvas.ActualHeight > 0 ? WaveFunctionCanvas.ActualHeight : 400;

            // Desenhar potencial
            if (ChkMostrarPotencial.IsChecked == true)
            {
                DesenharPotencial(canvasWidth, canvasHeight);
            }

            // Calcular e desenhar função de onda
            DesenharFuncaoOnda(canvasWidth, canvasHeight);

            // Atualizar informações
            AtualizarInformacoes();
        }

        private void DesenharPotencial(double width, double height)
        {
            switch (potencialAtual)
            {
                case TipoPotencial.PocoInfinito:
                    DesenharPocoInfinito(width, height);
                    break;
                case TipoPotencial.OsciladorHarmonico:
                    DesenharParabola(width, height);
                    break;
                case TipoPotencial.Barreira:
                    DesenharBarreira(width, height);
                    break;
            }
        }

        private void DesenharPocoInfinito(double width, double height)
        {
            double xStart = 100;
            double xEnd = width - 100;
            double yBottom = height - 30;
            double yTop = 50;

            // Parede esquerda
            var leftWall = new Line
            {
                X1 = xStart,
                Y1 = yTop,
                X2 = xStart,
                Y2 = yBottom,
                Stroke = new SolidColorBrush(Color.FromRgb(243, 156, 18)),
                StrokeThickness = 4
            };
            PotentialCanvas.Children.Add(leftWall);

            // Parede direita
            var rightWall = new Line
            {
                X1 = xEnd,
                Y1 = yTop,
                X2 = xEnd,
                Y2 = yBottom,
                Stroke = new SolidColorBrush(Color.FromRgb(243, 156, 18)),
                StrokeThickness = 4
            };
            PotentialCanvas.Children.Add(rightWall);

            // Chão
            var floor = new Line
            {
                X1 = xStart,
                Y1 = yBottom,
                X2 = xEnd,
                Y2 = yBottom,
                Stroke = new SolidColorBrush(Color.FromRgb(243, 156, 18)),
                StrokeThickness = 2
            };
            PotentialCanvas.Children.Add(floor);
        }

        private void DesenharParabola(double width, double height)
        {
            var polyline = new Polyline
            {
                Stroke = new SolidColorBrush(Color.FromRgb(243, 156, 18)),
                StrokeThickness = 3
            };

            double xStart = 100;
            double xEnd = width - 100;
            double yCenter = height - 30;

            int numPontos = 100;
            for (int i = 0; i <= numPontos; i++)
            {
                double x = xStart + i * (xEnd - xStart) / numPontos;
                double xNorm = (x - width / 2) / (width / 4);
                double y = yCenter - 150 * xNorm * xNorm;
                polyline.Points.Add(new Point(x, y));
            }

            PotentialCanvas.Children.Add(polyline);
        }

        private void DesenharBarreira(double width, double height)
        {
            double xCenter = width / 2;
            double barrierWidth = 80;
            double barrierHeight = 150;

            var barrier = new Rectangle
            {
                Width = barrierWidth,
                Height = barrierHeight,
                Fill = new SolidColorBrush(Color.FromArgb(100, 243, 156, 18)),
                Stroke = new SolidColorBrush(Color.FromRgb(243, 156, 18)),
                StrokeThickness = 2
            };

            Canvas.SetLeft(barrier, xCenter - barrierWidth / 2);
            Canvas.SetTop(barrier, height - 30 - barrierHeight);
            PotentialCanvas.Children.Add(barrier);
        }

        private void DesenharFuncaoOnda(double width, double height)
        {
            int numPontos = 200;
            double xStart = 100;
            double xEnd = width - 100;
            double yCenter = height - 30;

            var psiReal = new Polyline
            {
                Stroke = new SolidColorBrush(Color.FromRgb(52, 152, 219)),
                StrokeThickness = 2
            };

            var psiProb = new Polygon
            {
                Fill = new SolidColorBrush(Color.FromArgb(100, 231, 76, 60)),
                Stroke = new SolidColorBrush(Color.FromRgb(231, 76, 60)),
                StrokeThickness = 2
            };

            double maxPsi = 0;

            // Calcular valores
            var valores = new List<double>();
            for (int i = 0; i <= numPontos; i++)
            {
                double x = i / (double)numPontos * larguraPoco;
                double psi = 0;

                switch (potencialAtual)
                {
                    case TipoPotencial.PocoInfinito:
                        psi = PsiPocoInfinito(x, numeroQuantico);
                        break;
                    case TipoPotencial.OsciladorHarmonico:
                        double omega = 1e15; // Frequência típica
                        psi = PsiOscilador(x, numeroQuantico, omega);
                        break;
                }

                if (animando)
                {
                    double fase = -CalcularEnergiaPocoInfinito(numeroQuantico) * tempo / REDUCED_PLANCK;
                    psi *= Math.Cos(fase);
                }

                valores.Add(psi);
                if (Math.Abs(psi) > maxPsi)
                    maxPsi = Math.Abs(psi);
            }

            // Normalizar e desenhar
            double escala = 100 / (maxPsi > 0 ? maxPsi : 1);

            for (int i = 0; i <= numPontos; i++)
            {
                double xCanvas = xStart + i * (xEnd - xStart) / numPontos;
                double psi = valores[i];

                // Parte real
                double yReal = yCenter - psi * escala;
                psiReal.Points.Add(new Point(xCanvas, yReal));

                // Densidade de probabilidade
                double prob = psi * psi;
                double yProb = yCenter - prob * escala;
                psiProb.Points.Add(new Point(xCanvas, yProb));
            }

            // Fechar polígono
            psiProb.Points.Add(new Point(xEnd, yCenter));
            psiProb.Points.Add(new Point(xStart, yCenter));

            if (ChkMostrarProbabilidade.IsChecked == true)
                ProbabilityCanvas.Children.Add(psiProb);

            if (ChkMostrarReal.IsChecked == true)
                RealPartCanvas.Children.Add(psiReal);

            // Desenhar nível de energia
            if (ChkMostrarNivelEnergia.IsChecked == true)
            {
                double energia = CalcularEnergiaPocoInfinito(numeroQuantico);
                double yEnergia = MapearEnergiaParaY(energia, height);

                Canvas.SetTop(EnergyLevelLine, yEnergia);
                Canvas.SetTop(EnergyLabel, yEnergia - 10);
                EnergyLevelLine.Visibility = Visibility.Visible;
                EnergyLabel.Visibility = Visibility.Visible;
            }
            else
            {
                EnergyLevelLine.Visibility = Visibility.Collapsed;
                EnergyLabel.Visibility = Visibility.Collapsed;
            }
        }

        private double MapearEnergiaParaY(double energia, double height)
        {
            double energiaMax = CalcularEnergiaPocoInfinito(10);
            return height - 30 - (energia / energiaMax) * (height - 80);
        }

        private void AtualizarInformacoes()
        {
            TxtQuantumNumber.Text = numeroQuantico.ToString();

            double energia = CalcularEnergiaPocoInfinito(numeroQuantico);
            double energiaEV = energia / ELECTRON_VOLT;
            TxtEnergy.Text = $"{energiaEV:F2} eV";

            double lambda = CalcularComprimentoOnda(energia);
            double lambdaNm = lambda * 1e9;
            TxtWavelength.Text = $"{lambdaNm:F2} nm";

            int nos = numeroQuantico - 1;
            TxtNodes.Text = nos.ToString();

            TxtEnergyLevel.Text = $"E_{numeroQuantico}";
        }

        private void DesenharPocotesEstaticos()
        {
            // Poço infinito estático
            DesenharPocoInfinitoEstatico(InfiniteWellCanvas);

            // Oscilador estático
            DesenharOsciladorEstatico(HarmonicCanvas);
        }

        private void DesenharPocoInfinitoEstatico(Canvas canvas)
        {
            if (canvas == null) return;
            canvas.Children.Clear();

            double width = canvas.ActualWidth > 0 ? canvas.ActualWidth : 350;
            double height = canvas.ActualHeight > 0 ? canvas.ActualHeight : 100;

            // Paredes
            var leftWall = new Line { X1 = 50, Y1 = 20, X2 = 50, Y2 = 80, Stroke = Brushes.Black, StrokeThickness = 3 };
            var rightWall = new Line { X1 = width - 50, Y1 = 20, X2 = width - 50, Y2 = 80, Stroke = Brushes.Black, StrokeThickness = 3 };
            var floor = new Line { X1 = 50, Y1 = 80, X2 = width - 50, Y2 = 80, Stroke = Brushes.Black, StrokeThickness = 2 };

            canvas.Children.Add(leftWall);
            canvas.Children.Add(rightWall);
            canvas.Children.Add(floor);

            // Função de onda n=2
            var wave = new Polyline { Stroke = Brushes.Red, StrokeThickness = 2 };
            for (int i = 0; i <= 50; i++)
            {
                double x = 50 + i * (width - 100) / 50;
                double psi = Math.Sin(2 * Math.PI * i / 50);
                double y = 50 - psi * 20;
                wave.Points.Add(new Point(x, y));
            }
            canvas.Children.Add(wave);
        }

        private void DesenharOsciladorEstatico(Canvas canvas)
        {
            if (canvas == null) return;
            canvas.Children.Clear();

            double width = canvas.ActualWidth > 0 ? canvas.ActualWidth : 350;
            double height = canvas.ActualHeight > 0 ? canvas.ActualHeight : 100;

            // Parábola
            var parabola = new Polyline { Stroke = Brushes.Black, StrokeThickness = 2 };
            for (int i = 0; i <= 50; i++)
            {
                double x = 50 + i * (width - 100) / 50;
                double xNorm = (i - 25) / 25.0;
                double y = 80 - 30 * xNorm * xNorm;
                parabola.Points.Add(new Point(x, y));
            }
            canvas.Children.Add(parabola);

            // Função Gaussiana
            var wave = new Polyline { Stroke = Brushes.Blue, StrokeThickness = 2 };
            for (int i = 0; i <= 50; i++)
            {
                double x = 50 + i * (width - 100) / 50;
                double xNorm = (i - 25) / 12.0;
                double psi = Math.Exp(-xNorm * xNorm / 2);
                double y = 50 - psi * 20;
                wave.Points.Add(new Point(x, y));
            }
            canvas.Children.Add(wave);
        }

        #endregion

        #region Animação

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            tempo += 0.01;
            AtualizarVisualizacao();
        }

        #endregion

        #region Eventos dos Controles

        private void SliderQuantumNumber_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!IsLoaded) return;

            numeroQuantico = (int)e.NewValue;

            string[] estados = { "Estado Fundamental", "Primeiro Excitado", "Segundo Excitado",
                               "Terceiro Excitado", "Quarto Excitado", "Quinto Excitado",
                               "Sexto Excitado", "Sétimo Excitado", "Oitavo Excitado", "Nono Excitado" };

            int index = Math.Max(0, Math.Min(estados.Length - 1, numeroQuantico - 1));
            TxtSliderN.Text = $"n = {numeroQuantico} ({estados[index]})";

            AtualizarVisualizacao();
        }

        private void AnimarEvolucao_Click(object sender, RoutedEventArgs e)
        {
            animando = true;
            tempo = 0;
            animationTimer.Start();
        }

        private void Pausar_Click(object sender, RoutedEventArgs e)
        {
            animando = false;
            animationTimer.Stop();
        }

        private void ChkVisualizacao_Changed(object sender, RoutedEventArgs e)
        {
            if (!IsLoaded) return;
            AtualizarVisualizacao();
        }

        #endregion

        #region Potenciais

        private void PocoinFinito_Click(object sender, RoutedEventArgs e)
        {
            potencialAtual = TipoPotencial.PocoInfinito;
            AtualizarVisualizacao();
        }

        private void PocoFinito_Click(object sender, RoutedEventArgs e)
        {
            potencialAtual = TipoPotencial.PocoFinito;
            MessageBox.Show(
                "Poço Finito\n\n" +
                "V(x) = 0 para |x| < L/2\n" +
                "V(x) = V₀ caso contrário\n\n" +
                "Permite tunelamento quântico!\n" +
                "ψ ≠ 0 fora do poço.",
                "Poço Finito",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void OsciladorHarmonico_Click(object sender, RoutedEventArgs e)
        {
            potencialAtual = TipoPotencial.OsciladorHarmonico;
            AtualizarVisualizacao();
        }

        private void Barreira_Click(object sender, RoutedEventArgs e)
        {
            potencialAtual = TipoPotencial.Barreira;
            AtualizarVisualizacao();
        }

        private void ParticulaLivre_Click(object sender, RoutedEventArgs e)
        {
            potencialAtual = TipoPotencial.ParticulaLivre;
            MessageBox.Show(
                "Partícula Livre\n\n" +
                "V(x) = 0 para todo x\n\n" +
                "ψ(x) = e^(ikx) (onda plana)\n" +
                "Energia contínua, não quantizada!",
                "Partícula Livre",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        #endregion
    }
}