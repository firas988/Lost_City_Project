using System;
using UnityEngine;

public static class KillEnemyHandler
{
    private static Action<string> onKilledEnemy;
   
    public static void KilledEnemy(string enemyTag)
    {
        onKilledEnemy?.Invoke(enemyTag);
    }

    public static void Subscribe(Action<string> callback)
    {
        onKilledEnemy += callback;
    }

    public static void Unsubscribe(Action<string> callback)
    {
        onKilledEnemy -= callback;
    }
}
