using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Threading;

namespace Quantun_Forge.src.views.FisicaClassica
{
    public partial class MaxwellControl : UserControl
    {
        // Timer para animação da onda eletromagnética
        private DispatcherTimer timerOnda;
        private double fase = 0;
        private double frequencia = 1.0;
        private double amplitude = 60;
        private double velocidadeAnimacao = 3;
        private bool ondaRodando = false;

        // Linhas de campo elétrico para Lei de Gauss
        private List<Line> linhasCampoEletrico = new();
        private double cargaAtual = 5.0; // em μC
        private int numeroLinhas = 16;

        public MaxwellControl()
        {
            InitializeComponent();
            Loaded += MaxwellControl_Loaded;
        }

        private void MaxwellControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Inicializa timer da onda eletromagnética
            timerOnda = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(16) // ~60 FPS
            };
            timerOnda.Tick += AnimarOndaEletromagnetica;

            // Desenha a onda inicial
            DesenharOndaInicial();

            // Cria linhas de campo elétrico
            CriarLinhasCampoEletrico();

            // Atualiza labels iniciais
            AtualizarLabels();
        }

        #region Onda Eletromagnética

        private void DesenharOndaInicial()
        {
            AtualizarOnda(0);
        }

        private void AnimarOndaEletromagnetica(object? sender, EventArgs e)
        {
            fase += 0.05 * velocidadeAnimacao;
            AtualizarOnda(fase);
        }

        private void AtualizarOnda(double f)
        {
            if (OndaEletrica == null || OndaMagnetica == null) return;

            int numPontos = 200;
            double largura = 800;
            double centro = 175;

            // Gera pontos para a onda elétrica (vertical)
            PointCollection pontosE = new PointCollection();
            for (int i = 0; i < numPontos; i++)
            {
                double x = (i / (double)numPontos) * largura;
                double angulo = (x / largura) * 2 * Math.PI * frequencia - f;
                double y = centro - Math.Sin(angulo) * amplitude;
                pontosE.Add(new Point(x, y));
            }
            OndaEletrica.Points = pontosE;

            // Gera pontos para a onda magnética (perpendicular, deslocada 90°)
            PointCollection pontosB = new PointCollection();
            for (int i = 0; i < numPontos; i++)
            {
                double x = (i / (double)numPontos) * largura;
                double angulo = (x / largura) * 2 * Math.PI * frequencia - f;
                double y = centro - Math.Cos(angulo) * amplitude * 0.7; // B tem menor amplitude visual
                pontosB.Add(new Point(x, y));
            }
            OndaMagnetica.Points = pontosB;

            // Atualiza vetores dinâmicos no centro
            if (VetorE != null && VetorB != null)
            {
                double anguloAtual = Math.PI * frequencia - f;
                double yE = centro - Math.Sin(anguloAtual) * amplitude;
                double yB = centro;
                double xB = 400 + Math.Cos(anguloAtual) * amplitude * 0.7;

                VetorE.X1 = 400;
                VetorE.Y1 = centro;
                VetorE.X2 = 400;
                VetorE.Y2 = yE;

                VetorB.X1 = 400;
                VetorB.Y1 = centro;
                VetorB.X2 = xB;
                VetorB.Y2 = centro;
            }
        }

        private void BtnIniciarOnda_Click(object sender, RoutedEventArgs e)
        {
            if (timerOnda == null || ondaRodando) return;

            timerOnda.Start();
            ondaRodando = true;

            if (BtnIniciarOnda != null) BtnIniciarOnda.IsEnabled = false;
            if (BtnPausarOnda != null) BtnPausarOnda.IsEnabled = true;
        }

        private void BtnPausarOnda_Click(object sender, RoutedEventArgs e)
        {
            if (timerOnda == null || !ondaRodando) return;

            timerOnda.Stop();
            ondaRodando = false;

            if (BtnIniciarOnda != null) BtnIniciarOnda.IsEnabled = true;
            if (BtnPausarOnda != null) BtnPausarOnda.IsEnabled = false;
        }

        private void BtnResetarOnda_Click(object sender, RoutedEventArgs e)
        {
            if (timerOnda != null)
            {
                timerOnda.Stop();
            }
            ondaRodando = false;
            fase = 0;

            if (BtnIniciarOnda != null) BtnIniciarOnda.IsEnabled = true;
            if (BtnPausarOnda != null) BtnPausarOnda.IsEnabled = false;

            // Reseta sliders
            if (SliderFrequencia != null) SliderFrequencia.Value = 1.0;
            if (SliderAmplitude != null) SliderAmplitude.Value = 60;
            if (SliderVelocidade != null) SliderVelocidade.Value = 3;

            if (OndaEletrica != null)
            {
                DesenharOndaInicial();
            }
        }

        private void SliderFrequencia_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (SliderFrequencia == null || TxtFrequenciaSlider == null) return;

            frequencia = SliderFrequencia.Value;
            TxtFrequenciaSlider.Text = $"{frequencia:F1} Hz";

            // Atualiza informações
            if (TxtFrequencia != null)
            {
                TxtFrequencia.Text = $"Frequência: {frequencia:F1} Hz";
            }

            // Calcula comprimento de onda: λ = c/f
            double lambda = 3e8 / frequencia;
            if (TxtComprimento != null)
            {
                if (lambda >= 1e6)
                    TxtComprimento.Text = $"λ = {lambda / 1e6:F1}×10⁶ m";
                else if (lambda >= 1e3)
                    TxtComprimento.Text = $"λ = {lambda / 1e3:F1}×10³ m";
                else
                    TxtComprimento.Text = $"λ = {lambda:F1} m";
            }

            if (!ondaRodando && OndaEletrica != null)
            {
                DesenharOndaInicial();
            }
        }

        private void SliderAmplitude_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (SliderAmplitude == null || TxtAmplitudeSlider == null) return;

            amplitude = SliderAmplitude.Value;
            TxtAmplitudeSlider.Text = $"{amplitude:F0}";

            if (!ondaRodando && OndaEletrica != null)
            {
                DesenharOndaInicial();
            }
        }

        private void SliderVelocidade_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (SliderVelocidade == null || TxtVelocidadeSlider == null) return;

            velocidadeAnimacao = SliderVelocidade.Value;
            TxtVelocidadeSlider.Text = $"{velocidadeAnimacao:F0}x";
        }

        #endregion

        #region Campo Elétrico (Lei de Gauss)

        private void CriarLinhasCampoEletrico()
        {
            if (CanvasCampoEletrico == null || CargaCentral == null) return;

            // Limpa linhas anteriores
            var linhasParaRemover = new List<UIElement>();
            foreach (UIElement elemento in CanvasCampoEletrico.Children)
            {
                if (elemento is Line || (elemento is Polygon && elemento != CargaCentral))
                {
                    linhasParaRemover.Add(elemento);
                }
            }
            foreach (var linha in linhasParaRemover)
            {
                CanvasCampoEletrico.Children.Remove(linha);
            }
            linhasCampoEletrico.Clear();

            // Centro da carga
            double centroX = 400;
            double centroY = 150;
            double raioInicial = 25; // Começa após a carga
            double raioFinal = 150;   // Termina na borda

            // Cria linhas radiais
            for (int i = 0; i < numeroLinhas; i++)
            {
                double angulo = (2 * Math.PI * i) / numeroLinhas;

                double x1 = centroX + raioInicial * Math.Cos(angulo);
                double y1 = centroY + raioInicial * Math.Sin(angulo);
                double x2 = centroX + raioFinal * Math.Cos(angulo);
                double y2 = centroY + raioFinal * Math.Sin(angulo);

                Line linha = new Line
                {
                    X1 = x1,
                    Y1 = y1,
                    X2 = x2,
                    Y2 = y2,
                    StrokeThickness = 2,
                    Opacity = 0.7
                };

                // Define cor e direção baseado na carga
                if (cargaAtual > 0)
                {
                    // Carga positiva: linhas saindo (azul)
                    linha.Stroke = new SolidColorBrush(Color.FromRgb(52, 152, 219));

                    // Adiciona seta na ponta
                    Polygon seta = new Polygon
                    {
                        Fill = new SolidColorBrush(Color.FromRgb(52, 152, 219)),
                        Points = new PointCollection
                        {
                            new Point(x2, y2),
                            new Point(x2 - 8 * Math.Cos(angulo - 0.3), y2 - 8 * Math.Sin(angulo - 0.3)),
                            new Point(x2 - 8 * Math.Cos(angulo + 0.3), y2 - 8 * Math.Sin(angulo + 0.3))
                        },
                        Opacity = 0.7
                    };
                    CanvasCampoEletrico.Children.Add(seta);
                }
                else if (cargaAtual < 0)
                {
                    // Carga negativa: linhas entrando (vermelho)
                    linha.Stroke = new SolidColorBrush(Color.FromRgb(231, 76, 60));

                    // Adiciona seta na base (invertida)
                    Polygon seta = new Polygon
                    {
                        Fill = new SolidColorBrush(Color.FromRgb(231, 76, 60)),
                        Points = new PointCollection
                        {
                            new Point(x1, y1),
                            new Point(x1 + 8 * Math.Cos(angulo - 0.3), y1 + 8 * Math.Sin(angulo - 0.3)),
                            new Point(x1 + 8 * Math.Cos(angulo + 0.3), y1 + 8 * Math.Sin(angulo + 0.3))
                        },
                        Opacity = 0.7
                    };
                    CanvasCampoEletrico.Children.Add(seta);
                }

                CanvasCampoEletrico.Children.Add(linha);
                linhasCampoEletrico.Add(linha);
            }

            // Atualiza a carga visual
            if (cargaAtual > 0)
            {
                CargaCentral.Fill = new SolidColorBrush(Color.FromRgb(52, 152, 219));
                CargaCentral.Stroke = new SolidColorBrush(Color.FromRgb(41, 128, 185));
            }
            else if (cargaAtual < 0)
            {
                CargaCentral.Fill = new SolidColorBrush(Color.FromRgb(231, 76, 60));
                CargaCentral.Stroke = new SolidColorBrush(Color.FromRgb(192, 57, 43));
            }

            // Atualiza o símbolo no XAML
            // O TextBlock com + ou - está definido diretamente no XAML, precisaríamos de um x:Name para acessá-lo
        }

        private void SliderCarga_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (SliderCarga == null || TxtCargaSlider == null) return;

            cargaAtual = SliderCarga.Value;
            string sinal = cargaAtual >= 0 ? "+" : "";
            TxtCargaSlider.Text = $"{sinal}{cargaAtual:F0} μC";

            if (CanvasCampoEletrico != null)
            {
                CriarLinhasCampoEletrico();
                AtualizarInfoGauss();
            }
        }

        private void SliderLinhas_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (SliderLinhas == null || TxtLinhasSlider == null) return;

            numeroLinhas = (int)SliderLinhas.Value;
            TxtLinhasSlider.Text = $"{numeroLinhas} linhas";

            if (CanvasCampoEletrico != null)
            {
                CriarLinhasCampoEletrico();
            }
        }

        private void BtnAtualizarCampo_Click(object sender, RoutedEventArgs e)
        {
            if (CanvasCampoEletrico != null)
            {
                CriarLinhasCampoEletrico();
                AtualizarInfoGauss();
            }
        }

        private void AtualizarInfoGauss()
        {
            if (TxtInfoGauss == null) return;

            double fluxo = Math.Abs(cargaAtual) / 8.85; // Q/ε₀ simplificado
            string direcao = cargaAtual > 0 ? "saem radialmente" : "entram radialmente";
            string tipo = cargaAtual > 0 ? "positiva" : "negativa";

            TxtInfoGauss.Text = $"Carga {tipo} de {Math.Abs(cargaAtual):F0} μC. " +
                               $"As linhas de campo {direcao} da carga. " +
                               $"O fluxo elétrico através da superfície gaussiana é Φ = {fluxo:F2} unidades. " +
                               $"Pela Lei de Gauss: ∮E⃗·dA⃗ = Q/ε₀";
        }

        #endregion

        #region Métodos Auxiliares

        private void AtualizarLabels()
        {
            // Atualiza todos os labels iniciais
            SliderFrequencia_ValueChanged(null, null);
            SliderAmplitude_ValueChanged(null, null);
            SliderVelocidade_ValueChanged(null, null);
            SliderCarga_ValueChanged(null, null);
            SliderLinhas_ValueChanged(null, null);
        }

        #endregion
    }
}