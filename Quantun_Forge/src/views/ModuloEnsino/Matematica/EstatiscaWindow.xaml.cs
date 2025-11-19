using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Quantun_Forge.src.views.ModuloEnsino.Matematica.Testes
{
    public partial class EstatiscaWindow : Window
    {
        private List<double> dados = new List<double>();

        public EstatiscaWindow()
        {
            InitializeComponent();
            ConfigurarEventos();
        }

        private void ConfigurarEventos()
        {
            btnCarregarDados.Click += BtnCarregarDados_Click;
            btnLimpar.Click += BtnLimpar_Click;
            btnMedia.Click += BtnMedia_Click;
            btnMediana.Click += BtnMediana_Click;
            btnModa.Click += BtnModa_Click;
            btnAmplitude.Click += BtnAmplitude_Click;
            btnVariancia.Click += BtnVariancia_Click;
            btnDesvioPadrao.Click += BtnDesvioPadrao_Click;
            btnSoma.Click += BtnSoma_Click;
            btnContagem.Click += BtnContagem_Click;
            btnAnaliseCompleta.Click += BtnAnaliseCompleta_Click;
            btnOrdenarCrescente.Click += BtnOrdenarCrescente_Click;
            btnOrdenarDecrescente.Click += BtnOrdenarDecrescente_Click;
            btnExportar.Click += BtnExportar_Click;
        }

        private void BtnCarregarDados_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string input = txtDadosInput.Text.Trim();
                if (string.IsNullOrEmpty(input))
                {
                    MessageBox.Show("Por favor, insira os dados!", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Separar por vírgula, espaço ou ponto e vírgula
                string[] valores = input.Split(new[] { ',', ' ', ';', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

                dados.Clear();
                foreach (string valor in valores)
                {
                    if (double.TryParse(valor.Replace('.', ','), out double numero))
                    {
                        dados.Add(numero);
                    }
                }

                if (dados.Count == 0)
                {
                    MessageBox.Show("Nenhum valor válido foi encontrado!", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                txtDadosCarregados.Text = $"Dados carregados: {dados.Count} valores";
                txtResultados.Text = $"✓ {dados.Count} valores carregados com sucesso!\n\nDados: {string.Join(", ", dados)}";

                MessageBox.Show($"Dados carregados com sucesso!\nTotal: {dados.Count} valores", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar dados: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnLimpar_Click(object sender, RoutedEventArgs e)
        {
            dados.Clear();
            txtDadosInput.Clear();
            txtResultados.Text = "Aguardando cálculos...";
            txtDadosCarregados.Text = "Dados carregados: 0 valores";
        }

        private bool VerificarDados()
        {
            if (dados.Count == 0)
            {
                MessageBox.Show("Por favor, carregue os dados primeiro!", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }

        private void BtnMedia_Click(object sender, RoutedEventArgs e)
        {
            if (!VerificarDados()) return;

            double media = dados.Average();
            txtResultados.Text = $"📐 MÉDIA\n\n" +
                                $"Fórmula: Σx / n\n" +
                                $"Soma: {dados.Sum():F2}\n" +
                                $"Quantidade: {dados.Count}\n" +
                                $"Média = {media:F4}";
        }

        private void BtnMediana_Click(object sender, RoutedEventArgs e)
        {
            if (!VerificarDados()) return;

            var ordenados = dados.OrderBy(x => x).ToList();
            double mediana;

            if (ordenados.Count % 2 == 0)
            {
                int meio1 = ordenados.Count / 2 - 1;
                int meio2 = ordenados.Count / 2;
                mediana = (ordenados[meio1] + ordenados[meio2]) / 2.0;

                txtResultados.Text = $"📊 MEDIANA\n\n" +
                                    $"Dados ordenados: {string.Join(", ", ordenados)}\n\n" +
                                    $"Posição central: {meio1 + 1} e {meio2 + 1}\n" +
                                    $"Valores: {ordenados[meio1]} e {ordenados[meio2]}\n" +
                                    $"Mediana = ({ordenados[meio1]} + {ordenados[meio2]}) / 2\n" +
                                    $"Mediana = {mediana:F4}";
            }
            else
            {
                int meio = ordenados.Count / 2;
                mediana = ordenados[meio];

                txtResultados.Text = $"📊 MEDIANA\n\n" +
                                    $"Dados ordenados: {string.Join(", ", ordenados)}\n\n" +
                                    $"Posição central: {meio + 1}\n" +
                                    $"Mediana = {mediana:F4}";
            }
        }

        private void BtnModa_Click(object sender, RoutedEventArgs e)
        {
            if (!VerificarDados()) return;

            var frequencias = dados.GroupBy(x => x)
                                  .Select(g => new { Valor = g.Key, Frequencia = g.Count() })
                                  .OrderByDescending(x => x.Frequencia)
                                  .ToList();

            int maxFreq = frequencias.First().Frequencia;
            var modas = frequencias.Where(x => x.Frequencia == maxFreq).ToList();

            string resultado = "🎯 MODA\n\n";
            resultado += "Frequências:\n";
            foreach (var f in frequencias.Take(10))
            {
                resultado += $"  {f.Valor} → {f.Frequencia} vez(es)\n";
            }

            resultado += "\n";
            if (maxFreq == 1)
            {
                resultado += "Amodal (todos os valores aparecem apenas 1 vez)";
            }
            else if (modas.Count == 1)
            {
                resultado += $"Unimodal\nModa = {modas[0].Valor:F2} (aparece {maxFreq} vezes)";
            }
            else if (modas.Count == 2)
            {
                resultado += $"Bimodal\nModas: {string.Join(" e ", modas.Select(m => m.Valor.ToString("F2")))}\n(aparecem {maxFreq} vezes cada)";
            }
            else
            {
                resultado += $"Multimodal\nModas: {string.Join(", ", modas.Select(m => m.Valor.ToString("F2")))}\n(aparecem {maxFreq} vezes cada)";
            }

            txtResultados.Text = resultado;
        }

        private void BtnAmplitude_Click(object sender, RoutedEventArgs e)
        {
            if (!VerificarDados()) return;

            double min = dados.Min();
            double max = dados.Max();
            double amplitude = max - min;

            txtResultados.Text = $"📏 AMPLITUDE\n\n" +
                                $"Valor mínimo: {min:F2}\n" +
                                $"Valor máximo: {max:F2}\n" +
                                $"Amplitude = Máx - Mín\n" +
                                $"Amplitude = {max:F2} - {min:F2}\n" +
                                $"Amplitude = {amplitude:F4}";
        }

        private void BtnVariancia_Click(object sender, RoutedEventArgs e)
        {
            if (!VerificarDados()) return;

            double media = dados.Average();
            double somaQuadrados = dados.Sum(x => Math.Pow(x - media, 2));
            double variancia = somaQuadrados / dados.Count;

            txtResultados.Text = $"📈 VARIÂNCIA\n\n" +
                                $"Média: {media:F4}\n" +
                                $"Σ(x - média)²: {somaQuadrados:F4}\n" +
                                $"n = {dados.Count}\n\n" +
                                $"Variância = Σ(x - média)² / n\n" +
                                $"Variância = {variancia:F6}";
        }

        private void BtnDesvioPadrao_Click(object sender, RoutedEventArgs e)
        {
            if (!VerificarDados()) return;

            double media = dados.Average();
            double variancia = dados.Sum(x => Math.Pow(x - media, 2)) / dados.Count;
            double desvioPadrao = Math.Sqrt(variancia);

            txtResultados.Text = $"📉 DESVIO PADRÃO\n\n" +
                                $"Média: {media:F4}\n" +
                                $"Variância: {variancia:F6}\n\n" +
                                $"Desvio Padrão = √Variância\n" +
                                $"Desvio Padrão = √{variancia:F6}\n" +
                                $"Desvio Padrão = {desvioPadrao:F6}";
        }

        private void BtnSoma_Click(object sender, RoutedEventArgs e)
        {
            if (!VerificarDados()) return;

            double soma = dados.Sum();
            txtResultados.Text = $"Σ SOMA TOTAL\n\n" +
                                $"Valores: {string.Join(" + ", dados)}\n\n" +
                                $"Soma = {soma:F4}";
        }

        private void BtnContagem_Click(object sender, RoutedEventArgs e)
        {
            if (!VerificarDados()) return;

            txtResultados.Text = $"🔢 CONTAGEM\n\n" +
                                $"Quantidade de valores: {dados.Count}\n" +
                                $"Valores únicos: {dados.Distinct().Count()}";
        }

        private void BtnAnaliseCompleta_Click(object sender, RoutedEventArgs e)
        {
            if (!VerificarDados()) return;

            double media = dados.Average();
            double variancia = dados.Sum(x => Math.Pow(x - media, 2)) / dados.Count;
            double desvioPadrao = Math.Sqrt(variancia);

            var ordenados = dados.OrderBy(x => x).ToList();
            double mediana;
            if (ordenados.Count % 2 == 0)
            {
                mediana = (ordenados[ordenados.Count / 2 - 1] + ordenados[ordenados.Count / 2]) / 2.0;
            }
            else
            {
                mediana = ordenados[ordenados.Count / 2];
            }

            var frequencias = dados.GroupBy(x => x)
                                  .Select(g => new { Valor = g.Key, Freq = g.Count() })
                                  .OrderByDescending(x => x.Freq)
                                  .ToList();
            string moda = frequencias.First().Freq == 1 ? "Amodal" :
                         string.Join(", ", frequencias.Where(x => x.Freq == frequencias.First().Freq)
                                                     .Select(x => x.Valor.ToString("F2")));

            string resultado = "═══════════════════════════════════\n";
            resultado += "   📊 ANÁLISE ESTATÍSTICA COMPLETA\n";
            resultado += "═══════════════════════════════════\n\n";
            resultado += $"📝 DADOS ({dados.Count} valores):\n";
            resultado += $"   {string.Join(", ", dados)}\n\n";
            resultado += "───────────────────────────────────\n";
            resultado += "📐 MEDIDAS DE TENDÊNCIA CENTRAL:\n";
            resultado += "───────────────────────────────────\n";
            resultado += $"   Média:    {media:F4}\n";
            resultado += $"   Mediana:  {mediana:F4}\n";
            resultado += $"   Moda:     {moda}\n\n";
            resultado += "───────────────────────────────────\n";
            resultado += "📏 MEDIDAS DE DISPERSÃO:\n";
            resultado += "───────────────────────────────────\n";
            resultado += $"   Amplitude:      {(dados.Max() - dados.Min()):F4}\n";
            resultado += $"   Variância:      {variancia:F6}\n";
            resultado += $"   Desvio Padrão:  {desvioPadrao:F6}\n\n";
            resultado += "───────────────────────────────────\n";
            resultado += "📈 VALORES EXTREMOS:\n";
            resultado += "───────────────────────────────────\n";
            resultado += $"   Mínimo:  {dados.Min():F2}\n";
            resultado += $"   Máximo:  {dados.Max():F2}\n";
            resultado += $"   Soma:    {dados.Sum():F2}\n\n";
            resultado += "═══════════════════════════════════\n";

            txtResultados.Text = resultado;
        }

        private void BtnOrdenarCrescente_Click(object sender, RoutedEventArgs e)
        {
            if (!VerificarDados()) return;

            var ordenados = dados.OrderBy(x => x).ToList();
            txtResultados.Text = $"⬆️ ORDEM CRESCENTE\n\n{string.Join(", ", ordenados)}";
        }

        private void BtnOrdenarDecrescente_Click(object sender, RoutedEventArgs e)
        {
            if (!VerificarDados()) return;

            var ordenados = dados.OrderByDescending(x => x).ToList();
            txtResultados.Text = $"⬇️ ORDEM DECRESCENTE\n\n{string.Join(", ", ordenados)}";
        }

        private void BtnExportar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string conteudo = $"RELATÓRIO ESTATÍSTICO - {DateTime.Now:dd/MM/yyyy HH:mm}\n\n";
                conteudo += txtResultados.Text;

                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog
                {
                    FileName = $"Estatistica_{DateTime.Now:yyyyMMdd_HHmmss}",
                    DefaultExt = ".txt",
                    Filter = "Ficheiros de texto (.txt)|*.txt"
                };

                if (dlg.ShowDialog() == true)
                {
                    System.IO.File.WriteAllText(dlg.FileName, conteudo);
                    MessageBox.Show("Relatório exportado com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao exportar: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}