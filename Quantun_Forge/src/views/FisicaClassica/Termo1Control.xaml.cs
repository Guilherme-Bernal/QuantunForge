using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Quantun_Forge.src.views.FisicaClassica
{
    public partial class Termo1Control : UserControl
    {
        private bool desafioAtivo = false;
        private double ultimoDeltaU = 0;

        public Termo1Control()
        {
            InitializeComponent();
        }

        private void BtnCalcular_Click(object sender, RoutedEventArgs e)
        {
            double Q = SliderQ.Value;
            double W = SliderW.Value;
            double deltaU = Q - W;
            ultimoDeltaU = deltaU;

            // Exibe resultado (exceto se modo desafio estiver ativo)
            if (!desafioAtivo)
            {
                TxtResultado.Text = $"ΔU = Q - W\nΔU = {Q:F0} - {W:F0} = {deltaU:F0} J";
                MostrarInterpretacao(Q, W, deltaU);
            }
            else
            {
                TxtResultado.Text = "";
                TxtInterpretacao.Text = "";
                TxtRespostaDesafio.Text = $"🎯 Resposta do desafio: ΔU = {deltaU:F0} J";
                TxtRespostaDesafio.Visibility = Visibility.Visible;
                desafioAtivo = false;
            }

            AtualizarSimulacaoVisual(Q, W, deltaU);
        }

        private void MostrarInterpretacao(double Q, double W, double deltaU)
        {
            string resultado;
            if (deltaU > 0)
                resultado = "🟢 A energia interna aumentou: o sistema aqueceu!";
            else if (deltaU < 0)
                resultado = "🔴 A energia interna diminuiu: houve perda de calor ou muita expansão.";
            else
                resultado = "⚪ A energia interna permaneceu constante.";

            TxtInterpretacao.Text = resultado;
            TxtRespostaDesafio.Visibility = Visibility.Collapsed;
        }

        private void AtualizarSimulacaoVisual(double Q, double W, double deltaU)
        {
            // Altura da barra proporcional ao valor absoluto (máx 100)
            double max = 1000;
            double qH = Math.Min(Math.Abs(Q) * 100 / max, 100);
            double wH = Math.Min(Math.Abs(W) * 100 / max, 100);
            double uH = Math.Min(Math.Abs(deltaU) * 100 / max, 100);

            BarraQ.Height = qH;
            Canvas.SetTop(BarraQ, 150 - qH);

            BarraW.Height = wH;
            Canvas.SetTop(BarraW, 150 - wH);

            BarraU.Height = uH;
            Canvas.SetTop(BarraU, 150 - uH);

            // Cilindro: expansão ou compressão
            double baseTop = 50;
            if (W > 0)
            {
                Cilindro.Height = 120;
                Canvas.SetTop(Cilindro, baseTop - 20);
                TxtSimulacaoEstado.Text = "💨 Expansão do sistema";
            }
            else if (W < 0)
            {
                Cilindro.Height = 80;
                Canvas.SetTop(Cilindro, baseTop + 20);
                TxtSimulacaoEstado.Text = "🧱 Compressão do sistema";
            }
            else
            {
                Cilindro.Height = 100;
                Canvas.SetTop(Cilindro, baseTop);
                TxtSimulacaoEstado.Text = "🔹 Sem trabalho realizado";
            }

            // Cor conforme Q
            if (Q > 0)
                Cilindro.Fill = Brushes.OrangeRed; // calor adicionado
            else if (Q < 0)
                Cilindro.Fill = Brushes.SteelBlue; // calor removido
            else
                Cilindro.Fill = Brushes.LightBlue;
        }

        private void BtnResetar_Click(object sender, RoutedEventArgs e)
        {
            SliderQ.Value = 500;
            SliderW.Value = 200;

            TxtResultado.Text = "";
            TxtInterpretacao.Text = "";
            TxtRespostaDesafio.Visibility = Visibility.Collapsed;

            BarraQ.Height = BarraW.Height = BarraU.Height = 0;

            Cilindro.Height = 100;
            Canvas.SetTop(Cilindro, 50);
            Cilindro.Fill = Brushes.LightBlue;

            TxtSimulacaoEstado.Text = "🧊 Sistema em equilíbrio";
            desafioAtivo = false;
        }

        private void BtnDesafio_Click(object sender, RoutedEventArgs e)
        {
            desafioAtivo = true;
            TxtResultado.Text = "";
            TxtInterpretacao.Text = "";
            TxtRespostaDesafio.Text = "🔒 Resolva o desafio antes de calcular!";
            TxtRespostaDesafio.Visibility = Visibility.Visible;
        }

        private void BtnCenario1_Click(object sender, RoutedEventArgs e)
        {
            SliderQ.Value = 800;
            SliderW.Value = 300;
        }

        private void BtnCenario2_Click(object sender, RoutedEventArgs e)
        {
            SliderQ.Value = -300;
            SliderW.Value = -100;
        }
    }
}
