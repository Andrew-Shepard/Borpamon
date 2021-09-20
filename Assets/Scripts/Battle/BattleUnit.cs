using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BattleUnit : MonoBehaviour // this class has the unit animations and image loading
{
    [SerializeField] BorpamonBase borpamon_base;
    [SerializeField] int level;
    [SerializeField] bool isPlayerUnit;

    public Borpamon Borpamon { get; set; }

    Image image;
    Vector3 originalPosition;
    Color originalColor;

    private void Awake()
    {
        image = GetComponent<Image>();
        originalPosition = image.transform.localPosition;
        originalColor = image.color;
    }
    public void Setup()
    {
        Borpamon = new Borpamon(borpamon_base, level);
        if (isPlayerUnit)
        {
            image.sprite = Borpamon.Borpamon_base.BackSprite;
        }
        else
        {
            image.sprite = Borpamon.Borpamon_base.FrontSprite;
        }

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
            sequence.Append(image.transform.DOLocalMoveX(originalPosition.x + 1000, .25f));
        else
            sequence.Append(image.transform.DOLocalMoveX(originalPosition.x - 1000, .25f));

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

