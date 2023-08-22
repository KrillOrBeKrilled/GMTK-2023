using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Analytics;
using Unity.Services.Core;
public class UGS_Analytics : Singleton<UGS_Analytics>
{
    private async void Start()
   {
       try{
            await UnityServices.InitializeAsync();
       } catch (ConsentCheckException e) {
            Debug.Log(e.ToString());
       }

       // We want analytics to persist throughout the lifecycle of the entire game so we can trigger its functions
       // But this means testing must be done by starting with the MainMenu scene
       DontDestroyOnLoad(transform.gameObject);

       // TODO: Adjust this to ask for consent in the future...
       AnalyticsService.Instance.StartDataCollection();
   }

    public static void PlayerDeathByHeroCustomEvent(int coinBalance, float xPos, float yPos, float zPos)
    {
        var eventParameters = new Dictionary<string, object>
        {
            { "coinBalance", coinBalance },
            { "xPos", xPos },
            { "yPos", yPos },
            { "zPos", zPos }
        };

        // The ‘touchHeroDeath’ event will get cached locally and sent during the next scheduled upload, within 1 minute
        try {
            AnalyticsService.Instance.CustomData("touchHeroDeath", eventParameters);
        } catch (ServicesInitializationException e) {
            Debug.Log("<color=red>AnalyticsService Initialization Failed</color>");
        }

        // You can call Events.Flush() to send the event immediately
        // AnalyticsService.Instance.Flush();
    }

    public static void PlayerDeathByBoundaryCustomEvent(int coinBalance, float xPos, float yPos, float zPos)
    {
        var eventParameters = new Dictionary<string, object>
        {
            { "coinBalance", coinBalance },
            { "xPos", xPos },
            { "yPos", yPos },
            { "zPos", zPos }
        };

        try {
            AnalyticsService.Instance.CustomData("touchBoundaryDeath", eventParameters);
        } catch (ServicesInitializationException e) {
            Debug.Log("<color=red>AnalyticsService Initialization Failed</color>");
        }
    }

    public static void HeroDiedCustomEvent(int numberLivesLeft, float xPos, float yPos, float zPos)
    {
        var eventParameters = new Dictionary<string, object>
        {
            { "livesLeft", numberLivesLeft},
            { "xPos", xPos },
            { "yPos", yPos },
            { "zPos", zPos }
        };

        try {
            AnalyticsService.Instance.CustomData("heroDied", eventParameters);
        } catch (ServicesInitializationException e) {
            Debug.Log("<color=red>AnalyticsService Initialization Failed</color>");
        }
    }

    public static void HeroIsStuckCustomEvent(float xPos, float yPos, float zPos)
    {
        var eventParameters = new Dictionary<string, object>
        {
            { "xPos", xPos },
            { "yPos", yPos },
            { "zPos", zPos }
        };

        try {
            AnalyticsService.Instance.CustomData("heroIsStuck", eventParameters);
        } catch (ServicesInitializationException e) {
            Debug.Log("<color=red>AnalyticsService Initialization Failed</color>");
        }
    }

    public static void DeployTrapCustomEvent(int trapType)
    {
        var trapName = GenerateTrapName(trapType);

        var eventParameters = new Dictionary<string, object>
        {
            { "trapType", trapName }
        };

        try {
            AnalyticsService.Instance.CustomData("deployTrap", eventParameters);
        } catch (ServicesInitializationException e) {
            Debug.Log("<color=red>AnalyticsService Initialization Failed</color>");
        }
    }

    public static void SwitchTrapCustomEvent(int trapType, bool isAffordable)
    {
        var trapName = GenerateTrapName(trapType);

        var eventParameters = new Dictionary<string, object>
        {
            { "trapType", trapName },
            { "canAfford", isAffordable }
        };

        try {
            AnalyticsService.Instance.CustomData("switchTrap", eventParameters);
        } catch (ServicesInitializationException e) {
            Debug.Log("<color=red>AnalyticsService Initialization Failed</color>");
        }
    }

    private static string GenerateTrapName(int trapType)
    {
        switch (trapType)
        {
            case 0:
                return "Spike Trap";
            case 1:
                return "Swinging Axe Trap";
            case 2:
                return "Acid Pit Trap";
        }

        return "";
    }
}
