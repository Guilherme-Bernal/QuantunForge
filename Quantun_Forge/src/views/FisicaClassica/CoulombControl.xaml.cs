using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace Quantun_Forge.src.views.FisicaClassica
{
    public partial class CoulombControl : UserControl
    {
        private const double k = 8.99e9; // Constante de Coulomb (N·m²/C²)

        public CoulombControl()
        {
            InitializeComponent();
            AtualizarVisualizacao();
        }

        private void Sliders_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            AtualizarVisualizacao();
        }

        private void AtualizarVisualizacao()
        {
            if (SliderCarga1 == null || SliderCarga2 == null || SliderDistancia == null) return;

            double q1 = SliderCarga1.Value;
            double q2 = SliderCarga2.Value;
            double r = SliderDistancia.Value;

            // Atualizar labels dos sliders
            if (TxtCarga1Slider != null)
            {
                TxtCarga1Slider.Text = $"{(q1 >= 0 ? "+" : "")}{q1:F1} C";
            }

            if (TxtCarga2Slider != null)
            {
                TxtCarga2Slider.Text = $"{(q2 >= 0 ? "+" : "")}{q2:F1} C";
            }

            if (TxtDistanciaSlider != null)
            {
                TxtDistanciaSlider.Text = $"{r:F1} m";
            }

            if (TxtDistanciaVisual != null)
            {
                TxtDistanciaVisual.Text = $"{r:F1} m";
            }

            // Atualizar visualização das cargas
            AtualizarCarga1(q1);
            AtualizarCarga2(q2);

            // Ajustar posição das cargas baseado na distância
            AjustarPosicaoCargas(r);
        }

        private void AtualizarCarga1(double q1)
        {
            if (SimboloCarga1 == null || Carga1 == null || ValorCarga1 == null) return;

            if (q1 > 0)
            {
                SimboloCarga1.Text = "+";
                Carga1.Fill = new SolidColorBrush(Color.FromRgb(231, 76, 60)); // Vermelho
                Carga1.Stroke = new SolidColorBrush(Color.FromRgb(192, 57, 43));
                ((DropShadowEffect)Carga1.Effect).Color = Color.FromRgb(231, 76, 60);
            }
            else if (q1 < 0)
            {
                SimboloCarga1.Text = "-";
                Carga1.Fill = new SolidColorBrush(Color.FromRgb(52, 152, 219)); // Azul
                Carga1.Stroke = new SolidColorBrush(Color.FromRgb(41, 128, 185));
                ((DropShadowEffect)Carga1.Effect).Color = Color.FromRgb(52, 152, 219);
            }
            else
            {
                SimboloCarga1.Text = "0";
                Carga1.Fill = new SolidColorBrush(Color.FromRgb(149, 165, 166)); // Cinza
                Carga1.Stroke = new SolidColorBrush(Color.FromRgb(127, 140, 141));
                ((DropShadowEffect)Carga1.Effect).Color = Color.FromRgb(149, 165, 166);
            }

            ValorCarga1.Text = $"{(q1 >= 0 ? "+" : "")}{q1:F1} C";
        }

        private void AtualizarCarga2(double q2)
        {
            if (SimboloCarga2 == null || Carga2 == null || ValorCarga2 == null) return;

            if (q2 > 0)
            {
                SimboloCarga2.Text = "+";
                Carga2.Fill = new SolidColorBrush(Color.FromRgb(231, 76, 60)); // Vermelho
                Carga2.Stroke = new SolidColorBrush(Color.FromRgb(192, 57, 43));
                ((DropShadowEffect)Carga2.Effect).Color = Color.FromRgb(231, 76, 60);
            }
            else if (q2 < 0)
            {
                SimboloCarga2.Text = "-";
                Carga2.Fill = new SolidColorBrush(Color.FromRgb(52, 152, 219)); // Azul
                Carga2.Stroke = new SolidColorBrush(Color.FromRgb(41, 128, 185));
                ((DropShadowEffect)Carga2.Effect).Color = Color.FromRgb(52, 152, 219);
            }
            else
            {
                SimboloCarga2.Text = "0";
                Carga2.Fill = new SolidColorBrush(Color.FromRgb(149, 165, 166)); // Cinza
                Carga2.Stroke = new SolidColorBrush(Color.FromRgb(127, 140, 141));
                ((DropShadowEffect)Carga2.Effect).Color = Color.FromRgb(149, 165, 166);
            }

            ValorCarga2.Text = $"{(q2 >= 0 ? "+" : "")}{q2:F1} C";
        }

        private void AjustarPosicaoCargas(double distancia)
        {
            if (LinhaDistancia == null) return;

            // Ajustar linha de distância baseado no valor
            // Manter posições fixas mas ajustar visualmente a linha
            double escala = Math.Min(distancia / 10.0, 1.0);
            double larguraLinha = 480 * (0.5 + escala * 0.5);

            LinhaDistancia.X2 = 150 + larguraLinha;
        }

        private void BtnCalcular_Click(object sender, RoutedEventArgs e)
        {
            double q1 = SliderCarga1.Value;
            double q2 = SliderCarga2.Value;
            double r = SliderDistancia.Value;

            // Calcular força
            double F = k * Math.Abs(q1 * q2) / (r * r);

            // Determinar tipo de interação
            bool atracao = (q1 * q2) < 0;
            string tipoInteracao = atracao ? "ATRAÇÃO" : "REPULSÃO";
            Color corInteracao = atracao ? Color.FromRgb(231, 76, 60) : Color.FromRgb(52, 152, 219);

            // Mostrar tipo de interação
            if (TipoInteracao != null && TxtTipoInteracao != null)
            {
                TipoInteracao.Visibility = Visibility.Visible;
                TipoInteracao.Background = new SolidColorBrush(corInteracao);
                TxtTipoInteracao.Text = $"⚡ {tipoInteracao}";
            }

            // Mostrar vetores de força
            MostrarVetoresForca(atracao);

            // Atualizar resultado
            if (TxtForcaResultado != null)
            {
                TxtForcaResultado.Text = $"F = {F:E2} N";
            }

            // Comparação
            if (TxtComparacao != null)
            {
                double massaEquivalente = F / 9.8; // kg
                if (massaEquivalente >= 1e9)
                {
                    TxtComparacao.Text = $"≈ peso de {massaEquivalente / 1e9:F2} bilhões de kg";
                }
                else if (massaEquivalente >= 1e6)
                {
                    TxtComparacao.Text = $"≈ peso de {massaEquivalente / 1e6:F2} milhões de kg";
                }
                else if (massaEquivalente >= 1e3)
                {
                    TxtComparacao.Text = $"≈ peso de {massaEquivalente / 1e3:F2} mil kg";
                }
                else
                {
                    TxtComparacao.Text = $"≈ peso de {massaEquivalente:F2} kg";
                }
            }

            // Tipo de força
            if (TxtTipoForca != null)
            {
                TxtTipoForca.Text = $"📌 Tipo: {tipoInteracao}";
                TxtTipoForca.Foreground = new SolidColorBrush(corInteracao);
            }

            // Passo a passo
            if (TxtPassoAPasso != null)
            {
                TxtPassoAPasso.Text = $"1️⃣ Dados:\n" +
                                     $"   q₁ = {q1:F1} C\n" +
                                     $"   q₂ = {q2:F1} C\n" +
                                     $"   r = {r:F1} m\n" +
                                     $"   k = 8.99×10⁹ N·m²/C²\n\n" +
                                     $"2️⃣ Fórmula:\n" +
                                     $"   F = k × |q₁ × q₂| / r²\n\n" +
                                     $"3️⃣ Substituindo:\n" +
                                     $"   F = 8.99×10⁹ × |{q1:F1} × {q2:F1}| / {r:F1}²\n" +
                                     $"   F = 8.99×10⁹ × {Math.Abs(q1 * q2):F1} / {r * r:F2}\n\n" +
                                     $"4️⃣ Resultado:\n" +
                                     $"   F = {F:E2} N\n" +
                                     $"   Tipo: {tipoInteracao}";
            }

            // Overlay
            if (InfoOverlay != null && TxtForcaOverlay != null && TxtComparacaoOverlay != null)
            {
                InfoOverlay.Visibility = Visibility.Visible;
                TxtForcaOverlay.Text = $"F = {F:E2} N";

                double massaEq = F / 9.8;
                if (massaEq >= 1e9)
                {
                    TxtComparacaoOverlay.Text = $"≈ peso de {massaEq / 1e9:F1} bilhões de kg";
                }
                else if (massaEq >= 1e6)
                {
                    TxtComparacaoOverlay.Text = $"≈ peso de {massaEq / 1e6:F1} milhões de kg";
                }
                else
                {
                    TxtComparacaoOverlay.Text = $"≈ peso de {massaEq:F0} kg";
                }
            }
        }

        private void MostrarVetoresForca(bool atracao)
        {
            if (VetorForca1 == null || VetorForca2 == null ||
                SetaForca1 == null || SetaForca2 == null ||
                LabelForca1 == null || LabelForca2 == null) return;

            // Mostrar vetores
            VetorForca1.Visibility = Visibility.Visible;
            VetorForca2.Visibility = Visibility.Visible;
            SetaForca1.Visibility = Visibility.Visible;
            SetaForca2.Visibility = Visibility.Visible;
            LabelForca1.Visibility = Visibility.Visible;
            LabelForca2.Visibility = Visibility.Visible;

            if (atracao)
            {
                // Atração: forças apontam uma para a outra
                // Força 1 aponta para direita (em direção à carga 2)
                VetorForca1.X2 = 320;
                SetaForca1.Points = new PointCollection { new Point(320, 200), new Point(310, 195), new Point(310, 205) };

                // Força 2 aponta para esquerda (em direção à carga 1)
                VetorForca2.X2 = 530;
                SetaForca2.Points = new PointCollection { new Point(530, 200), new Point(540, 195), new Point(540, 205) };
            }
            else
            {
                // Repulsão: forças apontam para fora
                // Força 1 aponta para esquerda (se afastando)
                VetorForca1.X2 = 80;
                SetaForca1.Points = new PointCollection { new Point(80, 200), new Point(90, 195), new Point(90, 205) };

                // Força 2 aponta para direita (se afastando)
                VetorForca2.X2 = 720;
                SetaForca2.Points = new PointCollection { new Point(720, 200), new Point(710, 195), new Point(710, 205) };
            }
        }

        private void BtnResetar_Click(object sender, RoutedEventArgs e)
        {
            // Resetar valores padrão
            SliderCarga1.Value = 5;
            SliderCarga2.Value = -3;
            SliderDistancia.Value = 2;

            // Limpar resultados
            LimparResultados();
        }

        private void LimparResultados()
        {
            if (TxtForcaResultado != null)
                TxtForcaResultado.Text = "Aguardando cálculo...";

            if (TxtComparacao != null)
                TxtComparacao.Text = "A força será exibida após o cálculo";

            if (TxtTipoForca != null)
                TxtTipoForca.Text = "";

            if (TxtPassoAPasso != null)
                TxtPassoAPasso.Text = "Configure os valores e clique em 'Calcular Força' para ver o resultado";

            // Esconder elementos visuais
            if (VetorForca1 != null) VetorForca1.Visibility = Visibility.Collapsed;
            if (VetorForca2 != null) VetorForca2.Visibility = Visibility.Collapsed;
            if (SetaForca1 != null) SetaForca1.Visibility = Visibility.Collapsed;
            if (SetaForca2 != null) SetaForca2.Visibility = Visibility.Collapsed;
            if (LabelForca1 != null) LabelForca1.Visibility = Visibility.Collapsed;
            if (LabelForca2 != null) LabelForca2.Visibility = Visibility.Collapsed;
            if (TipoInteracao != null) TipoInteracao.Visibility = Visibility.Collapsed;
            if (InfoOverlay != null) InfoOverlay.Visibility = Visibility.Collapsed;
        }

        #region Cenários Prontos
        private void BtnAtracaoForte_Click(object sender, RoutedEventArgs e)
        {
            SliderCarga1.Value = 8;
            SliderCarga2.Value = -8;
            SliderDistancia.Value = 0.5;
            LimparResultados();
        }

        private void BtnRepulsaoForte_Click(object sender, RoutedEventArgs e)
        {
            SliderCarga1.Value = 10;
            SliderCarga2.Value = 10;
            SliderDistancia.Value = 0.3;
            LimparResultados();
        }

        private void BtnProtonEletron_Click(object sender, RoutedEventArgs e)
        {
            // Carga elementar: 1.6×10⁻¹⁹ C (aproximado para visualização)
            SliderCarga1.Value = 0.2; // Representa próton (escala ajustada)
            SliderCarga2.Value = -0.2; // Representa elétron
            SliderDistancia.Value = 1; // Distância atômica (escala ajustada)
            LimparResultados();
        }

        private void BtnCargasPequenas_Click(object sender, RoutedEventArgs e)
        {
            SliderCarga1.Value = 1;
            SliderCarga2.Value = -1;
            SliderDistancia.Value = 5;
            LimparResultados();
        }
        #endregion
    }
}