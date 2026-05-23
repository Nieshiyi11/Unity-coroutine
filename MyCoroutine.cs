using UnityEngine;
using System.Collections;

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

/*
启动 StartCoroutine(MyCoroutine("Hello", 3)) 之后 -> Unity立即执行IEnumerator MyCoroutine，一路跑到第一个yield：
  -执行: Debug.Log("协程开始")
  -进入 for循环
  -执行: Debug.Log("Hello 第1次")
  -遇到: yield return wait1s  停在这里，返回主程序
  --[imp]: 控制权还给Unity，Update正常跑，渲染正常跑

  ---
  把 yield return wait1s 拆成两步理解：
  1. return：把wait1s（一个WaitForSeconds(1)对象）交给Unity的协程调度器，意思是"1 秒之后再来找我"
  2. 方法挂起：局部变量（message、repeatCount、i）都在内存里保持着，不会销毁

  ---
  Unity在幕后每帧检查：这个协程交给我的WaitForSeconds对象，时间到了没？
  第 0 秒: 协程挂起，Unity开始计时
  第 1 帧: 没到 1 秒，不恢复
  第 2 帧: 没到 1 秒，不恢复
  ...60 帧后...
  第 1 秒: 到了！恢复这个协程

  ---
  循环 3 次的完整时间线
  ┌─ t=0s  协程开始 → 打印 "协程开始"
  │         i=0 → 打印 "Hello 第1次" → yield return wait1s（挂起）
  │
  ├─ t=1s  被唤醒 → i=1 → 打印 "Hello 第2次" → yield return wait1s（挂起）
  │
  ├─ t=2s  被唤醒 → i=2 → 打印 "Hello 第3次" → yield return wait1s（挂起）
  │
  └─ t=3s  被唤醒 → i=3 → for条件 i<3 不满足 → 跳过循环体
           执行Debug.Log("协程结束")
           方法自然结束 → 协程自动终止，不需要StopCoroutine

  ---
  不满足循环条件之后
  方法继续往下走，执行循环后面的代码。在你这个例子里就是 Debug.Log("协程结束")。然后方法跑到大括号尽头，自然返回
  协程调度器发现：这个IEnumerator的MoveNext()返回了false（迭代器中没有下一个 yield 了）,于是调度器把它标记为"已完成"，从调度列表中移除。不需要手动StopCoroutine
  这就是：被StopCoroutine中断 vs 自然结束

  ---
  按空格触发的 StopCoroutine 是强行掐断
*/

/*
Unity Console控制台应该输出:
协程开始
Hello 第1次
Hello 第2次
Hello 第3次
协程结束
(如果不按空格键强行结束的话)
*/