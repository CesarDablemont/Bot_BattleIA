using System;
using System.Threading.Tasks.Dataflow;
using BattleIA;

public static class MoveBot
{
  static Random rnd = new Random();


  private static string PreviewBotPosition(int move)
  {
    // sachant que 1 = North, 2 = West, 3 = South et 4 = East
    switch (move)
    {
      case 1: return Board.XYtoMapKey(Board.BotPosition.x, Board.BotPosition.y + 1);
      case 2: return Board.XYtoMapKey(Board.BotPosition.x - 1, Board.BotPosition.y);
      case 3: return Board.XYtoMapKey(Board.BotPosition.x, Board.BotPosition.y - 1);
      case 4: return Board.XYtoMapKey(Board.BotPosition.x + 1, Board.BotPosition.y);
      default: return Board.XYtoMapKey(0, 0);
    }
  }

  public static int RandomMoveSafe()
  {
    List<int> validMoves = new List<int>();

    for (int move = 1; move <= 4; move++)
    {
      string Key = PreviewBotPosition(move);
      if (Key == Board.MapPointToString(Board.PreviousPosition)) continue;

      CaseState c = Board.Map[Key];
      if (c != CaseState.Wall && c != CaseState.Malus && c != CaseState.Ennemy) validMoves.Add(move);
    }

    if (validMoves.Count == 0) return -1;

    int randomIndex = rnd.Next(validMoves.Count);
    return validMoves[randomIndex];
  }


  public static int TryEscape()
  {
    List<int> validMoves = new List<int>();
    for (int move = 1; move <= 4; move++)
    {
      string Key = PreviewBotPosition(move);
      CaseState c = Board.Map[Key];
      if (c != CaseState.Wall) validMoves.Add(move);
    }

    if (validMoves.Count == 0) return -1;

    int randomIndex = rnd.Next(validMoves.Count);
    return validMoves[randomIndex];
  }


  public static int GoCloseEnergy()
  {
    List<int> validMoves = new List<int>();
    int highestScore = int.MinValue;
    int bestMove = -1;

    for (int move = 1; move <= 4; move++)
    {
      string key = PreviewBotPosition(move);
      if (key == Board.MapPointToString(Board.PreviousPosition)) continue;

      CaseState c = Board.Map[key];
      if (c == CaseState.Energy || c == CaseState.Bonus)
      {
        validMoves.Add(move);
        int caseScore = Board.HeatMap[key];
        if (caseScore > highestScore)
        {
          highestScore = caseScore;
          bestMove = move;
        }
      }
    }

    if (validMoves.Count == 0) return -1;
    return bestMove;
  }

  public static int GoBestHeatMap()
  {
    List<int> validMoves = new List<int>();
    int highestScore = int.MinValue;
    int bestMove = -1;

    for (int move = 1; move <= 4; move++)
    {
      string key = PreviewBotPosition(move);
      if (key == Board.MapPointToString(Board.PreviousPosition)) continue;

      validMoves.Add(move);
      int caseScore = Board.HeatMap[key];
      if (caseScore > highestScore)
      {
        highestScore = caseScore;
        bestMove = move;
      }
    }

    if (validMoves.Count == 0) return -1;
    return bestMove;
  }

  public static void GoEnergy()
  {
    // List<int> path = new List<int>();
    Board.AllPaths.Clear();

    foreach (KeyValuePair<string, CaseState> item in Board.Map)
    {
      if (item.Value != CaseState.Energy) continue;
      Board.BestPathLength = 999999;
      SearchPath search = new SearchPath();

      List<MapPoint> path = new List<MapPoint>();
      MapPoint from = new MapPoint(Board.BotPosition.x, Board.BotPosition.y);

      List<MapPoint> start = new List<MapPoint>();
      start.Add(from);

      bool result = search.DoSearch(path, from, Board.StringToMapPoint(item.Key));
      // if (path.Count != 0 && path.Count < Board.BestPathLength) Board.BestPathLength = path.Count;
      if (Board.AllPaths.Count > 0)
      {
        // choose a path => the last one must be the best




      }
      else
      {
        Console.WriteLine("No path found...:(");
      }
      BotHelper.ActionNone();
    }

    // int Action = path[0];
    // Board.updateBotPosition(Action);
    // BotHelper.ActionMove((MoveDirection)Action);
  }






  // public static List<int> SearchPath(List<MapPoint> path, MapPoint start, MapPoint target)
  // {
  //   // sachant que 1 = North, 2 = West, 3 = South et 4 = East
  //   previous = start;

  //   if (start.y < target.y) // si + au nord
  //   {
  //     CaseState caseToTest = Board.Map[Board.XYtoMapKey(start.x, start.y + 1)];
  //     if (caseToTest != CaseState.Wall && caseToTest != CaseState.Malus && caseToTest != CaseState.Ennemy)
  //     {
  //       path.Add(1);
  //       start.y += 1;
  //       SearchPath(start, target, previous, path);
  //     }
  //   }

  //   else if (start.y > target.y) // si + au sud
  //   {
  //     CaseState caseToTest = Board.Map[Board.XYtoMapKey(start.x, start.y - 1)];
  //     if (caseToTest != CaseState.Wall && caseToTest != CaseState.Malus && caseToTest != CaseState.Ennemy)
  //     {
  //       path.Add(3);
  //       start.y -= 1;
  //       SearchPath(start, target, previous, path);
  //     }
  //   }

  //   else if (start.x > target.x) // si + à l' ouest
  //   {
  //     CaseState caseToTest = Board.Map[Board.XYtoMapKey(start.x - 1, start.y)];
  //     if (caseToTest != CaseState.Wall && caseToTest != CaseState.Malus && caseToTest != CaseState.Ennemy)
  //     {
  //       path.Add(2);
  //       start.x -= 1;
  //       SearchPath(start, target, previous, path);
  //     }
  //   }


  //   else if (start.x < target.x) // si + à l' est
  //   {
  //     CaseState caseToTest = Board.Map[Board.XYtoMapKey(start.x + 1, start.y)];
  //     if (caseToTest != CaseState.Wall && caseToTest != CaseState.Malus && caseToTest != CaseState.Ennemy)
  //     {
  //       path.Add(4);
  //       start.x += 1;
  //       SearchPath(start, target, previous, path);
  //     }
  //   }


  //   if (start.x != target.x && start.y != target.y) path.Clear();
  //   return path;
  // }

}