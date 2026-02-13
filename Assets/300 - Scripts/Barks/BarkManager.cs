using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public enum BarkPriority { VeryLow, Low, Normal, High, VeryHigh, Absolute }

public class BarkManager : MonoBehaviour
{
    // Todo : Subtiltes System 
    // Todo : Fade In/out ?

    public static BarkManager I;

    AudioSource _source;
    Bark _currentBark;

    Tween _currentBarkTween;

    void Awake()
    {
        if (I == null) I = this;
        else Destroy(this.gameObject);

        _source = GetComponent<AudioSource>();
    }

    public bool TryPlay(Bark newBark)
    {
        
        if (_currentBark == null)
        {
            Play(newBark);
            return true;
        }

        if (newBark.Priority > _currentBark.Priority || newBark.Priority == BarkPriority.Absolute)
        {
            _currentBarkTween?.Kill();
            _source.Stop();
            Play(newBark);
            return true;
        }

        return false;
    }

    private void Play(Bark bark)
    {
        _currentBark =  bark;
        _source.clip = _currentBark.Clip;
        _source.Play();
        Logger.Narration(_currentBark.Text);

        // Auto-clear après la durée du clip
        _currentBarkTween = DOVirtual.DelayedCall(bark.Clip.length, () => {
            _currentBark = null;
            _currentBarkTween = null;
        });
    }
     
    void OnDestroy()
    {
        _currentBarkTween?.Kill();
    }
}


