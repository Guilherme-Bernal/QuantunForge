using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Threading;
using System.Collections.Generic;
using System.Linq;

namespace Quantun_Forge.src.views
{
    /// <summary>
    /// Lógica de interação para Introducao.xaml
    /// </summary>
    public partial class Introducao : UserControl
    {
        // Timer para animações automáticas
        private DispatcherTimer _animationTimer;

        // Lista para rastrear os cards animados
        private List<FrameworkElement> _animatedElements;

        // Índice da tab atual
        private int _currentTabIndex = 0;

        public Introducao()
        {
            InitializeComponent();

            // Inicializa componentes
            InitializeAnimations();
            SetupEventHandlers();

            // Inicia animação de entrada
            StartEntryAnimation();
        }

        #region Inicialização

        /// <summary>
        /// Configura as animações iniciais
        /// </summary>
        private void InitializeAnimations()
        {
            _animatedElements = new List<FrameworkElement>();

            _animationTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(50)
            };
            _animationTimer.Tick += AnimationTimer_Tick;
        }

        /// <summary>
        /// Configura os event handlers
        /// </summary>
        private void SetupEventHandlers()
        {
            // Quando o UserControl é carregado
            this.Loaded += Introducao_Loaded;

            // Quando o UserControl é descarregado
            this.Unloaded += Introducao_Unloaded;

            // Adiciona handlers para elementos interativos
            AddInteractiveHandlers();
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Evento quando o controle é carregado
        /// </summary>
        private void Introducao_Loaded(object sender, RoutedEventArgs e)
        {
            // Aplica tema se necessário
            ApplyTheme();

            // Registra teclas de atalho
            RegisterKeyboardShortcuts();
        }

        /// <summary>
        /// Evento quando o controle é descarregado
        /// </summary>
        private void Introducao_Unloaded(object sender, RoutedEventArgs e)
        {
            // Limpa recursos
            _animationTimer?.Stop();
            _animationTimer = null;
            _animatedElements?.Clear();
        }

        /// <summary>
        /// Timer para animações sequenciais
        /// </summary>
        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            // Implementa lógica de animação sequencial se necessário
        }

        #endregion

        #region Animações

        /// <summary>
        /// Inicia animação de entrada do conteúdo
        /// </summary>
        private void StartEntryAnimation()
        {
            // Cria storyboard para fade in
            var storyboard = new Storyboard();

            // Animação de opacidade
            var fadeIn = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = new Duration(TimeSpan.FromMilliseconds(800)),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };

            Storyboard.SetTarget(fadeIn, this);
            Storyboard.SetTargetProperty(fadeIn, new PropertyPath(OpacityProperty));

            // Animação de transformação
            var translateTransform = new TranslateTransform(0, 20);
            this.RenderTransform = translateTransform;

            var slideUp = new DoubleAnimation
            {
                From = 20,
                To = 0,
                Duration = new Duration(TimeSpan.FromMilliseconds(800)),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };

            Storyboard.SetTarget(slideUp, this);
            Storyboard.SetTargetProperty(slideUp, new PropertyPath("RenderTransform.Y"));

            storyboard.Children.Add(fadeIn);
            storyboard.Children.Add(slideUp);

            storyboard.Begin();
        }

        /// <summary>
        /// Anima cards ao entrar na viewport
        /// </summary>
        private void AnimateCardOnScroll(Border card)
        {
            if (card == null || _animatedElements.Contains(card)) return;

            // Verifica se o card está visível
            if (IsElementVisible(card))
            {
                var storyboard = new Storyboard();

                // Fade in
                var fadeIn = new DoubleAnimation
                {
                    From = 0,
                    To = 1,
                    Duration = new Duration(TimeSpan.FromMilliseconds(600)),
                    EasingFunction = new PowerEase { EasingMode = EasingMode.EaseOut }
                };

                Storyboard.SetTarget(fadeIn, card);
                Storyboard.SetTargetProperty(fadeIn, new PropertyPath(OpacityProperty));

                storyboard.Children.Add(fadeIn);
                storyboard.Begin();

                _animatedElements.Add(card);
            }
        }

        /// <summary>
        /// Cria efeito de pulse em elemento
        /// </summary>
        private void CreatePulseEffect(FrameworkElement element)
        {
            var storyboard = new Storyboard();

            var scaleX = new DoubleAnimation
            {
                From = 1.0,
                To = 1.05,
                Duration = new Duration(TimeSpan.FromMilliseconds(300)),
                AutoReverse = true,
                EasingFunction = new SineEase { EasingMode = EasingMode.EaseInOut }
            };

            var scaleY = new DoubleAnimation
            {
                From = 1.0,
                To = 1.05,
                Duration = new Duration(TimeSpan.FromMilliseconds(300)),
                AutoReverse = true,
                EasingFunction = new SineEase { EasingMode = EasingMode.EaseInOut }
            };

            var scaleTransform = new ScaleTransform(1.0, 1.0);
            element.RenderTransform = scaleTransform;
            element.RenderTransformOrigin = new Point(0.5, 0.5);

            Storyboard.SetTarget(scaleX, element);
            Storyboard.SetTargetProperty(scaleX, new PropertyPath("RenderTransform.ScaleX"));

            Storyboard.SetTarget(scaleY, element);
            Storyboard.SetTargetProperty(scaleY, new PropertyPath("RenderTransform.ScaleY"));

            storyboard.Children.Add(scaleX);
            storyboard.Children.Add(scaleY);

            storyboard.Begin();
        }

        #endregion

        #region Interatividade

        /// <summary>
        /// Adiciona handlers para elementos interativos
        /// </summary>
        private void AddInteractiveHandlers()
        {
            // Encontra todos os TabItems e adiciona handlers
            var tabControl = FindTabControl(this);
            if (tabControl != null)
            {
                foreach (TabItem tab in tabControl.Items)
                {
                    tab.PreviewMouseLeftButtonDown += Tab_PreviewMouseLeftButtonDown;
                    tab.MouseEnter += Tab_MouseEnter;
                    tab.MouseLeave += Tab_MouseLeave;
                }

                tabControl.SelectionChanged += TabControl_SelectionChanged;
            }

            // Adiciona handlers para cards
            var cards = FindVisualChildren<Border>(this).Where(b => b.Style != null);
            foreach (var card in cards)
            {
                card.MouseEnter += Card_MouseEnter;
                card.MouseLeave += Card_MouseLeave;
                card.PreviewMouseLeftButtonDown += Card_Click;
            }
        }

        /// <summary>
        /// Handler para hover em tabs
        /// </summary>
        private void Tab_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is TabItem tab && !tab.IsSelected)
            {
                // Aplica efeito de hover customizado
                AnimateTabHover(tab, true);
            }
        }

        /// <summary>
        /// Handler para mouse leave em tabs
        /// </summary>
        private void Tab_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is TabItem tab && !tab.IsSelected)
            {
                AnimateTabHover(tab, false);
            }
        }

        /// <summary>
        /// Handler para clique em tab
        /// </summary>
        private void Tab_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is TabItem tab)
            {
                // Adiciona efeito de clique
                CreateClickRipple(tab, e.GetPosition(tab));
            }
        }

        /// <summary>
        /// Handler quando a seleção do tab muda
        /// </summary>
        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is TabControl tabControl)
            {
                _currentTabIndex = tabControl.SelectedIndex;

                // Anima conteúdo do novo tab
                AnimateTabContent(tabControl.SelectedItem as TabItem);

                // Dispara evento customizado
                OnTabChanged(_currentTabIndex);
            }
        }

        /// <summary>
        /// Handler para hover em cards
        /// </summary>
        private void Card_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Border card)
            {
                // Adiciona glow effect
                AddGlowEffect(card);
            }
        }

        /// <summary>
        /// Handler para mouse leave em cards
        /// </summary>
        private void Card_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Border card)
            {
                RemoveGlowEffect(card);
            }
        }

        /// <summary>
        /// Handler para clique em cards
        /// </summary>
        private void Card_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border card)
            {
                CreatePulseEffect(card);

                // Dispara evento customizado
                OnCardClicked(card);
            }
        }

        #endregion

        #region Efeitos Visuais

        /// <summary>
        /// Adiciona efeito de glow
        /// </summary>
        private void AddGlowEffect(FrameworkElement element)
        {
            var glowEffect = new DropShadowEffect
            {
                Color = Colors.Cyan,
                BlurRadius = 20,
                ShadowDepth = 0,
                Opacity = 0
            };

            element.Effect = glowEffect;

            var animation = new DoubleAnimation
            {
                To = 0.8,
                Duration = new Duration(TimeSpan.FromMilliseconds(200)),
                EasingFunction = new PowerEase { EasingMode = EasingMode.EaseOut }
            };

            glowEffect.BeginAnimation(DropShadowEffect.OpacityProperty, animation);
        }

        /// <summary>
        /// Remove efeito de glow
        /// </summary>
        private void RemoveGlowEffect(FrameworkElement element)
        {
            if (element.Effect is DropShadowEffect effect)
            {
                var animation = new DoubleAnimation
                {
                    To = 0,
                    Duration = new Duration(TimeSpan.FromMilliseconds(200)),
                    EasingFunction = new PowerEase { EasingMode = EasingMode.EaseOut }
                };

                animation.Completed += (s, e) => element.Effect = null;
                effect.BeginAnimation(DropShadowEffect.OpacityProperty, animation);
            }
        }

        /// <summary>
        /// Cria efeito ripple ao clicar
        /// </summary>
        private void CreateClickRipple(FrameworkElement element, Point position)
        {
            // Implementação do efeito ripple
            var ripple = new Border
            {
                Width = 10,
                Height = 10,
                CornerRadius = new CornerRadius(5),
                Background = new SolidColorBrush(Colors.White),
                Opacity = 0.5,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(position.X - 5, position.Y - 5, 0, 0),
                RenderTransform = new ScaleTransform(1, 1),
                RenderTransformOrigin = new Point(0.5, 0.5)
            };

            if (element is Panel panel)
            {
                panel.Children.Add(ripple);

                var storyboard = new Storyboard();

                // Scale animation
                var scaleX = new DoubleAnimation
                {
                    From = 1,
                    To = 10,
                    Duration = new Duration(TimeSpan.FromMilliseconds(500))
                };

                var scaleY = new DoubleAnimation
                {
                    From = 1,
                    To = 10,
                    Duration = new Duration(TimeSpan.FromMilliseconds(500))
                };

                // Fade out animation
                var fadeOut = new DoubleAnimation
                {
                    From = 0.5,
                    To = 0,
                    Duration = new Duration(TimeSpan.FromMilliseconds(500))
                };

                Storyboard.SetTarget(scaleX, ripple);
                Storyboard.SetTargetProperty(scaleX, new PropertyPath("RenderTransform.ScaleX"));

                Storyboard.SetTarget(scaleY, ripple);
                Storyboard.SetTargetProperty(scaleY, new PropertyPath("RenderTransform.ScaleY"));

                Storyboard.SetTarget(fadeOut, ripple);
                Storyboard.SetTargetProperty(fadeOut, new PropertyPath(OpacityProperty));

                storyboard.Children.Add(scaleX);
                storyboard.Children.Add(scaleY);
                storyboard.Children.Add(fadeOut);

                storyboard.Completed += (s, e) => panel.Children.Remove(ripple);
                storyboard.Begin();
            }
        }

        /// <summary>
        /// Anima hover do tab
        /// </summary>
        private void AnimateTabHover(TabItem tab, bool isEntering)
        {
            var animation = new DoubleAnimation
            {
                To = isEntering ? 1.05 : 1.0,
                Duration = new Duration(TimeSpan.FromMilliseconds(150)),
                EasingFunction = new PowerEase { EasingMode = EasingMode.EaseOut }
            };

            if (tab.RenderTransform is ScaleTransform transform)
            {
                transform.BeginAnimation(ScaleTransform.ScaleXProperty, animation);
                transform.BeginAnimation(ScaleTransform.ScaleYProperty, animation);
            }
            else
            {
                var scaleTransform = new ScaleTransform(1, 1);
                tab.RenderTransform = scaleTransform;
                tab.RenderTransformOrigin = new Point(0.5, 0.5);
                scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, animation);
                scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, animation);
            }
        }

        /// <summary>
        /// Anima conteúdo do tab
        /// </summary>
        private void AnimateTabContent(TabItem tab)
        {
            if (tab?.Content is FrameworkElement content)
            {
                content.Opacity = 0;

                var fadeIn = new DoubleAnimation
                {
                    From = 0,
                    To = 1,
                    Duration = new Duration(TimeSpan.FromMilliseconds(300)),
                    EasingFunction = new PowerEase { EasingMode = EasingMode.EaseOut }
                };

                content.BeginAnimation(OpacityProperty, fadeIn);
            }
        }

        #endregion

        #region Métodos Auxiliares

        /// <summary>
        /// Encontra o TabControl no visual tree
        /// </summary>
        private TabControl FindTabControl(DependencyObject parent)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child is TabControl tabControl)
                    return tabControl;

                var result = FindTabControl(child);
                if (result != null)
                    return result;
            }

            return null;
        }

        /// <summary>
        /// Encontra todos os filhos visuais de um tipo
        /// </summary>
        private IEnumerable<T> FindVisualChildren<T>(DependencyObject parent) where T : DependencyObject
        {
            if (parent != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
                {
                    var child = VisualTreeHelper.GetChild(parent, i);

                    if (child is T typedChild)
                        yield return typedChild;

                    foreach (var childOfChild in FindVisualChildren<T>(child))
                        yield return childOfChild;
                }
            }
        }

        /// <summary>
        /// Verifica se elemento está visível na viewport
        /// </summary>
        private bool IsElementVisible(FrameworkElement element)
        {
            if (!element.IsVisible)
                return false;

            var container = VisualTreeHelper.GetParent(element) as FrameworkElement;
            if (container == null)
                return false;

            var bounds = element.TransformToAncestor(container)
                              .TransformBounds(new Rect(0, 0, element.ActualWidth, element.ActualHeight));

            var containerBounds = new Rect(0, 0, container.ActualWidth, container.ActualHeight);

            return containerBounds.IntersectsWith(bounds);
        }

        /// <summary>
        /// Aplica tema baseado nas preferências
        /// </summary>
        private void ApplyTheme()
        {
            // Implementa lógica de tema se necessário
            // Por exemplo, modo escuro/claro
        }

        /// <summary>
        /// Registra atalhos de teclado
        /// </summary>
        private void RegisterKeyboardShortcuts()
        {
            // Ctrl+1, Ctrl+2, Ctrl+3 para navegar entre tabs
            this.PreviewKeyDown += (s, e) =>
            {
                if (Keyboard.Modifiers == ModifierKeys.Control)
                {
                    var tabControl = FindTabControl(this);
                    if (tabControl != null)
                    {
                        switch (e.Key)
                        {
                            case Key.D1:
                            case Key.NumPad1:
                                tabControl.SelectedIndex = 0;
                                e.Handled = true;
                                break;
                            case Key.D2:
                            case Key.NumPad2:
                                tabControl.SelectedIndex = 1;
                                e.Handled = true;
                                break;
                            case Key.D3:
                            case Key.NumPad3:
                                tabControl.SelectedIndex = 2;
                                e.Handled = true;
                                break;
                        }
                    }
                }
            };
        }

        #endregion

        #region Eventos Públicos

        /// <summary>
        /// Evento disparado quando um tab é alterado
        /// </summary>
        public event EventHandler<int> TabChanged;

        /// <summary>
        /// Evento disparado quando um card é clicado
        /// </summary>
        public event EventHandler<Border> CardClicked;

        /// <summary>
        /// Dispara evento de mudança de tab
        /// </summary>
        protected virtual void OnTabChanged(int tabIndex)
        {
            TabChanged?.Invoke(this, tabIndex);
        }

        /// <summary>
        /// Dispara evento de clique em card
        /// </summary>
        protected virtual void OnCardClicked(Border card)
        {
            CardClicked?.Invoke(this, card);
        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Navega para um tab específico
        /// </summary>
        public void NavigateToTab(int index)
        {
            var tabControl = FindTabControl(this);
            if (tabControl != null && index >= 0 && index < tabControl.Items.Count)
            {
                tabControl.SelectedIndex = index;
            }
        }

        /// <summary>
        /// Retorna o índice do tab atual
        /// </summary>
        public int GetCurrentTabIndex()
        {
            return _currentTabIndex;
        }

        /// <summary>
        /// Inicia tour guiado pela interface
        /// </summary>
        public void StartGuidedTour()
        {
            // Implementa tour guiado se necessário
            var tourSteps = new List<string>
            {
                "Bem-vindo ao Quantum Forge!",
                "Explore os conceitos de Computação Clássica e Quântica",
                "Entenda os princípios da Física que fundamentam a computação quântica",
                "Veja a evolução histórica desta tecnologia revolucionária"
            };

            // Implementar lógica do tour
        }

        /// <summary>
        /// Exporta conteúdo atual
        /// </summary>
        public void ExportContent()
        {
            // Implementa exportação do conteúdo se necessário
        }

        #endregion
    }
}