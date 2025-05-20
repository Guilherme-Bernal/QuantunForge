using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Quantun_Forge.src.views.FisicaClassica
{
    public partial class LeiNewton1Control : UserControl
    {
        private DispatcherTimer timer;
        private double velocidade = 0;
        private double massa = 5;
        private double atrito = 0;
        private bool pausado = false;

        public LeiNewton1Control()
        {
            InitializeComponent();

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(16); // ~60 FPS
            timer.Tick += Timer_Tick;
        }

        private void StartSimulation_Click(object sender, RoutedEventArgs e)
        {
            velocidade = SliderVelocidade.Value;
            massa = SliderMassa.Value;
            pausado = false;

            if (ComboCenario.SelectedIndex == 0)
                atrito = 0;         // Espaço (sem atrito)
            else
                atrito = 0.01;      // Terra (com atrito)

            timer.Start();
        }

        private void ResetSimulation_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            Canvas.SetLeft(MovingBall, 10);
            velocidade = 0;
            TxtVelocidadeAtual.Text = "";
            TxtPosicao.Text = "";
        }

        private void BtnReiniciar_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            Canvas.SetLeft(MovingBall, 10);
            velocidade = 0;
            SliderVelocidade.Value = 5;
            SliderMassa.Value = 5;
            ComboCenario.SelectedIndex = 0;
            TxtVelocidadeAtual.Text = "";
            TxtPosicao.Text = "";
            pausado = false;
        }

        private void BtnAplicarForca_Click(object sender, RoutedEventArgs e)
        {
            massa = SliderMassa.Value;

            double forca = 10; // N
            double aceleracao = forca / massa;

            velocidade += aceleracao; // aplica aceleração
        }

        private void BtnForcaContraria_Click(object sender, RoutedEventArgs e)
        {
            massa = SliderMassa.Value;

            double forca = 10; // N
            double aceleracao = forca / massa;

            // força oposta à direção atual
            if (velocidade > 0)
                velocidade -= aceleracao;
            else
                velocidade += aceleracao;
        }

        private void BtnPausar_Click(object sender, RoutedEventArgs e)
        {
            pausado = true;
        }

        private void BtnRetomar_Click(object sender, RoutedEventArgs e)
        {
            pausado = false;
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (pausado) return;

            double left = Canvas.GetLeft(MovingBall);

            // Aplica atrito (se houver)
            velocidade *= (1 - atrito);

            double novoLeft = left + velocidade;

            if (novoLeft + MovingBall.Width < SimulationCanvas.ActualWidth)
            {
                Canvas.SetLeft(MovingBall, novoLeft);
            }
            else
            {
                timer.Stop(); // colidiu com a borda
            }

            TxtVelocidadeAtual.Text = $"Velocidade atual: {velocidade:F2} px/frame";
            TxtPosicao.Text = $"Deslocamento: {novoLeft:F0} px";
        }
    }
}
