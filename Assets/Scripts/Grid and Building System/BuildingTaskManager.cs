using System.Collections.Generic;
using UnityEngine;

public class BuildingTaskManager : MonoBehaviour
{
    public static BuildingTaskManager Instance { get; private set; }

    private Queue<ConstructionTask> angelQueue = new Queue<ConstructionTask>();
    private Queue<ConstructionTask> devilQueue = new Queue<ConstructionTask>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    public void EnqueueTask(Construction construction)
    {
        var task = new ConstructionTask(construction);

        switch (construction.GetBuilderType())
        {
            case BuilderType.Angel:
                angelQueue.Enqueue(task);
                break;

            case BuilderType.Devil:
                devilQueue.Enqueue(task);
                break;
        }
    }

    public Construction DequeueNextTask(Agents.UnitType unitType)
    {
        Queue<ConstructionTask> queue = unitType == Agents.UnitType.Angel ? angelQueue : devilQueue;

        while (queue.Count > 0) 
        {
            ConstructionTask task = queue.Dequeue();

            if (!task.isAssigned && task.construction != null) 
            {
                task.isAssigned = true;
                return task.construction;
            }
        }

        return null;
    }
}
