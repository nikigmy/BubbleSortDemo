using Sirenix.OdinInspector;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using Unity.Jobs;
using UnityEngine.Serialization;

public class InstancedSortController : MonoBehaviour
{
    //setup
    [SerializeField] private  Material material;
    [SerializeField] private  Mesh mesh;
    [SerializeField] private int instaces = 10000;
    
    //controls
    private int speed;
    private bool paused;
    public bool Paused => paused;
    
    //sort
    private BubbleSortNative sort;
    private int targetDelay;
    private int currentDelay;
    private int stepCount = 1;
    private int currentStepCount = 1;
    private bool finished;
    private float sortMultiplier = 1;
    private bool isinit;
    
    //rendering
    private GraphicsBuffer commandBuf;
    private GraphicsBuffer.IndirectDrawIndexedArgs[] commandData;
    private RenderParams rp;
    
    //animation
    private AnimationJob _job;
    private float animationSpeed;
    private float animationMultiplier = 200;
    
    //positions
    private ComputeBuffer positionsBuffer;
    private NativeArray<float3> positions;
    private NativeArray<int> positionsMap;
    
    //Colors
    private ComputeBuffer colorsBuffer;
    private Color[] colors;

    void Update()
    {
        DoRendering();
    }

    private void FixedUpdate()
    {
        DoSortLoop();
    }
    void OnDestroy()
    {
        commandBuf?.Release();
        commandBuf = null;
        positionsBuffer?.Release();
        positionsBuffer = null;
        colorsBuffer?.Release();
        colorsBuffer = null;
    }

    private void DoRendering()
    {
        if (!isinit)
            return;
        
        sort.elements = sort.elements;

        _job.WorldPositions = positions;
        _job.ElementPositions = positionsMap;
        _job.animationSpeed = Time.deltaTime * animationSpeed;
        _job.Schedule(instaces, 64).Complete();
        positionsBuffer.SetData(positions);
        rp.matProps.SetBuffer("positionBuffer", positionsBuffer);
        Graphics.RenderMeshIndirect(rp, mesh, commandBuf, 1);
    }
    
    private void DoSortLoop()
    {
        if (Paused || finished || !isinit)
            return;
        if (currentDelay > 0)
        {
            currentDelay--;
            return;
        }
        else
        {
            currentDelay = targetDelay;
        }

        currentStepCount = stepCount;
        while (!finished && currentStepCount > 0)
        {
            finished= sort.Step();
            currentStepCount--;
        }
    }

    public void UpdateSpeed(int multiplier)
    {
        speed = multiplier;
        sortMultiplier = multiplier * 0.01f;
        if (sortMultiplier < 0.5f)
        {
            targetDelay = (int)(0.5f / sortMultiplier);
            animationSpeed = (0.5f / targetDelay) * animationMultiplier;
            stepCount = 1;
        }
        else
        {
            if (sortMultiplier > 1.5f)
            {
                stepCount = (int)sortMultiplier;
            }
            else
            {
                stepCount = 1;
            }
            
            targetDelay = 0;
            animationSpeed = stepCount * animationMultiplier;
            currentDelay = 0;
        }
    }
    
    #region Init

    [Button]
    public void Init()
    {
        sort = new BubbleSortNative();
        
        commandBuf = new GraphicsBuffer(GraphicsBuffer.Target.IndirectArguments, 1, GraphicsBuffer.IndirectDrawIndexedArgs.size);
        commandData = new GraphicsBuffer.IndirectDrawIndexedArgs[1];
        commandData[0].indexCountPerInstance = mesh.GetIndexCount(0);
        commandData[0].instanceCount = (uint)instaces;
        commandBuf.SetData(commandData);
        
        positions = new NativeArray<float3>(instaces, Allocator.Persistent);
        positionsMap = new NativeArray<int>(instaces, Allocator.Persistent);
        
        sort.Init(instaces);
        InitPosMap(sort.elements);
        sort.posMap = positionsMap;
        InitPositions();
        
        rp = new RenderParams(material);
        rp.worldBounds = new Bounds(Vector3.zero, 10000*Vector3.one); // use tighter bounds for better FOV culling
        rp.matProps = new MaterialPropertyBlock();
        
        positionsBuffer = new ComputeBuffer(instaces, sizeof(float) * 3);
        colorsBuffer = new ComputeBuffer(instaces, sizeof(float) * 4);
        
        colors = new Color[instaces];
        CubeHelpers.GetColorsRedGreen(instaces, ref colors);
        colorsBuffer.SetData(colors);
        rp.matProps.SetBuffer("colorBuffer", colorsBuffer);

        _job = new AnimationJob
        {
            WorldPositions = positions,
            ElementPositions = sort.elements
        };
        UpdateSpeed(speed);
        isinit = true;
    }

    private void InitPosMap(NativeArray<int> elements)
    {
        for (int i = 0; i < instaces; i++)
        {
            var el = elements[i];
            positionsMap[el] = i;
        }
    }
    
    private void InitPositions()
    {
        float3 tempElement;
        for (int i = 0; i < instaces; i++)
        {
            tempElement = positions[i];
            CubeHelpers.CalculatePosition(positionsMap[i], ref tempElement);
            positions[i] = tempElement;
        }
    }
    #endregion

    #region Controls

    [Button]
    public void Restart()
    {
        sort.Shuffle();
        InitPosMap(sort.elements);
        InitPositions();
        if (Paused)
        {
            paused = false;
        }

        finished = false;
    }

    private string pauseLabel2 = "asdasd";
    [Button("@paused ? \"Play\" : \"Pause\"")]
    public void TogglePause()
    {
        paused = !Paused;
    }

    public void SetSpeed(int speed)
    {
        UpdateSpeed(speed);
    }

    #endregion
}