using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Quantun_Forge.src.views.ModuloEnsino.Fisica.FisicaClassica
{
    /// <summary>
    /// Lógica interna para OndasEletromagenetismoWindow.xaml
    /// </summary>
    public partial class OndasEletromagenetismoWindow : Window
    {
        // Constantes físicas
        private const double VELOCIDADE_LUZ = 299792458; // m/s
        private const double CONSTANTE_PLANCK = 6.62607015e-34; // J·s
        private const double ELETRON_VOLT = 1.602176634e-19; // J

        // StringBuilder para saída do console
        private StringBuilder outputBuilder;

        public OndasEletromagenetismoWindow()
        {
            InitializeComponent();
            InicializarCalculadora();
        }

        private void InicializarCalculadora()
        {
            outputBuilder = new StringBuilder();
        }

        private void RunCode_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Limpa o console
                outputBuilder.Clear();

                // Validação do código
                if (string.IsNullOrWhiteSpace(CodeEditor.Text))
                {
                    AdicionarSaida("⚠️ Por favor, digite algum código antes de executar.\n", "#F39C12");
                    ConsoleOutput.Text = outputBuilder.ToString();
                    return;
                }

                // Pega o código do editor
                string codigo = CodeEditor.Text;

                // Adiciona mensagem inicial
                AdicionarSaida("🌊 Iniciando Calculadora de Ondas Eletromagnéticas...\n", "#8E44AD");
                AdicionarSaida("═══════════════════════════════════════\n\n", "#7F8C8D");

                // Processa o código linha por linha
                ProcessarCodigo(codigo);

                // Adiciona mensagem final
                AdicionarSaida("\n═══════════════════════════════════════\n", "#7F8C8D");
                AdicionarSaida("✅ Cálculos concluídos com sucesso!\n", "#27AE60");

                // Exibe resultado no console
                ConsoleOutput.Text = outputBuilder.ToString();
            }
            catch (Exception ex)
            {
                AdicionarSaida($"\n❌ ERRO: {ex.Message}\n", "#E74C3C");
                AdicionarSaida($"💡 Verifique a sintaxe do seu código.\n", "#F39C12");
                ConsoleOutput.Text = outputBuilder.ToString();
            }
        }

        private void ProcessarCodigo(string codigo)
        {
            // Remove comentários e divide em linhas
            var linhas = codigo.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var linha in linhas)
            {
                string linhaLimpa = linha.Trim();

                // Ignora comentários e linhas vazias
                if (linhaLimpa.StartsWith("//") || string.IsNullOrWhiteSpace(linhaLimpa))
                    continue;

                // Remove comentários inline
                int indexComentario = linhaLimpa.IndexOf("//");
                if (indexComentario >= 0)
                {
                    linhaLimpa = linhaLimpa.Substring(0, indexComentario).Trim();
                }

                // Processa comandos
                if (!string.IsNullOrWhiteSpace(linhaLimpa))
                {
                    ProcessarComando(linhaLimpa);
                }
            }
        }

        private void ProcessarComando(string comando)
        {
            comando = comando.Trim();

            try
            {
                // Comando: escreva("mensagem")
                if (comando.StartsWith("escreva("))
                {
                    string mensagem = ExtrairTextoEntreAspas(comando);
                    AdicionarSaida(mensagem + "\n", "#27AE60");
                }
                // Comando: calcularFrequencia(comprimentoOnda: X)
                else if (comando.StartsWith("calcularFrequencia("))
                {
                    CalcularFrequencia(comando);
                }
                // Comando: calcularComprimentoOnda(frequencia: X)
                else if (comando.StartsWith("calcularComprimentoOnda("))
                {
                    CalcularComprimentoOnda(comando);
                }
                // Comando: calcularEnergia(frequencia: X)
                else if (comando.StartsWith("calcularEnergia("))
                {
                    CalcularEnergia(comando);
                }
                // Comando: identificarTipo(comprimentoOnda: X)
                else if (comando.StartsWith("identificarTipo("))
                {
                    IdentificarTipo(comando);
                }
                else if (!string.IsNullOrWhiteSpace(comando))
                {
                    AdicionarSaida($"⚠️ Comando desconhecido: {comando}\n", "#F39C12");
                }
            }
            catch (Exception ex)
            {
                AdicionarSaida($"❌ Erro ao processar comando '{comando}': {ex.Message}\n", "#E74C3C");
            }
        }

        private void CalcularFrequencia(string comando)
        {
            try
            {
                // Extrai comprimento de onda em nanômetros
                double comprimentoOndaNm = ExtrairValorParametro(comando, "comprimentoOnda:");

                if (comprimentoOndaNm <= 0)
                {
                    AdicionarSaida("❌ Erro: O comprimento de onda deve ser maior que zero!\n", "#E74C3C");
                    return;
                }

                // Converte para metros
                double comprimentoOndaM = comprimentoOndaNm * 1e-9;

                // Calcula frequência: f = c / λ
                double frequencia = VELOCIDADE_LUZ / comprimentoOndaM;

                AdicionarSaida($"📊 Calculando Frequência:\n", "#8E44AD");
                AdicionarSaida($"   ═══════════════════════════════════\n", "#7F8C8D");
                AdicionarSaida($"   📐 Fórmula: f = c / λ\n", "#7D3C98");
                AdicionarSaida($"\n", "#7F8C8D");
                AdicionarSaida($"   📊 Dados:\n", "#7F8C8D");
                AdicionarSaida($"      • λ (lambda) = {comprimentoOndaNm} nm = {comprimentoOndaM:E2} m\n", "#7F8C8D");
                AdicionarSaida($"      • c (velocidade da luz) = {VELOCIDADE_LUZ:E2} m/s\n", "#7F8C8D");
                AdicionarSaida($"\n", "#7F8C8D");
                AdicionarSaida($"   🔢 Cálculo:\n", "#7F8C8D");
                AdicionarSaida($"      f = {VELOCIDADE_LUZ:E2} / {comprimentoOndaM:E2}\n", "#7F8C8D");
                AdicionarSaida($"\n", "#7F8C8D");
                AdicionarSaida($"   ✅ Resultado:\n", "#27AE60");
                AdicionarSaida($"      f = {frequencia:E3} Hz\n", "#27AE60");

                // Adiciona informação em diferentes unidades
                if (frequencia >= 1e12)
                {
                    AdicionarSaida($"      f = {frequencia / 1e12:F2} THz (TeraHertz)\n", "#27AE60");
                }
                else if (frequencia >= 1e9)
                {
                    AdicionarSaida($"      f = {frequencia / 1e9:F2} GHz (GigaHertz)\n", "#27AE60");
                }
                else if (frequencia >= 1e6)
                {
                    AdicionarSaida($"      f = {frequencia / 1e6:F2} MHz (MegaHertz)\n", "#27AE60");
                }

                AdicionarSaida($"   ═══════════════════════════════════\n", "#7F8C8D");
            }
            catch (Exception ex)
            {
                AdicionarSaida($"❌ Erro ao calcular frequência: {ex.Message}\n", "#E74C3C");
            }
        }

        private void CalcularComprimentoOnda(string comando)
        {
            try
            {
                // Extrai frequência em Hz
                double frequencia = ExtrairValorParametro(comando, "frequencia:");

                if (frequencia <= 0)
                {
                    AdicionarSaida("❌ Erro: A frequência deve ser maior que zero!\n", "#E74C3C");
                    return;
                }

                // Calcula comprimento de onda: λ = c / f
                double comprimentoOndaM = VELOCIDADE_LUZ / frequencia;
                double comprimentoOndaNm = comprimentoOndaM * 1e9;

                AdicionarSaida($"📊 Calculando Comprimento de Onda:\n", "#8E44AD");
                AdicionarSaida($"   ═══════════════════════════════════\n", "#7F8C8D");
                AdicionarSaida($"   📐 Fórmula: λ = c / f\n", "#7D3C98");
                AdicionarSaida($"\n", "#7F8C8D");
                AdicionarSaida($"   📊 Dados:\n", "#7F8C8D");
                AdicionarSaida($"      • f (frequência) = {frequencia:E2} Hz\n", "#7F8C8D");
                AdicionarSaida($"      • c (velocidade da luz) = {VELOCIDADE_LUZ:E2} m/s\n", "#7F8C8D");
                AdicionarSaida($"\n", "#7F8C8D");
                AdicionarSaida($"   🔢 Cálculo:\n", "#7F8C8D");
                AdicionarSaida($"      λ = {VELOCIDADE_LUZ:E2} / {frequencia:E2}\n", "#7F8C8D");
                AdicionarSaida($"\n", "#7F8C8D");
                AdicionarSaida($"   ✅ Resultado:\n", "#27AE60");
                AdicionarSaida($"      λ = {comprimentoOndaM:E3} m\n", "#27AE60");

                // Adiciona em nanômetros se apropriado
                if (comprimentoOndaM < 1e-3)
                {
                    AdicionarSaida($"      λ = {comprimentoOndaNm:F2} nm (nanômetros)\n", "#27AE60");
                }
                else if (comprimentoOndaM < 1)
                {
                    AdicionarSaida($"      λ = {comprimentoOndaM * 1e3:F2} mm (milímetros)\n", "#27AE60");
                }

                AdicionarSaida($"   ═══════════════════════════════════\n", "#7F8C8D");
            }
            catch (Exception ex)
            {
                AdicionarSaida($"❌ Erro ao calcular comprimento de onda: {ex.Message}\n", "#E74C3C");
            }
        }

        private void CalcularEnergia(string comando)
        {
            try
            {
                // Extrai frequência em Hz
                double frequencia = ExtrairValorParametro(comando, "frequencia:");

                if (frequencia <= 0)
                {
                    AdicionarSaida("❌ Erro: A frequência deve ser maior que zero!\n", "#E74C3C");
                    return;
                }

                // Calcula energia: E = h × f
                double energiaJoules = CONSTANTE_PLANCK * frequencia;
                double energiaEletronVolts = energiaJoules / ELETRON_VOLT;

                AdicionarSaida($"⚡ Calculando Energia do Fóton:\n", "#8E44AD");
                AdicionarSaida($"   ═══════════════════════════════════\n", "#7F8C8D");
                AdicionarSaida($"   📐 Fórmula: E = h × f\n", "#7D3C98");
                AdicionarSaida($"   (Relação de Planck-Einstein)\n", "#7D3C98");
                AdicionarSaida($"\n", "#7F8C8D");
                AdicionarSaida($"   📊 Dados:\n", "#7F8C8D");
                AdicionarSaida($"      • f (frequência) = {frequencia:E2} Hz\n", "#7F8C8D");
                AdicionarSaida($"      • h (constante de Planck) = {CONSTANTE_PLANCK:E3} J·s\n", "#7F8C8D");
                AdicionarSaida($"\n", "#7F8C8D");
                AdicionarSaida($"   🔢 Cálculo:\n", "#7F8C8D");
                AdicionarSaida($"      E = {CONSTANTE_PLANCK:E3} × {frequencia:E2}\n", "#7F8C8D");
                AdicionarSaida($"\n", "#7F8C8D");
                AdicionarSaida($"   ✅ Resultado:\n", "#27AE60");
                AdicionarSaida($"      E = {energiaJoules:E3} J (Joules)\n", "#27AE60");
                AdicionarSaida($"      E = {energiaEletronVolts:E3} eV (elétron-volts)\n", "#27AE60");

                // Adiciona contexto
                if (energiaEletronVolts > 1e6)
                {
                    AdicionarSaida($"\n   💡 Energia muito alta - região dos raios X ou gama!\n", "#F39C12");
                }
                else if (energiaEletronVolts > 3)
                {
                    AdicionarSaida($"\n   💡 Energia suficiente para excitar elétrons (UV ou superior)!\n", "#F39C12");
                }
                else
                {
                    AdicionarSaida($"\n   💡 Baixa energia - região visível, infravermelha ou menor.\n", "#F39C12");
                }

                AdicionarSaida($"   ═══════════════════════════════════\n", "#7F8C8D");
            }
            catch (Exception ex)
            {
                AdicionarSaida($"❌ Erro ao calcular energia: {ex.Message}\n", "#E74C3C");
            }
        }

        private void IdentificarTipo(string comando)
        {
            try
            {
                // Extrai comprimento de onda em nanômetros
                double comprimentoOndaNm = ExtrairValorParametro(comando, "comprimentoOnda:");

                if (comprimentoOndaNm <= 0)
                {
                    AdicionarSaida("❌ Erro: O comprimento de onda deve ser maior que zero!\n", "#E74C3C");
                    return;
                }

                // Converte para metros para facilitar comparações
                double comprimentoOndaM = comprimentoOndaNm * 1e-9;

                AdicionarSaida($"🔍 Identificando Tipo de Onda Eletromagnética:\n", "#8E44AD");
                AdicionarSaida($"   ═══════════════════════════════════\n", "#7F8C8D");
                AdicionarSaida($"   📊 Comprimento de onda: {comprimentoOndaNm} nm\n", "#7F8C8D");
                AdicionarSaida($"\n", "#7F8C8D");

                string tipo = "";
                string emoji = "";
                string cor = "";
                string aplicacoes = "";

                // Identifica o tipo baseado no comprimento de onda
                if (comprimentoOndaM > 1)
                {
                    tipo = "Ondas de Rádio";
                    emoji = "📻";
                    cor = "#E74C3C";
                    aplicacoes = "Rádio AM/FM, TV, comunicações sem fio";
                }
                else if (comprimentoOndaM > 1e-3)
                {
                    tipo = "Micro-ondas";
                    emoji = "📡";
                    cor = "#F39C12";
                    aplicacoes = "Fornos micro-ondas, radar, comunicação satelital, Wi-Fi";
                }
                else if (comprimentoOndaNm > 700)
                {
                    tipo = "Infravermelho";
                    emoji = "🔥";
                    cor = "#E74C3C";
                    aplicacoes = "Sensores de calor, controles remotos, visão noturna";
                }
                else if (comprimentoOndaNm >= 620 && comprimentoOndaNm <= 750)
                {
                    tipo = "Luz Visível - VERMELHA";
                    emoji = "🔴";
                    cor = "#E74C3C";
                    aplicacoes = "Visão humana, lasers vermelhos, sinalizações";
                }
                else if (comprimentoOndaNm >= 590 && comprimentoOndaNm < 620)
                {
                    tipo = "Luz Visível - LARANJA";
                    emoji = "🟠";
                    cor = "#F39C12";
                    aplicacoes = "Visão humana, iluminação decorativa";
                }
                else if (comprimentoOndaNm >= 570 && comprimentoOndaNm < 590)
                {
                    tipo = "Luz Visível - AMARELA";
                    emoji = "🟡";
                    cor = "#F4D03F";
                    aplicacoes = "Visão humana, iluminação pública (sódio)";
                }
                else if (comprimentoOndaNm >= 495 && comprimentoOndaNm < 570)
                {
                    tipo = "Luz Visível - VERDE";
                    emoji = "🟢";
                    cor = "#27AE60";
                    aplicacoes = "Visão humana (máxima sensibilidade), lasers verdes";
                }
                else if (comprimentoOndaNm >= 450 && comprimentoOndaNm < 495)
                {
                    tipo = "Luz Visível - AZUL";
                    emoji = "🔵";
                    cor = "#3498DB";
                    aplicacoes = "Visão humana, LEDs azuis, Blu-ray";
                }
                else if (comprimentoOndaNm >= 400 && comprimentoOndaNm < 450)
                {
                    tipo = "Luz Visível - VIOLETA";
                    emoji = "🟣";
                    cor = "#8E44AD";
                    aplicacoes = "Visão humana, lasers violeta";
                }
                else if (comprimentoOndaNm >= 10 && comprimentoOndaNm < 400)
                {
                    tipo = "Ultravioleta (UV)";
                    emoji = "🔮";
                    cor = "#8E44AD";
                    aplicacoes = "Esterilização, bronzeamento, luz negra, detecção de falsificações";
                }
                else if (comprimentoOndaNm >= 0.01 && comprimentoOndaNm < 10)
                {
                    tipo = "Raios X";
                    emoji = "⚕️";
                    cor = "#3498DB";
                    aplicacoes = "Radiografias médicas, inspeção de bagagens, cristalografia";
                }
                else if (comprimentoOndaNm < 0.01)
                {
                    tipo = "Raios Gama";
                    emoji = "☢️";
                    cor = "#27AE60";
                    aplicacoes = "Radioterapia contra câncer, esterilização, astronomia";
                }

                AdicionarSaida($"   {emoji} TIPO IDENTIFICADO:\n", cor);
                AdicionarSaida($"      • {tipo}\n", cor);
                AdicionarSaida($"\n", "#7F8C8D");
                AdicionarSaida($"   📱 Aplicações:\n", "#7F8C8D");
                AdicionarSaida($"      {aplicacoes}\n", "#7F8C8D");

                // Adiciona informação sobre visibilidade
                if (comprimentoOndaNm >= 400 && comprimentoOndaNm <= 700)
                {
                    AdicionarSaida($"\n   👁️ Esta onda é VISÍVEL ao olho humano!\n", "#27AE60");
                }
                else
                {
                    AdicionarSaida($"\n   👁️ Esta onda é INVISÍVEL ao olho humano.\n", "#F39C12");
                }

                AdicionarSaida($"   ═══════════════════════════════════\n", "#7F8C8D");
            }
            catch (Exception ex)
            {
                AdicionarSaida($"❌ Erro ao identificar tipo: {ex.Message}\n", "#E74C3C");
            }
        }

        // Métodos auxiliares
        private string ExtrairTextoEntreAspas(string texto)
        {
            try
            {
                int inicio = texto.IndexOf("\"");
                int fim = texto.LastIndexOf("\"");

                if (inicio >= 0 && fim > inicio)
                {
                    return texto.Substring(inicio + 1, fim - inicio - 1);
                }

                // Tenta com aspas simples
                inicio = texto.IndexOf("'");
                fim = texto.LastIndexOf("'");

                if (inicio >= 0 && fim > inicio)
                {
                    return texto.Substring(inicio + 1, fim - inicio - 1);
                }

                return "";
            }
            catch
            {
                return "";
            }
        }

        private double ExtrairValorParametro(string comando, string parametro)
        {
            try
            {
                int inicioParam = comando.IndexOf(parametro);
                if (inicioParam < 0)
                {
                    throw new Exception($"Parâmetro '{parametro}' não encontrado");
                }

                inicioParam += parametro.Length;
                string resto = comando.Substring(inicioParam).Trim();

                // Encontra o fim do número
                int fimNumero = resto.Length;
                for (int i = 0; i < resto.Length; i++)
                {
                    char c = resto[i];
                    if (c == ',' || c == ')' || (c == ' ' && i > 0 && !char.IsDigit(resto[i - 1])))
                    {
                        fimNumero = i;
                        break;
                    }
                }

                string valorStr = resto.Substring(0, fimNumero).Trim();

                // Suporta notação científica (e ou E)
                valorStr = valorStr.Replace(',', '.');

                if (double.TryParse(valorStr, System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out double valor))
                {
                    return valor;
                }

                throw new Exception($"Valor inválido para '{parametro}': {valorStr}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao extrair parâmetro '{parametro}': {ex.Message}");
            }
        }

        private void AdicionarSaida(string mensagem, string cor)
        {
            // Adiciona texto ao buffer de saída
            outputBuilder.Append(mensagem);
        }

        private void ClearCode_Click(object sender, RoutedEventArgs e)
        {
            // Limpa o editor de código
            CodeEditor.Text = "// Digite seu código aqui\nescreva(\"Olá, mundo eletromagnético!\")\n";

            // Limpa o console
            outputBuilder.Clear();
            ConsoleOutput.Text = "> Calculadora de Ondas EM iniciada.\n" +
                                "> Digite seu código e clique em EXECUTAR.\n" +
                                "> Explore o espectro eletromagnético!";
        }

        private void ExercisesButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "📝 Exercícios Práticos - Em Desenvolvimento!\n\n" +
                "Em breve você terá acesso a:\n" +
                "• Cálculos com ondas eletromagnéticas\n" +
                "• Problemas sobre as Equações de Maxwell\n" +
                "• Desafios de identificação no espectro\n" +
                "• Sistema de pontuação e certificados",
                "Exercícios Práticos",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }

        private void QuizButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "🧪 Quiz sobre Ondas EM - Em Desenvolvimento!\n\n" +
                "Em breve você poderá testar seus conhecimentos com:\n" +
                "• Questões sobre o espectro eletromagnético\n" +
                "• Perguntas sobre as Equações de Maxwell\n" +
                "• Problemas de cálculo de frequência e energia\n" +
                "• Sistema de pontuação e ranking",
                "Quiz",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            // Limpeza de recursos
            outputBuilder?.Clear();
            base.OnClosed(e);
        }
    }
}