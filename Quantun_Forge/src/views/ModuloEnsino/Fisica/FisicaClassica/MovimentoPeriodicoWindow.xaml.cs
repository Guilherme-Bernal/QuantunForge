using System;
using System.Text;
using System.Windows;

namespace Quantun_Forge.src.views.ModuloEnsino.Fisica.FisicaClassica
{
    /// <summary>
    /// Lógica interna para MovimentoPeriodicoWindow.xaml
    /// </summary>
    public partial class MovimentoPeriodicoWindow : Window
    {
        // Constantes físicas
        private const double PI = Math.PI;
        private const double GRAVIDADE = 9.8; // m/s²

        // StringBuilder para saída do console
        private StringBuilder outputBuilder;

        public MovimentoPeriodicoWindow()
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
                AdicionarSaida("🔄 Iniciando Calculadora de Movimento Harmônico Simples...\n", "#16A085");
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
                // Comando: calcularPeriodoMola(massa: X, k: Y)
                else if (comando.StartsWith("calcularPeriodoMola("))
                {
                    CalcularPeriodoMola(comando);
                }
                // Comando: calcularPeriodoPendulo(comprimento: X)
                else if (comando.StartsWith("calcularPeriodoPendulo("))
                {
                    CalcularPeriodoPendulo(comando);
                }
                // Comando: calcularFrequencia(periodo: X)
                else if (comando.StartsWith("calcularFrequencia("))
                {
                    CalcularFrequencia(comando);
                }
                // Comando: calcularEnergiaTotal(k: X, amplitude: Y)
                else if (comando.StartsWith("calcularEnergiaTotal("))
                {
                    CalcularEnergiaTotal(comando);
                }
                // Comando: calcularVelocidadeMaxima(amplitude: X, frequencia: Y)
                else if (comando.StartsWith("calcularVelocidadeMaxima("))
                {
                    CalcularVelocidadeMaxima(comando);
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

        private void CalcularPeriodoMola(string comando)
        {
            try
            {
                // Extrai massa e constante da mola
                double massa = ExtrairValorParametro(comando, "massa:");
                double k = ExtrairValorParametro(comando, "k:");

                if (massa <= 0)
                {
                    AdicionarSaida("❌ Erro: A massa deve ser maior que zero!\n", "#E74C3C");
                    return;
                }

                if (k <= 0)
                {
                    AdicionarSaida("❌ Erro: A constante da mola deve ser maior que zero!\n", "#E74C3C");
                    return;
                }

                // Calcula período: T = 2π√(m/k)
                double periodo = 2 * PI * Math.Sqrt(massa / k);
                double frequencia = 1 / periodo;
                double frequenciaAngular = 2 * PI * frequencia;

                AdicionarSaida($"🔧 Calculando Período - Sistema Massa-Mola:\n", "#16A085");
                AdicionarSaida($"   ═══════════════════════════════════\n", "#7F8C8D");
                AdicionarSaida($"   📐 Fórmula: T = 2π√(m/k)\n", "#138D75");
                AdicionarSaida($"\n", "#7F8C8D");
                AdicionarSaida($"   📊 Dados do Sistema:\n", "#7F8C8D");
                AdicionarSaida($"      • Massa (m) = {massa} kg\n", "#7F8C8D");
                AdicionarSaida($"      • Constante da mola (k) = {k} N/m\n", "#7F8C8D");
                AdicionarSaida($"\n", "#7F8C8D");
                AdicionarSaida($"   🔢 Cálculo:\n", "#7F8C8D");
                AdicionarSaida($"      T = 2π × √({massa}/{k})\n", "#7F8C8D");
                AdicionarSaida($"      T = 2π × √{massa / k:F4}\n", "#7F8C8D");
                AdicionarSaida($"      T = 2π × {Math.Sqrt(massa / k):F4}\n", "#7F8C8D");
                AdicionarSaida($"\n", "#7F8C8D");
                AdicionarSaida($"   ✅ Resultados:\n", "#27AE60");
                AdicionarSaida($"      • Período (T) = {periodo:F4} segundos\n", "#27AE60");
                AdicionarSaida($"      • Frequência (f) = {frequencia:F4} Hz\n", "#27AE60");
                AdicionarSaida($"      • Frequência Angular (ω) = {frequenciaAngular:F4} rad/s\n", "#27AE60");
                AdicionarSaida($"   ═══════════════════════════════════\n", "#7F8C8D");

                // Adiciona interpretação
                if (periodo < 1)
                {
                    AdicionarSaida($"   💡 Sistema oscila rapidamente (período curto)!\n", "#F39C12");
                }
                else if (periodo > 3)
                {
                    AdicionarSaida($"   💡 Sistema oscila lentamente (período longo)!\n", "#F39C12");
                }
                else
                {
                    AdicionarSaida($"   💡 Sistema com oscilação moderada.\n", "#F39C12");
                }
            }
            catch (Exception ex)
            {
                AdicionarSaida($"❌ Erro ao calcular período da mola: {ex.Message}\n", "#E74C3C");
            }
        }

        private void CalcularPeriodoPendulo(string comando)
        {
            try
            {
                // Extrai comprimento do pêndulo
                double comprimento = ExtrairValorParametro(comando, "comprimento:");

                if (comprimento <= 0)
                {
                    AdicionarSaida("❌ Erro: O comprimento deve ser maior que zero!\n", "#E74C3C");
                    return;
                }

                // Calcula período: T = 2π√(L/g)
                double periodo = 2 * PI * Math.Sqrt(comprimento / GRAVIDADE);
                double frequencia = 1 / periodo;

                AdicionarSaida($"🕰️ Calculando Período - Pêndulo Simples:\n", "#16A085");
                AdicionarSaida($"   ═══════════════════════════════════\n", "#7F8C8D");
                AdicionarSaida($"   📐 Fórmula: T = 2π√(L/g)\n", "#138D75");
                AdicionarSaida($"\n", "#7F8C8D");
                AdicionarSaida($"   📊 Dados do Sistema:\n", "#7F8C8D");
                AdicionarSaida($"      • Comprimento (L) = {comprimento} m\n", "#7F8C8D");
                AdicionarSaida($"      • Gravidade (g) = {GRAVIDADE} m/s²\n", "#7F8C8D");
                AdicionarSaida($"\n", "#7F8C8D");
                AdicionarSaida($"   🔢 Cálculo:\n", "#7F8C8D");
                AdicionarSaida($"      T = 2π × √({comprimento}/{GRAVIDADE})\n", "#7F8C8D");
                AdicionarSaida($"      T = 2π × √{comprimento / GRAVIDADE:F4}\n", "#7F8C8D");
                AdicionarSaida($"      T = 2π × {Math.Sqrt(comprimento / GRAVIDADE):F4}\n", "#7F8C8D");
                AdicionarSaida($"\n", "#7F8C8D");
                AdicionarSaida($"   ✅ Resultados:\n", "#27AE60");
                AdicionarSaida($"      • Período (T) = {periodo:F4} segundos\n", "#27AE60");
                AdicionarSaida($"      • Frequência (f) = {frequencia:F4} Hz\n", "#27AE60");
                AdicionarSaida($"   ═══════════════════════════════════\n", "#7F8C8D");

                // Adiciona interpretação
                AdicionarSaida($"   💡 O período NÃO depende da massa do pêndulo!\n", "#F39C12");

                if (comprimento >= 1)
                {
                    AdicionarSaida($"   💡 Pêndulo longo - oscila devagar.\n", "#F39C12");
                }
                else
                {
                    AdicionarSaida($"   💡 Pêndulo curto - oscila rapidamente.\n", "#F39C12");
                }

                AdicionarSaida($"   ⚠️ Válido apenas para pequenas amplitudes (&lt; 15°).\n", "#F39C12");
            }
            catch (Exception ex)
            {
                AdicionarSaida($"❌ Erro ao calcular período do pêndulo: {ex.Message}\n", "#E74C3C");
            }
        }

        private void CalcularFrequencia(string comando)
        {
            try
            {
                // Extrai período
                double periodo = ExtrairValorParametro(comando, "periodo:");

                if (periodo <= 0)
                {
                    AdicionarSaida("❌ Erro: O período deve ser maior que zero!\n", "#E74C3C");
                    return;
                }

                // Calcula frequência: f = 1/T
                double frequencia = 1 / periodo;
                double frequenciaAngular = 2 * PI * frequencia;

                AdicionarSaida($"📊 Calculando Frequência:\n", "#16A085");
                AdicionarSaida($"   ═══════════════════════════════════\n", "#7F8C8D");
                AdicionarSaida($"   📐 Fórmulas:\n", "#138D75");
                AdicionarSaida($"      f = 1/T\n", "#138D75");
                AdicionarSaida($"      ω = 2πf\n", "#138D75");
                AdicionarSaida($"\n", "#7F8C8D");
                AdicionarSaida($"   📊 Dados:\n", "#7F8C8D");
                AdicionarSaida($"      • Período (T) = {periodo} s\n", "#7F8C8D");
                AdicionarSaida($"\n", "#7F8C8D");
                AdicionarSaida($"   ✅ Resultados:\n", "#27AE60");
                AdicionarSaida($"      • Frequência (f) = {frequencia:F4} Hz\n", "#27AE60");
                AdicionarSaida($"      • Frequência Angular (ω) = {frequenciaAngular:F4} rad/s\n", "#27AE60");
                AdicionarSaida($"   ═══════════════════════════════════\n", "#7F8C8D");

                // Adiciona interpretação
                if (frequencia > 100)
                {
                    AdicionarSaida($"   💡 Frequência muito alta - vibração ultrassônica!\n", "#F39C12");
                }
                else if (frequencia > 20)
                {
                    AdicionarSaida($"   💡 Frequência audível - som agudo!\n", "#F39C12");
                }
                else if (frequencia >= 1)
                {
                    AdicionarSaida($"   💡 Oscilação visível - {frequencia:F1} ciclos por segundo.\n", "#F39C12");
                }
                else
                {
                    AdicionarSaida($"   💡 Oscilação lenta - período de {periodo:F1} segundos.\n", "#F39C12");
                }
            }
            catch (Exception ex)
            {
                AdicionarSaida($"❌ Erro ao calcular frequência: {ex.Message}\n", "#E74C3C");
            }
        }

        private void CalcularEnergiaTotal(string comando)
        {
            try
            {
                // Extrai constante da mola e amplitude
                double k = ExtrairValorParametro(comando, "k:");
                double amplitude = ExtrairValorParametro(comando, "amplitude:");

                if (k <= 0)
                {
                    AdicionarSaida("❌ Erro: A constante da mola deve ser maior que zero!\n", "#E74C3C");
                    return;
                }

                if (amplitude <= 0)
                {
                    AdicionarSaida("❌ Erro: A amplitude deve ser maior que zero!\n", "#E74C3C");
                    return;
                }

                // Calcula energia total: E = (1/2)kA²
                double energiaTotal = 0.5 * k * amplitude * amplitude;

                AdicionarSaida($"⚡ Calculando Energia Total do Oscilador:\n", "#16A085");
                AdicionarSaida($"   ═══════════════════════════════════\n", "#7F8C8D");
                AdicionarSaida($"   📐 Fórmula: E = (1/2)kA²\n", "#138D75");
                AdicionarSaida($"\n", "#7F8C8D");
                AdicionarSaida($"   📊 Dados:\n", "#7F8C8D");
                AdicionarSaida($"      • Constante da mola (k) = {k} N/m\n", "#7F8C8D");
                AdicionarSaida($"      • Amplitude (A) = {amplitude} m\n", "#7F8C8D");
                AdicionarSaida($"\n", "#7F8C8D");
                AdicionarSaida($"   🔢 Cálculo:\n", "#7F8C8D");
                AdicionarSaida($"      E = 0.5 × {k} × ({amplitude})²\n", "#7F8C8D");
                AdicionarSaida($"      E = 0.5 × {k} × {amplitude * amplitude:F4}\n", "#7F8C8D");
                AdicionarSaida($"\n", "#7F8C8D");
                AdicionarSaida($"   ✅ Resultado:\n", "#27AE60");
                AdicionarSaida($"      • Energia Total (E) = {energiaTotal:F4} Joules\n", "#27AE60");
                AdicionarSaida($"   ═══════════════════════════════════\n", "#7F8C8D");

                // Adiciona informações sobre energia
                AdicionarSaida($"   💡 Esta energia é CONSTANTE durante toda a oscilação!\n", "#F39C12");
                AdicionarSaida($"   💡 Nos extremos: toda energia é potencial (Ep = E).\n", "#F39C12");
                AdicionarSaida($"   💡 No centro: toda energia é cinética (Ec = E).\n", "#F39C12");
            }
            catch (Exception ex)
            {
                AdicionarSaida($"❌ Erro ao calcular energia total: {ex.Message}\n", "#E74C3C");
            }
        }

        private void CalcularVelocidadeMaxima(string comando)
        {
            try
            {
                // Extrai amplitude e frequência
                double amplitude = ExtrairValorParametro(comando, "amplitude:");
                double frequencia = ExtrairValorParametro(comando, "frequencia:");

                if (amplitude <= 0)
                {
                    AdicionarSaida("❌ Erro: A amplitude deve ser maior que zero!\n", "#E74C3C");
                    return;
                }

                if (frequencia <= 0)
                {
                    AdicionarSaida("❌ Erro: A frequência deve ser maior que zero!\n", "#E74C3C");
                    return;
                }

                // Calcula velocidade máxima: v_max = Aω = A(2πf)
                double frequenciaAngular = 2 * PI * frequencia;
                double velocidadeMaxima = amplitude * frequenciaAngular;

                AdicionarSaida($"🚀 Calculando Velocidade Máxima:\n", "#16A085");
                AdicionarSaida($"   ═══════════════════════════════════\n", "#7F8C8D");
                AdicionarSaida($"   📐 Fórmula: v_max = Aω = A(2πf)\n", "#138D75");
                AdicionarSaida($"\n", "#7F8C8D");
                AdicionarSaida($"   📊 Dados:\n", "#7F8C8D");
                AdicionarSaida($"      • Amplitude (A) = {amplitude} m\n", "#7F8C8D");
                AdicionarSaida($"      • Frequência (f) = {frequencia} Hz\n", "#7F8C8D");
                AdicionarSaida($"      • Frequência Angular (ω) = {frequenciaAngular:F4} rad/s\n", "#7F8C8D");
                AdicionarSaida($"\n", "#7F8C8D");
                AdicionarSaida($"   🔢 Cálculo:\n", "#7F8C8D");
                AdicionarSaida($"      ω = 2π × {frequencia} = {frequenciaAngular:F4} rad/s\n", "#7F8C8D");
                AdicionarSaida($"      v_max = {amplitude} × {frequenciaAngular:F4}\n", "#7F8C8D");
                AdicionarSaida($"\n", "#7F8C8D");
                AdicionarSaida($"   ✅ Resultado:\n", "#27AE60");
                AdicionarSaida($"      • Velocidade Máxima (v_max) = {velocidadeMaxima:F4} m/s\n", "#27AE60");
                AdicionarSaida($"   ═══════════════════════════════════\n", "#7F8C8D");

                // Adiciona interpretação
                AdicionarSaida($"   💡 A velocidade máxima ocorre quando x = 0 (centro da oscilação)!\n", "#F39C12");
                AdicionarSaida($"   💡 Nos extremos (x = ±A), a velocidade é zero.\n", "#F39C12");

                if (velocidadeMaxima > 10)
                {
                    AdicionarSaida($"   ⚠️ Velocidade muito alta! Cuidado com aproximações do MHS.\n", "#F39C12");
                }
            }
            catch (Exception ex)
            {
                AdicionarSaida($"❌ Erro ao calcular velocidade máxima: {ex.Message}\n", "#E74C3C");
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

                // Suporta notação científica e vírgula/ponto decimal
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
            CodeEditor.Text = "// Digite seu código aqui\nescreva(\"Bem-vindo ao MHS!\")\n";

            // Limpa o console
            outputBuilder.Clear();
            ConsoleOutput.Text = "> Calculadora de MHS iniciada.\n" +
                                "> Digite seu código e clique em EXECUTAR.\n" +
                                "> Explore o movimento periódico!";
        }

        private void ExercisesButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "📝 Exercícios Práticos - Em Desenvolvimento!\n\n" +
                "Em breve você terá acesso a:\n" +
                "• Problemas sobre sistemas massa-mola\n" +
                "• Desafios com pêndulos\n" +
                "• Cálculos de energia em osciladores\n" +
                "• Sistema de pontuação e certificados",
                "Exercícios Práticos",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }

        private void QuizButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "🧪 Quiz sobre MHS - Em Desenvolvimento!\n\n" +
                "Em breve você poderá testar seus conhecimentos com:\n" +
                "• Questões sobre movimento periódico\n" +
                "• Perguntas sobre oscilador harmônico\n" +
                "• Problemas de período e frequência\n" +
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