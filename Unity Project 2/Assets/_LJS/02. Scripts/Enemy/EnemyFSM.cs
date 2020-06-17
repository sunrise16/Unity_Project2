using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFSM : MonoBehaviour
{
    #region 컴포넌트 변수 관련

    // 몬스터 상태값 열거문
    public enum EnemyState { Idle, Move, Attack, Return, Damaged, Die}
    // 타겟 오브젝트
    private GameObject targetObject;
    // 캐릭터 컨트롤러 컴포넌트
    private CharacterController cc;

    #endregion

    #region 제어값 변수 관련

    // 몬스터의 최초 스폰 지점
    private Vector3 spawnPoint;
    // 몬스터의 상태
    private EnemyState enemyState;
    // 몬스터와 타겟(플레이어) 오브젝트와의 거리값
    private float distance;
    // 몬스터의 이동 속도
    public float enemyMoveSpeed;
    // 몬스터의 중력값
    public float enemyGravity;
    // 몬스터의 낙하 속도값
    private float enemyVelocityY;

    #region Idle 상태에 필요한 변수

    // 몬스터의 순찰 지점
    private Vector3 patrolPoint;
    // 몬스터의 현재 위치에서 순찰 지점으로의 방향
    private Vector3 patrolDirection;

    #endregion

    #region Move 상태에 필요한 변수

    // 몬스터와 타겟 간의 방향
    private Vector3 moveDirection;

    #endregion

    #region Attack 상태에 필요한 변수

    // 몬스터의 공격 사정거리
    private float attackRange;
    // 몬스터의 공격 범위
    private float attackAngle;
    // 타겟 오브젝트의 레이어 값
    private LayerMask targetLayer;

    #endregion

    #region Return 상태에 필요한 변수

    // Return 상태 제어값
    private bool enemyReturn = false;

    #endregion

    #region Damaged 상태에 필요한 변수

    // Damaged 상태 제어값
    private bool enemyDamaged = false;

    #endregion

    #region Die 상태에 필요한 변수

    // Die 상태 제어값
    private bool enemyDie = false;

    #endregion

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        #region 컴포넌트 변수 관련 초기화

        // 타겟 오브젝트 지정 (플레이어)
        targetObject = GameObject.Find("Player");

        // 캐릭터 컨트롤러 컴포넌트 추출
        cc = GetComponent<CharacterController>();

        #endregion

        #region 제어값 변수 관련 초기화

        // 몬스터의 최초 스폰 지점 설정
        spawnPoint = transform.position;
        // 몬스터의 상태 초기화
        enemyState = EnemyState.Idle;
        // 몬스터와 타겟(플레이어) 오브젝트와의 거리값 초기화
        distance = 0.0f;
        // 몬스터의 낙하 속도값 초기화
        enemyVelocityY = 0.0f;
        // 몬스터의 공격 사정거리 초기화
        attackRange = 5.0f;
        // 몬스터의 공격 범위 초기화
        attackAngle = 120.0f;
        // 타겟 오브젝트의 레이어 값 초기화
        targetLayer = LayerMask.NameToLayer("PLAYER");

        #endregion

        #region 코루틴 함수 실행

        StartCoroutine(EnemyPatrol());
        StartCoroutine(EnemyAttack());

        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        #region 실시간 변수값 갱신

        // 몬스터와 타겟(플레이어) 오브젝트와의 거리값 갱신
        distance = Vector3.Distance(transform.position, targetObject.transform.position);

        // 몬스터가 땅에 닿으면 낙하 속도와 점프 카운트를 0으로 초기화
        if (cc.collisionFlags == CollisionFlags.Below)
        {
            enemyVelocityY = 0;
        }

        #endregion

        #region 조건에 따른 상태 제어값 설정

        #region Return 상태

        // Return 상태 활성화
        if (Vector3.Distance(transform.position, spawnPoint) > 30.0f)
        {
            enemyReturn = true;
        }
        // 스폰 위치로 돌아온 후 Return 상태 비활성화
        if (enemyReturn == true && Vector3.Distance(transform.position, spawnPoint) <= 0.0f)
        {
            enemyReturn = false;
        }

        #endregion

        #region Damaged 상태
        #endregion

        #endregion

        #region 조건에 따른 몬스터 상태값 설정

        // 거리값이 12.0f 이상일 시 Idle 상태
        if (Mathf.Abs(distance) >= 12.0f)
        {
            enemyState = EnemyState.Idle;
        }
        // 거리값이 1.5f 이상 12.0f 미만일 시 Move 상태
        else if (Mathf.Abs(distance) >= 1.5f && Mathf.Abs(distance) < 12.0f)
        {
            enemyState = EnemyState.Move;
        }
        // 거리값이 1.5f 미만일 시 Attack 상태
        else if (Mathf.Abs(distance) < 1.5f)
        {
            enemyState = EnemyState.Attack;
        }
        // Return 상태가 활성화일 때
        else if (enemyReturn == true)
        {
            enemyState = EnemyState.Return;
        }
        // Damaged 상태가 활성화일 때
        else if (enemyDamaged == true)
        {
            enemyState = EnemyState.Damaged;
        }
        // Die 상태가 활성화일 때
        else if (enemyDie == true)
        {
            enemyState = EnemyState.Die;
        }

        #endregion

        #region 상태에 따른 몬스터의 행동 처리

        switch (enemyState)
        {
            // Idle 상태 시 행동
            case EnemyState.Idle:
                Idle();
                break;
            // Move 상태 시 행동
            case EnemyState.Move:
                Move();
                break;
            // Attack 상태 시 행동
            case EnemyState.Attack:
                Attack();
                break;
            // Return 상태 시 행동
            case EnemyState.Return:
                Return();
                break;
            // Damaged 상태 시 행동
            case EnemyState.Damaged:
                Damaged();
                break;
            // Die 상태 시 행동
            case EnemyState.Die:
                Die();
                break;
        }

        #endregion
    }

    #region 몬스터의 상태값에 따른 행동 실행 함수

    private void Idle()
    {
        // 1. 플레이어가 일정 범위 내로 들어오면 이동 상태로 변경
        // 2. 플레이어 찾기 (GameObject.Find("Player"))
        // 탐지 범위 : 12.0f 이상

        // 몬스터의 이동 속도 재설정
        enemyMoveSpeed = 2.0f;

        // 순찰 지점을 향해 이동
        // transform.Translate(patrolDirection * enemyMoveSpeed * Time.deltaTime);

        // 중력을 적용시킨 후 순찰 지점을 향해 이동
        enemyVelocityY += enemyGravity * (Time.deltaTime * 0.3f);
        patrolDirection.y = enemyVelocityY;
        cc.Move(patrolDirection * enemyMoveSpeed * Time.deltaTime);
    }
    private void Move()
    {
        // 1. 플레이어를 향해 이동 후 공격 범위 안에 들어오면 공격 상태로 변경
        // 2. 플레이어를 추격하더라도 처음 위치에서 일정 범위를 넘어가지 않도록 조치
        // 3. 플레이어처럼 캐릭터 컨트롤러를 이용
        // 탐지 범위 : 1.5f 이상 ~ 12.0f 미만

        // 몬스터의 이동 속도 재설정
        enemyMoveSpeed = 4.0f;
        // 타겟의 방향 구하기
        moveDirection = transform.position - targetObject.transform.position;
        moveDirection.Normalize();
        // 타겟을 향해 바라보기
        transform.LookAt(targetObject.transform);
        // 타겟을 향해 이동
        transform.Translate(moveDirection * enemyMoveSpeed * Time.deltaTime);
    }
    private void Attack()
    {
        // 1. 플레이어가 공격 범위 안에 있다면 일정한 시간 간격으로 플레이어를 공격
        // 2. 플레이어가 공격 범위를 벗어나면 이동 상태(재추격)로 변경
        // 탐지 범위 : 0.0f 이상 ~ 1.5f 미만
    }
    private void Return()
    {
        // 1. 몬스터가 플레이어를 추적하다가 너무 멀리 벗어났을 경우 최초 스폰 위치로 복귀
        // 탐지 범위 : 최초 스폰 위치에서 30.0f 이상
    }
    private void Damaged()
    {
        // 1. 몬스터의 체력이 1 이상일 경우 피격 상태로 전환 후 다시 이전의 상태로 변경
        // 2. 트랜지션은 Any State 에서 연결
    }
    private void Die()
    {
        // 1. 몬스터의 체력이 0일 경우 사망 상태로 전환, 그 후 몬스터 오브젝트 삭제
        // 2. 트랜지션은 Any State 에서 연결
    }

    #endregion

    #region 코루틴 함수

    #region 몬스터 순찰

    IEnumerator EnemyPatrol()
    {
        if (enemyState == EnemyState.Idle)
        {
            while (true)
            {
                // 대기 시간 랜덤 설정
                float waitSecond = Random.Range(4.0f, 6.0f);
                // 순찰할 지점 랜덤 설정
                patrolPoint = new Vector3(transform.position.x + Random.Range(-3.0f, 3.0f), 0, transform.position.z + Random.Range(-3.0f, 3.0f));
                // 순찰 지점의 방향 구하기
                patrolDirection = transform.position - patrolPoint;
                patrolDirection.Normalize();

                // 랜덤 설정된 waitSecond 초 대기
                yield return new WaitForSeconds(waitSecond);
            }
        }
    }

    #endregion

    #region 몬스터 공격

    IEnumerator EnemyAttack()
    {
        if (enemyState == EnemyState.Attack)
        {
            while (true)
            {
                // 몬스터의 공격 사정거리 안에서 타겟 오브젝트를 추출
                Collider[] colls = Physics.OverlapSphere(transform.position, attackRange, 1 << targetLayer);

                // 몬스터 공격 애니메이션 실행
                // Animation Code Here

                // 배열의 개수가 1일 때 타겟 오브젝트가 범위 안에 있다고 판단
                if (colls.Length == 1)
                {
                    // 몬스터와 타겟 오브젝트 사이의 방향 벡터를 계산
                    Vector3 attackDirection = (targetObject.transform.position - transform.position).normalized;

                    // 타겟 오브젝트가 몬스터의 공격 범위 안에 들어왔는지를 판단
                    if (Vector3.Angle(transform.forward, attackDirection) < attackAngle * 0.5f)
                    {
                        // 타겟 오브젝트의 체력 감소
                        // Target Object HP Down Here
                        Debug.Log("Enemy Attack Hit");
                    }
                }

                // 2초 대기
                yield return new WaitForSeconds(2.0f);

                // 몬스터의 공격 사정거리 안에 타겟 오브젝트가 존재하지 않을 경우
                if (colls.Length == 0)
                {
                    break;
                }
            }
        }
    }

    #endregion

    #endregion
}