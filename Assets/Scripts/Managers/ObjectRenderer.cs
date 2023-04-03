using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectRenderer : MonoBehaviour
{
    private GameObject player = null;
    private Chunk currentChunk = null;
    private Chunk previousChunk = null;

    private WorldManager wm = null;

    [SerializeField]
    private int renderDistance = 1;
    private int chunkSize = 0;

    private bool newChunk = true;

    private void Awake()
    {
        player = FindObjectOfType<CharacterController>().gameObject;
        wm = GetComponent<WorldManager>();
    }

    private void Start()
    {
        chunkSize = wm.GetChunkSize();
    }

    private void FixedUpdate()
    {
        FindCurrentChunk();
        RenderCurrentChunk();
    }

    private void FindCurrentChunk()
    {
        Chunk temp = wm.GetChunks()[(int)player.transform.position.x/ chunkSize, (int)player.transform.position.z / chunkSize];

        if (currentChunk == temp) return;
        else
        {
            previousChunk = currentChunk;
            currentChunk = temp;
            newChunk = true;
        }
    }

    private void RenderCurrentChunk()
    {
        if (!newChunk)
            return; 

        previousChunk?.RenderResources(false, renderDistance);
        currentChunk?.RenderResources(true, renderDistance);

        newChunk = false;
    }

    public Chunk GetCurrentChunk() { return currentChunk; }
}
