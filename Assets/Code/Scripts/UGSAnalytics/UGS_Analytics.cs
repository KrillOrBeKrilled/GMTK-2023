using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Analytics;
using Unity.Services.Core;
using Unity.Services.Core.Analytics;
public class UGS_Analytics : MonoBehaviour
{
    private async void Start()
   {
       try 
       {
            await UnityServices.InitializeAsync();
       } catch (ConsentCheckException e) {
            Debug.Log(e.ToString());
       }

       // We want analytics to persist throughout the lifecycle of the entire game so we can trigger its functions
       DontDestroyOnLoad(transform.gameObject);
       
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
        AnalyticsService.Instance.CustomData("touchHeroDeath", eventParameters);

        // You can call Events.Flush() to send the event immediately
        AnalyticsService.Instance.Flush();
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

        // The ‘touchHeroDeath’ event will get cached locally and sent during the next scheduled upload, within 1 minute
        AnalyticsService.Instance.CustomData("touchBoundaryDeath", eventParameters);

        // You can call Events.Flush() to send the event immediately
        AnalyticsService.Instance.Flush();
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
        
        AnalyticsService.Instance.CustomData("heroDied", eventParameters);
        AnalyticsService.Instance.Flush();
    }

    public static void DeployTrapCustomEvent(int type)
    {
        var trapName = "";
        switch (type)
        {
            case 0:
                trapName = "Spike Trap";
                break;
            case 1:
                trapName = "Swinging Axe Trap";
                break;
            case 2:
                trapName = "Acid Pit Trap";
                break;
        }
        
        var eventParameters = new Dictionary<string, object>
        {
            { "trapType", trapName }
        };

        // The ‘touchHeroDeath’ event will get cached locally and sent during the next scheduled upload, within 1 minute
        AnalyticsService.Instance.CustomData("deployTrap", eventParameters);

        // You can call Events.Flush() to send the event immediately
        AnalyticsService.Instance.Flush();
    }
}