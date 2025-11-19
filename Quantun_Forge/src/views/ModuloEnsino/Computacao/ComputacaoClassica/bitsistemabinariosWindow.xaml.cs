using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace Quantun_Forge.src.views.ModuloEnsino.Computacao.ComputacaoClassica
{
    public partial class bitsistemabinariosWindow : Window
    {
        public bitsistemabinariosWindow()
        {
            InitializeComponent();
        }

        private void RunCode_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string code = CodeEditor.Text;
                ConsoleOutput.Text = "> Executando código...\n\n";

                // Remove comentários
                code = RemoveComments(code);

                // Divide o código em linhas
                string[] lines = code.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string line in lines)
                {
                    string trimmedLine = line.Trim();
                    if (string.IsNullOrWhiteSpace(trimmedLine))
                        continue;

                    ProcessLine(trimmedLine);
                }

                ConsoleOutput.Text += "\n> Execução concluída com sucesso!";
            }
            catch (Exception ex)
            {
                ConsoleOutput.Text += $"\n❌ ERRO: {ex.Message}";
            }
        }

        private void ClearCode_Click(object sender, RoutedEventArgs e)
        {
            CodeEditor.Text = "// Digite seu código aqui\n";
            ConsoleOutput.Text = "> Sistema Binário Interpretador iniciado.\n> Digite seus comandos e clique em EXECUTAR.\n> Explore o mundo dos bits e bytes!\n";
        }

        private string RemoveComments(string code)
        {
            // Remove comentários de linha única //
            code = Regex.Replace(code, @"//.*$", "", RegexOptions.Multiline);
            return code;
        }

        private void ProcessLine(string line)
        {
            try
            {
                // escreva("texto")
                if (line.Contains("escreva("))
                {
                    ExecuteEscreva(line);
                }
                // binarioParaDecimal(numero)
                else if (line.Contains("binarioParaDecimal("))
                {
                    ExecuteBinarioParaDecimal(line);
                }
                // decimalParaBinario(numero)
                else if (line.Contains("decimalParaBinario("))
                {
                    ExecuteDecimalParaBinario(line);
                }
                // AND(bit1, bit2)
                else if (line.Contains("AND("))
                {
                    ExecuteAND(line);
                }
                // OR(bit1, bit2)
                else if (line.Contains("OR("))
                {
                    ExecuteOR(line);
                }
                // NOT(bit)
                else if (line.Contains("NOT("))
                {
                    ExecuteNOT(line);
                }
                // bytesParaBits(bytes)
                else if (line.Contains("bytesParaBits("))
                {
                    ExecuteBytesParaBits(line);
                }
                // bitsParaBytes(bits)
                else if (line.Contains("bitsParaBytes("))
                {
                    ExecuteBitsParaBytes(line);
                }
                // XOR(bit1, bit2)
                else if (line.Contains("XOR("))
                {
                    ExecuteXOR(line);
                }
                // somarBinarios(bin1, bin2)
                else if (line.Contains("somarBinarios("))
                {
                    ExecuteSomarBinarios(line);
                }
                // complementoDois(binario)
                else if (line.Contains("complementoDois("))
                {
                    ExecuteComplementoDois(line);
                }
                else
                {
                    ConsoleOutput.Text += $"⚠️  Comando não reconhecido: {line}\n";
                }
            }
            catch (Exception ex)
            {
                ConsoleOutput.Text += $"❌ Erro na linha '{line}': {ex.Message}\n";
            }
        }

        #region Funções de Comando

        private void ExecuteEscreva(string line)
        {
            // Extrai o texto entre aspas
            Match match = Regex.Match(line, @"escreva\s*\(\s*""([^""]*)""\s*\)");
            if (match.Success)
            {
                string texto = match.Groups[1].Value;
                ConsoleOutput.Text += $"{texto}\n";
            }
            else
            {
                ConsoleOutput.Text += "❌ Erro: Formato incorreto. Use: escreva(\"texto\")\n";
            }
        }

        private void ExecuteBinarioParaDecimal(string line)
        {
            Match match = Regex.Match(line, @"binarioParaDecimal\s*\(\s*(\d+)\s*\)");
            if (match.Success)
            {
                string binario = match.Groups[1].Value;

                // Valida se é binário
                if (!Regex.IsMatch(binario, @"^[01]+$"))
                {
                    ConsoleOutput.Text += "❌ Erro: Número binário inválido. Use apenas 0 e 1.\n";
                    return;
                }

                int decimal_value = Convert.ToInt32(binario, 2);
                ConsoleOutput.Text += $"📊 {binario}₂ = {decimal_value}₁₀\n";

                // Mostra o cálculo
                string calculo = MostrarCalculoBinarioParaDecimal(binario);
                ConsoleOutput.Text += $"   Cálculo: {calculo}\n";
            }
            else
            {
                ConsoleOutput.Text += "❌ Erro: Formato incorreto. Use: binarioParaDecimal(1010)\n";
            }
        }

        private string MostrarCalculoBinarioParaDecimal(string binario)
        {
            List<string> parcelas = new List<string>();
            int tamanho = binario.Length;

            for (int i = 0; i < tamanho; i++)
            {
                int bit = binario[i] - '0';
                int potencia = tamanho - 1 - i;
                int valor = bit * (int)Math.Pow(2, potencia);

                if (bit == 1)
                {
                    parcelas.Add($"{bit}×2^{potencia}");
                }
            }

            return string.Join(" + ", parcelas);
        }

        private void ExecuteDecimalParaBinario(string line)
        {
            Match match = Regex.Match(line, @"decimalParaBinario\s*\(\s*(\d+)\s*\)");
            if (match.Success)
            {
                int decimal_value = int.Parse(match.Groups[1].Value);

                if (decimal_value < 0)
                {
                    ConsoleOutput.Text += "❌ Erro: Use apenas números positivos.\n";
                    return;
                }

                string binario = Convert.ToString(decimal_value, 2);
                ConsoleOutput.Text += $"📊 {decimal_value}₁₀ = {binario}₂\n";

                // Mostra em formato de 8 bits se for menor que 256
                if (decimal_value < 256)
                {
                    string binario8bits = binario.PadLeft(8, '0');
                    ConsoleOutput.Text += $"   8 bits: {FormatarBinario(binario8bits)}\n";
                }
            }
            else
            {
                ConsoleOutput.Text += "❌ Erro: Formato incorreto. Use: decimalParaBinario(42)\n";
            }
        }

        private string FormatarBinario(string binario)
        {
            // Formata em grupos de 4 bits para melhor visualização
            if (binario.Length == 8)
            {
                return $"{binario.Substring(0, 4)} {binario.Substring(4, 4)}";
            }
            return binario;
        }

        private void ExecuteAND(string line)
        {
            Match match = Regex.Match(line, @"AND\s*\(\s*([01])\s*,\s*([01])\s*\)");
            if (match.Success)
            {
                int bit1 = int.Parse(match.Groups[1].Value);
                int bit2 = int.Parse(match.Groups[2].Value);
                int resultado = bit1 & bit2;

                ConsoleOutput.Text += $"🔧 {bit1} AND {bit2} = {resultado}\n";
                ConsoleOutput.Text += $"   Operação: {bit1} & {bit2} → {resultado}\n";
            }
            else
            {
                ConsoleOutput.Text += "❌ Erro: Formato incorreto. Use: AND(1, 0)\n";
            }
        }

        private void ExecuteOR(string line)
        {
            Match match = Regex.Match(line, @"OR\s*\(\s*([01])\s*,\s*([01])\s*\)");
            if (match.Success)
            {
                int bit1 = int.Parse(match.Groups[1].Value);
                int bit2 = int.Parse(match.Groups[2].Value);
                int resultado = bit1 | bit2;

                ConsoleOutput.Text += $"🔧 {bit1} OR {bit2} = {resultado}\n";
                ConsoleOutput.Text += $"   Operação: {bit1} | {bit2} → {resultado}\n";
            }
            else
            {
                ConsoleOutput.Text += "❌ Erro: Formato incorreto. Use: OR(1, 0)\n";
            }
        }

        private void ExecuteNOT(string line)
        {
            Match match = Regex.Match(line, @"NOT\s*\(\s*([01])\s*\)");
            if (match.Success)
            {
                int bit = int.Parse(match.Groups[1].Value);
                int resultado = bit == 0 ? 1 : 0;

                ConsoleOutput.Text += $"🔧 NOT {bit} = {resultado}\n";
                ConsoleOutput.Text += $"   Operação: ~{bit} → {resultado}\n";
            }
            else
            {
                ConsoleOutput.Text += "❌ Erro: Formato incorreto. Use: NOT(1)\n";
            }
        }

        private void ExecuteBytesParaBits(string line)
        {
            Match match = Regex.Match(line, @"bytesParaBits\s*\(\s*([\d.]+)\s*\)");
            if (match.Success)
            {
                double bytes = double.Parse(match.Groups[1].Value.Replace('.', ','));
                double bits = bytes * 8;

                ConsoleOutput.Text += $"💾 {bytes} bytes = {bits} bits\n";
                ConsoleOutput.Text += $"   Cálculo: {bytes} × 8 = {bits}\n";
            }
            else
            {
                ConsoleOutput.Text += "❌ Erro: Formato incorreto. Use: bytesParaBits(10)\n";
            }
        }

        private void ExecuteBitsParaBytes(string line)
        {
            Match match = Regex.Match(line, @"bitsParaBytes\s*\(\s*(\d+)\s*\)");
            if (match.Success)
            {
                double bits = double.Parse(match.Groups[1].Value);
                double bytes = bits / 8.0;

                ConsoleOutput.Text += $"💾 {bits} bits = {bytes} bytes\n";
                ConsoleOutput.Text += $"   Cálculo: {bits} ÷ 8 = {bytes}\n";
            }
            else
            {
                ConsoleOutput.Text += "❌ Erro: Formato incorreto. Use: bitsParaBytes(64)\n";
            }
        }

        private void ExecuteXOR(string line)
        {
            Match match = Regex.Match(line, @"XOR\s*\(\s*([01])\s*,\s*([01])\s*\)");
            if (match.Success)
            {
                int bit1 = int.Parse(match.Groups[1].Value);
                int bit2 = int.Parse(match.Groups[2].Value);
                int resultado = bit1 ^ bit2;

                ConsoleOutput.Text += $"🔧 {bit1} XOR {bit2} = {resultado}\n";
                ConsoleOutput.Text += $"   Operação: {bit1} ⊕ {bit2} → {resultado}\n";
            }
            else
            {
                ConsoleOutput.Text += "❌ Erro: Formato incorreto. Use: XOR(1, 0)\n";
            }
        }

        private void ExecuteSomarBinarios(string line)
        {
            Match match = Regex.Match(line, @"somarBinarios\s*\(\s*(\d+)\s*,\s*(\d+)\s*\)");
            if (match.Success)
            {
                string bin1 = match.Groups[1].Value;
                string bin2 = match.Groups[2].Value;

                // Valida se são binários
                if (!Regex.IsMatch(bin1, @"^[01]+$") || !Regex.IsMatch(bin2, @"^[01]+$"))
                {
                    ConsoleOutput.Text += "❌ Erro: Números binários inválidos.\n";
                    return;
                }

                int dec1 = Convert.ToInt32(bin1, 2);
                int dec2 = Convert.ToInt32(bin2, 2);
                int soma = dec1 + dec2;
                string resultado = Convert.ToString(soma, 2);

                ConsoleOutput.Text += $"➕ {bin1}₂ + {bin2}₂ = {resultado}₂\n";
                ConsoleOutput.Text += $"   Em decimal: {dec1} + {dec2} = {soma}\n";
            }
            else
            {
                ConsoleOutput.Text += "❌ Erro: Formato incorreto. Use: somarBinarios(1010, 0110)\n";
            }
        }

        private void ExecuteComplementoDois(string line)
        {
            Match match = Regex.Match(line, @"complementoDois\s*\(\s*(\d+)\s*\)");
            if (match.Success)
            {
                string binario = match.Groups[1].Value;

                if (!Regex.IsMatch(binario, @"^[01]+$"))
                {
                    ConsoleOutput.Text += "❌ Erro: Número binário inválido.\n";
                    return;
                }

                // Inverte todos os bits (complemento de 1)
                string complementoUm = "";
                foreach (char bit in binario)
                {
                    complementoUm += bit == '0' ? '1' : '0';
                }

                // Soma 1 (complemento de 2)
                int valorComplementoUm = Convert.ToInt32(complementoUm, 2);
                int complementoDois = valorComplementoUm + 1;
                string resultadoBinario = Convert.ToString(complementoDois, 2);

                // Ajusta para o mesmo tamanho
                if (resultadoBinario.Length > binario.Length)
                {
                    resultadoBinario = resultadoBinario.Substring(resultadoBinario.Length - binario.Length);
                }
                else
                {
                    resultadoBinario = resultadoBinario.PadLeft(binario.Length, '0');
                }

                ConsoleOutput.Text += $"🔄 Complemento de 2 de {binario}:\n";
                ConsoleOutput.Text += $"   Passo 1 (inverter): {complementoUm}\n";
                ConsoleOutput.Text += $"   Passo 2 (somar 1): {resultadoBinario}\n";
            }
            else
            {
                ConsoleOutput.Text += "❌ Erro: Formato incorreto. Use: complementoDois(1010)\n";
            }
        }

        #endregion

        private void btnExePratica_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "Exercícios práticos serão implementados em breve!\n\n" +
                "Aqui você poderá praticar:\n" +
                "• Conversões binárias\n" +
                "• Operações lógicas\n" +
                "• Aritmética binária\n" +
                "• Desafios progressivos",
                "Exercícios Práticos",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }
    }
}