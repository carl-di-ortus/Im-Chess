using System.Windows;
using CoreUtils;

namespace Im_Chess
{
    public partial class PromoteWindow : Window
    {
        public PromoteWindow(Player player)
        {
            InitializeComponent();
            PromoteBoard.ShowForPlayer(player);
        }

        public PieceType PromotePieceType
        {
            get
            {
                return PromoteBoard.PromotedType;
            } 
        }
    }
}
