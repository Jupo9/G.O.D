using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class AA_Social : Actions
{
    [Header("Social Settings")]
    [SerializeField] private float socialTime = 3f;
    [SerializeField] private float socialIncreasePerSecond = 10f;

    private Angel socialBuddy;
    private NavMeshAgent buddiesAgent;

    public override bool PrePerform()
    {
        Angel angelScript = agentScriptReference as Angel;

        if (angelScript != null && angelScript.social >= 0.8f)
        {
            Debug.Log("social is enough, jump to next action");
            FinishAction();
            return false;
        }

        socialBuddy = FindClosestAvailableAngel(angelScript);

        if (socialBuddy == null)
        {
            Debug.LogWarning("No buddy found! :(");
            return false;
        }
        
        //socialBuddy.isAvailable = false;
        buddiesAgent = socialBuddy.GetComponent<NavMeshAgent>();

        target = socialBuddy.gameObject;
        agent.SetDestination(target.transform.position);

        StartCoroutine(SocialRoutine());

        return true;
    }

    private Angel FindClosestAvailableAngel(Angel self)
    {
        Angel[] allAngels = Object.FindObjectsByType<Angel>(FindObjectsSortMode.None);

        Angel closest = null;
        float minDist = float.MaxValue;

        foreach (var angel in allAngels)
        {
            if (angel == self || !angel.isAvailable) continue;

            float dist = Vector3.Distance(transform.position, angel.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = angel;
            }
        }

        return closest;
    }

    private IEnumerator SocialRoutine()
    {
        while (Vector3.Distance(transform.position, target.transform.position) > 1.5f)
        {
            yield return null;
        }

        if (buddiesAgent != null)
        {
            buddiesAgent.isStopped = true;
            buddiesAgent.updatePosition = true;
            buddiesAgent.updateRotation = true;
            buddiesAgent.velocity = Vector3.zero;
            buddiesAgent.speed = 0f;
        }

        Vector3 toSelf = transform.position - socialBuddy.transform.position;
        toSelf.y = 0;
        if (toSelf != Vector3.zero)
        {
            StartCoroutine(RotateBuddyToFace(transform.position, 1f));
        }

        Coroutine regenRoutine = StartCoroutine(IncreaseSocialOverTime(socialTime));

        agent.isStopped = true;

        yield return new WaitForSeconds(socialTime);

        agent.isStopped = false;

        if (regenRoutine != null)
        {
            StopCoroutine(regenRoutine);
        }

        if (buddiesAgent != null)
        {
            buddiesAgent.isStopped = false;
            buddiesAgent.speed = 3.5f;
        }

        /*if (socialBuddy != null)
        {
            socialBuddy.isAvailable = true;
        }*/

        FinishAction();
    }

    private IEnumerator RotateBuddyToFace(Vector3 targetPosition, float duration)
    {
        Quaternion startRot = socialBuddy.transform.rotation;
        Vector3 direction = (targetPosition - socialBuddy.transform.position).normalized;
        direction.y = 0f; 
        Quaternion targetRot = Quaternion.LookRotation(direction);

        float elapsed = 0f;

        while (elapsed < duration)
        {
            if (socialBuddy == null) yield break;

            socialBuddy.transform.rotation = Quaternion.Slerp(startRot, targetRot, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        socialBuddy.transform.rotation = targetRot;
    }

    private IEnumerator IncreaseSocialOverTime(float duration)
    {
        float timer = 0f;
        Angel angelScript = agentScriptReference as Angel;

        if (angelScript == null)
        {
            Debug.LogWarning("AA_Social: agentScriptReference is not an Angel.");
            yield break;
        }

        while (timer < duration)
        {
            angelScript.social += socialIncreasePerSecond * Time.deltaTime;
            angelScript.social = Mathf.Clamp01(angelScript.social);

            socialBuddy.social += socialIncreasePerSecond * Time.deltaTime;
            socialBuddy.social = Mathf.Clamp01(socialBuddy.social);

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
