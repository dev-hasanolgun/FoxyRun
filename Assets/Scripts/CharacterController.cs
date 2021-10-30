using UnityEngine;

public class CharacterController : MonoBehaviour
{
    public Runner Runner;
    
    private Rigidbody _rb;

    public void MoveCharacter()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.position += Vector3.forward * (Time.deltaTime * 5f);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0,15,0), Time.deltaTime * 3f);
        }
        else
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0,90,0), Time.deltaTime * 3f);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.position += Vector3.back * (Time.deltaTime * 5f);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0,165,0), Time.deltaTime * 3f);
        }
        else
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0,90,0), Time.deltaTime * 3f);
        }
        
        var worldWidth = 14f;
        var ratioScreenToWorld = 2.0f;
        var screenWidth = (float) Screen.width;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Moved)
            {
                var pos = transform.position;
                float horizontal = -touch.deltaPosition.x;
                var coord = pos.z + horizontal * (worldWidth / screenWidth) * ratioScreenToWorld;
                var coordClamped = Mathf.Clamp(coord, -4, 3.5f);
                transform.position = new Vector3(pos.x, pos.y, coordClamped);
                if (horizontal > 0)
                {
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0,15,0), Time.deltaTime * 3f);
                }
                if (horizontal < 0)
                {
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0,165,0), Time.deltaTime * 3f);
                }
            }
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0,90,0), Time.deltaTime * 3f);
        }
    }
    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }
}
