using System;
using BattleIA;

public static class Board
{
  public static Dictionary<string, CaseState> Map { get; set; } = new Dictionary<string, CaseState>();
  public static Dictionary<string, int> HeatMap { get; set; } = new Dictionary<string, int>();

  public static MapPoint BotPosition { get; set; } = new MapPoint(0, 0);
  public static MapPoint PreviousPosition { get; set; } = new MapPoint(0, 0);

  public static MapPoint TLcorner { get; private set; } = new MapPoint(0, 0); // top left
  public static MapPoint BRcorner { get; private set; } = new MapPoint(0, 0); // bottom right

  public static List<List<MapPoint>> AllPaths = new List<List<MapPoint>>();
  public static int BestPathLength;


  private static int ENERGY_VALUE = 5;
  private static int ENNEMY_VALUE = -7;



  public static string MapPointToString(MapPoint point)
  {
    return "{" + point.x + "/" + point.y + "}";
  }

  public static string XYtoMapKey(int x, int y)
  {
    return "{" + x + "/" + y + "}";
  }


  public static MapPoint StringToMapPoint(string str)
  {
    str = str.Trim('{', '}');
    // Split the string into x and y values
    string[] parts = str.Split('/');

    if (parts.Length == 2 && int.TryParse(parts[0], out int x) && int.TryParse(parts[1], out int y))
    {
      return new MapPoint(x, y);
    }
    else
    {
      // Handle invalid input here, such as returning a default value or throwing an exception
      throw new ArgumentException("Invalid MapPoint string representation: " + str);
    }
  }




  public static void UpdateMap(int x, int y, CaseState state)
  {
    if (x < TLcorner.x) TLcorner.x = x; // si plus a gauche
    if (y > TLcorner.y) TLcorner.y = y; // si plus en haut

    if (x > BRcorner.x) BRcorner.x = x; // si plus a droite
    if (y < BRcorner.y) BRcorner.y = y; // si plus en bas


    string Position = MapPointToString(new MapPoint(x, y));

    if (Map.ContainsKey(Position)) Map[Position] = state;
    else Map.Add(Position, state);

  }

  public static void UpdateHeatMap()
  {
    int DeltaX;
    int DeltaY;
    int Distance;
    int ValueToAdd;


    // mise à 0 de la heatMap
    foreach (var pair in Map)
    {
      if (HeatMap.ContainsKey(pair.Key)) HeatMap[pair.Key] = 0;
      else HeatMap.Add(pair.Key, 0);
    }

    foreach (var pair in Map)
    {
      if (pair.Key == XYtoMapKey(BotPosition.x, BotPosition.y))
      {
        foreach (var item in Map)
        {
          MapPoint Pair = StringToMapPoint(pair.Key);
          MapPoint Item = StringToMapPoint(item.Key);

          DeltaX = Math.Abs(Pair.x - Item.x);
          DeltaY = Math.Abs(Pair.y - Item.y);
          Distance = DeltaX + DeltaY;

          if (Distance <= HeatMap[item.Key]) HeatMap[item.Key] -= Distance;
        }
        continue;
      }

      // Si la case contient de l'énergie, augmentez la valeur de la heatmap
      if (pair.Value == CaseState.Energy) ValueToAdd = ENERGY_VALUE;
      else if (pair.Value == CaseState.Ennemy) ValueToAdd = ENNEMY_VALUE;
      else continue;

      foreach (var item in Map)
      {
        MapPoint Pair = StringToMapPoint(pair.Key);
        MapPoint Item = StringToMapPoint(item.Key);

        DeltaX = Math.Abs(Pair.x - Item.x);
        DeltaY = Math.Abs(Pair.y - Item.y);
        Distance = DeltaX + DeltaY;

        if (pair.Value == CaseState.Energy)
          if (Distance <= ValueToAdd) HeatMap[item.Key] += ValueToAdd - Distance;

        if (pair.Value == CaseState.Ennemy)
          if (Distance <= Math.Abs(ValueToAdd)) HeatMap[item.Key] += ValueToAdd + Distance;
      }
    }

    foreach (var pair in Map)
    {
      if (Map[pair.Key] == CaseState.Wall) HeatMap[pair.Key] = -99;
    }
  }




  public static void PrintMap()
  {
    foreach (var kvp in Map)
    {
      Console.WriteLine($"Key: {kvp.Key}, Value: {kvp.Value}");
    }
  }


  public static void DisplayMap()
  {
    Console.WriteLine("DisplayMap:");


    for (int j = TLcorner.y; j > BRcorner.y - 1; j--)
    {
      for (int i = TLcorner.x; i < BRcorner.x + 1; i++)
      {
        if (Map.TryGetValue("{" + i + "/" + j + "}", out var state))
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


  // public static string FormatTwoDigits(int number)
  // {
  //   if (number < 10) return "0" + number.ToString();
  //   return number.ToString();
  // }


  public static void DisplayHeatMap()
  {
    Console.WriteLine("DisplayHeatMap:");

    for (int j = TLcorner.y; j > BRcorner.y - 1; j--)
    {
      for (int i = TLcorner.x; i < BRcorner.x + 1; i++)
      {
        if (HeatMap.TryGetValue("{" + i + "/" + j + "}", out var value))
          if (i == BotPosition.x && j == BotPosition.y) WriteInColor(ConsoleColor.DarkGreen, value.ToString().PadLeft(3));

          else if (Map[XYtoMapKey(i, j)] == CaseState.Wall) WriteInColor(ConsoleColor.Red, value.ToString().PadLeft(3));
          else Console.Write(value.ToString().PadLeft(3)); // complete avec des espaces devant pour avoir 3 char de long

        else Console.Write("   "); // Pas dans la Map
      }
      Console.WriteLine();
    }
  }


  public static void updateBotPosition(int move)
  {
    PreviousPosition = new MapPoint(BotPosition.x, BotPosition.y);
    Map[MapPointToString(BotPosition)] = CaseState.Empty;
    // sachant que 1 = North, 2 = West, 3 = South et 4 = East
    switch (move)
    {
      case 1: BotPosition.y += 1; break;
      case 2: BotPosition.x -= 1; break;
      case 3: BotPosition.y -= 1; break;
      case 4: BotPosition.x += 1; break;
      default: break;
    }

    Map[MapPointToString(BotPosition)] = CaseState.Ennemy;
    Console.WriteLine($"New bot position: ({BotPosition.x}, {BotPosition.y})");
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
    for (int i = 0; i < AllPaths.Count; i++)
    {
      Console.WriteLine($"Path {i + 1}:");
      ShowPath(AllPaths[i]);
    }
  }



  public static void WriteInColor(ConsoleColor color, string text)
  {
    ConsoleColor originalColor = Console.ForegroundColor;
    Console.ForegroundColor = color;
    Console.Write(text);
    Console.ForegroundColor = originalColor;
  }


}