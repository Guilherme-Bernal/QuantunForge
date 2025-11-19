using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Quantun_Forge.src.views.ModuloEnsino.Computacao.ComputacaoClassica.Testes
{
    /// <summary>
    /// Lógica interna para ALESTeste.xaml
    /// </summary>
    public partial class ALESTeste : Window
    {
        // Classes para representar as questões
        private class Question
        {
            public string Category { get; set; }
            public string Text { get; set; }
            public string[] Options { get; set; }
            public int CorrectAnswer { get; set; } // 1-4
            public string Hint { get; set; }
            public string Explanation { get; set; }
        }

        // Variáveis do teste
        private List<Question> questions;
        private int currentQuestionIndex = 0;
        private int score = 0;
        private int correctAnswers = 0;
        private int lives = 3;
        private int selectedOption = -1;
        private bool questionAnswered = false;
        private DispatcherTimer timer;
        private TimeSpan elapsedTime = TimeSpan.Zero;
        private const int POINTS_PER_CORRECT = 10;
        private const int TOTAL_QUESTIONS = 10;

        public ALESTeste()
        {
            InitializeComponent();
            InitializeQuestions();
            InitializeTimer();
            LoadQuestion();
        }

        private void InitializeQuestions()
        {
            questions = new List<Question>
            {
                new Question
                {
                    Category = "📊 Complexidade de Algoritmos",
                    Text = "Qual é a complexidade de tempo da busca binária em um array ordenado?",
                    Options = new[] { "A) O(n)", "B) O(log n)", "C) O(n²)", "D) O(1)" },
                    CorrectAnswer = 2,
                    Hint = "A busca binária divide o problema pela metade a cada iteração",
                    Explanation = "A busca binária divide o espaço de busca pela metade a cada iteração, resultando em complexidade logarítmica O(log n)."
                },
                new Question
                {
                    Category = "🔍 Algoritmos de Busca",
                    Text = "Qual algoritmo de ordenação possui complexidade O(n log n) no caso médio?",
                    Options = new[] { "A) Bubble Sort", "B) Quick Sort", "C) Selection Sort", "D) Insertion Sort" },
                    CorrectAnswer = 2,
                    Hint = "Este algoritmo usa a estratégia de dividir para conquistar",
                    Explanation = "Quick Sort tem complexidade O(n log n) no caso médio, usando divisão e conquista para ordenar eficientemente."
                },
                new Question
                {
                    Category = "📚 Estruturas de Dados",
                    Text = "Qual estrutura de dados segue o princípio LIFO (Last In, First Out)?",
                    Options = new[] { "A) Fila", "B) Array", "C) Pilha", "D) Lista Encadeada" },
                    CorrectAnswer = 3,
                    Hint = "Pense em uma pilha de pratos",
                    Explanation = "A Pilha (Stack) segue o princípio LIFO, onde o último elemento inserido é o primeiro a ser removido."
                },
                new Question
                {
                    Category = "🌳 Árvores",
                    Text = "Em uma árvore binária de busca balanceada, qual é a complexidade de busca?",
                    Options = new[] { "A) O(n)", "B) O(log n)", "C) O(n²)", "D) O(1)" },
                    CorrectAnswer = 2,
                    Hint = "Árvores balanceadas mantêm altura logarítmica",
                    Explanation = "Em uma árvore binária de busca balanceada, a altura é O(log n), resultando em busca O(log n)."
                },
                new Question
                {
                    Category = "📥 Pilha e Fila",
                    Text = "Qual operação NÃO é típica de uma fila (Queue)?",
                    Options = new[] { "A) Enqueue", "B) Dequeue", "C) Pop", "D) Peek" },
                    CorrectAnswer = 3,
                    Hint = "Pop é uma operação de outra estrutura de dados",
                    Explanation = "Pop é uma operação da Pilha (Stack), não da Fila. Filas usam Enqueue e Dequeue."
                },
                new Question
                {
                    Category = "📊 Complexidade",
                    Text = "Qual é a complexidade do algoritmo Bubble Sort no pior caso?",
                    Options = new[] { "A) O(n)", "B) O(log n)", "C) O(n²)", "D) O(n log n)" },
                    CorrectAnswer = 3,
                    Hint = "Bubble Sort compara cada elemento com todos os outros",
                    Explanation = "Bubble Sort tem complexidade O(n²) no pior caso, pois usa dois loops aninhados."
                },
                new Question
                {
                    Category = "🔗 Lista Encadeada",
                    Text = "Qual é a vantagem principal de uma lista encadeada sobre um array?",
                    Options = new[]
                    {
                        "A) Acesso mais rápido aos elementos",
                        "B) Inserção e remoção eficientes",
                        "C) Menor uso de memória",
                        "D) Acesso aleatório O(1)"
                    },
                    CorrectAnswer = 2,
                    Hint = "Pense em como adicionar elementos no meio",
                    Explanation = "Listas encadeadas permitem inserção e remoção O(1) quando se tem a referência, sem necessidade de deslocar elementos."
                },
                new Question
                {
                    Category = "🕸️ Grafos",
                    Text = "Qual algoritmo é usado para encontrar o caminho mais curto em um grafo ponderado?",
                    Options = new[] { "A) DFS", "B) BFS", "C) Dijkstra", "D) Bubble Sort" },
                    CorrectAnswer = 3,
                    Hint = "Este algoritmo usa uma fila de prioridade",
                    Explanation = "O algoritmo de Dijkstra encontra o caminho mais curto em grafos com pesos positivos usando fila de prioridade."
                },
                new Question
                {
                    Category = "📊 Big O Notation",
                    Text = "Qual complexidade é melhor que O(n)?",
                    Options = new[] { "A) O(n²)", "B) O(2ⁿ)", "C) O(log n)", "D) O(n log n)" },
                    CorrectAnswer = 3,
                    Hint = "Procure a complexidade que cresce mais lentamente",
                    Explanation = "O(log n) é melhor que O(n) pois cresce muito mais lentamente. Por exemplo: log(1000) ≈ 10."
                },
                new Question
                {
                    Category = "🌳 Estruturas Hierárquicas",
                    Text = "Quantos filhos pode ter cada nó em uma árvore binária?",
                    Options = new[] { "A) Nenhum", "B) No máximo 1", "C) No máximo 2", "D) Ilimitados" },
                    CorrectAnswer = 3,
                    Hint = "A palavra 'binária' é uma dica importante",
                    Explanation = "Em uma árvore binária, cada nó pode ter no máximo 2 filhos (esquerdo e direito). Daí o nome 'binária'."
                }
            };

            // Embaralhar as questões para variedade
            questions = questions.OrderBy(x => Guid.NewGuid()).ToList();
        }

        private void InitializeTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            elapsedTime = elapsedTime.Add(TimeSpan.FromSeconds(1));
            TimerText.Text = elapsedTime.ToString(@"mm\:ss");
        }

        private void LoadQuestion()
        {
            if (currentQuestionIndex >= questions.Count)
            {
                ShowResults();
                return;
            }

            var question = questions[currentQuestionIndex];

            // Atualizar UI
            SubtitleText.Text = $"Questão {currentQuestionIndex + 1} de {TOTAL_QUESTIONS}";
            CategoryText.Text = question.Category;
            QuestionText.Text = question.Text;

            // Configurar opções
            Option1.Content = question.Options[0];
            Option2.Content = question.Options[1];
            Option3.Content = question.Options[2];
            Option4.Content = question.Options[3];

            // Resetar dica
            HintPanel.Visibility = Visibility.Collapsed;
            HintText.Text = question.Hint;

            // Resetar feedback
            FeedbackPanel.Visibility = Visibility.Collapsed;
            ExplanationPanel.Visibility = Visibility.Collapsed;

            // Resetar botões
            ResetButtons();

            // Atualizar progresso
            UpdateProgress();

            // Resetar estado
            selectedOption = -1;
            questionAnswered = false;
            CheckButton.Content = "VERIFICAR RESPOSTA";
            CheckButton.IsEnabled = false;
        }

        private void ResetButtons()
        {
            Option1.Style = (Style)FindResource("OptionButton");
            Option2.Style = (Style)FindResource("OptionButton");
            Option3.Style = (Style)FindResource("OptionButton");
            Option4.Style = (Style)FindResource("OptionButton");

            Option1.IsEnabled = true;
            Option2.IsEnabled = true;
            Option3.IsEnabled = true;
            Option4.IsEnabled = true;
        }

        private void UpdateProgress()
        {
            double progress = (double)currentQuestionIndex / TOTAL_QUESTIONS;
            ProgressBar.Width = ActualWidth > 0 ? (ActualWidth - 60) * progress : 0;
            ProgressText.Text = $"{(int)(progress * 100)}%";

            ScoreText.Text = $"{score} pontos";
            CorrectText.Text = $"{correctAnswers}/{TOTAL_QUESTIONS}";
            UpdateLives();
        }

        private void UpdateLives()
        {
            switch (lives)
            {
                case 3:
                    LivesText.Text = "❤️ ❤️ ❤️";
                    break;
                case 2:
                    LivesText.Text = "❤️ ❤️ 🖤";
                    break;
                case 1:
                    LivesText.Text = "❤️ 🖤 🖤";
                    break;
                default:
                    LivesText.Text = "🖤 🖤 🖤";
                    break;
            }
        }

        private void Option_Click(object sender, RoutedEventArgs e)
        {
            if (questionAnswered) return;

            var button = sender as Button;
            selectedOption = int.Parse(button.Tag.ToString());

            // Resetar visual de todos os botões
            ResetButtons();

            // Destacar botão selecionado
            button.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FEF5E7"));
            button.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E67E22"));

            // Habilitar botão de verificar
            CheckButton.IsEnabled = true;
        }

        private void CheckAnswer_Click(object sender, RoutedEventArgs e)
        {
            if (questionAnswered)
            {
                // Ir para próxima questão
                currentQuestionIndex++;
                LoadQuestion();
            }
            else
            {
                // Verificar resposta
                VerifyAnswer();
            }
        }

        private void VerifyAnswer()
        {
            if (selectedOption == -1) return;

            questionAnswered = true;
            var question = questions[currentQuestionIndex];
            bool isCorrect = selectedOption == question.CorrectAnswer;

            // Desabilitar botões
            Option1.IsEnabled = false;
            Option2.IsEnabled = false;
            Option3.IsEnabled = false;
            Option4.IsEnabled = false;

            // Mostrar resposta correta e incorreta
            Button correctButton = GetButtonByTag(question.CorrectAnswer);
            Button selectedButton = GetButtonByTag(selectedOption);

            correctButton.Style = (Style)FindResource("CorrectButton");

            if (!isCorrect)
            {
                selectedButton.Style = (Style)FindResource("IncorrectButton");
                lives--;
                UpdateLives();

                // Verificar game over
                if (lives <= 0)
                {
                    ShowResults();
                    return;
                }
            }
            else
            {
                correctAnswers++;
                score += POINTS_PER_CORRECT;
            }

            // Mostrar feedback
            ShowFeedback(isCorrect, question);

            // Mudar botão para "Próxima"
            CheckButton.Content = currentQuestionIndex < TOTAL_QUESTIONS - 1 ? "PRÓXIMA QUESTÃO ➔" : "VER RESULTADO";

            // Atualizar estatísticas
            UpdateProgress();
        }

        private Button GetButtonByTag(int tag)
        {
            switch (tag)
            {
                case 1: return Option1;
                case 2: return Option2;
                case 3: return Option3;
                case 4: return Option4;
                default: return Option1;
            }
        }

        private void ShowFeedback(bool isCorrect, Question question)
        {
            FeedbackPanel.Visibility = Visibility.Visible;
            ExplanationPanel.Visibility = Visibility.Visible;

            if (isCorrect)
            {
                FeedbackPanel.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D5F4E6"));
                FeedbackPanel.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#27AE60"));
                FeedbackIcon.Text = "✓";
                FeedbackIcon.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#27AE60"));
                FeedbackTitle.Text = $"Correto! +{POINTS_PER_CORRECT} pontos";
                FeedbackTitle.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E8449"));
                FeedbackMessage.Text = "Muito bem! Você acertou!";
                FeedbackMessage.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#27AE60"));

                ExplanationPanel.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#27AE60"));
            }
            else
            {
                FeedbackPanel.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FADBD8"));
                FeedbackPanel.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E74C3C"));
                FeedbackIcon.Text = "✗";
                FeedbackIcon.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E74C3C"));
                FeedbackTitle.Text = "Incorreto! -1 vida";
                FeedbackTitle.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#C0392B"));
                FeedbackMessage.Text = "Não foi dessa vez. Veja a explicação abaixo.";
                FeedbackMessage.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E74C3C"));

                ExplanationPanel.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E74C3C"));
            }

            ExplanationText.Text = question.Explanation;
        }

        private void ShowHint_Click(object sender, RoutedEventArgs e)
        {
            HintPanel.Visibility = HintPanel.Visibility == Visibility.Visible
                ? Visibility.Collapsed
                : Visibility.Visible;
        }

        private void SkipQuestion_Click(object sender, RoutedEventArgs e)
        {
            if (questionAnswered) return;

            lives--;
            UpdateLives();

            if (lives <= 0)
            {
                ShowResults();
                return;
            }

            currentQuestionIndex++;
            LoadQuestion();
        }

        private void ShowResults()
        {
            timer.Stop();

            ResultPanel.Visibility = Visibility.Visible;

            // Calcular estatísticas
            int incorrectAnswers = TOTAL_QUESTIONS - correctAnswers;
            double accuracy = (double)correctAnswers / TOTAL_QUESTIONS * 100;

            // Atualizar UI
            FinalScoreText.Text = $"Pontuação: {score}/{TOTAL_QUESTIONS * POINTS_PER_CORRECT}";
            FinalCorrectText.Text = $"{correctAnswers}/{TOTAL_QUESTIONS}";
            FinalIncorrectText.Text = $"{incorrectAnswers}/{TOTAL_QUESTIONS}";
            AccuracyText.Text = $"{accuracy:F0}%";
            FinalTimeText.Text = elapsedTime.ToString(@"mm\:ss");

            // Mensagem de performance
            if (accuracy >= 90)
            {
                ResultIcon.Text = "🏆";
                ResultTitle.Text = "Excepcional!";
                PerformanceMessage.Text = "Você domina Algoritmos e Estruturas de Dados! Parabéns!";
            }
            else if (accuracy >= 70)
            {
                ResultIcon.Text = "🎉";
                ResultTitle.Text = "Muito Bom!";
                PerformanceMessage.Text = "Excelente desempenho! Continue praticando!";
            }
            else if (accuracy >= 50)
            {
                ResultIcon.Text = "👍";
                ResultTitle.Text = "Bom Trabalho!";
                PerformanceMessage.Text = "Bom resultado! Revise os conceitos e tente novamente.";
            }
            else
            {
                ResultIcon.Text = "📚";
                ResultTitle.Text = "Continue Estudando!";
                PerformanceMessage.Text = "Não desanime! Revise o conteúdo e pratique mais.";
            }
        }

        private void RestartTest_Click(object sender, RoutedEventArgs e)
        {
            // Resetar variáveis
            currentQuestionIndex = 0;
            score = 0;
            correctAnswers = 0;
            lives = 3;
            elapsedTime = TimeSpan.Zero;

            // Reembaralhar questões
            questions = questions.OrderBy(x => Guid.NewGuid()).ToList();

            // Esconder resultado
            ResultPanel.Visibility = Visibility.Collapsed;

            // Reiniciar timer
            timer.Start();

            // Carregar primeira questão
            LoadQuestion();
        }

        private void CloseTest_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            timer?.Stop();
            base.OnClosed(e);
        }
    }
}