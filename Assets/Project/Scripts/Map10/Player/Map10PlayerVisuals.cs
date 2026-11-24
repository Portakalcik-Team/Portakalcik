using UnityEngine;

[ExecuteAlways]
public class Map10PlayerVisuals : MonoBehaviour
{
    [Header("Karakter Renkleri")]
    [SerializeField] private Color bodyColor = new Color(0.2f, 0.6f, 0.9f);    // Şık mavi tulum/kıyafet
    [SerializeField] private Color skinColor = new Color(1.0f, 0.85f, 0.72f); // Tatlı ten rengi
    [SerializeField] private Color eyeColor = new Color(0.1f, 0.1f, 0.15f);   // Canlı koyu gözler
    [SerializeField] private Color shoeColor = new Color(0.3f, 0.2f, 0.15f);  // Ayakkabı kahverengisi
    [SerializeField] private Color leafColor = new Color(0.25f, 0.8f, 0.3f);  // Başındaki tatlı yaprak
    [SerializeField] private Color beltColor = new Color(0.35f, 0.2f, 0.1f);  // Deri kemer

    private Transform visualContainer;
    private Transform leftArm, rightArm;
    private Transform leftFoot, rightFoot;
    private Transform head;
    private Rigidbody rb;

    void OnEnable()
    {
        SetupVisuals();
    }

    void Start()
    {
        SetupVisuals();
    }

    private void SetupVisuals()
    {
        rb = GetComponent<Rigidbody>();

        // Player'ın eski kaba küp MeshRenderer'ını gizle
        MeshRenderer baseRend = GetComponent<MeshRenderer>();
        if (baseRend != null) baseRend.enabled = false;

        // Eski nose varsa gizle (kendi sevimli yüzümüzü yapacağız)
        Transform oldNose = transform.Find("Nose");
        if (oldNose != null)
        {
            MeshRenderer noseRend = oldNose.GetComponent<MeshRenderer>();
            if (noseRend != null) noseRend.enabled = false;
        }

        // Zaten oluşturulmuşsa tekrar oluşturma
        Transform existing = transform.Find("CharacterVisuals");
        if (existing != null)
        {
            visualContainer = existing;
            leftArm = existing.Find("LeftArm");
            rightArm = existing.Find("RightArm");
            leftFoot = existing.Find("LeftFoot");
            rightFoot = existing.Find("RightFoot");
            head = existing.Find("Head");
            return;
        }

        // Yeni karakter görselini oluştur
        BuildCharacterVisuals();
    }

    private void BuildCharacterVisuals()
    {
        GameObject container = new GameObject("CharacterVisuals");
        visualContainer = container.transform;
        visualContainer.SetParent(transform);
        visualContainer.localPosition = Vector3.zero;
        visualContainer.localRotation = Quaternion.identity;

        // Player'ın scale'i (0.5, 1.5, 0.5) olduğu için parçaların basık görünmemesi amacıyla
        // container scale'ini normalize ediyoruz
        visualContainer.localScale = new Vector3(1f / transform.localScale.x, 1f / transform.localScale.y, 1f / transform.localScale.z);

        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null) shader = Shader.Find("Standard");

        Material bodyMat = new Material(shader) { color = bodyColor };
        Material skinMat = new Material(shader) { color = skinColor };
        Material eyeMat = new Material(shader) { color = eyeColor };
        Material shoeMat = new Material(shader) { color = shoeColor };
        Material leafMat = new Material(shader) { color = leafColor };
        Material beltMat = new Material(shader) { color = beltColor };

        // 1. Gövde (Torso)
        GameObject torso = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        torso.name = "Torso";
        torso.transform.SetParent(visualContainer);
        torso.transform.localPosition = new Vector3(0f, 0.65f, 0f);
        torso.transform.localScale = new Vector3(0.55f, 0.45f, 0.45f);
        StripCollider(torso);
        torso.GetComponent<Renderer>().material = bodyMat;

        // 2. Kemer Çemberi (Belt Band - Mandalinaların takıldığı görsel zemin)
        GameObject belt = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        belt.name = "BeltBand";
        belt.transform.SetParent(visualContainer);
        belt.transform.localPosition = new Vector3(0f, 0.55f, 0f);
        belt.transform.localScale = new Vector3(0.58f, 0.06f, 0.48f);
        StripCollider(belt);
        belt.GetComponent<Renderer>().material = beltMat;

        // 3. Kafa (Head)
        GameObject headObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        headObj.name = "Head";
        head = headObj.transform;
        head.SetParent(visualContainer);
        head.localPosition = new Vector3(0f, 1.25f, 0f);
        head.localScale = new Vector3(0.58f, 0.52f, 0.52f);
        StripCollider(headObj);
        headObj.GetComponent<Renderer>().material = skinMat;

        // 4. Gözler (Eyes)
        CreateEye(head, new Vector3(0.12f, 0.06f, 0.22f), eyeMat);
        CreateEye(head, new Vector3(-0.12f, 0.06f, 0.22f), eyeMat);

        // 5. Sevimli Burun
        GameObject cuteNose = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        cuteNose.name = "CuteNose";
        cuteNose.transform.SetParent(head);
        cuteNose.transform.localPosition = new Vector3(0f, -0.02f, 0.25f);
        cuteNose.transform.localScale = new Vector3(0.09f, 0.08f, 0.1f);
        StripCollider(cuteNose);
        cuteNose.GetComponent<Renderer>().material = skinMat;

        // 6. Başındaki Portakalcık Yaprağı (Temaya uygun sevimli filiz)
        GameObject leaf = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        leaf.name = "HeadLeaf";
        leaf.transform.SetParent(head);
        leaf.transform.localPosition = new Vector3(0.04f, 0.32f, 0.02f);
        leaf.transform.localScale = new Vector3(0.18f, 0.08f, 0.12f);
        leaf.transform.localRotation = Quaternion.Euler(20f, 30f, 15f);
        StripCollider(leaf);
        leaf.GetComponent<Renderer>().material = leafMat;

        // 7. Kollar (Arms)
        GameObject lArm = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        lArm.name = "LeftArm";
        leftArm = lArm.transform;
        leftArm.SetParent(visualContainer);
        leftArm.localPosition = new Vector3(-0.35f, 0.7f, 0f);
        leftArm.localScale = new Vector3(0.16f, 0.26f, 0.16f);
        leftArm.localRotation = Quaternion.Euler(0f, 0f, 15f);
        StripCollider(lArm);
        lArm.GetComponent<Renderer>().material = bodyMat;

        GameObject rArm = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        rArm.name = "RightArm";
        rightArm = rArm.transform;
        rightArm.SetParent(visualContainer);
        rightArm.localPosition = new Vector3(0.35f, 0.7f, 0f);
        rightArm.localScale = new Vector3(0.16f, 0.26f, 0.16f);
        rightArm.localRotation = Quaternion.Euler(0f, 0f, -15f);
        StripCollider(rArm);
        rArm.GetComponent<Renderer>().material = bodyMat;

        // 8. Ayaklar (Feet)
        GameObject lFoot = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        lFoot.name = "LeftFoot";
        leftFoot = lFoot.transform;
        leftFoot.SetParent(visualContainer);
        leftFoot.localPosition = new Vector3(-0.16f, 0.18f, 0.03f);
        leftFoot.localScale = new Vector3(0.2f, 0.16f, 0.28f);
        StripCollider(lFoot);
        lFoot.GetComponent<Renderer>().material = shoeMat;

        GameObject rFoot = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        rFoot.name = "RightFoot";
        rightFoot = rFoot.transform;
        rightFoot.SetParent(visualContainer);
        rightFoot.localPosition = new Vector3(0.16f, 0.18f, 0.03f);
        rightFoot.localScale = new Vector3(0.2f, 0.16f, 0.28f);
        StripCollider(rFoot);
        rFoot.GetComponent<Renderer>().material = shoeMat;
    }

    private void CreateEye(Transform parent, Vector3 localPos, Material mat)
    {
        GameObject eye = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        eye.name = "Eye";
        eye.transform.SetParent(parent);
        eye.transform.localPosition = localPos;
        eye.transform.localScale = new Vector3(0.08f, 0.11f, 0.08f);
        StripCollider(eye);
        eye.GetComponent<Renderer>().material = mat;
    }

    private void StripCollider(GameObject obj)
    {
        Collider c = obj.GetComponent<Collider>();
        if (c != null)
        {
            if (Application.isPlaying) Destroy(c);
            else DestroyImmediate(c);
        }
    }

    void Update()
    {
        // Prosedürel yürüme animasyonu (Waddle & Bobbing)
        float speed = 0f;
        if (rb != null)
        {
            Vector3 v = rb.linearVelocity;
            speed = new Vector2(v.x, v.z).magnitude;
        }

        if (speed > 0.1f)
        {
            float walkTime = Time.time * 12f;

            // Sağa sola tatlı sallanma
            float tilt = Mathf.Sin(walkTime) * 6f;
            if (visualContainer != null)
            {
                visualContainer.localRotation = Quaternion.Euler(0f, 0f, tilt);
            }

            // Kolların ve ayakların salınımı
            float legSwing = Mathf.Sin(walkTime) * 25f;
            if (leftFoot != null) leftFoot.localPosition = new Vector3(-0.16f, 0.18f + Mathf.Max(0, Mathf.Sin(walkTime) * 0.08f), Mathf.Sin(walkTime) * 0.12f);
            if (rightFoot != null) rightFoot.localPosition = new Vector3(0.16f, 0.18f + Mathf.Max(0, -Mathf.Sin(walkTime) * 0.08f), -Mathf.Sin(walkTime) * 0.12f);

            if (leftArm != null) leftArm.localRotation = Quaternion.Euler(-legSwing * 0.8f, 0f, 15f);
            if (rightArm != null) rightArm.localRotation = Quaternion.Euler(legSwing * 0.8f, 0f, -15f);
        }
        else
        {
            // Dururken tatlı nefes alma salınımı
            float breath = Mathf.Sin(Time.time * 2.5f) * 1.5f;
            if (visualContainer != null)
            {
                visualContainer.localRotation = Quaternion.Euler(breath, 0f, 0f);
            }

            if (leftFoot != null) leftFoot.localPosition = new Vector3(-0.16f, 0.18f, 0.03f);
            if (rightFoot != null) rightFoot.localPosition = new Vector3(0.16f, 0.18f, 0.03f);
            if (leftArm != null) leftArm.localRotation = Quaternion.Euler(0f, 0f, 15f);
            if (rightArm != null) rightArm.localRotation = Quaternion.Euler(0f, 0f, -15f);
        }
    }
}
