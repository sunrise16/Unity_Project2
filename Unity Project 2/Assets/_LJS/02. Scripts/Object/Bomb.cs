using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    // 폭발 이펙트 오브젝트
    public GameObject explosionEffect;

    private void OnCollisionEnter(Collision collision)
    {
        // 폭발 이펙트 보여주기
        GameObject fx = Instantiate(explosionEffect);
        fx.transform.position = transform.position;

        // 이펙트 오브젝트가 사라지지 않는 경우
        Destroy(fx, 2.0f);
        Destroy(gameObject);
    }
}
