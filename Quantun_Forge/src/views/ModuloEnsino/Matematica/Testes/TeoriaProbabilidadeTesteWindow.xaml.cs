using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Quantun_Forge.src.views.ModuloEnsino.Matematica.Testes
{
    public partial class TeoriaProbabilidadeTesteWindow : Window
    {
        private int currentQuestionIndex = 0;
        private int score = 0;
        private int lives = 3;
        private int correctAnswerIndex;
        private bool answerSelected = false;
        private Button? selectedButton = null;

        private List<QuizQuestion> questions = new List<QuizQuestion>
        {
            new QuizQuestion
             {
             QuestionText = "Em um estacionamento há 12 carros pretos, 8 carros brancos e 5 carros vermelhos. Qual a probabilidade de o próximo carro a sair ser branco?",
             QuestionOptions = new string[] { "A) 8/25", "B) 1/3", "C) 12/25", "D) 2/5" },
             QuestionCorrectAnswer = 1,
             QuestionExplanation = "Total de carros = 12 + 8 + 5 = 25. P(branco) = 8/25"
            },

            new QuizQuestion
            {
                QuestionText = "Ao lançar um dado de 6 faces, qual é a probabilidade de obter um número par?",
                QuestionOptions = new string[] { "A) 1/6", "B) 1/3", "C) 1/2", "D) 2/3" },
                QuestionCorrectAnswer = 3,
                QuestionExplanation = "Há 3 números pares (2, 4, 6) em 6 possibilidades, logo P = 3/6 = 1/2"
            },
            new QuizQuestion
            {
                QuestionText = "Qual é a probabilidade de retirar uma carta de copas de um baralho padrão de 52 cartas?",
                QuestionOptions = new string[] { "A) 1/52", "B) 1/13", "C) 1/4", "D) 1/2" },
                QuestionCorrectAnswer = 3,
                QuestionExplanation = "Há 13 cartas de copas em 52 cartas totais, logo P = 13/52 = 1/4"
            },
            new QuizQuestion
            {
             QuestionText = "No sistema Mercosul (ABC1D23), qual a probabilidade de uma placa aleatória ter todas as letras iguais e todos os números ímpares?",
             QuestionOptions = new string[] { "A) 1/17576", "B) 26/17576000", "C) 13/8788000", "D) 1/676000" },
             QuestionCorrectAnswer = 3,
             QuestionExplanation = "Letras iguais: 26 opções (AAA, BBB, etc). Números ímpares: 5³ = 125 (1,3,5,7,9). Total placas possíveis: 26³ × 10³ = 17576000. P = (26×125)/17576000 = 3250/17576000 = 13/70304... revisar"
          },
          new QuizQuestion
          {
            QuestionText = "Urna com 5 vermelhas e 3 azuis. Retira-se 1 bola. Se for vermelha, adiciona-se 2 vermelhas; se azul, adiciona-se 1 azul. Qual P(a segunda bola retirada ser vermelha)?",
            QuestionOptions = new string[] { "A) 59/90", "B) 5/8", "C) 31/45", "D) 2/3" },
            QuestionCorrectAnswer = 1,
            QuestionExplanation = "P(2ªV) = P(1ªV)×P(2ªV|1ªV) + P(1ªA)×P(2ªV|1ªA) = (5/8)×(6/9) + (3/8)×(5/9) = 30/72 + 15/72 = 45/72 = 5/8... recalcular com adição correta"
          }

        };

        public TeoriaProbabilidadeTesteWindow()
        {
            InitializeComponent();
            LoadQuestion();
            UpdateUI();
        }

        private void LoadQuestion()
        {
            if (currentQuestionIndex < questions.Count)
            {
                var question = questions[currentQuestionIndex];
                QuestionText.Text = question.QuestionText;

                Option1.Content = question.QuestionOptions[0];
                Option2.Content = question.QuestionOptions[1];
                Option3.Content = question.QuestionOptions[2];
                Option4.Content = question.QuestionOptions[3];

                correctAnswerIndex = question.QuestionCorrectAnswer;

                // Resetar estilos dos botões
                ResetButtonStyles();

                // Resetar seleção
                answerSelected = false;
                selectedButton = null;
                CheckButton.IsEnabled = false;

                // Esconder feedback
                FeedbackPanel.Visibility = Visibility.Collapsed;

                // Atualizar subtítulo
                SubtitleText.Text = $"Questão {currentQuestionIndex + 1} de {questions.Count}";

                // Atualizar barra de progresso
                UpdateProgressBar();
            }
            else
            {
                ShowFinalResults();
            }
        }

        private void Option_Click(object sender, RoutedEventArgs e)
        {
            if (answerSelected) return;

            // Resetar estilos anteriores
            ResetButtonStyles();

            // Selecionar novo botão
            selectedButton = sender as Button;
            if (selectedButton != null)
            {
                selectedButton.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3498DB"));
                selectedButton.BorderThickness = new Thickness(3);
                CheckButton.IsEnabled = true;
            }
        }

        private void CheckAnswer_Click(object sender, RoutedEventArgs e)
        {
            if (selectedButton == null || answerSelected) return;

            answerSelected = true;
            CheckButton.IsEnabled = false;

            int selectedOption = int.Parse(selectedButton.Tag?.ToString() ?? "0");

            if (selectedOption == correctAnswerIndex)
            {
                // Resposta correta
                selectedButton.Style = (Style)FindResource("CorrectButton");
                score += 20;
                ShowFeedback(true, questions[currentQuestionIndex].QuestionExplanation);
            }
            else
            {
                // Resposta incorreta
                selectedButton.Style = (Style)FindResource("IncorrectButton");
                lives--;

                // Mostrar resposta correta
                Button correctButton = GetButtonByTag(correctAnswerIndex);
                if (correctButton != null)
                {
                    correctButton.Style = (Style)FindResource("CorrectButton");
                }

                ShowFeedback(false, questions[currentQuestionIndex].QuestionExplanation);

                if (lives <= 0)
                {
                    ShowGameOver();
                    return;
                }
            }

            UpdateUI();

            // Avançar automaticamente após 3 segundos
            var timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += (s, args) =>
            {
                timer.Stop();
                currentQuestionIndex++;
                LoadQuestion();
            };
            timer.Start();
        }

        private void SkipQuestion_Click(object sender, RoutedEventArgs e)
        {
            if (answerSelected) return;

            currentQuestionIndex++;
            LoadQuestion();
        }

        private void ShowFeedback(bool isCorrect, string explanation)
        {
            FeedbackPanel.Visibility = Visibility.Visible;

            if (isCorrect)
            {
                FeedbackPanel.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D5F4E6"));
                FeedbackPanel.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#27AE60"));
                FeedbackIcon.Text = "✓";
                FeedbackIcon.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#27AE60"));
                FeedbackTitle.Text = "Correto!";
                FeedbackTitle.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E8449"));
                FeedbackMessage.Text = $"Muito bem! {explanation}";
                FeedbackMessage.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#27AE60"));
            }
            else
            {
                FeedbackPanel.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FADBD8"));
                FeedbackPanel.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E74C3C"));
                FeedbackIcon.Text = "✗";
                FeedbackIcon.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E74C3C"));
                FeedbackTitle.Text = "Incorreto!";
                FeedbackTitle.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#C0392B"));
                FeedbackMessage.Text = $"A resposta correta é: {explanation}";
                FeedbackMessage.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E74C3C"));
            }
        }

        private void UpdateUI()
        {
            ScoreText.Text = $"{score} pontos";

            string hearts = "";
            for (int i = 0; i < lives; i++)
            {
                hearts += "❤️ ";
            }
            for (int i = lives; i < 3; i++)
            {
                hearts += "🖤 ";
            }
            LivesText.Text = hearts.TrimEnd();
        }

        private void UpdateProgressBar()
        {
            double progress = ((double)(currentQuestionIndex + 1) / questions.Count) * 100;
            ProgressText.Text = $"{Math.Round(progress)}%";

            // Animar a barra de progresso
            DoubleAnimation animation = new DoubleAnimation
            {
                To = (this.ActualWidth - 60) * (progress / 100),
                Duration = TimeSpan.FromMilliseconds(500),
                EasingFunction = new QuadraticEase()
            };
            ProgressBar.BeginAnimation(WidthProperty, animation);
        }

        private void ResetButtonStyles()
        {
            Style optionStyle = (Style)FindResource("OptionButton");
            Option1.Style = optionStyle;
            Option2.Style = optionStyle;
            Option3.Style = optionStyle;
            Option4.Style = optionStyle;

            Option1.BorderThickness = new Thickness(2);
            Option2.BorderThickness = new Thickness(2);
            Option3.BorderThickness = new Thickness(2);
            Option4.BorderThickness = new Thickness(2);
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

        private void ShowFinalResults()
        {
            double percentage = ((double)score / (questions.Count * 20)) * 100;
            string message = "";
            string title = "";

            if (percentage >= 80)
            {
                title = "Excelente!";
                message = $"Parabéns! Você obteve {score} pontos ({percentage:F0}%).\nVocê domina bem a Teoria da Probabilidade!";
            }
            else if (percentage >= 60)
            {
                title = "Bom trabalho!";
                message = $"Você obteve {score} pontos ({percentage:F0}%).\nContinue praticando para melhorar ainda mais!";
            }
            else
            {
                title = "Continue estudando!";
                message = $"Você obteve {score} pontos ({percentage:F0}%).\nRevise os conceitos de probabilidade e tente novamente!";
            }

            MessageBoxResult result = MessageBox.Show(
                message + "\n\nDeseja tentar novamente?",
                title,
                MessageBoxButton.YesNo,
                MessageBoxImage.Information
            );

            if (result == MessageBoxResult.Yes)
            {
                RestartTest();
            }
            else
            {
                this.Close();
            }
        }

        private void ShowGameOver()
        {
            MessageBoxResult result = MessageBox.Show(
                $"Você perdeu todas as vidas!\nPontuação final: {score} pontos\n\nDeseja tentar novamente?",
                "Game Over",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            );

            if (result == MessageBoxResult.Yes)
            {
                RestartTest();
            }
            else
            {
                this.Close();
            }
        }

        private void RestartTest()
        {
            currentQuestionIndex = 0;
            score = 0;
            lives = 3;
            answerSelected = false;
            selectedButton = null;
            LoadQuestion();
            UpdateUI();
        }
    }

    public class QuizQuestion
    {
        public string QuestionText { get; set; } = string.Empty;
        public string[] QuestionOptions { get; set; } = Array.Empty<string>();
        public int QuestionCorrectAnswer { get; set; }
        public string QuestionExplanation { get; set; } = string.Empty;
    }
}