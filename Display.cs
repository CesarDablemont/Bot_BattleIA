using System;
using BattleIA;

public static class Display
{
  public static void PrintMap()
  {
    foreach (var kvp in Board.Map)
    {
      Console.WriteLine($"Key: {kvp.Key}, Value: {kvp.Value}");
    }
  }


  public static void DisplayMap()
  {
    Console.WriteLine("DisplayMap:");

    for (int j = Board.TLcorner.y; j > Board.BRcorner.y - 1; j--)
    {
      for (int i = Board.TLcorner.x; i < Board.BRcorner.x + 1; i++)
      {
        if (Board.Map.TryGetValue("{" + i + "/" + j + "}", out var state))
        {
          switch (state)
          {
            case CaseState.Empty: Console.Write("· "); break;
            case CaseState.Energy: Console.Write("# "); break;
            case CaseState.Ennemy: Console.Write("* "); break;
            case CaseState.Wall: Console.Write("██"); break;
            case CaseState.Virtual: Console.Write("V "); break;
            default: Console.Write("  "); break;
          }
        }
        else Console.Write("  "); // Pas dans la Map

      }
      Console.WriteLine();
    }
  }


  public static void DisplayHeatMap()
  {
    Console.WriteLine("DisplayHeatMap:");

    for (int j = Board.TLcorner.y; j > Board.BRcorner.y - 1; j--)
    {
      for (int i = Board.TLcorner.x; i < Board.BRcorner.x + 1; i++)
      {
        if (Board.HeatMap.TryGetValue("{" + i + "/" + j + "}", out var value))
          if (i == Board.BotPosition.x && j == Board.BotPosition.y) WriteInColor(ConsoleColor.DarkGreen, value.ToString().PadLeft(3));

          else if (Board.Map[Board.XYtoMapKey(i, j)] == CaseState.Wall) WriteInColor(ConsoleColor.Red, value.ToString().PadLeft(3));
          else Console.Write(value.ToString().PadLeft(3)); // Complété avec des espaces devant pour avoir 3 caractères de long

        else Console.Write("   "); // Pas dans la Map
      }
      Console.WriteLine();
    }
  }

  public static void WriteInColor(ConsoleColor color, string text)
  {
    ConsoleColor originalColor = Console.ForegroundColor;
    Console.ForegroundColor = color;
    Console.Write(text);
    Console.ForegroundColor = originalColor;
  }

  public static void ShowPath(List<MapPoint> path)
  {
    Console.WriteLine("Path:");

    foreach (MapPoint point in path)
    {
      Console.WriteLine($"({point.x}, {point.y})");
    }
  }

  public static void ShowAllPaths()
  {
    Console.WriteLine("All Paths:");
    for (int i = 0; i < Board.AllPaths.Count; i++)
    {
      Console.WriteLine($"Path {i + 1}:");
      ShowPath(Board.AllPaths[i]);
    }
  }


}