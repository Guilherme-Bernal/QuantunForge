using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Quantun_Forge.src.views
{
    /// <summary>
    /// Módulo educativo sobre os Limites da Física Clássica
    /// Demonstra a transição do paradigma clássico para o quântico
    /// </summary>
    public partial class LimitesClassicos : UserControl
    {
        public LimitesClassicos()
        {
            InitializeComponent();
            InicializarAnimacoes();
        }

        /// <summary>
        /// Inicializa as animações de entrada do módulo
        /// </summary>
        private void InicializarAnimacoes()
        {
            // Aplica animação de fade-in ao carregar o controle
            Loaded += (sender, e) =>
            {
                var fadeIn = FindResource("FadeInAnimation") as Storyboard;
                fadeIn?.Begin(this);
            };
        }
    }
}