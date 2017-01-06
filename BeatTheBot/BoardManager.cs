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
    class BoardManager
    {
        private MainWindow mainWindow;
        int[] currentItemNums = new int[2];
        Brush[,] colorsToUse = new Brush[3, 10];


        public BoardManager(MainWindow mainWindowHandle)
        {
            mainWindow = mainWindowHandle;
            mainWindow.megaBoardHolder.Children.Add(buildBoard());
            setColors();
            //addRule(1, 0, 1, 0);
            //addRule(2, 0, 1, 0);
        }
        private void setColors()
        {
            //Bot 1 is red
            colorsToUse[0, 0] = Brushes.Red;//Rule Dockpanel fill
            colorsToUse[0, 1] = new SolidColorBrush(MediaColor.FromRgb(218, 0, 0));//Rule Dockpanel foreground
            colorsToUse[0, 2] = Brushes.White;//Rule Dockpanel text
            colorsToUse[0, 3] = Brushes.Red;//Pieces color
            colorsToUse[0, 4] = new SolidColorBrush(MediaColor.FromRgb(255, 155, 155));//Won square color
            colorsToUse[0, 5] = new SolidColorBrush(MediaColor.FromRgb(255, 91, 91));//Won square border
            //colorsToUse[0, 6] = new SolidColorBrush(MediaColor.FromRgb(255, 210, 210));//Won game color

            //Bot 2 is blue
            colorsToUse[1, 0] = new SolidColorBrush(MediaColor.FromRgb(0, 176, 240));
            colorsToUse[1, 1] = new SolidColorBrush(MediaColor.FromRgb(0, 136, 255));
            colorsToUse[1, 2] = Brushes.White;
            colorsToUse[1, 3] = new SolidColorBrush(MediaColor.FromRgb(0, 176, 240));
            colorsToUse[1, 4] = new SolidColorBrush(MediaColor.FromRgb(119, 219, 255));
            colorsToUse[1, 5] = new SolidColorBrush(MediaColor.FromRgb(64, 204, 255));
            //colorsToUse[1, 6] = new SolidColorBrush(MediaColor.FromRgb(191, 238, 255));//Won game color

            //Other colors
            colorsToUse[2, 0] = new SolidColorBrush(MediaColor.FromRgb(238, 238, 238));
            colorsToUse[2, 1] = new SolidColorBrush(MediaColor.FromRgb(255, 255, 166));
            colorsToUse[2, 3] = Brushes.White;
            colorsToUse[2, 5] = Brushes.White;
            //colorsToUse[2, 5] = new SolidColorBrush(MediaColor.FromRgb(80, 80, 80));
            //colorsToUse[2, 6] = new SolidColorBrush(MediaColor.FromRgb(0, 0, 0));//Tied game
        }
        private Border buildBoard()
        {
            Border border1 = new Border();
            border1.Name = "megaBoard";
            mainWindow.RegisterName(border1.Name, border1);
            border1.SnapsToDevicePixels = true;
            border1.Width = 250;
            border1.Height = 250;
            border1.Background = new SolidColorBrush(MediaColor.FromRgb(238, 238, 238));
            border1.BorderBrush = Brushes.Black;
            border1.BorderThickness = new Thickness(2);
            UniformGrid grid1 = new UniformGrid();
            for (int i = 0; i < 9; i++)
            {
                grid1.Children.Add(buildSquare(i));
            }
            border1.Child = grid1;
            return border1;
        }
        private Border buildSquare(int squareIndex)
        {
            Border border1 = new Border();
            border1.Name = "s" + squareIndex;
            mainWindow.RegisterName(border1.Name, border1);
            border1.SnapsToDevicePixels = true;
            border1.Background = new SolidColorBrush(MediaColor.FromRgb(238, 238, 238));
            border1.BorderBrush = Brushes.Black;
            border1.BorderThickness = new Thickness(2);
            UniformGrid uniformGrid1 = new UniformGrid();
            for (int i = 0; i < 9; i++)
            {
                Border currentCell = buildCell(i, squareIndex);
                uniformGrid1.Children.Add(currentCell);
            }
            border1.Child = uniformGrid1;
            return border1;
        }
        private Border buildCell(int cellIndex, int squareIndex)
        {
            Border border1 = new Border();
            border1.Name = "s" + squareIndex + "c" + cellIndex;
            mainWindow.RegisterName(border1.Name, border1);
            border1.SnapsToDevicePixels = true;
            border1.Background = colorsToUse[2, 0];
            border1.BorderBrush = Brushes.LightGray;
            border1.BorderThickness = new Thickness(1);
            border1.MouseUp += new MouseButtonEventHandler(mainWindow.cell_OnClick);
            return border1;
        }
        public void addRule(int botNum, int index1, int index2, int index3)
        {
            int currentItemNum = currentItemNums[botNum - 1];
            DockPanel dockPanel1 = new DockPanel();
            dockPanel1.Name = "bot" + botNum + "RuleDockPanelBN" + currentItemNum;
            mainWindow.RegisterName(dockPanel1.Name, dockPanel1);
            dockPanel1.Background = colorsToUse[botNum - 1, 0];
            dockPanel1.Margin = new Thickness(10, 10, 10, 0);
            dockPanel1.Height = 33;

            TextBlock removeButton = buildTextBlockBN("bot" + botNum + "RemoveRuleBN" + currentItemNum, "X", botNum);
            removeButton.MouseLeftButtonUp += new MouseButtonEventHandler(mainWindow.botRemoveRuleBN_Click);
            dockPanel1.Children.Add(removeButton);
            DockPanel.SetDock(removeButton, Dock.Left);

            TextBlock moveUpButton = buildTextBlockBN("bot" + botNum + "MoveRuleUpBN" + currentItemNum, "˄", botNum);
            moveUpButton.Margin = new Thickness(5, 5, 0, 5);
            moveUpButton.MouseLeftButtonUp += new MouseButtonEventHandler(mainWindow.botMoveRuleUpBN_Click);
            dockPanel1.Children.Add(moveUpButton);
            DockPanel.SetDock(moveUpButton, Dock.Left);

            TextBlock moveDownButton = buildTextBlockBN("bot" + botNum + "MoveRuleDownBN" + currentItemNum, "˅", botNum);
            moveDownButton.Margin = new Thickness(0, 5, 5, 5);
            moveDownButton.MouseLeftButtonUp += new MouseButtonEventHandler(mainWindow.botMoveRuleDownBN_Click);
            dockPanel1.Children.Add(moveDownButton);
            DockPanel.SetDock(moveDownButton, Dock.Left);

            string[] items1 = { "Always", "Never", "Make opponant", "Encourage opponant to", "Don't let opponant" };
            ComboBox combobox1 = buildComboBoxWithItems(items1);
            combobox1.Name = "bot" + botNum + "Combobox1Num" + currentItemNum;
            mainWindow.RegisterName(combobox1.Name, combobox1);
            combobox1.SelectionChanged += new SelectionChangedEventHandler(mainWindow.botComboBox_SelectionChanged);
            combobox1.Margin = new Thickness(5);
            dockPanel1.Children.Add(combobox1);
            combobox1.SelectedIndex = index1;
            DockPanel.SetDock(moveDownButton, Dock.Left);

            string[] items2 = { "Play in squares", "Play in cells", "Win squares" };
            ComboBox combobox2 = buildComboBoxWithItems(items2);
            combobox2.Name = "bot" + botNum + "Combobox2Num" + currentItemNum;
            mainWindow.RegisterName(combobox2.Name, combobox2);
            combobox2.SelectionChanged += new SelectionChangedEventHandler(mainWindow.botComboBox_SelectionChanged);
            combobox2.Margin = new Thickness(5);
            dockPanel1.Children.Add(combobox2);
            combobox2.SelectedIndex = index2;
            DockPanel.SetDock(moveDownButton, Dock.Left);

            string[] items3 = new string[18];
            items3[0] = "(any)";
            items3[1] = "That win the game";
            items3[2] = "Chosen randomly";
            items3[3] = "That block";
            items3[4] = "That make 2 in a row";
            items3[5] = "That give free momevent";
            items3[6] = "That are the center";
            items3[7] = "That are corners";
            items3[8] = "That are edges";
            items3[9] = "in the top left";
            items3[10] = "in the top center";
            items3[11] = "in the top right";
            items3[12] = "in the middle left";
            items3[13] = "in the middle center";
            items3[14] = "in the middle right";
            items3[15] = "in the bottom left";
            items3[16] = "in the bottom center";
            items3[17] = "in the bottom right";
            ComboBox combobox3 = buildComboBoxWithItems(items3);
            combobox3.Name = "bot" + botNum + "Combobox3Num" + currentItemNum;
            mainWindow.RegisterName(combobox3.Name, combobox3);
            combobox3.SelectionChanged += new SelectionChangedEventHandler(mainWindow.botComboBox_SelectionChanged);
            combobox3.Margin = new Thickness(5);
            dockPanel1.Children.Add(combobox3);
            combobox3.SelectedIndex = index3;

            TextBlock textblock1 = new TextBlock();
            dockPanel1.Children.Add(textblock1);

            StackPanel botRules = (StackPanel)mainWindow.FindName("bot" + botNum + "Rules");
            botRules.Children.Add(dockPanel1);
            ((ScrollViewer)mainWindow.FindName("bot" + botNum + "RuleScrollViewer")).ScrollToBottom();
            currentItemNums[botNum - 1]++;
        }
        public void removeRule(int botNum, int itemNum)
        {
            StackPanel botRules = (StackPanel)mainWindow.FindName("bot" + botNum + "Rules");
            //"bot1RuleDockPanelBN0"
            string dockPanelName = "bot" + botNum + "RuleDockPanelBN" + itemNum;
            string removeBNName = "bot" + botNum + "RemoveRuleBN" + itemNum;
            DockPanel ruleToRemove = (DockPanel)mainWindow.FindName(dockPanelName);
            botRules.Children.Remove(ruleToRemove);
            mainWindow.UnregisterName(dockPanelName);
            mainWindow.UnregisterName(removeBNName);
            mainWindow.UnregisterName("bot" + botNum + "Combobox1Num" + itemNum);
            mainWindow.UnregisterName("bot" + botNum + "Combobox2Num" + itemNum);
            mainWindow.UnregisterName("bot" + botNum + "Combobox3Num" + itemNum);
        }
        public void moveRule(int botNum, int itemNum, int direction)
        {
            StackPanel botRules = (StackPanel)mainWindow.FindName("bot" + botNum + "Rules");
            string dockPanelName = "bot" + botNum + "RuleDockPanelBN" + itemNum;
            DockPanel ruleToMoveUp = (DockPanel)mainWindow.FindName(dockPanelName);
            int currentLocation = botRules.Children.IndexOf(ruleToMoveUp);
            if (currentLocation + direction >= 0 && currentLocation + direction < botRules.Children.Count)
            {
                botRules.Children.Remove(ruleToMoveUp);
                botRules.Children.Insert(currentLocation + direction, ruleToMoveUp);
            }
        }
        public void updateBoard(int legalSquare, int[][] boardState, int[] megaBoardState, int winnerState, int currentBot)
        {
            Border currentmBorder = (Border)mainWindow.FindName("megaBoard");
            if (winnerState != 0)
            {
                currentmBorder.BorderBrush = colorsToUse[winnerState - 1, 3];
                if (winnerState != 3)
                {
                    mainWindow.botMakeMoveBN.Content = "Bot " + winnerState + " Wins!";
                    mainWindow.botMakeMoveBN.Foreground = colorsToUse[winnerState - 1, 3];
                }
                else
                {
                    mainWindow.botMakeMoveBN.Content = "Tie Game!";
                    mainWindow.botMakeMoveBN.Foreground = Brushes.Black;
                }

            }
            else
            {
                currentmBorder.BorderBrush = Brushes.Black;
                mainWindow.botMakeMoveBN.Content = "Bot " + currentBot;
                mainWindow.botMakeMoveBN.Foreground = colorsToUse[currentBot - 1, 3];
            }
            

            for (int square = 0; square < 9; square++)
            {
                Border currentSBorder = (Border)mainWindow.FindName("s" + square);
                if (winnerState != 0)
                {
                    currentSBorder.BorderBrush = colorsToUse[winnerState - 1, 3];
                }
                else
                {
                    currentSBorder.BorderBrush = Brushes.Black;
                }
                for (int cell = 0; cell < 9; cell++)
                {
                    Border currentBorder = (Border)mainWindow.FindName("s" + square + "c" + cell);
                    int cellVal = boardState[square][cell];
                    if (cellVal != 0)
                    {
                        currentBorder.Background = colorsToUse[cellVal - 1, 3];
                    }
                    else if (megaBoardState[square] == 1 || megaBoardState[square] == 2)
                    {
                        currentBorder.Background = colorsToUse[megaBoardState[square] - 1, 4];
                    }
                    else if (winnerState != 0)
                    {
                        currentBorder.Background = colorsToUse[2, 0];
                    }
                    else if (square == legalSquare || legalSquare == -1)
                    {
                        currentBorder.Background = colorsToUse[2, 1];//Yellow
                    }
                    else
                    {
                        currentBorder.Background = colorsToUse[2, 0];
                    }
                    if (megaBoardState[square] != 0)
                    {
                        currentBorder.BorderBrush = colorsToUse[megaBoardState[square] - 1, 5];
                    }
                    else
                    {
                        currentBorder.BorderBrush = Brushes.LightGray;
                    }
                }
            }
        }
        public int[][] readRules(int botNum)
        {
            StackPanel botRulesPanel = (StackPanel)mainWindow.FindName("bot" + botNum + "Rules");
            int count = botRulesPanel.Children.Count;
            int[][] rules = new int[count][];
            for (int i = 0; i < count; i++)
            {
                rules[i] = readRule(botNum, i, botRulesPanel);
            }
            return rules;
        }
        private int[] readRule(int botNum, int ruleIndex, StackPanel botRulesPanel)
        {
            DockPanel dockPanel = (DockPanel)botRulesPanel.Children[ruleIndex];
            int itemNum = getRuleNum(dockPanel, "bot0RuleDockPanelBN");
            string comboNameStart = "bot" + botNum + "Combobox";
            string comboNameEnd = "Num" + itemNum;
            //bot1ComboboxNum0
            ComboBox comboBox1 = (ComboBox)mainWindow.FindName(comboNameStart + 1 + comboNameEnd);
            ComboBox comboBox2 = (ComboBox)mainWindow.FindName(comboNameStart + 2 + comboNameEnd);
            ComboBox comboBox3 = (ComboBox)mainWindow.FindName(comboNameStart + 3 + comboNameEnd);
            int[] ruleSelections = new int[3];
            ruleSelections[0] = comboBox1.SelectedIndex;
            ruleSelections[1] = comboBox2.SelectedIndex;
            ruleSelections[2] = comboBox3.SelectedIndex;
            return ruleSelections;
        }
        private TextBlock buildTextBlockBN(string name, string text, int botNum)
        {
            TextBlock currentTextBlock = new TextBlock();


            //TextBlock removeButton = new TextBlock();
            currentTextBlock.Name = name;
            mainWindow.RegisterName(currentTextBlock.Name, currentTextBlock);
            //currentTextBlock.MouseLeftButtonUp += new MouseButtonEventHandler(mainWindow.botRemoveRuleBN_Click);
            currentTextBlock.Margin = new Thickness(5);
            currentTextBlock.Width = 20;
            currentTextBlock.Text = text;
            currentTextBlock.Background = colorsToUse[botNum - 1, 1];
            currentTextBlock.Padding = new Thickness(3);
            currentTextBlock.Foreground = colorsToUse[1, 2];
            currentTextBlock.TextAlignment = TextAlignment.Center;
            currentTextBlock.SnapsToDevicePixels = true;
            //dockPanel1.Children.Add(removeButton);
            //DockPanel.SetDock(removeButton, Dock.Left);
            return currentTextBlock;
        }
        private ComboBox buildComboBoxWithItems(string[] items)
        {
            ComboBox combobox1 = new ComboBox();
            foreach (string currentItem in items)
            {
                ComboBoxItem item1 = new ComboBoxItem();
                item1.Content = currentItem;
                combobox1.Items.Add(item1);
            }
            return combobox1;
        }
        public int getBotNum(object sender)
        {
            string senderName;
            try
            {
                Control castedSender = (Control)sender;
                senderName = castedSender.Name;
            }
            catch
            {
                TextBlock castedSender = (TextBlock)sender;
                senderName = castedSender.Name;
            }
            if (senderName.Substring(0, 4) == "bot1")
            {
                return 1;
            }
            else if (senderName.Substring(0, 4) == "bot2")
            {
                return 2;
            }
            string crashMessage = "getBotNumber crashed:" + senderName.Substring(0, 4);
            MessageBox.Show(crashMessage);
            throw new Exception(crashMessage);
        }
        public int getRuleNum(object sender, string sample)
        {
            string clippedName;
            try
            {
                FrameworkElement castedSender = (FrameworkElement)sender;
                clippedName = castedSender.Name.Substring(sample.Length);
            }
            catch
            {
                try
                {
                    Control castedSender = (Control)sender;
                    clippedName = castedSender.Name.Substring(sample.Length);
                }
                catch
                {
                    TextBlock castedSender = (TextBlock)sender;
                    clippedName = castedSender.Name.Substring(sample.Length);
                }
            }
            return Convert.ToInt32(clippedName);
        }
        public void importRules(int botNum, string rules)
        {
            string[] rulesArray = rules.Split(';');
            int[][] rulesArray2 = new int[rulesArray.Length][];
            bool error = false;
            for (int i = 0; i < rulesArray.Length; i++)
            {
                rulesArray2[i] = new int[3];
                string[] ruleString = rulesArray[i].Split(',');
                try
                {
                    rulesArray2[i][0] = Convert.ToInt32(ruleString[0]);
                    rulesArray2[i][1] = Convert.ToInt32(ruleString[1]);
                    rulesArray2[i][2] = Convert.ToInt32(ruleString[2]);
                }
                catch
                {
                    error = true;
                }
            }
            if (error)
            {
                MessageBox.Show("Not valid");
                return;
            }
            for (int i = 0; i < rulesArray2.Length; i++)
            {
                addRule(botNum, rulesArray2[i][0], rulesArray2[i][1], rulesArray2[i][2]);
            }
        }
        public string exportRules(int botNum)
        {
            int[][] rules = readRules(botNum);
            string output = "";
            for (int i = 0; i < rules.Length; i++)
            {
                if (i != 0)
                {
                    output += ";";
                }
                output += rules[i][0] + "," + rules[i][1] + "," + rules[i][2];
            }
            return output;
        }
    }
}
