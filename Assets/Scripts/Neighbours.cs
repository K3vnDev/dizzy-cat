using System.Collections.Generic;
using UnityEngine;

public class Neighbours
{
    public NavigationTarget left = null, right = null, top = null, bottom = null;

    readonly NavigationTarget main;
    readonly float threshold = 0.5f;

    public enum Axis { X, Y }

    public Neighbours(NavigationTarget main)
    {
        this.main = main;
    }

    public void Calculate(NavigationTarget[] targets)
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

        left = GetClosest(left_matches, Axis.X);
        right = GetClosest(right_matches, Axis.X);
        top = GetClosest(top_matches, Axis.Y);
        bottom = GetClosest(bottom_matches, Axis.Y);
    }

    public NavigationTarget GetClosest(List<NavigationTarget> matches, Axis axis)
    {
        // Prevent execution if the answer is obvious
        if (matches.Count == 0) return null;
        if (matches.Count == 1) return matches[0];

        List<NavigationTarget> sameAxisMatches = new();

        bool IsOnSameAxis(float a, float b) => Mathf.Abs(a - b) < threshold; 
        Vector2 mainPos = main.transform.position;

        // Find matches on the same axis
        foreach (NavigationTarget match in matches)
        {
            Vector2 matchPos = match.transform.position;

            if (axis == Axis.X && IsOnSameAxis(matchPos.y, mainPos.y))
                sameAxisMatches.Add(match);

            else if (axis == Axis.Y && IsOnSameAxis(matchPos.x, mainPos.x))
                sameAxisMatches.Add(match);
        }

        // If it couln't find any, select from the entire list
        if (sameAxisMatches.Count == 0)
            return GetClosest(matches);

        // Select from the same axis list
        return GetClosest(sameAxisMatches);
    }

    NavigationTarget GetClosest(List<NavigationTarget> matches)
    {
        NavigationTarget closest = matches[0];
        if (matches.Count == 1) return closest;

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
