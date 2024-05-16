using System;
using BattleIA;

public class SearchPath
{
  int[] testInProgressX = { 1, 0, -1, 0 };
  int[] testInProgressY = { 0, 1, 0, -1 };
  int moveX, moveY;

  public bool DoSearch(List<MapPoint> path, MapPoint origin, MapPoint to)
  {
    Board.DisplayMap();
    Board.ShowAllPaths();

    if (path.Count > Board.BestPathLength)
      return false;

    var pathClone = FastDeepCloner.DeepCloner.Clone(path);
    for (int test = 0; test < 4; test++)
    {
      path = FastDeepCloner.DeepCloner.Clone(pathClone);
      moveX = testInProgressX[test];
      moveY = testInProgressY[test];
      var from = FastDeepCloner.DeepCloner.Clone(origin);
      //Console.Write($"?{moveX}/{moveY}");

      if (from.x + moveX == to.x && from.y + moveY == to.y)
      {
        //Console.WriteLine("Yes! ***");
        // Arrivée !!!
        MapPoint found = new MapPoint(from.x + moveX, from.y + moveY);
        path.Add(found);
        if (path.Count < Board.BestPathLength)
        {
          Board.BestPathLength = path.Count;
          Board.AllPaths.Add(path);
          return true;
        }
        return false;
      }

      switch (Board.Map[Board.XYtoMapKey(from.x + moveX, from.y + moveY)])
      {
        case CaseState.Empty:
        case CaseState.Energy:
        case CaseState.Bonus:
          CaseState saveCase = Board.Map[Board.XYtoMapKey(from.x + moveX, from.y + moveY)];
          // on effectue le déplacement virtuel
          path.Add(new MapPoint(from.x + moveX, from.y + moveY));
          Board.Map[Board.XYtoMapKey(from.x + moveX, from.y + moveY)] = CaseState.Virtual;
          bool found = DoSearch(path, new MapPoint(from.x + moveX, from.y + moveY), to);
          path.RemoveAt(path.Count - 1);
          Board.Map[Board.XYtoMapKey(from.x + moveX, from.y + moveY)] = saveCase;
          if (!found)
          {
            Console.WriteLine("No found");
            // cette direction n'est pas bonne, tester les autres directions
          }
          else
          {
            int i = Board.AllPaths.Count();
            Console.Write($"Path found {i}: ");
            Board.ShowPath(Board.AllPaths[i - 1]);
          }
          break;
        default:
          // on ne fait pas ce déplacement !
          // cette direction n'est pas bonne, tester les autres directions
          Console.WriteLine("Can't go on this position");
          break;
      }
    }
    //Console.WriteLine(" return");
    return false;
  }
}