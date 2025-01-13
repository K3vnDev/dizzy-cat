using System.Collections.Generic;
using UnityEngine;

public class Neighbours
{
    public NavigationTarget left = null, right = null, top = null, bottom = null;
    readonly NavigationTarget main;

    public Neighbours(NavigationTarget main)
    {
        this.main = main;
    }

    public void Calculate(NavigationTarget[] targets, float threshold = 0.5f)
    {
        List<NavigationTarget>
            left_matches = new(), right_matches = new(), top_matches = new(), bottom_matches = new();

        foreach (NavigationTarget target in targets)
        {
            if (target == main) continue;

            Vector2 target_pos = target.transform.position,
                main_pos = main.transform.position;

            float x_diff = target_pos.x - main_pos.x,
                y_diff = target_pos.y - main_pos.y;

            if (x_diff < -threshold)
                left_matches.Add(target);

            if (x_diff > threshold)
                right_matches.Add(target);

            if (y_diff < -threshold)
                bottom_matches.Add(target);

            if (y_diff > threshold)
                top_matches.Add(target);
        }

        left = GetClosest(left_matches);
        right = GetClosest(right_matches);
        top = GetClosest(top_matches);
        bottom = GetClosest(bottom_matches);
    }

    public NavigationTarget GetClosest(List<NavigationTarget> matches)
    {
        if (matches.Count == 0) return null;

        NavigationTarget closest = matches[0];
        float closestDistance = Vector2.Distance(main.transform.position, closest.transform.position);

        foreach (NavigationTarget match in matches)
        {
            float distance = Vector2.Distance(main.transform.position, match.transform.position);
            if (distance < closestDistance)
            {
                closest = match;
                closestDistance = distance;
            }
        }
        return closest;
    }
}
