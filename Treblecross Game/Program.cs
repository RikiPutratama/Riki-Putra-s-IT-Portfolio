
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace GameFramework
{
    public enum Games
    {
        Treblecross,
    }
    public enum Action
    {
        MakeMove,
        InputCommand
    }
    public enum Commands
    {
        Undo,
        Redo,
        Save,
        ExitMenu,
        QuitGame
    }

    public interface IMoveValidator
    {
        List<Move> ValidMoves { get; }
        void CalculateValidMoves(GameBoard gameBoard);
    }
    class Program
    {
        static void Main()
        {
            GameLauncher.Instance.Run();

        }
    }
    public class GameLauncher
    {
        private static readonly GameLauncher instance = new();
        private GameLauncher() { }
        public static GameLauncher Instance => instance;

        private Games _selectedGame;
        public void Run()
        {
            bool exit = false;
            while (!exit)
            {
                SelectGame();

                GameState? gamestate = GetGameState();

                if (gamestate != null) StartGame(gamestate);
                else Console.WriteLine("Something went wrong when trying to start the game");

                exit = PromptForExit();
            }
        }
        private static int PromptForLoadingSave()
        {
            int option;
            do
            {
                Console.WriteLine("Saved games were found, do you want to load an existing save or start a new game ?");
                Console.WriteLine("1. Start a new game");
                Console.WriteLine("2. Load Save");
                if (!int.TryParse(Console.ReadLine(), out option) || !(option == 1 || option == 2))
                {
                    Console.WriteLine("\n Invalid input. Please enter 1 or 2.");
                }
            } while (option != 1 && option != 2);

            return option;
        }
        public void SelectGame()
        {
            Console.WriteLine("Available games:");
            foreach (Games game in Enum.GetValues(typeof(Games))) Console.WriteLine($"{(int)game}. {game}");

            Console.Write("Select a game by entering its number: ");
            while (true)
            {
                if (int.TryParse(Console.ReadLine(), out int selectedGameIndex) &&
                    Enum.IsDefined(typeof(Games), selectedGameIndex))
                {
                    _selectedGame = (Games)selectedGameIndex;
                    Console.WriteLine($"\n You selected: {_selectedGame} \n");
                    return;
                }
                else Console.WriteLine("Invalid input. Please select a valid game.");
            }
        }
        private GameState? GetGameState()
        {
            string[] fileSaves = GameLoader.CheckForSaveFiles(_selectedGame);

            if (fileSaves.Length > 0)
            {
                int option = PromptForLoadingSave();
                if (option == 2) return GameLoader.LoadSave(SelectSave(fileSaves));
            };
            return CreateGameState();
        }
        public static string SelectSave(string[] fileSaves)
        {
            while (true)
            {
                Console.WriteLine("Select a save by entering its number:");

                int i = 1;
                foreach (string save in fileSaves)
                {
                    Console.WriteLine($"{i}.{save}");
                    i++;
                }

                if (int.TryParse(Console.ReadLine(), out int selectedSave) && (selectedSave <= fileSaves.Length && selectedSave > 0))
                {
                    Console.WriteLine($" \n Loading {fileSaves[selectedSave - 1]} ... \n ");
                    return fileSaves[selectedSave - 1];
                }
                else
                {
                    Console.WriteLine($"\n Invalid input. Please enter a number between 1 and {fileSaves.Length}");
                }
            }
        }
        private GameState CreateGameState()
        {
            GameStateFactory gamestatefactory;
            switch (_selectedGame)
            {
                case Games.Treblecross:
                    gamestatefactory = new TreblecrossGamestateFactory();
                    break;
                default:
                    Console.WriteLine("Not implemented Yet");
                    throw new NotImplementedException("The specified game is not implemented yet.");
            }
            return gamestatefactory.CreateGameState();
        }
        private static bool PromptForExit()
        {
            while (true)
            {
                Console.WriteLine("\n Do you want to play another game? ");
                Console.WriteLine("1.Yes");
                Console.WriteLine("2.No");

                int input;

                while (!(int.TryParse(Console.ReadLine(), out  input) && input == 1 || input == 2))
                {
                    Console.WriteLine("Invalid input. Please enter 1 or 2.");

                }
                if (input == 1)
                    return false;

                else {
                    Console.WriteLine("Thanks for playing.");
                    return true;
                };

            }
        }
        public void StartGame(GameState gamestate)
        {
            Console.WriteLine("\n ...Loading game... \n");

            switch (_selectedGame)
            {
                case Games.Treblecross:
                    TrebleCross game = new(gamestate, Games.Treblecross);
                    game.PlayGame();
                    break;
                default:
                    throw new NotImplementedException("The specified game is not implemented yet.");
            }


        }
    }
    public abstract class GameStateFactory
    {
        public abstract GameState CreateGameState();
        protected abstract Players ConfigurePlayers();
        protected abstract GameBoard ConfigureGameBoard();
        protected static void SetStartingPlayer(Players players)
        {
            while (true)
            {
                Console.WriteLine("\n Select who's going first:");
                Console.WriteLine($"1. {players.Player1.Name}");
                Console.WriteLine($"2. {players.Player2.Name}");
                string? input = Console.ReadLine();
                if (int.TryParse(input, out int startingPlayer) && (startingPlayer == 1 || startingPlayer == 2))
                {
                    players.SetTurnPlayer(startingPlayer == 1 ? players.Player1 : players.Player2);
                    return;
                }
                else
                {
                    Console.WriteLine("Invalid input. Type 1 for player 1 and 2 for player 2.");
                }
            }

        }
    }
    public class TreblecrossGamestateFactory : GameStateFactory
    {
        public override GameState CreateGameState()
        {
            Players players = ConfigurePlayers();
            SetStartingPlayer(players);
            GameBoard gameBoard = ConfigureGameBoard();

            return new GameState(players, gameBoard);
        }
        protected override Players ConfigurePlayers()
            {
                while (true)
                {
                    Console.WriteLine("Select mode:");
                    Console.WriteLine("1. Human vs CPU");
                    Console.WriteLine("2. Human vs Human ");

                    if (int.TryParse(Console.ReadLine(), out int numberOfPlayers) && (numberOfPlayers == 1 || numberOfPlayers == 2))
                    {
                        Player player1 = numberOfPlayers == 1 ? new Human('X', "Human Player") : new Human('X', "Player 1");
                        Player player2 = numberOfPlayers == 1 ? new CPU('X', "CPU Player") : new Human('X', "Player 2");
                        return new Players(player1, player2, player1);
                    }
                    else
                    {
                        Console.WriteLine("\n Invalid input. Please enter 1 or 2.");
                    }
                }
            }
        protected override GameBoard ConfigureGameBoard()
        {
            Console.WriteLine("Enter the length of the board:");
            while (true)
            {
                string? input = Console.ReadLine();
                if (int.TryParse(input, out int boardSize) && boardSize > 3 && boardSize < 50)
                {
                    GameBoard board = new(1, boardSize);
                    return board;


                }
                else
                {
                    Console.WriteLine("Invalid input. Board size should be between 3 and 50");
                }
            }
        }
    }
    public abstract class Game (GameState gamestate, Games game)
    {
        public Games GameName = game;
        public GameState GameState { get; private set; } = gamestate;
        public abstract IMoveValidator MoveValidator { get; set; }
        public History History { get; set; } = new History();


        protected bool exitGame = false;
        protected Player? winner;

        protected abstract bool EndOfGame();
        protected virtual Action PlayerTurn()
        {
            if (GameState.Players.TurnPlayer is Human humanPlayer)
            {
                Action action = Human.SelectAction();
                if (action == Action.InputCommand)
                {
                    HandleCommands(humanPlayer);

                    return Action.InputCommand;
                }
            }

            Move playerMove = GameState.Players.TurnPlayer.MakeMove(GameState.GameBoard, MoveValidator.ValidMoves);
            GameState.GameBoard.PlacePiece(playerMove);
            History.Push(playerMove, GameState.Players.TurnPlayer);
            Console.WriteLine($"\n {GameState.Players.TurnPlayer.Name} placed {GameState.Players.TurnPlayer.Symbol} on cell ({playerMove.Row},{playerMove.Column}) \n ");
            return Action.MakeMove;


        }
        protected virtual void SwitchPlayer()
        {
            GameState.Players.SwitchPlayers();
        }
        protected virtual void HandleCommands(Human humanPlayer)
        {
            bool exit = false;
            do
            {
                Commands playerCommand = Human.InputCommand();
                if (playerCommand == Commands.Undo)
                    Undo();
                if (playerCommand == Commands.Redo)
                    Redo();
                if (playerCommand == Commands.Save)
                    Save();
                if (playerCommand == Commands.QuitGame)
                {
                    QuitGame();
                    exit = true;
                }
                if (playerCommand == Commands.ExitMenu)
                    exit = true;

            }
            while (exit == false);
            return;
        }
        protected virtual void Undo()
        {
            var previousMove = History.GetPreviousMove();
            if (previousMove != null)
            {
                var (move, player) = previousMove.Value;
                GameState.GameBoard.RemovePiece(move);
                GameState.Players.SetTurnPlayer(player);
            }
            else Console.WriteLine("\n Error: There are no previous moves");
            GameState.GameBoard.DisplayBoard();
        }
        protected virtual void Redo()
        {
            var NextMove = History.GetNextMove();
            if (NextMove != null)
            {
                var (move, player) = NextMove.Value;
                GameState.GameBoard.PlacePiece(move);
                if (player == GameState.Players.TurnPlayer)
                    GameState.Players.SwitchPlayers();
                //GameState.Players.SetTurnPlayer(player);
            }
            else Console.WriteLine("\n Error: Already at the last available move in history");
            GameState.GameBoard.DisplayBoard();
        }
        protected virtual void Save()
        {
            GameSaver.SaveGameState(this);
        }
        protected virtual void QuitGame()
        {
            exitGame = true;
            Console.WriteLine($"Quitting {GameName}...");

        }
        protected virtual void PrintWinner()
        {
            if (winner != null)
                Console.WriteLine("{0} Won !", winner.Name);
            else Console.WriteLine("Something went wrong.");
        }
        public virtual void PlayGame()
        {
            bool gameEnded = false;
            while (!gameEnded && !exitGame)
            {
                GameState.GameBoard.DisplayBoard();
                Console.WriteLine("\n {0}'s turn :\n", GameState.Players.TurnPlayer.Name);
                MoveValidator.CalculateValidMoves(GameState.GameBoard);
                Action playerAction = PlayerTurn();

                gameEnded = EndOfGame();

                if (!gameEnded && playerAction == Action.MakeMove)
                {
                    SwitchPlayer();
                }
                
            }

            if (!exitGame)
            {
                winner = GameState.Players.TurnPlayer;
                GameState.GameBoard.DisplayBoard();
                PrintWinner();
            }
        }
    }
    public class TrebleCross(GameState gamestate, Games gamename) : Game(gamestate, gamename)
    {
      public override  IMoveValidator MoveValidator { get; set; } = new TreblecrossMoveValidator();
        protected override bool EndOfGame()
        {
            char[][] board = GameState.GameBoard.Board;

            int height = board.Length;
            int width = board[0].Length;

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    if (col != 0
                        && col < width - 1
                        && board[row][col] == 'X'
                        && board[row][col + 1] == 'X'
                        && board[row][col - 1] == 'X')
                        return true;
                }

            }
            return false;
        }
    }
    public class Players(Player player1, Player player2, Player TurnPlayer)
    {

        public Player Player1 { get; set; } = player1;
        public Player Player2 { get; set; } = player2;
        public Player TurnPlayer { get; private set; } = TurnPlayer;

        public void SwitchPlayers()
        {
            TurnPlayer = TurnPlayer == Player1 ? Player2 : Player1;
        }

        public void SetTurnPlayer(Player player)
        {
            TurnPlayer = player;
        }
    }
    public abstract class Player
    {
        public char Symbol { get; set; }
        public string? Name { get; set; }
        public string PlayerType { get; set; }

        protected Player(char symbol, string? name)
        {
            Symbol = symbol;
            Name = name;
            PlayerType = GetType().Name;
        }
        public abstract Move MakeMove(GameBoard GameBoard, List<Move> validMoves);

    }
    public class Human(char Symbol, string? Name) : Player(Symbol, Name)
    {
        public static Action SelectAction()
        {
            Console.WriteLine("Choose your action:");
            Console.WriteLine("1. Place a piece");
            Console.WriteLine("2. Input a command");
            int choice;
            while (!(int.TryParse(Console.ReadLine(), out choice) && choice == 1 || choice == 2))
            {
                Console.WriteLine("Invalid choice. Please enter 1 or 2.");
            }
            if (choice == 1)
                return Action.MakeMove;
            else
                return Action.InputCommand;
        }

        public override Move MakeMove(GameBoard GameBoard, List<Move> validMoves)
        {

            int height = GameBoard.Board.Length;
            int width = GameBoard.Board[0].Length;

            int row = -1;
            int column = -1;
            bool validMove = false;
            while (!validMove)
            {
                Console.WriteLine(height > 1 && width > 1 ?

                "\nEnter the Row & Column number where you want to place your piece: " :
                height > 1 ? "\nEnter the Row number where you want to place your piece:" :
                "\nEnter the Column number where you want to place your piece:");

                string? input = Console.ReadLine();

                if (height > 1 && width > 1)
                {
                    string[]? inputParts = input?.Split();

                    if (inputParts?.Length == 2 &&
                        int.TryParse(inputParts[0], out row) && int.TryParse(inputParts[1], out column) &&
                        row >= 1 && row <= height && column >= 1 && column <= width) { }
                    else
                    {
                        Console.WriteLine("Invalid input. \n Please enter valid Row & Column numbers with a space between them. Ex: 1 3");
                        continue;
                    }
                }
                else if (height > 1)
                {
                    if (int.TryParse(input, out row) && row >= 1 && row <= height)
                    {
                        column = 1;
                    }
                    else
                    {
                        Console.WriteLine("Invalid input. Please enter valid Row numbers.  Ex: 5 ");
                        continue;
                    }
                }
                else
                {

                    if (int.TryParse(input, out column) && column >= 1 && column <= width)
                    {
                        row = 1;

                    }
                    else
                    {
                        Console.WriteLine("Invalid input. Please enter valid Column numbers.  Ex: 5");
                        continue;
                    }
                }
                if (!validMoves.Any(move => move.Row == row-1 && move.Column == column-1))
                {
                    Console.WriteLine("\n Invalid move \n");
                }
                else validMove = true;
            }
            return new Move(row-1, column-1, Symbol);
        }
        public static Commands InputCommand()
        {
            Console.WriteLine("\nChoose your command:");
            foreach (Commands command in Enum.GetValues(typeof(Commands)))
            {
                Console.WriteLine($"{(int)command}. {command}");
            }

            while (true)
            {
                string? choice = Console.ReadLine();

                if (Enum.TryParse(choice, true, out Commands selectedCommand)
                    && (Enum.IsDefined(typeof(Commands), selectedCommand)))
                    return selectedCommand;
                else Console.WriteLine("Invalid command. Please enter a valid option.");
            }
        }
    }
    public class CPU(char Symbol, string? Name) : Player(Symbol, Name)
    {
        public override Move MakeMove(GameBoard GameBoard, List<Move> validMoves)
        {
            Random random = new();
            int randomIndex = random.Next(0, validMoves.Count);
            return validMoves[randomIndex]; ;
        }


    }
    public class GameBoard
    {
        public char[][] Board { get; set; }
        public GameBoard(int rows, int columns)
        {
            char[][] board = new char[rows][];
            for (int i = 0; i < rows; i++)
            {
                board[i] = new char[columns];
            }
            Board = board;

        }
        // Constructor used by JsonDeserializer
        [JsonConstructor]
        public GameBoard(char[][] board)
        {
            Board = board;
        }
        public void PlacePiece(Move move)
        {
            Board[move.Row][move.Column] = move.Symbol;
        }
        public void ReplacePiece(Move move)
        {
            if (Board[move.Row][move.Column] != '\0')
                Board[move.Row][move.Column] = move.Symbol;
        }
        public bool IsEmpty(int Row, int Column)
        {
            return Board[Row][Column] == '\0';
        }
        public void RemovePiece(Move move)
        {
            if (Board[move.Row][move.Column] != '\0')
                Board[move.Row][move.Column] = '\0';
        }
        public void DisplayBoard()
        {
            int height = Board.Length;
            int width = Board[0].Length;

            // Write the number for each column
            Console.Write("   ");
            for (int c = 0; c < width; c++)
            {
                Console.Write($"{c+1,4}");

            }
            //Write the top of each row
            Console.WriteLine();
            Console.Write("  ");
            Console.Write("┌" + new string('─', 4 * width + 1) + "┐");
            Console.WriteLine();

            for (int i = 0; i < height; i++)
            {
                // Write Left margin with its row number
                Console.Write($"{i+1,3} ");

                Console.Write("│");

                for (int j = 0; j < width; j++)
                {
                    //Write each cell and its content
                    char content = Board[i][j];
                    string cellContent = content == '\0' ? " " : content.ToString();

                    Console.Write($" {cellContent} │");
                }

                Console.WriteLine();

                //Write the bottom of the row
                if (i < height - 1)
                {
                    Console.Write("  ├");
                    Console.Write(new string('─', 4 * width + 1));
                    Console.WriteLine("┤");
                }
            }
            //Write the bottom of the grid

            Console.Write("  ");
            Console.Write("└" + new string('─', 4 * width + 1) + "┘");
            Console.WriteLine();
        }
    }
    public class TreblecrossMoveValidator : IMoveValidator
    {
        public List<Move> ValidMoves { get; private  set; } = [];

        public virtual void CalculateValidMoves(GameBoard GameBoard)
        {
            List<Move> NewValidMoves = [];
            int height = GameBoard.Board.Length;
            int width = GameBoard.Board[0].Length;
            for (int row = 0; row < height; row++)
            {
                for (int column = 0; column < width; column++)
                {

                    if (GameBoard.IsEmpty(row, column))
                    {

                        NewValidMoves.Add(new Move(row, column, 'X'));
                    }
                }
            }
            ValidMoves = NewValidMoves;
        }

    }
    public class Move(int row, int column, char symbol)
    {
        public int Row = row;
        public int Column = column;
        public char Symbol = symbol;
    }
    public class History()
    {
        private List<(Move, Player)> history = [];
        private int index = -1;
        public void Push(Move move, Player player)
        {
            if (index + 1 < history.Count)
            {
                history.RemoveRange(index + 1, history.Count - index - 1);

            }
            
            history.Add((move, player));
            
            index++;

        }
        public (Move, Player)? GetPreviousMove()
        {
            if (index > -1)
            {
                var previousMove = history[index];
                index--;
                return previousMove;
            }
            return null;
        }
        public (Move, Player)? GetNextMove()
        {
            Console.WriteLine($"{index},{history.Count}");

            if (index < history.Count - 1)
            {
                index++;
                return history[index];
            }
            return null;
        }
        public void PrintHistory()
        {
            foreach (var (move, player) in history)
            {
                Console.WriteLine($"Player: {player.Name}, Move: ({move.Row},{move.Column})");
            }
        }
    }
    public class GameState(Players players, GameBoard gameBoard)
    {
        public Players Players { get; set; } = players;
        public GameBoard GameBoard { get; set; } = gameBoard;
    }
    public class GameSaver
    {
        public static void SaveGameState(Game Game)
        {
            try
            {
                string saveDirectory = $"GameSave/{Game.GameName}";
                Directory.CreateDirectory(saveDirectory);
                string formattedDateTime = DateTime.Now.ToString("yyyy-MM-dd");
                string fileName = GenerateUniqueFileName(saveDirectory, formattedDateTime, ".json");
                string json = JsonSerializer.Serialize(Game.GameState);

                File.WriteAllText(fileName, json);
                Console.WriteLine($"Saved{fileName}:\n{json}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving game state: {ex.Message}");
            }
        }

        private static string GenerateUniqueFileName(string directory, string formattedDateTime, string extension)
        {
            int count = 0;
            string fileName;
            do
            {
                fileName = Path.Combine(directory, $"{formattedDateTime}{(count > 0 ? $" ({count})" : null)}{extension}");
                count++;
            } while (File.Exists(fileName));

            return fileName;
        }
    }
    public class GameLoader
    {

        private static readonly JsonSerializerOptions s_readOptions = new()
        {
            IncludeFields = true,
            Converters = { new PlayerConverter() } // Add converters here
        };
        public static string[] CheckForSaveFiles(Games game)
        {
            {
                string saveDirectory = $"GameSave/{game}";
                Directory.CreateDirectory(saveDirectory);
                string[] saveFiles = Directory.GetFiles(saveDirectory, $"*.json");
                return saveFiles;
            }
        }
        public static GameState? LoadSave(string saveName)
        {
            GameState? gamestate = DeserializeSave(saveName);
            return gamestate ?? throw new Exception("Impossible to load the file");
        }
            private static GameState? DeserializeSave(string filePath)
        {
            try
            {
                string json = File.ReadAllText(filePath);
                GameState? gameState = JsonSerializer.Deserialize<GameState>(json, s_readOptions);
                return gameState;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading game state: {ex.Message}");
                return null;
            }
        }
    }
    public class PlayerConverter : JsonConverter<Player>
    {
        public override Player? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var jsonDocument = JsonDocument.ParseValue(ref reader);
            var jsonObject = jsonDocument.RootElement;

            string? playerType = jsonObject.GetProperty("PlayerType").GetString();

            Player? player = playerType switch
            {
                "Human" => JsonSerializer.Deserialize<Human>(jsonObject.GetRawText(), options),
                "CPU" => JsonSerializer.Deserialize<CPU>(jsonObject.GetRawText(), options),
                _ => throw new JsonException($"Unknown player type: {playerType}")
            };
            return player;
        }

        public override void Write(Utf8JsonWriter writer, Player value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }

}

