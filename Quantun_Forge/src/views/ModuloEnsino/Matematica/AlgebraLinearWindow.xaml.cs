using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Quantun_Forge.src.views.ModuloEnsino.Matematica
{
    public partial class AlgebraLinearWindow : Window
    {
        private Dictionary<string, object> variables = new Dictionary<string, object>();
        private StringBuilder consoleBuffer = new StringBuilder();

        public AlgebraLinearWindow()
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
                    WriteError("Código vazio! Digite algo para executar.");
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
                else if (line.Contains("matriz", StringComparison.OrdinalIgnoreCase))
                {
                    ExecuteMatriz(line);
                }
                else if (line.Contains("vetor", StringComparison.OrdinalIgnoreCase))
                {
                    ExecuteVetor(line);
                }
                else if (line.Contains("determinante", StringComparison.OrdinalIgnoreCase))
                {
                    ExecuteDeterminante(line);
                }
                else if (line.Contains("multiplicar", StringComparison.OrdinalIgnoreCase))
                {
                    ExecuteMultiplicar(line);
                }
                else if (line.Contains("transposta", StringComparison.OrdinalIgnoreCase))
                {
                    ExecuteTransposta(line);
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
            Match match = Regex.Match(line, @"escreva\s*\((.*)\)", RegexOptions.IgnoreCase);

            if (!match.Success)
            {
                WriteError("Sintaxe incorreta em escreva()");
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
                WriteLine(variables[content].ToString() ?? "");
            }
            else
            {
                WriteError($"Variável '{content}' não encontrada.");
            }
        }

        private void ExecuteAtribuicao(string line)
        {
            line = Regex.Replace(line, @"^\s*var\s+", "", RegexOptions.IgnoreCase);

            string[] parts = line.Split('=');
            if (parts.Length != 2)
            {
                WriteError("Sintaxe incorreta em atribuição");
                return;
            }

            string varName = parts[0].Trim();
            string valueStr = parts[1].Trim();

            if (!Regex.IsMatch(varName, @"^[a-zA-Z_][a-zA-Z0-9_]*$"))
            {
                WriteError($"Nome de variável inválido: {varName}");
                return;
            }

            // Se for chamada de função, executa
            if (valueStr.Contains("matriz("))
            {
                ExecuteMatriz(valueStr, varName);
            }
            else if (valueStr.Contains("vetor("))
            {
                ExecuteVetor(valueStr, varName);
            }
            else
            {
                WriteError($"Tipo de atribuição não suportado: {valueStr}");
            }
        }

        private void ExecuteMatriz(string line, string? varName = null)
        {
            Match match = Regex.Match(line, @"matriz\s*\(\s*\[(.*)\]\s*\)", RegexOptions.IgnoreCase);

            if (!match.Success)
            {
                WriteError("Sintaxe incorreta. Use: matriz([[1,2],[3,4]])");
                return;
            }

            try
            {
                string content = match.Groups[1].Value;
                string[] rows = content.Split(new[] { "],[" }, StringSplitOptions.None);

                double[,]? matrix = null;
                int numRows = rows.Length;
                int numCols = 0;

                for (int i = 0; i < rows.Length; i++)
                {
                    string row = rows[i].Replace("[", "").Replace("]", "").Trim();
                    string[] values = row.Split(',');

                    if (i == 0)
                    {
                        numCols = values.Length;
                        matrix = new double[numRows, numCols];
                    }

                    if (matrix != null)
                    {
                        for (int j = 0; j < values.Length; j++)
                        {
                            matrix[i, j] = double.Parse(values[j].Trim());
                        }
                    }
                }

                if (matrix != null)
                {
                    if (varName != null)
                    {
                        variables[varName] = matrix;
                        WriteSuccess($"Matriz {varName} ({numRows}x{numCols}) criada!");
                    }
                    else
                    {
                        MostrarMatriz(matrix);
                    }
                }
            }
            catch (Exception ex)
            {
                WriteError($"Erro ao criar matriz: {ex.Message}");
            }
        }

        private void ExecuteVetor(string line, string? varName = null)
        {
            Match match = Regex.Match(line, @"vetor\s*\(\s*\[(.*)\]\s*\)", RegexOptions.IgnoreCase);

            if (!match.Success)
            {
                WriteError("Sintaxe incorreta. Use: vetor([1,2,3])");
                return;
            }

            try
            {
                string content = match.Groups[1].Value;
                string[] values = content.Split(',');
                double[] vetor = new double[values.Length];

                for (int i = 0; i < values.Length; i++)
                {
                    vetor[i] = double.Parse(values[i].Trim());
                }

                if (varName != null)
                {
                    variables[varName] = vetor;
                    WriteSuccess($"Vetor {varName} (dimensão {vetor.Length}) criado!");
                }
                else
                {
                    MostrarVetor(vetor);
                }
            }
            catch (Exception ex)
            {
                WriteError($"Erro ao criar vetor: {ex.Message}");
            }
        }

        private void ExecuteDeterminante(string line)
        {
            Match match = Regex.Match(line, @"determinante\s*\(([^)]+)\)", RegexOptions.IgnoreCase);

            if (!match.Success)
            {
                WriteError("Sintaxe incorreta. Use: determinante(A)");
                return;
            }

            string varName = match.Groups[1].Value.Trim();

            if (!variables.ContainsKey(varName))
            {
                WriteError($"Matriz '{varName}' não encontrada.");
                return;
            }

            if (!(variables[varName] is double[,] matrix))
            {
                WriteError($"'{varName}' não é uma matriz.");
                return;
            }

            if (matrix.GetLength(0) != matrix.GetLength(1))
            {
                WriteError("Determinante só existe para matrizes quadradas.");
                return;
            }

            double det = CalcularDeterminante(matrix);
            WriteLine($"det({varName}) = {det}");
        }

        private void ExecuteMultiplicar(string line)
        {
            Match match = Regex.Match(line, @"multiplicar\s*\(([^,]+),([^)]+)\)", RegexOptions.IgnoreCase);

            if (!match.Success)
            {
                WriteError("Sintaxe incorreta. Use: multiplicar(A, B)");
                return;
            }

            string var1 = match.Groups[1].Value.Trim();
            string var2 = match.Groups[2].Value.Trim();

            if (!variables.ContainsKey(var1) || !variables.ContainsKey(var2))
            {
                WriteError("Uma das variáveis não foi encontrada.");
                return;
            }

            if (variables[var1] is double[,] m1 && variables[var2] is double[,] m2)
            {
                if (m1.GetLength(1) != m2.GetLength(0))
                {
                    WriteError("Dimensões incompatíveis para multiplicação.");
                    return;
                }

                double[,] result = MultiplicarMatrizes(m1, m2);
                WriteLine($"Resultado de {var1} × {var2}:");
                MostrarMatriz(result);
            }
            else
            {
                WriteError("Ambas variáveis devem ser matrizes.");
            }
        }

        private void ExecuteTransposta(string line)
        {
            Match match = Regex.Match(line, @"transposta\s*\(([^)]+)\)", RegexOptions.IgnoreCase);

            if (!match.Success)
            {
                WriteError("Sintaxe incorreta. Use: transposta(A)");
                return;
            }

            string varName = match.Groups[1].Value.Trim();

            if (!variables.ContainsKey(varName))
            {
                WriteError($"Matriz '{varName}' não encontrada.");
                return;
            }

            if (!(variables[varName] is double[,] matrix))
            {
                WriteError($"'{varName}' não é uma matriz.");
                return;
            }

            double[,] result = Transpor(matrix);
            WriteLine($"Transposta de {varName}:");
            MostrarMatriz(result);
        }

        #endregion

        #region Operações Matemáticas

        private double CalcularDeterminante(double[,] matrix)
        {
            int n = matrix.GetLength(0);

            if (n == 1)
                return matrix[0, 0];

            if (n == 2)
                return matrix[0, 0] * matrix[1, 1] - matrix[0, 1] * matrix[1, 0];

            double det = 0;
            for (int j = 0; j < n; j++)
            {
                det += Math.Pow(-1, j) * matrix[0, j] * CalcularDeterminante(GetSubmatrix(matrix, 0, j));
            }

            return det;
        }

        private double[,] GetSubmatrix(double[,] matrix, int row, int col)
        {
            int n = matrix.GetLength(0);
            double[,] result = new double[n - 1, n - 1];

            int r = 0;
            for (int i = 0; i < n; i++)
            {
                if (i == row) continue;
                int c = 0;
                for (int j = 0; j < n; j++)
                {
                    if (j == col) continue;
                    result[r, c] = matrix[i, j];
                    c++;
                }
                r++;
            }

            return result;
        }

        private double[,] MultiplicarMatrizes(double[,] m1, double[,] m2)
        {
            int rows1 = m1.GetLength(0);
            int cols1 = m1.GetLength(1);
            int cols2 = m2.GetLength(1);

            double[,] result = new double[rows1, cols2];

            for (int i = 0; i < rows1; i++)
            {
                for (int j = 0; j < cols2; j++)
                {
                    result[i, j] = 0;
                    for (int k = 0; k < cols1; k++)
                    {
                        result[i, j] += m1[i, k] * m2[k, j];
                    }
                }
            }

            return result;
        }

        private double[,] Transpor(double[,] matrix)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);
            double[,] result = new double[cols, rows];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    result[j, i] = matrix[i, j];
                }
            }

            return result;
        }

        #endregion

        #region Exibição

        private void MostrarMatriz(double[,] matrix)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            for (int i = 0; i < rows; i++)
            {
                StringBuilder row = new StringBuilder("[ ");
                for (int j = 0; j < cols; j++)
                {
                    row.Append($"{matrix[i, j],6:F2} ");
                }
                row.Append("]");
                WriteLine(row.ToString());
            }
        }

        private void MostrarVetor(double[] vetor)
        {
            StringBuilder sb = new StringBuilder("[ ");
            foreach (double val in vetor)
            {
                sb.Append($"{val} ");
            }
            sb.Append("]");
            WriteLine(sb.ToString());
        }

        #endregion

        #region Console

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
            try
            {
                var testeWindow = new Testes.AlgebraLinearTesteWindow();
                testeWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Erro ao abrir o teste: {ex.Message}",
                    "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        
        

        #endregion
    }
}