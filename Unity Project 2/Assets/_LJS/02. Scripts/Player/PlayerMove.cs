using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    // 캐릭터 컨트롤러 컴포넌트를 담을 변수
    private CharacterController cc;

    // 플레이어 이동 속도
    public float speed = 5.0f;
    // 플레이어 점프 파워값
    public float jumpPower = 2.0f;
    // 플레이어 중력값
    public float gravity = -20.0f;
    // 낙하 속도값
    private float velocityY = 0;
    // 플레이어 2단 점프 판별값
    private int jumpCount = 0;

    void Start()
    {
        // 캐릭터 컨트롤러 컴포넌트 가져오기
        cc = GetComponent<CharacterController>();
    }

    void Update()
    {
        // 플레이어 이동
        Move();
    }

    // 플레이어 이동 함수
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
        // cc.Move(dir * speed * Time.deltaTime);

        // 중력을 적용시킨 후 이동
        velocityY += gravity * (Time.deltaTime * 0.3f);
        dir.y = velocityY;
        cc.Move(dir * speed * Time.deltaTime);

        // 캐릭터 점프 (점프 버튼을 누르면 수직속도에 점프 파워를 넣음)
        // 땅에 닿으면 낙하 속도와 점프 카운트를 0으로 초기화
        if (cc.collisionFlags == CollisionFlags.Below)
        {
            velocityY = 0;
            jumpCount = 0;
        }
        // 점프키 입력 시 점프
        if (Input.GetButtonDown("Jump") && jumpCount < 2)
        {
            velocityY = jumpPower;
            jumpCount++;
        }
    }
}