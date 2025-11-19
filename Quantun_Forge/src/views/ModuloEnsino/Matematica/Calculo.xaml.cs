using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Text;
using System.Text.RegularExpressions;

namespace Quantun_Forge.src.views.ModuloEnsino.Matematica
{
    public partial class Calculo : Window
    {
        public Calculo()
        {
            InitializeComponent();
            ConfigurarEventos();
        }

        private void ConfigurarEventos()
        {
            // Eventos de Derivadas
            btnPrimeiraDerivada.Click += BtnPrimeiraDerivada_Click;
            btnSegundaDerivada.Click += BtnSegundaDerivada_Click;
            btnTerceiraDerivada.Click += BtnTerceiraDerivada_Click;
            btnDerivadaPonto.Click += BtnDerivadaPonto_Click;
            btnAnaliseDerivada.Click += BtnAnaliseDerivada_Click;
            btnPontosCriticos.Click += BtnPontosCriticos_Click;
            btnConcavidade.Click += BtnConcavidade_Click;
            btnLimparDerivada.Click += (s, e) => txtResultadosDerivada.Text = "Aguardando cálculos de derivadas...";

            // Eventos de Integrais
            btnIntegralIndefinida.Click += BtnIntegralIndefinida_Click;
            btnIntegralDefinida.Click += BtnIntegralDefinida_Click;
            btnAreaCurva.Click += BtnAreaCurva_Click;
            btnAnaliseIntegral.Click += BtnAnaliseIntegral_Click;
            btnMetodosNumericos.Click += BtnMetodosNumericos_Click;
            btnLimparIntegral.Click += (s, e) => txtResultadosIntegral.Text = "Aguardando cálculos de integrais...";

            // Eventos de Limites
            btnLimitePonto.Click += BtnLimitePonto_Click;
            btnLimiteEsquerda.Click += BtnLimiteEsquerda_Click;
            btnLimiteDireita.Click += BtnLimiteDireita_Click;
            btnLimiteInfinito.Click += BtnLimiteInfinito_Click;
            btnLimparLimite.Click += (s, e) => txtResultadosLimite.Text = "Aguardando cálculos de limites...";

            // Evento de mudança de aba no TabControl
            // Encontrar o TabControl no painel esquerdo
            var tabControl = FindTabControl(this);
            if (tabControl != null)
            {
                tabControl.SelectionChanged += TabControl_SelectionChanged;
            }
        }

        private TabControl FindTabControl(DependencyObject parent)
        {
            int childCount = System.Windows.Media.VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childCount; i++)
            {
                var child = System.Windows.Media.VisualTreeHelper.GetChild(parent, i);
                if (child is TabControl tabControl)
                    return tabControl;

                var result = FindTabControl(child);
                if (result != null)
                    return result;
            }
            return null;
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is TabControl tabControl && tabControl.SelectedIndex >= 0)
            {
                switch (tabControl.SelectedIndex)
                {
                    case 0: // Derivadas
                        txtResultadosDerivada.Visibility = Visibility.Visible;
                        txtResultadosIntegral.Visibility = Visibility.Collapsed;
                        txtResultadosLimite.Visibility = Visibility.Collapsed;

                        btnLimparDerivada.Visibility = Visibility.Visible;
                        btnLimparIntegral.Visibility = Visibility.Collapsed;
                        btnLimparLimite.Visibility = Visibility.Collapsed;
                        break;

                    case 1: // Integrais
                        txtResultadosDerivada.Visibility = Visibility.Collapsed;
                        txtResultadosIntegral.Visibility = Visibility.Visible;
                        txtResultadosLimite.Visibility = Visibility.Collapsed;

                        btnLimparDerivada.Visibility = Visibility.Collapsed;
                        btnLimparIntegral.Visibility = Visibility.Visible;
                        btnLimparLimite.Visibility = Visibility.Collapsed;
                        break;

                    case 2: // Limites
                        txtResultadosDerivada.Visibility = Visibility.Collapsed;
                        txtResultadosIntegral.Visibility = Visibility.Collapsed;
                        txtResultadosLimite.Visibility = Visibility.Visible;

                        btnLimparDerivada.Visibility = Visibility.Collapsed;
                        btnLimparIntegral.Visibility = Visibility.Collapsed;
                        btnLimparLimite.Visibility = Visibility.Visible;
                        break;
                }
            }
        }

        #region DERIVADAS

        private void BtnPrimeiraDerivada_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string funcao = txtFuncaoDerivada.Text.Trim();
                if (string.IsNullOrEmpty(funcao))
                {
                    MessageBox.Show("Por favor, insira uma função!", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string derivada = CalcularDerivada(funcao);

                StringBuilder resultado = new StringBuilder();
                resultado.AppendLine("═══════════════════════════════════");
                resultado.AppendLine("   📈 PRIMEIRA DERIVADA - f'(x)");
                resultado.AppendLine("═══════════════════════════════════\n");
                resultado.AppendLine($"Função original:");
                resultado.AppendLine($"  f(x) = {funcao}\n");
                resultado.AppendLine($"Primeira derivada:");
                resultado.AppendLine($"  f'(x) = {derivada}\n");
                resultado.AppendLine("───────────────────────────────────");
                resultado.AppendLine("Interpretação:");
                resultado.AppendLine("  • f'(x) representa a taxa de");
                resultado.AppendLine("    variação instantânea");
                resultado.AppendLine("  • f'(x) > 0 → função crescente");
                resultado.AppendLine("  • f'(x) < 0 → função decrescente");
                resultado.AppendLine("  • f'(x) = 0 → ponto crítico");
                resultado.AppendLine("═══════════════════════════════════");

                txtResultadosDerivada.Text = resultado.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao calcular derivada: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnSegundaDerivada_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string funcao = txtFuncaoDerivada.Text.Trim();
                if (string.IsNullOrEmpty(funcao))
                {
                    MessageBox.Show("Por favor, insira uma função!", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string derivada1 = CalcularDerivada(funcao);
                string derivada2 = CalcularDerivada(derivada1);

                StringBuilder resultado = new StringBuilder();
                resultado.AppendLine("═══════════════════════════════════");
                resultado.AppendLine("   📊 SEGUNDA DERIVADA - f''(x)");
                resultado.AppendLine("═══════════════════════════════════\n");
                resultado.AppendLine($"Função original:");
                resultado.AppendLine($"  f(x) = {funcao}\n");
                resultado.AppendLine($"Primeira derivada:");
                resultado.AppendLine($"  f'(x) = {derivada1}\n");
                resultado.AppendLine($"Segunda derivada:");
                resultado.AppendLine($"  f''(x) = {derivada2}\n");
                resultado.AppendLine("───────────────────────────────────");
                resultado.AppendLine("Interpretação:");
                resultado.AppendLine("  • f''(x) indica a concavidade");
                resultado.AppendLine("  • f''(x) > 0 → côncava para cima");
                resultado.AppendLine("  • f''(x) < 0 → côncava para baixo");
                resultado.AppendLine("  • f''(x) = 0 → possível inflexão");
                resultado.AppendLine("═══════════════════════════════════");

                txtResultadosDerivada.Text = resultado.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao calcular derivada: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnTerceiraDerivada_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string funcao = txtFuncaoDerivada.Text.Trim();
                string d1 = CalcularDerivada(funcao);
                string d2 = CalcularDerivada(d1);
                string d3 = CalcularDerivada(d2);

                StringBuilder resultado = new StringBuilder();
                resultado.AppendLine("═══════════════════════════════════");
                resultado.AppendLine("   🔬 TERCEIRA DERIVADA - f'''(x)");
                resultado.AppendLine("═══════════════════════════════════\n");
                resultado.AppendLine($"f(x)    = {funcao}");
                resultado.AppendLine($"f'(x)   = {d1}");
                resultado.AppendLine($"f''(x)  = {d2}");
                resultado.AppendLine($"f'''(x) = {d3}\n");
                resultado.AppendLine("───────────────────────────────────");
                resultado.AppendLine("A terceira derivada indica a taxa");
                resultado.AppendLine("de variação da concavidade.");
                resultado.AppendLine("═══════════════════════════════════");

                txtResultadosDerivada.Text = resultado.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnDerivadaPonto_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string funcao = txtFuncaoDerivada.Text.Trim();
                if (!double.TryParse(txtPontoDerivada.Text, out double x0))
                {
                    MessageBox.Show("Valor de x₀ inválido!", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                string derivada = CalcularDerivada(funcao);
                double valorDerivada = AvaliarFuncao(derivada, x0);
                double valorFuncao = AvaliarFuncao(funcao, x0);

                StringBuilder resultado = new StringBuilder();
                resultado.AppendLine("═══════════════════════════════════");
                resultado.AppendLine("   🎯 DERIVADA NO PONTO x₀");
                resultado.AppendLine("═══════════════════════════════════\n");
                resultado.AppendLine($"Função: f(x) = {funcao}");
                resultado.AppendLine($"Derivada: f'(x) = {derivada}\n");
                resultado.AppendLine($"Ponto: x₀ = {x0}\n");
                resultado.AppendLine("───────────────────────────────────");
                resultado.AppendLine("RESULTADOS:");
                resultado.AppendLine("───────────────────────────────────");
                resultado.AppendLine($"f({x0}) = {valorFuncao:F4}");
                resultado.AppendLine($"f'({x0}) = {valorDerivada:F4}\n");
                resultado.AppendLine("Interpretação:");
                if (Math.Abs(valorDerivada) < 0.0001)
                    resultado.AppendLine("  • Ponto crítico (derivada ≈ 0)");
                else if (valorDerivada > 0)
                    resultado.AppendLine("  • Função crescente neste ponto");
                else
                    resultado.AppendLine("  • Função decrescente neste ponto");

                resultado.AppendLine($"\n  • Inclinação da reta tangente:");
                resultado.AppendLine($"    m = {valorDerivada:F4}");
                resultado.AppendLine($"\n  • Equação da reta tangente:");
                double b = valorFuncao - valorDerivada * x0;
                resultado.AppendLine($"    y = {valorDerivada:F4}x + {b:F4}");
                resultado.AppendLine("═══════════════════════════════════");

                txtResultadosDerivada.Text = resultado.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnAnaliseDerivada_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string funcao = txtFuncaoDerivada.Text.Trim();
                string d1 = CalcularDerivada(funcao);
                string d2 = CalcularDerivada(d1);

                StringBuilder resultado = new StringBuilder();
                resultado.AppendLine("═══════════════════════════════════");
                resultado.AppendLine("   📊 ANÁLISE COMPLETA DA FUNÇÃO");
                resultado.AppendLine("═══════════════════════════════════\n");
                resultado.AppendLine($"f(x)   = {funcao}");
                resultado.AppendLine($"f'(x)  = {d1}");
                resultado.AppendLine($"f''(x) = {d2}\n");

                resultado.AppendLine("───────────────────────────────────");
                resultado.AppendLine("📈 CRESCIMENTO E DECRESCIMENTO:");
                resultado.AppendLine("───────────────────────────────────");
                resultado.AppendLine("• Resolver f'(x) = 0 para encontrar");
                resultado.AppendLine("  pontos críticos");
                resultado.AppendLine("• f'(x) > 0: função crescente");
                resultado.AppendLine("• f'(x) < 0: função decrescente\n");

                resultado.AppendLine("───────────────────────────────────");
                resultado.AppendLine("📉 CONCAVIDADE:");
                resultado.AppendLine("───────────────────────────────────");
                resultado.AppendLine("• Resolver f''(x) = 0 para encontrar");
                resultado.AppendLine("  pontos de inflexão");
                resultado.AppendLine("• f''(x) > 0: côncava para cima (∪)");
                resultado.AppendLine("• f''(x) < 0: côncava para baixo (∩)\n");

                resultado.AppendLine("───────────────────────────────────");
                resultado.AppendLine("🎯 PONTOS CRÍTICOS:");
                resultado.AppendLine("───────────────────────────────────");
                resultado.AppendLine("Para classificar um ponto crítico x₀:");
                resultado.AppendLine("• Se f''(x₀) > 0: mínimo local");
                resultado.AppendLine("• Se f''(x₀) < 0: máximo local");
                resultado.AppendLine("• Se f''(x₀) = 0: teste inconclusivo");
                resultado.AppendLine("\n═══════════════════════════════════");

                txtResultadosDerivada.Text = resultado.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnPontosCriticos_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string funcao = txtFuncaoDerivada.Text.Trim();
                string derivada = CalcularDerivada(funcao);

                StringBuilder resultado = new StringBuilder();
                resultado.AppendLine("═══════════════════════════════════");
                resultado.AppendLine("   🎯 PONTOS CRÍTICOS");
                resultado.AppendLine("═══════════════════════════════════\n");
                resultado.AppendLine($"Função: f(x) = {funcao}");
                resultado.AppendLine($"Derivada: f'(x) = {derivada}\n");
                resultado.AppendLine("───────────────────────────────────");
                resultado.AppendLine("MÉTODO:");
                resultado.AppendLine("───────────────────────────────────");
                resultado.AppendLine("1. Resolver f'(x) = 0");
                resultado.AppendLine("2. Encontrar valores de x onde");
                resultado.AppendLine("   f'(x) não existe\n");
                resultado.AppendLine("Para encontrar pontos críticos:");
                resultado.AppendLine($"  Resolver: {derivada} = 0\n");
                resultado.AppendLine("───────────────────────────────────");
                resultado.AppendLine("CLASSIFICAÇÃO:");
                resultado.AppendLine("───────────────────────────────────");
                resultado.AppendLine("• Teste da 1ª derivada:");
                resultado.AppendLine("  - f' muda de + para -: máximo");
                resultado.AppendLine("  - f' muda de - para +: mínimo");
                resultado.AppendLine("\n• Teste da 2ª derivada:");
                resultado.AppendLine("  - f''(x) > 0: mínimo local");
                resultado.AppendLine("  - f''(x) < 0: máximo local");
                resultado.AppendLine("═══════════════════════════════════");

                txtResultadosDerivada.Text = resultado.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnConcavidade_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string funcao = txtFuncaoDerivada.Text.Trim();
                string d1 = CalcularDerivada(funcao);
                string d2 = CalcularDerivada(d1);

                StringBuilder resultado = new StringBuilder();
                resultado.AppendLine("═══════════════════════════════════");
                resultado.AppendLine("   📈 ANÁLISE DE CONCAVIDADE");
                resultado.AppendLine("═══════════════════════════════════\n");
                resultado.AppendLine($"f(x)   = {funcao}");
                resultado.AppendLine($"f'(x)  = {d1}");
                resultado.AppendLine($"f''(x) = {d2}\n");
                resultado.AppendLine("───────────────────────────────────");
                resultado.AppendLine("MÉTODO:");
                resultado.AppendLine("───────────────────────────────────");
                resultado.AppendLine("1. Calcular f''(x)");
                resultado.AppendLine("2. Resolver f''(x) = 0 para pontos");
                resultado.AppendLine("   de inflexão candidatos");
                resultado.AppendLine("3. Testar sinais em intervalos\n");
                resultado.AppendLine($"Resolver: {d2} = 0\n");
                resultado.AppendLine("───────────────────────────────────");
                resultado.AppendLine("INTERPRETAÇÃO:");
                resultado.AppendLine("───────────────────────────────────");
                resultado.AppendLine("• f''(x) > 0:");
                resultado.AppendLine("  - Côncava para cima (∪)");
                resultado.AppendLine("  - Parábola abrindo para cima");
                resultado.AppendLine("\n• f''(x) < 0:");
                resultado.AppendLine("  - Côncava para baixo (∩)");
                resultado.AppendLine("  - Parábola abrindo para baixo");
                resultado.AppendLine("\n• f''(x) = 0:");
                resultado.AppendLine("  - Possível ponto de inflexão");
                resultado.AppendLine("  - Mudança de concavidade");
                resultado.AppendLine("═══════════════════════════════════");

                txtResultadosDerivada.Text = resultado.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region INTEGRAIS

        private void BtnIntegralIndefinida_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string funcao = txtFuncaoIntegral.Text.Trim();
                if (string.IsNullOrEmpty(funcao))
                {
                    MessageBox.Show("Por favor, insira uma função!", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string integral = CalcularIntegral(funcao);

                StringBuilder resultado = new StringBuilder();
                resultado.AppendLine("═══════════════════════════════════");
                resultado.AppendLine("   ∫ INTEGRAL INDEFINIDA");
                resultado.AppendLine("═══════════════════════════════════\n");
                resultado.AppendLine($"∫ ({funcao}) dx\n");
                resultado.AppendLine("───────────────────────────────────");
                resultado.AppendLine("RESULTADO:");
                resultado.AppendLine("───────────────────────────────────");
                resultado.AppendLine($"  = {integral} + C\n");
                resultado.AppendLine("Onde C é a constante de integração\n");
                resultado.AppendLine("───────────────────────────────────");
                resultado.AppendLine("PROPRIEDADES:");
                resultado.AppendLine("───────────────────────────────────");
                resultado.AppendLine("• ∫ k·f(x)dx = k·∫ f(x)dx");
                resultado.AppendLine("• ∫ [f(x)±g(x)]dx = ∫f(x)dx ± ∫g(x)dx");
                resultado.AppendLine("• d/dx[∫f(x)dx] = f(x)");
                resultado.AppendLine("═══════════════════════════════════");

                txtResultadosIntegral.Text = resultado.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnIntegralDefinida_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string funcao = txtFuncaoIntegral.Text.Trim();
                if (!double.TryParse(txtLimiteInferior.Text, out double a) ||
                    !double.TryParse(txtLimiteSuperior.Text, out double b))
                {
                    MessageBox.Show("Limites de integração inválidos!", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                string integral = CalcularIntegral(funcao);
                double Fa = AvaliarFuncao(integral, a);
                double Fb = AvaliarFuncao(integral, b);
                double resultado_valor = Fb - Fa;

                StringBuilder resultado = new StringBuilder();
                resultado.AppendLine("═══════════════════════════════════");
                resultado.AppendLine("   ∫ᵃᵇ INTEGRAL DEFINIDA");
                resultado.AppendLine("═══════════════════════════════════\n");
                resultado.AppendLine($"∫[{a}→{b}] ({funcao}) dx\n");
                resultado.AppendLine("───────────────────────────────────");
                resultado.AppendLine("PASSO 1: Integral indefinida");
                resultado.AppendLine("───────────────────────────────────");
                resultado.AppendLine($"F(x) = {integral}\n");
                resultado.AppendLine("───────────────────────────────────");
                resultado.AppendLine("PASSO 2: Teorema Fundamental");
                resultado.AppendLine("───────────────────────────────────");
                resultado.AppendLine($"F({b}) = {Fb:F6}");
                resultado.AppendLine($"F({a}) = {Fa:F6}\n");
                resultado.AppendLine("───────────────────────────────────");
                resultado.AppendLine("RESULTADO:");
                resultado.AppendLine("───────────────────────────────────");
                resultado.AppendLine($"∫[{a}→{b}] f(x)dx = F({b}) - F({a})");
                resultado.AppendLine($"                 = {Fb:F6} - {Fa:F6}");
                resultado.AppendLine($"                 = {resultado_valor:F6}\n");
                resultado.AppendLine("Interpretação geométrica:");
                resultado.AppendLine($"  Área = {Math.Abs(resultado_valor):F6} u²");
                resultado.AppendLine("═══════════════════════════════════");

                txtResultadosIntegral.Text = resultado.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnAreaCurva_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string funcao = txtFuncaoIntegral.Text.Trim();
                if (!double.TryParse(txtLimiteInferior.Text, out double a) ||
                    !double.TryParse(txtLimiteSuperior.Text, out double b))
                {
                    MessageBox.Show("Limites inválidos!", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                string integral = CalcularIntegral(funcao);
                double Fa = AvaliarFuncao(integral, a);
                double Fb = AvaliarFuncao(integral, b);
                double area = Math.Abs(Fb - Fa);

                StringBuilder resultado = new StringBuilder();
                resultado.AppendLine("═══════════════════════════════════");
                resultado.AppendLine("   📊 ÁREA SOB A CURVA");
                resultado.AppendLine("═══════════════════════════════════\n");
                resultado.AppendLine($"Função: f(x) = {funcao}");
                resultado.AppendLine($"Intervalo: [{a}, {b}]\n");
                resultado.AppendLine("───────────────────────────────────");
                resultado.AppendLine("CÁLCULO:");
                resultado.AppendLine("───────────────────────────────────");
                resultado.AppendLine($"Área = |∫[{a}→{b}] f(x)dx|");
                resultado.AppendLine($"     = |F({b}) - F({a})|");
                resultado.AppendLine($"     = |{Fb:F4} - {Fa:F4}|");
                resultado.AppendLine($"     = {area:F6} unidades²\n");
                resultado.AppendLine("───────────────────────────────────");
                resultado.AppendLine("INTERPRETAÇÃO:");
                resultado.AppendLine("───────────────────────────────────");
                resultado.AppendLine("A área representa a região entre:");
                resultado.AppendLine($"  • A curva f(x) = {funcao}");
                resultado.AppendLine("  • O eixo x");
                resultado.AppendLine($"  • As retas x = {a} e x = {b}");
                resultado.AppendLine("═══════════════════════════════════");

                txtResultadosIntegral.Text = resultado.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnAnaliseIntegral_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string funcao = txtFuncaoIntegral.Text.Trim();
                string integral = CalcularIntegral(funcao);

                StringBuilder resultado = new StringBuilder();
                resultado.AppendLine("═══════════════════════════════════");
                resultado.AppendLine("   📊 ANÁLISE COMPLETA - INTEGRAIS");
                resultado.AppendLine("═══════════════════════════════════\n");
                resultado.AppendLine($"f(x) = {funcao}\n");
                resultado.AppendLine("───────────────────────────────────");
                resultado.AppendLine("∫ INTEGRAL INDEFINIDA:");
                resultado.AppendLine("───────────────────────────────────");
                resultado.AppendLine($"∫ f(x)dx = {integral} + C\n");
                resultado.AppendLine("───────────────────────────────────");
                resultado.AppendLine("📚 TEOREMA FUNDAMENTAL DO CÁLCULO:");
                resultado.AppendLine("───────────────────────────────────");
                resultado.AppendLine("Parte 1:");
                resultado.AppendLine("  Se F'(x) = f(x), então:");
                resultado.AppendLine("  ∫[a→b] f(x)dx = F(b) - F(a)\n");
                resultado.AppendLine("Parte 2:");
                resultado.AppendLine("  d/dx[∫[a→x] f(t)dt] = f(x)\n");
                resultado.AppendLine("───────────────────────────────────");
                resultado.AppendLine("🎯 APLICAÇÕES:");
                resultado.AppendLine("───────────────────────────────────");
                resultado.AppendLine("• Cálculo de áreas");
                resultado.AppendLine("• Volumes de sólidos de revolução");
                resultado.AppendLine("• Comprimento de arco");
                resultado.AppendLine("• Trabalho e energia");
                resultado.AppendLine("• Problemas de acumulação");
                resultado.AppendLine("═══════════════════════════════════");

                txtResultadosIntegral.Text = resultado.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnMetodosNumericos_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string funcao = txtFuncaoIntegral.Text.Trim();
                if (!double.TryParse(txtLimiteInferior.Text, out double a) ||
                    !double.TryParse(txtLimiteSuperior.Text, out double b))
                {
                    MessageBox.Show("Limites inválidos!", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                int n = 100; // número de subdivisões
                double h = (b - a) / n;

                // Método do Trapézio
                double trapezio = 0;
                for (int i = 0; i <= n; i++)
                {
                    double x = a + i * h;
                    double fx = AvaliarFuncao(funcao, x);
                    if (i == 0 || i == n)
                        trapezio += fx;
                    else
                        trapezio += 2 * fx;
                }
                trapezio *= h / 2;

                // Método de Simpson (n deve ser par)
                double simpson = AvaliarFuncao(funcao, a) + AvaliarFuncao(funcao, b);
                for (int i = 1; i < n; i++)
                {
                    double x = a + i * h;
                    double fx = AvaliarFuncao(funcao, x);
                    simpson += (i % 2 == 0 ? 2 : 4) * fx;
                }
                simpson *= h / 3;

                StringBuilder resultado = new StringBuilder();
                resultado.AppendLine("═══════════════════════════════════");
                resultado.AppendLine("   🔢 MÉTODOS NUMÉRICOS");
                resultado.AppendLine("═══════════════════════════════════\n");
                resultado.AppendLine($"∫[{a}→{b}] ({funcao}) dx\n");
                resultado.AppendLine($"Subdivisões: n = {n}\n");
                resultado.AppendLine("───────────────────────────────────");
                resultado.AppendLine("📐 MÉTODO DO TRAPÉZIO:");
                resultado.AppendLine("───────────────────────────────────");
                resultado.AppendLine($"Resultado ≈ {trapezio:F8}\n");
                resultado.AppendLine("Fórmula:");
                resultado.AppendLine("  ∫[a→b]f(x)dx ≈ h/2[f(x₀)+2f(x₁)+");
                resultado.AppendLine("                      ...+2f(xₙ₋₁)+f(xₙ)]\n");
                resultado.AppendLine("───────────────────────────────────");
                resultado.AppendLine("📊 REGRA DE SIMPSON:");
                resultado.AppendLine("───────────────────────────────────");
                resultado.AppendLine($"Resultado ≈ {simpson:F8}\n");
                resultado.AppendLine("Fórmula:");
                resultado.AppendLine("  ∫[a→b]f(x)dx ≈ h/3[f(x₀)+4f(x₁)+");
                resultado.AppendLine("                      2f(x₂)+...+f(xₙ)]\n");
                resultado.AppendLine("───────────────────────────────────");
                resultado.AppendLine("📈 COMPARAÇÃO:");
                resultado.AppendLine("───────────────────────────────────");
                double diferenca = Math.Abs(trapezio - simpson);
                resultado.AppendLine($"Diferença: {diferenca:F8}");
                resultado.AppendLine("\nO método de Simpson geralmente é");
                resultado.AppendLine("mais preciso que o do Trapézio.");
                resultado.AppendLine("═══════════════════════════════════");

                txtResultadosIntegral.Text = resultado.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region LIMITES

        private void BtnLimitePonto_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string funcao = txtFuncaoLimite.Text.Trim();
                if (!double.TryParse(txtPontoLimite.Text, out double x0))
                {
                    MessageBox.Show("Ponto inválido!", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Aproximação numérica do limite
                double delta = 0.0001;
                double limEsq = AvaliarFuncao(funcao, x0 - delta);
                double limDir = AvaliarFuncao(funcao, x0 + delta);
                double valorPonto = double.NaN;

                try { valorPonto = AvaliarFuncao(funcao, x0); } catch { }

                StringBuilder resultado = new StringBuilder();
                resultado.AppendLine("═══════════════════════════════════");
                resultado.AppendLine("   🎯 LIMITE NO PONTO");
                resultado.AppendLine("═══════════════════════════════════\n");
                resultado.AppendLine($"lim[x→{x0}] ({funcao})\n");
                resultado.AppendLine("───────────────────────────────────");
                resultado.AppendLine("APROXIMAÇÃO NUMÉRICA:");
                resultado.AppendLine("───────────────────────────────────");
                resultado.AppendLine($"Limite pela esquerda: {limEsq:F6}");
                resultado.AppendLine($"Limite pela direita:  {limDir:F6}\n");

                if (double.IsNaN(valorPonto))
                    resultado.AppendLine($"f({x0}) não está definida\n");
                else
                    resultado.AppendLine($"f({x0}) = {valorPonto:F6}\n");

                resultado.AppendLine("───────────────────────────────────");
                resultado.AppendLine("CONCLUSÃO:");
                resultado.AppendLine("───────────────────────────────────");

                if (Math.Abs(limEsq - limDir) < 0.001)
                {
                    resultado.AppendLine($"✓ O limite existe!");
                    resultado.AppendLine($"  lim[x→{x0}] f(x) ≈ {limEsq:F6}\n");

                    if (!double.IsNaN(valorPonto))
                    {
                        if (Math.Abs(limEsq - valorPonto) < 0.001)
                            resultado.AppendLine("✓ A função é CONTÍNUA em x₀");
                        else
                            resultado.AppendLine("✗ A função é DESCONTÍNUA em x₀");
                    }
                }
                else
                {
                    resultado.AppendLine("✗ O limite NÃO existe!");
                    resultado.AppendLine("  (limites laterais diferentes)");
                }

                resultado.AppendLine("═══════════════════════════════════");

                txtResultadosLimite.Text = resultado.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnLimiteEsquerda_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string funcao = txtFuncaoLimite.Text.Trim();
                if (!double.TryParse(txtPontoLimite.Text, out double x0))
                {
                    MessageBox.Show("Ponto inválido!", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                StringBuilder resultado = new StringBuilder();
                resultado.AppendLine("═══════════════════════════════════");
                resultado.AppendLine("   ← LIMITE LATERAL ESQUERDO");
                resultado.AppendLine("═══════════════════════════════════\n");
                resultado.AppendLine($"lim[x→{x0}⁻] ({funcao})\n");
                resultado.AppendLine("───────────────────────────────────");
                resultado.AppendLine("APROXIMAÇÃO:");
                resultado.AppendLine("───────────────────────────────────");

                double[] deltas = { 0.1, 0.01, 0.001, 0.0001, 0.00001 };
                foreach (double d in deltas)
                {
                    double x = x0 - d;
                    double fx = AvaliarFuncao(funcao, x);
                    resultado.AppendLine($"f({x:F5}) = {fx:F8}");
                }

                double limiteEsq = AvaliarFuncao(funcao, x0 - 0.00001);
                resultado.AppendLine($"\n───────────────────────────────────");
                resultado.AppendLine($"lim[x→{x0}⁻] f(x) ≈ {limiteEsq:F6}");
                resultado.AppendLine("═══════════════════════════════════");

                txtResultadosLimite.Text = resultado.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnLimiteDireita_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string funcao = txtFuncaoLimite.Text.Trim();
                if (!double.TryParse(txtPontoLimite.Text, out double x0))
                {
                    MessageBox.Show("Ponto inválido!", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                StringBuilder resultado = new StringBuilder();
                resultado.AppendLine("═══════════════════════════════════");
                resultado.AppendLine("   → LIMITE LATERAL DIREITO");
                resultado.AppendLine("═══════════════════════════════════\n");
                resultado.AppendLine($"lim[x→{x0}⁺] ({funcao})\n");
                resultado.AppendLine("───────────────────────────────────");
                resultado.AppendLine("APROXIMAÇÃO:");
                resultado.AppendLine("───────────────────────────────────");

                double[] deltas = { 0.1, 0.01, 0.001, 0.0001, 0.00001 };
                foreach (double d in deltas)
                {
                    double x = x0 + d;
                    double fx = AvaliarFuncao(funcao, x);
                    resultado.AppendLine($"f({x:F5}) = {fx:F8}");
                }

                double limiteDir = AvaliarFuncao(funcao, x0 + 0.00001);
                resultado.AppendLine($"\n───────────────────────────────────");
                resultado.AppendLine($"lim[x→{x0}⁺] f(x) ≈ {limiteDir:F6}");
                resultado.AppendLine("═══════════════════════════════════");

                txtResultadosLimite.Text = resultado.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnLimiteInfinito_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string funcao = txtFuncaoLimite.Text.Trim();

                StringBuilder resultado = new StringBuilder();
                resultado.AppendLine("═══════════════════════════════════");
                resultado.AppendLine("   ∞ LIMITE NO INFINITO");
                resultado.AppendLine("═══════════════════════════════════\n");
                resultado.AppendLine($"lim[x→∞] ({funcao})\n");
                resultado.AppendLine("───────────────────────────────────");
                resultado.AppendLine("APROXIMAÇÃO:");
                resultado.AppendLine("───────────────────────────────────");

                double[] xValues = { 10, 100, 1000, 10000, 100000 };
                foreach (double x in xValues)
                {
                    try
                    {
                        double fx = AvaliarFuncao(funcao, x);
                        resultado.AppendLine($"f({x,8}) = {fx:F8}");
                    }
                    catch
                    {
                        resultado.AppendLine($"f({x,8}) = indefinido");
                    }
                }

                try
                {
                    double limInf = AvaliarFuncao(funcao, 100000);
                    resultado.AppendLine($"\n───────────────────────────────────");
                    resultado.AppendLine($"lim[x→∞] f(x) ≈ {limInf:F6}");
                }
                catch
                {
                    resultado.AppendLine($"\n───────────────────────────────────");
                    resultado.AppendLine("O limite pode ser ±∞ ou não existir");
                }

                resultado.AppendLine("\n───────────────────────────────────");
                resultado.AppendLine("DICA:");
                resultado.AppendLine("───────────────────────────────────");
                resultado.AppendLine("Para funções racionais:");
                resultado.AppendLine("• Compare graus do numerador e");
                resultado.AppendLine("  denominador");
                resultado.AppendLine("• Divida por maior potência de x");
                resultado.AppendLine("═══════════════════════════════════");

                txtResultadosLimite.Text = resultado.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region FUNÇÕES AUXILIARES

        private string CalcularDerivada(string funcao)
        {
            // Simplificação: implementação básica de derivadas
            funcao = funcao.Replace(" ", "").ToLower();

            // Derivada de polinômios: x^n -> n*x^(n-1)
            var matchPotencia = Regex.Match(funcao, @"(\d*\.?\d*)\*?x\^(\d+)");
            if (matchPotencia.Success)
            {
                double coef = string.IsNullOrEmpty(matchPotencia.Groups[1].Value) ? 1 :
                             double.Parse(matchPotencia.Groups[1].Value.Replace("*", ""));
                int exp = int.Parse(matchPotencia.Groups[2].Value);

                if (exp == 1)
                    return coef.ToString("F2");

                double novoCoef = coef * exp;
                int novoExp = exp - 1;

                if (novoExp == 1)
                    return $"{novoCoef:F2}*x";
                return $"{novoCoef:F2}*x^{novoExp}";
            }

            // Derivada de x^2 (sem coeficiente explícito)
            if (funcao.Contains("x^2"))
                return "2*x";

            // Derivada de x^3
            if (funcao.Contains("x^3"))
                return "3*x^2";

            // Derivada de ax + b
            var matchLinear = Regex.Match(funcao, @"(\d+\.?\d*)\*?x");
            if (matchLinear.Success)
            {
                string coef = matchLinear.Groups[1].Value;
                return string.IsNullOrEmpty(coef) ? "1" : coef;
            }

            // Derivada de x
            if (funcao == "x")
                return "1";

            // Derivada de constante
            if (double.TryParse(funcao, out _))
                return "0";

            // Para funções mais complexas, retornar expressão simbólica
            if (funcao.Contains("sin"))
                return funcao.Replace("sin", "cos");
            if (funcao.Contains("cos"))
                return "-" + funcao.Replace("cos", "sin");
            if (funcao.Contains("e^"))
                return funcao;
            if (funcao.Contains("ln"))
                return "1/x";

            return "d/dx[" + funcao + "]";
        }

        private string CalcularIntegral(string funcao)
        {
            // Simplificação: implementação básica de integrais
            funcao = funcao.Replace(" ", "").ToLower();

            // Integral de x^n -> x^(n+1)/(n+1)
            var matchPotencia = Regex.Match(funcao, @"(\d*\.?\d*)\*?x\^(\d+)");
            if (matchPotencia.Success)
            {
                double coef = string.IsNullOrEmpty(matchPotencia.Groups[1].Value) ? 1 :
                             double.Parse(matchPotencia.Groups[1].Value.Replace("*", ""));
                int exp = int.Parse(matchPotencia.Groups[2].Value);

                double novoCoef = coef / (exp + 1);
                int novoExp = exp + 1;

                return $"{novoCoef:F4}*x^{novoExp}";
            }

            // Integral de x^2
            if (funcao.Contains("x^2"))
                return "x^3/3";

            // Integral de x
            if (funcao == "x")
                return "x^2/2";

            // Integral de ax
            var matchLinear = Regex.Match(funcao, @"(\d+\.?\d*)\*?x");
            if (matchLinear.Success)
            {
                double coef = double.Parse(matchLinear.Groups[1].Value.Replace("*", ""));
                return $"{coef / 2:F4}*x^2";
            }

            // Integral de constante
            if (double.TryParse(funcao, out double constante))
                return $"{constante}*x";

            // Funções especiais
            if (funcao.Contains("sin"))
                return "-" + funcao.Replace("sin", "cos");
            if (funcao.Contains("cos"))
                return funcao.Replace("cos", "sin");
            if (funcao.Contains("e^"))
                return funcao;
            if (funcao == "1/x")
                return "ln|x|";

            return "∫[" + funcao + "]dx";
        }

        private double AvaliarFuncao(string funcao, double x)
        {
            // Substituir x pelo valor e avaliar
            funcao = funcao.Replace(" ", "").ToLower();

            // Substituir x por seu valor
            funcao = Regex.Replace(funcao, @"(\d)x", "$1*" + x.ToString());
            funcao = funcao.Replace("x", x.ToString());

            // Avaliar potências
            while (funcao.Contains("^"))
            {
                var match = Regex.Match(funcao, @"([\d.]+)\^([\d.]+)");
                if (match.Success)
                {
                    double baseNum = double.Parse(match.Groups[1].Value);
                    double exp = double.Parse(match.Groups[2].Value);
                    double resultado = Math.Pow(baseNum, exp);
                    funcao = funcao.Replace(match.Value, resultado.ToString());
                }
                else
                    break;
            }

            // Avaliar funções trigonométricas e exponenciais
            funcao = Regex.Replace(funcao, @"sin\(([\d.]+)\)", m => Math.Sin(double.Parse(m.Groups[1].Value)).ToString());
            funcao = Regex.Replace(funcao, @"cos\(([\d.]+)\)", m => Math.Cos(double.Parse(m.Groups[1].Value)).ToString());
            funcao = Regex.Replace(funcao, @"tan\(([\d.]+)\)", m => Math.Tan(double.Parse(m.Groups[1].Value)).ToString());
            funcao = Regex.Replace(funcao, @"ln\(([\d.]+)\)", m => Math.Log(double.Parse(m.Groups[1].Value)).ToString());
            funcao = Regex.Replace(funcao, @"e\^([\d.]+)", m => Math.Exp(double.Parse(m.Groups[1].Value)).ToString());

            // Avaliar a expressão matemática
            try
            {
                var dataTable = new System.Data.DataTable();
                var resultado = dataTable.Compute(funcao, "");
                return Convert.ToDouble(resultado);
            }
            catch
            {
                throw new Exception("Não foi possível avaliar a função. Verifique a sintaxe.");
            }
        }

        #endregion
    }
}