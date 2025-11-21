using UnityEngine;

/// <summary>
/// Враг, который держит дистанцию и стреляет по кулдауну.
/// 3D версия.
/// </summary>
[RequireComponent(typeof(Rigidbody), typeof(Collider))] // гарантируем физику
public class EnemyController : MonoBehaviour
{
    [Header("Движение")]
    [SerializeField] private float minDistance = 4f;       // минимальная дистанция — ближе отходим
    [SerializeField] private float desiredDistance = 7f;   // стремимся держать эту дистанцию
    [SerializeField] private float maxDistance = 11f;      // дальше приближаемся

    [Header("Линия видимости и укрытия")]
    [SerializeField] private LayerMask obstacleMask = ~0; // слои препятствий для Raycast
    [SerializeField] private float coverSearchRadius = 12f; // радиус поиска укрытия
    [SerializeField] private float coverRepathInterval = 0.5f; // как часто пересчитывать укрытие
    [SerializeField] private float coverTriggerDistance = 9f;  // стремимся к укрытию, если близко к игроку

    [Header("Выглядывание из укрытия")]
    [SerializeField] private bool enablePeek = true;      // включить механику выглядывания
    [SerializeField] private float peekOffset = 1.1f;     // насколько выезжаем из укрытия (м)
    [SerializeField] private float peekDuration = 0.7f;   // сколько держим выглядывание (с)
    [SerializeField] private float peekCooldown = 1.3f;   // пауза между выглядываниями (с)
    [SerializeField] private LayerMask playerLayer;       // Слой игрока (для стрельбы)

    private TankMover _mover;
    private TurretController _turret;
    private WeaponController _weapon;
    
    private Transform target;                               // текущая цель (игрок)
    private CoverPoint currentCover;                        // выбранная точка укрытия
    private float nextCoverRepathTime;                      // тайминг пересчёта укрытий
    private bool isPeeking;                                 // сейчас выезжаем из укрытия
    private bool isRetreating;                              // возвращаемся в укрытие
    private Vector3 peekTargetPos;                          // цель выглядывания
    private float peekEndTime;                              // когда закончить выглядывание
    private float nextPeekAllowedTime;                      // когда можно снова выглянуть

    private void Awake()
    {
        _mover = GetComponent<TankMover>();
        _turret = GetComponent<TurretController>();
        _weapon = GetComponent<WeaponController>();

        // Ensure components exist
        if (_mover == null) _mover = gameObject.AddComponent<TankMover>();
        if (_turret == null) _turret = gameObject.AddComponent<TurretController>();
        if (_weapon == null) _weapon = gameObject.AddComponent<WeaponController>();
    }

    private void Update()
    {
        AcquireTarget();
        AimAndFire();
    }

    private void FixedUpdate()
    {
        MoveTowardsTarget();
    }

    private void AcquireTarget()
    {
        if (target != null) return;
        var player = FindObjectOfType<PlayerController>();
        if (player != null) target = player.transform;
    }

    private void AimAndFire()
    {
        if (target == null) return;

        // Delegate aiming
        if (_turret != null)
        {
            _turret.AimTowards(target.position);
        }

        // Check conditions for shooting
        // Note: We need muzzle position for LOS check. 
        // Ideally WeaponController should expose it, or we check from transform.
        // For simplicity, we'll check from transform position + up offset
        Vector3 checkPos = transform.position + Vector3.up * 0.5f;
        
        float dist = Vector3.Distance(transform.position, target.position);
        bool hasLOS = HasLineOfSight(checkPos, target.position, target);
        
        // We don't have 'fireDistance' anymore, let's assume a default or add it back if needed.
        // Let's use maxDistance as a rough guide or hardcode for now.
        float shootDist = 10f; 

        if (hasLOS && dist <= shootDist)
        {
            if (_weapon != null)
            {
                _weapon.TryShoot(playerLayer);
            }

            if (isPeeking && currentCover != null)
            {
                isPeeking = false;
                isRetreating = true;
            }
        }
    }

    private void MoveTowardsTarget()
    {
        if (target == null)
        {
            _mover.Stop();
            return;
        }

        Vector3 selfPos = transform.position;
        Vector3 targetPos = target.position;
        targetPos.y = selfPos.y;
        
        float dist = Vector3.Distance(selfPos, targetPos);

        bool wantCover = dist <= coverTriggerDistance && HasLineOfSight(selfPos, targetPos, target);
        if (wantCover && Time.time >= nextCoverRepathTime)
        {
            currentCover = FindBestCover(selfPos, targetPos);
            nextCoverRepathTime = Time.time + coverRepathInterval;
        }

        Vector3 moveDir = Vector3.zero;
        if (currentCover != null)
        {
            Vector3 coverPos = currentCover.transform.position;
            coverPos.y = selfPos.y;
            
            float coverReach = currentCover.reachRadius;
            float dToCover = Vector3.Distance(selfPos, coverPos);
            
            if (dToCover > coverReach && !isPeeking)
            {
                moveDir = (coverPos - selfPos).normalized;
            }
            else
            {
                if (!isPeeking && !isRetreating && enablePeek)
                {
                    bool losFromCover = HasLineOfSight(coverPos, targetPos, target);
                    // Hardcoded fireDistance check replacement
                    bool readyToPeek = Time.time >= nextPeekAllowedTime && Vector3.Distance(coverPos, targetPos) <= 10f; 
                    if (!losFromCover && readyToPeek)
                    {
                        Vector3 outDir = (targetPos - coverPos).normalized;
                        peekTargetPos = coverPos + outDir * peekOffset;
                        isPeeking = true;
                        isRetreating = false;
                        peekEndTime = Time.time + peekDuration;
                        nextPeekAllowedTime = Time.time + peekCooldown;
                    }
                }

                if (isPeeking)
                {
                    moveDir = (peekTargetPos - selfPos).normalized;
                    if (Time.time >= peekEndTime || (peekTargetPos - selfPos).sqrMagnitude < 0.04f)
                    {
                        isPeeking = false;
                        isRetreating = true;
                    }
                }
                else if (isRetreating)
                {
                    if (dToCover > coverReach * 0.5f)
                        moveDir = (coverPos - selfPos).normalized;
                    else
                    {
                        isRetreating = false;
                        moveDir = Vector3.zero;
                    }
                }
            }
        }
        else
        {
            if (dist < minDistance)
                moveDir = (selfPos - targetPos).normalized;
            else if (dist > maxDistance)
                moveDir = (targetPos - selfPos).normalized;
        }

        // Delegate movement
        if (_mover != null)
        {
            // TankMover expects Vector2 input (X, Y) which maps to (X, Z) world
            _mover.Move(new Vector2(moveDir.x, moveDir.z));
        }
    }

    private bool HasLineOfSight(Vector3 from, Vector3 to, Transform expectedTarget)
    {
        Vector3 dir = to - from;
        float dist = dir.magnitude;
        
        RaycastHit[] hits = Physics.RaycastAll(from, dir, dist, obstacleMask);
        
        float bestDist = float.PositiveInfinity;
        RaycastHit best = default;
        bool found = false;

        foreach (var h in hits)
        {
            if (h.collider.transform == transform || h.collider.transform.IsChildOf(transform)) continue;
            if (h.distance < bestDist)
            {
                bestDist = h.distance;
                best = h;
                found = true;
            }
        }

        if (!found) return true;
        
        Transform t = best.collider.transform;
        if (expectedTarget != null && (t == expectedTarget || t.IsChildOf(expectedTarget))) return true;
        
        return false;
    }

    private CoverPoint FindBestCover(Vector3 selfPos, Vector3 targetPos)
    {
        var all = FindObjectsOfType<CoverPoint>();
        CoverPoint best = null;
        float bestScore = float.PositiveInfinity;
        foreach (var cp in all)
        {
            float dSelf = Vector3.Distance(selfPos, cp.transform.position);
            if (dSelf > coverSearchRadius) continue;

            if (HasLineOfSight(cp.transform.position, targetPos, target))
                continue;

            float score = dSelf;
            if (score < bestScore)
            {
                bestScore = score;
                best = cp;
            }
        }
        return best;
    }
}
