using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Quantun_Forge.src.views.FisicaClassica
{
    public partial class GravidadeUniversalControl : UserControl
    {
        // Constante gravitacional universal
        private const double G = 6.674e-11; // N·m²/kg²

        // Valores atuais
        private double massa1 = 5.97e24; // Terra em kg
        private double massa2 = 7.35e22; // Lua em kg
        private double distancia = 3.84e8; // Terra-Lua em metros

        public GravidadeUniversalControl()
        {
            InitializeComponent();
            Loaded += GravidadeUniversalControl_Loaded;
        }

        private void GravidadeUniversalControl_Loaded(object sender, RoutedEventArgs e)
        {
            AtualizarLabels();
        }

        #region Cálculos

        private double CalcularForcaGravitacional(double m1, double m2, double r)
        {
            // F = G × (m₁ × m₂) / r²
            return G * (m1 * m2) / (r * r);
        }

        private void BtnCalcular_Click(object sender, RoutedEventArgs e)
        {
            if (SliderMassa1 == null || SliderMassa2 == null || SliderDistancia == null) return;

            // Calcula a força
            double forca = CalcularForcaGravitacional(massa1, massa2, distancia);

            // Mostra os vetores de força
            MostrarVetoresForca(true);

            // Atualiza o passo a passo
            AtualizarPassoAPasso(forca);

            // Atualiza o resultado
            AtualizarResultado(forca);

            // Atualiza a visualização
            AtualizarVisualizacao();

            // Mostra o overlay
            if (InfoOverlay != null)
            {
                InfoOverlay.Visibility = Visibility.Visible;
                if (TxtForcaOverlay != null)
                {
                    TxtForcaOverlay.Text = $"F = {FormatarNotacaoCientifica(forca)} N";
                }
                if (TxtComparacaoOverlay != null)
                {
                    double pesoEquivalente = forca / 9.8;
                    TxtComparacaoOverlay.Text = $"≈ peso de {FormatarNotacaoCientifica(pesoEquivalente)} kg";
                }
            }
        }

        private void AtualizarPassoAPasso(double forca)
        {
            if (TxtPassoAPasso == null) return;

            string m1Str = FormatarNotacaoCientifica(massa1);
            string m2Str = FormatarNotacaoCientifica(massa2);
            string rStr = FormatarNotacaoCientifica(distancia);
            string gStr = "6.674×10⁻¹¹";
            string forcaStr = FormatarNotacaoCientifica(forca);

            TxtPassoAPasso.Text =
                $"1. Dados:\n" +
                $"   m₁ = {m1Str} kg\n" +
                $"   m₂ = {m2Str} kg\n" +
                $"   r = {rStr} m\n" +
                $"   G = {gStr} N·m²/kg²\n\n" +
                $"2. Aplicando a fórmula:\n" +
                $"   F = G × (m₁ × m₂) / r²\n\n" +
                $"3. Substituindo:\n" +
                $"   F = {gStr} × ({m1Str} × {m2Str}) / ({rStr})²\n\n" +
                $"4. Resultado:\n" +
                $"   F = {forcaStr} N";
        }

        private void AtualizarResultado(double forca)
        {
            if (TxtForcaResultado == null || TxtComparacao == null) return;

            TxtForcaResultado.Text = $"F = {FormatarNotacaoCientifica(forca)} N";

            // Comparações interessantes
            double pesoEquivalente = forca / 9.8;
            string comparacao = "";

            if (pesoEquivalente > 1e20)
            {
                comparacao = $"Isso equivale ao peso de aproximadamente {FormatarNotacaoCientifica(pesoEquivalente)} kg! " +
                           "Uma força astronômica que mantém os corpos celestes em suas órbitas.";
            }
            else if (pesoEquivalente > 1e15)
            {
                comparacao = $"Equivale ao peso de {FormatarNotacaoCientifica(pesoEquivalente)} kg. " +
                           "Uma força imensa em escala cósmica!";
            }
            else if (pesoEquivalente > 1e10)
            {
                comparacao = $"Equivale ao peso de {FormatarNotacaoCientifica(pesoEquivalente)} kg. " +
                           "Aproximadamente o peso de milhões de elefantes!";
            }
            else if (pesoEquivalente > 1e5)
            {
                comparacao = $"Equivale ao peso de {FormatarNotacaoCientifica(pesoEquivalente)} kg. " +
                           "Como centenas de carros!";
            }
            else if (pesoEquivalente > 100)
            {
                comparacao = $"Equivale ao peso de {pesoEquivalente:F0} kg. " +
                           "Como uma pessoa pesada!";
            }
            else if (pesoEquivalente > 1)
            {
                comparacao = $"Equivale ao peso de {pesoEquivalente:F2} kg. " +
                           "Uma força bastante pequena.";
            }
            else
            {
                comparacao = $"Equivale ao peso de apenas {pesoEquivalente:F4} kg. " +
                           "Uma força extremamente fraca!";
            }

            TxtComparacao.Text = comparacao;
        }

        private void MostrarVetoresForca(bool mostrar)
        {
            Visibility vis = mostrar ? Visibility.Visible : Visibility.Collapsed;

            if (VetorForca1 != null) VetorForca1.Visibility = vis;
            if (SetaForca1 != null) SetaForca1.Visibility = vis;
            if (LabelForca1 != null) LabelForca1.Visibility = vis;

            if (VetorForca2 != null) VetorForca2.Visibility = vis;
            if (SetaForca2 != null) SetaForca2.Visibility = vis;
            if (LabelForca2 != null) LabelForca2.Visibility = vis;
        }

        #endregion

        #region Sliders

        private void Sliders_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (SliderMassa1 == null || SliderMassa2 == null || SliderDistancia == null) return;

            // Slider trabalha com expoentes (log10)
            massa1 = Math.Pow(10, SliderMassa1.Value);
            massa2 = Math.Pow(10, SliderMassa2.Value);
            distancia = Math.Pow(10, SliderDistancia.Value);

            AtualizarLabels();
            AtualizarVisualizacao();

            // Esconde os vetores quando os valores mudam
            MostrarVetoresForca(false);
            if (InfoOverlay != null) InfoOverlay.Visibility = Visibility.Collapsed;
        }

        private void AtualizarLabels()
        {
            if (TxtMassa1Slider != null)
                TxtMassa1Slider.Text = $"{FormatarNotacaoCientifica(massa1)} kg";

            if (TxtMassa2Slider != null)
                TxtMassa2Slider.Text = $"{FormatarNotacaoCientifica(massa2)} kg";

            if (TxtDistanciaSlider != null)
                TxtDistanciaSlider.Text = $"{FormatarNotacaoCientifica(distancia)} m";

            if (TxtDistanciaVisual != null)
                TxtDistanciaVisual.Text = $"{FormatarNotacaoCientifica(distancia)} m";

            if (MassaCorpo1 != null)
                MassaCorpo1.Text = $"{FormatarNotacaoCientifica(massa1)} kg";

            if (MassaCorpo2 != null)
                MassaCorpo2.Text = $"{FormatarNotacaoCientifica(massa2)} kg";
        }

        private void AtualizarVisualizacao()
        {
            if (Corpo1 == null || Corpo2 == null || Corpo1Container == null || Corpo2Container == null) return;

            // Ajusta o tamanho dos corpos baseado nas massas (proporcionalmente)
            double escala1 = Math.Log10(massa1);
            double escala2 = Math.Log10(massa2);

            // Normaliza entre 50 e 120 pixels
            double tamanho1 = Math.Max(50, Math.Min(120, escala1 * 4));
            double tamanho2 = Math.Max(40, Math.Min(100, escala2 * 4));

            Corpo1.Width = tamanho1;
            Corpo1.Height = tamanho1;

            Corpo2.Width = tamanho2;
            Corpo2.Height = tamanho2;

            // Ajusta a posição dos corpos baseado na distância
            double posicaoRelativa = Math.Log10(distancia);
            double espacamento = Math.Max(250, Math.Min(600, posicaoRelativa * 60));

            Canvas.SetLeft(Corpo1Container, 100);
            Canvas.SetLeft(Corpo2Container, 100 + espacamento);

            // Atualiza a linha de distância
            if (LinhaDistancia != null)
            {
                LinhaDistancia.X1 = 100 + tamanho1 / 2;
                LinhaDistancia.X2 = 100 + espacamento + tamanho2 / 2;
            }

            // Atualiza vetores de força se visíveis
            if (VetorForca1 != null && VetorForca1.Visibility == Visibility.Visible)
            {
                double centro1X = 100 + tamanho1 / 2;
                double centro2X = 100 + espacamento + tamanho2 / 2;

                VetorForca1.X1 = centro1X + tamanho1 / 2;
                VetorForca1.X2 = centro1X + tamanho1 / 2 + 80;

                VetorForca2.X1 = centro2X - tamanho2 / 2;
                VetorForca2.X2 = centro2X - tamanho2 / 2 - 80;

                if (SetaForca1 != null)
                {
                    Canvas.SetLeft(SetaForca1, VetorForca1.X2 - 10);
                }

                if (SetaForca2 != null)
                {
                    Canvas.SetLeft(SetaForca2, VetorForca2.X2);
                }
            }
        }

        #endregion

        #region Cenários Predefinidos

        private void BtnTerraLua_Click(object sender, RoutedEventArgs e)
        {
            ConfigurarCenario(
                5.97e24,  // Massa da Terra
                7.35e22,  // Massa da Lua
                3.84e8,   // Distância Terra-Lua
                "Terra", "🌍",
                "Lua", "🌕"
            );
        }

        private void BtnTerraMaca_Click(object sender, RoutedEventArgs e)
        {
            ConfigurarCenario(
                5.97e24,  // Massa da Terra
                0.2,      // Massa de uma maçã (~200g)
                6.371e6,  // Raio da Terra (maçã na superfície)
                "Terra", "🌍",
                "Maçã", "🍎"
            );
        }

        private void BtnSolTerra_Click(object sender, RoutedEventArgs e)
        {
            ConfigurarCenario(
                1.989e30, // Massa do Sol
                5.97e24,  // Massa da Terra
                1.496e11, // Distância Sol-Terra (1 UA)
                "Sol", "☀️",
                "Terra", "🌍"
            );
        }

        private void BtnTerraSatelite_Click(object sender, RoutedEventArgs e)
        {
            ConfigurarCenario(
                5.97e24,  // Massa da Terra
                500,      // Massa de um satélite típico
                6.771e6,  // Raio da Terra + 400km (órbita baixa)
                "Terra", "🌍",
                "Satélite", "🛰️"
            );
        }

        private void ConfigurarCenario(double m1, double m2, double r,
                                      string nome1, string emoji1,
                                      string nome2, string emoji2)
        {
            massa1 = m1;
            massa2 = m2;
            distancia = r;

            // Atualiza sliders
            if (SliderMassa1 != null) SliderMassa1.Value = Math.Log10(m1);
            if (SliderMassa2 != null) SliderMassa2.Value = Math.Log10(m2);
            if (SliderDistancia != null) SliderDistancia.Value = Math.Log10(r);

            // Atualiza labels dos corpos
            if (LabelCorpo1 != null) LabelCorpo1.Text = nome1;
            if (LabelCorpo2 != null) LabelCorpo2.Text = nome2;

            // Atualiza emojis (precisaria criar TextBlocks para emojis no XAML)

            AtualizarLabels();
            AtualizarVisualizacao();
            MostrarVetoresForca(false);

            if (InfoOverlay != null) InfoOverlay.Visibility = Visibility.Collapsed;

            if (TxtPassoAPasso != null)
                TxtPassoAPasso.Text = $"Cenário configurado: {nome1} e {nome2}. Clique em 'Calcular Força' para ver o resultado.";
        }

        #endregion

        #region Utilitários

        private void BtnResetar_Click(object sender, RoutedEventArgs e)
        {
            // Reseta para Terra-Lua
            BtnTerraLua_Click(sender, e);

            MostrarVetoresForca(false);

            if (InfoOverlay != null) InfoOverlay.Visibility = Visibility.Collapsed;

            if (TxtPassoAPasso != null)
                TxtPassoAPasso.Text = "Configure os valores e clique em 'Calcular Força' para ver o resultado";

            if (TxtForcaResultado != null)
                TxtForcaResultado.Text = "Aguardando cálculo...";

            if (TxtComparacao != null)
                TxtComparacao.Text = "A força será exibida após o cálculo";
        }

        private string FormatarNotacaoCientifica(double valor)
        {
            if (valor == 0) return "0";

            int expoente = (int)Math.Floor(Math.Log10(Math.Abs(valor)));
            double mantissa = valor / Math.Pow(10, expoente);

            // Se o número é "simples", mostra normal
            if (Math.Abs(expoente) <= 3 && valor >= 1)
            {
                return valor.ToString("N2");
            }

            return $"{mantissa:F2}×10{FormatarExpoente(expoente)}";
        }

        private string FormatarExpoente(int exp)
        {
            if (exp == 0) return "⁰";

            string resultado = "";
            if (exp < 0)
            {
                resultado = "⁻";
                exp = Math.Abs(exp);
            }

            string expStr = exp.ToString();
            foreach (char c in expStr)
            {
                resultado += c switch
                {
                    '0' => '⁰',
                    '1' => '¹',
                    '2' => '²',
                    '3' => '³',
                    '4' => '⁴',
                    '5' => '⁵',
                    '6' => '⁶',
                    '7' => '⁷',
                    '8' => '⁸',
                    '9' => '⁹',
                    _ => c
                };
            }

            return resultado;
        }

        #endregion
    }
}