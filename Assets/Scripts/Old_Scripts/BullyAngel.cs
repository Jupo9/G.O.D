using UnityEngine;

public class BullyAngel : Actions
{
    public override bool PrePerform()
    {
        // Suche nach allen Engeln in der Szene
        GameObject[] angels = GameObject.FindGameObjectsWithTag("Angel");
        if (angels.Length == 0) return false;

        // Finde den n�chstgelegenen Engel
        GameObject closestAngel = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject angel in angels)
        {
            float distance = Vector3.Distance(this.transform.position, angel.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestAngel = angel;
            }
        }

        // Wenn kein Engel gefunden wurde, beende die Aktion
        if (closestAngel == null) return false;

        // Setze das Ziel f�r den Agenten auf den n�chstgelegenen Engel
        target = closestAngel;
        agent.SetDestination(target.transform.position);

        return true;
    }

    public override bool PostPerform()
    {
        return true;
    }

}
