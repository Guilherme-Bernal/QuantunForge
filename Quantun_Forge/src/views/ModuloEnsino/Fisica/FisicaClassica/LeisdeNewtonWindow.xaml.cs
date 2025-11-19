using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Quantun_Forge.src.views.ModuloEnsino.Fisica.FisicaClassica
{
    /// <summary>
    /// Lógica interna para LeisdeNewtonWindow.xaml
    /// </summary>
    public partial class LeisdeNewtonWindow : Window
    {
        // Classe para representar um objeto físico
        private class ObjetoFisico
        {
            public double Massa { get; set; }
            public double Velocidade { get; set; }
            public double Aceleracao { get; set; }
            public double ForcaAplicada { get; set; }
            public string Nome { get; set; }

            public ObjetoFisico(double massa, double velocidade, string nome = "Objeto")
            {
                Massa = massa;
                Velocidade = velocidade;
                Aceleracao = 0;
                ForcaAplicada = 0;
                Nome = nome;
            }
        }

        // Dicionário para armazenar objetos criados
        private Dictionary<string, ObjetoFisico> objetos;
        private StringBuilder outputBuilder;

        public LeisdeNewtonWindow()
        {
            InitializeComponent();
            InicializarSimulador();
        }

        private void InicializarSimulador()
        {
            objetos = new Dictionary<string, ObjetoFisico>();
            outputBuilder = new StringBuilder();
        }

        private void RunCode_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Limpa o console e reinicia
                outputBuilder.Clear();
                objetos.Clear();

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
                AdicionarSaida("🚀 Iniciando simulação física...\n", "#3498DB");
                AdicionarSaida("═══════════════════════════════════════\n\n", "#7F8C8D");

                // Processa o código linha por linha
                ProcessarCodigo(codigo);

                // Adiciona mensagem final
                AdicionarSaida("\n═══════════════════════════════════════\n", "#7F8C8D");
                AdicionarSaida("✅ Simulação concluída com sucesso!\n", "#27AE60");

                // Exibe resultado no console
                ConsoleOutput.Text = outputBuilder.ToString();
            }
            catch (Exception ex)
            {
                AdicionarSaida($"\n❌ ERRO FATAL: {ex.Message}\n", "#E74C3C");
                AdicionarSaida($"💡 Verifique a sintaxe do seu código e tente novamente.\n", "#F39C12");
                ConsoleOutput.Text = outputBuilder.ToString();
            }
        }

        private void ProcessarCodigo(string codigo)
        {
            // Remove comentários de linha única e divide em linhas
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
                // Comando: var nome = objeto(massa: X, velocidade: Y)
                else if (comando.Contains("objeto("))
                {
                    CriarObjeto(comando);
                }
                // Comando: aplicarForca(obj, forca: X)
                else if (comando.StartsWith("aplicarForca("))
                {
                    AplicarForca(comando);
                }
                // Comando: calcularAceleracao(obj)
                else if (comando.StartsWith("calcularAceleracao("))
                {
                    CalcularAceleracao(comando);
                }
                // Comando: simularColisao(obj1, obj2)
                else if (comando.StartsWith("simularColisao("))
                {
                    SimularColisao(comando);
                }
                // Comando: simularInercia(obj, tempo: X)
                else if (comando.StartsWith("simularInercia("))
                {
                    SimularInercia(comando);
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

        private void CriarObjeto(string comando)
        {
            try
            {
                // Extrai o nome da variável
                if (!comando.Contains("var "))
                {
                    AdicionarSaida("❌ Erro: Use 'var nome = objeto(...)' para criar objetos\n", "#E74C3C");
                    return;
                }

                string nomeVar = comando.Substring(comando.IndexOf("var ") + 4,
                    comando.IndexOf("=") - comando.IndexOf("var ") - 4).Trim();

                // Validação do nome da variável
                if (string.IsNullOrWhiteSpace(nomeVar))
                {
                    AdicionarSaida("❌ Erro: Nome da variável inválido\n", "#E74C3C");
                    return;
                }

                // Extrai massa
                double massa = ExtrairValorParametro(comando, "massa:");

                // Extrai velocidade
                double velocidade = ExtrairValorParametro(comando, "velocidade:");

                // Validações
                if (massa <= 0)
                {
                    AdicionarSaida("❌ Erro: A massa deve ser maior que zero!\n", "#E74C3C");
                    return;
                }

                // Verifica se já existe
                if (objetos.ContainsKey(nomeVar))
                {
                    AdicionarSaida($"⚠️ Aviso: Objeto '{nomeVar}' já existe. Será substituído.\n", "#F39C12");
                }

                // Cria objeto
                ObjetoFisico obj = new ObjetoFisico(massa, velocidade, nomeVar);
                objetos[nomeVar] = obj;

                AdicionarSaida($"✅ Objeto '{nomeVar}' criado com sucesso!\n", "#16A085");
                AdicionarSaida($"   📊 Massa: {massa} kg\n", "#7F8C8D");
                AdicionarSaida($"   📊 Velocidade inicial: {velocidade} m/s\n", "#7F8C8D");
            }
            catch (Exception ex)
            {
                AdicionarSaida($"❌ Erro ao criar objeto: {ex.Message}\n", "#E74C3C");
            }
        }

        private void AplicarForca(string comando)
        {
            try
            {
                // Extrai nome do objeto
                int inicio = comando.IndexOf("(") + 1;
                int fim = comando.IndexOf(",");

                if (fim <= inicio)
                {
                    AdicionarSaida("❌ Erro: Sintaxe incorreta. Use: aplicarForca(objeto, forca: valor)\n", "#E74C3C");
                    return;
                }

                string nomeObj = comando.Substring(inicio, fim - inicio).Trim();

                // Extrai força
                double forca = ExtrairValorParametro(comando, "forca:");

                if (objetos.ContainsKey(nomeObj))
                {
                    objetos[nomeObj].ForcaAplicada = forca;
                    AdicionarSaida($"⚡ Força aplicada ao objeto '{nomeObj}':\n", "#3498DB");
                    AdicionarSaida($"   📊 Intensidade: {Math.Abs(forca)} N\n", "#7F8C8D");
                    AdicionarSaida($"   📊 Direção: {(forca >= 0 ? "Positiva (+)" : "Negativa (-)")}\n", "#7F8C8D");

                    if (forca == 0)
                    {
                        AdicionarSaida($"   💡 Força resultante zero → movimento uniforme (1ª Lei)\n", "#F39C12");
                    }
                }
                else
                {
                    AdicionarSaida($"❌ Objeto '{nomeObj}' não encontrado!\n", "#E74C3C");
                    AdicionarSaida($"💡 Objetos disponíveis: {string.Join(", ", objetos.Keys)}\n", "#F39C12");
                }
            }
            catch (Exception ex)
            {
                AdicionarSaida($"❌ Erro ao aplicar força: {ex.Message}\n", "#E74C3C");
            }
        }

        private void CalcularAceleracao(string comando)
        {
            try
            {
                // Extrai nome do objeto
                int inicio = comando.IndexOf("(") + 1;
                int fim = comando.IndexOf(")");

                if (fim <= inicio)
                {
                    AdicionarSaida("❌ Erro: Sintaxe incorreta. Use: calcularAceleracao(objeto)\n", "#E74C3C");
                    return;
                }

                string nomeObj = comando.Substring(inicio, fim - inicio).Trim();

                if (objetos.ContainsKey(nomeObj))
                {
                    ObjetoFisico obj = objetos[nomeObj];

                    // Calcula aceleração: a = F / m (Segunda Lei de Newton)
                    if (obj.Massa > 0)
                    {
                        obj.Aceleracao = obj.ForcaAplicada / obj.Massa;

                        AdicionarSaida($"🔢 Calculando aceleração - Objeto '{nomeObj}':\n", "#2980B9");
                        AdicionarSaida($"   ═══════════════════════════════════\n", "#7F8C8D");
                        AdicionarSaida($"   📐 2ª Lei de Newton: F = m × a\n", "#3498DB");
                        AdicionarSaida($"   📐 Logo: a = F / m\n", "#3498DB");
                        AdicionarSaida($"   \n", "#7F8C8D");
                        AdicionarSaida($"   📊 Dados:\n", "#7F8C8D");
                        AdicionarSaida($"      • Força (F) = {obj.ForcaAplicada} N\n", "#7F8C8D");
                        AdicionarSaida($"      • Massa (m) = {obj.Massa} kg\n", "#7F8C8D");
                        AdicionarSaida($"   \n", "#7F8C8D");
                        AdicionarSaida($"   🔢 Cálculo:\n", "#7F8C8D");
                        AdicionarSaida($"      a = {obj.ForcaAplicada} N ÷ {obj.Massa} kg\n", "#7F8C8D");
                        AdicionarSaida($"   \n", "#7F8C8D");
                        AdicionarSaida($"   ✅ Resultado:\n", "#27AE60");
                        AdicionarSaida($"      a = {obj.Aceleracao:F2} m/s²\n", "#27AE60");
                        AdicionarSaida($"   ═══════════════════════════════════\n", "#7F8C8D");

                        // Mensagem explicativa baseada no valor
                        if (obj.Aceleracao > 0)
                        {
                            AdicionarSaida($"   💡 O objeto está acelerando para frente!\n", "#F39C12");
                        }
                        else if (obj.Aceleracao < 0)
                        {
                            AdicionarSaida($"   💡 O objeto está desacelerando (freando)!\n", "#F39C12");
                        }
                        else
                        {
                            AdicionarSaida($"   💡 Sem aceleração → Movimento Uniforme (1ª Lei)!\n", "#F39C12");
                        }
                    }
                    else
                    {
                        AdicionarSaida($"❌ Erro: Massa não pode ser zero ou negativa!\n", "#E74C3C");
                    }
                }
                else
                {
                    AdicionarSaida($"❌ Objeto '{nomeObj}' não encontrado!\n", "#E74C3C");
                    if (objetos.Count > 0)
                    {
                        AdicionarSaida($"💡 Objetos disponíveis: {string.Join(", ", objetos.Keys)}\n", "#F39C12");
                    }
                }
            }
            catch (Exception ex)
            {
                AdicionarSaida($"❌ Erro ao calcular aceleração: {ex.Message}\n", "#E74C3C");
            }
        }

        private void SimularColisao(string comando)
        {
            try
            {
                // Extrai nomes dos objetos
                string parametros = comando.Substring(comando.IndexOf("(") + 1,
                    comando.IndexOf(")") - comando.IndexOf("(") - 1);
                string[] objNomes = parametros.Split(',');

                if (objNomes.Length < 2)
                {
                    AdicionarSaida("❌ Erro: simularColisao precisa de 2 objetos!\n", "#E74C3C");
                    return;
                }

                string nomeObj1 = objNomes[0].Trim();
                string nomeObj2 = objNomes[1].Trim();

                if (objetos.ContainsKey(nomeObj1) && objetos.ContainsKey(nomeObj2))
                {
                    ObjetoFisico obj1 = objetos[nomeObj1];
                    ObjetoFisico obj2 = objetos[nomeObj2];

                    AdicionarSaida($"💥 SIMULAÇÃO DE COLISÃO\n", "#E74C3C");
                    AdicionarSaida($"═══════════════════════════════════════\n", "#7F8C8D");
                    AdicionarSaida($"   Objetos: '{nomeObj1}' ↔️ '{nomeObj2}'\n", "#7F8C8D");
                    AdicionarSaida($"\n", "#7F8C8D");
                    AdicionarSaida($"   📊 Dados ANTES da colisão:\n", "#7F8C8D");
                    AdicionarSaida($"      • {nomeObj1}:\n", "#7F8C8D");
                    AdicionarSaida($"         - Massa: {obj1.Massa} kg\n", "#7F8C8D");
                    AdicionarSaida($"         - Velocidade: {obj1.Velocidade} m/s\n", "#7F8C8D");
                    AdicionarSaida($"         - Momento: {obj1.Massa * obj1.Velocidade:F2} kg·m/s\n", "#7F8C8D");
                    AdicionarSaida($"      • {nomeObj2}:\n", "#7F8C8D");
                    AdicionarSaida($"         - Massa: {obj2.Massa} kg\n", "#7F8C8D");
                    AdicionarSaida($"         - Velocidade: {obj2.Velocidade} m/s\n", "#7F8C8D");
                    AdicionarSaida($"         - Momento: {obj2.Massa * obj2.Velocidade:F2} kg·m/s\n", "#7F8C8D");

                    // Conservação de momento linear
                    double momentoInicial = (obj1.Massa * obj1.Velocidade) + (obj2.Massa * obj2.Velocidade);

                    AdicionarSaida($"\n", "#7F8C8D");
                    AdicionarSaida($"   🔬 3ª Lei de Newton - Ação e Reação:\n", "#C0392B");
                    AdicionarSaida($"      • Força de {nomeObj1} em {nomeObj2} = - Força de {nomeObj2} em {nomeObj1}\n", "#7F8C8D");
                    AdicionarSaida($"      • As forças são iguais em intensidade mas opostas em direção\n", "#7F8C8D");
                    AdicionarSaida($"\n", "#7F8C8D");
                    AdicionarSaida($"   📐 Conservação do Momento Linear:\n", "#7F8C8D");
                    AdicionarSaida($"      p_total = m₁v₁ + m₂v₂ = constante\n", "#7F8C8D");
                    AdicionarSaida($"      p_total = {momentoInicial:F2} kg·m/s\n", "#7F8C8D");

                    // Colisão perfeitamente inelástica (objetos grudam)
                    double velocidadeFinal = momentoInicial / (obj1.Massa + obj2.Massa);

                    AdicionarSaida($"\n", "#7F8C8D");
                    AdicionarSaida($"   📊 Dados DEPOIS da colisão (inelástica):\n", "#27AE60");
                    AdicionarSaida($"      • Velocidade final (objetos juntos): {velocidadeFinal:F2} m/s\n", "#27AE60");
                    AdicionarSaida($"      • Massa total: {obj1.Massa + obj2.Massa} kg\n", "#27AE60");
                    AdicionarSaida($"      • Momento final: {(obj1.Massa + obj2.Massa) * velocidadeFinal:F2} kg·m/s\n", "#27AE60");
                    AdicionarSaida($"   ═══════════════════════════════════════\n", "#7F8C8D");

                    // Atualiza velocidades
                    obj1.Velocidade = velocidadeFinal;
                    obj2.Velocidade = velocidadeFinal;

                    AdicionarSaida($"   ✅ Momento conservado! Sistema físico coerente.\n", "#27AE60");
                }
                else
                {
                    AdicionarSaida($"❌ Um ou ambos os objetos não foram encontrados!\n", "#E74C3C");
                    if (objetos.Count > 0)
                    {
                        AdicionarSaida($"💡 Objetos disponíveis: {string.Join(", ", objetos.Keys)}\n", "#F39C12");
                    }
                }
            }
            catch (Exception ex)
            {
                AdicionarSaida($"❌ Erro ao simular colisão: {ex.Message}\n", "#E74C3C");
            }
        }

        private void SimularInercia(string comando)
        {
            try
            {
                // Extrai nome do objeto
                int inicio = comando.IndexOf("(") + 1;
                int fim = comando.IndexOf(",");

                if (fim <= inicio)
                {
                    AdicionarSaida("❌ Erro: Sintaxe incorreta. Use: simularInercia(objeto, tempo: valor)\n", "#E74C3C");
                    return;
                }

                string nomeObj = comando.Substring(inicio, fim - inicio).Trim();

                // Extrai tempo
                double tempo = ExtrairValorParametro(comando, "tempo:");

                if (tempo <= 0)
                {
                    AdicionarSaida("❌ Erro: O tempo deve ser maior que zero!\n", "#E74C3C");
                    return;
                }

                if (objetos.ContainsKey(nomeObj))
                {
                    ObjetoFisico obj = objetos[nomeObj];

                    AdicionarSaida($"🌟 SIMULAÇÃO DE INÉRCIA - 1ª Lei de Newton\n", "#16A085");
                    AdicionarSaida($"═══════════════════════════════════════\n", "#7F8C8D");
                    AdicionarSaida($"   Objeto: '{nomeObj}'\n", "#7F8C8D");
                    AdicionarSaida($"\n", "#7F8C8D");
                    AdicionarSaida($"   📊 Condições iniciais:\n", "#7F8C8D");
                    AdicionarSaida($"      • Velocidade: {obj.Velocidade} m/s\n", "#7F8C8D");
                    AdicionarSaida($"      • Força resultante: 0 N (sem forças externas)\n", "#7F8C8D");
                    AdicionarSaida($"      • Tempo de observação: {tempo} segundos\n", "#7F8C8D");

                    // Calcula deslocamento (sem aceleração, movimento uniforme)
                    double deslocamento = obj.Velocidade * tempo;

                    AdicionarSaida($"\n", "#7F8C8D");
                    AdicionarSaida($"   🔬 1ª Lei de Newton - Princípio da Inércia:\n", "#16A085");
                    AdicionarSaida($"      • ΣF = 0 → a = 0\n", "#7F8C8D");
                    AdicionarSaida($"      • Sem força resultante, o objeto mantém seu estado!\n", "#7F8C8D");

                    if (obj.Velocidade == 0)
                    {
                        AdicionarSaida($"      • Objeto parado → permanece parado (inércia de repouso)\n", "#7F8C8D");
                    }
                    else
                    {
                        AdicionarSaida($"      • Objeto em movimento → continua com velocidade constante\n", "#7F8C8D");
                    }

                    AdicionarSaida($"\n", "#7F8C8D");
                    AdicionarSaida($"   📐 Cálculo do Movimento Uniforme:\n", "#7F8C8D");
                    AdicionarSaida($"      Δs = v × t\n", "#7F8C8D");
                    AdicionarSaida($"      Δs = {obj.Velocidade} m/s × {tempo} s\n", "#7F8C8D");

                    AdicionarSaida($"\n", "#7F8C8D");
                    AdicionarSaida($"   ✅ Resultados após {tempo} segundos:\n", "#27AE60");
                    AdicionarSaida($"      • Deslocamento: {deslocamento:F2} metros\n", "#27AE60");
                    AdicionarSaida($"      • Velocidade final: {obj.Velocidade} m/s (inalterada)\n", "#27AE60");
                    AdicionarSaida($"      • Aceleração: 0 m/s²\n", "#27AE60");
                    AdicionarSaida($"   ═══════════════════════════════════════\n", "#7F8C8D");

                    if (obj.Velocidade == 0)
                    {
                        AdicionarSaida($"   💡 Objeto permaneceu em repouso - Inércia!\n", "#F39C12");
                    }
                    else
                    {
                        AdicionarSaida($"   💡 Movimento Retilíneo Uniforme mantido - Inércia!\n", "#F39C12");
                    }
                }
                else
                {
                    AdicionarSaida($"❌ Objeto '{nomeObj}' não encontrado!\n", "#E74C3C");
                    if (objetos.Count > 0)
                    {
                        AdicionarSaida($"💡 Objetos disponíveis: {string.Join(", ", objetos.Keys)}\n", "#F39C12");
                    }
                }
            }
            catch (Exception ex)
            {
                AdicionarSaida($"❌ Erro ao simular inércia: {ex.Message}\n", "#E74C3C");
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
                    if (c == ',' || c == ')' || (c == ' ' && i > 0))
                    {
                        fimNumero = i;
                        break;
                    }
                }

                string valorStr = resto.Substring(0, fimNumero).Trim();

                // Tenta converter, aceitando tanto ponto quanto vírgula
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
            CodeEditor.Text = "// Digite seu código aqui\nescreva(\"Olá, mundo da Física!\")\n";

            // Limpa o console
            outputBuilder.Clear();
            ConsoleOutput.Text = "> Simulador de Física iniciado.\n" +
                                "> Digite seu código e clique em EXECUTAR.\n" +
                                "> Explore as Leis de Newton!";

            // Limpa objetos criados
            objetos.Clear();
        }

        private void ExercisesButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "📝 Exercícios Práticos - Em Desenvolvimento!\n\n" +
                "Em breve você terá acesso a:\n" +
                "• Desafios práticos sobre as Leis de Newton\n" +
                "• Problemas interativos para resolver\n" +
                "• Sistema de pontuação e feedback\n" +
                "• Certificados de conclusão",
                "Exercícios Práticos",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }

        private void QuizButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "🧪 Quiz sobre Leis de Newton - Em Desenvolvimento!\n\n" +
                "Em breve você poderá testar seus conhecimentos com:\n" +
                "• Questões de múltipla escolha\n" +
                "• Problemas de cálculo\n" +
                "• Situações práticas\n" +
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
            objetos?.Clear();
            outputBuilder?.Clear();
            base.OnClosed(e);
        }
    }
}