using UnityEngine;
using System.Collections;
public class CountdownTimer : MonoBehaviour
{
    public float duration = 10f;
    void Start()
    {
        StartCoroutine(Countdown());
    }

    //协程方法
    IEnumerator Countdown()
    {
        float remaining = duration;
        while(remaining > 0f)
        {
            Debug.Log($"倒计时：{remaining:F1} 秒");
            yield return new WaitForSeconds(1f);
            remaining -= 1f;
        }
        Debug.Log("时间到。");
    }
  }
  /*
  $"倒计时: {remaining} 秒"       //输出: 倒计时: 9秒
  $"倒计时: {remaining:F1} 秒"    //输出: 倒计时: 9.0秒
  $"倒计时: {remaining:F2} 秒"    //输出: 倒计时: 9.00秒
  */