using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MediaColor = System.Windows.Media.Color;

namespace BeatTheBot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private BoardManager boardManager;
        private GameManager gameManager;
        public MainWindow()
        {
            InitializeComponent();
            boardManager = new BoardManager(this);
            boardManager.importRules(1, "0,1,1;4,1,1;4,2,0;0,1,3;0,2,0;0,1,4;0,1,6;0,1,7");
            boardManager.importRules(2, "0,1,1;4,1,1;0,2,6;0,2,4;0,2,7;0,2,0;0,1,6;0,1,4;0,1,7");
        }
        public void cell_OnClick(object sender, MouseButtonEventArgs e)
        {
            Border cellClicked = (Border)sender;
            int square = Convert.ToInt32(cellClicked.Name[1] + "");
            int cell = Convert.ToInt32(cellClicked.Name[3] + "");
            gameManager.cellClicked(square, cell);
        }

        private void botAddRuleBN_Click(object sender, RoutedEventArgs e)
        {
            int botNum = boardManager.getBotNum(sender);
            boardManager.addRule(botNum, 0, 1, 0);
        }
        public void botRemoveRuleBN_Click(object sender, MouseButtonEventArgs e)
        {
            int botNum = boardManager.getBotNum(sender);
            int ruleNum = boardManager.getRuleNum(sender, "bot0RemoveRuleBN");
            boardManager.removeRule(botNum, ruleNum);
        }
        public void botMoveRuleUpBN_Click(object sender, MouseButtonEventArgs e)
        {
            int botNum = boardManager.getBotNum(sender);
            int ruleNum = boardManager.getRuleNum(sender, "bot0MoveRuleUpBN");
            boardManager.moveRule(botNum, ruleNum, -1);
        }
        public void botMoveRuleDownBN_Click(object sender, MouseButtonEventArgs e)
        {
            int botNum = boardManager.getBotNum(sender);
            int ruleNum = boardManager.getRuleNum(sender, "bot0MoveRuleDownBN");
            boardManager.moveRule(botNum, ruleNum, 1);
        }
        public void botComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void botMakeMoveBN_Click(object sender, RoutedEventArgs e)
        {
            gameManager.botMakeMove(boardManager.getBotNum(sender));
        }
        private void botMakeMoveBN_Click2(object sender, RoutedEventArgs e)
        {
            gameManager.botMakeMove(gameManager.currentBot);
            
        }
        private void playBN_Click(object sender, RoutedEventArgs e)
        {
            gameManager = new GameManager(this, boardManager);
            playBN.Visibility = Visibility.Collapsed;
            makeMoveGrid.Visibility = Visibility.Visible;
        }
        private void resetBN_Click(object sender, RoutedEventArgs e)
        {
            gameManager = new GameManager(this, boardManager);
            playBN.Visibility = Visibility.Collapsed;
            makeMoveGrid.Visibility = Visibility.Visible;
        }

        private void botImportBN_Click(object sender, RoutedEventArgs e)
        {
            int botNum = boardManager.getBotNum(sender);
            TextBox textbox1 = (TextBox)FindName("bot" + botNum + "RuleTBX");
            boardManager.importRules(botNum, textbox1.Text);
        }

        private void botExportBN_Click(object sender, RoutedEventArgs e)
        {
            int botNum = boardManager.getBotNum(sender);
            TextBox textbox1 = (TextBox)FindName("bot" + botNum + "RuleTBX");
            textbox1.Text = boardManager.exportRules(botNum);
        }

        
    }

    public static class ExceptionHelper
    {
        public static int LineNumber(this Exception e)
        {

            int linenum = 0;
            try
            {
                linenum = Convert.ToInt32(e.StackTrace.Substring(e.StackTrace.LastIndexOf(":line") + 5));
            }
            catch
            {
                //Stack trace is not available!
            }
            return linenum;
        }
    }
}
