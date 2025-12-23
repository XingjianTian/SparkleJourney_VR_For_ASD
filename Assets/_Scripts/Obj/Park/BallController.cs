using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class BallController : MonoSingleton<BallController>
{
    [FormerlySerializedAs("leftBallTargetTransform")] [FormerlySerializedAs("ballTargetTransform")] public Transform rightBallTargetTransform;
    [FormerlySerializedAs("rightBallTargetTransform")] public Transform leftBallTargetTransform;
    public Transform ballObjTransform;



    public Transform TestEndTransform;
    public float height = 1;


    private Vector3 rightBallTargetOriginPos;
    private Vector3 leftBallTargetOriginPos;
    private Vector3 ballObjOriginPos;

    private Vector3 originRotation;
    private Vector3 idleRotation = new Vector3(341.6f, 98.5f, 105.2f);
    void Start()
    {
        rightBallTargetOriginPos = rightBallTargetTransform.position;
        originRotation = rightBallTargetTransform.rotation.eulerAngles;
        ballObjOriginPos = ballObjTransform.position;
        leftBallTargetOriginPos = leftBallTargetTransform.position;

    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.lKey.wasPressedThisFrame)
        {
            ThrowBall();
        }

        if (Keyboard.current.pKey.wasPressedThisFrame)
        {
            ReciveBall();
        }

        TestEndTransform = PlayerObj.Instance.transform;
    }

    private Tweener throwTweener;
    private float v;
    public void ThrowBall()
    {
        ballObjTransform.parent = null;
        reciveTweener?.Kill(true);

        HandleBallRandomRotate();
        throwTweener =DOTween.To((value =>
        {
            ballObjTransform.position = Parabola(ballObjOriginPos, TestEndTransform.position,height,value);
            if (value != 1)
                v = value;
            if (value < 0.5f)
            {
                rightBallTargetTransform.position = Parabola(rightBallTargetOriginPos, TestEndTransform.position, height, value);
                leftBallTargetTransform.position = Parabola(leftBallTargetOriginPos, TestEndTransform.position, height, value);
            }
            else
            {
                rightBallTargetTransform.position = Parabola(rightBallTargetOriginPos, TestEndTransform.position, height,
                    1f-value);
                leftBallTargetTransform.position = Parabola(leftBallTargetOriginPos, TestEndTransform.position, height,
                    1f-value);
            }
        }), 0, 1, 1.5f).SetEase(Ease.Linear);
    }

    private Tweener reciveTweener;
    private Vector3 currentTarget;
    public void ReciveBall()
    {
        throwTweener?.Kill(true);
        rightBallTargetTransform.position = ballObjTransform.position;
        leftBallTargetTransform.position = ballObjTransform.position;
        ballObjTransform.parent = rightBallTargetTransform;
        currentTarget = rightBallTargetTransform.position;
        HandleBallRandomRotate();
        reciveTweener = DOTween.To((value =>
        {
            rightBallTargetTransform.position = Parabola(ballObjOriginPos, currentTarget,height,value);
            leftBallTargetTransform.position = Parabola(ballObjOriginPos, currentTarget,height,value);
        }), v, 0, 1.5f).SetEase(Ease.Linear);
    }


    public static Vector3 Parabola(Vector3 start, Vector3 end, float height, float t)
    {
        float Func(float x) => 4 * (-height * x * x + height * x);

        var mid = Vector3.Lerp(start, end, t);

        return new Vector3(mid.x, Func(t) + Mathf.Lerp(start.y, end.y, t), mid.z);
    }

    private void HandleBallRandomRotate()
    {

        // 生成随机旋转角度
        Vector3 randomRotation = new Vector3(
            Random.Range(0, 360),
            Random.Range(0, 360),
            Random.Range(0, 360)
        );

        // 平滑旋转到随机角度
        ballObjTransform.DORotate(randomRotation, 1.5f).SetEase(Ease.Linear);
    }


}
