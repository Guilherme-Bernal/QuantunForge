using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace Quantun_Forge.src.views.ModuloEnsino.Fisica.FisicaClassica
{
    /// <summary>
    /// Lógica interna para MomentoAngularWindow.xaml
    /// </summary>
    public partial class MomentoAngularWindow : Window
    {
        // Constantes físicas
        private const double PI = Math.PI;

        // StringBuilder para saída do console
        private StringBuilder outputBuilder;

        // Dicionário com tipos de objetos e suas fórmulas de momento de inércia
        private Dictionary<string, Func<double, double, double>> momentosInercia;

        public MomentoAngularWindow()
        {
            InitializeComponent();
            InicializarCalculadora();
        }

        private void InicializarCalculadora()
        {
            outputBuilder = new StringBuilder();

            // Inicializa fórmulas de momento de inércia
            momentosInercia = new Dictionary<string, Func<double, double, double>>
            {
                { "esfera", (m, r) => (2.0 / 5.0) * m * r * r },           // I = (2/5)MR²
                { "cilindro", (m, r) => (1.0 / 2.0) * m * r * r },         // I = (1/2)MR²
                { "disco", (m, r) => m * r * r },                          // I = MR²
                { "anel", (m, r) => m * r * r },                           // I = MR²
                { "barra", (m, l) => (1.0 / 12.0) * m * l * l }           // I = (1/12)ML²
            };
        }

        private void RunCode_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                outputBuilder.Clear();

                if (string.IsNullOrWhiteSpace(CodeEditor.Text))
                {
                    AdicionarSaida("⚠️ Por favor, digite algum código antes de executar.\n", "#F39C12");
                    ConsoleOutput.Text = outputBuilder.ToString();
                    return;
                }

                string codigo = CodeEditor.Text;

                AdicionarSaida("🔄 Iniciando Calculadora Rotacional...\n", "#E67E22");
                AdicionarSaida("═══════════════════════════════════════\n\n", "#7F8C8D");

                ProcessarCodigo(codigo);

                AdicionarSaida("\n═══════════════════════════════════════\n", "#7F8C8D");
                AdicionarSaida("✅ Cálculos concluídos com sucesso!\n", "#27AE60");

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
            var linhas = codigo.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var linha in linhas)
            {
                string linhaLimpa = linha.Trim();

                if (linhaLimpa.StartsWith("//") || string.IsNullOrWhiteSpace(linhaLimpa))
                    continue;

                int indexComentario = linhaLimpa.IndexOf("//");
                if (indexComentario >= 0)
                {
                    linhaLimpa = linhaLimpa.Substring(0, indexComentario).Trim();
                }

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
                if (comando.StartsWith("escreva("))
                {
                    string mensagem = ExtrairTextoEntreAspas(comando);
                    AdicionarSaida(mensagem + "\n", "#27AE60");
                }
                else if (comando.StartsWith("calcularTorque("))
                {
                    CalcularTorque(comando);
                }
                else if (comando.StartsWith("calcularMomentoInercia("))
                {
                    CalcularMomentoInercia(comando);
                }
                else if (comando.StartsWith("calcularMomentoAngular("))
                {
                    CalcularMomentoAngular(comando);
                }
                else if (comando.StartsWith("calcularEnergiaRotacional("))
                {
                    CalcularEnergiaRotacional(comando);
                }
                else if (comando.StartsWith("conservacaoMomento("))
                {
                    ConservacaoMomento(comando);
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

        private void CalcularTorque(string comando)
        {
            try
            {
                double forca = ExtrairValorParametro(comando, "forca:");
                double raio = ExtrairValorParametro(comando, "raio:");
                double angulo = ExtrairValorParametro(comando, "angulo:");

                if (forca <= 0 || raio <= 0)
                {
                    AdicionarSaida("❌ Erro: Força e raio devem ser positivos!\n", "#E74C3C");
                    return;
                }

                // Converte ângulo para radianos
                double anguloRad = angulo * PI / 180.0;
                double torque = raio * forca * Math.Sin(anguloRad);

                AdicionarSaida($"🔧 Calculando Torque:\n", "#E67E22");
                AdicionarSaida($"   ═══════════════════════════════════\n", "#7F8C8D");
                AdicionarSaida($"   📐 Fórmula: τ = r × F × sen(θ)\n", "#D35400");
                AdicionarSaida($"\n", "#7F8C8D");
                AdicionarSaida($"   📊 Dados:\n", "#7F8C8D");
                AdicionarSaida($"      • Força (F) = {forca} N\n", "#7F8C8D");
                AdicionarSaida($"      • Raio/Braço (r) = {raio} m\n", "#7F8C8D");
                AdicionarSaida($"      • Ângulo (θ) = {angulo}° = {anguloRad:F4} rad\n", "#7F8C8D");
                AdicionarSaida($"\n", "#7F8C8D");
                AdicionarSaida($"   🔢 Cálculo:\n", "#7F8C8D");
                AdicionarSaida($"      τ = {raio} × {forca} × sen({angulo}°)\n", "#7F8C8D");
                AdicionarSaida($"      τ = {raio} × {forca} × {Math.Sin(anguloRad):F4}\n", "#7F8C8D");
                AdicionarSaida($"\n", "#7F8C8D");
                AdicionarSaida($"   ✅ Resultado:\n", "#27AE60");
                AdicionarSaida($"      • Torque (τ) = {torque:F4} N·m\n", "#27AE60");
                AdicionarSaida($"   ═══════════════════════════════════\n", "#7F8C8D");

                if (Math.Abs(angulo - 90) < 1)
                {
                    AdicionarSaida($"   💡 Torque MÁXIMO! Força perpendicular ao raio.\n", "#F39C12");
                }
                else if (Math.Abs(angulo) < 1 || Math.Abs(angulo - 180) < 1)
                {
                    AdicionarSaida($"   ⚠️ Torque ZERO! Força paralela ao raio.\n", "#F39C12");
                }
            }
            catch (Exception ex)
            {
                AdicionarSaida($"❌ Erro ao calcular torque: {ex.Message}\n", "#E74C3C");
            }
        }

        private void CalcularMomentoInercia(string comando)
        {
            try
            {
                string tipo = ExtrairTextoEntreAspas(comando).ToLower();
                double massa = ExtrairValorParametro(comando, "massa:");
                double dimensao = 0;

                // Determina qual parâmetro usar baseado no tipo
                if (tipo == "barra")
                {
                    dimensao = ExtrairValorParametro(comando, "comprimento:");
                }
                else
                {
                    dimensao = ExtrairValorParametro(comando, "raio:");
                }

                if (massa <= 0 || dimensao <= 0)
                {
                    AdicionarSaida("❌ Erro: Massa e dimensão devem ser positivos!\n", "#E74C3C");
                    return;
                }

                if (!momentosInercia.ContainsKey(tipo))
                {
                    AdicionarSaida($"❌ Erro: Tipo '{tipo}' não reconhecido!\n", "#E74C3C");
                    AdicionarSaida($"   Tipos válidos: esfera, cilindro, disco, anel, barra\n", "#F39C12");
                    return;
                }

                double I = momentosInercia[tipo](massa, dimensao);

                AdicionarSaida($"📊 Calculando Momento de Inércia:\n", "#E67E22");
                AdicionarSaida($"   ═══════════════════════════════════\n", "#7F8C8D");
                AdicionarSaida($"   🔍 Tipo de objeto: {tipo.ToUpper()}\n", "#D35400");
                AdicionarSaida($"\n", "#7F8C8D");
                AdicionarSaida($"   📊 Dados:\n", "#7F8C8D");
                AdicionarSaida($"      • Massa (M) = {massa} kg\n", "#7F8C8D");

                if (tipo == "barra")
                {
                    AdicionarSaida($"      • Comprimento (L) = {dimensao} m\n", "#7F8C8D");
                    AdicionarSaida($"\n", "#7F8C8D");
                    AdicionarSaida($"   📐 Fórmula: I = (1/12)ML²\n", "#D35400");
                }
                else
                {
                    AdicionarSaida($"      • Raio (R) = {dimensao} m\n", "#7F8C8D");
                    AdicionarSaida($"\n", "#7F8C8D");

                    string formula = tipo switch
                    {
                        "esfera" => "I = (2/5)MR²",
                        "cilindro" => "I = (1/2)MR²",
                        "disco" => "I = MR²",
                        "anel" => "I = MR²",
                        _ => "I = ?"
                    };

                    AdicionarSaida($"   📐 Fórmula: {formula}\n", "#D35400");
                }

                AdicionarSaida($"\n", "#7F8C8D");
                AdicionarSaida($"   ✅ Resultado:\n", "#27AE60");
                AdicionarSaida($"      • Momento de Inércia (I) = {I:F4} kg·m²\n", "#27AE60");
                AdicionarSaida($"   ═══════════════════════════════════\n", "#7F8C8D");

                AdicionarSaida($"   💡 Quanto maior I, mais difícil girar o objeto!\n", "#F39C12");
            }
            catch (Exception ex)
            {
                AdicionarSaida($"❌ Erro ao calcular momento de inércia: {ex.Message}\n", "#E74C3C");
            }
        }

        private void CalcularMomentoAngular(string comando)
        {
            try
            {
                double I = ExtrairValorParametro(comando, "I:");
                double omega = ExtrairValorParametro(comando, "omega:");

                if (I <= 0)
                {
                    AdicionarSaida("❌ Erro: Momento de inércia deve ser positivo!\n", "#E74C3C");
                    return;
                }

                double L = I * omega;

                AdicionarSaida($"🌀 Calculando Momento Angular:\n", "#E67E22");
                AdicionarSaida($"   ═══════════════════════════════════\n", "#7F8C8D");
                AdicionarSaida($"   📐 Fórmula: L = I × ω\n", "#D35400");
                AdicionarSaida($"\n", "#7F8C8D");
                AdicionarSaida($"   📊 Dados:\n", "#7F8C8D");
                AdicionarSaida($"      • Momento de Inércia (I) = {I} kg·m²\n", "#7F8C8D");
                AdicionarSaida($"      • Velocidade Angular (ω) = {omega} rad/s\n", "#7F8C8D");
                AdicionarSaida($"\n", "#7F8C8D");
                AdicionarSaida($"   🔢 Cálculo:\n", "#7F8C8D");
                AdicionarSaida($"      L = {I} × {omega}\n", "#7F8C8D");
                AdicionarSaida($"\n", "#7F8C8D");
                AdicionarSaida($"   ✅ Resultado:\n", "#27AE60");
                AdicionarSaida($"      • Momento Angular (L) = {L:F4} kg·m²/s\n", "#27AE60");
                AdicionarSaida($"   ═══════════════════════════════════\n", "#7F8C8D");

                AdicionarSaida($"   💡 Compare com momento linear: p = m × v\n", "#F39C12");
            }
            catch (Exception ex)
            {
                AdicionarSaida($"❌ Erro ao calcular momento angular: {ex.Message}\n", "#E74C3C");
            }
        }

        private void CalcularEnergiaRotacional(string comando)
        {
            try
            {
                double I = ExtrairValorParametro(comando, "I:");
                double omega = ExtrairValorParametro(comando, "omega:");

                if (I <= 0)
                {
                    AdicionarSaida("❌ Erro: Momento de inércia deve ser positivo!\n", "#E74C3C");
                    return;
                }

                double Ec = 0.5 * I * omega * omega;

                AdicionarSaida($"⚡ Calculando Energia Cinética Rotacional:\n", "#E67E22");
                AdicionarSaida($"   ═══════════════════════════════════\n", "#7F8C8D");
                AdicionarSaida($"   📐 Fórmula: Ec = (1/2)Iω²\n", "#D35400");
                AdicionarSaida($"\n", "#7F8C8D");
                AdicionarSaida($"   📊 Dados:\n", "#7F8C8D");
                AdicionarSaida($"      • Momento de Inércia (I) = {I} kg·m²\n", "#7F8C8D");
                AdicionarSaida($"      • Velocidade Angular (ω) = {omega} rad/s\n", "#7F8C8D");
                AdicionarSaida($"\n", "#7F8C8D");
                AdicionarSaida($"   🔢 Cálculo:\n", "#7F8C8D");
                AdicionarSaida($"      Ec = 0.5 × {I} × ({omega})²\n", "#7F8C8D");
                AdicionarSaida($"      Ec = 0.5 × {I} × {omega * omega:F4}\n", "#7F8C8D");
                AdicionarSaida($"\n", "#7F8C8D");
                AdicionarSaida($"   ✅ Resultado:\n", "#27AE60");
                AdicionarSaida($"      • Energia Cinética (Ec) = {Ec:F4} J (Joules)\n", "#27AE60");
                AdicionarSaida($"   ═══════════════════════════════════\n", "#7F8C8D");

                AdicionarSaida($"   💡 Compare com Ec linear: Ec = (1/2)mv²\n", "#F39C12");
            }
            catch (Exception ex)
            {
                AdicionarSaida($"❌ Erro ao calcular energia rotacional: {ex.Message}\n", "#E74C3C");
            }
        }

        private void ConservacaoMomento(string comando)
        {
            try
            {
                double I1 = ExtrairValorParametro(comando, "I1:");
                double omega1 = ExtrairValorParametro(comando, "omega1:");
                double I2 = ExtrairValorParametro(comando, "I2:");

                if (I1 <= 0 || I2 <= 0)
                {
                    AdicionarSaida("❌ Erro: Momentos de inércia devem ser positivos!\n", "#E74C3C");
                    return;
                }

                // Conservação: L1 = L2 → I1*ω1 = I2*ω2
                double omega2 = (I1 * omega1) / I2;
                double L = I1 * omega1;

                AdicionarSaida($"⭐ CONSERVAÇÃO DO MOMENTO ANGULAR:\n", "#E74C3C");
                AdicionarSaida($"   ═══════════════════════════════════\n", "#7F8C8D");
                AdicionarSaida($"   📐 Lei: I₁ω₁ = I₂ω₂ (sem torque externo)\n", "#C0392B");
                AdicionarSaida($"\n", "#7F8C8D");
                AdicionarSaida($"   📊 Estado INICIAL:\n", "#7F8C8D");
                AdicionarSaida($"      • I₁ = {I1} kg·m²\n", "#7F8C8D");
                AdicionarSaida($"      • ω₁ = {omega1} rad/s\n", "#7F8C8D");
                AdicionarSaida($"      • L₁ = {I1 * omega1:F4} kg·m²/s\n", "#7F8C8D");
                AdicionarSaida($"\n", "#7F8C8D");
                AdicionarSaida($"   📊 Estado FINAL:\n", "#7F8C8D");
                AdicionarSaida($"      • I₂ = {I2} kg·m²\n", "#7F8C8D");
                AdicionarSaida($"      • ω₂ = ?\n", "#7F8C8D");
                AdicionarSaida($"\n", "#7F8C8D");
                AdicionarSaida($"   🔢 Cálculo:\n", "#7F8C8D");
                AdicionarSaida($"      I₁ω₁ = I₂ω₂\n", "#7F8C8D");
                AdicionarSaida($"      {I1} × {omega1} = {I2} × ω₂\n", "#7F8C8D");
                AdicionarSaida($"      ω₂ = ({I1} × {omega1}) / {I2}\n", "#7F8C8D");
                AdicionarSaida($"\n", "#7F8C8D");
                AdicionarSaida($"   ✅ Resultado:\n", "#27AE60");
                AdicionarSaida($"      • ω₂ = {omega2:F4} rad/s\n", "#27AE60");
                AdicionarSaida($"      • L₂ = {I2 * omega2:F4} kg·m²/s\n", "#27AE60");
                AdicionarSaida($"   ═══════════════════════════════════\n", "#7F8C8D");

                if (I2 < I1)
                {
                    double percentual = ((omega2 - omega1) / omega1) * 100;
                    AdicionarSaida($"   💡 Momento de inércia DIMINUIU → velocidade AUMENTOU {Math.Abs(percentual):F1}%!\n", "#F39C12");
                    AdicionarSaida($"   ⛸️ Como um patinador encolhendo os braços!\n", "#F39C12");
                }
                else if (I2 > I1)
                {
                    double percentual = ((omega1 - omega2) / omega1) * 100;
                    AdicionarSaida($"   💡 Momento de inércia AUMENTOU → velocidade DIMINUIU {Math.Abs(percentual):F1}%!\n", "#F39C12");
                    AdicionarSaida($"   ⛸️ Como um patinador abrindo os braços!\n", "#F39C12");
                }

                AdicionarSaida($"   ⭐ Momento angular CONSERVADO: L = {L:F4} kg·m²/s\n", "#27AE60");
            }
            catch (Exception ex)
            {
                AdicionarSaida($"❌ Erro ao calcular conservação: {ex.Message}\n", "#E74C3C");
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

                int fimNumero = resto.Length;
                for (int i = 0; i < resto.Length; i++)
                {
                    char c = resto[i];
                    if (c == ',' || c == ')' || (c == ' ' && i > 0 && !char.IsDigit(resto[i - 1]) && resto[i - 1] != '.'))
                    {
                        fimNumero = i;
                        break;
                    }
                }

                string valorStr = resto.Substring(0, fimNumero).Trim();
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
            outputBuilder.Append(mensagem);
        }

        private void ClearCode_Click(object sender, RoutedEventArgs e)
        {
            CodeEditor.Text = "// Digite seu código aqui\nescreva(\"Bem-vindo à rotação!\")\n";

            outputBuilder.Clear();
            ConsoleOutput.Text = "> Calculadora Rotacional iniciada.\n" +
                                "> Digite seu código e clique em EXECUTAR.\n" +
                                "> Explore o momento angular!";
        }

        private void ExercisesButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "📝 Exercícios Práticos - Em Desenvolvimento!\n\n" +
                "Em breve você terá acesso a:\n" +
                "• Problemas sobre torque e momento angular\n" +
                "• Desafios de conservação do momento\n" +
                "• Cálculos de energia rotacional\n" +
                "• Sistema de pontuação e certificados",
                "Exercícios Práticos",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }

        private void QuizButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "🧪 Quiz sobre Rotação - Em Desenvolvimento!\n\n" +
                "Em breve você poderá testar seus conhecimentos com:\n" +
                "• Questões sobre dinâmica rotacional\n" +
                "• Perguntas sobre momento angular\n" +
                "• Problemas de conservação\n" +
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
            outputBuilder?.Clear();
            momentosInercia?.Clear();
            base.OnClosed(e);
        }
    }
}