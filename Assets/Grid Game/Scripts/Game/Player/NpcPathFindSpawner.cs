
using SNR_Event;
using UnityEngine;
using UtilSNR.Pool;

[System.Serializable]
public class NpcPathFindSpawner
{
    public enum SetPhase
    {
        SetStart,
        SetEnd
    }
    
    [SerializeField] private TestNPC npcPrefab;

    private TestNPC instance;
    private Vector3 startPos;
    private Vector3 endPos;
    
    public SetPhase CurrentPhase { get; private set; } = SetPhase.SetStart;
    
    public void Setup()
    {
        
    }

    public void Dispose()
    {
        
    }

    public void SpawnNpc(Vector3 pos)
    {
        if (!npcPrefab)
            return;
        
        CurrentPhase = SetPhase.SetEnd;
        
        if(!instance)
            instance = PoolManager.Instance.Spawn(npcPrefab, pos, Quaternion.identity);
        else 
            instance.transform.position = pos;
        
        startPos = pos;
        
        EventManager.RaiseEvent(new OnSpawnNpc()
        {
            Npc =  instance
        });
    }

    public void SetTarget(Vector3 pos)
    {
        if (!npcPrefab)
            return;
        
        CurrentPhase = SetPhase.SetStart;
        
        endPos = pos;
        instance.RequestMoveToTarget(endPos);

        EventManager.RaiseEvent(new OnSetNpcTarget()
        {
            Npc =  instance,
            TargetPos = endPos
        });
    }
}
