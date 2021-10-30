using System.Collections;
using UnityEngine;

public class Trap : MonoBehaviour, IPoolable
{
    public MeshRenderer MeshRenderer;
    public Rigidbody Rb;
    public BoxCollider Collider;

    private float _counter;
    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("Player"))
        {
            StartCoroutine(ActivateTrap());
        }
    }

    private IEnumerator ActivateTrap()
    {
        while (true)
        {
            _counter += Time.deltaTime;
            MeshRenderer.material.color = Color.Lerp(Color.white, Color.black, _counter*1.5f);
            if (_counter > 0.15f)
            {
                Collider.isTrigger = true;
                Collider.attachedRigidbody.isKinematic = false;
            }

            if (MeshRenderer.material.color == Color.black)
            {
                _counter = 0;
                yield break;
            }
            yield return null;
        }
    }
}
