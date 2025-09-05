using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Base class for all NPCs in the game.
/// Provides basic properties and functionality for navigation and identification.
/// </summary>
public class NPC
{
    #region Protected Fields

    /// <summary>
    /// Unique identifier for the NPC.
    /// </summary>
    protected int id;

    #endregion

    #region Private Fields

    /// <summary>
    /// Display name of the NPC.
    /// </summary>
    private string name;

    /// <summary>
    /// Type/category of the NPC.
    /// </summary>
    private string type;

    /// <summary>
    /// Radius within which the NPC can walk.
    /// </summary>
    private float walkRadius;

    /// <summary>
    /// Navigation area mask for the NPC.
    /// </summary>
    private int areaMask;

    /// <summary>
    /// Range of wait times between actions.
    /// </summary>
    private Vector2 waitTimeRange;

    /// <summary>
    /// Name of the navigation mesh area.
    /// </summary>
    private string navMeshAreaName;

    /// <summary>
    /// Current movement speed of the NPC.
    /// </summary>
    private float speed;

    /// <summary>
    /// Maximum movement speed of the NPC.
    /// </summary>
    private float maxSpeed;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new NPC with full navigation parameters.
    /// </summary>
    /// <param name="id">Unique identifier for the NPC.</param>
    /// <param name="name">Display name of the NPC.</param>
    /// <param name="type">Type/category of the NPC.</param>
    /// <param name="walkRadius">Radius within which the NPC can walk.</param>
    /// <param name="areaMask">Navigation area mask for the NPC.</param>
    /// <param name="waitTimeRange">Range of wait times between actions.</param>
    /// <param name="navMeshAreaName">Name of the navigation mesh area.</param>
    /// <param name="speed">Current movement speed.</param>
    /// <param name="maxSpeed">Maximum movement speed.</param>
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
        // COMPLEXITY ANALYSIS: NPC() - O(1)
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

    /// <summary>
    /// Initializes a new NPC with basic identification only.
    /// Navigation parameters are set to default values.
    /// </summary>
    /// <param name="id">Unique identifier for the NPC.</param>
    /// <param name="name">Display name of the NPC.</param>
    /// <param name="type">Type/category of the NPC.</param>
    public NPC(int id, string name, string type)
    {
        // COMPLEXITY ANALYSIS: NPC() - O(1)
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

    #endregion

    #region Public Methods

    /// <summary>
    /// Gets the unique identifier of the NPC.
    /// </summary>
    /// <returns>The NPC's ID.</returns>
    public int GetId()
    {
        // COMPLEXITY ANALYSIS: GetId() - O(1)
        return this.id;
    }

    /// <summary>
    /// Gets the display name of the NPC.
    /// </summary>
    /// <returns>The NPC's name.</returns>
    public string GetName()
    {
        // COMPLEXITY ANALYSIS: GetName() - O(1)
        return this.name;
    }

    /// <summary>
    /// Gets the range of wait times between actions.
    /// </summary>
    /// <returns>The wait time range as a Vector2.</returns>
    public Vector2 GetWaitingTimeRange()
    {
        // COMPLEXITY ANALYSIS: GetWaitingTimeRange() - O(1)
        return this.waitTimeRange;
    }

    /// <summary>
    /// Gets the walk radius of the NPC.
    /// </summary>
    /// <returns>The walk radius value.</returns>
    public float GetWalkRadius()
    {
        // COMPLEXITY ANALYSIS: GetWalkRadius() - O(1)
        return this.walkRadius;
    }

    /// <summary>
    /// Gets the area mask for navigation.
    /// </summary>
    /// <returns>The area mask value.</returns>
    public int GetAreaMask()
    {
        // COMPLEXITY ANALYSIS: GetAreaMask() - O(1)
        return this.areaMask;
    }

    /// <summary>
    /// Gets the navigation mesh area name.
    /// </summary>
    /// <returns>The navigation mesh area name.</returns>
    public string GetNavMeshAreaName()
    {
        // COMPLEXITY ANALYSIS: GetNavMeshAreaName() - O(1)
        return this.navMeshAreaName;
    }

    /// <summary>
    /// Gets the current movement speed of the NPC.
    /// </summary>
    /// <returns>The current speed value.</returns>
    public float GetSpeed()
    {
        // COMPLEXITY ANALYSIS: GetSpeed() - O(1)
        return this.speed;
    }

    /// <summary>
    /// Gets the maximum movement speed of the NPC.
    /// </summary>
    /// <returns>The maximum speed value.</returns>
    public float GetMaxSpeed()
    {
        // COMPLEXITY ANALYSIS: GetMaxSpeed() - O(1)
        return this.maxSpeed;
    }

    #endregion
}
