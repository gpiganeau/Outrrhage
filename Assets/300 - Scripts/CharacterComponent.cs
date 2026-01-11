using UnityEngine;
using System.Collections;

public class CharacterComponent : MonoBehaviour
{
	[SerializeField] private CharacterSettings characterSettings;
    private CharacterData characterData;
	private AttackerController attackerController;

	[SerializeField] private SkillData debugSkillData;

    // Use this for initialization
    void Start()
	{
		characterData = new CharacterData(characterSettings);
		characterData.skillData.Add(debugSkillData);

        attackerController = GetComponent<AttackerController>();
		attackerController.Initialize(characterData);
    }

	// Update is called once per frame
	void Update()
	{

	}
}
