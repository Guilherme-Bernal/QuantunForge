using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Text;

namespace Quantun_Forge.src.views.ModuloEnsino.Matematica
{
    public partial class LogaritmoExponenciaisWindow : Window
    {
        public LogaritmoExponenciaisWindow()
        {
            InitializeComponent();
            ConfigurarEventos();
        }

        private void ConfigurarEventos()
        {
            // Eventos de Logaritmos
            btnCalcularLog.Click += BtnCalcularLog_Click;
            btnLogNatural.Click += BtnLogNatural_Click;
            btnLog10.Click += BtnLog10_Click;
            btnLogProduto.Click += BtnLogProduto_Click;
            btnLogQuociente.Click += BtnLogQuociente_Click;
            btnLogPotencia.Click += BtnLogPotencia_Click;
            btnMudancaBase.Click += BtnMudancaBase_Click;
            btnTodasPropriedades.Click += BtnTodasPropriedades_Click;
            btnResolverEqLog.Click += BtnResolverEqLog_Click;
            btnLimparLog.Click += (s, e) => txtResultadosLog.Text = "Aguardando cálculos...";

            // Eventos de Exponenciais
            btnCalcularExp.Click += BtnCalcularExp_Click;
            btnEuler.Click += BtnEuler_Click;
            btnDezPotencia.Click += BtnDezPotencia_Click;
            btnExpProduto.Click += BtnExpProduto_Click;
            btnExpQuociente.Click += BtnExpQuociente_Click;
            btnExpPotencia.Click += BtnExpPotencia_Click;
            btnExpRaiz.Click += BtnExpRaiz_Click;
            btnTodasPropExp.Click += BtnTodasPropExp_Click;
            btnResolverEqExp.Click += BtnResolverEqExp_Click;
            btnCrescimento.Click += BtnCrescimento_Click;
            btnLimparExp.Click += (s, e) => txtResultadosExp.Text = "Aguardando cálculos...";

            // Eventos de Conversor
            btnLogParaExp.Click += BtnLogParaExp_Click;
            btnExpParaLog.Click += BtnExpParaLog_Click;
            btnTabelaPot2.Click += BtnTabelaPot2_Click;
            btnTabelaPot10.Click += BtnTabelaPot10_Click;
            btnTabelaLn.Click += BtnTabelaLn_Click;
            btnNumeroE.Click += BtnNumeroE_Click;
            btnLimparConversor.Click += (s, e) => txtResultadosConversor.Text = "Aguardando operações...";
        }

        #region LOGARITMOS

        private void BtnCalcularLog_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!double.TryParse(txtBaseLog.Text, out double baseLog) ||
                    !double.TryParse(txtValorLog.Text, out double valor))
                {
                    MessageBox.Show("Valores inválidos!", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (baseLog <= 0 || baseLog == 1 || valor <= 0)
                {
                    MessageBox.Show("Base deve ser > 0 e ≠ 1, e valor deve ser > 0!", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                double resultado = Math.Log(valor, baseLog);

                StringBuilder res = new StringBuilder();
                res.AppendLine("═══════════════════════════════════");
                res.AppendLine("   📐 CÁLCULO DE LOGARITMO");
                res.AppendLine("═══════════════════════════════════\n");
                res.AppendLine($"log_{baseLog}({valor})\n");
                res.AppendLine("───────────────────────────────────");
                res.AppendLine("RESULTADO:");
                res.AppendLine("───────────────────────────────────");
                res.AppendLine($"log_{baseLog}({valor}) = {resultado:F6}\n");
                res.AppendLine("───────────────────────────────────");
                res.AppendLine("VERIFICAÇÃO:");
                res.AppendLine("───────────────────────────────────");
                double verificacao = Math.Pow(baseLog, resultado);
                res.AppendLine($"{baseLog}^{resultado:F6} = {verificacao:F6}");
                res.AppendLine($"✓ Conferido: {valor:F6} ≈ {verificacao:F6}");
                res.AppendLine("═══════════════════════════════════");

                txtResultadosLog.Text = res.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnLogNatural_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!double.TryParse(txtValorLog.Text, out double valor) || valor <= 0)
                {
                    MessageBox.Show("Valor deve ser > 0!", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                double resultado = Math.Log(valor);

                StringBuilder res = new StringBuilder();
                res.AppendLine("═══════════════════════════════════");
                res.AppendLine("   📊 LOGARITMO NATURAL (ln)");
                res.AppendLine("═══════════════════════════════════\n");
                res.AppendLine($"ln({valor})\n");
                res.AppendLine("───────────────────────────────────");
                res.AppendLine("DEFINIÇÃO:");
                res.AppendLine("───────────────────────────────────");
                res.AppendLine("ln(x) = log_e(x)");
                res.AppendLine($"onde e ≈ {Math.E:F10}\n");
                res.AppendLine("───────────────────────────────────");
                res.AppendLine("RESULTADO:");
                res.AppendLine("───────────────────────────────────");
                res.AppendLine($"ln({valor}) = {resultado:F8}\n");
                res.AppendLine("───────────────────────────────────");
                res.AppendLine("VERIFICAÇÃO:");
                res.AppendLine("───────────────────────────────────");
                double verificacao = Math.Exp(resultado);
                res.AppendLine($"e^{resultado:F8} = {verificacao:F8}");
                res.AppendLine("═══════════════════════════════════");

                txtResultadosLog.Text = res.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnLog10_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!double.TryParse(txtValorLog.Text, out double valor) || valor <= 0)
                {
                    MessageBox.Show("Valor deve ser > 0!", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                double resultado = Math.Log10(valor);

                StringBuilder res = new StringBuilder();
                res.AppendLine("═══════════════════════════════════");
                res.AppendLine("   🔟 LOGARITMO DECIMAL (log₁₀)");
                res.AppendLine("═══════════════════════════════════\n");
                res.AppendLine($"log₁₀({valor})\n");
                res.AppendLine("───────────────────────────────────");
                res.AppendLine("RESULTADO:");
                res.AppendLine("───────────────────────────────────");
                res.AppendLine($"log₁₀({valor}) = {resultado:F8}\n");
                res.AppendLine("───────────────────────────────────");
                res.AppendLine("VERIFICAÇÃO:");
                res.AppendLine("───────────────────────────────────");
                double verificacao = Math.Pow(10, resultado);
                res.AppendLine($"10^{resultado:F8} = {verificacao:F8}");
                res.AppendLine("\n───────────────────────────────────");
                res.AppendLine("CURIOSIDADE:");
                res.AppendLine("───────────────────────────────────");
                res.AppendLine("O logaritmo decimal é usado em:");
                res.AppendLine("• pH (acidez)");
                res.AppendLine("• Decibéis (som)");
                res.AppendLine("• Escala Richter (terremotos)");
                res.AppendLine("═══════════════════════════════════");

                txtResultadosLog.Text = res.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnLogProduto_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder res = new StringBuilder();
            res.AppendLine("═══════════════════════════════════");
            res.AppendLine("   📐 PROPRIEDADE: PRODUTO");
            res.AppendLine("═══════════════════════════════════\n");
            res.AppendLine("FÓRMULA:");
            res.AppendLine("───────────────────────────────────");
            res.AppendLine("log_b(x · y) = log_b(x) + log_b(y)\n");
            res.AppendLine("───────────────────────────────────");
            res.AppendLine("EXEMPLO:");
            res.AppendLine("───────────────────────────────────");
            res.AppendLine("log₂(8 · 4) = log₂(8) + log₂(4)");
            res.AppendLine("log₂(32)    = 3 + 2");
            res.AppendLine("5           = 5 ✓\n");
            res.AppendLine("───────────────────────────────────");
            res.AppendLine("INTERPRETAÇÃO:");
            res.AppendLine("───────────────────────────────────");
            res.AppendLine("O logaritmo transforma produto em");
            res.AppendLine("soma, facilitando cálculos com");
            res.AppendLine("números muito grandes.");
            res.AppendLine("═══════════════════════════════════");

            txtResultadosLog.Text = res.ToString();
        }

        private void BtnLogQuociente_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder res = new StringBuilder();
            res.AppendLine("═══════════════════════════════════");
            res.AppendLine("   ➗ PROPRIEDADE: QUOCIENTE");
            res.AppendLine("═══════════════════════════════════\n");
            res.AppendLine("FÓRMULA:");
            res.AppendLine("───────────────────────────────────");
            res.AppendLine("log_b(x / y) = log_b(x) - log_b(y)\n");
            res.AppendLine("───────────────────────────────────");
            res.AppendLine("EXEMPLO:");
            res.AppendLine("───────────────────────────────────");
            res.AppendLine("log₁₀(1000 / 10) = log₁₀(1000) - log₁₀(10)");
            res.AppendLine("log₁₀(100)       = 3 - 1");
            res.AppendLine("2                = 2 ✓\n");
            res.AppendLine("───────────────────────────────────");
            res.AppendLine("INTERPRETAÇÃO:");
            res.AppendLine("───────────────────────────────────");
            res.AppendLine("O logaritmo transforma divisão em");
            res.AppendLine("subtração.");
            res.AppendLine("═══════════════════════════════════");

            txtResultadosLog.Text = res.ToString();
        }

        private void BtnLogPotencia_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder res = new StringBuilder();
            res.AppendLine("═══════════════════════════════════");
            res.AppendLine("   📈 PROPRIEDADE: POTÊNCIA");
            res.AppendLine("═══════════════════════════════════\n");
            res.AppendLine("FÓRMULA:");
            res.AppendLine("───────────────────────────────────");
            res.AppendLine("log_b(x^n) = n · log_b(x)\n");
            res.AppendLine("───────────────────────────────────");
            res.AppendLine("EXEMPLO:");
            res.AppendLine("───────────────────────────────────");
            res.AppendLine("log₂(8³) = 3 · log₂(8)");
            res.AppendLine("log₂(512) = 3 · 3");
            res.AppendLine("9         = 9 ✓\n");
            res.AppendLine("───────────────────────────────────");
            res.AppendLine("CASOS ESPECIAIS:");
            res.AppendLine("───────────────────────────────────");
            res.AppendLine("• log_b(√x) = (1/2)·log_b(x)");
            res.AppendLine("• log_b(ⁿ√x) = (1/n)·log_b(x)");
            res.AppendLine("• log_b(1/x) = -log_b(x)");
            res.AppendLine("═══════════════════════════════════");

            txtResultadosLog.Text = res.ToString();
        }

        private void BtnMudancaBase_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder res = new StringBuilder();
            res.AppendLine("═══════════════════════════════════");
            res.AppendLine("   🔄 MUDANÇA DE BASE");
            res.AppendLine("═══════════════════════════════════\n");
            res.AppendLine("FÓRMULA:");
            res.AppendLine("───────────────────────────────────");
            res.AppendLine("         log_c(x)");
            res.AppendLine("log_b(x) = ─────────");
            res.AppendLine("         log_c(b)\n");
            res.AppendLine("───────────────────────────────────");
            res.AppendLine("EXEMPLO:");
            res.AppendLine("───────────────────────────────────");
            res.AppendLine("Calcular log₂(10) usando log₁₀:\n");
            res.AppendLine("         log₁₀(10)");
            res.AppendLine("log₂(10) = ─────────");
            res.AppendLine("         log₁₀(2)");
            res.AppendLine("");
            double resultado = Math.Log10(10) / Math.Log10(2);
            res.AppendLine($"         1");
            res.AppendLine($"       = ─────── = {resultado:F6}");
            res.AppendLine($"         0.30103\n");
            res.AppendLine("───────────────────────────────────");
            res.AppendLine("APLICAÇÃO:");
            res.AppendLine("───────────────────────────────────");
            res.AppendLine("Permite calcular logaritmos em");
            res.AppendLine("qualquer base usando apenas ln ou");
            res.AppendLine("log₁₀ (disponíveis na calculadora).");
            res.AppendLine("═══════════════════════════════════");

            txtResultadosLog.Text = res.ToString();
        }

        private void BtnTodasPropriedades_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder res = new StringBuilder();
            res.AppendLine("═══════════════════════════════════");
            res.AppendLine("   📋 TODAS AS PROPRIEDADES");
            res.AppendLine("═══════════════════════════════════\n");
            res.AppendLine("DEFINIÇÃO:");
            res.AppendLine("───────────────────────────────────");
            res.AppendLine("log_b(x) = y  ⟺  b^y = x\n");
            res.AppendLine("───────────────────────────────────");
            res.AppendLine("PROPRIEDADES FUNDAMENTAIS:");
            res.AppendLine("───────────────────────────────────");
            res.AppendLine("1. log_b(x·y) = log_b(x) + log_b(y)");
            res.AppendLine("2. log_b(x/y) = log_b(x) - log_b(y)");
            res.AppendLine("3. log_b(x^n) = n·log_b(x)");
            res.AppendLine("4. log_b(b) = 1");
            res.AppendLine("5. log_b(1) = 0");
            res.AppendLine("6. b^(log_b(x)) = x");
            res.AppendLine("7. log_b(b^x) = x\n");
            res.AppendLine("───────────────────────────────────");
            res.AppendLine("MUDANÇA DE BASE:");
            res.AppendLine("───────────────────────────────────");
            res.AppendLine("log_b(x) = log_c(x) / log_c(b)\n");
            res.AppendLine("───────────────────────────────────");
            res.AppendLine("LOGARITMOS ESPECIAIS:");
            res.AppendLine("───────────────────────────────────");
            res.AppendLine("• ln(x)    = log_e(x)  (e ≈ 2.71828)");
            res.AppendLine("• log(x)   = log₁₀(x)");
            res.AppendLine("• lg(x)    = log₂(x)\n");
            res.AppendLine("───────────────────────────────────");
            res.AppendLine("CONDIÇÕES DE EXISTÊNCIA:");
            res.AppendLine("───────────────────────────────────");
            res.AppendLine("• x > 0  (logaritmando positivo)");
            res.AppendLine("• b > 0  (base positiva)");
            res.AppendLine("• b ≠ 1  (base diferente de 1)");
            res.AppendLine("═══════════════════════════════════");

            txtResultadosLog.Text = res.ToString();
        }

        private void BtnResolverEqLog_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!double.TryParse(txtBaseEqLog.Text, out double baseLog) ||
                    !double.TryParse(txtResultadoEqLog.Text, out double resultado))
                {
                    MessageBox.Show("Valores inválidos!", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (baseLog <= 0 || baseLog == 1)
                {
                    MessageBox.Show("Base deve ser > 0 e ≠ 1!", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                double x = Math.Pow(baseLog, resultado);

                StringBuilder res = new StringBuilder();
                res.AppendLine("═══════════════════════════════════");
                res.AppendLine("   ⚡ EQUAÇÃO LOGARÍTMICA");
                res.AppendLine("═══════════════════════════════════\n");
                res.AppendLine($"log_{baseLog}(x) = {resultado}\n");
                res.AppendLine("───────────────────────────────────");
                res.AppendLine("MÉTODO:");
                res.AppendLine("───────────────────────────────────");
                res.AppendLine("Se log_b(x) = y, então:");
                res.AppendLine("x = b^y\n");
                res.AppendLine("───────────────────────────────────");
                res.AppendLine("RESOLUÇÃO:");
                res.AppendLine("───────────────────────────────────");
                res.AppendLine($"x = {baseLog}^{resultado}");
                res.AppendLine($"x = {x:F6}\n");
                res.AppendLine("───────────────────────────────────");
                res.AppendLine("VERIFICAÇÃO:");
                res.AppendLine("───────────────────────────────────");
                double verificacao = Math.Log(x, baseLog);
                res.AppendLine($"log_{baseLog}({x:F6}) = {verificacao:F6}");
                res.AppendLine($"✓ Correto: {resultado:F6} ≈ {verificacao:F6}");
                res.AppendLine("═══════════════════════════════════");

                txtResultadosLog.Text = res.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region EXPONENCIAIS

        private void BtnCalcularExp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!double.TryParse(txtBaseExp.Text, out double baseExp) ||
                    !double.TryParse(txtExpoente.Text, out double expoente))
                {
                    MessageBox.Show("Valores inválidos!", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                double resultado = Math.Pow(baseExp, expoente);

                StringBuilder res = new StringBuilder();
                res.AppendLine("═══════════════════════════════════");
                res.AppendLine("   📈 CÁLCULO EXPONENCIAL");
                res.AppendLine("═══════════════════════════════════\n");
                res.AppendLine($"{baseExp}^{expoente}\n");
                res.AppendLine("───────────────────────────────────");
                res.AppendLine("RESULTADO:");
                res.AppendLine("───────────────────────────────────");
                res.AppendLine($"{baseExp}^{expoente} = {resultado:F8}\n");
                res.AppendLine("───────────────────────────────────");
                res.AppendLine("NOTAÇÃO CIENTÍFICA:");
                res.AppendLine("───────────────────────────────────");
                res.AppendLine($"{resultado:E6}\n");
                res.AppendLine("───────────────────────────────────");
                res.AppendLine("LOGARITMO INVERSO:");
                res.AppendLine("───────────────────────────────────");
                double logInverso = Math.Log(resultado, baseExp);
                res.AppendLine($"log_{baseExp}({resultado:F8}) = {logInverso:F6}");
                res.AppendLine("═══════════════════════════════════");

                txtResultadosExp.Text = res.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnEuler_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!double.TryParse(txtExpoente.Text, out double expoente))
                {
                    MessageBox.Show("Expoente inválido!", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                double resultado = Math.Exp(expoente);

                StringBuilder res = new StringBuilder();
                res.AppendLine("═══════════════════════════════════");
                res.AppendLine("   📊 FUNÇÃO EXPONENCIAL (e^x)");
                res.AppendLine("═══════════════════════════════════\n");
                res.AppendLine($"e^{expoente}\n");
                res.AppendLine("───────────────────────────────────");
                res.AppendLine("NÚMERO DE EULER:");
                res.AppendLine("───────────────────────────────────");
                res.AppendLine($"e ≈ {Math.E:F15}\n");
                res.AppendLine("───────────────────────────────────");
                res.AppendLine("RESULTADO:");
                res.AppendLine("───────────────────────────────────");
                res.AppendLine($"e^{expoente} = {resultado:F10}\n");
                res.AppendLine("───────────────────────────────────");
                res.AppendLine("LOGARITMO NATURAL:");
                res.AppendLine("───────────────────────────────────");
                double ln = Math.Log(resultado);
                res.AppendLine($"ln({resultado:F10}) = {ln:F6}");
                res.AppendLine("\n───────────────────────────────────");
                res.AppendLine("APLICAÇÕES:");
                res.AppendLine("───────────────────────────────────");
                res.AppendLine("• Crescimento populacional");
                res.AppendLine("• Juros compostos contínuos");
                res.AppendLine("• Decaimento radioativo");
                res.AppendLine("• Crescimento bacteriano");
                res.AppendLine("═══════════════════════════════════");

                txtResultadosExp.Text = res.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnDezPotencia_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!double.TryParse(txtExpoente.Text, out double expoente))
                {
                    MessageBox.Show("Expoente inválido!", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                double resultado = Math.Pow(10, expoente);

                StringBuilder res = new StringBuilder();
                res.AppendLine("═══════════════════════════════════");
                res.AppendLine("   🔟 POTÊNCIA DE 10");
                res.AppendLine("═══════════════════════════════════\n");
                res.AppendLine($"10^{expoente}\n");
                res.AppendLine("───────────────────────────────────");
                res.AppendLine("RESULTADO:");
                res.AppendLine("───────────────────────────────────");
                res.AppendLine($"10^{expoente} = {resultado:F2}\n");
                res.AppendLine("───────────────────────────────────");
                res.AppendLine("NOTAÇÃO CIENTÍFICA:");
                res.AppendLine("───────────────────────────────────");
                res.AppendLine($"{resultado:E6}\n");
                res.AppendLine("───────────────────────────────────");
                res.AppendLine("LOGARITMO BASE 10:");
                res.AppendLine("───────────────────────────────────");
                res.AppendLine($"log₁₀({resultado:F2}) = {expoente:F6}");
                res.AppendLine("═══════════════════════════════════");

                txtResultadosExp.Text = res.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnExpProduto_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder res = new StringBuilder();
            res.AppendLine("═══════════════════════════════════");
            res.AppendLine("   ✖️ PROPRIEDADE: PRODUTO");
            res.AppendLine("═══════════════════════════════════\n");
            res.AppendLine("FÓRMULA:");
            res.AppendLine("───────────────────────────────────");
            res.AppendLine("a^m · a^n = a^(m+n)\n");
            res.AppendLine("───────────────────────────────────");
            res.AppendLine("EXEMPLO:");
            res.AppendLine("───────────────────────────────────");
            res.AppendLine("2³ · 2⁴ = 2^(3+4)");
            res.AppendLine("8 · 16 = 2⁷");
            res.AppendLine("128    = 128 ✓\n");
            res.AppendLine("───────────────────────────────────");
            res.AppendLine("REGRA:");
            res.AppendLine("───────────────────────────────────");
            res.AppendLine("Ao multiplicar potências de mesma");
            res.AppendLine("base, CONSERVA-SE a base e SOMA-SE");
            res.AppendLine("os expoentes.");
            res.AppendLine("═══════════════════════════════════");

            txtResultadosExp.Text = res.ToString();
        }

        private void BtnExpQuociente_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder res = new StringBuilder();
            res.AppendLine("═══════════════════════════════════");
            res.AppendLine("   ➗ PROPRIEDADE: QUOCIENTE");
            res.AppendLine("═══════════════════════════════════\n");
            res.AppendLine("FÓRMULA:");
            res.AppendLine("───────────────────────────────────");
            res.AppendLine("a^m / a^n = a^(m-n)\n");
            res.AppendLine("───────────────────────────────────");
            res.AppendLine("EXEMPLO:");
            res.AppendLine("───────────────────────────────────");
            res.AppendLine("2⁵ / 2² = 2^(5-2)");
            res.AppendLine("32 / 4  = 2³");
            res.AppendLine("8       = 8 ✓\n");
            res.AppendLine("───────────────────────────────────");
            res.AppendLine("REGRA:");
            res.AppendLine("───────────────────────────────────");
            res.AppendLine("Ao dividir potências de mesma base,");
            res.AppendLine("CONSERVA-SE a base e SUBTRAI-SE");
            res.AppendLine("os expoentes.");
            res.AppendLine("═══════════════════════════════════");

            txtResultadosExp.Text = res.ToString();
        }

        private void BtnExpPotencia_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder res = new StringBuilder();
            res.AppendLine("═══════════════════════════════════");
            res.AppendLine("   📊 PROPRIEDADE: POTÊNCIA");
            res.AppendLine("═══════════════════════════════════\n");
            res.AppendLine("FÓRMULA:");
            res.AppendLine("───────────────────────────────────");
            res.AppendLine("(a^m)^n = a^(m·n)\n");
            res.AppendLine("───────────────────────────────────");
            res.AppendLine("EXEMPLO:");
            res.AppendLine("───────────────────────────────────");
            res.AppendLine("(2³)² = 2^(3·2)");
            res.AppendLine("8²    = 2⁶");
            res.AppendLine("64    = 64 ✓\n");
            res.AppendLine("───────────────────────────────────");
            res.AppendLine("REGRA:");
            res.AppendLine("───────────────────────────────────");
            res.AppendLine("Potência de potência: CONSERVA-SE");
            res.AppendLine("a base e MULTIPLICA-SE os expoentes.");
            res.AppendLine("═══════════════════════════════════");

            txtResultadosExp.Text = res.ToString();
        }

        private void BtnExpRaiz_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder res = new StringBuilder();
            res.AppendLine("═══════════════════════════════════");
            res.AppendLine("   🔄 PROPRIEDADE: RAIZ");
            res.AppendLine("═══════════════════════════════════\n");
            res.AppendLine("FÓRMULA:");
            res.AppendLine("───────────────────────────────────");
            res.AppendLine("ⁿ√(a^m) = a^(m/n)\n");
            res.AppendLine("───────────────────────────────────");
            res.AppendLine("EXEMPLOS:");
            res.AppendLine("───────────────────────────────────");
            res.AppendLine("√(2⁴) = 2^(4/2) = 2² = 4");
            res.AppendLine("³√(8²) = 8^(2/3) = 2^(6/3) = 2² = 4");
            res.AppendLine("√a = a^(1/2)");
            res.AppendLine("³√a = a^(1/3)\n");
            res.AppendLine("───────────────────────────────────");
            res.AppendLine("CASOS ESPECIAIS:");
            res.AppendLine("───────────────────────────────────");
            res.AppendLine("• a^(1/2) = √a");
            res.AppendLine("• a^(1/3) = ³√a");
            res.AppendLine("• a^(1/n) = ⁿ√a");
            res.AppendLine("• a^(-1/n) = 1/ⁿ√a");
            res.AppendLine("═══════════════════════════════════");

            txtResultadosExp.Text = res.ToString();
        }

        private void BtnTodasPropExp_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder res = new StringBuilder();
            res.AppendLine("═══════════════════════════════════");
            res.AppendLine("   📋 TODAS AS PROPRIEDADES");
            res.AppendLine("═══════════════════════════════════\n");
            res.AppendLine("PRODUTO DE POTÊNCIAS:");
            res.AppendLine("───────────────────────────────────");
            res.AppendLine("• a^m · a^n = a^(m+n)");
            res.AppendLine("• a^m · b^m = (a·b)^m\n");
            res.AppendLine("───────────────────────────────────");
            res.AppendLine("QUOCIENTE DE POTÊNCIAS:");
            res.AppendLine("───────────────────────────────────");
            res.AppendLine("• a^m / a^n = a^(m-n)");
            res.AppendLine("• a^m / b^m = (a/b)^m\n");
            res.AppendLine("───────────────────────────────────");
            res.AppendLine("POTÊNCIA DE POTÊNCIA:");
            res.AppendLine("───────────────────────────────────");
            res.AppendLine("• (a^m)^n = a^(m·n)\n");
            res.AppendLine("───────────────────────────────────");
            res.AppendLine("EXPOENTES ESPECIAIS:");
            res.AppendLine("───────────────────────────────────");
            res.AppendLine("• a^0 = 1  (a ≠ 0)");
            res.AppendLine("• a^1 = a");
            res.AppendLine("• a^(-n) = 1/a^n");
            res.AppendLine("• a^(1/n) = ⁿ√a");
            res.AppendLine("• a^(m/n) = ⁿ√(a^m)\n");
            res.AppendLine("───────────────────────────────────");
            res.AppendLine("RAÍZES:");
            res.AppendLine("───────────────────────────────────");
            res.AppendLine("• ⁿ√(a^m) = a^(m/n)");
            res.AppendLine("• ⁿ√a · ⁿ√b = ⁿ√(a·b)");
            res.AppendLine("• ⁿ√a / ⁿ√b = ⁿ√(a/b)");
            res.AppendLine("═══════════════════════════════════");

            txtResultadosExp.Text = res.ToString();
        }

        private void BtnResolverEqExp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!double.TryParse(txtBaseEqExp.Text, out double baseExp) ||
                    !double.TryParse(txtResultadoEqExp.Text, out double resultado))
                {
                    MessageBox.Show("Valores inválidos!", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (baseExp <= 0)
                {
                    MessageBox.Show("Base deve ser > 0!", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                double x = Math.Log(resultado, baseExp);

                StringBuilder res = new StringBuilder();
                res.AppendLine("═══════════════════════════════════");
                res.AppendLine("   ⚡ EQUAÇÃO EXPONENCIAL");
                res.AppendLine("═══════════════════════════════════\n");
                res.AppendLine($"{baseExp}^x = {resultado}\n");
                res.AppendLine("───────────────────────────────────");
                res.AppendLine("MÉTODO:");
                res.AppendLine("───────────────────────────────────");
                res.AppendLine("Se a^x = b, então:");
                res.AppendLine("x = log_a(b)\n");
                res.AppendLine("───────────────────────────────────");
                res.AppendLine("RESOLUÇÃO:");
                res.AppendLine("───────────────────────────────────");
                res.AppendLine($"x = log_{baseExp}({resultado})");
                res.AppendLine($"x = {x:F6}\n");
                res.AppendLine("───────────────────────────────────");
                res.AppendLine("VERIFICAÇÃO:");
                res.AppendLine("───────────────────────────────────");
                double verificacao = Math.Pow(baseExp, x);
                res.AppendLine($"{baseExp}^{x:F6} = {verificacao:F6}");
                res.AppendLine($"✓ Correto: {resultado:F6} ≈ {verificacao:F6}");
                res.AppendLine("═══════════════════════════════════");

                txtResultadosExp.Text = res.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCrescimento_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!double.TryParse(txtP0.Text, out double P0) ||
                    !double.TryParse(txtK.Text, out double k) ||
                    !double.TryParse(txtT.Text, out double t))
                {
                    MessageBox.Show("Valores inválidos!", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                double Pt = P0 * Math.Exp(k * t);

                StringBuilder res = new StringBuilder();
                res.AppendLine("═══════════════════════════════════");
                res.AppendLine("   📉 CRESCIMENTO EXPONENCIAL");
                res.AppendLine("═══════════════════════════════════\n");
                res.AppendLine("FÓRMULA:");
                res.AppendLine("───────────────────────────────────");
                res.AppendLine("P(t) = P₀ · e^(k·t)\n");
                res.AppendLine("───────────────────────────────────");
                res.AppendLine("DADOS:");
                res.AppendLine("───────────────────────────────────");
                res.AppendLine($"P₀ = {P0:F2}  (valor inicial)");
                res.AppendLine($"k  = {k:F4}  (taxa de crescimento)");
                res.AppendLine($"t  = {t:F2}  (tempo)\n");
                res.AppendLine("───────────────────────────────────");
                res.AppendLine("CÁLCULO:");
                res.AppendLine("───────────────────────────────────");
                res.AppendLine($"P({t}) = {P0} · e^({k}·{t})");
                res.AppendLine($"P({t}) = {P0} · e^{k * t:F4}");
                res.AppendLine($"P({t}) = {P0} · {Math.Exp(k * t):F6}");
                res.AppendLine($"P({t}) = {Pt:F2}\n");
                res.AppendLine("───────────────────────────────────");
                res.AppendLine("INTERPRETAÇÃO:");
                res.AppendLine("───────────────────────────────────");
                if (k > 0)
                {
                    res.AppendLine("✓ CRESCIMENTO (k > 0)");
                    res.AppendLine($"  Aumento de {((Pt - P0) / P0 * 100):F2}%");
                    double tempoDobragem = Math.Log(2) / k;
                    res.AppendLine($"  Tempo de dobragem: {tempoDobragem:F2}");
                }
                else if (k < 0)
                {
                    res.AppendLine("✓ DECAIMENTO (k < 0)");
                    res.AppendLine($"  Redução de {((P0 - Pt) / P0 * 100):F2}%");
                    double meiavida = Math.Log(2) / Math.Abs(k);
                    res.AppendLine($"  Meia-vida: {meiavida:F2}");
                }
                else
                {
                    res.AppendLine("✓ CONSTANTE (k = 0)");
                }
                res.AppendLine("═══════════════════════════════════");

                txtResultadosExp.Text = res.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region CONVERSOR

        private void BtnLogParaExp_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder res = new StringBuilder();
            res.AppendLine("═══════════════════════════════════");
            res.AppendLine("   🔄 CONVERSÃO: LOG → EXP");
            res.AppendLine("═══════════════════════════════════\n");
            res.AppendLine("REGRA:");
            res.AppendLine("───────────────────────────────────");
            res.AppendLine("log_b(x) = y  ⟺  b^y = x\n");
            res.AppendLine("───────────────────────────────────");
            res.AppendLine("EXEMPLOS:");
            res.AppendLine("───────────────────────────────────");
            res.AppendLine("log₂(8) = 3  →  2³ = 8");
            res.AppendLine("log₁₀(100) = 2  →  10² = 100");
            res.AppendLine("ln(e) = 1  →  e¹ = e");
            res.AppendLine("log₅(25) = 2  →  5² = 25\n");
            res.AppendLine("───────────────────────────────────");
            res.AppendLine("PASSO A PASSO:");
            res.AppendLine("───────────────────────────────────");
            res.AppendLine("1. Identifique: log_b(x) = y");
            res.AppendLine("2. A base 'b' permanece base");
            res.AppendLine("3. O resultado 'y' vira expoente");
            res.AppendLine("4. O logaritmando 'x' vira resultado");
            res.AppendLine("═══════════════════════════════════");

            txtResultadosConversor.Text = res.ToString();
        }

        private void BtnExpParaLog_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder res = new StringBuilder();
            res.AppendLine("═══════════════════════════════════");
            res.AppendLine("   🔄 CONVERSÃO: EXP → LOG");
            res.AppendLine("═══════════════════════════════════\n");
            res.AppendLine("REGRA:");
            res.AppendLine("───────────────────────────────────");
            res.AppendLine("b^y = x  ⟺  log_b(x) = y\n");
            res.AppendLine("───────────────────────────────────");
            res.AppendLine("EXEMPLOS:");
            res.AppendLine("───────────────────────────────────");
            res.AppendLine("2³ = 8  →  log₂(8) = 3");
            res.AppendLine("10² = 100  →  log₁₀(100) = 2");
            res.AppendLine("e¹ = e  →  ln(e) = 1");
            res.AppendLine("5² = 25  →  log₅(25) = 2\n");
            res.AppendLine("───────────────────────────────────");
            res.AppendLine("PASSO A PASSO:");
            res.AppendLine("───────────────────────────────────");
            res.AppendLine("1. Identifique: b^y = x");
            res.AppendLine("2. A base 'b' permanece base");
            res.AppendLine("3. O resultado 'x' vira logaritmando");
            res.AppendLine("4. O expoente 'y' vira resultado");
            res.AppendLine("═══════════════════════════════════");

            txtResultadosConversor.Text = res.ToString();
        }

        private void BtnTabelaPot2_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder res = new StringBuilder();
            res.AppendLine("═══════════════════════════════════");
            res.AppendLine("   📊 TABELA DE POTÊNCIAS DE 2");
            res.AppendLine("═══════════════════════════════════\n");
            res.AppendLine("  n  │    2^n    │  log₂(2^n)");
            res.AppendLine("─────┼───────────┼────────────");
            for (int n = 0; n <= 20; n++)
            {
                double valor = Math.Pow(2, n);
                res.AppendLine($" {n,2}  │ {valor,9:F0} │     {n,2}");
            }
            res.AppendLine("═══════════════════════════════════");

            txtResultadosConversor.Text = res.ToString();
        }

        private void BtnTabelaPot10_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder res = new StringBuilder();
            res.AppendLine("═══════════════════════════════════");
            res.AppendLine("   🔟 TABELA DE POTÊNCIAS DE 10");
            res.AppendLine("═══════════════════════════════════\n");
            res.AppendLine("  n  │      10^n       │ log₁₀(10^n)");
            res.AppendLine("─────┼─────────────────┼─────────────");
            for (int n = 0; n <= 15; n++)
            {
                double valor = Math.Pow(10, n);
                res.AppendLine($" {n,2}  │ {valor,15:E2} │      {n,2}");
            }
            res.AppendLine("═══════════════════════════════════");

            txtResultadosConversor.Text = res.ToString();
        }

        private void BtnTabelaLn_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder res = new StringBuilder();
            res.AppendLine("═══════════════════════════════════");
            res.AppendLine("   📐 TABELA DE LOGARITMOS NATURAIS");
            res.AppendLine("═══════════════════════════════════\n");
            res.AppendLine("  x   │   ln(x)   │    e^(ln x)");
            res.AppendLine("──────┼───────────┼────────────");
            double[] valores = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 20, 50, 100, 500, 1000 };
            foreach (double x in valores)
            {
                double lnx = Math.Log(x);
                res.AppendLine($" {x,4} │ {lnx,9:F6} │ {x,10:F2}");
            }
            res.AppendLine("═══════════════════════════════════");

            txtResultadosConversor.Text = res.ToString();
        }

        private void BtnNumeroE_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder res = new StringBuilder();
            res.AppendLine("═══════════════════════════════════");
            res.AppendLine("   📈 NÚMERO DE EULER (e)");
            res.AppendLine("═══════════════════════════════════\n");
            res.AppendLine("VALOR:");
            res.AppendLine("───────────────────────────────────");
            res.AppendLine($"e ≈ {Math.E:F15}\n");
            res.AppendLine("───────────────────────────────────");
            res.AppendLine("DEFINIÇÃO:");
            res.AppendLine("───────────────────────────────────");
            res.AppendLine("       ⎛     1 ⎞ⁿ");
            res.AppendLine("e = lim⎜1 + ───⎟");
            res.AppendLine("   n→∞ ⎝     n ⎠\n");
            res.AppendLine("───────────────────────────────────");
            res.AppendLine("PROPRIEDADES:");
            res.AppendLine("───────────────────────────────────");
            res.AppendLine("• ln(e) = 1");
            res.AppendLine("• e^1 = e");
            res.AppendLine("• e^0 = 1");
            res.AppendLine("• e^(ln x) = x");
            res.AppendLine("• ln(e^x) = x\n");
            res.AppendLine("───────────────────────────────────");
            res.AppendLine("APLICAÇÕES:");
            res.AppendLine("───────────────────────────────────");
            res.AppendLine("• Crescimento contínuo");
            res.AppendLine("• Juros compostos contínuos");
            res.AppendLine("• Probabilidade e estatística");
            res.AppendLine("• Física (decaimento radioativo)");
            res.AppendLine("• Biologia (crescimento populacional)");
            res.AppendLine("\n───────────────────────────────────");
            res.AppendLine("CURIOSIDADE:");
            res.AppendLine("───────────────────────────────────");
            res.AppendLine("e é um número irracional e");
            res.AppendLine("transcendente, descoberto por");
            res.AppendLine("Leonhard Euler (1707-1783).");
            res.AppendLine("═══════════════════════════════════");

            txtResultadosConversor.Text = res.ToString();
        }

        #endregion
    }
}