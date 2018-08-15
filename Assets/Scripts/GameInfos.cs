using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class GameInfos
{
    public static int level = 0; //floor index, start at zero
    public static bool paused = false;
    public static bool pauseMenu = false;
    public static int playerModifierCount = 6;
    public static bool hardmode;
    public static List<ModifierBase> modifiers;
    public static int life = int.MaxValue;
    public static int killCount;
    public static int bossKillCount;

    public static void clear()
    {
        level = 0;
        paused = false;
        pauseMenu = false;
        playerModifierCount = 0;
        hardmode = false;
        modifiers = null;
        life = int.MaxValue;
        killCount = 0;
        bossKillCount = 0;
    }
}
