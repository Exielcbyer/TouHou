using UnityEngine;

public class RubbingBulletsArea : MonoBehaviour
{
    public int energy = 5;
    public bool isRubOver;

    private void OnEnable()
    {
        isRubOver = false;
    }

    public void Rub(Transform target)
    {
        SpellCardFactory.Instance.CreatBarrage(transform.position, Vector3.zero, 0, target, energy);
    }
}
