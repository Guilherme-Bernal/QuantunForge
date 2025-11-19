using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Quantun_Forge.src.views.ModuloEnsino.Computacao.ComputacaoClassica.Testes;

namespace Quantun_Forge.src.views.ModuloEnsino.Computacao.ComputacaoClassica
{
    public partial class ALES : Window
    {
        private List<int> currentArray;
        private int currentAlgorithm = 0;

        public ALES()
        {
            InitializeComponent();
            Loaded += ALES_Loaded;
        }

        private void ALES_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeArray();
        }

        private void InitializeArray()
        {
            currentArray = new List<int> { 10, 25, 30, 45, 60, 75, 90 };
            UpdateArrayDisplay();
        }

        private void UpdateArrayDisplay(int highlightIndex = -1, int compareIndex = -1)
        {
            if (ArrayPanel == null) return;

            ArrayPanel.Children.Clear();

            for (int i = 0; i < currentArray.Count; i++)
            {
                Border cell = new Border
                {
                    Width = 45,
                    Height = 45,
                    Margin = new Thickness(3),
                    CornerRadius = new CornerRadius(6),
                    BorderThickness = new Thickness(2)
                };

                // Define cores baseado no estado
                if (i == highlightIndex)
                {
                    cell.Background = new SolidColorBrush(Color.FromRgb(230, 126, 34)); // Laranja
                    cell.BorderBrush = new SolidColorBrush(Color.FromRgb(211, 84, 0));
                }
                else if (i == compareIndex)
                {
                    cell.Background = new SolidColorBrush(Color.FromRgb(52, 152, 219)); // Azul
                    cell.BorderBrush = new SolidColorBrush(Color.FromRgb(41, 128, 185));
                }
                else
                {
                    cell.Background = new SolidColorBrush(Colors.White);
                    cell.BorderBrush = new SolidColorBrush(Color.FromRgb(189, 195, 199));
                }

                TextBlock text = new TextBlock
                {
                    Text = currentArray[i].ToString(),
                    FontSize = 16,
                    FontWeight = FontWeights.Bold,
                    Foreground = (i == highlightIndex || i == compareIndex) ?
                        new SolidColorBrush(Colors.White) :
                        new SolidColorBrush(Color.FromRgb(44, 62, 80)),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };

                cell.Child = text;
                ArrayPanel.Children.Add(cell);
            }
        }

        private void AlgorithmComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AlgorithmComboBox == null || AlgorithmDescription == null) return;

            currentAlgorithm = AlgorithmComboBox.SelectedIndex;
            ResultPanel.Visibility = Visibility.Collapsed;

            switch (currentAlgorithm)
            {
                case 0: // Busca Linear
                    AlgorithmDescription.Text = "Percorre o array elemento por elemento procurando o valor desejado. Complexidade: O(n)";
                    InitializeArray();
                    break;
                case 1: // Busca Binária
                    AlgorithmDescription.Text = "Divide o array ao meio repetidamente. Requer array ordenado. Complexidade: O(log n)";
                    InitializeArray();
                    break;
                case 2: // Bubble Sort
                    AlgorithmDescription.Text = "Compara pares adjacentes e troca se estiverem fora de ordem. Complexidade: O(n²)";
                    currentArray = new List<int> { 64, 34, 25, 12, 22, 11, 90 };
                    UpdateArrayDisplay();
                    break;
                case 3: // Pilha
                    AlgorithmDescription.Text = "Insere elemento no topo da pilha (LIFO - Last In, First Out). Complexidade: O(1)";
                    currentArray = new List<int> { 10, 20, 30, 40 };
                    UpdateArrayDisplay();
                    break;
                case 4: // Fila
                    AlgorithmDescription.Text = "Insere elemento no fim da fila (FIFO - First In, First Out). Complexidade: O(1)";
                    currentArray = new List<int> { 10, 20, 30, 40 };
                    UpdateArrayDisplay();
                    break;
            }
        }

        private async void ExecuteButton_Click(object sender, RoutedEventArgs e)
        {
            ExecuteButton.IsEnabled = false;
            ResultPanel.Visibility = Visibility.Collapsed;

            switch (currentAlgorithm)
            {
                case 0:
                    await ExecuteLinearSearch();
                    break;
                case 1:
                    await ExecuteBinarySearch();
                    break;
                case 2:
                    await ExecuteBubbleSort();
                    break;
                case 3:
                    await ExecuteStackPush();
                    break;
                case 4:
                    await ExecuteQueueEnqueue();
                    break;
            }

            ExecuteButton.IsEnabled = true;
        }

        private async Task ExecuteLinearSearch()
        {
            if (!int.TryParse(SearchValueTextBox.Text, out int searchValue))
            {
                ShowResult("Por favor, insira um número válido!", false, "O(n)");
                return;
            }

            for (int i = 0; i < currentArray.Count; i++)
            {
                UpdateArrayDisplay(i);
                await Task.Delay(500);

                if (currentArray[i] == searchValue)
                {
                    ShowResult($"✓ Valor {searchValue} encontrado na posição {i}!", true, "O(n)");
                    return;
                }
            }

            UpdateArrayDisplay();
            ShowResult($"✗ Valor {searchValue} não encontrado no array.", false, "O(n)");
        }

        private async Task ExecuteBinarySearch()
        {
            if (!int.TryParse(SearchValueTextBox.Text, out int searchValue))
            {
                ShowResult("Por favor, insira um número válido!", false, "O(log n)");
                return;
            }

            int left = 0;
            int right = currentArray.Count - 1;

            while (left <= right)
            {
                int mid = (left + right) / 2;
                UpdateArrayDisplay(mid, -1);
                await Task.Delay(700);

                if (currentArray[mid] == searchValue)
                {
                    ShowResult($"✓ Valor {searchValue} encontrado na posição {mid}!", true, "O(log n)");
                    return;
                }

                if (currentArray[mid] < searchValue)
                    left = mid + 1;
                else
                    right = mid - 1;
            }

            UpdateArrayDisplay();
            ShowResult($"✗ Valor {searchValue} não encontrado no array.", false, "O(log n)");
        }

        private async Task ExecuteBubbleSort()
        {
            int n = currentArray.Count;
            bool swapped;

            for (int i = 0; i < n - 1; i++)
            {
                swapped = false;
                for (int j = 0; j < n - i - 1; j++)
                {
                    UpdateArrayDisplay(j, j + 1);
                    await Task.Delay(400);

                    if (currentArray[j] > currentArray[j + 1])
                    {
                        // Troca
                        int temp = currentArray[j];
                        currentArray[j] = currentArray[j + 1];
                        currentArray[j + 1] = temp;
                        swapped = true;

                        UpdateArrayDisplay(j, j + 1);
                        await Task.Delay(400);
                    }
                }

                if (!swapped) break;
            }

            UpdateArrayDisplay();
            ShowResult("✓ Array ordenado com sucesso!", true, "O(n²)");
        }

        private async Task ExecuteStackPush()
        {
            if (!int.TryParse(SearchValueTextBox.Text, out int newValue))
            {
                ShowResult("Por favor, insira um número válido!", false, "O(1)");
                return;
            }

            // Adiciona no final (topo da pilha)
            currentArray.Add(newValue);
            UpdateArrayDisplay(currentArray.Count - 1);
            await Task.Delay(500);

            ShowResult($"✓ Valor {newValue} inserido no topo da pilha!", true, "O(1)");
        }

        private async Task ExecuteQueueEnqueue()
        {
            if (!int.TryParse(SearchValueTextBox.Text, out int newValue))
            {
                ShowResult("Por favor, insira um número válido!", false, "O(1)");
                return;
            }

            // Adiciona no final da fila
            currentArray.Add(newValue);
            UpdateArrayDisplay(currentArray.Count - 1);
            await Task.Delay(500);

            ShowResult($"✓ Valor {newValue} inserido no fim da fila!", true, "O(1)");
        }

        private void ShowResult(string message, bool success, string complexity)
        {
            if (ResultPanel == null) return;

            ResultPanel.Visibility = Visibility.Visible;
            ResultText.Text = message;
            ComplexityText.Text = $"Complexidade: {complexity}";

            if (success)
            {
                ResultPanel.Background = new SolidColorBrush(Color.FromRgb(213, 244, 230));
                ResultPanel.BorderBrush = new SolidColorBrush(Color.FromRgb(39, 174, 96));
                ResultText.Foreground = new SolidColorBrush(Color.FromRgb(39, 174, 96));
            }
            else
            {
                ResultPanel.Background = new SolidColorBrush(Color.FromRgb(250, 219, 216));
                ResultPanel.BorderBrush = new SolidColorBrush(Color.FromRgb(231, 76, 60));
                ResultText.Foreground = new SolidColorBrush(Color.FromRgb(231, 76, 60));
            }
        }

        private void ResetArray_Click(object sender, RoutedEventArgs e)
        {
            ResultPanel.Visibility = Visibility.Collapsed;
            AlgorithmComboBox_SelectionChanged(null, null);
        }

        private void ExercisesButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var testeWindow = new ALESTeste();
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