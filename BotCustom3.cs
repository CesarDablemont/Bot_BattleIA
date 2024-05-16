using System;
using BattleIA;

public class Custom3
{
  // Pour détecter si c'est le tout premier tour du jeu
  bool IsFirstTime;

  // mémorisation du niveau du bouclier de protection
  UInt16 CurrentShieldLevel;
  // variable qui permet de savoir si le bot a été touché ou non
  bool HasBeenHit;

  uint TimeLastScan; // combien de temps depuis le dernier scan ?

  uint Turn;
  uint BotEnergy;



  public void DoInit()
  {
    IsFirstTime = true;
    CurrentShieldLevel = 0;
    HasBeenHit = false;
    TimeLastScan = 0;
    Turn = 0;
    BotEnergy = 200; // voir settings.json
  }

  // ****************************************************************************************************
  /// Réception de la mise à jour des informations du bot
  public void StatusReport(UInt16 turn, UInt16 energy, UInt16 shieldLevel, UInt16 cloakLevel)
  {
    // Si le niveau du bouclier a baissé, c'est que l'on a reçu un coup
    if (CurrentShieldLevel != shieldLevel) HasBeenHit = true;
    CurrentShieldLevel = shieldLevel;

    Turn = turn;
    BotEnergy = energy;
  }


  // ****************************************************************************************************
  /// On doit effectuer une action
  public byte[] GetAction()
  {
    // Console.ReadKey(true);


    if (TimeLastScan == 0)
    {
      // sachant que 1 = North, 2 = West, 3 = South et 4 = East
      for (int i = Board.TLcorner.x; i < Board.BotPosition.x; i++)
      {
        if (Board.Map.ContainsKey(Board.XYtoMapKey(i, Board.BotPosition.y)))
          if (Board.Map[Board.XYtoMapKey(i, Board.BotPosition.y)] == CaseState.Ennemy)
            return BotHelper.ActionShoot((MoveDirection)2);
      }

      for (int i = Board.BotPosition.x + 1; i <= Board.BRcorner.x; i++)
      {
        if (Board.Map.ContainsKey(Board.XYtoMapKey(i, Board.BotPosition.y)))
          if (Board.Map[Board.XYtoMapKey(i, Board.BotPosition.y)] == CaseState.Ennemy)
            return BotHelper.ActionShoot((MoveDirection)4);
      }

      for (int i = Board.BRcorner.y; i > Board.BotPosition.y; i++)
      {
        if (Board.Map.ContainsKey(Board.XYtoMapKey(Board.BotPosition.x, i)))
          if (Board.Map[Board.XYtoMapKey(Board.BotPosition.x, i)] == CaseState.Ennemy)
            return BotHelper.ActionShoot((MoveDirection)1);
      }

      for (int i = Board.BotPosition.y + 1; i >= Board.TLcorner.y; i++)
      {
        if (Board.Map.ContainsKey(Board.XYtoMapKey(Board.BotPosition.x, i)))
          if (Board.Map[Board.XYtoMapKey(Board.BotPosition.x, i)] == CaseState.Ennemy)
            return BotHelper.ActionShoot((MoveDirection)3);
      }
    }

    if (HasBeenHit == true && BotEnergy >= 100)
      return BotHelper.ActionShield((ushort)(BotEnergy / 10));

    if (BotEnergy < 50 && CurrentShieldLevel > 0) // je recupere le shield si j'en ai
      return BotHelper.ActionShield(0);




    // RIP PathFinding
    // MoveBot.GoEnergy();

    int Move = MoveBot.GoCloseEnergy();

    // On déplace le bot au hazard
    if (Move == -1) Move = MoveBot.GoBestHeatMap();
    if (Move == -1) Move = MoveBot.RandomMoveSafe();
    if (Move == -1) Move = MoveBot.TryEscape();
    if (Move == -1) return BotHelper.ActionNone();

    Board.updateBotPosition(Move);
    return BotHelper.ActionMove((MoveDirection)Move);


    // Voici d'autres exemples d'actions possibles
    // -------------------------------------------

    // Si on ne veut rien faire, passer son tour
    // return BotHelper.ActionNone();

    // Déplacement du bot au nord
    // return BotHelper.ActionMove(MoveDirection.North);

    // Activation d'un bouclier de protection de niveau 10 (peut encaisser 10 points de dégats)
    // return BotHelper.ActionShield(10);

    // Activation d'un voile d'invisibilité sur une surface de 15
    // return BotHelper.ActionCloak(15);

    // Tir dans la direction sud
    // return BotHelper.ActionShoot(MoveDirection.South);

  }


  // ****************************************************************************************************
  /// On nous demande la distance de scan que l'on veut effectuer
  public byte GetScanSurface()
  {
    if (IsFirstTime)
    {
      IsFirstTime = false;
      return 10;
    }

    TimeLastScan++;
    if (TimeLastScan >= 5 && Turn > 10)
    {
      TimeLastScan = 0;
      return 7;
    }

    return 0;
  }


  // ****************************************************************************************************
  /// Résultat du scan
  public void AreaInformation(byte distance, byte[] informations)
  {
    int Radius = (distance - 1) / 2;
    Console.WriteLine($"bot position: ({Board.BotPosition.x}, {Board.BotPosition.y})");
    Console.WriteLine($"Area: {distance}");

    int Index = 0;
    for (int y = 0; y < distance; y++)
    {
      for (int x = 0; x < distance; x++)
      {
        Board.UpdateMap(x - Radius + Board.BotPosition.x, Radius - y + Board.BotPosition.y, (CaseState)informations[Index++]);
      }
    }

    // Board.DisplayMap();
    Board.UpdateHeatMap();
    // Board.DisplayHeatMap();
  }



}



