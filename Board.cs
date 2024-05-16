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



  public static int CheckAndShootEnemy()
  {
    // Nord
    for (int y = BotPosition.y + 1; y <= TLcorner.y; y++)
      if (Map.TryGetValue(XYtoMapKey(BotPosition.x, y), out var state) && state == CaseState.Ennemy) return 1;

    // Sud
    for (int y = BotPosition.y - 1; y >= BRcorner.y; y--)
      if (Map.TryGetValue(XYtoMapKey(BotPosition.x, y), out var state) && state == CaseState.Ennemy) return 3;

    // Est
    for (int x = BotPosition.x + 1; x <= BRcorner.x; x++)
      if (Map.TryGetValue(XYtoMapKey(x, BotPosition.y), out var state) && state == CaseState.Ennemy) return 2;

    // Ouest
    for (int x = BotPosition.x - 1; x >= TLcorner.x; x--)
      if (Map.TryGetValue(XYtoMapKey(x, BotPosition.y), out var state) && state == CaseState.Ennemy) return 4;

    return -1;
  }

}