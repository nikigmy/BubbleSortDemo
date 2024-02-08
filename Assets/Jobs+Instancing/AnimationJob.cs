using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

[BurstCompile]
public struct AnimationJob : IJobParallelFor
{
    public float animationSpeed;
    public NativeArray<float3> WorldPositions;
    public NativeArray<int> ElementPositions;
    
    private float3 targetPosition;
    private float3 currentPosition;
    private float deltaX;
    private float deltaY;
    private float deltaZ;
    private float totaldelta;
    private float deltaSqrt;
    
    public void Execute(int index)
    {
        currentPosition = WorldPositions[index];
        CubeHelpers.CalculatePosition(ElementPositions[index], ref targetPosition);
        
        deltaX = targetPosition.x - currentPosition.x;
        deltaY = targetPosition.y - currentPosition.y;
        deltaZ = targetPosition.z - currentPosition.z;
        totaldelta = (deltaX * deltaX + deltaY * deltaY + deltaZ * deltaZ);
        if (totaldelta == 0.0 || animationSpeed >= 0.0 && totaldelta <= animationSpeed * animationSpeed)
        {
            currentPosition = targetPosition;
        }
        else
        {
            deltaSqrt = (float) Math.Sqrt(totaldelta);
            currentPosition.x += deltaX / deltaSqrt * animationSpeed;
            currentPosition.y += deltaY / deltaSqrt * animationSpeed;
            currentPosition.z += deltaZ / deltaSqrt * animationSpeed;
        }

        WorldPositions[index] = currentPosition;
     }
}