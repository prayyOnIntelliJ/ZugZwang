using System;
using System.Collections.Generic;
using MyBox;
using UnityEngine;
using UnityEngine.Serialization;

public class BlessingOfIce : MonoBehaviour, IBlessing
{
    [Separator("Field References")] [SerializeField]
    private GameObject fieldObject;

    [FormerlySerializedAs("fieldLayer")] [SerializeField] private LayerMask rowsLayer;
    [SerializeField, Tag] private string playerTag = "Player";
    [SerializeField, Tag] private string newFieldTag = "Field";

    [Separator("Field Settings")] [SerializeField]
    private int rowsToOverwrite;

    [SerializeField] private float obsidianHeightOffset = 0.6f;

    public static event Action<Vector3> OnPickUp;


    public void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;
        
        foreach (GameObject row in GetFollowingRows())
        {
            foreach (LavaField lavaField in row.GetComponentsInChildren<LavaField>())
            {
                lavaField.transform.tag = newFieldTag;
                Instantiate(fieldObject, lavaField.transform.position.AddY(-obsidianHeightOffset),
                    lavaField.transform.rotation, lavaField.transform);
            }
        }

        OnPickUp?.Invoke(transform.position); // Yona

        gameObject.SetActive(false);
    }

    private List<GameObject> GetFollowingRows()
    {
        Vector3 currentPosition = transform.position;
        currentPosition.y += 3;
        List<GameObject> rows = new List<GameObject>();
        for (int i = 0; i < rowsToOverwrite; i++)
        {
            if (Physics.Raycast(currentPosition, Vector3.down, out var hit, 100f, rowsLayer))
            {
                GameObject row = hit.collider.gameObject;
                rows.Add(row);
            }

            currentPosition.x += 3;
        }

        return rows;
    }
}