using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Quantun_Forge.src.views.ModuloEnsino.Computacao.ComputacaoClassica.Testes;

namespace Quantun_Forge.src.views.ModuloEnsino.Computacao.ComputacaoClassica
{
    /// <summary>
    /// Lógica interna para PLABWindow.xaml
    /// </summary>
    public partial class PLABWindow : Window
    {
        private StringBuilder output;
        private Dictionary<string, int> variables;

        public PLABWindow()
        {
            InitializeComponent();
            output = new StringBuilder();
            variables = new Dictionary<string, int>();
        }

        /// <summary>
        /// Executa o código do editor
        /// </summary>
        private void RunCode_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                output.Clear();
                variables.Clear();

                string code = CodeEditor.Text;
                string[] lines = code.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string line in lines)
                {
                    string trimmedLine = line.Trim();

                    // Ignora comentários e linhas vazias
                    if (string.IsNullOrWhiteSpace(trimmedLine) || trimmedLine.StartsWith("//"))
                        continue;

                    ProcessLine(trimmedLine);
                }

                ConsoleOutput.Text = output.ToString();
            }
            catch (Exception ex)
            {
                output.AppendLine($"[ERRO] {ex.Message}");
                ConsoleOutput.Text = output.ToString();
                ConsoleOutput.Foreground = new SolidColorBrush(Colors.Red);
            }
        }

        /// <summary>
        /// Processa uma linha de código
        /// </summary>
        private void ProcessLine(string line)
        {
            try
            {
                // Remove ponto e vírgula no final
                line = line.TrimEnd(';');

                // Comando escreva
                if (line.StartsWith("escreva(") && line.EndsWith(")"))
                {
                    string content = ExtractParameter(line, "escreva");
                    content = EvaluateExpression(content);
                    output.AppendLine($"> {content}");
                    return;
                }

                // Portas Lógicas Básicas
                if (line.Contains("AND("))
                {
                    ProcessLogicGate(line, "AND", AND);
                    return;
                }

                if (line.Contains("OR("))
                {
                    ProcessLogicGate(line, "OR", OR);
                    return;
                }

                if (line.Contains("NOT("))
                {
                    ProcessLogicGateUnary(line, "NOT", NOT);
                    return;
                }

                // Portas Lógicas Compostas
                if (line.Contains("NAND("))
                {
                    ProcessLogicGate(line, "NAND", NAND);
                    return;
                }

                if (line.Contains("NOR("))
                {
                    ProcessLogicGate(line, "NOR", NOR);
                    return;
                }

                if (line.Contains("XOR("))
                {
                    ProcessLogicGate(line, "XOR", XOR);
                    return;
                }

                if (line.Contains("XNOR("))
                {
                    ProcessLogicGate(line, "XNOR", XNOR);
                    return;
                }

                // Tabela Verdade
                if (line.StartsWith("tabelaVerdade(") && line.EndsWith(")"))
                {
                    string gateName = ExtractParameter(line, "tabelaVerdade");
                    gateName = gateName.Trim('"', '\'').ToUpper();
                    MostrarTabelaVerdade(gateName);
                    return;
                }

                // Verificar Lei
                if (line.StartsWith("verificarLei("))
                {
                    VerificarLei(line);
                    return;
                }

                // Atribuição de variável
                if (line.Contains("=") && !line.Contains("=="))
                {
                    string[] parts = line.Split('=');
                    if (parts.Length == 2)
                    {
                        string varName = parts[0].Trim();
                        string expression = parts[1].Trim();
                        int value = EvaluateLogicExpression(expression);
                        variables[varName] = value;
                        output.AppendLine($"> Variável '{varName}' = {value}");
                        return;
                    }
                }

                // Se nenhum comando foi reconhecido
                output.AppendLine($"[AVISO] Comando não reconhecido: {line}");
            }
            catch (Exception ex)
            {
                output.AppendLine($"[ERRO na linha] {ex.Message}");
            }
        }

        /// <summary>
        /// Processa uma porta lógica binária
        /// </summary>
        private void ProcessLogicGate(string line, string gateName, Func<int, int, int> gateFunction)
        {
            Match match = Regex.Match(line, $@"{gateName}\(([^,]+),([^)]+)\)");
            if (match.Success)
            {
                string param1 = match.Groups[1].Value.Trim();
                string param2 = match.Groups[2].Value.Trim();

                int a = ParseBit(param1);
                int b = ParseBit(param2);
                int result = gateFunction(a, b);

                // Verifica se há escreva
                if (line.Contains("escreva("))
                {
                    string outputLine = line.Replace($"{gateName}({param1},{param2})", result.ToString());
                    ProcessLine(outputLine);
                }
                else
                {
                    output.AppendLine($"> {gateName}({a}, {b}) = {result}");
                }
            }
        }

        /// <summary>
        /// Processa uma porta lógica unária (NOT)
        /// </summary>
        private void ProcessLogicGateUnary(string line, string gateName, Func<int, int> gateFunction)
        {
            Match match = Regex.Match(line, $@"{gateName}\(([^)]+)\)");
            if (match.Success)
            {
                string param = match.Groups[1].Value.Trim();
                int a = ParseBit(param);
                int result = gateFunction(a);

                // Verifica se há escreva
                if (line.Contains("escreva("))
                {
                    string outputLine = line.Replace($"{gateName}({param})", result.ToString());
                    ProcessLine(outputLine);
                }
                else
                {
                    output.AppendLine($"> {gateName}({a}) = {result}");
                }
            }
        }

        /// <summary>
        /// Extrai o parâmetro de uma função
        /// </summary>
        private string ExtractParameter(string line, string functionName)
        {
            int start = line.IndexOf('(') + 1;
            int end = line.LastIndexOf(')');
            string content = line.Substring(start, end - start);

            // Remove aspas se existirem
            content = content.Trim('"', '\'');

            return content;
        }

        /// <summary>
        /// Avalia uma expressão (suporta concatenação)
        /// </summary>
        private string EvaluateExpression(string expression)
        {
            // Remove aspas do início e fim
            expression = expression.Trim('"', '\'');

            // Processa concatenação com +
            if (expression.Contains("+"))
            {
                string[] parts = expression.Split('+');
                StringBuilder result = new StringBuilder();

                foreach (string part in parts)
                {
                    string trimmed = part.Trim().Trim('"', '\'');

                    // Tenta avaliar como expressão lógica
                    if (IsLogicExpression(trimmed))
                    {
                        result.Append(EvaluateLogicExpression(trimmed));
                    }
                    else
                    {
                        result.Append(trimmed);
                    }
                }

                return result.ToString();
            }

            return expression;
        }

        /// <summary>
        /// Verifica se é uma expressão lógica
        /// </summary>
        private bool IsLogicExpression(string expr)
        {
            return expr.Contains("AND(") || expr.Contains("OR(") || expr.Contains("NOT(") ||
                   expr.Contains("NAND(") || expr.Contains("NOR(") || expr.Contains("XOR(") ||
                   expr.Contains("XNOR(");
        }

        /// <summary>
        /// Avalia uma expressão lógica e retorna o resultado
        /// </summary>
        private int EvaluateLogicExpression(string expression)
        {
            expression = expression.Trim();

            // AND
            Match andMatch = Regex.Match(expression, @"AND\(([^,]+),([^)]+)\)");
            if (andMatch.Success)
            {
                int a = ParseBit(andMatch.Groups[1].Value.Trim());
                int b = ParseBit(andMatch.Groups[2].Value.Trim());
                return AND(a, b);
            }

            // OR
            Match orMatch = Regex.Match(expression, @"OR\(([^,]+),([^)]+)\)");
            if (orMatch.Success)
            {
                int a = ParseBit(orMatch.Groups[1].Value.Trim());
                int b = ParseBit(orMatch.Groups[2].Value.Trim());
                return OR(a, b);
            }

            // NOT
            Match notMatch = Regex.Match(expression, @"NOT\(([^)]+)\)");
            if (notMatch.Success)
            {
                int a = ParseBit(notMatch.Groups[1].Value.Trim());
                return NOT(a);
            }

            // NAND
            Match nandMatch = Regex.Match(expression, @"NAND\(([^,]+),([^)]+)\)");
            if (nandMatch.Success)
            {
                int a = ParseBit(nandMatch.Groups[1].Value.Trim());
                int b = ParseBit(nandMatch.Groups[2].Value.Trim());
                return NAND(a, b);
            }

            // NOR
            Match norMatch = Regex.Match(expression, @"NOR\(([^,]+),([^)]+)\)");
            if (norMatch.Success)
            {
                int a = ParseBit(norMatch.Groups[1].Value.Trim());
                int b = ParseBit(norMatch.Groups[2].Value.Trim());
                return NOR(a, b);
            }

            // XOR
            Match xorMatch = Regex.Match(expression, @"XOR\(([^,]+),([^)]+)\)");
            if (xorMatch.Success)
            {
                int a = ParseBit(xorMatch.Groups[1].Value.Trim());
                int b = ParseBit(xorMatch.Groups[2].Value.Trim());
                return XOR(a, b);
            }

            // XNOR
            Match xnorMatch = Regex.Match(expression, @"XNOR\(([^,]+),([^)]+)\)");
            if (xnorMatch.Success)
            {
                int a = ParseBit(xorMatch.Groups[1].Value.Trim());
                int b = ParseBit(xnorMatch.Groups[2].Value.Trim());
                return XNOR(a, b);
            }

            // Tenta parsear como número
            if (int.TryParse(expression, out int value))
            {
                return value;
            }

            // Tenta buscar variável
            if (variables.ContainsKey(expression))
            {
                return variables[expression];
            }

            throw new Exception($"Não foi possível avaliar a expressão: {expression}");
        }

        /// <summary>
        /// Converte string para bit (0 ou 1)
        /// </summary>
        private int ParseBit(string value)
        {
            value = value.Trim();

            // Verifica se é uma variável
            if (variables.ContainsKey(value))
            {
                return variables[value];
            }

            // Tenta converter para número
            if (int.TryParse(value, out int result))
            {
                return result == 0 ? 0 : 1; // Normaliza para 0 ou 1
            }

            throw new Exception($"Valor inválido: {value}. Use 0 ou 1.");
        }

        /// <summary>
        /// Mostra a tabela verdade de uma porta lógica
        /// </summary>
        private void MostrarTabelaVerdade(string gateName)
        {
            output.AppendLine($"\n> ===== TABELA VERDADE: {gateName} =====");

            switch (gateName)
            {
                case "AND":
                    output.AppendLine("> A | B | Saída");
                    output.AppendLine("> --|---|------");
                    for (int a = 0; a <= 1; a++)
                        for (int b = 0; b <= 1; b++)
                            output.AppendLine($"> {a} | {b} |   {AND(a, b)}");
                    break;

                case "OR":
                    output.AppendLine("> A | B | Saída");
                    output.AppendLine("> --|---|------");
                    for (int a = 0; a <= 1; a++)
                        for (int b = 0; b <= 1; b++)
                            output.AppendLine($"> {a} | {b} |   {OR(a, b)}");
                    break;

                case "NOT":
                    output.AppendLine("> A | Saída");
                    output.AppendLine("> --|------");
                    for (int a = 0; a <= 1; a++)
                        output.AppendLine($"> {a} |   {NOT(a)}");
                    break;

                case "NAND":
                    output.AppendLine("> A | B | Saída");
                    output.AppendLine("> --|---|------");
                    for (int a = 0; a <= 1; a++)
                        for (int b = 0; b <= 1; b++)
                            output.AppendLine($"> {a} | {b} |   {NAND(a, b)}");
                    break;

                case "NOR":
                    output.AppendLine("> A | B | Saída");
                    output.AppendLine("> --|---|------");
                    for (int a = 0; a <= 1; a++)
                        for (int b = 0; b <= 1; b++)
                            output.AppendLine($"> {a} | {b} |   {NOR(a, b)}");
                    break;

                case "XOR":
                    output.AppendLine("> A | B | Saída");
                    output.AppendLine("> --|---|------");
                    for (int a = 0; a <= 1; a++)
                        for (int b = 0; b <= 1; b++)
                            output.AppendLine($"> {a} | {b} |   {XOR(a, b)}");
                    break;

                case "XNOR":
                    output.AppendLine("> A | B | Saída");
                    output.AppendLine("> --|---|------");
                    for (int a = 0; a <= 1; a++)
                        for (int b = 0; b <= 1; b++)
                            output.AppendLine($"> {a} | {b} |   {XNOR(a, b)}");
                    break;

                default:
                    output.AppendLine($"[ERRO] Porta lógica '{gateName}' não reconhecida.");
                    break;
            }

            output.AppendLine("> ================================\n");
        }

        /// <summary>
        /// Verifica leis da álgebra booleana
        /// </summary>
        private void VerificarLei(string line)
        {
            Match match = Regex.Match(line, @"verificarLei\([""']([^""']+)[""'],?\s*(\d)?,?\s*(\d)?\)");

            if (match.Success)
            {
                string leiNome = match.Groups[1].Value.ToLower();

                output.AppendLine($"\n> ===== VERIFICANDO LEI: {leiNome.ToUpper()} =====");

                switch (leiNome)
                {
                    case "demorgan":
                    case "de morgan":
                        VerificarLeiDeMorgan();
                        break;

                    case "comutativa":
                        VerificarLeiComutativa();
                        break;

                    case "associativa":
                        VerificarLeiAssociativa();
                        break;

                    case "distributiva":
                        VerificarLeiDistributiva();
                        break;

                    case "identidade":
                        VerificarLeiIdentidade();
                        break;

                    case "complemento":
                        VerificarLeiComplemento();
                        break;

                    case "idempotencia":
                    case "idempotência":
                        VerificarLeiIdempotencia();
                        break;

                    default:
                        output.AppendLine($"[ERRO] Lei '{leiNome}' não reconhecida.");
                        output.AppendLine("> Leis disponíveis: DeMorgan, Comutativa, Associativa, Distributiva, Identidade, Complemento, Idempotencia");
                        break;
                }

                output.AppendLine("> ================================\n");
            }
        }

        #region Leis da Álgebra Booleana

        private void VerificarLeiDeMorgan()
        {
            output.AppendLine("> Lei de De Morgan:");
            output.AppendLine("> 1) NOT(A AND B) = NOT(A) OR NOT(B)");
            output.AppendLine("> 2) NOT(A OR B) = NOT(A) AND NOT(B)");
            output.AppendLine();

            bool lei1Valida = true;
            bool lei2Valida = true;

            for (int a = 0; a <= 1; a++)
            {
                for (int b = 0; b <= 1; b++)
                {
                    // Lei 1
                    int lado1 = NOT(AND(a, b));
                    int lado2 = OR(NOT(a), NOT(b));
                    bool valida1 = (lado1 == lado2);

                    output.AppendLine($"> A={a}, B={b}: NOT({a} AND {b})={lado1}, NOT({a}) OR NOT({b})={lado2} → {(valida1 ? "✓" : "✗")}");

                    if (!valida1) lei1Valida = false;

                    // Lei 2
                    int lado3 = NOT(OR(a, b));
                    int lado4 = AND(NOT(a), NOT(b));
                    bool valida2 = (lado3 == lado4);

                    output.AppendLine($"> A={a}, B={b}: NOT({a} OR {b})={lado3}, NOT({a}) AND NOT({b})={lado4} → {(valida2 ? "✓" : "✗")}");

                    if (!valida2) lei2Valida = false;
                }
            }

            output.AppendLine($"\n> Resultado: {(lei1Valida && lei2Valida ? "VÁLIDA ✓" : "INVÁLIDA ✗")}");
        }

        private void VerificarLeiComutativa()
        {
            output.AppendLine("> Lei Comutativa:");
            output.AppendLine("> A AND B = B AND A");
            output.AppendLine("> A OR B = B OR A");
            output.AppendLine();

            bool valida = true;

            for (int a = 0; a <= 1; a++)
            {
                for (int b = 0; b <= 1; b++)
                {
                    bool andValido = (AND(a, b) == AND(b, a));
                    bool orValido = (OR(a, b) == OR(b, a));

                    output.AppendLine($"> A={a}, B={b}: {a} AND {b}={AND(a, b)}, {b} AND {a}={AND(b, a)} → {(andValido ? "✓" : "✗")}");
                    output.AppendLine($"> A={a}, B={b}: {a} OR {b}={OR(a, b)}, {b} OR {a}={OR(b, a)} → {(orValido ? "✓" : "✗")}");

                    if (!andValido || !orValido) valida = false;
                }
            }

            output.AppendLine($"\n> Resultado: {(valida ? "VÁLIDA ✓" : "INVÁLIDA ✗")}");
        }

        private void VerificarLeiAssociativa()
        {
            output.AppendLine("> Lei Associativa:");
            output.AppendLine("> (A AND B) AND C = A AND (B AND C)");
            output.AppendLine("> (A OR B) OR C = A OR (B OR C)");
            output.AppendLine();

            bool valida = true;

            for (int a = 0; a <= 1; a++)
            {
                for (int b = 0; b <= 1; b++)
                {
                    for (int c = 0; c <= 1; c++)
                    {
                        int andEsq = AND(AND(a, b), c);
                        int andDir = AND(a, AND(b, c));
                        bool andValido = (andEsq == andDir);

                        int orEsq = OR(OR(a, b), c);
                        int orDir = OR(a, OR(b, c));
                        bool orValido = (orEsq == orDir);

                        if (a == 1 && b == 1 && c == 1) // Mostra apenas um exemplo
                        {
                            output.AppendLine($"> ({a} AND {b}) AND {c} = {andEsq}, {a} AND ({b} AND {c}) = {andDir} → {(andValido ? "✓" : "✗")}");
                            output.AppendLine($"> ({a} OR {b}) OR {c} = {orEsq}, {a} OR ({b} OR {c}) = {orDir} → {(orValido ? "✓" : "✗")}");
                        }

                        if (!andValido || !orValido) valida = false;
                    }
                }
            }

            output.AppendLine($"\n> Resultado: {(valida ? "VÁLIDA ✓" : "INVÁLIDA ✗")}");
        }

        private void VerificarLeiDistributiva()
        {
            output.AppendLine("> Lei Distributiva:");
            output.AppendLine("> A AND (B OR C) = (A AND B) OR (A AND C)");
            output.AppendLine();

            bool valida = true;

            for (int a = 0; a <= 1; a++)
            {
                for (int b = 0; b <= 1; b++)
                {
                    for (int c = 0; c <= 1; c++)
                    {
                        int esq = AND(a, OR(b, c));
                        int dir = OR(AND(a, b), AND(a, c));
                        bool eValido = (esq == dir);

                        if (a == 1 && b == 1 && c == 0) // Mostra apenas um exemplo
                        {
                            output.AppendLine($"> {a} AND ({b} OR {c}) = {esq}");
                            output.AppendLine($"> ({a} AND {b}) OR ({a} AND {c}) = {dir}");
                            output.AppendLine($"> Resultado: {(eValido ? "✓" : "✗")}");
                        }

                        if (!eValido) valida = false;
                    }
                }
            }

            output.AppendLine($"\n> Resultado: {(valida ? "VÁLIDA ✓" : "INVÁLIDA ✗")}");
        }

        private void VerificarLeiIdentidade()
        {
            output.AppendLine("> Lei da Identidade:");
            output.AppendLine("> A AND 1 = A");
            output.AppendLine("> A OR 0 = A");
            output.AppendLine();

            bool valida = true;

            for (int a = 0; a <= 1; a++)
            {
                bool andValido = (AND(a, 1) == a);
                bool orValido = (OR(a, 0) == a);

                output.AppendLine($"> {a} AND 1 = {AND(a, 1)} → {(andValido ? "✓" : "✗")}");
                output.AppendLine($"> {a} OR 0 = {OR(a, 0)} → {(orValido ? "✓" : "✗")}");

                if (!andValido || !orValido) valida = false;
            }

            output.AppendLine($"\n> Resultado: {(valida ? "VÁLIDA ✓" : "INVÁLIDA ✗")}");
        }

        private void VerificarLeiComplemento()
        {
            output.AppendLine("> Lei do Complemento:");
            output.AppendLine("> A AND NOT(A) = 0");
            output.AppendLine("> A OR NOT(A) = 1");
            output.AppendLine();

            bool valida = true;

            for (int a = 0; a <= 1; a++)
            {
                bool andValido = (AND(a, NOT(a)) == 0);
                bool orValido = (OR(a, NOT(a)) == 1);

                output.AppendLine($"> {a} AND NOT({a}) = {AND(a, NOT(a))} → {(andValido ? "✓" : "✗")}");
                output.AppendLine($"> {a} OR NOT({a}) = {OR(a, NOT(a))} → {(orValido ? "✓" : "✗")}");

                if (!andValido || !orValido) valida = false;
            }

            output.AppendLine($"\n> Resultado: {(valida ? "VÁLIDA ✓" : "INVÁLIDA ✗")}");
        }

        private void VerificarLeiIdempotencia()
        {
            output.AppendLine("> Lei da Idempotência:");
            output.AppendLine("> A AND A = A");
            output.AppendLine("> A OR A = A");
            output.AppendLine();

            bool valida = true;

            for (int a = 0; a <= 1; a++)
            {
                bool andValido = (AND(a, a) == a);
                bool orValido = (OR(a, a) == a);

                output.AppendLine($"> {a} AND {a} = {AND(a, a)} → {(andValido ? "✓" : "✗")}");
                output.AppendLine($"> {a} OR {a} = {OR(a, a)} → {(orValido ? "✓" : "✗")}");

                if (!andValido || !orValido) valida = false;
            }

            output.AppendLine($"\n> Resultado: {(valida ? "VÁLIDA ✓" : "INVÁLIDA ✗")}");
        }

        #endregion

        #region Implementação das Portas Lógicas

        /// <summary>
        /// Porta AND: retorna 1 apenas se ambos forem 1
        /// </summary>
        private int AND(int a, int b)
        {
            return (a == 1 && b == 1) ? 1 : 0;
        }

        /// <summary>
        /// Porta OR: retorna 1 se pelo menos um for 1
        /// </summary>
        private int OR(int a, int b)
        {
            return (a == 1 || b == 1) ? 1 : 0;
        }

        /// <summary>
        /// Porta NOT: inverte o bit
        /// </summary>
        private int NOT(int a)
        {
            return (a == 1) ? 0 : 1;
        }

        /// <summary>
        /// Porta NAND: NOT AND
        /// </summary>
        private int NAND(int a, int b)
        {
            return NOT(AND(a, b));
        }

        /// <summary>
        /// Porta NOR: NOT OR
        /// </summary>
        private int NOR(int a, int b)
        {
            return NOT(OR(a, b));
        }

        /// <summary>
        /// Porta XOR: retorna 1 se os bits forem diferentes
        /// </summary>
        private int XOR(int a, int b)
        {
            return (a != b) ? 1 : 0;
        }

        /// <summary>
        /// Porta XNOR: retorna 1 se os bits forem iguais
        /// </summary>
        private int XNOR(int a, int b)
        {
            return (a == b) ? 1 : 0;
        }

        #endregion

        /// <summary>
        /// Limpa o editor e o console
        /// </summary>
        private void ClearCode_Click(object sender, RoutedEventArgs e)
        {
            CodeEditor.Text = "// Digite seu código aqui\n";
            ConsoleOutput.Text = "> Console limpo. Pronto para novos comandos!\n";
            ConsoleOutput.Foreground = new SolidColorBrush(Color.FromRgb(39, 174, 96)); // #27AE60
            output.Clear();
            variables.Clear();
        }

        /// <summary>
        /// Vai para a janela de exercícios práticos
        /// </summary>
        private void btnExePratica_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var testeWindow = new PLABTeste();
                testeWindow.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Erro ao abrir exercícios: {ex.Message}",
                    "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }
    }
}