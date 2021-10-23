using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BattleUnit : MonoBehaviour // this class has the unit animations and image loading
{
    [SerializeField] bool isPlayerUnit;
    [SerializeField] BattleHud hud;

    public Borpamon Borpamon { get; set; }

    public bool IsPlayerUnit { get; set; }

    public BattleHud Hud { get { return hud; } }

    Image image;
    Vector3 originalPosition;
    Color originalColor;

    private void Awake()
    {
        image = GetComponent<Image>();
        originalPosition = image.transform.localPosition;
        originalColor = image.color;
    }
    public void Setup(Borpamon borpamon)
    {
        Borpamon = borpamon;
        if (isPlayerUnit)
        {
            image.sprite = Borpamon.Base.BackSprite;
        }
        else
        {
            image.sprite = Borpamon.Base.FrontSprite;
        }
        hud.SetData(Borpamon);
        image.color = originalColor;
        PlayEnterAnimation();
    }

    public void PlayEnterAnimation()
    {
        if (isPlayerUnit)
            image.transform.localPosition = new Vector3(-500f, originalPosition.y);
        else
            image.transform.localPosition = new Vector3(500f, originalPosition.y);

        image.transform.DOLocalMoveX(originalPosition.x, 1f);
    }

    public void PlayAttackAnimation()
    {
        var sequence = DOTween.Sequence();

        if (isPlayerUnit) // May need to make transform amount relative to the image size.
            sequence.Append(image.transform.DOLocalMoveX(originalPosition.x + 500, .25f));
        else
            sequence.Append(image.transform.DOLocalMoveX(originalPosition.x - 500, .25f));

        image.transform.DOLocalMoveX(originalPosition.x, .25f);
    }

    public void PlayHitAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(image.DOColor(Color.gray, 0.1f));
        sequence.Append(image.DOColor(originalColor, 0.1f));
    }

    public void PlayFaintAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(image.transform.DOLocalMoveY(originalPosition.y - 150, 0.5f));
        sequence.Join(image.DOFade(0f, 0.5f));
    }
}

