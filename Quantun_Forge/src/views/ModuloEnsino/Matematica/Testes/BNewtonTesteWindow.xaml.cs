using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Quantun_Forge.src.views.ModuloEnsino.Matematica.Testes
{
    public partial class BNewtonTesteWindow : Window
    {
        #region Constantes
        private const int INITIAL_LIVES = 3;
        private const double ANIMATION_DURATION = 0.3;
        private const double SCORE_SCALE_FACTOR = 1.3;
        #endregion

        #region Propriedades
        private List<Question> questions;
        private int currentQuestionIndex = 0;
        private int selectedOption = -1;
        private int score = 0;
        private int lives = INITIAL_LIVES;
        private int correctAnswers = 0;
        #endregion

        #region Construtor e Inicialização
        public BNewtonTesteWindow()
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
                    Text = "Qual é o valor de C(5, 2)?",
                    Options = new List<string> { "A) 5", "B) 10", "C) 20", "D) 25" },
                    CorrectAnswer = 2,
                    Explanation = "C(5,2) = 5!/(2!·3!) = (5·4)/(2·1) = 10",
                    Points = 100
                },
                new Question
                {
                    Text = "Quantos termos tem a expansão de (x + a)⁴?",
                    Options = new List<string> { "A) 3 termos", "B) 4 termos", "C) 5 termos", "D) 6 termos" },
                    CorrectAnswer = 3,
                    Explanation = "A expansão sempre tem n + 1 termos. Para n=4, temos 5 termos.",
                    Points = 100
                },
                new Question
                {
                    Text = "Qual é a soma dos coeficientes da expansão de (x + a)³?",
                    Options = new List<string> { "A) 3", "B) 6", "C) 8", "D) 9" },
                    CorrectAnswer = 3,
                    Explanation = "A soma dos coeficientes é sempre 2ⁿ. Para n=3: 2³ = 8",
                    Points = 150
                },
                new Question
                {
                    Text = "Na linha 4 do Triângulo de Pascal, qual é o terceiro número?",
                    Options = new List<string> { "A) 4", "B) 6", "C) 10", "D) 3" },
                    CorrectAnswer = 2,
                    Explanation = "Linha 4: 1  4  6  4  1. O terceiro número é 6 (C(4,2))",
                    Points = 150
                },
                new Question
                {
                    Text = "Qual é o termo central da expansão de (x + 2)⁴?",
                    Options = new List<string> { "A) 24x²", "B) 32x²", "C) 48x²", "D) 16x²" },
                    CorrectAnswer = 1,
                    Explanation = "Termo central (k=2): C(4,2)·x²·2² = 6·x²·4 = 24x²",
                    Points = 200
                }
            };
        }
        #endregion

        #region Controles da Janela
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                try
                {
                    this.DragMove();
                }
                catch (InvalidOperationException)
                {
                    // Ignora se a janela não puder ser movida
                }
            }
        }

        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            if (ConfirmExit())
            {
                this.Close();
            }
        }

        private bool ConfirmExit()
        {
            var result = MessageBox.Show(
                "Tem certeza que deseja sair? Seu progresso será perdido.",
                "Confirmar Saída",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            return result == MessageBoxResult.Yes;
        }
        #endregion

        #region Carregamento de Questões
        private void LoadQuestion()
        {
            if (currentQuestionIndex >= questions.Count)
            {
                ShowResults();
                return;
            }

            var question = questions[currentQuestionIndex];

            UpdateQuestionUI(question);
            ResetOptionButtons();
            UpdateQuestionInfo();
            HideFeedback();
        }

        private void UpdateQuestionUI(Question question)
        {
            if (QuestionText == null || Option1 == null || Option2 == null ||
                Option3 == null || Option4 == null) return;

            QuestionText.Text = question.Text;
            Option1.Content = question.Options[0];
            Option2.Content = question.Options[1];
            Option3.Content = question.Options[2];
            Option4.Content = question.Options[3];
        }

        private void ResetOptionButtons()
        {
            var optionStyle = FindResource("OptionButton") as Style;
            if (optionStyle == null) return;

            SetButtonState(Option1, optionStyle, true);
            SetButtonState(Option2, optionStyle, true);
            SetButtonState(Option3, optionStyle, true);
            SetButtonState(Option4, optionStyle, true);
        }

        private void SetButtonState(Button button, Style style, bool enabled)
        {
            if (button == null) return;
            button.Style = style;
            button.IsEnabled = enabled;
        }

        private void UpdateQuestionInfo()
        {
            if (SubtitleText != null)
                SubtitleText.Text = $"Questão {currentQuestionIndex + 1} de {questions.Count}";

            UpdateProgress();
        }

        private void HideFeedback()
        {
            if (FeedbackPanel != null)
                FeedbackPanel.Visibility = Visibility.Collapsed;

            selectedOption = -1;

            if (CheckButton != null)
                CheckButton.IsEnabled = false;
        }
        #endregion

        #region Manipulação de Respostas
        private void Option_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag == null) return;

            if (int.TryParse(button.Tag.ToString(), out int option))
            {
                selectedOption = option;
                ResetOptionButtons();
                HighlightSelectedButton(button);

                if (CheckButton != null)
                    CheckButton.IsEnabled = true;
            }
        }

        private void HighlightSelectedButton(Button button)
        {
            button.BorderBrush = new SolidColorBrush(Color.FromRgb(52, 152, 219));
            button.BorderThickness = new Thickness(3);
        }

        private void CheckAnswer_Click(object sender, RoutedEventArgs e)
        {
            if (selectedOption == -1 || currentQuestionIndex >= questions.Count) return;

            var question = questions[currentQuestionIndex];
            bool isCorrect = selectedOption == question.CorrectAnswer;

            DisableAllOptions();
            HighlightAnswers(question.CorrectAnswer, selectedOption);

            ProcessAnswer(isCorrect, question);
            UpdateCheckButton();
        }

        private void DisableAllOptions()
        {
            if (Option1 != null) Option1.IsEnabled = false;
            if (Option2 != null) Option2.IsEnabled = false;
            if (Option3 != null) Option3.IsEnabled = false;
            if (Option4 != null) Option4.IsEnabled = false;
        }

        private void ProcessAnswer(bool isCorrect, Question question)
        {
            if (isCorrect)
            {
                correctAnswers++;
                score += question.Points;
                ShowFeedback(true, question.Explanation);
                AnimateScore();
                UpdateScoreDisplay();
            }
            else
            {
                lives--;
                UpdateLives();
                ShowFeedback(false, question.Explanation);

                if (lives <= 0)
                {
                    GameOver();
                    return;
                }
            }
        }

        private void UpdateCheckButton()
        {
            if (CheckButton == null) return;

            CheckButton.Content = "PRÓXIMA QUESTÃO";
            CheckButton.Click -= CheckAnswer_Click;
            CheckButton.Click += NextQuestion_Click;
        }

        private void UpdateScoreDisplay()
        {
            if (ScoreText != null)
                ScoreText.Text = $"{score} pontos";
        }

        private void HighlightAnswers(int correctAnswer, int selectedAnswer)
        {
            var correctStyle = FindResource("CorrectButton") as Style;
            var incorrectStyle = FindResource("IncorrectButton") as Style;

            if (correctStyle == null || incorrectStyle == null) return;

            Button correctButton = GetButtonByTag(correctAnswer);
            if (correctButton != null)
                correctButton.Style = correctStyle;

            if (selectedAnswer != correctAnswer)
            {
                Button incorrectButton = GetButtonByTag(selectedAnswer);
                if (incorrectButton != null)
                    incorrectButton.Style = incorrectStyle;
            }
        }

        private Button GetButtonByTag(int tag)
        {
            return tag switch
            {
                1 => Option1,
                2 => Option2,
                3 => Option3,
                4 => Option4,
                _ => Option1
            };
        }
        #endregion

        #region Feedback e Animações
        private void ShowFeedback(bool isCorrect, string explanation)
        {
            if (FeedbackPanel == null || FeedbackIcon == null ||
                FeedbackTitle == null || FeedbackMessage == null) return;

            FeedbackPanel.Visibility = Visibility.Visible;

            if (isCorrect)
            {
                ConfigureFeedbackCorrect(explanation);
            }
            else
            {
                ConfigureFeedbackIncorrect(explanation);
            }

            AnimateFeedback();
        }

        private void ConfigureFeedbackCorrect(string explanation)
        {
            FeedbackPanel.Background = new SolidColorBrush(Color.FromRgb(213, 244, 230));
            FeedbackPanel.BorderBrush = new SolidColorBrush(Color.FromRgb(39, 174, 96));
            FeedbackIcon.Text = "✓";
            FeedbackIcon.Foreground = new SolidColorBrush(Color.FromRgb(39, 174, 96));
            FeedbackTitle.Text = "Correto!";
            FeedbackTitle.Foreground = new SolidColorBrush(Color.FromRgb(30, 132, 73));
            FeedbackMessage.Text = explanation;
            FeedbackMessage.Foreground = new SolidColorBrush(Color.FromRgb(39, 174, 96));
        }

        private void ConfigureFeedbackIncorrect(string explanation)
        {
            if (currentQuestionIndex >= questions.Count) return;

            FeedbackPanel.Background = new SolidColorBrush(Color.FromRgb(250, 219, 216));
            FeedbackPanel.BorderBrush = new SolidColorBrush(Color.FromRgb(231, 76, 60));
            FeedbackIcon.Text = "✗";
            FeedbackIcon.Foreground = new SolidColorBrush(Color.FromRgb(231, 76, 60));
            FeedbackTitle.Text = "Incorreto";
            FeedbackTitle.Foreground = new SolidColorBrush(Color.FromRgb(192, 57, 43));
            FeedbackMessage.Text = $"A resposta correta é a opção {GetOptionLetter(questions[currentQuestionIndex].CorrectAnswer)}. {explanation}";
            FeedbackMessage.Foreground = new SolidColorBrush(Color.FromRgb(231, 76, 60));
        }

        private string GetOptionLetter(int option)
        {
            return option switch
            {
                1 => "A",
                2 => "B",
                3 => "C",
                4 => "D",
                _ => "A"
            };
        }

        private void AnimateFeedback()
        {
            if (FeedbackPanel == null) return;

            var animation = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromSeconds(ANIMATION_DURATION)
            };
            FeedbackPanel.BeginAnimation(OpacityProperty, animation);
        }

        private void AnimateScore()
        {
            if (ScoreText == null) return;

            var scaleTransform = new ScaleTransform(1, 1);
            ScoreText.RenderTransform = scaleTransform;
            ScoreText.RenderTransformOrigin = new Point(0.5, 0.5);

            var animation = new DoubleAnimation
            {
                From = 1,
                To = SCORE_SCALE_FACTOR,
                Duration = TimeSpan.FromSeconds(0.2),
                AutoReverse = true
            };

            scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, animation);
            scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, animation);
        }
        #endregion

        #region Navegação
        private void NextQuestion_Click(object sender, RoutedEventArgs e)
        {
            currentQuestionIndex++;

            if (CheckButton != null)
            {
                CheckButton.Content = "VERIFICAR RESPOSTA";
                CheckButton.Click -= NextQuestion_Click;
                CheckButton.Click += CheckAnswer_Click;
            }

            LoadQuestion();
        }

        private void SkipQuestion_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Deseja pular esta questão? Você não ganhará pontos.",
                "Pular Questão",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                currentQuestionIndex++;
                LoadQuestion();
            }
        }
        #endregion

        #region Sistema de Progresso
        private void UpdateProgress()
        {
            if (ProgressBar == null || ProgressText == null || questions == null || questions.Count == 0)
                return;

            double progress = ((double)currentQuestionIndex / questions.Count) * 100;
            double windowWidth = this.ActualWidth > 0 ? this.ActualWidth : 1100;
            double width = (windowWidth - 60) * (progress / 100);

            var animation = new DoubleAnimation
            {
                To = width,
                Duration = TimeSpan.FromSeconds(0.5),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };

            ProgressBar.BeginAnimation(WidthProperty, animation);
            ProgressText.Text = $"{(int)progress}%";
        }

        private void UpdateLives()
        {
            if (LivesText == null) return;

            string hearts = string.Join(" ", Enumerable.Repeat("❤️", lives));
            string broken = string.Join(" ", Enumerable.Repeat("🖤", INITIAL_LIVES - lives));
            LivesText.Text = hearts + (lives < INITIAL_LIVES ? " " + broken : "");
        }
        #endregion

        #region Finalização
        private void ShowResults()
        {
            if (questions == null || questions.Count == 0) return;

            double percentage = ((double)correctAnswers / questions.Count) * 100;
            string grade = GetGrade(percentage);

            MessageBox.Show(
                $"Teste Concluído!\n\n" +
                $"Acertos: {correctAnswers}/{questions.Count}\n" +
                $"Pontuação Final: {score} pontos\n" +
                $"Desempenho: {percentage:F1}%\n" +
                $"Nota: {grade}",
                "Resultado Final",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            this.Close();
        }

        private void GameOver()
        {
            MessageBox.Show(
                $"Game Over!\n\n" +
                $"Você perdeu todas as vidas.\n" +
                $"Acertos: {correctAnswers}/{currentQuestionIndex + 1}\n" +
                $"Pontuação Final: {score} pontos",
                "Fim do Teste",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            this.Close();
        }

        private string GetGrade(double percentage)
        {
            return percentage switch
            {
                >= 90 => "A+ (Excelente!)",
                >= 80 => "A (Muito Bom!)",
                >= 70 => "B (Bom)",
                >= 60 => "C (Regular)",
                _ => "D (Precisa Melhorar)"
            };
        }
        #endregion
    }

    #region Classe Question
    public class Question
    {
        public string Text { get; set; }
        public List<string> Options { get; set; }
        public int CorrectAnswer { get; set; }
        public string Explanation { get; set; }
        public int Points { get; set; }
    }
    #endregion
}