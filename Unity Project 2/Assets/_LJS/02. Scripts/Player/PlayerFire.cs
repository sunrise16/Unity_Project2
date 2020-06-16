using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFire : MonoBehaviour
{
    // 폭탄 오브젝트
    public GameObject bombObject;
    // 폭탄 투척 위치 기준점
    public GameObject firePoint;
    // 총알 착탄 이펙트 오브젝트
    public GameObject bulletEffect;

    // Ray의 최대 사정거리
    public float rayMaxDistance = 30.0f;
    // 폭탄 투척 파워
    public float bombThrowPower = 20.0f;

    // Update is called once per frame
    void Update()
    {
        Fire();
    }

    void Fire()
    {
        // 마우스 좌클릭 시 Raycast로 총알 발사
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            ray.origin = Camera.main.transform.position;
            ray.direction = Camera.main.transform.forward;
            Debug.DrawRay(transform.position, transform.forward * rayMaxDistance, Color.blue, 0.3f);

            // 레이캐스트에 검출된 객체의 정보를 저장할 변수
            RaycastHit hitInfo;
            // 레이캐스트를 생성해 적 캐릭터를 검출
            if (Physics.Raycast(ray, out hitInfo, rayMaxDistance))
            {
                // hitInfo.transform.GetComponent<MeshRenderer>().material.color = Color.red;
                GameObject bulletImpact = Instantiate(bulletEffect);
                bulletImpact.transform.position = hitInfo.transform.position;
                // 파편 이펙트 (부딪힌 지점이 향하는 방향으로 튀게 해줘야 함)
                bulletImpact.transform.forward = hitInfo.normal;
            }

            // 레이어 마스크를 사용한 충돌 처리
            // int layer = gameObject.layer;
            // layer = 1 << 8 | 1 << 9 | 1 << 12;
        }
        // 마우스 우클릭 시 수류탄 투척
        if (Input.GetMouseButtonDown(1))
        {
            // 폭탄 생성
            GameObject bomb = Instantiate(bombObject);
            bomb.transform.position = firePoint.transform.position;

            // 전방으로 물리적인 힘 가하기
            Rigidbody rb = bomb.GetComponent<Rigidbody>();

            // 각도를 부여하여 발사
            Vector3 dir = Camera.main.transform.forward + Camera.main.transform.up;
            dir.Normalize();
            rb.AddForce(dir * bombThrowPower, ForceMode.Impulse);

            // ForceMode.Acceleration = 연속적인 힘을 가함 (질량의 영향을 받지 않음)
            // ForceMode.Force = 연속적인 힘을 가함 (질량의 영향을 받음)
            // ForceMode.Impulse = 순간적인 힘을 가함 (질량의 영향을 받음)
            // ForceMode.VelocityChange = 순간적인 힘을 가함 (질량의 영향을 받지 않음)
        }
    }
}
