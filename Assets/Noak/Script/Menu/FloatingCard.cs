using DG.Tweening;
using UnityEngine;

public class FloatingCard : MonoBehaviour
{
    void Start()
    {
        transform.DOLocalMoveY(transform.localPosition.y + 10f, 1.5f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }
}
