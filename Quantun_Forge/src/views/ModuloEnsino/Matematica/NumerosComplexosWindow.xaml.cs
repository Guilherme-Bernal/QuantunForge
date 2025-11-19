using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using ComplexNumber = System.Numerics.Complex; // Alias para evitar conflito

namespace Quantun_Forge.src.views.ModuloEnsino.Matematica
{
    public partial class NumerosComplexosWindow : Window
    {
        private Dictionary<string, ComplexNumber> variables = new Dictionary<string, ComplexNumber>();
        private StringBuilder consoleBuffer = new StringBuilder();

        public NumerosComplexosWindow()
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
                variables.Clear();

                string code = CodeEditor.Text.Trim();

                if (string.IsNullOrWhiteSpace(code))
                {
                    WriteError("Código vazio!");
                    UpdateConsole();
                    return;
                }

                WriteLine("═══════════════════════════════════════");
                WriteLine("▶ EXECUTANDO CÓDIGO...");
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
                WriteLine("✓ EXECUÇÃO CONCLUÍDA!");
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

                if (line.StartsWith("escreva", StringComparison.OrdinalIgnoreCase))
                {
                    ExecuteEscreva(line);
                }
                else if (line.Contains("=") && !line.Contains("=="))
                {
                    ExecuteAtribuicao(line);
                }
                else if (line.Contains("complexo", StringComparison.OrdinalIgnoreCase))
                {
                    ExecuteComplexo(line);
                }
                else if (line.Contains("modulo", StringComparison.OrdinalIgnoreCase))
                {
                    ExecuteModulo(line);
                }
                else if (line.Contains("argumento", StringComparison.OrdinalIgnoreCase))
                {
                    ExecuteArgumento(line);
                }
                else if (line.Contains("conjugado", StringComparison.OrdinalIgnoreCase))
                {
                    ExecuteConjugado(line);
                }
                else if (line.Contains("somar", StringComparison.OrdinalIgnoreCase))
                {
                    ExecuteSomar(line);
                }
                else if (line.Contains("subtrair", StringComparison.OrdinalIgnoreCase))
                {
                    ExecuteSubtrair(line);
                }
                else if (line.Contains("multiplicar", StringComparison.OrdinalIgnoreCase))
                {
                    ExecuteMultiplicar(line);
                }
                else if (line.Contains("dividir", StringComparison.OrdinalIgnoreCase))
                {
                    ExecuteDividir(line);
                }
                else if (line.Contains("potencia", StringComparison.OrdinalIgnoreCase))
                {
                    ExecutePotencia(line);
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

        #region Comandos do Interpretador

        private void ExecuteEscreva(string line)
        {
            Match match = Regex.Match(line, @"escreva\s*\((.*)\)", RegexOptions.IgnoreCase);
            if (!match.Success)
            {
                WriteError("Sintaxe: escreva(\"texto\") ou escreva(variavel)");
                return;
            }

            string content = match.Groups[1].Value.Trim();

            if (content.StartsWith("\"") && content.EndsWith("\""))
            {
                string text = content.Substring(1, content.Length - 2);
                WriteLine(text);
            }
            else if (variables.ContainsKey(content))
            {
                WriteLine(FormatComplex(variables[content]));
            }
            else
            {
                WriteError($"Variável '{content}' não encontrada");
            }
        }

        private void ExecuteAtribuicao(string line)
        {
            line = Regex.Replace(line, @"^\s*var\s+", "", RegexOptions.IgnoreCase);

            string[] parts = line.Split('=');
            if (parts.Length != 2)
            {
                WriteError("Sintaxe: var nome = valor");
                return;
            }

            string varName = parts[0].Trim();
            string valueStr = parts[1].Trim();

            if (!Regex.IsMatch(varName, @"^[a-zA-Z_][a-zA-Z0-9_]*$"))
            {
                WriteError($"Nome de variável inválido: {varName}");
                return;
            }

            if (valueStr.Contains("complexo("))
            {
                ExecuteComplexo(valueStr, varName);
            }
            else
            {
                WriteError("Tipo de atribuição não suportado");
            }
        }

        private void ExecuteComplexo(string line, string? varName = null)
        {
            Match match = Regex.Match(line, @"complexo\s*\(\s*(-?\d+\.?\d*)\s*,\s*(-?\d+\.?\d*)\s*\)", RegexOptions.IgnoreCase);

            if (!match.Success)
            {
                WriteError("Sintaxe: complexo(parte_real, parte_imaginaria)");
                return;
            }

            try
            {
                double real = double.Parse(match.Groups[1].Value);
                double imag = double.Parse(match.Groups[2].Value);
                ComplexNumber z = new ComplexNumber(real, imag);

                if (varName != null)
                {
                    variables[varName] = z;
                    WriteSuccess($"{varName} = {FormatComplex(z)}");
                }
                else
                {
                    WriteLine(FormatComplex(z));
                }
            }
            catch (Exception ex)
            {
                WriteError($"Erro ao criar número complexo: {ex.Message}");
            }
        }

        private void ExecuteModulo(string line)
        {
            Match match = Regex.Match(line, @"modulo\s*\(([^)]+)\)", RegexOptions.IgnoreCase);

            if (!match.Success)
            {
                WriteError("Sintaxe: modulo(z)");
                return;
            }

            string varName = match.Groups[1].Value.Trim();

            if (!variables.ContainsKey(varName))
            {
                WriteError($"Variável '{varName}' não encontrada");
                return;
            }

            double modulo = variables[varName].Magnitude;
            WriteLine($"|{varName}| = {modulo:F4}");
        }

        private void ExecuteArgumento(string line)
        {
            Match match = Regex.Match(line, @"argumento\s*\(([^)]+)\)", RegexOptions.IgnoreCase);

            if (!match.Success)
            {
                WriteError("Sintaxe: argumento(z)");
                return;
            }

            string varName = match.Groups[1].Value.Trim();

            if (!variables.ContainsKey(varName))
            {
                WriteError($"Variável '{varName}' não encontrada");
                return;
            }

            double argRadianos = variables[varName].Phase;
            double argGraus = argRadianos * (180.0 / Math.PI);

            WriteLine($"arg({varName}) = {argRadianos:F4} rad = {argGraus:F2}°");
        }

        private void ExecuteConjugado(string line)
        {
            Match match = Regex.Match(line, @"conjugado\s*\(([^)]+)\)", RegexOptions.IgnoreCase);

            if (!match.Success)
            {
                WriteError("Sintaxe: conjugado(z)");
                return;
            }

            string varName = match.Groups[1].Value.Trim();

            if (!variables.ContainsKey(varName))
            {
                WriteError($"Variável '{varName}' não encontrada");
                return;
            }

            ComplexNumber conjugado = ComplexNumber.Conjugate(variables[varName]);
            WriteLine($"conjugado({varName}) = {FormatComplex(conjugado)}");
        }

        private void ExecuteSomar(string line)
        {
            Match match = Regex.Match(line, @"somar\s*\(([^,]+),([^)]+)\)", RegexOptions.IgnoreCase);

            if (!match.Success)
            {
                WriteError("Sintaxe: somar(z1, z2)");
                return;
            }

            string var1 = match.Groups[1].Value.Trim();
            string var2 = match.Groups[2].Value.Trim();

            if (!variables.ContainsKey(var1) || !variables.ContainsKey(var2))
            {
                WriteError("Uma das variáveis não foi encontrada");
                return;
            }

            ComplexNumber resultado = variables[var1] + variables[var2];
            WriteLine($"{var1} + {var2} = {FormatComplex(resultado)}");
        }

        private void ExecuteSubtrair(string line)
        {
            Match match = Regex.Match(line, @"subtrair\s*\(([^,]+),([^)]+)\)", RegexOptions.IgnoreCase);

            if (!match.Success)
            {
                WriteError("Sintaxe: subtrair(z1, z2)");
                return;
            }

            string var1 = match.Groups[1].Value.Trim();
            string var2 = match.Groups[2].Value.Trim();

            if (!variables.ContainsKey(var1) || !variables.ContainsKey(var2))
            {
                WriteError("Uma das variáveis não foi encontrada");
                return;
            }

            ComplexNumber resultado = variables[var1] - variables[var2];
            WriteLine($"{var1} - {var2} = {FormatComplex(resultado)}");
        }

        private void ExecuteMultiplicar(string line)
        {
            Match match = Regex.Match(line, @"multiplicar\s*\(([^,]+),([^)]+)\)", RegexOptions.IgnoreCase);

            if (!match.Success)
            {
                WriteError("Sintaxe: multiplicar(z1, z2)");
                return;
            }

            string var1 = match.Groups[1].Value.Trim();
            string var2 = match.Groups[2].Value.Trim();

            if (!variables.ContainsKey(var1) || !variables.ContainsKey(var2))
            {
                WriteError("Uma das variáveis não foi encontrada");
                return;
            }

            ComplexNumber resultado = variables[var1] * variables[var2];
            WriteLine($"{var1} × {var2} = {FormatComplex(resultado)}");
        }

        private void ExecuteDividir(string line)
        {
            Match match = Regex.Match(line, @"dividir\s*\(([^,]+),([^)]+)\)", RegexOptions.IgnoreCase);

            if (!match.Success)
            {
                WriteError("Sintaxe: dividir(z1, z2)");
                return;
            }

            string var1 = match.Groups[1].Value.Trim();
            string var2 = match.Groups[2].Value.Trim();

            if (!variables.ContainsKey(var1) || !variables.ContainsKey(var2))
            {
                WriteError("Uma das variáveis não foi encontrada");
                return;
            }

            if (variables[var2].Magnitude == 0)
            {
                WriteError("Divisão por zero!");
                return;
            }

            ComplexNumber resultado = variables[var1] / variables[var2];
            WriteLine($"{var1} ÷ {var2} = {FormatComplex(resultado)}");
        }

        private void ExecutePotencia(string line)
        {
            Match match = Regex.Match(line, @"potencia\s*\(([^,]+),([^)]+)\)", RegexOptions.IgnoreCase);

            if (!match.Success)
            {
                WriteError("Sintaxe: potencia(z, n)");
                return;
            }

            string varName = match.Groups[1].Value.Trim();
            string exponenteStr = match.Groups[2].Value.Trim();

            if (!variables.ContainsKey(varName))
            {
                WriteError($"Variável '{varName}' não encontrada");
                return;
            }

            try
            {
                double expoente = double.Parse(exponenteStr);
                ComplexNumber resultado = ComplexNumber.Pow(variables[varName], expoente);
                WriteLine($"{varName}^{expoente} = {FormatComplex(resultado)}");
            }
            catch
            {
                WriteError("Expoente inválido");
            }
        }

        #endregion

        #region Funções Auxiliares

        private string FormatComplex(ComplexNumber z)
        {
            double real = Math.Round(z.Real, 4);
            double imag = Math.Round(z.Imaginary, 4);

            if (imag == 0)
                return real.ToString("F2");

            if (real == 0)
            {
                if (imag == 1) return "i";
                if (imag == -1) return "-i";
                return $"{imag:F2}i";
            }

            string sign = imag >= 0 ? "+" : "";
            string imagPart = Math.Abs(imag) == 1 ? "i" : $"{Math.Abs(imag):F2}i";

            if (imag < 0)
                return $"{real:F2} - {imagPart}";

            return $"{real:F2} {sign} {imagPart}";
        }

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
            ConsoleOutput.Text = "> Console limpo.\n> Digite seu código e clique em EXECUTAR.";
            ConsoleOutput.Foreground = new SolidColorBrush(Color.FromRgb(0, 255, 65));
        }

        private void btnExerciciosClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "Exercícios de Números Complexos em desenvolvimento!",
                "Em breve",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        #endregion
    }
}