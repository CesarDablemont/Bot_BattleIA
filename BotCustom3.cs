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

    // SHIELD

    // if (HasBeenHit == true && BotEnergy >= 100)
    // {
    //   return BotHelper.ActionShield(10);
    //   HasBeenHit = false;
    // }
    // else

    if (BotEnergy > 1000 && CurrentShieldLevel < 100)
      return BotHelper.ActionShield(200);

    else if (BotEnergy > 400 && CurrentShieldLevel < 50)
      return BotHelper.ActionShield(100);

    else if (BotEnergy < 50 && CurrentShieldLevel > 0) // je recupere le shield si j'en ai
      return BotHelper.ActionShield(0);


    // ATTACK
    int Direction = Board.CheckAndShootEnemy();

    if (TimeLastScan <= 1 && BotEnergy > 50 && Direction != -1)
      return BotHelper.ActionShoot((MoveDirection)Direction);

    else if (BotEnergy > 5000 && Direction != -1)
      return BotHelper.ActionShoot((MoveDirection)Direction);




    // RIP PathFinding
    // MoveBot.GoEnergy();

    int Move = MoveBot.GoCloseEnergy(); // On se dirige vers l'énergie la plus proche qui a le plus haut score dans la heatmap

    // On déplace le bot au hasard
    if (Move == -1) Move = MoveBot.GoBestHeatMap(); // Sinon, on se déplace vers la case avec le score le plus élevé dans la heatmap
    // if (Move == -1) Move = MoveBot.RandomMoveSafe();
    if (Move == -1) Move = MoveBot.TryEscape(); // Si le bot est dans une impasse
    if (Move == -1) return BotHelper.ActionNone(); // Alors le bot est coincé entre 4 murs :)


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

    Display.DisplayMap();
    Board.UpdateHeatMap();
    Display.DisplayHeatMap();
  }
}

