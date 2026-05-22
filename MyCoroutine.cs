using System.Collections;
using UnityEngine;

public class CoroutineExample : MonoBehaviour
{
    //1. 缓存WaitForSeconds，避免每次new产生GC
    private WaitForSeconds wait1s = new WaitForSeconds(1f);
    private Coroutine currentCoroutine; //2. 保存协程引用，方便停止

    void Start()
    {
        //3. 启动协程
        currentCoroutine = StartCoroutine(MyCoroutine("Hello", 3));
    }

    void Update()
    {
        //按空格停止协程
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StopCoroutine(currentCoroutine);
            Debug.Log("协程被手动停止");
        }
    }

    //4. 协程方法：返回类型必须是IEnumerator
    IEnumerator MyCoroutine(string message, int repeatCount)
    {
        Debug.Log("协程开始");
        for (int i = 0; i < repeatCount; i++)
        {
            Debug.Log($"{message} 第{i + 1}次");
            yield return wait1s; //5. 每次暂停 1 秒
        }
        Debug.Log("协程结束");
        //6. 方法自然跑完 = 协程自动结束，不需要手动stop
    }
}
