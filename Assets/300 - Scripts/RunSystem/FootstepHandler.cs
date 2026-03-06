using UnityEngine;

public class FootstepHandler : MonoBehaviour
{
    [SerializeField] private AudioClip[] _footstepClips;
    [SerializeField] private float _volume = 0.4f;

    public void OnFootstep()
    {
        if (_footstepClips.Length == 0) return;
        AudioManager.Instance.PlayClipAtPoint(_footstepClips.Random(), transform.position, SoundGroup.SFX);
    }
}