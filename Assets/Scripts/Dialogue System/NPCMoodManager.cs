using System.Collections.Generic;
using UnityEngine;

public enum MoodType { Normal, Happy, Angry }

public class NPCMoodManager : MonoBehaviour
{
    public static NPCMoodManager Instance;
    private Dictionary<string, MoodType> npcMoods = new Dictionary<string, MoodType>();

    private readonly System.Collections.Generic.Dictionary<string, MoodType> forcedPreviousMoods = new System.Collections.Generic.Dictionary<string, MoodType>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    public void SetMood(string npcId, MoodType mood)
    {
        npcMoods[npcId] = mood;
    }

    public MoodType GetMood(string npcId)
    {
        if (npcMoods.TryGetValue(npcId, out var mood)) return mood;
        return MoodType.Normal; // default
    }
    public void ResetAllMoods()
    {
        npcMoods.Clear();
    }

    public void SetForcedPreviousMood(string npcId, MoodType prev)
    {
        forcedPreviousMoods[npcId] = prev;
    }

    public bool TryGetForcedPreviousMood(string npcId, out MoodType prev)
    {
        return forcedPreviousMoods.TryGetValue(npcId, out prev);
    }

    public bool HasForcedPreviousMood(string npcId)
    {
        return forcedPreviousMoods.ContainsKey(npcId);
    }

    public void ClearForcedPreviousMood(string npcId)
    {
        forcedPreviousMoods.Remove(npcId);
    }

}
