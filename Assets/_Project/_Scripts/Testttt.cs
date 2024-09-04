using EranCore.Tweening;
using UnityEngine;

public class Testttt : MonoBehaviour
{
    void Start()
    {
        hello();
    }

    private void hello()
    {
        ContenManager.EmitResource("Cube", (data) =>
        {
            data.transform.TweenMove(Vector3.up * 10, 2).SetDelay(Random.Range(0.2f, 2f)).From(Vector3.zero).OnComplete(() =>
            {
                Invoke(nameof(hello), 1);
            });
        });
    }
}
