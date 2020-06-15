using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollow : MonoBehaviour
{
    // 플레이어한테 바로 카메라를 붙여서 이동해도 상관 X
    // 게임에 따라서 드라마틱한 연출에 타겟을 따라다니도록 하는게 1인칭에서 3인칭 또는 그 반대로의 변경이 원활

    // 카메라가 플레이어를 따라다니기
    public Transform firstPersonTarget;
    public Transform thirdPersonTarget;
    // 카메라 이동 속도
    public float speed = 10.0f;
    // 1인칭, 3인칭 시점 변경할 변수
    private bool isFirstPerson = false;

    // Update is called once per frame
    void Update()
    {
        // transform.position = target.position;
        // 시점 변경
        ChangeView();
        
        // FirstPerson();
    }

    void ChangeView()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (isFirstPerson == true)
            {
                isFirstPerson = false;
            }
            else
            {
                isFirstPerson = true;
            }
        }

        if (isFirstPerson == true)
        {
            transform.position = firstPersonTarget.position;
        }
        else
        {
            transform.position = thirdPersonTarget.position;
        }
    }

    // void FirstPerson()
    // {
    //     // 타겟의 방향 구하기 (벡터의 뺄셈)
    //     Vector3 dir = target.position - transform.position;
    //     dir.Normalize();
    //     transform.Translate(dir * speed * Time.deltaTime);
    // 
    //     if (Vector3.Distance(transform.position, target.position) < 1.0f)
    //     {
    //         transform.position = target.position;
    //     }
    // }
}
