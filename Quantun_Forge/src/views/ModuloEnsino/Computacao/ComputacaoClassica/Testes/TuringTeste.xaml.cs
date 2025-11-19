using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Quantun_Forge.src.views.ModuloEnsino.Computacao.ComputacaoClassica.Testes
{
    public partial class TuringTeste : Window
    {
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
        private DispatcherTimer timer;
        private TimeSpan elapsedTime;

        public TuringTeste()
        {
            InitializeComponent();
            InitializeQuestions();
            InitializeTimer();
            Loaded += TuringTeste_Loaded;
        }

        private void TuringTeste_Loaded(object sender, RoutedEventArgs e)
        {
            LoadQuestion();
        }

        private void InitializeQuestions()
        {
            questions = new List<Question>
            {
                // NÍVEL FÁCIL (1-4)
                new Question
                {
                    Category = "🧮 História da Computação",
                    Difficulty = "Fácil",
                    QuestionText = "Em que ano Alan Turing propôs o conceito da Máquina de Turing?",
                    Options = new[] { "A) 1936", "B) 1945", "C) 1950", "D) 1954" },
                    CorrectAnswer = 1,
                    Explanation = "Alan Turing propôs a Máquina de Turing em 1936, em seu artigo 'On Computable Numbers', revolucionando a teoria da computação.",
                    Hint = "Foi antes da Segunda Guerra Mundial",
                    Points = 10
                },
                new Question
                {
                    Category = "🧮 História da Computação",
                    Difficulty = "Fácil",
                    QuestionText = "Qual foi a principal contribuição de Turing durante a Segunda Guerra Mundial?",
                    Options = new[] { "A) Criou o primeiro computador", "B) Quebrou o código Enigma", "C) Inventou a internet", "D) Desenvolveu a bomba atômica" },
                    CorrectAnswer = 2,
                    Explanation = "Turing desenvolveu técnicas que quebraram o código Enigma nazista em Bletchley Park, ajudando a encurtar a guerra e salvando milhões de vidas.",
                    Hint = "Envolvia criptografia nazista",
                    Points = 10
                },
                new Question
                {
                    Category = "🤖 Teste de Turing",
                    Difficulty = "Fácil",
                    QuestionText = "O que o Teste de Turing avalia?",
                    Options = new[] { "A) Velocidade de processamento", "B) Capacidade de uma máquina exibir comportamento inteligente", "C) Consumo de energia", "D) Capacidade de armazenamento" },
                    CorrectAnswer = 2,
                    Explanation = "O Teste de Turing, proposto em 1950, avalia se uma máquina pode exibir comportamento inteligente indistinguível de um humano através de conversação.",
                    Hint = "Relacionado à inteligência artificial",
                    Points = 10
                },
                new Question
                {
                    Category = "⚙️ Componentes da Máquina",
                    Difficulty = "Fácil",
                    QuestionText = "Qual componente da Máquina de Turing representa a memória?",
                    Options = new[] { "A) Cabeça de leitura", "B) Fita infinita", "C) Estados", "D) Transições" },
                    CorrectAnswer = 2,
                    Explanation = "A fita infinita dividida em células representa a memória da Máquina de Turing, onde símbolos podem ser lidos e escritos.",
                    Hint = "É dividida em células e contém símbolos",
                    Points = 10
                },
                // NÍVEL MÉDIO (5-8)
                new Question
                {
                    Category = "⚙️ Componentes da Máquina",
                    Difficulty = "Médio",
                    QuestionText = "Quantos movimentos a cabeça de leitura/escrita pode fazer?",
                    Options = new[] { "A) 1 (apenas direita)", "B) 2 (esquerda ou direita)", "C) 3 (esquerda, direita ou parar)", "D) 4 (todas as direções)" },
                    CorrectAnswer = 2,
                    Explanation = "A cabeça pode mover-se para a ESQUERDA (L) ou para a DIREITA (R) após cada operação, navegando pela fita infinita.",
                    Hint = "Pense em uma linha unidimensional",
                    Points = 15
                },
                new Question
                {
                    Category = "🔄 Funcionamento",
                    Difficulty = "Médio",
                    QuestionText = "Qual é a ordem correta de operação da Máquina de Turing?",
                    Options = new[] { "A) Escrever → Ler → Mover", "B) Ler → Consultar regra → Executar", "C) Mover → Ler → Escrever", "D) Consultar → Mover → Ler" },
                    CorrectAnswer = 2,
                    Explanation = "O ciclo é: LER o símbolo atual, CONSULTAR a tabela de transições, EXECUTAR a ação (escrever, mover e mudar de estado).",
                    Hint = "Primeiro você precisa saber o que está na fita",
                    Points = 15
                },
                new Question
                {
                    Category = "🔄 Funcionamento",
                    Difficulty = "Médio",
                    QuestionText = "O que define qual ação a Máquina de Turing deve executar?",
                    Options = new[] { "A) Apenas o estado atual", "B) Apenas o símbolo lido", "C) Estado atual e símbolo lido", "D) Posição da cabeça" },
                    CorrectAnswer = 3,
                    Explanation = "A tabela de transições usa AMBOS o estado atual E o símbolo lido para determinar qual ação executar (escrever, mover, próximo estado).",
                    Hint = "São necessárias duas informações",
                    Points = 15
                },
                new Question
                {
                    Category = "🌟 Conceitos Teóricos",
                    Difficulty = "Médio",
                    QuestionText = "O que é uma Máquina Universal de Turing?",
                    Options = new[] {
                        "A) Uma máquina física real",
                        "B) Uma máquina que pode simular qualquer outra Máquina de Turing",
                        "C) A primeira máquina criada por Turing",
                        "D) Uma máquina que resolve todos os problemas"
                    },
                    CorrectAnswer = 2,
                    Explanation = "A Máquina Universal de Turing pode simular qualquer outra Máquina de Turing. Este conceito é a base dos computadores modernos programáveis.",
                    Hint = "Pense nos computadores modernos que executam qualquer programa",
                    Points = 15
                },
                // NÍVEL DIFÍCIL (9-10)
                new Question
                {
                    Category = "🌟 Conceitos Teóricos",
                    Difficulty = "Difícil",
                    QuestionText = "O que afirma a Tese de Church-Turing?",
                    Options = new[] {
                        "A) Todos os problemas são computáveis",
                        "B) Tudo que é computável pode ser computado por uma Máquina de Turing",
                        "C) Máquinas sempre param",
                        "D) Humanos são mais inteligentes que máquinas"
                    },
                    CorrectAnswer = 2,
                    Explanation = "A Tese de Church-Turing afirma que qualquer função computável pode ser calculada por uma Máquina de Turing, definindo o limite teórico do que é computacionalmente possível.",
                    Hint = "Define os limites da computação",
                    Points = 20
                },
                new Question
                {
                    Category = "⚠️ Problema da Parada",
                    Difficulty = "Difícil",
                    QuestionText = "O que o Problema da Parada demonstra?",
                    Options = new[] {
                        "A) Todas as máquinas eventualmente param",
                        "B) É impossível criar um algoritmo que determine se qualquer programa irá parar",
                        "C) Programas devem sempre parar",
                        "D) Máquinas de Turing são lentas"
                    },
                    CorrectAnswer = 2,
                    Explanation = "O Problema da Parada prova que NÃO existe um algoritmo geral que determine se qualquer programa irá parar ou executar indefinidamente, demonstrando limites fundamentais da computação.",
                    Hint = "É sobre impossibilidade, não possibilidade",
                    Points = 20
                },
                // NÍVEL AVANÇADO (11-12)
                new Question
                {
                    Category = "🚀 Conceitos Avançados",
                    Difficulty = "Avançado",
                    QuestionText = "Por que a Máquina de Turing é considerada um modelo teórico e não prático?",
                    Options = new[] {
                        "A) É muito lenta",
                        "B) Requer fita infinita e tempo ilimitado",
                        "C) Usa muito espaço",
                        "D) Consome muita energia"
                    },
                    CorrectAnswer = 2,
                    Explanation = "A Máquina de Turing é um modelo TEÓRICO porque assume recursos ilimitados (fita infinita e tempo infinito), o que não existe na prática. Seu valor está em definir o que é computável.",
                    Hint = "Pense nos recursos que ela assume ter",
                    Points = 25
                },
                new Question
                {
                    Category = "🚀 Conceitos Avançados",
                    Difficulty = "Avançado",
                    QuestionText = "Qual é a relação entre a Máquina de Turing e os computadores modernos?",
                    Options = new[] {
                        "A) Não há relação",
                        "B) Computadores são baseados diretamente na Máquina de Turing",
                        "C) Ambos são equivalentes em poder computacional (Turing-completos)",
                        "D) Computadores são mais poderosos que Máquinas de Turing"
                    },
                    CorrectAnswer = 3,
                    Explanation = "Computadores modernos e Máquinas de Turing são Turing-completos: ambos podem computar as mesmas funções (desconsiderando limites práticos de memória e tempo). A Máquina de Turing define o limite teórico da computação.",
                    Hint = "Pense em equivalência de poder computacional",
                    Points = 25
                }
            };
        }

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

        private void LoadQuestion()
        {
            if (currentQuestionIndex >= questions.Count)
            {
                ShowResults();
                return;
            }

            var question = questions[currentQuestionIndex];
            answered = false;
            hintUsed = false;
            selectedOption = 0;

            SubtitleText.Text = $"Questão {currentQuestionIndex + 1} de {questions.Count}";
            CategoryText.Text = question.Category;
            QuestionText.Text = question.QuestionText;

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

            Option1.Content = question.Options[0];
            Option2.Content = question.Options[1];
            Option3.Content = question.Options[2];
            Option4.Content = question.Options[3];

            Option1.Style = (Style)FindResource("OptionButton");
            Option2.Style = (Style)FindResource("OptionButton");
            Option3.Style = (Style)FindResource("OptionButton");
            Option4.Style = (Style)FindResource("OptionButton");

            Option1.IsEnabled = true;
            Option2.IsEnabled = true;
            Option3.IsEnabled = true;
            Option4.IsEnabled = true;

            HintPanel.Visibility = Visibility.Collapsed;
            FeedbackPanel.Visibility = Visibility.Collapsed;

            HintButton.IsEnabled = true;
            CheckButton.IsEnabled = false;
            CheckButton.Content = "VERIFICAR RESPOSTA";

            UpdateProgress();
        }

        private void UpdateProgress()
        {
            double percentage = ((double)currentQuestionIndex / questions.Count) * 100;
            ProgressBar.Width = (this.ActualWidth - 60) * (percentage / 100);
            ProgressText.Text = $"{(int)percentage}%";

            ScoreText.Text = $"{score} pontos";
            CorrectText.Text = $"{correctAnswers}/{questions.Count}";

            string hearts = "";
            for (int i = 0; i < lives; i++) hearts += "❤️ ";
            for (int i = lives; i < 3; i++) hearts += "🖤 ";
            LivesText.Text = hearts.Trim();
        }

        private void Option_Click(object sender, RoutedEventArgs e)
        {
            if (answered) return;
            selectedOption = int.Parse((sender as Button).Tag.ToString());
            CheckButton.IsEnabled = true;
        }

        private void CheckAnswer_Click(object sender, RoutedEventArgs e)
        {
            if (!answered && selectedOption > 0)
            {
                answered = true;
                var question = questions[currentQuestionIndex];
                bool isCorrect = (selectedOption == question.CorrectAnswer);

                Option1.IsEnabled = false;
                Option2.IsEnabled = false;
                Option3.IsEnabled = false;
                Option4.IsEnabled = false;

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
            return index switch { 1 => Option1, 2 => Option2, 3 => Option3, 4 => Option4, _ => Option1 };
        }

        private void ShowHint_Click(object sender, RoutedEventArgs e)
        {
            HintText.Text = questions[currentQuestionIndex].Hint;
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
                PerformanceMessage.Text = "Desempenho excepcional! Você domina Alan Turing e sua contribuição!";
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