using System.Collections;
using UnityEngine;

public class SkillStrategy : MonoBehaviour
{
    private string debugName;
    public virtual void Initialize(SkillData skillData)
    {
        debugName = skillData.name;
    }

    public virtual void Call()
    {
        Debug.Log($"Skill {debugName} used");
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
