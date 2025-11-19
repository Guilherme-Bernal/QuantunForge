using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Quantun_Forge.src.views.ModuloEnsino.Matematica.Testes;

namespace Quantun_Forge.src.views.ModuloEnsino.Matematica
{
    public partial class TeoriaProbabilidadeWindow : Window
    {
        private StringBuilder consoleBuffer = new StringBuilder();

        public TeoriaProbabilidadeWindow()
        {
            InitializeComponent();
        }

        #region Controles da Janela

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void MinimizeWindow_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void MaximizeWindow_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = this.WindowState == WindowState.Maximized
                ? WindowState.Normal
                : WindowState.Maximized;
        }

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

                string code = CodeEditor.Text.Trim();

                if (string.IsNullOrWhiteSpace(code))
                {
                    WriteError("Código vazio!");
                    UpdateConsole();
                    return;
                }

                WriteLine("═══════════════════════════════════════");
                WriteLine("▶ EXECUTANDO CÁLCULOS...");
                WriteLine("═══════════════════════════════════════\n");

                string[] lines = code.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string rawLine in lines)
                {
                    string line = rawLine.Trim();
                    if (string.IsNullOrWhiteSpace(line) || line.StartsWith("//"))
                        continue;

                    ExecuteLine(line);
                }

                WriteLine("\n═══════════════════════════════════════");
                WriteLine("✓ CÁLCULOS CONCLUÍDOS!");
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
                line = line.TrimEnd(';');

                if (line.Contains("probabilidade", StringComparison.OrdinalIgnoreCase))
                {
                    ExecuteProbabilidade(line);
                }
                else if (line.Contains("combinacao", StringComparison.OrdinalIgnoreCase))
                {
                    ExecuteCombinacao(line);
                }
                else if (line.Contains("permutacao", StringComparison.OrdinalIgnoreCase))
                {
                    ExecutePermutacao(line);
                }
                else if (line.Contains("arranjo", StringComparison.OrdinalIgnoreCase))
                {
                    ExecuteArranjo(line);
                }
                else if (line.Contains("fatorial", StringComparison.OrdinalIgnoreCase))
                {
                    ExecuteFatorial(line);
                }
                else if (line.Contains("bayes", StringComparison.OrdinalIgnoreCase))
                {
                    ExecuteBayes(line);
                }
                else
                {
                    WriteError($"Comando não reconhecido: {line}");
                }
            }
            catch (Exception ex)
            {
                WriteError($"Erro: {ex.Message}");
            }
        }

        #endregion

        #region Comandos

        private void ExecuteProbabilidade(string line)
        {
            Match match = Regex.Match(line, @"probabilidade\s*\(\s*(\d+)\s*,\s*(\d+)\s*\)", RegexOptions.IgnoreCase);

            if (!match.Success)
            {
                WriteError("Sintaxe: probabilidade(casos_favoraveis, casos_possiveis)");
                return;
            }

            try
            {
                int favoraveis = int.Parse(match.Groups[1].Value);
                int possiveis = int.Parse(match.Groups[2].Value);

                if (possiveis == 0)
                {
                    WriteError("Casos possíveis não pode ser zero!");
                    return;
                }

                if (favoraveis > possiveis)
                {
                    WriteError("Casos favoráveis não pode ser maior que casos possíveis!");
                    return;
                }

                double probabilidade = (double)favoraveis / possiveis;
                double percentual = probabilidade * 100;

                WriteLine($"P(E) = {favoraveis}/{possiveis} = {probabilidade:F4}");
                WriteLine($"Percentual: {percentual:F2}%");
            }
            catch (Exception ex)
            {
                WriteError($"Erro ao calcular probabilidade: {ex.Message}");
            }
        }

        private void ExecuteCombinacao(string line)
        {
            Match match = Regex.Match(line, @"combinacao\s*\(\s*(\d+)\s*,\s*(\d+)\s*\)", RegexOptions.IgnoreCase);

            if (!match.Success)
            {
                WriteError("Sintaxe: combinacao(n, k)");
                return;
            }

            try
            {
                int n = int.Parse(match.Groups[1].Value);
                int k = int.Parse(match.Groups[2].Value);

                if (k > n)
                {
                    WriteError("k não pode ser maior que n!");
                    return;
                }

                if (n < 0 || k < 0)
                {
                    WriteError("n e k devem ser não-negativos!");
                    return;
                }

                long resultado = Combinacao(n, k);
                WriteLine($"C({n},{k}) = {n}! / ({k}! × {n - k}!) = {resultado}");
            }
            catch (OverflowException)
            {
                WriteError("Resultado muito grande para ser calculado!");
            }
            catch (Exception ex)
            {
                WriteError($"Erro: {ex.Message}");
            }
        }

        private void ExecutePermutacao(string line)
        {
            Match match = Regex.Match(line, @"permutacao\s*\(\s*(\d+)\s*\)", RegexOptions.IgnoreCase);

            if (!match.Success)
            {
                WriteError("Sintaxe: permutacao(n)");
                return;
            }

            try
            {
                int n = int.Parse(match.Groups[1].Value);

                if (n < 0)
                {
                    WriteError("n deve ser não-negativo!");
                    return;
                }

                if (n > 20)
                {
                    WriteError("n muito grande! Use n ≤ 20");
                    return;
                }

                long resultado = Fatorial(n);
                WriteLine($"P({n}) = {n}! = {resultado}");
            }
            catch (Exception ex)
            {
                WriteError($"Erro: {ex.Message}");
            }
        }

        private void ExecuteArranjo(string line)
        {
            Match match = Regex.Match(line, @"arranjo\s*\(\s*(\d+)\s*,\s*(\d+)\s*\)", RegexOptions.IgnoreCase);

            if (!match.Success)
            {
                WriteError("Sintaxe: arranjo(n, k)");
                return;
            }

            try
            {
                int n = int.Parse(match.Groups[1].Value);
                int k = int.Parse(match.Groups[2].Value);

                if (k > n)
                {
                    WriteError("k não pode ser maior que n!");
                    return;
                }

                if (n < 0 || k < 0)
                {
                    WriteError("n e k devem ser não-negativos!");
                    return;
                }

                long resultado = Arranjo(n, k);
                WriteLine($"A({n},{k}) = {n}! / ({n - k})! = {resultado}");
            }
            catch (OverflowException)
            {
                WriteError("Resultado muito grande para ser calculado!");
            }
            catch (Exception ex)
            {
                WriteError($"Erro: {ex.Message}");
            }
        }

        private void ExecuteFatorial(string line)
        {
            Match match = Regex.Match(line, @"fatorial\s*\(\s*(\d+)\s*\)", RegexOptions.IgnoreCase);

            if (!match.Success)
            {
                WriteError("Sintaxe: fatorial(n)");
                return;
            }

            try
            {
                int n = int.Parse(match.Groups[1].Value);

                if (n < 0)
                {
                    WriteError("n deve ser não-negativo!");
                    return;
                }

                if (n > 20)
                {
                    WriteError("n muito grande! Use n ≤ 20");
                    return;
                }

                long resultado = Fatorial(n);
                WriteLine($"{n}! = {resultado}");
            }
            catch (Exception ex)
            {
                WriteError($"Erro: {ex.Message}");
            }
        }

        private void ExecuteBayes(string line)
        {
            Match match = Regex.Match(line,
                @"bayes\s*\(\s*(\d+\.?\d*)\s*,\s*(\d+\.?\d*)\s*,\s*(\d+\.?\d*)\s*\)",
                RegexOptions.IgnoreCase);

            if (!match.Success)
            {
                WriteError("Sintaxe: bayes(P_B_dado_A, P_A, P_B)");
                return;
            }

            try
            {
                double pBdadoA = double.Parse(match.Groups[1].Value);
                double pA = double.Parse(match.Groups[2].Value);
                double pB = double.Parse(match.Groups[3].Value);

                if (pB == 0)
                {
                    WriteError("P(B) não pode ser zero!");
                    return;
                }

                if (pBdadoA < 0 || pBdadoA > 1 || pA < 0 || pA > 1 || pB < 0 || pB > 1)
                {
                    WriteError("Probabilidades devem estar entre 0 e 1!");
                    return;
                }

                double pAdadoB = (pBdadoA * pA) / pB;

                WriteLine($"P(A|B) = P(B|A) × P(A) / P(B)");
                WriteLine($"P(A|B) = {pBdadoA:F4} × {pA:F4} / {pB:F4}");
                WriteLine($"P(A|B) = {pAdadoB:F4} ({pAdadoB * 100:F2}%)");
            }
            catch (Exception ex)
            {
                WriteError($"Erro: {ex.Message}");
            }
        }

        #endregion

        #region Funções Matemáticas

        private long Fatorial(int n)
        {
            if (n == 0 || n == 1) return 1;

            long resultado = 1;
            for (int i = 2; i <= n; i++)
            {
                resultado *= i;
            }
            return resultado;
        }

        private long Combinacao(int n, int k)
        {
            if (k == 0 || k == n) return 1;
            if (k > n - k) k = n - k; // Otimização

            long resultado = 1;
            for (int i = 0; i < k; i++)
            {
                resultado *= (n - i);
                resultado /= (i + 1);
            }
            return resultado;
        }

        private long Arranjo(int n, int k)
        {
            long resultado = 1;
            for (int i = 0; i < k; i++)
            {
                resultado *= (n - i);
            }
            return resultado;
        }

        #endregion

        #region Helpers

        private void WriteLine(string text)
        {
            consoleBuffer.AppendLine("> " + text);
        }

        private void WriteError(string text)
        {
            consoleBuffer.AppendLine($"> ✗ ERRO: {text}");
        }

        private void UpdateConsole()
        {
            ConsoleOutput.Text = consoleBuffer.ToString();

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

        #region Botões

        private void ClearCode_Click(object sender, RoutedEventArgs e)
        {
            CodeEditor.Text = "";
            ConsoleOutput.Text = "> Console limpo.\n> Digite seus cálculos e clique em EXECUTAR.";
            ConsoleOutput.Foreground = new SolidColorBrush(Color.FromRgb(0, 255, 65));
        }

        private void btnExerciciosClick(object sender, RoutedEventArgs e)
        {
            var win = new TeoriaProbabilidadeTesteWindow();

            win.Show();      
        }

        #endregion
    }
}