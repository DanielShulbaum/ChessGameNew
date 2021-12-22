using System;

namespace ChessGameNew
{
    public class Program
    {
        public static void Main(string[] args)
        {
            new ChessGame().Game();
        }
    }
    public class Game 
    {
    
    }
    public class ChessGame
    {

        public void Game()
        {
            Board boardForGame = new Board();
            //boardForGame.CreateFirstBoard();
            boardForGame.printBoard();
            bool isWhitePlayerTurn = true;
            Console.OutputEncoding = System.Text.Encoding.Unicode;
            bool isCheckmate = false;
            bool isADraw = false;
            int counterOfMoves = 0;
            while (!isCheckmate && !isADraw)       
            {
                bool isInputCorrect = false;
                bool isCorrectChoice = false;
                bool isMoveLegal = false;
                counterOfMoves++;
                while(!isInputCorrect || !isCorrectChoice || !isMoveLegal)                     
                {
                    boardForGame.resetCanBeCapturedAtEnpassantAndCastling(boardForGame, isWhitePlayerTurn);          //required to reset can be captured by Enpassant for pawns of a given colour
                    Console.WriteLine( (isWhitePlayerTurn ? "Player 1 (White)," : "Player 2 (Black),") + "Please type your move(starting position final position without space between, for example: E2E4)");
                    string input =  Console.ReadLine().Trim();
                    isInputCorrect = this.isInputCorrect(input);
                    if(isInputCorrect)
                    { 
                        int startingLocation = processInputSymbols("" + input[0] + input[1]);
                        int finishLocation = processInputSymbols("" + input[2] + input[3]);                   
                        isCorrectChoice = boardForGame.isCorrectChoice(isWhitePlayerTurn, startingLocation);
                        if(isCorrectChoice)
                        { 
                            isMoveLegal = boardForGame.board[startingLocation].isMoveLegal(startingLocation, finishLocation, boardForGame, isWhitePlayerTurn);                           
                            if (isMoveLegal)
                            { 
                               isMoveLegal = !boardForGame.isKingExposedToCheckAfterMove(startingLocation, finishLocation, boardForGame, isWhitePlayerTurn); 
                                if (isMoveLegal)        
                                {
                                    boardForGame = boardForGame.changeIndicatorBeforeFirstMove(startingLocation,boardForGame);
                                    boardForGame = boardForGame.completeUniqueMoves(startingLocation, finishLocation, boardForGame, isWhitePlayerTurn);
                                    boardForGame = boardForGame.moveFigureOnBoard(startingLocation, finishLocation, boardForGame);
                                    boardForGame.printBoard();    
                                }
                            }
                        }
                    }
                }
                isCheckmate = boardForGame.isCheckmate(boardForGame, isWhitePlayerTurn);
                if (boardForGame.isACheck(boardForGame.lookForTheLocationOfKing(boardForGame, !isWhitePlayerTurn), boardForGame, !isWhitePlayerTurn))
                {
                    Console.WriteLine((isWhitePlayerTurn ? "Player 2, (The Black)" : "Player 1, (The White)") + "You have a check");
                }
                isADraw = boardForGame.isADraw(boardForGame, isWhitePlayerTurn, boardForGame.getVersionsOfBoard());
                isWhitePlayerTurn = !isWhitePlayerTurn;
            }
        }
        bool isInputCorrect(string input)
        {
            bool correctChars;
            if (isInputContainFourSymbols(input) && isInputContainOnlyChars(input))
                correctChars = true;
            else correctChars = false;
            bool contentInputCorrect = false;
            if (correctChars)
            { 
                int startingLocation = processInputSymbols("" + input[0] + input[1]);
                int finishLocation = processInputSymbols("" + input[2] + input[3]);
                contentInputCorrect = true;
                if (startingLocation >= 99 || finishLocation >= 99)
                {
                    Console.WriteLine("The content of your input is wrong, Please type again");
                    contentInputCorrect = false;
                }
            }
            return contentInputCorrect;
        }
        bool isInputContainFourSymbols(string input)
        {
            if(input.Length != 4)
            {
                Console.WriteLine("Your input doesn`t contain the correct number of symbols, please type again");
                return false;
            }
            return true;
        }
        bool isInputContainOnlyChars(string input)
        {
            if(input.Contains(' '))             //at original version " ", verify that this is works correctly
            {
                Console.WriteLine("Your input contains empty chars, please type again");
                return false;
            }
            return true;
        }
        int processInputSymbols(string input)
        {
            int symbol1 = 0, symbol2 = 0;
            switch (input[0])
            {
                case 'A': case 'a': symbol1 = 0; break; case 'B': case 'b': symbol1 = 1; break; case 'C': case 'c': symbol1 = 2; break; case 'D': case 'd': symbol1 = 3; break; case 'E': case 'e': symbol1 = 4; break; case 'F': case 'f': symbol1 = 5; break;
                case 'G': case 'g': symbol1 = 6; break; case 'H': case 'h': symbol1 = 7; break; default: symbol1 = 99; break;
            }
            switch (input[1])
            {
                case '1': symbol2 = 7; break; case '2': symbol2 = 6; break; case '3': symbol2 = 5; break; case '4': symbol2 = 4; break; case '5': symbol2 = 3; break; case '6': symbol2 = 2; break; case '7': symbol2 = 1; break; case '8': symbol2 = 0; break; default: symbol2 = 99; break;
            }
            return (symbol1 + (8 * symbol2));
        }        
    }
    public class Board
    {   
        public Figure[] board = new Figure[64];
        bool isProcessOfCheck;
        int countedNumberOfFiguresOnBoard = 32;
        int counterOfTurnsWithoutChange = 0;
        int counterOfTurnsWithoutCaptureOrPawnMovement = 0;
        string[] versionsOfBoard = new string[50];

        public Board()
        {
            board[0] = new Rook(false); board[7] = new Rook(false);
            board[1] = new Knight(false); board[6] = new Knight(false);
            board[2] = new Bishop(false); board[5] = new Bishop(false);
            board[3] = new Queen(false);
            board[4] = new King(false);
            for (int index = 8; index < 16; index++)
                board[index] = new Pawn(false);
            for (int index = 48; index < 56; index++)
                board[index] = new Pawn(true);
            board[56] = new Rook(true); board[63] = new Rook(true);
            board[57] = new Knight(true); board[62] = new Knight(true);
            board[58] = new Bishop(true); board[61] = new Bishop(true);
            board[59] = new Queen(true);
            board[60] = new King(true);
        }

        public string[] getVersionsOfBoard()
        {
            return this.versionsOfBoard;
        }
        public void setVersionsOfBoard(string[] updatedVersionsOfBoard)
        {
            this.versionsOfBoard = updatedVersionsOfBoard;
        }
        public void setCounterOfTurnsWithoutCaptureOrPawnMovement(int countOfMovesWithoutCaptureOrPawnMove)
        {
            this.counterOfTurnsWithoutCaptureOrPawnMovement = countOfMovesWithoutCaptureOrPawnMove;
        }
        public int getCounterOfTurnsWithoutCaptureOrPawnMovement()
        {
            return this.counterOfTurnsWithoutCaptureOrPawnMovement;
        }
        public void setCounterOfTurnsWithoutChange(int countOfMovesWithoutChangeUpdated)
        {
            this.counterOfTurnsWithoutChange = countOfMovesWithoutChangeUpdated;
        }
        public int getCounterOfTurnsWithoutChange()
        {
            return this.counterOfTurnsWithoutChange;
        }
        public void setNumberOfFiguresOnBoard(int countNumberOfFiguresUpdated)
        {
            this.countedNumberOfFiguresOnBoard = countNumberOfFiguresUpdated;
        }
        public int getCountedNumberOfFiguresOnBoard()
        {
            return this.countedNumberOfFiguresOnBoard;
        }
        public bool isProcessOfCheckTest()
        {
            return this.isProcessOfCheck;
        }
        public void setIsProcessOfCheckTest(bool processCheckTest)
        {
            this.isProcessOfCheck = processCheckTest;
        }
        public Board changeIndicatorBeforeFirstMove(int startingLocation, Board boardForGame)
        {
            if (boardForGame.board[startingLocation] is Pawn)
            { 
                ((Pawn)boardForGame.board[startingLocation]).setIsBeforeFirstMove(false);
                boardForGame.setCounterOfTurnsWithoutCaptureOrPawnMovement(0);
                boardForGame.setCounterOfTurnsWithoutChange(0);
            }
            if (boardForGame.board[startingLocation] is Rook)
            { 
                if(((Rook)boardForGame.board[startingLocation]).getIsBeforeFirstMove() == true)
                {
                    ((Rook)boardForGame.board[startingLocation]).setIsBeforeFirstMove(false);
                    boardForGame.setCounterOfTurnsWithoutChange(0);
                }
            }
            if (boardForGame.board[startingLocation] is King)
            {
                if (((King)boardForGame.board[startingLocation]).getIsBeforeFirstMove() == true)
                {
                    ((King)boardForGame.board[startingLocation]).setIsBeforeFirstMove(false);
                    boardForGame.setCounterOfTurnsWithoutChange(0);
                }
            }
            return boardForGame;
        }
        public bool isCheckmate(Board boardForGame, bool whitePlayerTurn)
        {            
            bool isACheckmate = true;
            bool isCheckOnOppositeSide = boardForGame.isACheck(boardForGame.lookForTheLocationOfKing(boardForGame, !whitePlayerTurn), boardForGame, !whitePlayerTurn);
            isProcessOfCheck = true;
            bool isOppositeSideHasAMove;
            for (int index = 0; index < 64; index++)
            {
                for (int indexBoard = 0; indexBoard < 64; indexBoard++)
                {
                    if (boardForGame.board[index] is Figure && ((Figure)boardForGame.board[index]).getIsWhiteColourFigure() != whitePlayerTurn)
                    {
                        isOppositeSideHasAMove = ((Figure)board[index]).isMoveLegal(index, indexBoard, boardForGame, !whitePlayerTurn);
                        if (isOppositeSideHasAMove)
                        {
                            if(!(boardForGame.isKingExposedToCheckAfterMove(index, indexBoard, boardForGame, !whitePlayerTurn)))
                                                                                                                            //SetProcessCheckmate(false);
                            return false;
                        }
                    }
                }
            }
            if (isACheckmate && isCheckOnOppositeSide)
                Console.WriteLine((whitePlayerTurn ? ("Player 1(White),") : ("Player 2 (Black),")) + "You made a checkmate, you won this game!!!");
            if (isACheckmate && !isCheckOnOppositeSide)
                Console.WriteLine((whitePlayerTurn ? ("Player 2 (Black),") : ("Player 1(White),")) + "You have a stalemate, it is a draw!!!");
            isProcessOfCheck = false;
            return isACheckmate;
        }
        public bool isADraw(Board boardForGame, bool whitePlayerTurn, string[] versionsOfBoard)
        {
            bool isDraw = false;
            boardForGame.saveVersionsOfBoard(boardForGame, versionsOfBoard);
            boardForGame.numberOfFiguresOnBoard(boardForGame);
            bool isFiftyTurnsWithoutCaptureOrPawnMove = boardForGame.isFiftyMoveDraw(boardForGame);
            bool isThreeFoldRepetition = boardForGame.isThreeFoldRepetition(boardForGame, versionsOfBoard, whitePlayerTurn);
            bool isInsuficientMeansForMate = boardForGame.isDeadPosition(boardForGame);
            if (isFiftyTurnsWithoutCaptureOrPawnMove)
                Console.WriteLine("It is a draw!!! There were no capture of figures or movement of pawns during last 50 moves");
            if (isThreeFoldRepetition)
                Console.WriteLine("It is a draw!!! 3 similar positions of board were identified");
            if (isInsuficientMeansForMate)
                Console.WriteLine("It is a draw!!! Both players don`t have sufficient number of figures to perfom a checkmate on the opposite side");
            if (isFiftyTurnsWithoutCaptureOrPawnMove || isThreeFoldRepetition || isInsuficientMeansForMate)
                isDraw = true;
            return isDraw;
        }
        public string[] saveVersionsOfBoard(Board boardForGame, string[] versionsOfBoard)
        {
            string boardToSave = "";
            for(int index = 0; index < 64; index++)
            {
                if (boardForGame.board[index] is Figure)
                {
                    boardToSave += (boardForGame.board[index].getIsWhiteColourFigure() ? "W" : "B") + boardForGame.board[index].getName() + "/";
                }
                else boardToSave += "null/";
            }
            versionsOfBoard[boardForGame.getCounterOfTurnsWithoutChange()] = boardToSave;
            boardForGame.setCounterOfTurnsWithoutChange(boardForGame.getCounterOfTurnsWithoutChange() + 1);
            return versionsOfBoard;
        }
        public bool isThreeFoldRepetition(Board boardForGame, string[] versionsOfBoard, bool isWhitePlayerTurn)
        {
            int counterOfSimilarPosition = 0;
            int start = isWhitePlayerTurn ? 0 : 1;
            if (boardForGame.getCounterOfTurnsWithoutChange() < 6)
                return false;
            for (int index1 = start; index1 < boardForGame.getCounterOfTurnsWithoutChange(); index1 += 2)
            {
                for(int index2 = index1 + 2; index2 < boardForGame.getCounterOfTurnsWithoutChange(); index2 +=2)
                {
                    if (versionsOfBoard[index1] == versionsOfBoard[index2])
                        counterOfSimilarPosition++;
                }
            }
            if (counterOfSimilarPosition > 2)
                return true;
            else return false;
        }
        public bool isDeadPosition(Board boardForGame)
        {
            int numberOfFiguresOnBoard = boardForGame.getCountedNumberOfFiguresOnBoard();
            int numberOfCrucialFigures = 0;
            int locationOfBlackBishop = 99;
            int locationOfWhiteBishop = 99;
            if (numberOfFiguresOnBoard > 4)
                return false;
            for (int index = 0; index < 64; index++)
            {
                if(boardForGame.board[index] is Figure)
                {
                    if (boardForGame.board[index] is King || boardForGame.board[index] is Knight || boardForGame.board[index] is Bishop)
                        numberOfCrucialFigures++;
                    if (boardForGame.board[index] is Bishop && boardForGame.board[index].getIsWhiteColourFigure())
                        locationOfWhiteBishop = index;
                    if (boardForGame.board[index] is King && !boardForGame.board[index].getIsWhiteColourFigure())
                        locationOfBlackBishop = index;
                }
            }
            if (numberOfFiguresOnBoard == 3 && numberOfCrucialFigures == 3)
                return true;
            if (numberOfFiguresOnBoard < 3)
                return true;
            bool isSquareOfWhiteBishopIsWhite;
            bool isSquareOfBlackBishopIsWhite;
            if (((locationOfWhiteBishop / 8) % 2 == 0 && (locationOfWhiteBishop % 8) % 2 == 0) || ((locationOfWhiteBishop / 8) % 2 != 0 && (locationOfWhiteBishop % 8) % 2 != 0))
                isSquareOfWhiteBishopIsWhite = true;
            else isSquareOfWhiteBishopIsWhite = false;
            if (((locationOfBlackBishop / 8) % 2 == 0 && (locationOfBlackBishop % 8) % 2 == 0) || ((locationOfBlackBishop / 8) % 2 != 0 && (locationOfBlackBishop % 8) % 2 != 0))
                isSquareOfBlackBishopIsWhite = true;
            else isSquareOfBlackBishopIsWhite = false;

            if (numberOfFiguresOnBoard == 4 && numberOfCrucialFigures == 4 && locationOfBlackBishop != 99 && locationOfWhiteBishop != 99)
            {
                if (isSquareOfWhiteBishopIsWhite != isSquareOfBlackBishopIsWhite)
                    return true;
            }
            return false;
        }
        public bool isFiftyMoveDraw(Board boardForGame)
        {
            if (boardForGame.getCounterOfTurnsWithoutCaptureOrPawnMovement() > 49)
                return true;
            else return false;
        }
        public void numberOfFiguresOnBoard(Board boardForGame)
        {
            int currentNumberOfFiguresOnBoard = 0;
            for (int index = 0; index <64; index++)
            {
                if (boardForGame.board[index] is Figure)
                    currentNumberOfFiguresOnBoard++;
            }
            if (currentNumberOfFiguresOnBoard < boardForGame.getCountedNumberOfFiguresOnBoard())
            {
                boardForGame.setCounterOfTurnsWithoutCaptureOrPawnMovement(0);
                boardForGame.setCounterOfTurnsWithoutChange(0);
                boardForGame.setNumberOfFiguresOnBoard(currentNumberOfFiguresOnBoard);
            }
            else setCounterOfTurnsWithoutCaptureOrPawnMovement(boardForGame.getCounterOfTurnsWithoutCaptureOrPawnMovement() + 1);
        }
        public bool isKingExposedToCheckAfterMove(int startingLocation, int finishLocation, Board board, bool whitePlayerTurn)
        {
            bool exposedToCheck = false;
            Board boardToSimulateMove = createBoardToSimulateMove(board);
            boardToSimulateMove = moveFigureOnBoard(startingLocation, finishLocation, boardToSimulateMove);
            exposedToCheck = isACheck(lookForTheLocationOfKing(boardToSimulateMove, whitePlayerTurn), boardToSimulateMove, whitePlayerTurn);
            if (exposedToCheck)
                if(!isProcessOfCheckTest())
                    Console.WriteLine("You can not perform that move, cause your king will be exposed to check");
            return exposedToCheck;
        }
        public Board moveFigureOnBoard(int startingLocation, int finishLocation, Board board) // performs a change, a movement of figure on the board
        {
            board.board[finishLocation] = board.board[startingLocation];
            board.board[startingLocation] = null;

          /*  Squares nObj = new Squares(startPos, " ee", board[startPos].whiteColourSquare);
            board[startPos].whiteColourSquare = board[finishPos].whiteColourSquare;         // important to have as long as colour of squares used to test legit moves, testing by colour can be replaced with more extense control use of change in rows and columns
            //board[startPos].index = board[finishPos].index;                               // this line can be used if field of index will become important, for example to find king faster, along with new objects of king 
            board[finishPos] = board[startPos];
            board[startPos] = nObj;*/
            return board;
        }
        public Board completeUniqueMoves (int startingLocation, int finishLocation, Board boardForGame, bool whitePlayerTurn)
        {
            if(boardForGame.board[startingLocation] is Pawn)
                { 
                    if( Math.Abs(startingLocation - finishLocation) == 7 || Math.Abs(startingLocation - finishLocation) == 9   )
                    {
                        if(whitePlayerTurn)
                        {
                            if (boardForGame.board[finishLocation + 8] is Pawn && (boardForGame.board[finishLocation + 8].getIsWhiteColourFigure() != whitePlayerTurn) && (((Pawn)boardForGame.board[finishLocation + 8]).getCanBeCapturedAtEnpassant() == true))
                                boardForGame = boardForGame.deleteFigure(finishLocation + 8, boardForGame);
                        }
                        else if (boardForGame.board[finishLocation - 8] is Pawn && (boardForGame.board[finishLocation - 8].getIsWhiteColourFigure() != whitePlayerTurn) && (((Pawn)boardForGame.board[finishLocation - 8]).getCanBeCapturedAtEnpassant() == true))
                            boardForGame = boardForGame.deleteFigure(finishLocation - 8, boardForGame);
                    }
                if  ((whitePlayerTurn && finishLocation / 8 == 0) || (!whitePlayerTurn && finishLocation / 8 == 7))
                    boardForGame = transform(startingLocation, boardForGame, whitePlayerTurn);
                }
            if(boardForGame.board[startingLocation] is King)
            {
                if (((King)boardForGame.board[startingLocation]).getIsCastlingApprovedMinus())
                { 
                    ((Rook)boardForGame.board[startingLocation - 4]).setIsBeforeFirstMove(false);
                    ((Rook)boardForGame.board[startingLocation + 3]).setIsBeforeFirstMove(false);
                    boardForGame = moveFigureOnBoard(startingLocation - 4, finishLocation + 1, boardForGame);
                    boardForGame.setCounterOfTurnsWithoutChange(0);
                }
                if (((King)boardForGame.board[startingLocation]).getIsCastlingApprovedPlus())
                {                     
                    ((Rook)boardForGame.board[startingLocation + 3]).setIsBeforeFirstMove(false);
                    ((Rook)boardForGame.board[startingLocation - 4]).setIsBeforeFirstMove(false);
                    boardForGame = moveFigureOnBoard(startingLocation + 3, finishLocation - 1, boardForGame);
                    boardForGame.setCounterOfTurnsWithoutChange(0);
                }
            }
            return boardForGame;
        }
        public Board transform(int startingLocation, Board boardForGame, bool whitePlayerTurn)
        {
            bool inputCorrect = false;
            string input = "";
            while (!inputCorrect)
            {
                Console.WriteLine("You may now exchange your pawn for other figure. Please type your choice:  R for Rook, N for Knight, B for Bishop, Q for Queen, and press ENTER");
                input = (Console.ReadLine()).Trim();
                //   inputCorrect = false;
                if (input.Length != 1)
                    inputCorrect = false;
                if (input == "r" || input == "R" || input == "N" || input == "n" || input == "B" || input == "b" || input == "Q" || input == "q")
                    inputCorrect = true;
                if (!inputCorrect)
                    Console.WriteLine("Your choice is not correct, please type again");
            }
            input += "";                                                                                    //maybe unneccesary 
                if (input == "r" || input == "R")
                { 
                    boardForGame.board[startingLocation] = new Rook( whitePlayerTurn ? true :false);
                    ((Rook)boardForGame.board[startingLocation]).setIsBeforeFirstMove(false);
                }
                if (input == "n" || input == "N")
                    boardForGame.board[startingLocation] = new Knight(whitePlayerTurn ? true : false);
                if (input == "b" || input == "B")
                    boardForGame.board[startingLocation] = new Bishop(whitePlayerTurn ? true : false);
                if (input == "q" || input == "Q")
                    boardForGame.board[startingLocation] = new Queen(whitePlayerTurn ? true : false);
            return boardForGame;
        }
        public Board deleteFigure(int location, Board boardForGame)
        {
            boardForGame.board[location] = null;
            return boardForGame;
        }
        public Board createBoardToSimulateMove(Board boardForGame)                  //used in Move func to test check after player`s move approval, other way to do it with board and reverting previous board situation from 2D array in case if there is a check after approved move
        {
            Board boardToSimulateMove = new Board();
            //Figure [] boardToSimulateMove = new Figure[64];
            for (int index = 0; index < 64; index++)
            {
                if (boardForGame.board[index] is Figure)
                {
                    if (boardForGame.board[index] is Pawn)
                    {
                        boardToSimulateMove.board[index] = new Pawn(boardForGame.board[index].getIsWhiteColourFigure());
                        ((Pawn)boardToSimulateMove.board[index]).setIsBeforeFirstMove(((Pawn)boardForGame.board[index]).getIsBeforeFirstMove());
                    }
                    if (boardForGame.board[index] is Knight)
                        boardToSimulateMove.board[index] = new Knight(boardForGame.board[index].getIsWhiteColourFigure());
                    if (boardForGame.board[index] is Bishop)
                        boardToSimulateMove.board[index] = new Bishop(boardForGame.board[index].getIsWhiteColourFigure());
                    if (boardForGame.board[index] is Rook)
                    {
                        boardToSimulateMove.board[index] = new Rook(boardForGame.board[index].getIsWhiteColourFigure());
                        ((Rook)boardToSimulateMove.board[index]).setIsBeforeFirstMove(((Rook)boardForGame.board[index]).getIsBeforeFirstMove());
                    }
                    if (boardForGame.board[index] is Queen)
                        boardToSimulateMove.board[index] = new Queen(boardForGame.board[index].getIsWhiteColourFigure());
                    if (boardForGame.board[index] is King)
                    {
                        boardToSimulateMove.board[index] = new King(boardForGame.board[index].getIsWhiteColourFigure());
                        ((King)boardToSimulateMove.board[index]).setIsBeforeFirstMove(((King)boardForGame.board[index]).getIsBeforeFirstMove());
                    }
                }
                else boardToSimulateMove.board[index] = null;
            }
            return boardToSimulateMove;
        }
        public void resetCanBeCapturedAtEnpassantAndCastling(Board boardForGame, bool whitePlayerTurn)
        {
            for (int index = 0; index < 64; index++)
            { 
                if (boardForGame.board[index] is Pawn && boardForGame.board[index].getIsWhiteColourFigure() == whitePlayerTurn)
                    ((Pawn)boardForGame.board[index]).setCanBeCapturedAtEnpassant(false);
                if (boardForGame.board[index] is King && boardForGame.board[index].getIsWhiteColourFigure() == whitePlayerTurn)
                {
                    ((King)boardForGame.board[index]).setIsCastlingApprovedMinus(false);
                    ((King)boardForGame.board[index]).setIsCastlingApprovedPlus(false);
                }
            }
        }
        
        public void printBoard()
        {
            string[] letters = new string[9] { "   ", "  A ", "  B ", "  C ", "  D ", "  E ", "  F ", "  G ", "  H " };
            string[] numbers = new string[8] { " 8 ", " 7 ", " 6 ", " 5 ", " 4 ", " 3 ", " 2 ", " 1 " };
            string UNDERLINE = "\x1B[4m"; // if will be possible due to deadline check if possible to configure width of underline for grid marking of the board
            string RESET = "\x1B[0m";       // this line and above enable perform underline, consider find vertical underlining if exists
            for (int index = 0; index < 9; index++) //prints the line of letters with underline
                Console.Write(UNDERLINE + letters[index] + RESET);
            Console.WriteLine();
            for (int index = 0; index < 8; index++)
            {
                Console.Write(numbers[index] + "|"); //prints numbers aside of game table
                for (int index2 = 0; index2 < 8; index2++) //prints the game board lines underlined
                {
                    if (board[(8 * index + index2)] is Figure)
                    {
                        Console.Write(UNDERLINE + " " + (board[8 * index + index2].getIsWhiteColourFigure() == true ? "W" : "B") + board[(8 * index + index2)].getName() + "|" + RESET);
                    }
                    else Console.Write(UNDERLINE + "   " + "|"+ RESET);
                }
                Console.Write("|"); // prints game board border
                Console.WriteLine();
            }
        }
        public bool  isCorrectChoice(bool WhitePlayerTurn, int startingLocation)
        {
            bool correctChoice = false;
            bool whiteFigureColour = WhitePlayerTurn ? true : false;
            if (board[startingLocation] is Figure && ((Figure)board[startingLocation]).getIsWhiteColourFigure() == whiteFigureColour)              // can be acomplished only after construction of classes
                correctChoice = true;
            if (!correctChoice)
                Console.WriteLine("You didn`t choose your figure, please choose again");
            return correctChoice;
        }
        public int lookForTheLocationOfKing(Board boardForGame, bool whitePlayerTurn)           //based on purpose if required to find player`s turn king or opposite - possible to change bool of white player
        {                                                                                       // current set is a test of check on white king at white player turn
            int location = 99;          //unrealistic value in purpose
            for(int index = 0; index < 64; index++)
            {
                if(boardForGame.board[index] is King && boardForGame.board[index].getIsWhiteColourFigure() == whitePlayerTurn)
                {
                    location = index;
                    break;
                }
            }
            return location;
        }
        public bool isACheck(int location, Board boardForGame, bool whitePlayerTurn)         
        {
            boardForGame.setIsProcessOfCheckTest(true);   
            bool thereIsACheck = false;
            for (int index = 0; index < 64; index++)
            {
                if(  (boardForGame.board[index] is Figure) && (boardForGame.board[index].getIsWhiteColourFigure() != whitePlayerTurn)     )
                {
                    thereIsACheck = boardForGame.board[index].isMoveLegal(index, location, boardForGame, !whitePlayerTurn);        // important to notice current setting that performing test for opposite side figures
                    if (thereIsACheck)
                        break;
                }
            }
            boardForGame.setIsProcessOfCheckTest(false);
            return thereIsACheck;
        }
    }
    public class Figure
    {
        string name;
        bool isWhiteColourFigure;

        public Figure(bool isWhiteColourFigure) 
        { 
            this.isWhiteColourFigure = isWhiteColourFigure;
        }     
        
        public virtual string getName()
        {
            return this.name;
        }
        public virtual bool getIsWhiteColourFigure()
        {
            return this.isWhiteColourFigure;
        }
        public virtual bool isMoveLegal(int startingLocation, int finishLocation, Board board, bool isWhitePlayerTurn) { return false; }
    }
    public class Pawn : Figure  
    {
        bool isBeforeFirstMove = true;
        bool canBeCapturedAtEnpassant = false;
        string name = "P";

        public Pawn(bool isWhiteColourFigure) : base(isWhiteColourFigure) { }

        public void setIsBeforeFirstMove(bool isFirstMove)
        {
            this.isBeforeFirstMove = isFirstMove;
        }
        public bool getIsBeforeFirstMove()
        {
            return this.isBeforeFirstMove;
        }
        public void setCanBeCapturedAtEnpassant( bool canBeCaptured)
        {
          this.canBeCapturedAtEnpassant = canBeCaptured;
        }
        public bool getCanBeCapturedAtEnpassant()
        {
            return this.canBeCapturedAtEnpassant;
        }        
        public override string getName()
        {
            return this.name;
        }
        public bool isDirectionLegal(int startingLocation, int finishLocation, Board boardForGame, bool isWhitePlayerTurn)
        {
            bool isLegalDirectionOfMove = true;
            if ( boardForGame.board[startingLocation].getIsWhiteColourFigure())
            {
                if((startingLocation - finishLocation) < 0)
                {
                    if(!boardForGame.isProcessOfCheckTest())
                        Console.WriteLine("Your move is not toward allowed direction, please try again");          
                    isLegalDirectionOfMove = false;
                }
            }
            else if ((startingLocation - finishLocation) > 0)
            {
                if (!boardForGame.isProcessOfCheckTest())
                    Console.WriteLine("Your move is against allowed direction, please try again");              
                isLegalDirectionOfMove = false;
            }
            return isLegalDirectionOfMove;
        }
        public bool isCaptureLegal(int startingLocation, int finishLocation, Board boardForGame, bool whitePlayerTurn)
        {
            bool legalCapture = false;
            if (Math.Abs(startingLocation - finishLocation) == 7 || Math.Abs(startingLocation - finishLocation) == 9) // performs a test of legal eating movement 
            {
                if (Utilities.calculateRowsDifference(startingLocation, finishLocation) != 1)  // performs a test if a capture movement is for the next raw "
                {
                    if (!boardForGame.isProcessOfCheckTest())
                        Console.WriteLine("Your move is illegal, please try again");                
                    return false;
                }
                if ((boardForGame.board[finishLocation] is Figure) && (boardForGame.board[finishLocation].getIsWhiteColourFigure() != whitePlayerTurn)) //   performs a test if a figure to be eaten if of a different colour                
                {
                    legalCapture = true;
                }
                else
                {
                    if (whitePlayerTurn) // check possibility for enpassant movement and returns true if it is. want to modify to get rid off 2d board                                          
                    {
                        if(boardForGame.board[finishLocation+8] is Pawn && !(boardForGame.board[finishLocation + 8].getIsWhiteColourFigure()) && ((Pawn)boardForGame.board[finishLocation + 8]).getCanBeCapturedAtEnpassant())
                        legalCapture = true;
                    }
                    else if (!whitePlayerTurn)
                        if (boardForGame.board[finishLocation - 8] is Pawn && (boardForGame.board[finishLocation - 8].getIsWhiteColourFigure()) && ((Pawn)boardForGame.board[finishLocation - 8]).getCanBeCapturedAtEnpassant())
                            legalCapture = true;
                    else
                    {
                            if (!boardForGame.isProcessOfCheckTest())
                                Console.WriteLine("Your move is illegal, please try again");                
                        legalCapture = false;
                    }
                }
            }
            return legalCapture;
        }
        public void setEnpassant(int startingLocation, int finishLocation, Board boardForGame, bool whitePlayerTurn)
        {
            if ((boardForGame.board[finishLocation - 1] is Figure && boardForGame.board[finishLocation - 1].getIsWhiteColourFigure() != whitePlayerTurn) || (boardForGame.board[finishLocation + 1] is Figure && boardForGame.board[finishLocation + 1].getIsWhiteColourFigure() != whitePlayerTurn))
                canBeCapturedAtEnpassant = true;
        }
        public bool isMoveStraightLegal(int startingLocation, int finishLocation, Board boardForGame, bool whitePlayerTurn)
        {
            bool isLegalMoveStraight = false;
            switch (Math.Abs(startingLocation - finishLocation))     //performs a test for pawns movement
            {
                case 8: isLegalMoveStraight = true; break;
                case 16: isLegalMoveStraight = isBeforeFirstMove ? true : false; break;
                default: isLegalMoveStraight = false; break;
            }
            if(isLegalMoveStraight)
            {
                if (Math.Abs(startingLocation - finishLocation) == 8)
                { 
                    if(boardForGame.board[finishLocation] != null)
                    isLegalMoveStraight = false;
                }
                else if ((boardForGame.board[finishLocation] != null) || (boardForGame.board[(finishLocation + startingLocation) / 2] != null) )
                    isLegalMoveStraight = false;
            }
            if ((Math.Abs(startingLocation - finishLocation) == 16) && isLegalMoveStraight)   
                setEnpassant(startingLocation, finishLocation, boardForGame, whitePlayerTurn);
            return isLegalMoveStraight;
        }
        public override bool isMoveLegal(int startingLocation, int finishLocation, Board boardForGame, bool isWhitePlayerTurn)
        {           
            bool isRightMove = false;
            bool isLegalDirectionOfMove = isDirectionLegal(startingLocation, finishLocation, boardForGame, isWhitePlayerTurn);
            if (!isLegalDirectionOfMove)
                return false;
            bool isLegalCapture = isCaptureLegal(startingLocation, finishLocation, boardForGame, isWhitePlayerTurn);
            bool isLegalStraightMove = false;
            if (!isLegalCapture)
                isLegalStraightMove = isMoveStraightLegal(startingLocation, finishLocation, boardForGame, isWhitePlayerTurn);
            if (!isLegalStraightMove && !isLegalCapture)
                isRightMove = false;
            if (isLegalDirectionOfMove && (isLegalCapture || isLegalStraightMove))
            {
                isRightMove = true;
            }
            if (!isRightMove)
                if (!boardForGame.isProcessOfCheckTest())
                    Console.WriteLine("Your move Illegal");
            return isRightMove;
        }    
    }
    public class Knight : Figure  
    {
        string name = "N";

        public Knight(bool whiteColourFigure) : base(whiteColourFigure) {  }

        public override string getName()
        {
            return this.name;
        }
        public override bool isMoveLegal(int startingLocation, int finishLocation, Board boardForGame, bool whitePlayerTurn)
        {
            bool isRightMove = false;
            if (((Math.Abs(startingLocation - finishLocation) == 10 || Math.Abs(startingLocation - finishLocation) == 6) && Utilities.calculateRowsDifference(startingLocation, finishLocation) == 1 && Utilities.CalculateColumnDifference(startingLocation, finishLocation) == 2)
                || ((Math.Abs(startingLocation - finishLocation) == 17 || Math.Abs(startingLocation - finishLocation) == 15) && Utilities.calculateRowsDifference(startingLocation, finishLocation) == 2 && Utilities.CalculateColumnDifference(startingLocation, finishLocation) == 1))
                 // performs a check of legal movement based on index 
            {
                if ((boardForGame.board[finishLocation] is Figure) && (whitePlayerTurn == ((Figure)boardForGame.board[finishLocation]).getIsWhiteColourFigure())) //  false the only illegal move - the case if final position have a figure of same colour             
                {
                    if (!boardForGame.isProcessOfCheckTest())
                        Console.WriteLine("Your move is illegal");
                    return false;
                }
                else
                    isRightMove = true;
                
            }
            if (!isRightMove)
            {
                if (!boardForGame.isProcessOfCheckTest())
                    Console.WriteLine("Your move is illegal");
            }
            return isRightMove;
        }
    }
    public class Bishop : Figure 
    {
        string name = "B";

        public Bishop(bool whiteColourFigure) : base(whiteColourFigure) { }

        public override string getName()
        {
            return this.name;
        }
        public override bool isMoveLegal(int startingLocation, int finishLocation, Board boardForGame, bool whitePlayerTurn)
        {
            bool isRightMove = false;
            if (    (Math.Abs(startingLocation - finishLocation) % 7 == 0 || Math.Abs(startingLocation - finishLocation) % 9 == 0) && (Utilities.CalculateColumnDifference(startingLocation, finishLocation) == Utilities.calculateRowsDifference(startingLocation,finishLocation)) && (Utilities.CalculateColumnDifference(startingLocation, finishLocation) != 0)) // performs a check of legal  movement based on index and on colour of square which should not change after each move of a bishop
            {
                if ((boardForGame.board[finishLocation] is Figure) && whitePlayerTurn == boardForGame.board[finishLocation].getIsWhiteColourFigure()) //  falses the only option to be falsed - figure of same colour on finish square        
                {
                    if (!boardForGame.isProcessOfCheckTest())
                        Console.WriteLine("Your move is illegal");
                    return false;
                }
                else
                    isRightMove = isSquaresOnBishopWayAreFree(startingLocation, finishLocation, boardForGame);
            }
            if (!isRightMove)
                if (!boardForGame.isProcessOfCheckTest())
                    Console.WriteLine("Your move illegal");
            return isRightMove;
        }
        public bool isSquaresOnBishopWayAreFree(int startingLocation, int finishLocation, Board boardForGame)
        {
            bool isFreeLine = true;
            int start = 0, finish = 0, div = 0;
            if (Math.Abs(startingLocation - finishLocation) % 9 == 0)      // starting to check from 9 because there is one number(situation on desk from 0 to 63) that can be divided by both 7 and 9, and this situation should be divided by 9 then
                div = 9;
            else div = 7;
            start = ((startingLocation > finishLocation) ? finishLocation : startingLocation) + div;
            finish = (startingLocation < finishLocation) ? finishLocation : startingLocation;
            for (int index = start; index < finish; index += div)
            {
                if (boardForGame.board[index] != null)
                    isFreeLine = false;
            }
            return isFreeLine;
        }
    }
    public class Rook : Figure  
    {
        bool isBeforeFirstMove = true;
        string name = "R";

        public Rook(bool whiteColourFigure) : base(whiteColourFigure) { }

        public override string getName()
        {
            return this.name;
        }
        public void setIsBeforeFirstMove(bool isFirstMove)
        {
            this.isBeforeFirstMove = isFirstMove;
        }
        public bool getIsBeforeFirstMove()
        {
            return this.isBeforeFirstMove;
        }        
        public override bool isMoveLegal(int startingLocation, int finishLocation, Board boardForGame, bool whitePlayerTurn)
        {
            bool isRightMove = false;
            if (Math.Abs(startingLocation - finishLocation) % 8 == 0 || Utilities.calculateRowsDifference(startingLocation, finishLocation) == 0) // performs a test of legal  movement based on index, Rock can move only on same raw or column
            {
                if ((boardForGame.board[finishLocation] is Figure) && (whitePlayerTurn == ((Figure)boardForGame.board[finishLocation]).getIsWhiteColourFigure())) //  falses the only illegal option of same colour figure at finish pos                 
                {
                    if (!boardForGame.isProcessOfCheckTest())
                        Console.WriteLine("Your move is illegal");
                    return false;
                }
                else
                    isRightMove = isSquaresOnRookWayAreFree(startingLocation, finishLocation, boardForGame);
            }
            return isRightMove;
        }
        public bool isSquaresOnRookWayAreFree(int startingLocation, int finishLocation, Board boardForGame)
        {
            bool isFreeLine = true;
            int start = 0, finish = 0, div = 0;
            if (Math.Abs(startingLocation - finishLocation) % 8 == 0)      // performs a test of movement line on same column
                div = 8;
            else div = 1;                                    //performs a test of movement on the same row
            start = ((startingLocation > finishLocation) ? finishLocation : startingLocation) + div;
            finish = (startingLocation < finishLocation) ? finishLocation : startingLocation;
            for (int index = start; index < finish; index += div)
            {
                if (boardForGame.board[index] != null)
                    isFreeLine = false;
            }
            return isFreeLine;
        }
    }
    public class Queen : Figure 
    {
        string name = "Q";

        public Queen(bool isWhiteColourFigure) : base(isWhiteColourFigure) { }

        public override string getName()
        {
            return this.name;
        }
        public override bool isMoveLegal(int startingLocation, int finishLocation, Board boardForGame, bool isWhitePlayerTurn)
        {
            bool isRightMove = false;          
            Bishop rightMoveAsBishop = new Bishop(isWhitePlayerTurn);
            Rook rightMoveAsRook = new Rook(isWhitePlayerTurn);
            if(rightMoveAsBishop.isMoveLegal(startingLocation, finishLocation, boardForGame, isWhitePlayerTurn) || rightMoveAsRook.isMoveLegal(startingLocation, finishLocation, boardForGame, isWhitePlayerTurn))
                isRightMove = true;
            if (!isRightMove)
                if (!boardForGame.isProcessOfCheckTest())
                    Console.WriteLine("Your Move is Illegal");          
            return isRightMove;
        }
    }
    public class King : Figure
    {
        bool isBeforeFirstMove = true;
        string name = "K";
        bool isCastlingApprovedMinus;
        bool isCastlingApprovedPlus;

        public King(bool whiteColourFigure) : base(whiteColourFigure) { }

        public void setIsBeforeFirstMove(bool isFirstMove)
        {
            this.isBeforeFirstMove = isFirstMove;
        }
        public void setIsCastlingApprovedMinus(bool setting)
        {
            this.isCastlingApprovedMinus = setting;
        }
        public void setIsCastlingApprovedPlus(bool setting)
        {
             this.isCastlingApprovedPlus = setting;
        }
        public bool getIsCastlingApprovedPlus()
        {
            return this.isCastlingApprovedPlus;
        }
        public bool getIsCastlingApprovedMinus()
        {
            return this.isCastlingApprovedMinus;
        }
        public bool getIsBeforeFirstMove()
        {
            return this.isBeforeFirstMove;
        }
        public override string getName()
        {
            return this.name;
        }
        public override bool isMoveLegal(int startingLocation, int finishLocation, Board boardForGame, bool whitePlayerTurn)
        {
            bool rightMove = false;
            if ((Utilities.calculateRowsDifference(startingLocation, finishLocation) == 0) && (Math.Abs(startingLocation - finishLocation) == 2) )
            {
                if ((boardForGame.board[finishLocation] is Figure) && boardForGame.board[finishLocation].getIsWhiteColourFigure() == whitePlayerTurn) //  falses the only illegal move
                {
                    if (!boardForGame.isProcessOfCheckTest())
                        Console.WriteLine("Your move is illegal");
                    return false;
                }
                if (this.isBeforeFirstMove == false)
                {
                    if (!boardForGame.isProcessOfCheckTest())
                        Console.WriteLine("The King already moved, it is impossible to perform castling");
                    return false;
                }
                else rightMove = IsCastlingLegal(startingLocation, finishLocation, boardForGame, whitePlayerTurn);                //required to complete
            }           
            if (Math.Abs(startingLocation - finishLocation) == 1 && Utilities.calculateRowsDifference(startingLocation, finishLocation) == 0) // performs a test of legal  movement based on index 
            {
                if ((boardForGame.board[finishLocation] is Figure) && (whitePlayerTurn == ((Figure)boardForGame.board[finishLocation]).getIsWhiteColourFigure())) //  performs a test if a final pos is figure of same colour, the only option to be falsed                
                {
                    if (!boardForGame.isProcessOfCheckTest())
                        Console.WriteLine("Your move is illegal");
                    return false;
                }
                else
                    rightMove = true;
            }
            if (((Math.Abs(startingLocation - finishLocation) == 9) || (Math.Abs(startingLocation - finishLocation) == 7) || (Math.Abs(startingLocation - finishLocation) == 8)) && (Utilities.calculateRowsDifference(startingLocation, finishLocation) == 1)) // performs a test of legal  movement based on index
            {
                if ((boardForGame.board[finishLocation] is Figure) && (whitePlayerTurn == ((Figure)boardForGame.board[finishLocation]).getIsWhiteColourFigure())) //  performs a check if a final pos is figure of same colour, the only option to be falsed                
                {
                    if (!boardForGame.isProcessOfCheckTest())
                        Console.WriteLine("Your move is illegal");
                    return false;
                }
                else
                    rightMove = true;
            }
            return rightMove;
        }
        public bool isSquaresOnKingsWayLegalForMovement(int startingLocation, int finishLocation, Board boardForGame,bool isWhitePlayerTurn)           // performs a test if a cells on a line of king moving are empty.      
        {
            bool isFreeSquares = true;
            int start = 0, finish = 0, div = 1;                                             //performs a test of movement on the same row
            start = ((startingLocation > finishLocation) ? finishLocation : startingLocation) + div;
            finish = (startingLocation < finishLocation) ? finishLocation : startingLocation;
            for (int index = start; index < finish; index += div)
            {
                if (boardForGame.board[index] != null)
                    isFreeSquares = false;
            }
            if(isFreeSquares)
            {
                if (startingLocation > finishLocation)
                { 
                    if (boardForGame.isACheck(startingLocation - 1, boardForGame, isWhitePlayerTurn) || boardForGame.isACheck(startingLocation - 2, boardForGame, isWhitePlayerTurn))
                        isFreeSquares = false;
                }
                else if (boardForGame.isACheck(startingLocation + 1, boardForGame, isWhitePlayerTurn) || boardForGame.isACheck(startingLocation + 2, boardForGame, isWhitePlayerTurn))
                        isFreeSquares = false;
            }
            if (!isFreeSquares)
            { 
                if(!boardForGame.isProcessOfCheckTest())
                    Console.WriteLine("The castling is not possible in current situation");
            }
            return isFreeSquares;
        }
        public bool IsCastlingLegal(int startingLocation, int finishLocation, Board boardForGame, bool isWhitePlayerTurn)
        {
            bool castlingAllowed = false;
            if ( boardForGame.isACheck(startingLocation,boardForGame,isWhitePlayerTurn))          //performs a test if a king of current player has a check, because this is a castling legacy test, starting location is the location that need to be tested for having check
            {
                if (!boardForGame.isProcessOfCheckTest())
                    Console.WriteLine("You can not perform castling while your king is checked");
                return false;
            }
            if ((startingLocation - finishLocation) == 2)
            {
                if ((boardForGame.board[finishLocation - 2] is Rook) && ((Rook)boardForGame.board[finishLocation - 2]).getIsBeforeFirstMove())
                {
                    castlingAllowed = isSquaresOnKingsWayLegalForMovement(startingLocation, finishLocation, boardForGame, isWhitePlayerTurn);                           //consider to add to function of test of King`s lines also test if cells under threat, both are rellevant only in situations of castling 
                    if (castlingAllowed)
                        isCastlingApprovedMinus = true;
                }
                else if (!castlingAllowed)
                {
                    if (!boardForGame.isProcessOfCheckTest())
                        Console.WriteLine("The Castling is not possible because Rook already moved");
                }
            }
            if ((finishLocation - startingLocation) == 2)
            {
                if ((boardForGame.board[finishLocation + 1] is Rook) && ((Rook)boardForGame.board[finishLocation + 1]).getIsBeforeFirstMove())
                {
                    castlingAllowed = isSquaresOnKingsWayLegalForMovement(startingLocation, finishLocation, boardForGame, isWhitePlayerTurn);
                    if (castlingAllowed)
                        isCastlingApprovedPlus = true;
                }
                else if (!castlingAllowed)
                {
                    if (!boardForGame.isProcessOfCheckTest())
                        Console.WriteLine("The Castling is not possible because Rook already moved ");
                }
            }
            return castlingAllowed;
        }
    }
    public class Utilities
    {
        public static int calculateRowsDifference(int startingLocation, int finishLocation) //function to return change in number of rows in move of a figure
        {
            return Math.Abs(startingLocation / 8 - finishLocation / 8);
        }
        public static int CalculateColumnDifference(int startingLocation, int finishLocation) //function to return change in number of rows in move of a figure
        {
            return Math.Abs(startingLocation % 8 - finishLocation % 8);
        }
    }
}
//