using UnityEngine;

public class PlayButton : MonoBehaviour
{
    public void PlayLevel()
    {
        EventManager.TriggerEvent("OnPlayLevel", null);
    }
}
