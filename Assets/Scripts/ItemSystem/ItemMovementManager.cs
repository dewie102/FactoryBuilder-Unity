using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.EntitySystem.Interfaces;
using UnityEngine;

public class ItemMovementManager : MonoBehaviour
{
    public static ItemMovementManager Instance { get; private set; }
    public static event Action<ItemTransfer> ItemTransferred;

    private Queue<ItemTransfer> _movementQueue = new();
    private HashSet<Vector3Int> _queuedTargets = new();

    public void Awake()
    {
        Instance = this;
    }

    public void ClearQueue()
    {
        _movementQueue.Clear();
        _queuedTargets.Clear();
    }

    public void QueueTransfer(ItemTransfer transfer)
    {
        if (_queuedTargets.Contains(transfer.to))
        {
            Debug.Log($"Transfer to {transfer.to} already queued - skipping.");
        }
        
        _movementQueue.Enqueue(transfer);
        _queuedTargets.Add(transfer.to);
        Debug.Log($"Consumer at {transfer.to} queued consumtion of item from {transfer.from}");
    }

    public void ApplyTransfers()
    {
        while (_movementQueue.Count > 0)
        {
            ItemTransfer transfer = _movementQueue.Dequeue();
            if (transfer.toEntity is IItemConsumer consumer &&
                transfer.fromEntity is IItemProducer producer &&
                consumer.TryConsumeItem(transfer.item))
            {
                producer.RemoveItem();
                ItemTransferred?.Invoke(transfer);
                Debug.Log($"Consumer at {transfer.to} consumed item from {transfer.from}");
            }
            else
            {
                Debug.LogWarning($"Transfer from {transfer.from} to {transfer.to} failed during Apply.");
            }
        }
    }
}