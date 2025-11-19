using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Quantun_Forge.src.views.ModuloEnsino.Matematica.Testes
{
    public partial class AlgebraLinearTesteWindow : Window
    {
        private List<Question> questions = new List<Question>();
        private int currentQuestionIndex = 0;
        private int score = 0;
        private int lives = 3;
        private int selectedOption = -1;
        private bool answered = false;

        public AlgebraLinearTesteWindow()
        {
            InitializeComponent();
            InitializeQuestions();
            LoadQuestion();
        }

        private void InitializeQuestions()
        {
            questions = new List<Question>
            {
                new Question
                {
                    Text = "Qual é o determinante da matriz:\n[2  3]\n[1  4]",
                    Options = new List<string> { "A) 3", "B) 5", "C) 8", "D) 11" },
                    CorrectAnswer = 2,
                    Explanation = "det(A) = (2×4) - (3×1) = 8 - 3 = 5"
                },
                new Question
                {
                    Text = "Qual é o resultado da multiplicação das matrizes:\n[1  2]  ×  [5]\n[3  4]      [6]",
                    Options = new List<string> { "A) [17, 39]", "B) [11, 23]", "C) [17, 23]", "D) [11, 39]" },
                    CorrectAnswer = 1,
                    Explanation = "Resultado: [1×5 + 2×6, 3×5 + 4×6] = [17, 39]"
                },
                new Question
                {
                    Text = "Qual é a transposta da matriz:\n[1  2  3]\n[4  5  6]",
                    Options = new List<string>
                    {
                        "A) [1  4]\n    [2  5]\n    [3  6]",
                        "B) [6  5  4]\n    [3  2  1]",
                        "C) [1  2  3]\n    [4  5  6]",
                        "D) [4  5  6]\n    [1  2  3]"
                    },
                    CorrectAnswer = 1,
                    Explanation = "A transposta inverte linhas por colunas"
                },
                new Question
                {
                    Text = "Vetores v = (3, 4) e w = (4, -3) são:",
                    Options = new List<string>
                    {
                        "A) Paralelos",
                        "B) Ortogonais (perpendiculares)",
                        "C) Linearmente dependentes",
                        "D) Colineares"
                    },
                    CorrectAnswer = 2,
                    Explanation = "v · w = 3×4 + 4×(-3) = 12 - 12 = 0, portanto são ortogonais"
                },
                new Question
                {
                    Text = "Qual é o posto (rank) da matriz:\n[1  2  3]\n[2  4  6]\n[1  2  3]",
                    Options = new List<string> { "A) 1", "B) 2", "C) 3", "D) 0" },
                    CorrectAnswer = 1,
                    Explanation = "Todas as linhas são múltiplas da primeira, então o posto é 1"
                },
                new Question
                {
                    Text = "Qual é o produto escalar (dot product) dos vetores v = (2, 3, 1) e w = (1, 0, 4)?",
                    Options = new List<string> { "A) 2", "B) 5", "C) 6", "D) 10" },
                    CorrectAnswer = 3,
                    Explanation = "v · w = 2×1 + 3×0 + 1×4 = 2 + 0 + 4 = 6"
                },
                new Question
                {
                    Text = "A matriz identidade 3×3 tem determinante igual a:",
                    Options = new List<string> { "A) 0", "B) 1", "C) 3", "D) 9" },
                    CorrectAnswer = 2,
                    Explanation = "O determinante da matriz identidade é sempre 1"
                },
                new Question
                {
                    Text = "Qual é a norma (módulo) do vetor v = (3, 4)?",
                    Options = new List<string> { "A) 5", "B) 7", "C) 12", "D) 25" },
                    CorrectAnswer = 1,
                    Explanation = "||v|| = √(3² + 4²) = √(9 + 16) = √25 = 5"
                },
                new Question
                {
                    Text = "Quantas soluções tem o sistema linear:\nx + y = 3\n2x + 2y = 6",
                    Options = new List<string>
                    {
                        "A) Nenhuma solução",
                        "B) Uma única solução",
                        "C) Infinitas soluções",
                        "D) Duas soluções"
                    },
                    CorrectAnswer = 3,
                    Explanation = "As equações são equivalentes (segunda é 2× a primeira), portanto infinitas soluções"
                },
                new Question
                {
                    Text = "Uma matriz 2×2 é invertível se e somente se:",
                    Options = new List<string>
                    {
                        "A) Seu determinante é zero",
                        "B) Seu determinante é diferente de zero",
                        "C) É simétrica",
                        "D) É diagonal"
                    },
                    CorrectAnswer = 2,
                    Explanation = "Uma matriz é invertível quando seu determinante é diferente de zero"
                }
            };

            // Embaralhar questões
            ShuffleQuestions();
        }

        private void ShuffleQuestions()
        {
            Random rng = new Random();
            int n = questions.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                var temp = questions[k];
                questions[k] = questions[n];
                questions[n] = temp;
            }

            // Pegar apenas 5 questões
            if (questions.Count > 5)
                questions = questions.GetRange(0, 5);
        }

        private void LoadQuestion()
        {
            if (currentQuestionIndex >= questions.Count)
            {
                ShowResults();
                return;
            }

            var question = questions[currentQuestionIndex];
            QuestionText.Text = question.Text;

            Option1.Content = question.Options[0];
            Option2.Content = question.Options[1];
            Option3.Content = question.Options[2];
            Option4.Content = question.Options[3];

            // Reset opções
            ResetOptions();
            selectedOption = -1;
            answered = false;
            CheckButton.IsEnabled = false;
            FeedbackPanel.Visibility = Visibility.Collapsed;

            // Atualizar progresso
            SubtitleText.Text = $"Questão {currentQuestionIndex + 1} de {questions.Count}";
            UpdateProgressBar();
        }

        private void ResetOptions()
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

        private void Option_Click(object sender, RoutedEventArgs e)
        {
            if (answered) return;

            var button = sender as Button;
            if (button?.Tag == null) return;

            selectedOption = int.Parse(button.Tag.ToString()!);

            // Reset visual de todas opções
            ResetOptions();

            // Destacar selecionada
            button.Style = (Style)FindResource("OptionButton");
            button.BorderBrush = new SolidColorBrush(Color.FromRgb(52, 152, 219));
            button.BorderThickness = new Thickness(3);

            CheckButton.IsEnabled = true;
        }

        private void CheckAnswer_Click(object sender, RoutedEventArgs e)
        {
            if (answered || selectedOption == -1) return;

            answered = true;
            var question = questions[currentQuestionIndex];
            bool isCorrect = selectedOption == question.CorrectAnswer;

            // Desabilitar todas opções
            Option1.IsEnabled = false;
            Option2.IsEnabled = false;
            Option3.IsEnabled = false;
            Option4.IsEnabled = false;

            // Mostrar resposta correta e incorreta
            var buttons = new[] { Option1, Option2, Option3, Option4 };

            if (isCorrect)
            {
                buttons[selectedOption - 1].Style = (Style)FindResource("CorrectButton");
                score += 20;
                ScoreText.Text = $"{score} pontos";

                FeedbackPanel.Background = new SolidColorBrush(Color.FromRgb(213, 244, 230));
                FeedbackPanel.BorderBrush = new SolidColorBrush(Color.FromRgb(39, 174, 96));
                FeedbackIcon.Text = "✓";
                FeedbackIcon.Foreground = new SolidColorBrush(Color.FromRgb(39, 174, 96));
                FeedbackTitle.Text = "Correto!";
                FeedbackTitle.Foreground = new SolidColorBrush(Color.FromRgb(30, 132, 73));
                FeedbackMessage.Text = $"Excelente! {question.Explanation}";
                FeedbackMessage.Foreground = new SolidColorBrush(Color.FromRgb(39, 174, 96));
            }
            else
            {
                buttons[selectedOption - 1].Style = (Style)FindResource("IncorrectButton");
                buttons[question.CorrectAnswer - 1].Style = (Style)FindResource("CorrectButton");

                lives--;
                UpdateLives();

                FeedbackPanel.Background = new SolidColorBrush(Color.FromRgb(250, 219, 216));
                FeedbackPanel.BorderBrush = new SolidColorBrush(Color.FromRgb(231, 76, 60));
                FeedbackIcon.Text = "✗";
                FeedbackIcon.Foreground = new SolidColorBrush(Color.FromRgb(231, 76, 60));
                FeedbackTitle.Text = "Incorreto";
                FeedbackTitle.Foreground = new SolidColorBrush(Color.FromRgb(192, 57, 43));
                FeedbackMessage.Text = $"A resposta correta era: {question.Options[question.CorrectAnswer - 1]}\n{question.Explanation}";
                FeedbackMessage.Foreground = new SolidColorBrush(Color.FromRgb(231, 76, 60));

                if (lives <= 0)
                {
                    MessageBox.Show("Você perdeu todas as vidas! O teste será encerrado.", "Game Over",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    ShowResults();
                    return;
                }
            }

            FeedbackPanel.Visibility = Visibility.Visible;
            CheckButton.Content = "PRÓXIMA QUESTÃO";
            CheckButton.Click -= CheckAnswer_Click;
            CheckButton.Click += NextQuestion_Click;
        }

        private void NextQuestion_Click(object sender, RoutedEventArgs e)
        {
            CheckButton.Click -= NextQuestion_Click;
            CheckButton.Click += CheckAnswer_Click;
            CheckButton.Content = "VERIFICAR RESPOSTA";

            currentQuestionIndex++;
            LoadQuestion();
        }

        private void SkipQuestion_Click(object sender, RoutedEventArgs e)
        {
            if (answered) return;

            var result = MessageBox.Show("Tem certeza que deseja pular esta questão? Você não ganhará pontos.",
                "Pular Questão", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                currentQuestionIndex++;
                LoadQuestion();
            }
        }

        private void UpdateLives()
        {
            string hearts = "";
            for (int i = 0; i < lives; i++)
                hearts += "❤️ ";
            for (int i = lives; i < 3; i++)
                hearts += "🖤 ";

            LivesText.Text = hearts.Trim();
        }

        private void UpdateProgressBar()
        {
            double progress = ((double)(currentQuestionIndex) / questions.Count) * 100;
            double width = (progress / 100) * 1040; // Largura total aproximada

            var animation = new DoubleAnimation
            {
                To = width,
                Duration = TimeSpan.FromMilliseconds(500),
                EasingFunction = new QuadraticEase()
            };

            ProgressBar.BeginAnimation(WidthProperty, animation);
            ProgressText.Text = $"{(int)progress}%";
        }

        private void ShowResults()
        {
            string performance;
            string message;

            double percentage = (double)score / (questions.Count * 20) * 100;

            if (percentage >= 80)
            {
                performance = "Excelente!";
                message = "Você domina bem os conceitos de Álgebra Linear!";
            }
            else if (percentage >= 60)
            {
                performance = "Bom!";
                message = "Você tem um bom entendimento de Álgebra Linear, mas pode melhorar.";
            }
            else if (percentage >= 40)
            {
                performance = "Regular";
                message = "É recomendado revisar os conceitos de Álgebra Linear.";
            }
            else
            {
                performance = "Precisa Melhorar";
                message = "Estude mais sobre Álgebra Linear antes de refazer o teste.";
            }

            MessageBox.Show($"{performance}\n\nPontuação Final: {score} pontos ({percentage:F1}%)\n{message}",
                "Resultado do Teste", MessageBoxButton.OK, MessageBoxImage.Information);

            this.Close();
        }

        private class Question
        {
            public string Text { get; set; } = "";
            public List<string> Options { get; set; } = new List<string>();
            public int CorrectAnswer { get; set; }
            public string Explanation { get; set; } = "";
        }
    }
}