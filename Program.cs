﻿namespace CommandLineGame;

public static class Program
{
    private const int Rows = 10;
    private const int Cols = 30;
    private static char[,] _grid = new char[Rows, Cols];
    private static int _coins;

    private const char ObstacleChar = '#';
    private const char CoinChar = '*';
    private const char PlayerChar = 'x';
    private const char WallChar = '0';
    private const char TileChar = '_';

    public static void Main()
    {
        Console.Clear();
        while (Run()) { }
        Console.WriteLine("\nBBye!");
    }

    private static bool Run()
    {
        var player = (X: 1, Y: 1);

        _grid = new char[Rows, Cols];
        _coins = 0;

        // Populate grid
        for (var x = 0; x < Cols; x++)
        {
            for (var y = 0; y < Rows; y++)
            {
                if (x == 0 || x == Cols - 1 || y == 0 || y == Rows - 1)
                {
                    _grid[y, x] = WallChar; continue;
                }
                _grid[y, x] = TileChar;
            }
        }

        // Place boxes and coins
        var rnd = new Random();
        var boxes = Enumerable.Range(0, 10).Select(_ => (X: rnd.Next(2, Cols - 2), Y: rnd.Next(2, Rows - 2))).ToArray();
        foreach (var box in boxes) { _grid[box.Y, box.X] = ObstacleChar; }
        var coinsPositions = Enumerable.Range(0, 5).Select(_ => (X: rnd.Next(1, Cols - 1), Y: rnd.Next(1, Rows - 1))).ToArray();
        foreach (var coin in coinsPositions) { _grid[coin.Y, coin.X] = CoinChar; }

        // Place the player
        _grid[player.Y, player.X] = PlayerChar;

        while (true)
        {
            Console.Clear();
            Console.WriteLine($"Welcome to this anonymous Sokoban-like game!\n\nCoins: {_coins}\n");

            // Update the grid
            for (var i = 0; i < _grid.GetLength(0); i++)
            {
                for (var j = 0; j < _grid.GetLength(1); j++)
                {
                    Console.Write(_grid[i, j]);
                }
                Console.Write("\n");
            }

            // Check winning condition
            if (_coins == coinsPositions.Length)
            {
                Console.WriteLine("\nYOU WIN!\nPRESS R TO RESTART OR ESC TO QUIT.");
                while (true)
                {
                    var key = Console.ReadKey();
                    if (key.Key == ConsoleKey.R) return true;
                    if (key.Key == ConsoleKey.Escape) return false;
                }
            }

            // elaborate user input
            Console.WriteLine("\nArrow keys => move the player\nEscape => exit.");
            var input = Console.ReadKey();
            switch (input.Key)
            {
                case ConsoleKey.Escape: return false;
                case ConsoleKey.UpArrow: Move(0, -1, ref player); break;
                case ConsoleKey.DownArrow: Move(0, 1, ref player); break;
                case ConsoleKey.LeftArrow: Move(-1, 0, ref player); break;
                case ConsoleKey.RightArrow: Move(1, 0, ref player); break;
                case ConsoleKey.R: return true;
            }
        }
    }

    private static void Move(int x, int y, ref (int X, int Y) player)
    {
        var newPos = (X: player.X + x, Y: player.Y + y);
        if (IsOutOfBounds(newPos.X, newPos.Y)) return;

        if (_grid[newPos.Y, newPos.X] == ObstacleChar)
        {
            var beyondObstaclePos = (X: newPos.X + x, Y: newPos.Y + y);
            if (!IsOutOfBounds(beyondObstaclePos.X, beyondObstaclePos.Y) && _grid[beyondObstaclePos.Y, beyondObstaclePos.X] == TileChar)
            {
                _grid[beyondObstaclePos.Y, beyondObstaclePos.X] = ObstacleChar;
                _grid[newPos.Y, newPos.X] = PlayerChar;
                _grid[player.Y, player.X] = TileChar;
                player = newPos;
            }
        }
        else if (_grid[newPos.Y, newPos.X] == CoinChar)
        {
            _coins++;
            MovePlayer(newPos, ref player);
        }
        else if (_grid[newPos.Y, newPos.X] == TileChar)
        {
            MovePlayer(newPos, ref player);
        }
    }

    private static bool IsOutOfBounds(int x, int y) => x <= 0 || y <= 0 || x >= Cols - 1 || y >= Rows - 1;

    private static void MovePlayer((int X, int Y) newPos, ref (int X, int Y) player)
    {
        _grid[newPos.Y, newPos.X] = PlayerChar;
        _grid[player.Y, player.X] = TileChar;
        player = newPos; // Update the reference to player's position directly
    }

}
