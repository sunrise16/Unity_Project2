using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRotate : MonoBehaviour
{
    // 카메라 회전 속도 (카메라를 마우스 움직이는 방향으로 회전하기)
    public float speed = 150.0f;
    // 회전 각도를 직접 제어할 변수
    private float angleX;

    // Update is called once per frame
    void Update()
    {
        // 플레이어 회전
        Rotate();
    }

    // 플레이어 회전 처리 함수
    void Rotate()
    {
        float h = Input.GetAxis("Mouse X");

        angleX += h * speed * Time.deltaTime;
        transform.eulerAngles = new Vector3(0, angleX, 0);
    }
}
