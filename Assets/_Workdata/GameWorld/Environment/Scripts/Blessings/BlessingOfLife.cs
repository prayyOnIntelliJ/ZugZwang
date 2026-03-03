using System;
using System.Collections;
using System.Collections.Generic;
using MyBox;
using UnityEngine;
using Random = System.Random;

public class BlessingOfLife : MonoBehaviour, IBlessing
{
    [Separator("References")]
    [SerializeField] private FieldScaleSO fieldScaleSO;
    [SerializeField] private FieldStatusChecker fieldStatusChecker;

    [Separator("Settings")]
    [SerializeField, Tag] private string tagToCheck = "Player";
    [SerializeField] private bool spawnSpecificFigures;

    [SerializeField, ConditionalField(nameof(spawnSpecificFigures))]
    private GameObject figureToSpawn;
    [SerializeField]
    private int maxBlessingCount = 2;

    [SerializeField] private int searchRadius = 4;

    [Separator("Events")]
    public Action<Collider> OnCollected;
    public static event Action<Vector3> OnFigureSpawnPositionCalculated;

    public void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(tagToCheck)) return;

        StartCoroutine(DeactivateDelayed());

        OnCollected?.Invoke(other);

        FigureInjector hitInjector = other.GetComponentInChildren<FigureInjector>();
        if (hitInjector == null) return;
        
        var factory = hitInjector.HeritageFactory;
        if (factory == null) return;
        
        Vector3 spawnPos = GetRandomAdjacentPosition();

        OnFigureSpawnPositionCalculated?.Invoke(spawnPos);
        
        if (spawnSpecificFigures && figureToSpawn) 
            factory.SpawnFigure(figureToSpawn, spawnPos);
        else 
            factory.SpawnRandomFigure(spawnPos);
    }

    private IEnumerator DeactivateDelayed()
    {
        yield return new WaitForSeconds(0.1f);
        gameObject.SetActive(false);
    }

    private Vector3 GetRandomAdjacentPosition()
    {
        List<Vector3> possiblePositions = new(8);
        Vector3 blessingPos = transform.position;

        for (int i = -searchRadius; i <= searchRadius; i++)
        {
            if (i == 0) continue;

            var currentCheckPos = blessingPos.ChangeZ(blessingPos.z - fieldScaleSO.fieldMultiplier.z * i);
            var currentCheck = fieldStatusChecker.GetFieldStatus(currentCheckPos);

            if (currentCheck.cantMoveReason == CantMoveReason.CAN_MOVE)
                possiblePositions.Add(currentCheckPos);
        }

        if (possiblePositions.Count < 1)
            return blessingPos;
        
        return possiblePositions[UnityEngine.Random.Range(0, possiblePositions.Count)];
    }

    public void OnFigureCountChanged(List<GameObject> obj)
    {
        if (obj.Count >= maxBlessingCount) 
            gameObject.SetActive(false);
    }
}