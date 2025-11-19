using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Quantun_Forge.src.views.ModuloEnsino.Computacao.ComputacaoClassica.Testes
{
    public partial class bitsistemasbinariosteste : Window
    {
        // Classes auxiliares
        public class Question
        {
            public string Category { get; set; } = string.Empty;
            public string Text { get; set; } = string.Empty;
            public List<string> Options { get; set; } = new List<string>();
            public int CorrectAnswer { get; set; } // 1-4
            public string Hint { get; set; } = string.Empty;
            public string Explanation { get; set; } = string.Empty;
            public int Points { get; set; } = 10;
        }

        // Variáveis do jogo
        private List<Question> questions = new List<Question>();
        private int currentQuestionIndex = 0;
        private int score = 0;
        private int correctAnswers = 0;
        private int incorrectAnswers = 0;
        private int lives = 3;
        private int? selectedOption = null;
        private bool answerChecked = false;
        private bool hintUsed = false;

        // Timer
        private DispatcherTimer gameTimer = new DispatcherTimer();
        private TimeSpan elapsedTime = TimeSpan.Zero;

        // Constantes
        private const int TOTAL_QUESTIONS = 10;
        private const int POINTS_PER_QUESTION = 10;
        private const int HINT_PENALTY = 2;

        public bitsistemasbinariosteste()
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
                    Category = "💻 Conversão Binária",
                    Text = "Qual é o valor decimal do número binário 1010?",
                    Options = new List<string> { "A) 8", "B) 10", "C) 12", "D) 14" },
                    CorrectAnswer = 2,
                    Hint = "Lembre-se: cada posição representa uma potência de 2",
                    Explanation = "1×2³ + 0×2² + 1×2¹ + 0×2⁰ = 8 + 0 + 2 + 0 = 10"
                },
                new Question
                {
                    Category = "💻 Conversão Decimal",
                    Text = "Qual é a representação binária do número decimal 15?",
                    Options = new List<string> { "A) 1110", "B) 1111", "C) 1101", "D) 1011" },
                    CorrectAnswer = 2,
                    Hint = "15 é 8 + 4 + 2 + 1. Pense nas potências de 2",
                    Explanation = "15 = 8 + 4 + 2 + 1 = 2³ + 2² + 2¹ + 2⁰ = 1111₂"
                },
                new Question
                {
                    Category = "🔢 Conceitos Básicos",
                    Text = "O que representa um bit na computação?",
                    Options = new List<string>
                    {
                        "A) Um byte de informação",
                        "B) A menor unidade de informação (0 ou 1)",
                        "C) 8 unidades de dados",
                        "D) Um caractere de texto"
                    },
                    CorrectAnswer = 2,
                    Hint = "Pense na menor unidade possível de armazenamento",
                    Explanation = "Bit é a menor unidade de informação, podendo assumir apenas dois valores: 0 ou 1"
                },
                new Question
                {
                    Category = "🔢 Conceitos Básicos",
                    Text = "Quantos bits existem em 1 byte?",
                    Options = new List<string> { "A) 4 bits", "B) 8 bits", "C) 16 bits", "D) 32 bits" },
                    CorrectAnswer = 2,
                    Hint = "É uma potência de 2 entre 4 e 16",
                    Explanation = "1 byte = 8 bits. Esta é uma convenção padrão na computação"
                },
                new Question
                {
                    Category = "💻 Conversão Binária",
                    Text = "Qual é o valor decimal de 11111111 (8 bits todos em 1)?",
                    Options = new List<string> { "A) 128", "B) 255", "C) 256", "D) 512" },
                    CorrectAnswer = 2,
                    Hint = "É o maior número que pode ser representado com 8 bits",
                    Explanation = "2⁷ + 2⁶ + 2⁵ + 2⁴ + 2³ + 2² + 2¹ + 2⁰ = 128 + 64 + 32 + 16 + 8 + 4 + 2 + 1 = 255"
                },
                new Question
                {
                    Category = "🧮 Operações Binárias",
                    Text = "Qual é o resultado da operação binária 1010 + 0101?",
                    Options = new List<string> { "A) 1110", "B) 1111", "C) 1101", "D) 1011" },
                    CorrectAnswer = 2,
                    Hint = "Some como decimal (10 + 5) e converta de volta",
                    Explanation = "1010₂ = 10₁₀ e 0101₂ = 5₁₀. Logo, 10 + 5 = 15 = 1111₂"
                },
                new Question
                {
                    Category = "💻 Conversão Binária",
                    Text = "Qual é o valor binário do número decimal 32?",
                    Options = new List<string> { "A) 10000", "B) 100000", "C) 11111", "D) 101010" },
                    CorrectAnswer = 2,
                    Hint = "32 é uma potência de 2 (2⁵)",
                    Explanation = "32 = 2⁵ = 100000₂"
                },
                new Question
                {
                    Category = "🔢 Conceitos Básicos",
                    Text = "Quantos valores diferentes podem ser representados com 4 bits?",
                    Options = new List<string> { "A) 4", "B) 8", "C) 16", "D) 32" },
                    CorrectAnswer = 3,
                    Hint = "Calcule 2 elevado ao número de bits",
                    Explanation = "Com n bits podemos representar 2ⁿ valores. Logo, 2⁴ = 16 valores (0 a 15)"
                },
                new Question
                {
                    Category = "🧮 Operações Binárias",
                    Text = "Qual é o resultado de 1101 AND 1011 (operação E lógico)?",
                    Options = new List<string> { "A) 1001", "B) 1011", "C) 1101", "D) 1111" },
                    CorrectAnswer = 1,
                    Hint = "AND retorna 1 apenas quando ambos os bits são 1",
                    Explanation = "1101 AND 1011 = 1001 (bit a bit: 1&1=1, 1&0=0, 0&1=0, 1&1=1)"
                },
                new Question
                {
                    Category = "💻 Conversão Decimal",
                    Text = "Qual é a representação binária do número decimal 7?",
                    Options = new List<string> { "A) 101", "B) 110", "C) 111", "D) 100" },
                    CorrectAnswer = 3,
                    Hint = "7 = 4 + 2 + 1",
                    Explanation = "7 = 4 + 2 + 1 = 2² + 2¹ + 2⁰ = 111₂"
                }
            };

            // Embaralhar questões
            Random rnd = new Random();
            questions = questions.OrderBy(x => rnd.Next()).Take(TOTAL_QUESTIONS).ToList();
        }

        private void InitializeTimer()
        {
            gameTimer.Interval = TimeSpan.FromSeconds(1);
            gameTimer.Tick += GameTimer_Tick;
            gameTimer.Start();
        }

        private void GameTimer_Tick(object? sender, EventArgs e)
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

            // Resetar estado
            ResetQuestionState();

            // Atualizar barra de progresso
            UpdateProgress();
        }

        private void ResetQuestionState()
        {
            selectedOption = null;
            answerChecked = false;
            hintUsed = false;

            // Resetar estilos dos botões
            Option1.Style = (Style)FindResource("OptionButton");
            Option2.Style = (Style)FindResource("OptionButton");
            Option3.Style = (Style)FindResource("OptionButton");
            Option4.Style = (Style)FindResource("OptionButton");

            // Esconder painéis
            FeedbackPanel.Visibility = Visibility.Collapsed;
            HintPanel.Visibility = Visibility.Collapsed;
            ExplanationPanel.Visibility = Visibility.Collapsed;

            // Resetar botões
            CheckButton.Content = "VERIFICAR RESPOSTA";
            CheckButton.IsEnabled = false;
            HintButton.IsEnabled = true;
            SkipButton.IsEnabled = true;
        }

        private void Option_Click(object sender, RoutedEventArgs e)
        {
            if (answerChecked) return;

            Button? clickedButton = sender as Button;
            if (clickedButton == null) return;

            selectedOption = int.Parse(clickedButton.Tag.ToString() ?? "0");

            // Resetar estilos
            Option1.Style = (Style)FindResource("OptionButton");
            Option2.Style = (Style)FindResource("OptionButton");
            Option3.Style = (Style)FindResource("OptionButton");
            Option4.Style = (Style)FindResource("OptionButton");

            // Destacar selecionada
            clickedButton.BorderBrush = new SolidColorBrush(Color.FromRgb(230, 126, 34));
            clickedButton.BorderThickness = new Thickness(3);

            CheckButton.IsEnabled = true;
        }

        private void CheckAnswer_Click(object sender, RoutedEventArgs e)
        {
            if (!answerChecked)
            {
                CheckAnswer();
            }
            else
            {
                NextQuestion();
            }
        }

        private void CheckAnswer()
        {
            if (selectedOption == null) return;

            answerChecked = true;
            var question = questions[currentQuestionIndex];
            bool isCorrect = selectedOption == question.CorrectAnswer;

            // Desabilitar botões
            HintButton.IsEnabled = false;
            SkipButton.IsEnabled = false;

            // Mostrar resposta correta e incorreta
            Button? correctButton = GetButtonByTag(question.CorrectAnswer);
            Button? selectedButton = GetButtonByTag(selectedOption.Value);

            if (correctButton != null)
            {
                correctButton.Style = (Style)FindResource("CorrectButton");
            }

            if (!isCorrect && selectedButton != null)
            {
                selectedButton.Style = (Style)FindResource("IncorrectButton");
            }

            // Atualizar estatísticas
            if (isCorrect)
            {
                int points = POINTS_PER_QUESTION - (hintUsed ? HINT_PENALTY : 0);
                score += points;
                correctAnswers++;

                // Feedback positivo
                FeedbackPanel.Background = new SolidColorBrush(Color.FromRgb(213, 244, 230));
                FeedbackPanel.BorderBrush = new SolidColorBrush(Color.FromRgb(39, 174, 96));
                FeedbackIcon.Text = "✓";
                FeedbackIcon.Foreground = new SolidColorBrush(Color.FromRgb(39, 174, 96));
                FeedbackTitle.Text = $"Correto! +{points} pontos";
                FeedbackTitle.Foreground = new SolidColorBrush(Color.FromRgb(30, 132, 73));
                FeedbackMessage.Text = "Muito bem! Resposta correta.";
                FeedbackMessage.Foreground = new SolidColorBrush(Color.FromRgb(39, 174, 96));
            }
            else
            {
                lives--;
                incorrectAnswers++;

                // Feedback negativo
                FeedbackPanel.Background = new SolidColorBrush(Color.FromRgb(250, 219, 216));
                FeedbackPanel.BorderBrush = new SolidColorBrush(Color.FromRgb(231, 76, 60));
                FeedbackIcon.Text = "✗";
                FeedbackIcon.Foreground = new SolidColorBrush(Color.FromRgb(231, 76, 60));
                FeedbackTitle.Text = "Incorreto!";
                FeedbackTitle.Foreground = new SolidColorBrush(Color.FromRgb(192, 57, 43));
                FeedbackMessage.Text = $"A resposta correta é: {question.Options[question.CorrectAnswer - 1]}";
                FeedbackMessage.Foreground = new SolidColorBrush(Color.FromRgb(231, 76, 60));

                UpdateLives();

                if (lives <= 0)
                {
                    gameTimer.Stop();
                    ShowResults();
                    return;
                }
            }

            // Mostrar explicação
            ExplanationText.Text = question.Explanation;
            ExplanationPanel.Visibility = Visibility.Visible;
            ExplanationPanel.BorderBrush = isCorrect ?
                new SolidColorBrush(Color.FromRgb(39, 174, 96)) :
                new SolidColorBrush(Color.FromRgb(231, 76, 60));

            FeedbackPanel.Visibility = Visibility.Visible;

            // Atualizar pontuação
            UpdateScore();

            // Mudar botão para próxima
            CheckButton.Content = currentQuestionIndex < questions.Count - 1 ? "PRÓXIMA QUESTÃO" : "VER RESULTADO";
        }

        private void NextQuestion()
        {
            currentQuestionIndex++;
            LoadQuestion();
        }

        private void ShowHint_Click(object sender, RoutedEventArgs e)
        {
            if (answerChecked) return;

            var question = questions[currentQuestionIndex];
            HintText.Text = question.Hint;
            HintPanel.Visibility = Visibility.Visible;
            HintButton.IsEnabled = false;
            hintUsed = true;
        }

        private void SkipQuestion_Click(object sender, RoutedEventArgs e)
        {
            if (answerChecked) return;

            incorrectAnswers++;
            lives--;
            UpdateLives();

            if (lives <= 0)
            {
                gameTimer.Stop();
                ShowResults();
                return;
            }

            currentQuestionIndex++;
            LoadQuestion();
        }

        private void UpdateScore()
        {
            ScoreText.Text = $"{score} pontos";
            CorrectText.Text = $"{correctAnswers}/{TOTAL_QUESTIONS}";
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
                case 0:
                    LivesText.Text = "🖤 🖤 🖤";
                    break;
            }
        }

        private void UpdateProgress()
        {
            double progress = (double)currentQuestionIndex / TOTAL_QUESTIONS * 100;
            ProgressBar.Width = (this.ActualWidth - 60) * progress / 100;
            ProgressText.Text = $"{(int)progress}%";
        }

        private void ShowResults()
        {
            gameTimer.Stop();

            // Calcular estatísticas
            double accuracy = (double)correctAnswers / TOTAL_QUESTIONS * 100;

            // Atualizar UI
            FinalScoreText.Text = $"Pontuação: {score}/{TOTAL_QUESTIONS * POINTS_PER_QUESTION}";
            FinalCorrectText.Text = $"{correctAnswers}/{TOTAL_QUESTIONS}";
            FinalIncorrectText.Text = $"{incorrectAnswers}/{TOTAL_QUESTIONS}";
            AccuracyText.Text = $"{accuracy:F0}%";
            FinalTimeText.Text = elapsedTime.ToString(@"mm\:ss");

            // Mensagem de performance
            if (accuracy >= 90)
            {
                ResultIcon.Text = "🏆";
                ResultTitle.Text = "Excelente!";
                PerformanceMessage.Text = "Desempenho excepcional! Você domina o assunto!";
            }
            else if (accuracy >= 70)
            {
                ResultIcon.Text = "🎉";
                ResultTitle.Text = "Muito Bom!";
                PerformanceMessage.Text = "Bom desempenho! Continue praticando para melhorar ainda mais!";
            }
            else if (accuracy >= 50)
            {
                ResultIcon.Text = "👍";
                ResultTitle.Text = "Bom Trabalho!";
                PerformanceMessage.Text = "Você está no caminho certo! Revise o conteúdo e tente novamente.";
            }
            else
            {
                ResultIcon.Text = "📚";
                ResultTitle.Text = "Continue Estudando!";
                PerformanceMessage.Text = "Não desanime! Revise o material e pratique mais.";
            }

            ResultPanel.Visibility = Visibility.Visible;
        }

        private void RestartTest_Click(object sender, RoutedEventArgs e)
        {
            // Resetar variáveis
            currentQuestionIndex = 0;
            score = 0;
            correctAnswers = 0;
            incorrectAnswers = 0;
            lives = 3;
            elapsedTime = TimeSpan.Zero;

            // Reinicializar
            InitializeQuestions();
            UpdateLives();
            UpdateScore();

            ResultPanel.Visibility = Visibility.Collapsed;

            gameTimer.Start();
            LoadQuestion();
        }

        private void CloseTest_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private Button? GetButtonByTag(int tag)
        {
            switch (tag)
            {
                case 1: return Option1;
                case 2: return Option2;
                case 3: return Option3;
                case 4: return Option4;
                default: return null;
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            gameTimer?.Stop();
            base.OnClosed(e);
        }
    }
}