using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class DA_Evil : Actions
{
    [Header("Evil Settings")]
    [SerializeField] private float evilTime = 3f;
    [SerializeField] private float evilIncreasePerSecond = 10f;

    private Angel evilTarget;
    private Coroutine stunRotationCoroutine;

    public override bool PrePerform()
    {
        Devil devilScript = agentScriptReference as Devil;

        if (devilScript != null && devilScript.evil >= 0.8f)
        {
            Debug.Log("evil is enough, jump to next action");
            FinishAction();
            return false;
        }

        evilTarget = FindClosestAvailableAngel();

        if (evilTarget == null)
        {
            Debug.LogWarning("No evil target found!");
            return false;
        }

        target = evilTarget.gameObject;
        agent.SetDestination(target.transform.position);

        StartCoroutine(EvilRoutine());

        return true;
    }

    private Angel FindClosestAvailableAngel()
    {
        Angel[] allAngels = Object.FindObjectsByType<Angel>(FindObjectsSortMode.None);

        Angel closest = null;
        float minDist = float.MaxValue;

        foreach (var angel in allAngels)
        {
            if (!angel.isAvailable) continue;

            float dist = Vector3.Distance(transform.position, angel.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = angel;
            }
        }

        return closest;
    }

    private IEnumerator EvilRoutine()
    {
        while (Vector3.Distance(transform.position, target.transform.position) > 1.1f)
        {
            yield return null;
        }

        // STUN ANGEL
        if (evilTarget != null)
        {
            var angelAgent = evilTarget.GetComponent<NavMeshAgent>();
            if (angelAgent != null)
            {
                angelAgent.isStopped = true;
                angelAgent.speed = 0f;
            }

            if (evilTarget.stunEffect != null)
            {
                evilTarget.stunEffect.SetActive(true);
                stunRotationCoroutine = StartCoroutine(RotateStunEffect(evilTarget.stunEffect));
            }
        }

        Coroutine regenRoutine = StartCoroutine(IncreaseEvilOverTime(evilTime));

        yield return new WaitForSeconds(evilTime);

        if (regenRoutine != null)
            StopCoroutine(regenRoutine);

        if (evilTarget != null)
        {
            var angelAgent = evilTarget.GetComponent<NavMeshAgent>();
            if (angelAgent != null)
            {
                angelAgent.isStopped = false;
                angelAgent.speed = 3.5f;
            }

            if (evilTarget.stunEffect != null)
            {
                evilTarget.stunEffect.SetActive(false);
                if (stunRotationCoroutine != null)
                    StopCoroutine(stunRotationCoroutine);
            }
        }

        FinishAction();
    }

    private IEnumerator RotateStunEffect(GameObject effect)
    {
        while (true)
        {
            effect.transform.Rotate(Vector3.up * 180 * Time.deltaTime, Space.Self); 
            yield return null;
        }
    }

    private IEnumerator IncreaseEvilOverTime(float duration)
    {
        float timer = 0f;
        Devil devilScript = agentScriptReference as Devil;

        if (devilScript == null)
        {
            Debug.LogWarning("DA_Evil: agentScriptReference is not a Devil.");
            yield break;
        }

        while (timer < duration)
        {
            devilScript.evil += evilIncreasePerSecond * Time.deltaTime;
            devilScript.evil = Mathf.Clamp01(devilScript.evil);

            timer += Time.deltaTime;
            yield return null;
        }
    }

    public override bool PostPerform()
    {
        Debug.Log("Finished Action: " + actionName);
        return true;
    }
}
