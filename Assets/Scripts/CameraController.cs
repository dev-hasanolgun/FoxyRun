using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Runner Runner;
    public Vector3 OffsetPos;
    public Quaternion CamRotation;
    
    private float _angle = 0;

    public void FocusRunner()
    {
        var rotation = transform.rotation;
        
        if (Runner.transform.position.y < -1f)
        {
            var rot = Quaternion.LookRotation(Runner.transform.position - transform.position);
            transform.rotation = Quaternion.Slerp(rotation, rot, Time.deltaTime * 3f);
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position,Runner.transform.position + OffsetPos,Time.deltaTime * 3f);
            transform.rotation = Quaternion.Slerp(rotation, CamRotation, Time.deltaTime * 3f);
        }
    }

    public void VictoryCameraRotation()
    {
        if (Mathf.Abs(transform.eulerAngles.y + Runner.transform.eulerAngles.y - 360f) > 1f)
        {
            _angle += Time.deltaTime;
            var pos = transform.position;
            var runnerPos = Runner.transform.position;
            var dir = runnerPos - pos;
            var rot = Quaternion.LookRotation(dir);
            transform.RotateAround(runnerPos, Vector3.up, _angle);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 3f);
            transform.position += dir* (Time.deltaTime / 1.5f);
        }
        else
        {
            _angle = 0;
        }
    }
}
