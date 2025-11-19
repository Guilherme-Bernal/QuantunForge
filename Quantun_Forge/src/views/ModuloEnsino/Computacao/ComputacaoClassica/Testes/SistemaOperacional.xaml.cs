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
    /// Lógica interna para SistemaOperacionalTeste.xaml
    /// </summary>
    public partial class SistemaOperacionalTeste : Window
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

        public SistemaOperacionalTeste()
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
                    Category = "🖥️ Gerenciamento de Processos",
                    Text = "Qual é a função principal do Kernel (núcleo) do sistema operacional?",
                    Options = new[]
                    {
                        "A) Gerenciar recursos de hardware e fornecer serviços básicos",
                        "B) Criar interfaces gráficas para o usuário",
                        "C) Executar aplicativos de produtividade",
                        "D) Armazenar arquivos no disco rígido"
                    },
                    CorrectAnswer = 1,
                    Hint = "O kernel é o componente central que interage diretamente com o hardware",
                    Explanation = "O Kernel é o núcleo do sistema operacional, responsável por gerenciar recursos de hardware (CPU, memória, dispositivos) e fornecer serviços básicos para outros componentes do sistema."
                },
                new Question
                {
                    Category = "💾 Gerenciamento de Memória",
                    Text = "O que é memória virtual?",
                    Options = new[]
                    {
                        "A) Memória RAM de alta velocidade",
                        "B) Técnica que usa disco como extensão da RAM",
                        "C) Memória cache do processador",
                        "D) Memória ROM do sistema"
                    },
                    CorrectAnswer = 2,
                    Hint = "Permite executar programas maiores que a RAM disponível",
                    Explanation = "Memória virtual é uma técnica que usa espaço em disco como extensão da RAM, permitindo executar programas que excedem a memória física disponível através de paginação."
                },
                new Question
                {
                    Category = "⚙️ Escalonamento de CPU",
                    Text = "No algoritmo de escalonamento FCFS (First Come, First Served), qual processo é executado primeiro?",
                    Options = new[]
                    {
                        "A) O processo com maior prioridade",
                        "B) O processo que chegou primeiro",
                        "C) O processo com menor tempo de execução",
                        "D) O processo com maior tempo de espera"
                    },
                    CorrectAnswer = 2,
                    Hint = "O nome do algoritmo já dá a resposta",
                    Explanation = "FCFS executa os processos na ordem exata de chegada, sem considerar prioridade ou tempo de execução. É o algoritmo mais simples, mas pode causar espera longa."
                },
                new Question
                {
                    Category = "📁 Sistema de Arquivos",
                    Text = "Qual sistema de arquivos é padrão no Windows moderno?",
                    Options = new[]
                    {
                        "A) FAT32",
                        "B) ext4",
                        "C) NTFS",
                        "D) HFS+"
                    },
                    CorrectAnswer = 3,
                    Hint = "Este sistema suporta permissões, criptografia e arquivos grandes",
                    Explanation = "NTFS (New Technology File System) é o sistema de arquivos padrão do Windows desde o Windows NT. Oferece recursos avançados como permissões, criptografia, compressão e suporte a arquivos grandes."
                },
                new Question
                {
                    Category = "🔄 Threads e Processos",
                    Text = "Qual é a principal diferença entre processo e thread?",
                    Options = new[]
                    {
                        "A) Processos são mais rápidos que threads",
                        "B) Threads compartilham o mesmo espaço de memória do processo pai",
                        "C) Threads não podem ser executadas em paralelo",
                        "D) Processos não podem se comunicar entre si"
                    },
                    CorrectAnswer = 2,
                    Hint = "Pense em como eles usam a memória",
                    Explanation = "Threads são unidades de execução dentro de um processo e compartilham o mesmo espaço de memória, enquanto processos têm espaços de memória separados. Isso torna threads mais leves e eficientes para comunicação."
                },
                new Question
                {
                    Category = "🐧 Tipos de SO",
                    Text = "Qual destes sistemas operacionais é de código aberto?",
                    Options = new[]
                    {
                        "A) Windows",
                        "B) macOS",
                        "C) Linux",
                        "D) iOS"
                    },
                    CorrectAnswer = 3,
                    Hint = "Este SO tem um pinguim como mascote",
                    Explanation = "Linux é um sistema operacional de código aberto (open source), ou seja, seu código-fonte está disponível publicamente e pode ser modificado e distribuído livremente."
                },
                new Question
                {
                    Category = "🔒 Segurança",
                    Text = "O que são permissões de arquivo em um sistema operacional?",
                    Options = new[]
                    {
                        "A) Configurações de antivírus",
                        "B) Regras que controlam quem pode acessar e modificar arquivos",
                        "C) Senhas para abrir arquivos",
                        "D) Tipos de arquivo permitidos no sistema"
                    },
                    CorrectAnswer = 2,
                    Hint = "Controlam leitura, escrita e execução",
                    Explanation = "Permissões de arquivo definem quais usuários ou grupos podem ler, escrever ou executar arquivos específicos, sendo um mecanismo fundamental de segurança do sistema operacional."
                },
                new Question
                {
                    Category = "⚡ Escalonamento",
                    Text = "Qual algoritmo de escalonamento garante que todos os processos recebam fatias iguais de tempo de CPU?",
                    Options = new[]
                    {
                        "A) FCFS",
                        "B) SJF",
                        "C) Round Robin",
                        "D) Prioridade"
                    },
                    CorrectAnswer = 3,
                    Hint = "O nome sugere um revezamento circular",
                    Explanation = "Round Robin distribui o tempo da CPU igualmente entre todos os processos, dando a cada um uma fatia de tempo (quantum) antes de passar para o próximo, garantindo justiça e responsividade."
                },
                new Question
                {
                    Category = "💿 Armazenamento",
                    Text = "O que é um driver de dispositivo?",
                    Options = new[]
                    {
                        "A) Software que permite ao SO comunicar com hardware específico",
                        "B) Disco rígido externo",
                        "C) Programa de backup",
                        "D) Ferramenta de formatação"
                    },
                    CorrectAnswer = 1,
                    Hint = "Atua como tradutor entre SO e hardware",
                    Explanation = "Um driver é um software que permite ao sistema operacional se comunicar e controlar um dispositivo de hardware específico, atuando como interface entre o SO e o hardware."
                },
                new Question
                {
                    Category = "🔄 Estados de Processo",
                    Text = "Em qual estado um processo está quando aguarda a conclusão de uma operação de E/S (entrada/saída)?",
                    Options = new[]
                    {
                        "A) Executando",
                        "B) Pronto",
                        "C) Esperando (Bloqueado)",
                        "D) Terminado"
                    },
                    CorrectAnswer = 3,
                    Hint = "O processo não pode continuar até receber os dados",
                    Explanation = "Um processo entra no estado 'Esperando' ou 'Bloqueado' quando precisa aguardar a conclusão de uma operação de E/S (como leitura de disco ou rede) antes de poder continuar sua execução."
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
                PerformanceMessage.Text = "Você domina Sistemas Operacionais! Parabéns pelo excelente desempenho!";
            }
            else if (accuracy >= 70)
            {
                ResultIcon.Text = "🎉";
                ResultTitle.Text = "Muito Bom!";
                PerformanceMessage.Text = "Ótimo resultado! Você tem um bom conhecimento sobre Sistemas Operacionais!";
            }
            else if (accuracy >= 50)
            {
                ResultIcon.Text = "👍";
                ResultTitle.Text = "Bom Trabalho!";
                PerformanceMessage.Text = "Bom resultado! Revise os conceitos e tente novamente para melhorar.";
            }
            else
            {
                ResultIcon.Text = "📚";
                ResultTitle.Text = "Continue Estudando!";
                PerformanceMessage.Text = "Não desanime! Estude mais sobre Sistemas Operacionais e pratique novamente.";
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