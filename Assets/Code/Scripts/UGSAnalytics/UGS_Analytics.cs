using System.Collections.Generic;
using KrillOrBeKrilled.Managers;
using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;

//*******************************************************************************************
// UGSAnalytics
//*******************************************************************************************
namespace KrillOrBeKrilled.UGSAnalytics {
    /// <summary>
    /// Manages all initialization and data collection for <see cref="AnalyticsService"/>
    /// to collect gameplay analytics data that can be accessed through the UGS Analytics
    /// dashboard.
    /// </summary>
    /// <remarks> Direct access to data collection methods is handled by the
    /// GameManager. </remarks>
    public class UGS_Analytics : Singleton<UGS_Analytics> {
        private async void Start() {
            try {
                await UnityServices.InitializeAsync();
            } catch (ConsentCheckException e) {
                Debug.Log(e.ToString());
            }

            // We want analytics to persist throughout the lifecycle of the entire game so we can trigger its functions
            // But this means testing must be done by starting with the MainMenu scene
            DontDestroyOnLoad(this.transform.gameObject);

            // TODO: Adjust this to ask for consent in the future...
            AnalyticsService.Instance.StartDataCollection();
        }

        /// <summary>
        /// Packages player death by hero position and coin data to send to the UGS Analytics server.
        /// </summary>
        /// <param name="coinBalance"> The number of coins in the player's position at the time of death. </param>
        /// <param name="pos"> The player's current position. </param>
        public static void PlayerDeathByHeroCustomEvent(int coinBalance, Vector3 pos) {
            var eventParameters = new Dictionary<string, object> {
                { "coinBalance", coinBalance },
                { "xPos", pos.x },
                { "yPos", pos.y },
                { "zPos", pos.z }
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

        /// <summary>
        /// Packages player death by boundary position and coin data to send to the UGS Analytics server.
        /// </summary>
        /// <param name="coinBalance"> The number of coins in the player's position at the time of death. </param>
        /// <param name="xPos"> The player's current position along the x-axis. </param>
        /// <param name="yPos"> The player's current position along the y-axis. </param>
        /// <param name="zPos"> The player's current position along the z-axis. </param>
        public static void PlayerDeathByBoundaryCustomEvent(int coinBalance, float xPos, float yPos, float zPos) {
            var eventParameters = new Dictionary<string, object> {
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

        /// <summary>
        /// Packages hero death position data to send to the UGS Analytics server.
        /// </summary>
        /// <param name="numberLivesLeft"> The number of remaining lives the hero has at the time of death. </param>
        /// <param name="xPos"> The hero's current position along the x-axis. </param>
        /// <param name="yPos"> The hero's current position along the y-axis. </param>
        /// <param name="zPos"> The hero's current position along the z-axis. </param>
        public static void HeroDiedCustomEvent(float xPos, float yPos, float zPos) {
            var eventParameters = new Dictionary<string, object> {
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

        /// <summary>
        /// Packages hero stuck position data to send to the UGS Analytics server.
        /// </summary>
        /// <param name="xPos"> The hero's current position along the x-axis. </param>
        /// <param name="yPos"> The hero's current position along the y-axis. </param>
        /// <param name="zPos"> The hero's current position along the z-axis. </param>
        public static void HeroIsStuckCustomEvent(float xPos, float yPos, float zPos) {
            var eventParameters = new Dictionary<string, object> {
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

        /// <summary>
        /// Packages trap deployment type data to send to the UGS Analytics server.
        /// </summary>
        /// <param name="trapType"> The index of the trap that has been deployed. </param>
        public static void DeployTrapCustomEvent(string trapName) {
            var eventParameters = new Dictionary<string, object> {
                { "trapType", trapName }
            };

            try {
                AnalyticsService.Instance.CustomData("deployTrap", eventParameters);
            } catch (ServicesInitializationException e) {
                Debug.Log("<color=red>AnalyticsService Initialization Failed</color>");
            }
        }

        /// <summary>
        /// Packages trap selection type and cost data to send to the UGS Analytics server.
        /// </summary>
        /// <param name="trapType"> The index of the trap that has been selected. </param>
        /// <param name="isAffordable"> If the selected trap cost can be afforded with the current coin balance. </param>
        public static void SwitchTrapCustomEvent(string trapName, bool isAffordable) {
            var eventParameters = new Dictionary<string, object> {
                { "trapType", trapName },
                { "canAfford", isAffordable }
            };

            try {
                AnalyticsService.Instance.CustomData("switchTrap", eventParameters);
            } catch (ServicesInitializationException e) {
                Debug.Log("<color=red>AnalyticsService Initialization Failed</color>");
            }
        }
    }
}
