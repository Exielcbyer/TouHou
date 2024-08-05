using UnityEngine;

public class SpellCardFactory : Singleton<SpellCardFactory>, BarrageFactory
{
    public GameObject spellCardPrefab;

    public void CreatBarrage(Vector3 creatPos, Vector3 direction, float angle = 0, Transform target = null, int index = 0)
    {
        GameObject spellCard = ObjectPool.Instance.GetObject(spellCardPrefab);
        spellCard.GetComponent<SpellCard>().Init(creatPos, direction, angle, target, index);
    }
}
