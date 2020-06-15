using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    // 캐릭터 컨트롤러 컴포넌트를 담을 변수
    private CharacterController cc;

    // 플레이어 이동 속도
    public float speed = 5.0f;

    private void Start()
    {
        cc = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    void Move()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 dir = new Vector3(h, 0, v);
        // 대각선 이동 속도를 상하좌우 속도와 동일하게 만들기
        // 게임에 따라 일부러 대각선은 빠르게 이동하도록 하는 경우도 존재하므로 이 경우 벡터 정규화를 하지 말 것
        // dir.Normalize();
        // transform.Translate(dir * speed * Time.deltaTime);

        // 카메라가 보는 방향으로 이동해야 함
        dir = Camera.main.transform.TransformDirection(dir);
        // transform.Translate(dir * speed * Time.deltaTime);

        cc.Move(dir * speed * Time.deltaTime);
    }
}
