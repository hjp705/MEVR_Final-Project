using UnityEngine;

[DisallowMultipleComponent]
public sealed class BirdFlapPreview : MonoBehaviour
{
    [SerializeField, Range(0f, 1f)] private float intensity = 0.35f;

    private const float TurnRadius = 24f;
    private Transform leftWing;
    private Transform rightWing;
    private Vector3 start;
    private Quaternion startRotation;
    private float orbitAngle;
    private float flapPhase;
    private Material previewMaterial;

    private float FlapsPerSecond => 3.5f * intensity;
    private float FlightSpeed => 16f * intensity;

    private void Awake()
    {
        start = transform.position;
        startRotation = transform.rotation;
        GetComponent<Renderer>().enabled = false;

        previewMaterial = new Material(Shader.Find("Standard"));
        var green = new Color(0.16f, 0.76f, 0.35f);
        var teal = new Color(0.06f, 0.49f, 0.48f);
        var yellow = new Color(1f, 0.69f, 0.11f);
        var dark = new Color(0.07f, 0.12f, 0.17f);

        Part("Body", PrimitiveType.Sphere, transform, new Vector3(0, 0, 0), new Vector3(0.84f, 0.65f, 1.32f), green);
        Part("Head", PrimitiveType.Sphere, transform, new Vector3(0, 0.23f, 0.52f), new Vector3(0.57f, 0.58f, 0.58f), green);
        Part("Beak", PrimitiveType.Sphere, transform, new Vector3(0, 0.15f, 0.8f), new Vector3(0.25f, 0.22f, 0.31f), yellow);
        Part("Left Eye", PrimitiveType.Sphere, transform, new Vector3(-0.248f, 0.31f, 0.62f), Vector3.one * 0.09f, dark);
        Part("Right Eye", PrimitiveType.Sphere, transform, new Vector3(0.248f, 0.31f, 0.62f), Vector3.one * 0.09f, dark);
        var tail = Part("Tail", PrimitiveType.Cube, transform, new Vector3(0, -0.05f, -0.76f), new Vector3(0.38f, 0.09f, 0.85f), teal);
        tail.localRotation = Quaternion.Euler(-10f, 0, 0);
        leftWing = Wing("Left Wing", -1, teal, yellow);
        rightWing = Wing("Right Wing", 1, teal, yellow);
    }

    private Transform Wing(string name, int side, Color baseColor, Color tipColor)
    {
        var pivot = new GameObject(name).transform;
        pivot.SetParent(transform, false);
        pivot.localPosition = new Vector3(side * 0.31f, 0.08f, 0.04f);
        Part("Wing", PrimitiveType.Cube, pivot, new Vector3(side * 0.49f, 0, -0.04f), new Vector3(1.08f, 0.11f, 0.62f), baseColor)
            .localRotation = Quaternion.Euler(0, side * 12f, 0);
        Part("Tip", PrimitiveType.Cube, pivot, new Vector3(side * 1.13f, 0, -0.23f), new Vector3(0.52f, 0.08f, 0.38f), tipColor)
            .localRotation = Quaternion.Euler(0, side * 24f, 0);
        return pivot;
    }

    private Transform Part(string name, PrimitiveType type, Transform parent, Vector3 position, Vector3 scale, Color color)
    {
        var part = GameObject.CreatePrimitive(type);
        part.name = name;
        part.transform.SetParent(parent, false);
        part.transform.localPosition = position;
        part.transform.localScale = scale;
        var collider = part.GetComponent<Collider>();
        collider.enabled = false;
        Destroy(collider);
        var renderer = part.GetComponent<Renderer>();
        renderer.sharedMaterial = previewMaterial;
        var properties = new MaterialPropertyBlock();
        properties.SetColor("_Color", color);
        renderer.SetPropertyBlock(properties);
        return part.transform;
    }

    private void Update()
    {
        float dt = Time.deltaTime;
        flapPhase = Mathf.Repeat(flapPhase + FlapsPerSecond * 2f * Mathf.PI * dt, 2f * Mathf.PI);
        float wingAngle = intensity > 0f ? 38f * Mathf.Sin(flapPhase) : 0f;
        leftWing.localRotation = Quaternion.Euler(0, 0, -wingAngle);
        rightWing.localRotation = Quaternion.Euler(0, 0, wingAngle);

        orbitAngle = Mathf.Repeat(orbitAngle + FlightSpeed / TurnRadius * dt, 2f * Mathf.PI);
        var offset = new Vector3(TurnRadius * (1f - Mathf.Cos(orbitAngle)), 0f, TurnRadius * Mathf.Sin(orbitAngle));
        transform.SetPositionAndRotation(start + startRotation * offset,
            startRotation * Quaternion.Euler(0, orbitAngle * Mathf.Rad2Deg, 0));
    }

    private void OnGUI()
    {
        var area = new Rect(Mathf.Max(8f, Screen.width - 234f), 12f, 222f, 80f);
        GUILayout.BeginArea(area, GUI.skin.box);
        GUILayout.Label("Bird preview | Flap intensity");
        intensity = GUILayout.HorizontalSlider(intensity, 0f, 1f);
        GUILayout.Label($"{FlapsPerSecond:0.0} flaps/s    {FlightSpeed:0.0} m/s");
        GUILayout.EndArea();
    }

    private void OnDestroy()
    {
        Destroy(previewMaterial);
    }
}
