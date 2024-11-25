using System;

namespace SOSGame
{
    class Program
    {
        static char playerSymbol;
        static char playerSymbol2;
        static char[,] board = new char[3, 3];

        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to SOS Classic!");

            while (true)
            {
                Console.WriteLine("\nMain Menu:");
                Console.WriteLine("1. Player 1 VS Player 2");
                Console.WriteLine("2. Player 1 VS Computer");
                Console.WriteLine("3. Exit");

                int choice = GetMenuChoice(1, 3);

                if (choice == 3)
                {
                    Console.WriteLine("Thank you for playing!");
                    break;
                }

                InitializeBoard();

                switch (choice)
                {
                    case 1:
                        PlayGameAgainstPlayer();
                        break;
                    case 2:
                        PlayGameAgainstComputer();
                        break;
                }
            }
        }

        static int GetMenuChoice(int min, int max)
        {
            int choice;
            while (true)
            {
                Console.Write("Select menu (number): ");
                if (int.TryParse(Console.ReadLine(), out choice) && choice >= min && choice <= max)
                {
                    return choice;
                }
                Console.WriteLine("Invalid option. Please try again.");
            }
        }

        static void InitializeBoard()
        {
            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < 3; col++)
                {
                    board[row, col] = ' ';
                }
            }
        }

        static void DisplayBoard()
        {
            Console.WriteLine("-------------");
            for (int row = 0; row < 3; row++)
            {
                Console.Write("| ");
                for (int col = 0; col < 3; col++)
                {
                    Console.Write(board[row, col] + " | ");
                }
                Console.WriteLine("\n-------------");
            }
        }

        static bool PlaceSymbol(int row, int col, char symbol)
        {
            if (board[row, col] == ' ')
            {
                board[row, col] = symbol;
                return true;
            }
            return false;
        }

        static bool CheckForWin() // VS player 2 mode
        {
            // Checking diagonal pattern

            if ((board[0, 0] == 'S' && board[1, 1] == 'O' && board[2, 2] == 'S') ||
                (board[0, 2] == 'S' && board[1, 1] == 'O' && board[2, 0] == 'S'))
            {
                return true;
            }

            // Checking vertical pattern

            else if ((board[0, 0] == 'S' && board[1, 0] == 'O' && board[2, 0] == 'S') ||
                (board[0, 1] == 'S' && board[1, 1] == 'O' && board[2, 1] == 'S') ||
                (board[0, 2] == 'S' && board[1, 2] == 'O' && board[2, 2] == 'S'))
            {
                return true;
            }

            // Checking horizontal pattern

            else if ((board[0, 0] == 'S' && board[0, 1] == 'O' && board[0, 2] == 'S') ||
                (board[1, 0] == 'S' && board[1, 1] == 'O' && board[1, 2] == 'S') ||
                (board[2, 0] == 'S' && board[2, 1] == 'O' && board[2, 2] == 'S'))
            {
                return true;
            }
            return false;
        }

        static bool CheckForWin2() // VS Computer mode
        {
            // Checking diagonal pattern

            if ((board[0, 0] == 'S' && board[1, 1] == 'O' && board[2, 2] == 'S') ||
                (board[0, 2] == 'S' && board[1, 1] == 'O' && board[2, 0] == 'S'))
            {
                return true;
            }

            // Checking vertical pattern

            else if ((board[0, 0] == 'S' && board[1, 0] == 'O' && board[2, 0] == 'S') ||
                (board[0, 1] == 'S' && board[1, 1] == 'O' && board[2, 1] == 'S') ||
                (board[0, 2] == 'S' && board[1, 2] == 'O' && board[2, 2] == 'S'))
            {
                return true;
            }

            // Checking horizontal pattern

            else if ((board[0, 0] == 'S' && board[0, 1] == 'O' && board[0, 2] == 'S') ||
                (board[1, 0] == 'S' && board[1, 1] == 'O' && board[1, 2] == 'S') ||
                (board[2, 0] == 'S' && board[2, 1] == 'O' && board[2, 2] == 'S'))
            {
                return true;
            }
            return false;
        }


        static bool IsBoardFull()
        {
            foreach (char cell in board)
            {
                if (cell == ' ')
                {
                    return false;
                }
            }
            return true;
        }

        static void PlayGameAgainstPlayer()
        {
            bool playerTurn = true;

            while (true)
            {
                DisplayBoard();

                int currentPlayer = playerTurn ? 1 : 2;

                Console.WriteLine($"Player {currentPlayer}, enter coordinate (Row and column):");

                Console.Write("Select row (0-2): ");
                int row = int.Parse(Console.ReadLine());
                Console.Write("Select column (0-2): ");
                int col = int.Parse(Console.ReadLine());

                Console.Write("Player 1, Select input 'S' or 'O' (Capital only): ");
                char playerSymbol = Console.ReadLine()[0];

                if (PlaceSymbol(row, col, playerSymbol))
                {
                    if (CheckForWin())
                    {
                        DisplayBoard();
                        Console.WriteLine($"Player {currentPlayer} WINS!");
                        break;
                    }
                    else if (IsBoardFull())
                    {
                        DisplayBoard();
                        Console.WriteLine("It's a DRAW!");
                        break;
                    }

                    playerTurn = !playerTurn;
                }
                else
                {
                    Console.WriteLine("Coordinate is taken. Please try another one");
                }
            }
        }

        static void PlayGameAgainstComputer()
        {
            bool playerTurn = true;

            while (true)
            {
                DisplayBoard();

                int currentPlayer = playerTurn ? 1 : 2;

                // Human or computer movement selection

                if (currentPlayer == 1)//player Human
                {
                    Console.WriteLine("Player 1, Enter coordinate (Row and column):");
                    Console.Write("Select row (0-2): ");
                    int row = int.Parse(Console.ReadLine());
                    Console.Write("Select column (0-2): ");
                    int col = int.Parse(Console.ReadLine());
                    Console.Write("Player 1, select input 'S' or 'O' (Capital Only): ");
                    char playerSymbol = Console.ReadLine()[0];

                    if (PlaceSymbol(row, col, playerSymbol))
                    {
                        if (CheckForWin2())
                        {
                            DisplayBoard();
                            Console.WriteLine($"Player {currentPlayer} WINS!");
                            break;
                        }
                        else if (IsBoardFull())
                        {
                            DisplayBoard();
                            Console.WriteLine("It's a DRAW!");
                            break;
                        }

                        playerTurn = !playerTurn;
                    }
                    else
                    {
                        Console.WriteLine("The coordinate is taken. Please try another one");
                    }
                }

                else if (currentPlayer == 2)//Computer player
                {
                    // Random row selection
                    Console.WriteLine($"Computer is picking the coordinate:");
                    int[] baris = new int[] { 0, 1, 2 };
                    Random random1 = new Random();
                    int row2 = baris[random1.Next(baris.Length)];
                    Console.WriteLine($"Select row (0-2) : {row2}");

                    // Random column selection
                    int[] kolom = new int[] { 0, 1, 2 };
                    Random random2 = new Random();
                    int col2 = kolom[random2.Next(kolom.Length)];
                    Console.WriteLine($"Select column (0-2) : {col2}");

                    // Random pick S or O
                    char[] pilihan = { 'S', 'O' };
                    Random random = new Random();
                    char playerSymbol2 = pilihan[random.Next(pilihan.Length)];


                    if (PlaceSymbol(row2, col2, playerSymbol2))
                    {
                        if (CheckForWin2())
                        {
                            DisplayBoard();
                            Console.WriteLine($"Player {currentPlayer} WINS!");
                            break;
                        }
                        else if (IsBoardFull())
                        {
                            DisplayBoard();
                            Console.WriteLine("It's a DRAW!");
                            break;
                        }

                        playerTurn = !playerTurn;
                    }
                    else
                    {
                        Console.WriteLine("Coordinate is  taken. Please try another one");
                    }

                }
            }
        }
    }
}