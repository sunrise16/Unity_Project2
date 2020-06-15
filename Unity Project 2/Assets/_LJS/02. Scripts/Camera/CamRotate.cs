using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamRotate : MonoBehaviour
{
    // 카메라 회전 속도 (카메라를 마우스 움직이는 방향으로 회전하기)
    public float speed = 150.0f;
    // 회전 각도를 직접 제어할 변수
    private float angleX, angleY;

    // Update is called once per frame
    void Update()
    {
        // 카메라 회전
        Rotate();
    }

    // 카메라 회전 처리 함수
    void Rotate()
    {
        float h = Input.GetAxis("Mouse X");
        float v = Input.GetAxis("Mouse Y");

        // Vector3 dir = new Vector3(v, -h, 0);
        // transform.Rotate(dir * speed * Time.deltaTime);

        // 유니티 엔진에서 제공해주는 함수를 사용함에 있어서 Translate 함수는 사용하는데 큰 불편함이 없음
        // 그러나 Rotate 함수는 직접 제어하기가 힘듬 (두 개 이상의 회전축이 겹치게 되는 짐벌락 현상이 발생)
        // Inspector 창의 로테이션 값은 우리가 보기 편하게 오일러 각도로 표시되지만 내부적으로는 Quaternion 값으로 회전 처리가 되고 있음
        // Quaternion을 사용하는 이유는 짐벌락 현상을 방지할 수 있기 때문
        // 회전을 직접 제어할 때는 Rotate 함수를 사용하지 않고 Transform의 Euler Angle을 사용하면 됨

        // eulerAngles를 사용하면 카메라가 고정되었다 풀렸다 하는 문제가 있음
        // 직접 회전각도를 제한해서 처리하면 됨
        // 그러나 여기에도 문제가 있는데 유니티는 내부적으로 음수의 각도는 360도를 더해서 처리됨
        // 즉 자신이 만든 각도를 가지고 계산을 처리해야 함
        // transform.eulerAngles += dir * speed * Time.deltaTime;
        // Vector3 angle = transform.eulerAngles;
        // angle += dir * speed * Time.deltaTime;
        // if (angle.x > 60) angle.x = 60;
        // if (angle.x < -60) angle.x = -60;
        // transform.eulerAngles = angle;

        angleX += h * speed * Time.deltaTime;
        angleY += v * speed * Time.deltaTime;
        angleY = Mathf.Clamp(angleY, -60, 60);
        transform.eulerAngles = new Vector3(-angleY, angleX, 0);
    }
}
