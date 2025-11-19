using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace Quantun_Forge.src.views.ModuloEnsino.Fisica.FisicaClassica
{
    public partial class TermodinamicaWindow : Window
    {
        public TermodinamicaWindow()
        {
            InitializeComponent();
        }

        // Botão EXECUTAR
        private void RunCode_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string code = CodeEditor.Text;
                ConsoleOutput.Text = "> Executando código...\n\n";

                // Processa cada linha do código
                string[] lines = code.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string line in lines)
                {
                    string trimmedLine = line.Trim();

                    // Ignora comentários e linhas vazias
                    if (string.IsNullOrWhiteSpace(trimmedLine) || trimmedLine.StartsWith("//"))
                        continue;

                    // Processa os comandos
                    ProcessCommand(trimmedLine);
                }

                ConsoleOutput.Text += "\n> Execução concluída com sucesso! ✓";
            }
            catch (Exception ex)
            {
                ConsoleOutput.Text += $"\n❌ ERRO: {ex.Message}";
            }
        }

        // Processa cada comando
        private void ProcessCommand(string command)
        {
            try
            {
                // escreva("mensagem")
                if (command.Contains("escreva("))
                {
                    string message = ExtractStringParameter(command, "escreva");
                    ConsoleOutput.Text += message + "\n";
                }
                // converterTemperatura(valor: X, de: 'Y', para: 'Z')
                else if (command.Contains("converterTemperatura("))
                {
                    var parameters = ExtractNamedParameters(command);
                    double valor = double.Parse(parameters["valor"], CultureInfo.InvariantCulture);
                    string de = parameters["de"].Trim('\'', '"');
                    string para = parameters["para"].Trim('\'', '"');

                    double resultado = ConverterTemperatura(valor, de, para);
                    ConsoleOutput.Text += $"🌡️ Conversão: {valor}°{de} = {resultado:F2}°{para}\n";
                }
                // primeiraLei(Q: X, W: Y)
                else if (command.Contains("primeiraLei("))
                {
                    var parameters = ExtractNamedParameters(command);
                    double Q = double.Parse(parameters["Q"], CultureInfo.InvariantCulture);
                    double W = double.Parse(parameters["W"], CultureInfo.InvariantCulture);

                    double deltaU = Q - W;
                    ConsoleOutput.Text += $"⚡ PRIMEIRA LEI DA TERMODINÂMICA\n";
                    ConsoleOutput.Text += $"   ΔU = Q - W\n";
                    ConsoleOutput.Text += $"   ΔU = {Q} - {W}\n";
                    ConsoleOutput.Text += $"   ΔU = {deltaU} J\n";

                    if (deltaU > 0)
                        ConsoleOutput.Text += $"   → Energia interna AUMENTOU em {deltaU} J\n";
                    else if (deltaU < 0)
                        ConsoleOutput.Text += $"   → Energia interna DIMINUIU em {Math.Abs(deltaU)} J\n";
                    else
                        ConsoleOutput.Text += $"   → Energia interna CONSTANTE\n";
                }
                // calcularEficiencia(trabalho: X, calorQuente: Y)
                else if (command.Contains("calcularEficiencia("))
                {
                    var parameters = ExtractNamedParameters(command);
                    double trabalho = double.Parse(parameters["trabalho"], CultureInfo.InvariantCulture);
                    double calorQuente = double.Parse(parameters["calorQuente"], CultureInfo.InvariantCulture);

                    double eficiencia = (trabalho / calorQuente) * 100;
                    ConsoleOutput.Text += $"📊 EFICIÊNCIA DA MÁQUINA TÉRMICA\n";
                    ConsoleOutput.Text += $"   η = W / Q_quente\n";
                    ConsoleOutput.Text += $"   η = {trabalho} / {calorQuente}\n";
                    ConsoleOutput.Text += $"   η = {eficiencia:F2}%\n";

                    if (eficiencia < 30)
                        ConsoleOutput.Text += $"   → Eficiência baixa\n";
                    else if (eficiencia < 60)
                        ConsoleOutput.Text += $"   → Eficiência moderada\n";
                    else
                        ConsoleOutput.Text += $"   → Eficiência alta\n";
                }
                // eficienciaCarnot(Tquente: X, Tfrio: Y)
                else if (command.Contains("eficienciaCarnot("))
                {
                    var parameters = ExtractNamedParameters(command);
                    double Tquente = double.Parse(parameters["Tquente"], CultureInfo.InvariantCulture);
                    double Tfrio = double.Parse(parameters["Tfrio"], CultureInfo.InvariantCulture);

                    double eficienciaCarnot = (1 - (Tfrio / Tquente)) * 100;
                    ConsoleOutput.Text += $"🏆 EFICIÊNCIA MÁXIMA (CICLO DE CARNOT)\n";
                    ConsoleOutput.Text += $"   η_Carnot = 1 - (T_frio / T_quente)\n";
                    ConsoleOutput.Text += $"   η_Carnot = 1 - ({Tfrio} K / {Tquente} K)\n";
                    ConsoleOutput.Text += $"   η_Carnot = {eficienciaCarnot:F2}%\n";
                    ConsoleOutput.Text += $"   → Esta é a eficiência MÁXIMA teórica!\n";
                    ConsoleOutput.Text += $"   → Nenhuma máquina real pode superar este valor\n";
                }
                // calcularEntropia(calor: X, temperatura: Y)
                else if (command.Contains("calcularEntropia("))
                {
                    var parameters = ExtractNamedParameters(command);
                    double calor = double.Parse(parameters["calor"], CultureInfo.InvariantCulture);
                    double temperatura = double.Parse(parameters["temperatura"], CultureInfo.InvariantCulture);

                    double entropia = calor / temperatura;
                    ConsoleOutput.Text += $"🎲 VARIAÇÃO DE ENTROPIA\n";
                    ConsoleOutput.Text += $"   ΔS = Q / T\n";
                    ConsoleOutput.Text += $"   ΔS = {calor} J / {temperatura} K\n";
                    ConsoleOutput.Text += $"   ΔS = {entropia:F4} J/K\n";

                    if (entropia > 0)
                        ConsoleOutput.Text += $"   → Entropia AUMENTOU (desordem aumentou)\n";
                    else if (entropia < 0)
                        ConsoleOutput.Text += $"   → Entropia DIMINUIU (ordem aumentou)\n";
                    else
                        ConsoleOutput.Text += $"   → Entropia CONSTANTE (processo reversível)\n";
                }
            }
            catch (Exception ex)
            {
                ConsoleOutput.Text += $"❌ Erro ao processar comando: {ex.Message}\n";
            }
        }

        // Converte temperatura entre escalas
        private double ConverterTemperatura(double valor, string de, string para)
        {
            de = de.ToUpper();
            para = para.ToUpper();

            // Primeiro converte para Kelvin
            double kelvin = 0;
            switch (de)
            {
                case "K":
                    kelvin = valor;
                    break;
                case "C":
                    kelvin = valor + 273.15;
                    break;
                case "F":
                    kelvin = (valor - 32) * 5.0 / 9.0 + 273.15;
                    break;
                default:
                    throw new ArgumentException($"Escala '{de}' não reconhecida. Use C, K ou F.");
            }

            // Depois converte de Kelvin para a escala desejada
            switch (para)
            {
                case "K":
                    return kelvin;
                case "C":
                    return kelvin - 273.15;
                case "F":
                    return (kelvin - 273.15) * 9.0 / 5.0 + 32;
                default:
                    throw new ArgumentException($"Escala '{para}' não reconhecida. Use C, K ou F.");
            }
        }

        // Extrai string de dentro de aspas
        private string ExtractStringParameter(string command, string functionName)
        {
            int start = command.IndexOf('(') + 1;
            int end = command.LastIndexOf(')');
            string content = command.Substring(start, end - start);

            // Remove aspas
            content = content.Trim().Trim('"', '\'');
            return content;
        }

        // Extrai parâmetros nomeados (param: valor)
        private Dictionary<string, string> ExtractNamedParameters(string command)
        {
            var parameters = new Dictionary<string, string>();

            int start = command.IndexOf('(') + 1;
            int end = command.LastIndexOf(')');
            string content = command.Substring(start, end - start);

            // Divide por vírgula (mas não dentro de strings)
            var parts = Regex.Split(content, @",(?=(?:[^""']*[""'][^""']*[""'])*[^""']*$)");

            foreach (string part in parts)
            {
                string[] keyValue = part.Split(new[] { ':' }, 2);
                if (keyValue.Length == 2)
                {
                    string key = keyValue[0].Trim();
                    string value = keyValue[1].Trim();
                    parameters[key] = value;
                }
            }

            return parameters;
        }

        // Botão LIMPAR
        private void ClearCode_Click(object sender, RoutedEventArgs e)
        {
            CodeEditor.Text = "// Digite seu código aqui\n";
            ConsoleOutput.Text = "> Console limpo.\n> Digite seu código e clique em EXECUTAR.";
        }

        // Botão EXERCÍCIOS
        private void ExercisesButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "📝 EXERCÍCIOS DE TERMODINÂMICA\n\n" +
                "1. Calcule a variação de energia interna de um gás que recebe 500 J de calor e realiza 200 J de trabalho.\n\n" +
                "2. Um motor térmico recebe 3000 J de calor e realiza 900 J de trabalho. Qual sua eficiência?\n\n" +
                "3. Qual a eficiência máxima de uma máquina de Carnot operando entre 600 K e 300 K?\n\n" +
                "4. Converta 100°C para Kelvin e Fahrenheit.\n\n" +
                "5. Calcule a variação de entropia quando 2000 J de calor são transferidos a 400 K.",
                "Exercícios - Termodinâmica",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }

        // Botão QUIZ
        private void QuizButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "🧪 QUIZ DE TERMODINÂMICA\n\n" +
                "1. Qual lei afirma que a energia não pode ser criada nem destruída?\n" +
                "   R: Primeira Lei da Termodinâmica\n\n" +
                "2. O que significa entropia?\n" +
                "   R: Medida da desordem de um sistema\n\n" +
                "3. Qual é a temperatura do zero absoluto em Celsius?\n" +
                "   R: -273.15°C\n\n" +
                "4. Por que nenhuma máquina térmica pode ter 100% de eficiência?\n" +
                "   R: Por causa da Segunda Lei da Termodinâmica\n\n" +
                "5. O que é o Ciclo de Carnot?\n" +
                "   R: Ciclo termodinâmico ideal com eficiência máxima teórica",
                "Quiz - Termodinâmica",
                MessageBoxButton.OK,
                MessageBoxImage.Question
            );
        }

        // Botão VOLTAR
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}