using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace Quantun_Forge.src.views.ModuloEnsino.Computacao.ComputacaoClassica.Testes
{
    public partial class PLABTeste : Window
    {
        // Classe para representar uma questão
        private class Question
        {
            public string Category { get; set; }
            public string Difficulty { get; set; } // Fácil, Médio, Difícil, Avançado
            public string QuestionText { get; set; }
            public string[] Options { get; set; }
            public int CorrectAnswer { get; set; } // 1-4
            public string Explanation { get; set; }
            public string Hint { get; set; }
            public string Diagram { get; set; } // Opcional
            public int Points { get; set; }
        }

        private List<Question> questions;
        private int currentQuestionIndex = 0;
        private int score = 0;
        private int correctAnswers = 0;
        private int incorrectAnswers = 0;
        private int lives = 3;
        private int selectedOption = 0;
        private bool answered = false;
        private bool hintUsed = false;

        // Timer
        private DispatcherTimer timer;
        private TimeSpan elapsedTime;

        public PLABTeste()
        {
            InitializeComponent();
            InitializeQuestions();
            InitializeTimer();
            Loaded += PLABTeste_Loaded;
        }

        private void PLABTeste_Loaded(object sender, RoutedEventArgs e)
        {
            LoadQuestion();
        }

        /// <summary>
        /// Inicializa as 15 questões (do básico ao avançado)
        /// </summary>
        private void InitializeQuestions()
        {
            questions = new List<Question>
            {
                // ===== NÍVEL FÁCIL (Questões 1-5) =====
                new Question
                {
                    Category = "⚡ Portas Básicas",
                    Difficulty = "Fácil",
                    QuestionText = "Qual é o resultado da operação AND entre 1 e 1?",
                    Options = new[] { "A) 0", "B) 1", "C) 2", "D) Indefinido" },
                    CorrectAnswer = 2,
                    Explanation = "A porta AND retorna 1 apenas quando TODAS as entradas são 1. Como ambas as entradas (1 e 1) são verdadeiras, o resultado é 1.",
                    Hint = "A porta AND só retorna 1 quando TODAS as entradas são 1",
                    Diagram = "A(1) ──┐\n       │ AND ──── 1\nB(1) ──┘",
                    Points = 10
                },

                new Question
                {
                    Category = "⚡ Portas Básicas",
                    Difficulty = "Fácil",
                    QuestionText = "Qual porta lógica inverte o sinal de entrada?",
                    Options = new[] { "A) AND", "B) OR", "C) NOT", "D) XOR" },
                    CorrectAnswer = 3,
                    Explanation = "A porta NOT (ou inversor) inverte o valor da entrada: se a entrada é 0, a saída é 1, e vice-versa.",
                    Hint = "Essa porta também é conhecida como 'inversor'",
                    Points = 10
                },

                new Question
                {
                    Category = "⚡ Portas Básicas",
                    Difficulty = "Fácil",
                    QuestionText = "Qual é o resultado da operação OR entre 0 e 1?",
                    CorrectAnswer = 2,
                    Explanation = "A porta OR retorna 1 quando PELO MENOS UMA das entradas é 1. Como uma das entradas é 1, o resultado é 1.",
                    Hint = "A porta OR precisa de pelo menos uma entrada igual a 1",
                    Diagram = "A(0) ──┐\n       │ OR ──── 1\nB(1) ──┘",
                    Points = 10
                },

                new Question
                {
                    Category = "⚡ Portas Básicas",
                    Difficulty = "Fácil",
                    QuestionText = "O que significa o resultado 0 em uma porta lógica?",
                    Options = new[] { "A) Verdadeiro", "B) Falso", "C) Nulo", "D) Erro" },
                    CorrectAnswer = 2,
                    Explanation = "Na lógica digital, 0 representa FALSO e 1 representa VERDADEIRO. Essa convenção é fundamental em toda computação.",
                    Hint = "Pense na lógica booleana: 0 e 1 representam valores lógicos",
                    Points = 10
                },

                new Question
                {
                    Category = "⚡ Portas Básicas",
                    Difficulty = "Fácil",
                    QuestionText = "Qual é o resultado de NOT(0)?",
                    Options = new[] { "A) 0", "B) 1", "C) -1", "D) NULL" },
                    CorrectAnswer = 2,
                    Explanation = "A porta NOT inverte o bit: NOT(0) = 1 e NOT(1) = 0. É uma operação fundamental na álgebra booleana.",
                    Hint = "NOT inverte o valor do bit",
                    Points = 10
                },

                // ===== NÍVEL MÉDIO (Questões 6-10) =====
                new Question
                {
                    Category = "🔧 Portas Compostas",
                    Difficulty = "Médio",
                    QuestionText = "Qual é o resultado da operação NAND(1, 1)?",
                    Options = new[] { "A) 0", "B) 1", "C) 2", "D) Indefinido" },
                    CorrectAnswer = 1,
                    Explanation = "NAND é a negação do AND. Como AND(1,1) = 1, então NAND(1,1) = NOT(1) = 0. A porta NAND inverte o resultado do AND.",
                    Hint = "NAND = NOT AND. Primeiro calcule o AND, depois inverta",
                    Diagram = "A(1) ──┐\n       │ NAND ──── 0\nB(1) ──┘",
                    Points = 15
                },

                new Question
                {
                    Category = "🔧 Portas Compostas",
                    Difficulty = "Médio",
                    QuestionText = "Qual porta retorna 1 apenas quando as entradas são DIFERENTES?",
                    Options = new[] { "A) AND", "B) OR", "C) XOR", "D) NAND" },
                    CorrectAnswer = 3,
                    Explanation = "A porta XOR (OU Exclusivo) retorna 1 quando as entradas são diferentes: XOR(0,1)=1, XOR(1,0)=1, mas XOR(0,0)=0 e XOR(1,1)=0.",
                    Hint = "XOR significa 'OU Exclusivo' - exclusivo porque não inclui o caso onde ambos são 1",
                    Points = 15
                },

                new Question
                {
                    Category = "🔧 Portas Compostas",
                    Difficulty = "Médio",
                    QuestionText = "Qual é o resultado de XOR(1, 1)?",
                    Options = new[] { "A) 0", "B) 1", "C) 2", "D) Erro" },
                    CorrectAnswer = 1,
                    Explanation = "XOR retorna 1 apenas quando as entradas são diferentes. Como ambas são 1 (iguais), o resultado é 0.",
                    Hint = "XOR retorna 1 quando os bits são DIFERENTES",
                    Diagram = "A(1) ──┐\n       │ XOR ──── 0\nB(1) ──┘",
                    Points = 15
                },

                new Question
                {
                    Category = "🔧 Portas Compostas",
                    Difficulty = "Médio",
                    QuestionText = "Qual porta é a negação da porta OR?",
                    Options = new[] { "A) NAND", "B) NOR", "C) XNOR", "D) NOT" },
                    CorrectAnswer = 2,
                    Explanation = "NOR = NOT OR. A porta NOR inverte o resultado da porta OR. NOR retorna 1 apenas quando todas as entradas são 0.",
                    Hint = "Procure pela porta que tem 'NOR' no nome",
                    Points = 15
                },

                new Question
                {
                    Category = "🔧 Portas Compostas",
                    Difficulty = "Médio",
                    QuestionText = "Qual é o resultado de NOR(0, 0)?",
                    Options = new[] { "A) 0", "B) 1", "C) Indefinido", "D) Erro" },
                    CorrectAnswer = 2,
                    Explanation = "NOR = NOT OR. Como OR(0,0) = 0, então NOR(0,0) = NOT(0) = 1. A porta NOR retorna 1 apenas quando todas as entradas são 0.",
                    Hint = "NOR = NOT OR. Primeiro calcule OR(0,0), depois inverta",
                    Diagram = "A(0) ──┐\n       │ NOR ──── 1\nB(0) ──┘",
                    Points = 15
                },

                // ===== NÍVEL DIFÍCIL (Questões 11-13) =====
                new Question
                {
                    Category = "📚 Álgebra Booleana",
                    Difficulty = "Difícil",
                    QuestionText = "De acordo com a Lei de De Morgan, NOT(A AND B) é equivalente a:",
                    Options = new[] { "A) NOT(A) AND NOT(B)", "B) NOT(A) OR NOT(B)", "C) A OR B", "D) A AND B" },
                    CorrectAnswer = 2,
                    Explanation = "Lei de De Morgan: NOT(A AND B) = NOT(A) OR NOT(B). Esta é uma das leis fundamentais da álgebra booleana e é muito usada em simplificação de circuitos.",
                    Hint = "A Lei de De Morgan transforma AND em OR (e vice-versa) quando negamos a expressão",
                    Points = 20
                },

                new Question
                {
                    Category = "📚 Álgebra Booleana",
                    Difficulty = "Difícil",
                    QuestionText = "Qual é o resultado da expressão: (1 AND 0) OR (1 AND 1)?",
                    Options = new[] { "A) 0", "B) 1", "C) 2", "D) Indefinido" },
                    CorrectAnswer = 2,
                    Explanation = "Resolva passo a passo: (1 AND 0) = 0, (1 AND 1) = 1, então 0 OR 1 = 1. A ordem de operação é importante: primeiro AND, depois OR.",
                    Hint = "Resolva primeiro as operações dentro dos parênteses, depois o OR",
                    Points = 20
                },

                new Question
                {
                    Category = "📚 Álgebra Booleana",
                    Difficulty = "Difícil",
                    QuestionText = "Qual lei afirma que A OR 0 = A?",
                    Options = new[] { "A) Lei do Complemento", "B) Lei da Identidade", "C) Lei de De Morgan", "D) Lei Distributiva" },
                    CorrectAnswer = 2,
                    Explanation = "Lei da Identidade: A OR 0 = A e A AND 1 = A. O elemento neutro do OR é 0, e o elemento neutro do AND é 1.",
                    Hint = "Essa lei mantém a 'identidade' do valor original",
                    Points = 20
                },

                // ===== NÍVEL AVANÇADO (Questões 14-15) =====
                new Question
                {
                    Category = "🚀 Circuitos Complexos",
                    Difficulty = "Avançado",
                    QuestionText = "Qual é o resultado da expressão: NOT((A OR B) AND (A OR C)) quando A=0, B=1, C=1?",
                    Options = new[] { "A) 0", "B) 1", "C) Depende de A", "D) Indefinido" },
                    CorrectAnswer = 1,
                    Explanation = "Passo a passo: A=0, B=1, C=1. (0 OR 1) = 1, (0 OR 1) = 1. (1 AND 1) = 1. NOT(1) = 0. Portanto, o resultado é 0.",
                    Hint = "Substitua os valores e resolva de dentro para fora: primeiro os OR, depois o AND, por fim o NOT",
                    Points = 25
                },

                new Question
                {
                    Category = "🚀 Circuitos Complexos",
                    Difficulty = "Avançado",
                    QuestionText = "Simplifique a expressão: A AND (A OR B). Qual é o resultado?",
                    Options = new[] { "A) A", "B) B", "C) A OR B", "D) A AND B" },
                    CorrectAnswer = 1,
                    Explanation = "Pela Lei da Absorção: A AND (A OR B) = A. Isso porque se A=1, o resultado é 1 independente de B. Se A=0, o resultado é 0 independente de B.",
                    Hint = "Use a Lei da Absorção ou teste com diferentes valores de A e B",
                    Points = 25
                }
            };
        }

        /// <summary>
        /// Inicializa o timer
        /// </summary>
        private void InitializeTimer()
        {
            elapsedTime = TimeSpan.Zero;
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

        /// <summary>
        /// Carrega a questão atual
        /// </summary>
        private void LoadQuestion()
        {
            if (currentQuestionIndex >= questions.Count)
            {
                ShowResults();
                return;
            }

            var question = questions[currentQuestionIndex];

            // Reset do estado
            answered = false;
            hintUsed = false;
            selectedOption = 0;

            // Atualiza interface
            SubtitleText.Text = $"Questão {currentQuestionIndex + 1} de {questions.Count}";
            CategoryText.Text = question.Category;
            QuestionText.Text = question.QuestionText;

            // Dificuldade
            DifficultyText.Text = question.Difficulty;
            switch (question.Difficulty)
            {
                case "Fácil":
                    DifficultyBadge.Background = new SolidColorBrush(Color.FromRgb(213, 244, 230)); // Verde claro
                    DifficultyBadge.BorderBrush = new SolidColorBrush(Color.FromRgb(39, 174, 96));
                    DifficultyText.Foreground = new SolidColorBrush(Color.FromRgb(39, 174, 96));
                    break;
                case "Médio":
                    DifficultyBadge.Background = new SolidColorBrush(Color.FromRgb(255, 243, 205)); // Amarelo claro
                    DifficultyBadge.BorderBrush = new SolidColorBrush(Color.FromRgb(243, 156, 18));
                    DifficultyText.Foreground = new SolidColorBrush(Color.FromRgb(243, 156, 18));
                    break;
                case "Difícil":
                    DifficultyBadge.Background = new SolidColorBrush(Color.FromRgb(253, 237, 236)); // Laranja claro
                    DifficultyBadge.BorderBrush = new SolidColorBrush(Color.FromRgb(230, 126, 34));
                    DifficultyText.Foreground = new SolidColorBrush(Color.FromRgb(230, 126, 34));
                    break;
                case "Avançado":
                    DifficultyBadge.Background = new SolidColorBrush(Color.FromRgb(250, 219, 216)); // Vermelho claro
                    DifficultyBadge.BorderBrush = new SolidColorBrush(Color.FromRgb(231, 76, 60));
                    DifficultyText.Foreground = new SolidColorBrush(Color.FromRgb(231, 76, 60));
                    break;
            }

            // Opções
            Option1.Content = question.Options[0];
            Option2.Content = question.Options[1];
            Option3.Content = question.Options[2];
            Option4.Content = question.Options[3];

            // Reset dos estilos
            Option1.Style = (Style)FindResource("OptionButton");
            Option2.Style = (Style)FindResource("OptionButton");
            Option3.Style = (Style)FindResource("OptionButton");
            Option4.Style = (Style)FindResource("OptionButton");

            // Habilita/desabilita opções
            Option1.IsEnabled = true;
            Option2.IsEnabled = true;
            Option3.IsEnabled = true;
            Option4.IsEnabled = true;

            // Diagrama (se houver)
            if (!string.IsNullOrEmpty(question.Diagram))
            {
                DiagramText.Text = question.Diagram;
                DiagramPanel.Visibility = Visibility.Visible;
            }
            else
            {
                DiagramPanel.Visibility = Visibility.Collapsed;
            }

            // Esconde elementos
            HintPanel.Visibility = Visibility.Collapsed;
            FeedbackPanel.Visibility = Visibility.Collapsed;
            ExplanationPanel.Visibility = Visibility.Visible;

            // Botões
            HintButton.IsEnabled = true;
            CheckButton.IsEnabled = false;
            CheckButton.Content = "VERIFICAR RESPOSTA";

            // Atualiza progresso
            UpdateProgress();
        }

        /// <summary>
        /// Atualiza a barra de progresso
        /// </summary>
        private void UpdateProgress()
        {
            double percentage = ((double)currentQuestionIndex / questions.Count) * 100;
            ProgressBar.Width = (this.ActualWidth - 60) * (percentage / 100);
            ProgressText.Text = $"{(int)percentage}%";

            ScoreText.Text = $"{score} pontos";
            CorrectText.Text = $"{correctAnswers}/{questions.Count}";

            // Atualiza vidas
            string hearts = "";
            for (int i = 0; i < lives; i++)
                hearts += "❤️ ";
            for (int i = lives; i < 3; i++)
                hearts += "🖤 ";
            LivesText.Text = hearts.Trim();
        }

        /// <summary>
        /// Evento de clique em uma opção
        /// </summary>
        private void Option_Click(object sender, RoutedEventArgs e)
        {
            if (answered) return;

            var button = sender as Button;
            selectedOption = int.Parse(button.Tag.ToString());

            CheckButton.IsEnabled = true;
        }

        /// <summary>
        /// Verifica a resposta
        /// </summary>
        private void CheckAnswer_Click(object sender, RoutedEventArgs e)
        {
            if (!answered && selectedOption > 0)
            {
                // Verifica resposta
                answered = true;
                var question = questions[currentQuestionIndex];
                bool isCorrect = (selectedOption == question.CorrectAnswer);

                // Desabilita opções
                Option1.IsEnabled = false;
                Option2.IsEnabled = false;
                Option3.IsEnabled = false;
                Option4.IsEnabled = false;

                // Mostra feedback
                if (isCorrect)
                {
                    correctAnswers++;
                    int pointsEarned = hintUsed ? question.Points / 2 : question.Points;
                    score += pointsEarned;

                    FeedbackPanel.Background = new SolidColorBrush(Color.FromRgb(213, 244, 230));
                    FeedbackPanel.BorderBrush = new SolidColorBrush(Color.FromRgb(39, 174, 96));
                    FeedbackIcon.Text = "✓";
                    FeedbackIcon.Foreground = new SolidColorBrush(Color.FromRgb(39, 174, 96));
                    FeedbackTitle.Text = $"Correto! +{pointsEarned} pontos";
                    FeedbackTitle.Foreground = new SolidColorBrush(Color.FromRgb(30, 132, 73));
                    FeedbackMessage.Text = $"Muito bem! {question.Options[question.CorrectAnswer - 1]}";
                    FeedbackMessage.Foreground = new SolidColorBrush(Color.FromRgb(39, 174, 96));

                    ExplanationPanel.BorderBrush = new SolidColorBrush(Color.FromRgb(39, 174, 96));

                    // Marca a opção correta
                    GetOptionButton(selectedOption).Style = (Style)FindResource("CorrectButton");
                }
                else
                {
                    incorrectAnswers++;
                    lives--;

                    FeedbackPanel.Background = new SolidColorBrush(Color.FromRgb(250, 219, 216));
                    FeedbackPanel.BorderBrush = new SolidColorBrush(Color.FromRgb(231, 76, 60));
                    FeedbackIcon.Text = "✗";
                    FeedbackIcon.Foreground = new SolidColorBrush(Color.FromRgb(231, 76, 60));
                    FeedbackTitle.Text = "Incorreto!";
                    FeedbackTitle.Foreground = new SolidColorBrush(Color.FromRgb(192, 57, 43));
                    FeedbackMessage.Text = $"A resposta correta é: {question.Options[question.CorrectAnswer - 1]}";
                    FeedbackMessage.Foreground = new SolidColorBrush(Color.FromRgb(231, 76, 60));

                    ExplanationPanel.BorderBrush = new SolidColorBrush(Color.FromRgb(231, 76, 60));

                    // Marca opções
                    GetOptionButton(selectedOption).Style = (Style)FindResource("IncorrectButton");
                    GetOptionButton(question.CorrectAnswer).Style = (Style)FindResource("CorrectButton");

                    // Verifica se perdeu todas as vidas
                    if (lives <= 0)
                    {
                        ShowResults();
                        return;
                    }
                }

                ExplanationText.Text = question.Explanation;
                FeedbackPanel.Visibility = Visibility.Visible;

                UpdateProgress();

                CheckButton.Content = "PRÓXIMA QUESTÃO ▶";
            }
            else
            {
                // Próxima questão
                currentQuestionIndex++;
                LoadQuestion();
            }
        }

        /// <summary>
        /// Obtém o botão de opção pelo índice
        /// </summary>
        private Button GetOptionButton(int index)
        {
            switch (index)
            {
                case 1: return Option1;
                case 2: return Option2;
                case 3: return Option3;
                case 4: return Option4;
                default: return Option1;
            }
        }

        /// <summary>
        /// Mostra a dica
        /// </summary>
        private void ShowHint_Click(object sender, RoutedEventArgs e)
        {
            var question = questions[currentQuestionIndex];
            HintText.Text = question.Hint;
            HintPanel.Visibility = Visibility.Visible;
            HintButton.IsEnabled = false;
            hintUsed = true;
        }

        /// <summary>
        /// Pula a questão
        /// </summary>
        private void SkipQuestion_Click(object sender, RoutedEventArgs e)
        {
            if (!answered)
            {
                incorrectAnswers++;
                currentQuestionIndex++;
                LoadQuestion();
            }
        }

        /// <summary>
        /// Mostra os resultados finais
        /// </summary>
        private void ShowResults()
        {
            timer.Stop();

            // Calcula estatísticas
            double accuracy = questions.Count > 0 ? ((double)correctAnswers / questions.Count) * 100 : 0;
            int maxScore = questions.Sum(q => q.Points);

            // Ícone e mensagem baseado no desempenho
            if (accuracy >= 90)
            {
                ResultIcon.Text = "🏆";
                ResultTitle.Text = "Excelente!";
                PerformanceMessage.Text = "Desempenho excepcional! Você domina portas lógicas e álgebra booleana!";
            }
            else if (accuracy >= 70)
            {
                ResultIcon.Text = "🎉";
                ResultTitle.Text = "Muito Bom!";
                PerformanceMessage.Text = "Ótimo desempenho! Continue praticando para alcançar a perfeição!";
            }
            else if (accuracy >= 50)
            {
                ResultIcon.Text = "👍";
                ResultTitle.Text = "Bom trabalho!";
                PerformanceMessage.Text = "Bom resultado! Revise os conceitos e tente novamente!";
            }
            else
            {
                ResultIcon.Text = "📚";
                ResultTitle.Text = "Continue estudando!";
                PerformanceMessage.Text = "Não desanime! Revise o material teórico e pratique mais!";
            }

            // Atualiza textos
            FinalScoreText.Text = $"Pontuação: {score}/{maxScore}";
            FinalCorrectText.Text = $"{correctAnswers}/{questions.Count}";
            FinalIncorrectText.Text = $"{incorrectAnswers}/{questions.Count}";
            AccuracyText.Text = $"{accuracy:F1}%";
            FinalTimeText.Text = elapsedTime.ToString(@"mm\:ss");

            ResultPanel.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Reinicia o teste
        /// </summary>
        private void RestartTest_Click(object sender, RoutedEventArgs e)
        {
            currentQuestionIndex = 0;
            score = 0;
            correctAnswers = 0;
            incorrectAnswers = 0;
            lives = 3;
            elapsedTime = TimeSpan.Zero;

            ResultPanel.Visibility = Visibility.Collapsed;

            timer.Start();
            LoadQuestion();
        }

        /// <summary>
        /// Fecha o teste
        /// </summary>
        private void CloseTest_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}