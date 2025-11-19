using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Quantun_Forge.src.views.ModuloEnsino.Computacao.ComputacaoClassica.Testes
{
    public partial class VonNeumannTeste : Window
    {
        // Classe para representar uma questão
        private class Question
        {
            public string Category { get; set; }
            public string Difficulty { get; set; }
            public string QuestionText { get; set; }
            public string[] Options { get; set; }
            public int CorrectAnswer { get; set; }
            public string Explanation { get; set; }
            public string Hint { get; set; }
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

        public VonNeumannTeste()
        {
            InitializeComponent();
            InitializeQuestions();
            InitializeTimer();
            Loaded += VonNeumannTeste_Loaded;
        }

        private void VonNeumannTeste_Loaded(object sender, RoutedEventArgs e)
        {
            LoadQuestion();
        }

        /// <summary>
        /// Inicializa as 12 questões (do básico ao avançado)
        /// </summary>
        private void InitializeQuestions()
        {
            questions = new List<Question>
            {
                // ===== NÍVEL FÁCIL (Questões 1-4) =====
                new Question
                {
                    Category = "🖥️ Conceitos Fundamentais",
                    Difficulty = "Fácil",
                    QuestionText = "Quem propôs a arquitetura de computador que leva seu nome?",
                    Options = new[] { "A) Alan Turing", "B) John von Neumann", "C) Charles Babbage", "D) Bill Gates" },
                    CorrectAnswer = 2,
                    Explanation = "John von Neumann propôs esta arquitetura em 1945, revolucionando a computação ao introduzir o conceito de programa armazenado.",
                    Hint = "O nome da arquitetura tem o nome do cientista",
                    Points = 10
                },

                new Question
                {
                    Category = "🖥️ Conceitos Fundamentais",
                    Difficulty = "Fácil",
                    QuestionText = "Qual é a principal inovação da arquitetura de Von Neumann?",
                    Options = new[] {
                        "A) Uso de transistores",
                        "B) Programa armazenado na memória",
                        "C) Interface gráfica",
                        "D) Processamento em nuvem"
                    },
                    CorrectAnswer = 2,
                    Explanation = "A principal inovação foi o conceito de programa armazenado: instruções e dados compartilham o mesmo espaço de memória, permitindo que programas sejam carregados e executados sem modificar o hardware.",
                    Hint = "Pense em onde ficam as instruções do programa",
                    Points = 10
                },

                new Question
                {
                    Category = "🔧 Componentes",
                    Difficulty = "Fácil",
                    QuestionText = "Qual componente é considerado o 'cérebro' do computador?",
                    Options = new[] { "A) Memória RAM", "B) CPU", "C) Disco Rígido", "D) Placa-mãe" },
                    CorrectAnswer = 2,
                    Explanation = "A CPU (Unidade Central de Processamento) é o cérebro do computador, responsável por executar instruções e processar dados.",
                    Hint = "É responsável por executar as instruções",
                    Points = 10
                },

                new Question
                {
                    Category = "🔧 Componentes",
                    Difficulty = "Fácil",
                    QuestionText = "Qual característica define a memória RAM?",
                    Options = new[] {
                        "A) Armazena dados permanentemente",
                        "B) É volátil e perde dados ao desligar",
                        "C) Só armazena o sistema operacional",
                        "D) Não pode ser modificada"
                    },
                    CorrectAnswer = 2,
                    Explanation = "A RAM (Random Access Memory) é volátil, ou seja, perde todo seu conteúdo quando o computador é desligado. Ela armazena temporariamente dados e instruções dos programas em execução.",
                    Hint = "Pense no que acontece com os dados ao desligar o computador",
                    Points = 10
                },

                // ===== NÍVEL MÉDIO (Questões 5-8) =====
                new Question
                {
                    Category = "🔄 Ciclo de Instrução",
                    Difficulty = "Médio",
                    QuestionText = "Qual é a ordem correta do ciclo de instrução?",
                    Options = new[] {
                        "A) Execute → Fetch → Decode → Store",
                        "B) Fetch → Decode → Execute → Store",
                        "C) Decode → Fetch → Store → Execute",
                        "D) Store → Execute → Fetch → Decode"
                    },
                    CorrectAnswer = 2,
                    Explanation = "O ciclo correto é: FETCH (buscar instrução), DECODE (decodificar), EXECUTE (executar) e STORE (armazenar resultado). Este ciclo se repete continuamente durante a execução de um programa.",
                    Hint = "Primeiro busca, depois interpreta, executa e por fim armazena",
                    Points = 15
                },

                new Question
                {
                    Category = "🔧 Componentes",
                    Difficulty = "Médio",
                    QuestionText = "A CPU é dividida em duas unidades principais. Quais são elas?",
                    Options = new[] {
                        "A) RAM e ROM",
                        "B) UC e ULA",
                        "C) Cache e Registradores",
                        "D) Input e Output"
                    },
                    CorrectAnswer = 2,
                    Explanation = "A CPU é composta pela UC (Unidade de Controle), que coordena as operações, e pela ULA (Unidade Lógica Aritmética), que realiza operações matemáticas e lógicas.",
                    Hint = "Uma unidade controla, outra calcula",
                    Points = 15
                },

                new Question
                {
                    Category = "🚌 Barramentos",
                    Difficulty = "Médio",
                    QuestionText = "Qual barramento transporta dados entre os componentes?",
                    Options = new[] {
                        "A) Barramento de Controle",
                        "B) Barramento de Endereços",
                        "C) Barramento de Dados",
                        "D) Barramento de Energia"
                    },
                    CorrectAnswer = 3,
                    Explanation = "O Barramento de Dados é responsável por transportar dados entre CPU, memória e dispositivos de E/S. Os outros barramentos são: Endereços (localização) e Controle (sinais de comando).",
                    Hint = "O nome já indica sua função principal",
                    Points = 15
                },

                new Question
                {
                    Category = "🔄 Ciclo de Instrução",
                    Difficulty = "Médio",
                    QuestionText = "O que o Program Counter (PC) armazena?",
                    Options = new[] {
                        "A) O resultado da última operação",
                        "B) O endereço da próxima instrução",
                        "C) O número de programas em execução",
                        "D) A velocidade do processador"
                    },
                    CorrectAnswer = 2,
                    Explanation = "O Program Counter (Contador de Programa) é um registrador especial que armazena o endereço de memória da próxima instrução a ser executada, sendo fundamental no ciclo FETCH.",
                    Hint = "Ele 'conta' qual instrução vem a seguir",
                    Points = 15
                },

                // ===== NÍVEL DIFÍCIL (Questões 9-10) =====
                new Question
                {
                    Category = "⚠️ Limitações",
                    Difficulty = "Difícil",
                    QuestionText = "O que é o 'Gargalo de Von Neumann'?",
                    Options = new[] {
                        "A) Excesso de memória RAM",
                        "B) Limitação do barramento compartilhado CPU-Memória",
                        "C) Falta de dispositivos de entrada",
                        "D) CPU muito rápida"
                    },
                    CorrectAnswer = 2,
                    Explanation = "O Gargalo de Von Neumann refere-se à limitação de velocidade causada pelo barramento único compartilhado entre CPU e memória. Como dados e instruções usam o mesmo caminho, não podem ser transferidos simultaneamente.",
                    Hint = "Pense no problema do caminho compartilhado",
                    Points = 20
                },

                new Question
                {
                    Category = "🔧 Componentes",
                    Difficulty = "Difícil",
                    QuestionText = "Qual é a função principal da Unidade de Controle (UC)?",
                    Options = new[] {
                        "A) Realizar cálculos matemáticos",
                        "B) Armazenar dados temporariamente",
                        "C) Coordenar e sincronizar operações do computador",
                        "D) Conectar dispositivos externos"
                    },
                    CorrectAnswer = 3,
                    Explanation = "A Unidade de Controle (UC) é responsável por coordenar e sincronizar todas as operações do computador, interpretando instruções, gerenciando o fluxo de dados e controlando os demais componentes.",
                    Hint = "O nome 'controle' indica sua função principal",
                    Points = 20
                },

                // ===== NÍVEL AVANÇADO (Questões 11-12) =====
                new Question
                {
                    Category = "🚀 Conceitos Avançados",
                    Difficulty = "Avançado",
                    QuestionText = "Qual das seguintes NÃO é uma característica da arquitetura de Von Neumann?",
                    Options = new[] {
                        "A) Programa e dados na mesma memória",
                        "B) Processamento sequencial de instruções",
                        "C) Múltiplos barramentos independentes para dados e instruções",
                        "D) Ciclo Fetch-Decode-Execute"
                    },
                    CorrectAnswer = 3,
                    Explanation = "A arquitetura de Von Neumann usa um ÚNICO barramento compartilhado para dados e instruções (o que causa o gargalo). Arquiteturas alternativas como Harvard usam barramentos separados.",
                    Hint = "Pense na limitação principal desta arquitetura",
                    Points = 25
                },

                new Question
                {
                    Category = "🚀 Conceitos Avançados",
                    Difficulty = "Avançado",
                    QuestionText = "Por que a arquitetura de Von Neumann permite que programas se auto-modifiquem?",
                    Options = new[] {
                        "A) Porque a CPU é muito rápida",
                        "B) Porque instruções são tratadas como dados na memória",
                        "C) Porque existem múltiplos processadores",
                        "D) Porque a memória é volátil"
                    },
                    CorrectAnswer = 2,
                    Explanation = "Como instruções e dados compartilham o mesmo espaço de memória e são tratados da mesma forma, um programa pode modificar suas próprias instruções durante a execução, tratando-as como dados. Isso é útil mas também pode ser perigoso.",
                    Hint = "Pense no conceito de programa armazenado",
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
                    DifficultyBadge.Background = new SolidColorBrush(Color.FromRgb(213, 244, 230));
                    DifficultyBadge.BorderBrush = new SolidColorBrush(Color.FromRgb(39, 174, 96));
                    DifficultyText.Foreground = new SolidColorBrush(Color.FromRgb(39, 174, 96));
                    break;
                case "Médio":
                    DifficultyBadge.Background = new SolidColorBrush(Color.FromRgb(255, 243, 205));
                    DifficultyBadge.BorderBrush = new SolidColorBrush(Color.FromRgb(243, 156, 18));
                    DifficultyText.Foreground = new SolidColorBrush(Color.FromRgb(243, 156, 18));
                    break;
                case "Difícil":
                    DifficultyBadge.Background = new SolidColorBrush(Color.FromRgb(253, 237, 236));
                    DifficultyBadge.BorderBrush = new SolidColorBrush(Color.FromRgb(230, 126, 34));
                    DifficultyText.Foreground = new SolidColorBrush(Color.FromRgb(230, 126, 34));
                    break;
                case "Avançado":
                    DifficultyBadge.Background = new SolidColorBrush(Color.FromRgb(250, 219, 216));
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

            // Habilita opções
            Option1.IsEnabled = true;
            Option2.IsEnabled = true;
            Option3.IsEnabled = true;
            Option4.IsEnabled = true;

            // Esconde elementos
            HintPanel.Visibility = Visibility.Collapsed;
            FeedbackPanel.Visibility = Visibility.Collapsed;

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

                    GetOptionButton(selectedOption).Style = (Style)FindResource("IncorrectButton");
                    GetOptionButton(question.CorrectAnswer).Style = (Style)FindResource("CorrectButton");

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
                currentQuestionIndex++;
                LoadQuestion();
            }
        }

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

        private void ShowHint_Click(object sender, RoutedEventArgs e)
        {
            var question = questions[currentQuestionIndex];
            HintText.Text = question.Hint;
            HintPanel.Visibility = Visibility.Visible;
            HintButton.IsEnabled = false;
            hintUsed = true;
        }

        private void SkipQuestion_Click(object sender, RoutedEventArgs e)
        {
            if (!answered)
            {
                incorrectAnswers++;
                currentQuestionIndex++;
                LoadQuestion();
            }
        }

        private void ShowResults()
        {
            timer.Stop();

            double accuracy = questions.Count > 0 ? ((double)correctAnswers / questions.Count) * 100 : 0;
            int maxScore = questions.Sum(q => q.Points);

            if (accuracy >= 90)
            {
                ResultIcon.Text = "🏆";
                ResultTitle.Text = "Excelente!";
                PerformanceMessage.Text = "Desempenho excepcional! Você domina a Arquitetura de Von Neumann!";
            }
            else if (accuracy >= 70)
            {
                ResultIcon.Text = "🎉";
                ResultTitle.Text = "Muito Bom!";
                PerformanceMessage.Text = "Ótimo desempenho! Continue estudando para alcançar a perfeição!";
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

            FinalScoreText.Text = $"Pontuação: {score}/{maxScore}";
            FinalCorrectText.Text = $"{correctAnswers}/{questions.Count}";
            FinalIncorrectText.Text = $"{incorrectAnswers}/{questions.Count}";
            AccuracyText.Text = $"{accuracy:F1}%";
            FinalTimeText.Text = elapsedTime.ToString(@"mm\:ss");

            ResultPanel.Visibility = Visibility.Visible;
        }

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

        private void CloseTest_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}