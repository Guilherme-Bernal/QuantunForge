using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Quantun_Forge.src.views.FisicaQuantica
{
    /// <summary>
    /// Simulador Visual de Espectros Atômicos e Transições Eletrônicas
    /// </summary>
    public partial class EspectroAto : UserControl
    {
        #region Constantes Físicas

        private const double PLANCK_CONSTANT = 6.62607015e-34;      // J·s
        private const double SPEED_OF_LIGHT = 299792458;            // m/s
        private const double ELECTRON_VOLT = 1.602176634e-19;       // J
        private const double RYDBERG_CONSTANT = 1.0973731568160e7;  // m⁻¹
        private const double ENERGIA_FUNDAMENTAL_H = -13.6;         // eV

        #endregion

        #region Posições dos Níveis no Canvas

        private readonly Dictionary<int, double> posYNiveis = new Dictionary<int, double>
        {
            { 1, 380 },
            { 2, 310 },
            { 3, 260 },
            { 4, 225 },
            { 5, 200 },
            { 6, 180 }
        };

        private readonly Dictionary<int, Color> coresNiveis = new Dictionary<int, Color>
        {
            { 1, Color.FromRgb(231, 76, 60) },   // Vermelho
            { 2, Color.FromRgb(230, 126, 34) },  // Laranja
            { 3, Color.FromRgb(243, 156, 18) },  // Amarelo
            { 4, Color.FromRgb(241, 196, 15) },  // Amarelo claro
            { 5, Color.FromRgb(46, 204, 113) },  // Verde
            { 6, Color.FromRgb(52, 152, 219) }   // Azul
        };

        #endregion

        #region Variáveis de Estado

        private int nivelAtual = 2;

        #endregion

        public EspectroAto()
        {
            InitializeComponent();
            Loaded += EspectroAto_Loaded;
        }

        private void EspectroAto_Loaded(object sender, RoutedEventArgs e)
        {
            InicializarSimulador();
        }

        #region Inicialização

        private void InicializarSimulador()
        {
            // Desenhar espectro de Balmer
            DesenharSerieBalmer();

            // Desenhar linhas espectrais no espectro completo
            DesenharLinhasEspectrais();

            // Posicionar elétron inicial
            Canvas.SetTop(Electron, posYNiveis[nivelAtual] - 8);
        }

        #endregion

        #region Cálculos Físicos

        /// <summary>
        /// Calcula a energia de um nível
        /// E_n = -13.6 / n²
        /// </summary>
        private double CalcularEnergiaNivel(int n)
        {
            return ENERGIA_FUNDAMENTAL_H / (n * n);
        }

        /// <summary>
        /// Calcula a diferença de energia em uma transição
        /// ΔE = E_f - E_i
        /// </summary>
        private double CalcularDeltaE(int ni, int nf)
        {
            return CalcularEnergiaNivel(nf) - CalcularEnergiaNivel(ni);
        }

        /// <summary>
        /// Calcula o comprimento de onda usando a fórmula de Rydberg
        /// 1/λ = R∞ (1/n₁² - 1/n₂²)
        /// </summary>
        private double CalcularComprimentoOnda(int n1, int n2)
        {
            // n2 > n1 (transição de maior para menor)
            if (n2 < n1)
            {
                int temp = n1;
                n1 = n2;
                n2 = temp;
            }

            double inversoLambda = RYDBERG_CONSTANT * (1.0 / (n1 * n1) - 1.0 / (n2 * n2));
            return 1.0 / inversoLambda; // em metros
        }

        /// <summary>
        /// Calcula a frequência do fóton
        /// f = c / λ
        /// </summary>
        private double CalcularFrequencia(double lambda)
        {
            return SPEED_OF_LIGHT / lambda;
        }

        /// <summary>
        /// Determina a região do espectro baseado no comprimento de onda
        /// </summary>
        private string DeterminarRegiaoEspectro(double lambdaNm)
        {
            if (lambdaNm < 380)
                return "Ultravioleta (UV)";
            else if (lambdaNm >= 380 && lambdaNm < 450)
                return "Violeta";
            else if (lambdaNm >= 450 && lambdaNm < 495)
                return "Azul";
            else if (lambdaNm >= 495 && lambdaNm < 570)
                return "Verde";
            else if (lambdaNm >= 570 && lambdaNm < 590)
                return "Amarelo";
            else if (lambdaNm >= 590 && lambdaNm < 620)
                return "Laranja";
            else if (lambdaNm >= 620 && lambdaNm < 750)
                return "Vermelho";
            else
                return "Infravermelho (IR)";
        }

        /// <summary>
        /// Converte comprimento de onda para cor RGB
        /// </summary>
        private Color LambdaParaCor(double lambdaNm)
        {
            double r = 0, g = 0, b = 0;

            if (lambdaNm >= 380 && lambdaNm < 440)
            {
                r = -(lambdaNm - 440) / (440 - 380);
                g = 0;
                b = 1;
            }
            else if (lambdaNm >= 440 && lambdaNm < 490)
            {
                r = 0;
                g = (lambdaNm - 440) / (490 - 440);
                b = 1;
            }
            else if (lambdaNm >= 490 && lambdaNm < 510)
            {
                r = 0;
                g = 1;
                b = -(lambdaNm - 510) / (510 - 490);
            }
            else if (lambdaNm >= 510 && lambdaNm < 580)
            {
                r = (lambdaNm - 510) / (580 - 510);
                g = 1;
                b = 0;
            }
            else if (lambdaNm >= 580 && lambdaNm < 645)
            {
                r = 1;
                g = -(lambdaNm - 645) / (645 - 580);
                b = 0;
            }
            else if (lambdaNm >= 645 && lambdaNm <= 750)
            {
                r = 1;
                g = 0;
                b = 0;
            }

            // Ajuste de intensidade
            double factor = 1.0;
            if (lambdaNm >= 380 && lambdaNm < 420)
                factor = 0.3 + 0.7 * (lambdaNm - 380) / (420 - 380);
            else if (lambdaNm >= 700 && lambdaNm <= 750)
                factor = 0.3 + 0.7 * (750 - lambdaNm) / (750 - 700);

            byte R = (byte)(255 * r * factor);
            byte G = (byte)(255 * g * factor);
            byte B = (byte)(255 * b * factor);

            return Color.FromRgb(R, G, B);
        }

        #endregion

        #region Transições

        private void RealizarTransicao_Click(object sender, RoutedEventArgs e)
        {
            int ni = int.Parse((CboNivelInicial.SelectedItem as ComboBoxItem).Tag.ToString());
            int nf = int.Parse((CboNivelFinal.SelectedItem as ComboBoxItem).Tag.ToString());

            if (ni == nf)
            {
                MessageBox.Show(
                    "Os níveis inicial e final devem ser diferentes!",
                    "Transição Inválida",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            RealizarTransicao(ni, nf);
        }

        private void RealizarTransicao(int ni, int nf)
        {
            // Calcular parâmetros da transição
            double deltaE = CalcularDeltaE(ni, nf);
            double lambda = CalcularComprimentoOnda(Math.Min(ni, nf), Math.Max(ni, nf));
            double lambdaNm = lambda * 1e9; // Converter para nm
            double frequencia = CalcularFrequencia(lambda);

            // Atualizar informações
            TxtTransicao.Text = $"n={ni} → n={nf}";
            TxtDeltaE.Text = $"{Math.Abs(deltaE):F2} eV";
            TxtLambda.Text = $"{lambdaNm:F1} nm";

            TxtEnergiaInicial.Text = $"{CalcularEnergiaNivel(ni):F2} eV";
            TxtEnergiaFinal.Text = $"{CalcularEnergiaNivel(nf):F2} eV";
            TxtFrequencia.Text = $"{frequencia:E2} Hz";
            TxtRegiaoEspectro.Text = DeterminarRegiaoEspectro(lambdaNm);

            // Animar transição
            AnimarEletron(ni, nf);

            bool emiteFoton = nf < ni;
            if (emiteFoton)
            {
                DesenharSetaTransicao(ni, nf);
                EmitirFoton(ni, nf, lambdaNm);
            }
            else
            {
                DesenharSetaAbsorcao(ni, nf);
            }

            nivelAtual = nf;
        }

        #endregion

        #region Animações

        private void AnimarEletron(int ni, int nf)
        {
            double yInicial = posYNiveis[ni] - 8;
            double yFinal = posYNiveis[nf] - 8;

            var anim = new DoubleAnimation
            {
                From = yInicial,
                To = yFinal,
                Duration = TimeSpan.FromSeconds(1),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };

            Electron.BeginAnimation(Canvas.TopProperty, anim);
        }

        private void DesenharSetaTransicao(int ni, int nf)
        {
            TransitionArrowsCanvas.Children.Clear();

            double y1 = posYNiveis[ni];
            double y2 = posYNiveis[nf];

            // Linha da seta
            var line = new Line
            {
                X1 = 200,
                Y1 = y1,
                X2 = 200,
                Y2 = y2,
                Stroke = new SolidColorBrush(Color.FromRgb(255, 215, 0)),
                StrokeThickness = 3
            };

            // Ponta da seta
            var arrow = new Polygon
            {
                Points = new PointCollection
                {
                    new Point(200, y2),
                    new Point(195, y2 + 10),
                    new Point(205, y2 + 10)
                },
                Fill = new SolidColorBrush(Color.FromRgb(255, 215, 0))
            };

            TransitionArrowsCanvas.Children.Add(line);
            TransitionArrowsCanvas.Children.Add(arrow);

            // Label
            var label = new TextBlock
            {
                Text = "📉 Emissão",
                Foreground = new SolidColorBrush(Color.FromRgb(255, 215, 0)),
                FontSize = 10,
                FontWeight = FontWeights.Bold
            };
            Canvas.SetLeft(label, 210);
            Canvas.SetTop(label, (y1 + y2) / 2 - 10);
            TransitionArrowsCanvas.Children.Add(label);
        }

        private void DesenharSetaAbsorcao(int ni, int nf)
        {
            TransitionArrowsCanvas.Children.Clear();

            double y1 = posYNiveis[ni];
            double y2 = posYNiveis[nf];

            // Linha da seta
            var line = new Line
            {
                X1 = 200,
                Y1 = y1,
                X2 = 200,
                Y2 = y2,
                Stroke = new SolidColorBrush(Color.FromRgb(0, 217, 255)),
                StrokeThickness = 3,
                StrokeDashArray = new DoubleCollection { 5, 3 }
            };

            // Ponta da seta
            var arrow = new Polygon
            {
                Points = new PointCollection
                {
                    new Point(200, y2),
                    new Point(195, y2 + 10),
                    new Point(205, y2 + 10)
                },
                Fill = new SolidColorBrush(Color.FromRgb(0, 217, 255))
            };

            TransitionArrowsCanvas.Children.Add(line);
            TransitionArrowsCanvas.Children.Add(arrow);

            // Label
            var label = new TextBlock
            {
                Text = "📈 Absorção",
                Foreground = new SolidColorBrush(Color.FromRgb(0, 217, 255)),
                FontSize = 10,
                FontWeight = FontWeights.Bold
            };
            Canvas.SetLeft(label, 210);
            Canvas.SetTop(label, (y1 + y2) / 2 - 10);
            TransitionArrowsCanvas.Children.Add(label);
        }

        private void EmitirFoton(int ni, int nf, double lambdaNm)
        {
            Color corFoton = LambdaParaCor(lambdaNm);

            // Criar fóton
            var foton = new Ellipse
            {
                Width = 12,
                Height = 12,
                Fill = new SolidColorBrush(corFoton),
                Stroke = Brushes.White,
                StrokeThickness = 2
            };

            foton.Effect = new System.Windows.Media.Effects.DropShadowEffect
            {
                Color = corFoton,
                BlurRadius = 15,
                ShadowDepth = 0
            };

            double startY = (posYNiveis[ni] + posYNiveis[nf]) / 2;
            Canvas.SetLeft(foton, 200);
            Canvas.SetTop(foton, startY);

            PhotonsCanvas.Children.Add(foton);

            // Animar fóton
            var animX = new DoubleAnimation
            {
                From = 200,
                To = 450,
                Duration = TimeSpan.FromSeconds(1.5)
            };

            var animOpacity = new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = TimeSpan.FromSeconds(1.5)
            };

            animX.Completed += (s, e) =>
            {
                PhotonsCanvas.Children.Remove(foton);
            };

            foton.BeginAnimation(Canvas.LeftProperty, animX);
            foton.BeginAnimation(OpacityProperty, animOpacity);
        }

        #endregion

        #region Desenho de Espectros

        private void DesenharSerieBalmer()
        {
            BalmerSeriesCanvas.Children.Clear();

            // Transições de Balmer: n → 2
            var transicoes = new List<(int n, double lambda, string nome)>
            {
                (3, 656.3, "Hα"),  // Vermelho
                (4, 486.1, "Hβ"),  // Azul
                (5, 434.0, "Hγ"),  // Violeta
                (6, 410.2, "Hδ")   // Violeta
            };

            double width = 320;
            int index = 0;

            foreach (var (n, lambda, nome) in transicoes)
            {
                Color cor = LambdaParaCor(lambda);

                double x = 10 + (index * width / transicoes.Count);
                double largura = width / transicoes.Count - 5;

                var linha = new Rectangle
                {
                    Width = largura,
                    Height = 35,
                    Fill = new SolidColorBrush(cor)
                };

                Canvas.SetLeft(linha, x);
                Canvas.SetTop(linha, 0);

                BalmerSeriesCanvas.Children.Add(linha);

                // Label
                var label = new TextBlock
                {
                    Text = nome,
                    Foreground = Brushes.White,
                    FontSize = 9,
                    FontWeight = FontWeights.Bold
                };
                Canvas.SetLeft(label, x + largura / 2 - 10);
                Canvas.SetTop(label, 10);
                BalmerSeriesCanvas.Children.Add(label);

                index++;
            }
        }

        private void DesenharLinhasEspectrais()
        {
            SpectralLinesCanvas.Children.Clear();

            double canvasWidth = SpectrumCanvas.ActualWidth > 0 ? SpectrumCanvas.ActualWidth : 800;

            // Série de Lyman (UV) - linhas no início
            DesenharLinhaEspectral(121.6, canvasWidth, Brushes.Purple);
            DesenharLinhaEspectral(102.6, canvasWidth, Brushes.Purple);
            DesenharLinhaEspectral(97.3, canvasWidth, Brushes.Purple);

            // Série de Balmer (Visível)
            DesenharLinhaEspectral(656.3, canvasWidth, Brushes.Red);
            DesenharLinhaEspectral(486.1, canvasWidth, Brushes.Blue);
            DesenharLinhaEspectral(434.0, canvasWidth, Brushes.Violet);
            DesenharLinhaEspectral(410.2, canvasWidth, Brushes.Violet);

            // Série de Paschen (IR) - linhas no final
            DesenharLinhaEspectral(1875, canvasWidth, Brushes.DarkRed);
            DesenharLinhaEspectral(1282, canvasWidth, Brushes.DarkRed);
            DesenharLinhaEspectral(1094, canvasWidth, Brushes.DarkRed);
        }

        private void DesenharLinhaEspectral(double lambdaNm, double canvasWidth, Brush cor)
        {
            // Mapear lambda para posição X no canvas
            double x;

            if (lambdaNm < 380) // UV
            {
                x = (lambdaNm / 380) * (canvasWidth * 0.15);
            }
            else if (lambdaNm >= 380 && lambdaNm <= 750) // Visível
            {
                x = 0.15 * canvasWidth + ((lambdaNm - 380) / (750 - 380)) * (canvasWidth * 0.55);
            }
            else // IR
            {
                x = 0.70 * canvasWidth + ((lambdaNm - 750) / 2000) * (canvasWidth * 0.30);
            }

            var linha = new Line
            {
                X1 = x,
                Y1 = 20,
                X2 = x,
                Y2 = 80,
                Stroke = cor,
                StrokeThickness = 3
            };

            SpectralLinesCanvas.Children.Add(linha);
        }

        #endregion

        #region Eventos dos Controles

        private void CboNivelInicial_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Atualização automática quando necessário
        }

        private void CboNivelFinal_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Atualização automática quando necessário
        }

        private void Resetar_Click(object sender, RoutedEventArgs e)
        {
            // Resetar para estado inicial
            CboNivelInicial.SelectedIndex = 1; // n=2
            CboNivelFinal.SelectedIndex = 0;   // n=1

            nivelAtual = 2;
            Canvas.SetTop(Electron, posYNiveis[2] - 8);

            TransitionArrowsCanvas.Children.Clear();
            PhotonsCanvas.Children.Clear();

            TxtTransicao.Text = "n=1 → n=1";
            TxtDeltaE.Text = "0.00 eV";
            TxtLambda.Text = "--- nm";
            TxtEnergiaInicial.Text = "0.00 eV";
            TxtEnergiaFinal.Text = "0.00 eV";
            TxtFrequencia.Text = "0.00 Hz";
            TxtRegiaoEspectro.Text = "---";
        }

        #endregion

        #region Séries Espectrais

        private void SerieLyman_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "Série de Lyman (UV)\n\n" +
                "Transições: n → 1\n" +
                "Região: Ultravioleta (91-122 nm)\n\n" +
                "Exemplo: Lyman-α (2→1): 121.6 nm\n\n" +
                "Descoberta por Theodore Lyman em 1906.",
                "Série de Lyman",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void SerieBalmer_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "Série de Balmer (Visível)\n\n" +
                "Transições: n → 2\n" +
                "Região: Luz Visível (365-656 nm)\n\n" +
                "Hα (3→2): 656.3 nm (Vermelho)\n" +
                "Hβ (4→2): 486.1 nm (Azul)\n" +
                "Hγ (5→2): 434.0 nm (Violeta)\n" +
                "Hδ (6→2): 410.2 nm (Violeta)\n\n" +
                "Descoberta por Johann Balmer em 1885.",
                "Série de Balmer",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void SeriePaschen_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "Série de Paschen (IR)\n\n" +
                "Transições: n → 3\n" +
                "Região: Infravermelho (820-1875 nm)\n\n" +
                "Exemplo: Pα (4→3): 1875 nm\n\n" +
                "Descoberta por Friedrich Paschen em 1908.",
                "Série de Paschen",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void SerieBrackett_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "Série de Brackett (IR)\n\n" +
                "Transições: n → 4\n" +
                "Região: Infravermelho (1.46-4.05 μm)\n\n" +
                "Descoberta por Frederick Sumner Brackett em 1922.",
                "Série de Brackett",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        #endregion

        #region Transições Famosas

        private void TransicaoHAlpha_Click(object sender, RoutedEventArgs e)
        {
            CboNivelInicial.SelectedIndex = 2; // n=3
            CboNivelFinal.SelectedIndex = 1;   // n=2
            RealizarTransicao(3, 2);
        }

        private void TransicaoHBeta_Click(object sender, RoutedEventArgs e)
        {
            CboNivelInicial.SelectedIndex = 3; // n=4
            CboNivelFinal.SelectedIndex = 1;   // n=2
            RealizarTransicao(4, 2);
        }

        private void TransicaoHGamma_Click(object sender, RoutedEventArgs e)
        {
            CboNivelInicial.SelectedIndex = 4; // n=5
            CboNivelFinal.SelectedIndex = 1;   // n=2
            RealizarTransicao(5, 2);
        }

        private void TransicaoHDelta_Click(object sender, RoutedEventArgs e)
        {
            CboNivelInicial.SelectedIndex = 5; // n=6
            CboNivelFinal.SelectedIndex = 1;   // n=2
            RealizarTransicao(6, 2);
        }

        private void TransicaoLymanAlpha_Click(object sender, RoutedEventArgs e)
        {
            CboNivelInicial.SelectedIndex = 1; // n=2
            CboNivelFinal.SelectedIndex = 0;   // n=1
            RealizarTransicao(2, 1);
        }

        #endregion
    }
}