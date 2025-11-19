using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Quantun_Forge.src.views.FisicaClassica
{
    public partial class OsciladorHarmonicoControl : UserControl
    {
        private DispatcherTimer timer;
        private double tempo = 0;
        private double massa = 1.0;
        private double constanteMola = 10.0;
        private double amplitude = 0.5;
        private double posicaoInicial = 0.5;
        private double velocidadeInicial = 0;
        private double velocidadeAnimacao = 1.0;

        private double frequenciaAngular;
        private double periodo;
        private double frequencia;

        private bool simulacaoAtiva = false;
        private bool simulacaoPausada = false;

        private const double PI = Math.PI;
        private const double escalaPixels = 80; // pixels por metro

        public OsciladorHarmonicoControl()
        {
            InitializeComponent();

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(16); // ~60 FPS
            timer.Tick += Timer_Tick;

            CalcularPropriedades();
            AtualizarLabels();
        }

        private void Sliders_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!IsLoaded) return;

            massa = SliderMassa.Value;
            constanteMola = SliderConstanteMola.Value;
            amplitude = SliderAmplitude.Value;
            posicaoInicial = SliderPosicaoInicial.Value;
            velocidadeInicial = SliderVelocidadeInicial.Value;

            CalcularPropriedades();
            AtualizarLabels();

            if (!simulacaoAtiva)
            {
                AtualizarPosicaoInicial();
            }
        }

        private void SliderVelocidadeAnimacao_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!IsLoaded) return;
            velocidadeAnimacao = SliderVelocidadeAnimacao.Value;
            TxtVelocidadeAnimacaoSlider.Text = $"{velocidadeAnimacao:F1}x";
        }

        private void CalcularPropriedades()
        {
            // ω = √(k/m)
            frequenciaAngular = Math.Sqrt(constanteMola / massa);

            // T = 2π/ω
            periodo = (2 * PI) / frequenciaAngular;

            // f = 1/T
            frequencia = 1 / periodo;
        }

        private void AtualizarLabels()
        {
            TxtMassaSlider.Text = $"{massa:F1} kg";
            TxtConstanteMolaSlider.Text = $"{constanteMola:F0} N/m";
            TxtAmplitudeSlider.Text = $"{amplitude:F2} m";
            TxtPosicaoInicialSlider.Text = $"{posicaoInicial:F2} m";
            TxtVelocidadeInicialSlider.Text = $"{velocidadeInicial:F1} m/s";

            TxtPeriodoInfo.Text = $"Período: {periodo:F3} s";
            TxtFrequenciaInfo.Text = $"Frequência: {frequencia:F3} Hz";
            TxtFrequenciaAngularInfo.Text = $"ω: {frequenciaAngular:F3} rad/s";

            TxtPeriodoResultado.Text = $"T = {periodo:F3} s";
            TxtFrequenciaResultado.Text = $"f = {frequencia:F3} Hz";

            AtualizarCalculos();
        }

        private void AtualizarCalculos()
        {
            TxtCalculos.Text = $"📐 Cálculos do Sistema:\n\n" +
                              $"Frequência Angular:\n" +
                              $"   ω = √(k/m)\n" +
                              $"   ω = √({constanteMola}/{massa})\n" +
                              $"   ω = {frequenciaAngular:F3} rad/s\n\n" +
                              $"Período:\n" +
                              $"   T = 2π/ω\n" +
                              $"   T = 2π/{frequenciaAngular:F3}\n" +
                              $"   T = {periodo:F3} s\n\n" +
                              $"Frequência:\n" +
                              $"   f = 1/T\n" +
                              $"   f = 1/{periodo:F3}\n" +
                              $"   f = {frequencia:F3} Hz\n\n" +
                              $"Energia Total:\n" +
                              $"   E = (1/2)kA²\n" +
                              $"   E = 0.5 × {constanteMola} × {amplitude}²\n" +
                              $"   E = {0.5 * constanteMola * amplitude * amplitude:F3} J";
        }

        private void AtualizarPosicaoInicial()
        {
            double posicaoX = 260 + (posicaoInicial * escalaPixels);
            Canvas.SetLeft(Massa, posicaoX - 40);

            // Atualizar mola
            AtualizarMola(posicaoX);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (simulacaoPausada) return;

            tempo += 0.016 * velocidadeAnimacao; // incremento de tempo ajustado

            // Equação do MHS: x(t) = A·cos(ωt + φ)
            // Considerando condições iniciais
            double fase = Math.Atan2(-velocidadeInicial, frequenciaAngular * posicaoInicial);
            double amplitudeReal = Math.Sqrt(posicaoInicial * posicaoInicial +
                                            (velocidadeInicial / frequenciaAngular) * (velocidadeInicial / frequenciaAngular));

            double posicao = amplitudeReal * Math.Cos(frequenciaAngular * tempo + fase);
            double velocidade = -amplitudeReal * frequenciaAngular * Math.Sin(frequenciaAngular * tempo + fase);
            double aceleracao = -frequenciaAngular * frequenciaAngular * posicao;

            // Atualizar posição da massa
            double posicaoX = 260 + (posicao * escalaPixels);
            Canvas.SetLeft(Massa, posicaoX - 40);

            // Atualizar mola
            AtualizarMola(posicaoX);

            // Atualizar informações em tempo real
            if (ChkMostrarEnergia.IsChecked == true)
            {
                InfoTempoReal.Visibility = Visibility.Visible;
                TxtPosicao.Text = $"Posição: {posicao:F3} m";
                TxtVelocidade.Text = $"Velocidade: {velocidade:F3} m/s";
                TxtAceleracao.Text = $"Aceleração: {aceleracao:F3} m/s²";

                double energiaTotal = 0.5 * constanteMola * amplitudeReal * amplitudeReal;
                TxtEnergia.Text = $"Energia Total: {energiaTotal:F3} J";

                double energiaCinetica = 0.5 * massa * velocidade * velocidade;
                double energiaPotencial = 0.5 * constanteMola * posicao * posicao;

                TxtEnergiaCinetica.Text = $"Ec = {energiaCinetica:F3} J";
                TxtEnergiaPotencial.Text = $"Ep = {energiaPotencial:F3} J";
            }

            // Atualizar vetores de força
            if (ChkMostrarVetores.IsChecked == true)
            {
                AtualizarVetores(posicao, posicaoX);
            }

            // Atualizar gráfico
            if (ChkMostrarGrafico.IsChecked == true)
            {
                AtualizarGrafico(tempo, posicao);
            }

            // Mostrar deslocamento
            LabelDeslocamento.Text = $"x = {posicao:F2} m";
            LabelDeslocamentoBorder.Visibility = Visibility.Visible;
            Canvas.SetLeft(LabelDeslocamentoBorder, posicaoX + 20);

            // Atualizar linha de deslocamento
            LinhaDeslocamento.X1 = 260;
            LinhaDeslocamento.X2 = posicaoX;
            LinhaDeslocamento.Visibility = Visibility.Visible;
            Canvas.SetLeft(SetaDeslocamento1, posicaoX);
            Canvas.SetLeft(SetaDeslocamento2, 260);
            SetaDeslocamento1.Visibility = Visibility.Visible;
            SetaDeslocamento2.Visibility = Visibility.Visible;
        }

        private void AtualizarMola(double posicaoX)
        {
            // Recalcular pontos da mola
            double comprimento = posicaoX - 70;
            int numEspiras = 10;
            double espacamento = comprimento / numEspiras;

            PointCollection pontos = new PointCollection();
            pontos.Add(new Point(0, 0));

            for (int i = 0; i < numEspiras; i++)
            {
                double x = i * espacamento + espacamento / 2;
                double y = (i % 2 == 0) ? -10 : 10;
                pontos.Add(new Point(x, y));
            }

            pontos.Add(new Point(comprimento, 0));
            MolaSegment.Points = pontos;
        }

        private void AtualizarVetores(double posicao, double posicaoX)
        {
            // Força restauradora: F = -kx
            double forca = -constanteMola * posicao;
            double forcaVisual = forca * 5; // escala para visualização

            if (Math.Abs(forcaVisual) > 5)
            {
                VetorForca.Visibility = Visibility.Visible;
                SetaForca.Visibility = Visibility.Visible;
                LabelForcaBorder.Visibility = Visibility.Visible;

                VetorForca.X1 = posicaoX;
                VetorForca.Y1 = 200;
                VetorForca.X2 = posicaoX + forcaVisual;
                VetorForca.Y2 = 200;

                Canvas.SetLeft(SetaForca, posicaoX + forcaVisual);
                Canvas.SetTop(SetaForca, 195);

                if (forcaVisual > 0)
                {
                    SetaForca.Points = new PointCollection { new Point(0, 0), new Point(-10, -5), new Point(-10, 5) };
                }
                else
                {
                    SetaForca.Points = new PointCollection { new Point(0, 0), new Point(10, -5), new Point(10, 5) };
                }

                LabelForca.Text = $"F = {forca:F1} N";
                Canvas.SetLeft(LabelForcaBorder, posicaoX + forcaVisual / 2 - 30);
            }
            else
            {
                VetorForca.Visibility = Visibility.Collapsed;
                SetaForca.Visibility = Visibility.Collapsed;
                LabelForcaBorder.Visibility = Visibility.Collapsed;
            }
        }

        private void AtualizarGrafico(double t, double x)
        {
            double graficoX = 50 + (t * 100) % 700;
            double graficoY = 125 - (x * 50);

            if (graficoX < 55)
            {
                CurvaGrafico.Points.Clear();
                CurvaGrafico.Points.Add(new Point(50, 125));
            }

            CurvaGrafico.Points.Add(new Point(graficoX, graficoY));

            MarcadorAtual.Visibility = Visibility.Visible;
            Canvas.SetLeft(MarcadorAtual, graficoX - 5);
            Canvas.SetTop(MarcadorAtual, graficoY - 5);
        }

        private void BtnIniciar_Click(object sender, RoutedEventArgs e)
        {
            simulacaoAtiva = true;
            simulacaoPausada = false;
            tempo = 0;

            CurvaGrafico.Points.Clear();
            CurvaGrafico.Points.Add(new Point(50, 125));

            timer.Start();

            BtnIniciar.IsEnabled = false;
            BtnPausar.IsEnabled = true;
            BtnParar.IsEnabled = true;
        }

        private void BtnPausar_Click(object sender, RoutedEventArgs e)
        {
            simulacaoPausada = !simulacaoPausada;
            BtnPausar.Content = simulacaoPausada ? "▶️ Retomar" : "⏸️ Pausar";
        }

        private void BtnParar_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            simulacaoAtiva = false;
            simulacaoPausada = false;
            tempo = 0;

            BtnIniciar.IsEnabled = true;
            BtnPausar.IsEnabled = false;
            BtnParar.IsEnabled = false;
            BtnPausar.Content = "⏸️ Pausar";

            LimparVisualizacao();
        }

        private void BtnResetar_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            simulacaoAtiva = false;
            simulacaoPausada = false;
            tempo = 0;

            SliderMassa.Value = 1.0;
            SliderConstanteMola.Value = 10.0;
            SliderAmplitude.Value = 0.5;
            SliderPosicaoInicial.Value = 0.5;
            SliderVelocidadeInicial.Value = 0;
            SliderVelocidadeAnimacao.Value = 1.0;

            BtnIniciar.IsEnabled = true;
            BtnPausar.IsEnabled = false;
            BtnParar.IsEnabled = false;
            BtnPausar.Content = "⏸️ Pausar";

            LimparVisualizacao();
        }

        private void LimparVisualizacao()
        {
            VetorForca.Visibility = Visibility.Collapsed;
            SetaForca.Visibility = Visibility.Collapsed;
            LabelForcaBorder.Visibility = Visibility.Collapsed;
            InfoTempoReal.Visibility = Visibility.Collapsed;
            MarcadorAtual.Visibility = Visibility.Collapsed;
            LinhaDeslocamento.Visibility = Visibility.Collapsed;
            SetaDeslocamento1.Visibility = Visibility.Collapsed;
            SetaDeslocamento2.Visibility = Visibility.Collapsed;
            LabelDeslocamentoBorder.Visibility = Visibility.Collapsed;

            CurvaGrafico.Points.Clear();
            CurvaGrafico.Points.Add(new Point(50, 125));

            AtualizarPosicaoInicial();
        }

        private void ChkVisualizacao_Changed(object sender, RoutedEventArgs e)
        {
            if (!IsLoaded) return;

            if (ChkMostrarVetores.IsChecked == false)
            {
                VetorForca.Visibility = Visibility.Collapsed;
                SetaForca.Visibility = Visibility.Collapsed;
                LabelForcaBorder.Visibility = Visibility.Collapsed;
            }

            if (ChkMostrarEnergia.IsChecked == false)
            {
                InfoTempoReal.Visibility = Visibility.Collapsed;
            }
        }

        #region Exemplos Pré-Configurados

        private void BtnExemplo1_Click(object sender, RoutedEventArgs e)
        {
            // Sistema Leve
            SliderMassa.Value = 0.5;
            SliderConstanteMola.Value = 10;
            SliderAmplitude.Value = 0.5;
            SliderPosicaoInicial.Value = 0.5;
            SliderVelocidadeInicial.Value = 0;
        }

        private void BtnExemplo2_Click(object sender, RoutedEventArgs e)
        {
            // Sistema Padrão
            SliderMassa.Value = 1.0;
            SliderConstanteMola.Value = 10;
            SliderAmplitude.Value = 0.5;
            SliderPosicaoInicial.Value = 0.5;
            SliderVelocidadeInicial.Value = 0;
        }

        private void BtnExemplo3_Click(object sender, RoutedEventArgs e)
        {
            // Sistema Pesado
            SliderMassa.Value = 5.0;
            SliderConstanteMola.Value = 10;
            SliderAmplitude.Value = 0.5;
            SliderPosicaoInicial.Value = 0.5;
            SliderVelocidadeInicial.Value = 0;
        }

        private void BtnExemplo4_Click(object sender, RoutedEventArgs e)
        {
            // Mola Rígida
            SliderMassa.Value = 1.0;
            SliderConstanteMola.Value = 50;
            SliderAmplitude.Value = 0.3;
            SliderPosicaoInicial.Value = 0.3;
            SliderVelocidadeInicial.Value = 0;
        }

        private void BtnExemplo5_Click(object sender, RoutedEventArgs e)
        {
            // Mola Fraca
            SliderMassa.Value = 1.0;
            SliderConstanteMola.Value = 5;
            SliderAmplitude.Value = 0.8;
            SliderPosicaoInicial.Value = 0.8;
            SliderVelocidadeInicial.Value = 0;
        }

        #endregion
    }
}