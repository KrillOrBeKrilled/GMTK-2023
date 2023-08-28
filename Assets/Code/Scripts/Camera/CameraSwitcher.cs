using UnityEngine;
using Yarn.Unity;

//*******************************************************************************************
// CameraSwitcher
//*******************************************************************************************
/// <summary>
/// Handles the transitions between the various cameras used to focus on parts of the
/// levels and its actors.
/// </summary>
/// <remarks> Manages and exposes all references to the cameras in a level. </remarks>
public class CameraSwitcher : MonoBehaviour
{
    /// <summary>
    /// The camera focused on the player.
    /// </summary>
    public GameObject PlayerCamera;
    
    /// <summary>
    /// The camera focused on the starting position of the level.
    /// </summary>
    public GameObject StartCamera;
    
    /// <summary>
    /// The camera focused on the end position of the level.
    /// </summary>
    public GameObject EndCamera;

    /// <summary>
    /// Disables every camera managed by this class.
    /// </summary>
    private void DisableAll()
    {
        this.PlayerCamera.SetActive(false);
        StartCamera.SetActive(false);
        EndCamera.SetActive(false);
    }

    /// <summary>
    /// Enables only the <see cref="PlayerCamera"/> to transition the screen to focus on the player.
    /// </summary>
    /// <remarks> Can be accessed as a YarnCommand. </remarks>
    [YarnCommand("show_player")]
    public void ShowPlayer()
    {
        DisableAll();
        this.PlayerCamera.SetActive(true);
    }

    /// <summary>
    /// Enables only the <see cref="StartCamera"/> to transition the screen to focus on the beginning of the level.
    /// </summary>
    /// <remarks> Can be accessed as a YarnCommand. </remarks>
    [YarnCommand("show_start")]
    public void ShowStart()
    {
        DisableAll();
        StartCamera.SetActive(true);
    }

    /// <summary>
    /// Enables only the <see cref="EndCamera"/> to transition the screen to focus on the level goal.
    /// </summary>
    /// <remarks> Can be accessed as a YarnCommand. </remarks>
    [YarnCommand("show_end")]
    public void ShowEnd()
    {
        DisableAll();
        EndCamera.SetActive(true);
    }
}
