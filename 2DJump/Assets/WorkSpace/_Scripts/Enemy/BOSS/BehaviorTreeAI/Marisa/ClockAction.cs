using System.Collections;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

public class ClockAction : Action
{
    [SerializeField] private SharedTransform Clocks;
    private Transform[] ClockList = new Transform[3];

    [SerializeField] private int mode;

    public override void OnAwake()
    {
        for (int i = 0; i < ClockList.Length; i++)
        {
            ClockList[i] = Clocks.Value.GetChild(i);
        }
    }

    public override void OnStart()
    {
        switch (mode)
        {
            case 1:// 随机开一个
                StartCoroutine(ClockLightColumn(1));
                break;
            case 2:// 随机开两个
                StartCoroutine(ClockLightColumn(2));
                break;
            case 3:// 全开
                StartCoroutine(ClockLightColumn(ClockList.Length));
                break;
            default:
                break;
        }
    }

    private IEnumerator ClockLightColumn(int Length)
    {
        int count = 0;
        int[] randomArr = { 0, 1, 2 };
        Shuffle(randomArr);

        while (count < Length)
        {
            LightColumnBarrageFactory.Instance.CreatBarrage(ClockList[randomArr[count]].GetChild(0).GetChild(0).position, Vector3.zero, 0, ClockList[randomArr[count]].GetChild(0), randomArr[count]);
            count++;
            yield return new WaitForSeconds(2f);
        }
    }

    /// 洗牌算法（不重复随机）
    public int[] Shuffle(int[] dataArray)
    {
        for (int i = 0; i < dataArray.Length; i++)
        {
            int randomNum = Random.Range(i, dataArray.Length);

            int temp = dataArray[randomNum];
            dataArray[randomNum] = dataArray[i];
            dataArray[i] = temp;
        }
        return dataArray;
    }
}
