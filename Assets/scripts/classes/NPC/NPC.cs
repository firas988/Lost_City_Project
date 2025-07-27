using UnityEngine;
using UnityEngine.AI;

public class NPC
{
    protected int id;
    private string name;
    private string type;
    private float walkRadius;
    private int areaMask;
    private Vector2 waitTimeRange;
    private string navMeshAreaName;
    private float speed;
    private float maxSpeed;

    public NPC(
        int id,
        string name,
        string type,
        float walkRadius,
        int areaMask,
        Vector2 waitTimeRange,
        string navMeshAreaName,
        float speed,
        float maxSpeed
    )
    {
        this.id = id;
        this.name = name;
        this.type = type;
        this.walkRadius = walkRadius;
        this.areaMask = areaMask;
        this.waitTimeRange = waitTimeRange;
        this.navMeshAreaName = navMeshAreaName;
        this.speed = speed;
        this.maxSpeed = maxSpeed;
    }

    public NPC(
        int id,
        string name,
        string type
    )
    {
        this.id = id;
        this.name = name;
        this.type = type;
        this.walkRadius = 0;
        this.areaMask = 0;
        this.waitTimeRange = new Vector2(0, 0);
        this.navMeshAreaName = "";
        this.speed = 0;
        this.maxSpeed = 0;
    }

    //getters and setters
    public int GetId()
    {
        return this.id;
    }

    public string GetName()
    {
        return this.name;
    }

    public Vector2 GetWaitingTimeRange()
    {
        return this.waitTimeRange;
    }

    public float GetWalkRadius()
    {
        return this.walkRadius;
    }

    public int GetAreaMask()
    {
        return this.areaMask;
    }

    public string GetNavMeshAreaName()
    {
        return this.navMeshAreaName;
    }

    public float GetSpeed()
    {
        return this.speed;
    }

    public float GetMaxSpeed()
    {
        return this.maxSpeed;
    }

   
}
