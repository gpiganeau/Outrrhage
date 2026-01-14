using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class DamageController: MonoBehaviour
{
    //Techniquement un vase devrait pouvoir marcher avec seulement les valeurs par défaut, sans besoin d'initialisation
    private int _maxHealth = 1;
	private int _currentHealth = 1;

	private bool[] _blockedZones = new bool[8]; //From front left clockwise, each zone covers 45 degrees
    private MovementController _movementController;
	[SerializeField] private bool _usesFixedForward = false;

    [HideInInspector] public UnityEvent<int, int> OnDamaged;
    [HideInInspector] public UnityEvent<int, int> OnHealed;
    [HideInInspector] public UnityEvent OnBlocked;
    [HideInInspector] public UnityEvent OnDied;

	public void Initialize(ActorSetupData data)
	{
		_maxHealth = data.maxHealth;
		_currentHealth = _maxHealth;
		_movementController = GetComponent<MovementController>();
		if(!_usesFixedForward && _movementController == null)
		{
			Debug.LogError("DamageController requires a MovementController component if not using fixed forward.");
			_usesFixedForward = true;
        }
    }

	public void Initialize(int maxHealth)
	{
		_maxHealth = maxHealth;
		_currentHealth = _maxHealth;
    }

    public void UpdateBlockedZones(bool[] newBlockedZones)
	{
		_blockedZones = newBlockedZones;
    }

	public void Damage(int amount, Vector3 origin)
	{
		bool blocked = IsBlockedFromDirection(origin);
		if (blocked)
			OnBlocked?.Invoke();
        else
		{
			_currentHealth -= amount;
            OnDamaged?.Invoke(_currentHealth, _maxHealth);
			if (_currentHealth <= 0)
				OnDied?.Invoke();
        }
    }

	public void Heal(int amount)
	{
		_currentHealth += amount;
		if (_currentHealth > _maxHealth)
			_currentHealth = _maxHealth;
		OnHealed?.Invoke(_currentHealth, _maxHealth);
    }


    private bool IsBlockedFromDirection(Vector3 origin)
	{
        if (origin != Vector3.zero)
        {
            Vector3 forward = _usesFixedForward ? Vector3.forward : _movementController.GetFacingDirection();
            Vector3 left = Quaternion.Euler(0, -90f, 0) * forward;
            Vector3 toOrigin = (origin - transform.position).normalized;
            float angle = Vector3.SignedAngle(left, toOrigin, Vector3.up);
            if (angle < 0) angle += 360f;
            int zone = Mathf.FloorToInt(angle / 45f);
            return _blockedZones[zone];
        }
		return false;
    }
}
