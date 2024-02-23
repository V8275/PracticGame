using UnityEngine;

public class CableComponent : MonoBehaviour
{
    #region Class members

    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform endPoint;
    [SerializeField] private Material cableMaterial;

    // Cable config
    [SerializeField] private float cableLength = 0.5f;
    [SerializeField] private int totalSegments = 5;
    [SerializeField] private float segmentsPerUnit = 2f;
    private int segments = 0;
    [SerializeField] private float cableWidth = 0.1f;

    // Solver config
    [SerializeField] private int verletIterations = 1;
    [SerializeField] private int solverIterations = 1;

    private LineRenderer line;
    private CableParticle[] points;

    #endregion


    #region Initial setup

    void Start()
    {
        InitCableParticles();
        InitLineRenderer();
    }

    void FixedUpdate()
    {
        RenderCable();
        for (int verletIdx = 0; verletIdx < verletIterations; verletIdx++)
        {
            VerletIntegrate();
            SolveConstraints();
        }
    }

    void InitCableParticles()
    {
        // Calculate segments to use
        if (totalSegments > 0)
            segments = totalSegments;
        else
            segments = Mathf.CeilToInt(cableLength * segmentsPerUnit);

        Vector3 cableDirection = (endPoint.position - transform.position).normalized;
        float initialSegmentLength = cableLength / segments;
        points = new CableParticle[segments + 1];

        // Foreach point
        for (int pointIdx = 0; pointIdx <= segments; pointIdx++)
        {
            // Initial position
            Vector3 initialPosition = transform.position + (cableDirection * (initialSegmentLength * pointIdx));
            points[pointIdx] = new CableParticle(initialPosition);
        }

        // Bind start and end particles with their respective gameobjects
        CableParticle start = points[0];
        CableParticle end = points[segments];
        start.Bind(startPoint.transform);
        end.Bind(endPoint.transform);
    }

    void InitLineRenderer()
    {
        line = gameObject.AddComponent<LineRenderer>();
        line.startWidth = cableWidth;
        line.endWidth = cableWidth;
        line.positionCount = segments + 1;
        line.material = cableMaterial;
        line.GetComponent<Renderer>().enabled = true;
    }

    #endregion


    #region Render Pass
    void RenderCable()
    {
        for (int pointIdx = 0; pointIdx < segments + 1; pointIdx++)
            line.SetPosition(pointIdx, points[pointIdx].Position);
    }

    #endregion


    #region Verlet integration & solver pass

    void VerletIntegrate()
    {
        Vector3 gravityDisplacement = Time.fixedDeltaTime * Time.fixedDeltaTime * Physics.gravity;
        foreach (CableParticle particle in points)
        {
            particle.UpdateVerlet(gravityDisplacement);
        }
    }

    void SolveConstraints()
    {
        for (int iterationIdx = 0; iterationIdx < solverIterations; iterationIdx++) SolveDistanceConstraint();
    }

    #endregion

    void SolveDistanceConstraint()
    {
        float segmentLength = cableLength / segments;
        for (int SegIdx = 0; SegIdx < segments; SegIdx++)
        {
            CableParticle particleA = points[SegIdx];
            CableParticle particleB = points[SegIdx + 1];

            // Solve for this pair of particles
            SolveDistanceConstraint(particleA, particleB, segmentLength);
        }
    }

    void SolveDistanceConstraint(CableParticle particleA, CableParticle particleB, float segmentLength)
    {
        // Find current vector between particles
        Vector3 delta = particleB.Position - particleA.Position;
        // 
        float currentDistance = delta.magnitude;
        float errorFactor = (currentDistance - segmentLength) / currentDistance;

        // Only move free particles to satisfy constraints
        if (particleA.IsFree() && particleB.IsFree())
        {
            particleA.Position += errorFactor * 0.5f * delta;
            particleB.Position -= errorFactor * 0.5f * delta;
        }
        else if (particleA.IsFree()) particleA.Position += errorFactor * delta;
        else if (particleB.IsFree()) particleB.Position -= errorFactor * delta;
    }
}

