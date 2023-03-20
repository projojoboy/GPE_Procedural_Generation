using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    private float h, v;
    [SerializeField]
    private float speed = 5;

    private WorldManager wm = null;

    private void Update()
    {
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");
    }

    private void FixedUpdate()
    {
        transform.Translate(speed * Time.deltaTime * new Vector3(h, 0, v), Space.World);
    }
}
