using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFSM : MonoBehaviour
{
    #region 컴포넌트 변수 관련

    // 몬스터 상태값 열거문
    public enum EnemyState { Idle, Move, Attack, Return, Damaged, Die }
    // 타겟 오브젝트
    private GameObject targetObject;
    // 캐릭터 컨트롤러
    private CharacterController cc;

    #endregion

    #region 제어값 변수 관련

    #region 메인 제어 변수

    // 몬스터의 상태
    public EnemyState enemyState;
    // 몬스터와 타겟(플레이어) 오브젝트와의 거리값
    private float distance;
    // 몬스터의 체력
    public float enemyHP;
    // 몬스터의 공격력
    public float enemyAttackValue;
    // 몬스터의 이동 속도
    public float enemyMoveSpeed;

    #endregion

    #region Idle 상태에 필요한 변수

    // 몬스터의 순찰 지점
    private Vector3 patrolPoint;
    // 몬스터의 현재 위치에서 순찰 지점으로의 방향
    private Vector3 patrolDirection;

    #endregion

    #region Move 상태에 필요한 변수

    // 몬스터와 타겟 간의 방향
    private Vector3 moveDirection;
    // 플레이어 감지 범위
    public float findRange;

    #endregion

    #region Attack 상태에 필요한 변수

    // 타겟 오브젝트의 레이어 값
    private LayerMask targetLayer;
    // 몬스터의 공격 사정거리
    public float attackRange;
    // 몬스터의 공격 딜레이 기준값
    public float attackDelay;
    // 몬스터의 공격 딜레이값
    private float attackDelayTimer;

    #endregion

    #region Return 상태에 필요한 변수

    // Return 상태 제어값
    private bool enemyReturn = false;
    // 몬스터의 최초 스폰 지점
    private Vector3 spawnPoint;
    // 시작 지점에서 최대 이동 가능한 범위
    public float moveRange;

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

        // 몬스터의 상태 초기화
        enemyState = EnemyState.Idle;

        // 타겟 오브젝트 지정 (플레이어)
        targetObject = GameObject.Find("Player");

        // 캐릭터 컨트롤러 컴포넌트 추출
        cc = GetComponent<CharacterController>();

        #endregion

        #region 제어값 변수 관련 초기화

        // 몬스터의 최초 스폰 지점 설정
        spawnPoint = transform.position;

        // 몬스터와 타겟(플레이어) 오브젝트와의 거리값 초기화
        distance = 0.0f;

        // 타겟 오브젝트의 레이어 값 초기화
        targetLayer = LayerMask.NameToLayer("PLAYER");

        #endregion

        #region 코루틴 함수 실행

        // 몬스터 순찰 코루틴
        StartCoroutine(EnemyPatrol());

        // 몬스터 공격 코루틴
        StartCoroutine(EnemyAttack());

        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        #region 실시간 변수값 갱신

        // 몬스터와 타겟(플레이어) 오브젝트와의 거리값 갱신
        distance = Vector3.Distance(transform.position, targetObject.transform.position);

        #endregion

        #region 조건에 따른 상태 제어값 설정

        #region Return 상태

        // Return 상태 활성화
        if (Vector3.Distance(transform.position, spawnPoint) >= moveRange)
        {
            enemyReturn = true;
        }
        // 스폰 위치로 돌아온 후 Return 상태 비활성화
        if (enemyReturn == true && Vector3.Distance(transform.position, spawnPoint) <= 0.1f)
        {
            enemyReturn = false;
            enemyState = EnemyState.Idle;
        }

        #endregion

        #region Damaged 상태
        #endregion

        #endregion

        #region 조건에 따른 몬스터 상태값 설정

        // Return 상태가 활성화일 때
        if (enemyReturn == true)
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
        // 그 이외
        else
        {
            // 거리값이 findRange 이상일 시 Idle 상태
            if (Mathf.Abs(distance) >= findRange)
            {
                enemyState = EnemyState.Idle;
            }
            // 거리값이 attackRange 이상 findRange 미만일 시 Move 상태
            else if (Mathf.Abs(distance) >= attackRange && Mathf.Abs(distance) < findRange)
            {
                enemyState = EnemyState.Move;
            }
            // 거리값이 attackRange 미만일 시 Attack 상태
            else if (Mathf.Abs(distance) < attackRange)
            {
                enemyState = EnemyState.Attack;
            }
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

        // 순찰 지점을 향해 바라보기
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(patrolDirection), 10.0f * Time.deltaTime);

        // 순찰 지점을 향해 이동
        cc.SimpleMove(patrolDirection * enemyMoveSpeed);
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
        moveDirection = (targetObject.transform.position - transform.position).normalized;

        // 타겟을 향해 바라보기
        // transform.LookAt(targetObject.transform);
        // transform.forward = Vector3.Lerp(transform.position, moveDirection, 1.0f * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(moveDirection), 10.0f * Time.deltaTime);

        // 타겟을 향해 이동
        cc.SimpleMove(moveDirection * enemyMoveSpeed);
    }
    private void Attack()
    {
        // 1. 플레이어가 공격 범위 안에 있다면 일정한 시간 간격으로 플레이어를 공격
        // 2. 플레이어가 공격 범위를 벗어나면 이동 상태(재추격)로 변경
        // 탐지 범위 : 0.0f 이상 ~ 1.5f 미만

        // 몬스터의 공격 딜레이값 증가
        attackDelayTimer += Time.deltaTime;

        // 공격 딜레이값이 기준값을 넘어섰을 경우
        if (attackDelayTimer > attackDelay)
        {
            // 플레이어 체력 감소
            // targetObject.GetComponent<PlayerHit>().PlayerHit(enemyAttackValue);
            Debug.Log("Enemy Attack");

            // 타이머 초기화
            attackDelayTimer = 0.0f;

            if (Vector3.Distance(transform.position, targetObject.transform.position) > attackRange)
            {
                enemyState = EnemyState.Move;
            }
        }
    }
    private void Return()
    {
        // 1. 몬스터가 플레이어를 추적하다가 너무 멀리 벗어났을 경우 최초 스폰 위치로 복귀
        // 탐지 범위 : 최초 스폰 위치에서 30.0f 이상

        // 몬스터의 이동 속도 재설정
        enemyMoveSpeed = 6.0f;

        // 타겟의 방향 구하기
        moveDirection = spawnPoint - transform.position;
        moveDirection.Normalize();

        // 최초 스폰 위치를 향해 이동
        cc.SimpleMove(moveDirection * enemyMoveSpeed);

        // 목표 지점을 향해 바라보기
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(moveDirection), 10.0f * Time.deltaTime);
    }
    // 몬스터 피격 시 데미지 계산 함수
    public void hitDamage(int value)
    {
        // 피격 상태이거나 사망 상태일 경우 데미지 계산 무시
        if (enemyState == EnemyState.Damaged || enemyState == EnemyState.Die)
        {
            return;
        }

        // 들어온 데미지(value) 만큼 몬스터의 체력 감소
        enemyHP -= value;

        // 몬스터의 체력이 1 이상일 경우 피격 상태로 전환
        if (enemyHP > 0)
        {
            enemyState = EnemyState.Damaged;
            Damaged();
        }
        // 몬스터의 체력이 0일 경우 사망 상태로 전환
        else
        {
            enemyState = EnemyState.Die;
            Die();
        }
    }
    private void Damaged()
    {
        // 1. 몬스터의 체력이 1 이상일 경우 피격 상태로 전환 후 다시 이전의 상태로 변경
        // 2. 트랜지션은 Any State 에서 연결

        // 피격 상태를 처리하기 위한 코루틴 실행
        StartCoroutine(DamageProc());
    }
    private void Die()
    {
        // 1. 몬스터의 체력이 0일 경우 사망 상태로 전환, 그 후 몬스터 오브젝트 삭제
        // 2. 트랜지션은 Any State 에서 연결

        // 진행중인 모든 코루틴 정지
        StopAllCoroutines();

        // 사망 상태를 처리하기 위한 코루틴 실행
        StartCoroutine(DieProc());
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
            yield return null;
        }
    }

    #endregion

    #region 몬스터 피격 데미지

    IEnumerator DamageProc()
    {
        // 피격 모션 시간만큼 대기
        yield return new WaitForSeconds(1.0f);

        // 현재 상태를 이동으로 전환
        enemyState = EnemyState.Move;
        print("상태전환 : Damaged -> Move");
    }

    #endregion

    #region 몬스터 사망

    IEnumerator DieProc()
    {
        // 캐릭터 컨트롤러 비활성화
        cc.enabled = false;

        // 2초 후 자기 자신을 제거
        yield return new WaitForSeconds(2.0f);
        print("죽었다!!");
        Destroy(gameObject);
    }

    #endregion

    #endregion

    #region 기즈모 출력

    private void OnDrawGizmos()
    {
        // 공격 가능한 범위
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // 플레이어 추적 범위
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, findRange);

        // 이동 가능한 최대 범위
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(spawnPoint, moveRange);
    }

    #endregion
}