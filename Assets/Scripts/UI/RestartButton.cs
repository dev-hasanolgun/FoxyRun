using UnityEngine;

public class RestartButton : MonoBehaviour
{
    public void RestartLevel()
    {
        EventManager.TriggerEvent("OnRestartLevel", null);
    }
}
