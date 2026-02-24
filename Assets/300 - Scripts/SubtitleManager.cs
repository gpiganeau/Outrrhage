using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class SubtitleManager : MonoBehaviour
{
    public static SubtitleManager I;

    [Header("Refs")]
    [SerializeField] CanvasGroup _canvasGroup;
    [SerializeField] TextMeshProUGUI _label;

    [Header("Typewriter")]
    [SerializeField] float _charsPerSecond = 40f;

    [Header("Emphasis")]
    [SerializeField] Color _emphasisColor = new Color(1f, 0.3f, 0.2f);
    [SerializeField] float _emphasisScale = 1.25f;
    [SerializeField] float _emphasisDuration = 0.12f;

    [Header("Fade")]
    [SerializeField] float _fadeInDuration = 0.15f;
    [SerializeField] float _fadeOutDuration = 0.3f;

    Coroutine _typewriterCoroutine;
    Tween _fadeTween;

    void Awake()
    {
        if (I == null) I = this;
        else Destroy(gameObject);

        _canvasGroup.alpha = 0f;
        _label.text = "";
    }

    // ─── API publique ──────────────────────────────────────────────

    public void Show(Bark bark)
    {
        if (!bark.ShowSubtitles) return;

        StopAll();

        _fadeTween = _canvasGroup.DOFade(1f, _fadeInDuration);
        _typewriterCoroutine = StartCoroutine(TypewriterRoutine(bark));
    }

    public void Hide()
    {
        StopAll();
        _fadeTween = _canvasGroup.DOFade(0f, _fadeOutDuration)
            .OnComplete(() => _label.text = "");
    }

    // ─── Parsing ───────────────────────────────────────────────────

    // Transforme notre markup en tokens prêts à afficher
    List<SubtitleToken> Parse(string raw, ColorPalette palette)
    {
        var tokens = new List<SubtitleToken>();
        int i = 0;

        while (i < raw.Length)
        {
            // Tag ouvrant ?
            if (raw[i] == '<')
            {
                int close = raw.IndexOf('>', i);
                if (close == -1) { tokens.Add(Char(raw[i], palette.Main)); i++; continue; }

                string tag = raw.Substring(i + 1, close - i - 1); // contenu entre < >

                // <em>
                if (tag == "em")
                {
                    int endTag = raw.IndexOf("</em>", close);
                    if (endTag == -1) { i = close + 1; continue; }
                    string word = raw.Substring(close + 1, endTag - close - 1);
                    tokens.Add(new SubtitleToken { Text = word, Type = TokenType.Emphasis, Color = _emphasisColor });
                    i = endTag + 5;
                    continue;
                }

                // <c=#RRGGBB>
                if (tag.StartsWith("c="))
                {
                    string hex = tag.Substring(2);
                    ColorUtility.TryParseHtmlString(hex, out Color col);
                    int endTag = raw.IndexOf("</c>", close);
                    if (endTag == -1) { i = close + 1; continue; }
                    string word = raw.Substring(close + 1, endTag - close - 1);
                    tokens.Add(new SubtitleToken { Text = word, Type = TokenType.Colored, Color = col });
                    i = endTag + 4;
                    continue;
                }

                // Tag inconnu : on skip
                i = close + 1;
            }
            else
            {
                // Texte plain : on accumule jusqu'au prochain tag ou fin
                int next = raw.IndexOf('<', i);
                string chunk = next == -1 ? raw.Substring(i) : raw.Substring(i, next - i);
                if (chunk.Length > 0)
                    tokens.Add(new SubtitleToken { Text = chunk, Type = TokenType.Normal, Color = palette.Main });
                i = next == -1 ? raw.Length : next;
            }
        }

        return tokens;
    }

    SubtitleToken Char(char c, Color col) =>
        new SubtitleToken { Text = c.ToString(), Type = TokenType.Normal, Color = col };

    // ─── Typewriter ────────────────────────────────────────────────

    IEnumerator TypewriterRoutine(Bark bark)
    {
        _label.text = "";
        var tokens = Parse(bark.Text, bark.SpeakerPalette);

        float delay = 1f / _charsPerSecond;

        foreach (var token in tokens)
        {
            if (token.Type == TokenType.Emphasis)
            {
                // On affiche le mot d'un coup + animation de pop
                AppendRichText(token.Text, token.Color, bold: true);
                yield return StartCoroutine(EmphasisPop());
                yield return new WaitForSeconds(delay * 3f); // légère pause après un mot fort
            }
            else
            {
                // Caractère par caractère
                for (int c = 0; c < token.Text.Length; c++)
                {
                    AppendRichText(token.Text[c].ToString(), token.Color);

                    // Pause plus longue sur ponctuation
                    char ch = token.Text[c];
                    float wait = (ch == ',' || ch == ';') ? delay * 4f
                               : (ch == '.' || ch == '!' || ch == '?') ? delay * 6f
                               : delay;

                    yield return new WaitForSeconds(wait);
                }
            }
        }
    }

    void AppendRichText(string text, Color col, bool bold = false)
    {
        string hex = ColorUtility.ToHtmlStringRGB(col);
        string wrapped = bold
            ? $"<color=#{hex}><b>{text}</b></color>"
            : $"<color=#{hex}>{text}</color>";
        _label.text += wrapped;
    }

    IEnumerator EmphasisPop()
    {
        // Scale up/down sur le Transform du label (simple mais efficace)
        yield return _label.transform
            .DOScale(_emphasisScale, _emphasisDuration)
            .SetEase(Ease.OutBack)
            .WaitForCompletion();

        yield return _label.transform
            .DOScale(1f, _emphasisDuration)
            .SetEase(Ease.InOutSine)
            .WaitForCompletion();
    }

    void StopAll()
    {
        if (_typewriterCoroutine != null) StopCoroutine(_typewriterCoroutine);
        _fadeTween?.Kill();
        _label.transform.DOKill();
        _label.transform.localScale = Vector3.one;
    }

    void OnDestroy() => StopAll();
}

// ─── Types ─────────────────────────────────────────────────────────────────────

public enum TokenType { Normal, Colored, Emphasis }

public class SubtitleToken
{
    public string Text;
    public TokenType Type;
    public Color Color;
}