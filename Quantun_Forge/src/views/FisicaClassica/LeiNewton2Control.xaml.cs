using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Quantun_Forge.src.views.FisicaClassica
{
    public partial class LeiNewton2Control : UserControl
    {
        private DispatcherTimer timer;
        private double massa;
        private double forca;
        private double aceleracao;
        private double velocidade = 0;

        public LeiNewton2Control()
        {
            InitializeComponent();

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(16); // ~60 FPS
            timer.Tick += Timer_Tick;
        }

        private void BtnSimular_Click(object sender, RoutedEventArgs e)
        {
            massa = SliderMassa.Value;
            forca = SliderForca.Value;

            aceleracao = forca / massa;
            velocidade = 0;

            TxtFormulaCalculada.Text = $"F = m × a  →  {forca:F0} N = {massa:F0} kg × {aceleracao:F2} m/s²";
            TxtExplicacao.Text = $"A força aplicada causará uma aceleração de {aceleracao:F2} m/s². O objeto começará a se mover com velocidade crescente.";
            TxtPosicao.Text = "";

            timer.Start();
        }

        private void BtnResetar_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            Canvas.SetLeft(Ball, 10);
            velocidade = 0;
            TxtFormulaCalculada.Text = "";
            TxtExplicacao.Text = "";
            TxtPosicao.Text = "";
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            velocidade += aceleracao * 0.016; // Δt ≈ 16ms
            double left = Canvas.GetLeft(Ball);
            double novoLeft = left + velocidade;

            if (novoLeft + Ball.Width < CanvasSimulacao.ActualWidth)
            {
                Canvas.SetLeft(Ball, novoLeft);
                TxtPosicao.Text = $"Deslocamento: {novoLeft:F0} px";
            }
            else
            {
                timer.Stop();
                TxtExplicacao.Text += "\n\nA bola atingiu o limite da área de simulação.";
            }
        }
    }
}
