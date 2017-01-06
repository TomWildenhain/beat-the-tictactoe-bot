using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;//For Messagebox

namespace BeatTheBot
{
    class GameManager
    {
        private MainWindow mainWindow;
        private BoardManager boardManager;
        private int legalSquare = -1;
        public int currentBot = 1;
        private int[][] boardState = new int[9][];//[outerRow,outerColumn][innerRow,innerColumn]
        private int[] megaBoardState = new int[9];//[row,column]
        private int winnerState = 0;

        private int legalSquareB1 = -1;
        private int currentBotB1 = 1;
        private int[][] boardStateB1 = new int[9][];
        private int[] megaBoardStateB1 = new int[9];
        private int winnerStateB1 = 0;

        private int legalSquareB2 = -1;
        private int currentBotB2 = 1;
        private int[][] boardStateB2 = new int[9][];
        private int[] megaBoardStateB2 = new int[9];
        private int winnerStateB2 = 0;

        private int testingState = 0;
        private int[][][][] testBoardState = new int[9][][][];//[Outer place][Inner place][2 sets][Checks]
        //private int[][][] testBoardOpponentState = new int[9][][];//[Outer place][Inner place][Checks]
        private readonly int numOfChecks = 32;
        private int[] infoCatcher = new int[32];
        private int[] infoCatcherO = new int[32];
        Random rand = new Random();

        public GameManager(MainWindow mainWindowHandle, BoardManager boardManagerHandle)
        {
            mainWindow = mainWindowHandle;
            boardManager = boardManagerHandle;
            initializeBoard();
            startGame();
        }
        private void initializeBoard()
        {
            legalSquare = -1;
            winnerState = 0;
            boardState = new int[9][];
            megaBoardState = new int[9];
            for (int square = 0; square < 9; square++)
            {
                boardState[square] = new int[9];
            }
            clearTestBoard();
        }
        private void clearTestBoard()
        {
            testBoardState = new int[9][][][];
            for (int square = 0; square < 9; square++)
            {
                testBoardState[square] = new int[9][][];
                for (int cell = 0; cell < 9; cell++)
                {
                    testBoardState[square][cell] = new int[2][];
                    testBoardState[square][cell][0] = new int[numOfChecks];
                    testBoardState[square][cell][1] = new int[numOfChecks];
                }
            }
        }
        //private void clearTestBoardO()
        //{
        //    testBoardOpponentState = new int[9][][];
        //    for (int square = 0; square < 9; square++)
        //    {
        //        testBoardOpponentState[square] = new int[9][];
        //        for (int cell = 0; cell < 9; cell++)
        //        {
        //            testBoardOpponentState[square][cell] = new int[numOfChecks];
        //        }
        //    }
        //}
        private void joinInfoCatcherO()
        {
       
            for (int i = 0; i < numOfChecks; i++)
            {
                bool state = infoCatcher[i] == 1;
                int current = infoCatcherO[i];
                int endval = 2;
                if (current == 2 || (state && current == -1) || (!state && current == 1))
                {
                    if (i == 1)
                    {

                    }
                }
                else if (state) { endval = 1; }
                else { endval = -1; }
                infoCatcherO[i] = endval;
            }
             
        }
        public void startGame()
        {
            boardManager.updateBoard(legalSquare, boardState, megaBoardState, winnerState, currentBot);
        }
        public void cellClicked(int square, int cell)
        {
            if (checkMove(square, cell))
            {
                makeMove(square, cell, currentBot);
                boardManager.updateBoard(legalSquare, boardState, megaBoardState, winnerState, currentBot);
            }
        }
        private void makeMove(int square, int cell, int botNum)
        {
            boardState[square][cell] = botNum;
            legalSquare = cell;
            megaBoardState[square] = checkFor3InRow(boardState[square]);
            if (megaBoardState[square] != 0)
            {
                updateTestBoard(0);//It wins a square
            }
            winnerState = checkFor3InRow(megaBoardState);
            if (winnerState == botNum)
            {
                updateTestBoard(1);//It wins the game
            }
            checkBlockAnd2RowRules(square, cell, botNum);
            if (megaBoardState[cell] != 0)
            {
                legalSquare = square;
                if (megaBoardState[square] != 0)
                {
                    legalSquare = -1;
                    updateTestBoard(2);//It gives free movement
                }
            }

            currentBot = otherBot(currentBot);
        }
        private bool checkMove(int square, int cell)
        {
            return winnerState == 0 && (square == legalSquare || legalSquare == -1) && boardState[square][cell] == 0 && megaBoardState[square] == 0;
        }
        public void botMakeMove(int botNum)
        {
            int[][] botRules = boardManager.readRules(botNum);
            botTestAllMoves(botNum, 0, 2);
            int[][] gridOfElimination = new int[9][];
            for (int i = 0; i < 9; i++)
            {
                gridOfElimination[i] = new int[9];
                for (int j = 0; j < 9; j++)
                {
                    if (checkMove(i, j))
                        gridOfElimination[i][j] = 1;
                }
            }
            for (int i = 0; i < botRules.Length; i++)
            {
                gridOfElimination = applyRuleToGrid(gridOfElimination, botRules[i]);
            }
            List<int> validMoves = new List<int>();
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (gridOfElimination[i][j] == 1)
                    {
                        validMoves.Add(i * 9 + j);
                    }
                }
            }
            if (validMoves.Count > 0)
            {
                int move = validMoves.ElementAt(rand.Next(0, validMoves.Count));
                cellClicked(move / 9, move % 9);
            }
        }
        private int[][] applyRuleToGrid(int[][] grid, int[] rule)
        {
            int[][] ruleGrid = new int[9][];
            bool validMoves = false;
            for (int square = 0; square < 9; square++)
            {
                ruleGrid[square] = new int[9];
                for (int cell = 0; cell < 9; cell++)
                {
                    bool currentVal1 = true;
                    bool currentVal2 = true;
                    int index1 = getIndexToLookFor(rule[1], rule[2]);
                    int index2 = -1;
                    if (rule[1] == 2) index2 = 0;
                    int desiredVal = 0;
                    int index3 = 1;
                    if (rule[0] < 2) index3 = 0;
                    if (rule[0] == 0 || rule[0] == 2) desiredVal = 1;
                    if (rule[0] == 4) desiredVal = -1;
                    else if (rule[0] == 3) desiredVal = 2;
                    currentVal1 = index1 < 0 || testBoardState[square][cell][index3][index1] == desiredVal;
                    currentVal2 = index2 < 0 || testBoardState[square][cell][index3][index2] == desiredVal;
                    if (desiredVal == 2)
                    {
                        currentVal1 = currentVal1 || index1 < 0 || testBoardState[square][cell][index3][index1] == 1;
                        currentVal2 = currentVal2 || index2 < 0 || testBoardState[square][cell][index3][index2] == 1;
                    }
                    ruleGrid[square][cell] = 0;
                    bool currentVal3 = currentVal1 && currentVal2;
                    if (desiredVal == -1 && index1 >= 0 && index2 >= 0)
                    {
                        currentVal3 = currentVal1 || currentVal2;
                    }
                    if (currentVal3 && grid[square][cell] == 1)
                    {
                        ruleGrid[square][cell] = 1;
                        validMoves = true;
                    }
                }
            }
            if (validMoves)
            {
                return ruleGrid;
            }
            return grid;
        }
        private int getIndexToLookFor(int box2, int box3)
        {
            int index1 = -1;
            switch (box3)
            {
                case 0:
                    index1 = -1;
                    break;
                case 1:
                    index1 = 1;//Win the game?
                    break;
                case 2:
                    index1 = -1;//Random
                    break;
                case 3:
                    if (box2 == 1)
                    {
                        index1 = 3;//Block cells
                    }
                    else
                    {
                        index1 = 5;//Block squares
                    }
                    break;
                case 4:
                    if (box2 == 1)
                    {
                        index1 = 4;//Block cells
                    }
                    else
                    {
                        index1 = 6;//Block squares
                    }
                    break;
                case 5:
                    index1 = 2;//Free Movement
                    break;
                case 6:
                    goto case 8;
                case 7:
                    goto case 8;
                    break;
                case 8:
                    if (box2 == 1)
                    {
                        index1 = box3 + 2;//center,corners,edges for cells
                    }
                    else
                    {
                        index1 = box3 + 14;//center,corners,edges for squares
                    }
                    break;
                default:
                    if (box2 == 1)
                    {
                        index1 = box3 + 2;//speciic positions for cells
                    }
                    else
                    {
                        index1 = box3 + 14;//speciic positions for cells
                    }
                    break;
            }
            return index1;
        }
        private void botTestAllMoves(int botNum, int recurse, int backup)
        {
            backupState(backup, 0);
            for (int square = 0; square < 9; square++)
            {
                for (int cell = 0; cell < 9; cell++)
                {
                    if (checkMove(square, cell))
                    {
                        infoCatcher = new int[numOfChecks];
                        updateTestBoard(7);
                        makeMove(square, cell, botNum);
                        if (recurse == 0)
                        {
                            infoCatcher.CopyTo(testBoardState[square][cell][0], 0);
                            infoCatcherO = new int[numOfChecks];
                            botTestAllMoves(otherBot(botNum), 1, 1);
                            infoCatcherO.CopyTo(testBoardState[square][cell][1], 0);
                        }
                        else
                        {
                            joinInfoCatcherO();
                        }
                    }
                    backupState(0, backup);
                }
            }
            backupState(0, backup);
        }
        private void backupState(int to, int from)
        {
            int legalSquareF;
            int currentBotF;
            int[][] boardStateF;
            int[] megaBoardStateF;
            int[][] boardStateF2;
            int[] megaBoardStateF2;
            int winnerStateF;
            if (from == 0)
            {
                legalSquareF = legalSquare;
                currentBotF = currentBot;
                boardStateF = boardState;
                megaBoardStateF = megaBoardState;
                winnerStateF = winnerState;
            }
            else if (from == 1)
            {
                legalSquareF = legalSquareB1;
                currentBotF = currentBotB1;
                boardStateF = boardStateB1;
                megaBoardStateF = megaBoardStateB1;
                winnerStateF = winnerStateB1;
            }
            else
            {
                legalSquareF = legalSquareB2;
                currentBotF = currentBotB2;
                boardStateF = boardStateB2;
                megaBoardStateF = megaBoardStateB2;
                winnerStateF = winnerStateB2;
            }
            boardStateF2 = new int[9][];
            for (int i = 0; i < 9; i++)
            {
                boardStateF2[i] = new int[9];
                boardStateF[i].CopyTo(boardStateF2[i], 0);
            }
            megaBoardStateF2 = new int[9];
            megaBoardStateF.CopyTo(megaBoardStateF2, 0);

            if (to == 0)
            {
                legalSquare = legalSquareF;
                currentBot = currentBotF;
                boardState = boardStateF2;
                megaBoardState = megaBoardStateF2;
                winnerState = winnerStateF;
            }
            if (to == 1)
            {
                legalSquareB1 = legalSquareF;
                currentBotB1 = currentBotF;
                boardStateB1 = boardStateF2;
                megaBoardStateB1 = megaBoardStateF2;
                winnerStateB1 = winnerStateF;
            }
            if (to == 2)
            {
                legalSquareB2 = legalSquareF;
                currentBotB2 = currentBotF;
                boardStateB2 = boardStateF2;
                megaBoardStateB2 = megaBoardStateF2;
                winnerStateB2 = winnerStateF;
            }
        }
        private int checkFor3InRow(int[] board)
        {
            string[] patterns = new string[8];
            patterns[0] = "100100100";
            patterns[1] = "010010010";
            patterns[2] = "001001001";
            patterns[3] = "111000000";
            patterns[4] = "000111000";
            patterns[5] = "000000111";
            patterns[6] = "100010001";
            patterns[7] = "001010100";
            int[] bot1qualifyList = new int[8];
            int[] bot2qualifyList = new int[8];
            bool full = true;
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (patterns[j][i] == '1')
                    {
                        if (board[i] != 1)
                        {
                            bot1qualifyList[j] = 1;
                        }
                        if (board[i] != 2)
                        {
                            bot2qualifyList[j] = 1;
                        }
                    }
                }
                if (board[i] == 0)
                {
                    full = false;
                }
            }
            bool bot1qualify = bot1qualifyList.Sum() < bot1qualifyList.Length;
            bool bot2qualify = bot2qualifyList.Sum() < bot2qualifyList.Length;
            if (bot1qualify) return 1;
            if (bot2qualify) return 2;
            if (full) return 3;
            return 0;
        }
        private int checkFor2InRow(int[] board, int botNum, int cellNum)
        {
            string[] patterns = new string[8];
            patterns[0] = "100100100";
            patterns[1] = "010010010";
            patterns[2] = "001001001";
            patterns[3] = "111000000";
            patterns[4] = "000111000";
            patterns[5] = "000000111";
            patterns[6] = "100010001";
            patterns[7] = "001010100";
            int[] botQualifyList = new int[8];
            int[] twoQualifyList = new int[8];
            int[] blockQualifyList = new int[8];
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (patterns[j][i] == '1')
                    {
                        if (board[i] == botNum)
                        {
                            botQualifyList[j]++;
                            if (cellNum == i && twoQualifyList[j] != -1)
                            {
                                twoQualifyList[j] = 1;
                            }
                        }
                        if (board[i] == otherBot(botNum))
                        {
                            twoQualifyList[j] = -1;
                            if (cellNum == i)
                            {
                                blockQualifyList[j] = 1;
                            }
                        }
                    }
                }
            }
            for (int i = 0; i < 8; i++)
            {
                if (botQualifyList[i] == 2 && twoQualifyList[i] == 1) return 2;
                if (botQualifyList[i] == 2 && blockQualifyList[i] == 1) return 1;
            }
            return 0;
        }
        private void checkBlockAnd2RowRules(int square, int cell, int botNum)
        {
            int twoInARowResult = checkFor2InRow(boardState[square], otherBot(botNum), cell);
            if (twoInARowResult == 1)
            {
                updateTestBoard(3);//It blocks a cell
            }
            twoInARowResult = checkFor2InRow(boardState[square], botNum, cell);
            if (twoInARowResult == 2)
            {
                updateTestBoard(4);//It makes a 2 in a row cells
            }
            twoInARowResult = checkFor2InRow(megaBoardState, otherBot(botNum), square);
            if (twoInARowResult == 1)
            {
                updateTestBoard(5);//It blocks a square
            }
            twoInARowResult = checkFor2InRow(megaBoardState, botNum, square);
            if (twoInARowResult == 2)
            {
                updateTestBoard(6);//It makes a 2 in a row squares
            }
            if (cell == 4)
            {
                updateTestBoard(8);
            }
            else if (cell % 2 == 0)
            {
                updateTestBoard(9);
            }
            else
            {
                updateTestBoard(10);
            }
            updateTestBoard(11 + cell);
            if (square == 4)
            {
                updateTestBoard(20);
            }
            else if (square % 2 == 0)
            {
                updateTestBoard(21);
            }
            else
            {
                updateTestBoard(22);
            }
            updateTestBoard(23 + square);
        }
        private int otherBot(int botNum)
        {
            int res = botNum + 1;
            if (res == 3) res = 1;
            return res;
        }
        private void updateTestBoard(int test)
        {
            infoCatcher[test] = 1;
            //MessageBox.Show("Test" + test);
        }
        
    }
}
