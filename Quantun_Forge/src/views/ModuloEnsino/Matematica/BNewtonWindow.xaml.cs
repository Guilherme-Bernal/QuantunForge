using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Quantun_Forge.src.views.ModuloEnsino.Matematica.Testes;

namespace Quantun_Forge.src.views.ModuloEnsino.Matematica
{
    public partial class BNewtonWindow : Window
    {
        private Dictionary<string, object> variables = new Dictionary<string, object>();
        private StringBuilder consoleBuffer = new StringBuilder();

        public BNewtonWindow()
        {
            InitializeComponent();
            ConsoleOutput.Text = "> Interpretador Portugol iniciado.\n> Digite seu código e clique em EXECUTAR.\n";
        }

        #region Controles da Janela

        // Permite arrastar a janela
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        // Minimizar janela
        private void MinimizeWindow_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        // Maximizar/Restaurar janela
        private void MaximizeWindow_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = this.WindowState == WindowState.Maximized
                ? WindowState.Normal
                : WindowState.Maximized;
        }

        // Fechar janela
        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        #endregion

        #region Execução do Código

        private void RunCode_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                consoleBuffer.Clear();
                variables.Clear();

                string code = CodeEditor.Text.Trim();

                if (string.IsNullOrWhiteSpace(code))
                {
                    WriteError("Código vazio! Digite algo para executar.");
                    UpdateConsole();
                    return;
                }

                WriteLine("═══════════════════════════════════════");
                WriteLine("▶ EXECUTANDO CÓDIGO...");
                WriteLine("═══════════════════════════════════════\n");

                // Divide o código em linhas e executa cada uma
                string[] lines = code.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string rawLine in lines)
                {
                    string line = rawLine.Trim();

                    // Ignora linhas vazias e comentários
                    if (string.IsNullOrWhiteSpace(line) || line.StartsWith("//"))
                        continue;

                    ExecuteLine(line);
                }

                WriteLine("\n═══════════════════════════════════════");
                WriteLine("✓ EXECUÇÃO CONCLUÍDA COM SUCESSO!");
                WriteLine("═══════════════════════════════════════");

                UpdateConsole();
            }
            catch (Exception ex)
            {
                WriteError($"Erro inesperado: {ex.Message}");
                UpdateConsole();
            }
        }

        private void ExecuteLine(string line)
        {
            try
            {
                // Remove ponto e vírgula final se existir
                line = line.TrimEnd(';');

                // COMANDO: escreva("texto") ou escreva(variavel) ou escreva(expressão)
                if (line.StartsWith("escreva", StringComparison.OrdinalIgnoreCase))
                {
                    ExecuteEscreva(line);
                }
                // COMANDO: var nome = valor
                else if (line.Contains("=") && !line.Contains("=="))
                {
                    ExecuteAtribuicao(line);
                }
                // COMANDO: fatorial(n)
                else if (line.Contains("fatorial", StringComparison.OrdinalIgnoreCase))
                {
                    ExecuteFatorial(line);
                }
                // COMANDO: combinacao(n, k) ou C(n, k)
                else if (line.Contains("combinacao", StringComparison.OrdinalIgnoreCase) ||
                         Regex.IsMatch(line, @"C\s*\(\s*\d+\s*,\s*\d+\s*\)"))
                {
                    ExecuteCombinacao(line);
                }
                // COMANDO: binomio(x, a, n)
                else if (line.Contains("binomio", StringComparison.OrdinalIgnoreCase))
                {
                    ExecuteBinomio(line);
                }
                // COMANDO: pascal(n) - Exibe linha n do triângulo de Pascal
                else if (line.Contains("pascal", StringComparison.OrdinalIgnoreCase))
                {
                    ExecutePascal(line);
                }
                else
                {
                    WriteError($"Comando não reconhecido: {line}");
                }
            }
            catch (Exception ex)
            {
                WriteError($"Erro na linha '{line}': {ex.Message}");
            }
        }

        #endregion

        #region Comandos do Interpretador

        private void ExecuteEscreva(string line)
        {
            // Regex para capturar o conteúdo dentro de escreva(...)
            Match match = Regex.Match(line, @"escreva\s*\((.*)\)", RegexOptions.IgnoreCase);

            if (!match.Success)
            {
                WriteError("Sintaxe incorreta em escreva(). Use: escreva(\"texto\") ou escreva(variavel)");
                return;
            }

            string content = match.Groups[1].Value.Trim();

            // Verifica se é uma string literal
            if (content.StartsWith("\"") && content.EndsWith("\""))
            {
                string text = content.Substring(1, content.Length - 2);
                // Processa caracteres especiais
                text = text.Replace("\\n", "\n").Replace("\\t", "\t");
                WriteLine(text);
            }
            // Verifica se é uma variável
            else if (variables.ContainsKey(content))
            {
                WriteLine(variables[content].ToString());
            }
            // Tenta avaliar como expressão
            else
            {
                try
                {
                    double result = EvaluateExpression(content);
                    WriteLine(result.ToString());
                }
                catch
                {
                    WriteError($"Variável ou expressão '{content}' não reconhecida.");
                }
            }
        }

        private void ExecuteAtribuicao(string line)
        {
            // Remove "var" se existir
            line = Regex.Replace(line, @"^\s*var\s+", "", RegexOptions.IgnoreCase);

            string[] parts = line.Split('=');
            if (parts.Length != 2)
            {
                WriteError("Sintaxe incorreta em atribuição. Use: var nome = valor");
                return;
            }

            string varName = parts[0].Trim();
            string valueStr = parts[1].Trim();

            // Valida nome da variável
            if (!Regex.IsMatch(varName, @"^[a-zA-Z_][a-zA-Z0-9_]*$"))
            {
                WriteError($"Nome de variável inválido: {varName}");
                return;
            }

            // Avalia o valor
            try
            {
                double value = EvaluateExpression(valueStr);
                variables[varName] = value;
                WriteSuccess($"{varName} = {value}");
            }
            catch (Exception ex)
            {
                WriteError($"Erro ao avaliar expressão '{valueStr}': {ex.Message}");
            }
        }

        private void ExecuteFatorial(string line)
        {
            Match match = Regex.Match(line, @"fatorial\s*\((\d+)\)", RegexOptions.IgnoreCase);

            if (!match.Success)
            {
                WriteError("Sintaxe incorreta. Use: fatorial(n)");
                return;
            }

            int n = int.Parse(match.Groups[1].Value);

            if (n < 0)
            {
                WriteError("Fatorial não é definido para números negativos.");
                return;
            }

            if (n > 170)
            {
                WriteError("Número muito grande para calcular fatorial (máximo: 170).");
                return;
            }

            double result = Factorial(n);
            WriteLine($"fatorial({n}) = {result:G}");
        }

        private void ExecuteCombinacao(string line)
        {
            // Aceita combinacao(n, k) ou C(n, k)
            Match match = Regex.Match(line, @"(?:combinacao|C)\s*\(\s*(\d+)\s*,\s*(\d+)\s*\)", RegexOptions.IgnoreCase);

            if (!match.Success)
            {
                WriteError("Sintaxe incorreta. Use: combinacao(n, k) ou C(n, k)");
                return;
            }

            int n = int.Parse(match.Groups[1].Value);
            int k = int.Parse(match.Groups[2].Value);

            if (n < 0 || k < 0)
            {
                WriteError("n e k devem ser não-negativos.");
                return;
            }

            if (k > n)
            {
                WriteError("k não pode ser maior que n.");
                return;
            }

            double result = Combination(n, k);
            WriteLine($"C({n}, {k}) = {result:G}");
        }

        private void ExecuteBinomio(string line)
        {
            // binomio(x, a, n) - Expande (x + a)^n
            Match match = Regex.Match(line, @"binomio\s*\(\s*([^,]+)\s*,\s*([^,]+)\s*,\s*(\d+)\s*\)", RegexOptions.IgnoreCase);

            if (!match.Success)
            {
                WriteError("Sintaxe incorreta. Use: binomio(x, a, n)");
                return;
            }

            string xStr = match.Groups[1].Value.Trim();
            string aStr = match.Groups[2].Value.Trim();
            int n = int.Parse(match.Groups[3].Value);

            if (n < 0)
            {
                WriteError("O expoente n deve ser não-negativo.");
                return;
            }

            if (n > 20)
            {
                WriteError("Expoente muito grande (máximo: 20).");
                return;
            }

            // Monta a expansão
            StringBuilder expansion = new StringBuilder();
            expansion.Append($"({xStr} + {aStr})^{n} = ");

            for (int k = 0; k <= n; k++)
            {
                double coef = Combination(n, k);
                int xPower = n - k;
                int aPower = k;

                if (k > 0) expansion.Append(" + ");

                // Monta o termo
                if (coef != 1 || (xPower == 0 && aPower == 0))
                    expansion.Append(coef.ToString("G"));

                if (xPower > 0)
                {
                    expansion.Append(xStr);
                    if (xPower > 1) expansion.Append($"^{xPower}");
                }

                if (aPower > 0)
                {
                    expansion.Append(aStr);
                    if (aPower > 1) expansion.Append($"^{aPower}");
                }
            }

            WriteLine(expansion.ToString());
        }

        private void ExecutePascal(string line)
        {
            Match match = Regex.Match(line, @"pascal\s*\((\d+)\)", RegexOptions.IgnoreCase);

            if (!match.Success)
            {
                WriteError("Sintaxe incorreta. Use: pascal(n)");
                return;
            }

            int n = int.Parse(match.Groups[1].Value);

            if (n < 0)
            {
                WriteError("n deve ser não-negativo.");
                return;
            }

            if (n > 30)
            {
                WriteError("Número muito grande (máximo: 30).");
                return;
            }

            // Gera a linha n do triângulo de Pascal
            StringBuilder line_pascal = new StringBuilder();
            line_pascal.Append($"Pascal(n={n}): ");

            for (int k = 0; k <= n; k++)
            {
                if (k > 0) line_pascal.Append("  ");
                line_pascal.Append(Combination(n, k).ToString("G"));
            }

            WriteLine(line_pascal.ToString());
        }

        #endregion

        #region Funções Matemáticas

        private double Factorial(int n)
        {
            if (n <= 1) return 1;

            double result = 1;
            for (int i = 2; i <= n; i++)
                result *= i;

            return result;
        }

        private double Combination(int n, int k)
        {
            if (k > n) return 0;
            if (k == 0 || k == n) return 1;

            // Otimização: C(n,k) = C(n, n-k)
            if (k > n - k)
                k = n - k;

            double result = 1;
            for (int i = 0; i < k; i++)
            {
                result *= (n - i);
                result /= (i + 1);
            }

            return result;
        }

        private double EvaluateExpression(string expr)
        {
            expr = expr.Trim();

            // Substitui variáveis por seus valores
            foreach (var kvp in variables)
            {
                expr = Regex.Replace(expr, @"\b" + kvp.Key + @"\b", kvp.Value.ToString());
            }

            // Avaliador simples de expressões (suporta +, -, *, /, ^, parênteses)
            return EvaluateSimpleExpression(expr);
        }

        private double EvaluateSimpleExpression(string expr)
        {
            expr = expr.Replace(" ", "");

            // Trata potenciação (^)
            if (expr.Contains("^"))
            {
                Match match = Regex.Match(expr, @"([\d.]+)\^([\d.]+)");
                if (match.Success)
                {
                    double baseNum = double.Parse(match.Groups[1].Value);
                    double exponent = double.Parse(match.Groups[2].Value);
                    double result = Math.Pow(baseNum, exponent);
                    expr = expr.Replace(match.Value, result.ToString());
                    return EvaluateSimpleExpression(expr);
                }
            }

            // Usa DataTable para avaliar expressões simples
            try
            {
                var dataTable = new System.Data.DataTable();
                var result = dataTable.Compute(expr, "");
                return Convert.ToDouble(result);
            }
            catch
            {
                throw new Exception("Expressão matemática inválida");
            }
        }

        #endregion

        #region Funções de Console

        private void WriteLine(string text)
        {
            consoleBuffer.AppendLine("> " + text);
        }

        private void WriteSuccess(string text)
        {
            consoleBuffer.AppendLine($"> ✓ {text}");
        }

        private void WriteError(string text)
        {
            consoleBuffer.AppendLine($"> ✗ ERRO: {text}");
        }

        private void UpdateConsole()
        {
            ConsoleOutput.Text = consoleBuffer.ToString();

            // Destaca erros em vermelho ou sucesso em verde
            if (consoleBuffer.ToString().Contains("✗ ERRO"))
            {
                ConsoleOutput.Foreground = new SolidColorBrush(Color.FromRgb(255, 100, 100));
            }
            else
            {
                ConsoleOutput.Foreground = new SolidColorBrush(Color.FromRgb(0, 255, 65));
            }
        }

        #endregion
        
        private void ClearConsole_Click(object sender, RoutedEventArgs e)
        {
            consoleBuffer.Clear();
            ConsoleOutput.Text = "> Console limpo.\n";
            ConsoleOutput.Foreground = new SolidColorBrush(Colors.White);
        }

        private void btnExepratica_Click(object sender, RoutedEventArgs e)
        {
            var win = new BNewtonTesteWindow();
            win.Show();
        }
    }
}