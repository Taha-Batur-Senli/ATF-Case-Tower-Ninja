using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

//[ExecuteInEditMode]
public class enemyScript : MonoBehaviour
{
    [SerializeField] ParticleSystem gun;
    [SerializeField] GameObject bullet;
    [SerializeField] VisionCone cone;
    [SerializeField] public int rotRightAngle = 150;
    [SerializeField] public int rotLeftAngle = 210;
    [SerializeField] public TextMeshProUGUI textUI;
    float leftPoint;
    float rightPoint;
    public float distance = 10;
    public float angle = 30;
    public float height = 1.0f;
    public Color meshColor = Color.red;
    public int scanFrequency = 30;
    public LayerMask layers;
    public LayerMask occlusionLayers;
    bool seenApriori = false;
    public float movementSpeed = 0.5f;
    public float returnSpeed = 100f;
    Vector3 ogPos;
    Vector3 ogRot;
    bool left = false;
    bool right = false;
    int cnt = 0;
    bool add = false;
    int ss = 0;
    bool returned = true;

    public List<GameObject> Objects = new List<GameObject>();
    Collider[] colliders = new Collider[50];
    Mesh mesh;
    int count;
    float scanInterval;
    float scanTimer;
    bool shootable;
    public bool over = false;
    bool back = false;

    // Start is called before the first frame update
    void Start()
    {
        textUI.text = "";
        ogPos = gameObject.transform.position;
        ogRot = gameObject.transform.rotation.eulerAngles;
        shootable = true;
        scanInterval = 1.0f / scanFrequency;
    }

    // Update is called once per frame
    void Update()
    {
        scanTimer -= Time.deltaTime;
        if (scanTimer < 0 && !over)
        {
            scanTimer += scanInterval;
            Scan();
        }

        if (returned)
        {
            if (cnt == 3)
            {
                cnt++;
                ss++;
            }

            if (cnt < 3)
            {
                StartCoroutine(Look());
            }
            else
            {
                StartCoroutine(turn());
            }

            if (add && cnt < 3)
            {
                add = false;
                cnt++;
            }
        }
        else
        {
            left = false;
            right = false;
            cnt = 0;
            add = false;
            ss = 0;

            if (over)
            {
                gun.Stop();
            }

            if (seenApriori)
            {
                StartCoroutine(Chase());
            }

            if (back && gameObject.transform.position == ogPos)
            {
                back = false;
                StartCoroutine(returnToRot());
            }

            if (back)
            {
                var lookPos = ogPos - transform.position;
                lookPos.y = 0;
                var rotation = Quaternion.LookRotation(lookPos);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime);

                transform.position = Vector3.MoveTowards(transform.position, ogPos, returnSpeed * Time.deltaTime);
            }
        }
    }

    IEnumerator turn()
    {
        yield return new WaitForSeconds(1f);

        //Declare a yield instruction.
        WaitForSeconds wait = new WaitForSeconds(0.2f);

        //first debug gives 0, second one gives 210 something, turns without stopping now

        //Look around works now, but the look function calls itself for the front, add a bool inside for back and forth
        //same goes for the turn, make it turn back as well after going forward

        while ((int)transform.rotation.eulerAngles.y > Math.Abs(ogRot.y - (180 * (ss % 2))))
        {
            if(ss % 2 == 1)
            {
                transform.rotation *= Quaternion.Euler(Vector3.down * Time.deltaTime * 8f);
            }
            else
            {
                transform.rotation *= Quaternion.Euler(Vector3.up * Time.deltaTime * 8f);
            }
            yield return wait;
        }

        yield return new WaitForSeconds(1f);

        cnt = 0;
    }

    IEnumerator Look()
    {
        if(left && right)
        {
            left = false;
            right = false;
        }
        else
        {
            if (ss % 2 == 1)
            {
                rotRightAngle = 330;
                rotLeftAngle = 30;
            }
            else
            {
                rotRightAngle = 150;
                rotLeftAngle = 210;
            }

            yield return new WaitForSeconds(1f);

            //Declare a yield instruction.
            WaitForSeconds wait = new WaitForSeconds(0.2f);

            while ((int)transform.rotation.eulerAngles.y != rotLeftAngle && !left)
            {
                transform.rotation *= Quaternion.Euler(Vector3.up * Time.deltaTime * 8f);
                yield return wait;
            }

            left = true;
            yield return new WaitForSeconds(1f);

            while ((int)transform.rotation.eulerAngles.y != rotRightAngle && !right)
            {
                transform.rotation *= Quaternion.Euler(Vector3.down * Time.deltaTime * 4f);
                yield return wait;
            }
            yield return new WaitForSeconds(1f);

            right = true;


            if ((int)transform.rotation.eulerAngles.y == rotLeftAngle )
            {
                add = true;
            }

        }
    }

    IEnumerator returnToRot()
    {
        seenApriori = false;

        //Declare a yield instruction.
        WaitForSeconds wait = new WaitForSeconds(0.00000002f);

        while((int) transform.rotation.eulerAngles.y != ogRot.y)
        {
            if(transform.rotation.eulerAngles.y > 180)
            {
                transform.rotation *= Quaternion.Euler(Vector3.down * Time.deltaTime * 100f);
                yield return wait;
            }
            else
            {
                transform.rotation *= Quaternion.Euler(Vector3.up * Time.deltaTime * 100f);
                yield return wait;
            }
        }

        returned = true;
        textUI.text = "";
    }

    IEnumerator Chase()
    {
        float time = 0;

        do
        {
            transform.position += transform.forward * movementSpeed * Time.deltaTime;
            yield return new WaitForEndOfFrame();

            time += Time.deltaTime;
        } while (time < 0.02f);

        time = 0.0f;
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(0.5f);

        shootable = true;
    }

    public bool IsInSight(GameObject obj)
    {
        Vector3 origin = transform.position;
        Vector3 dest = obj.transform.position;
        Vector3 direction = dest - origin;

        /*if(direction.y < 0 || direction.y  > height)
        {
            return false;
        }*/

        direction.y = 0;
        float deltaAngle = Vector3.Angle(direction, transform.forward);

        if(deltaAngle > angle)
        {
            return false;
        }

        origin.y += height / 2;
        dest.y = origin.y;
        if(Physics.Linecast(origin, dest, occlusionLayers))
        {
            return false;
        }

        return true;
    }

    private void Scan()
    {
        count = Physics.OverlapSphereNonAlloc(transform.position, distance, colliders, layers, QueryTriggerInteraction.Collide);

        Objects.Clear();
        for(int i = 0; i < count; ++i)
        {
            GameObject obj = colliders[i].gameObject;
            if(IsInSight(obj))
            {
                Objects.Add(obj);
                gun.Play();

                Vector3 looker = obj.transform.position;
                looker.y = gameObject.transform.position.y;

                textUI.text = "!";
                gameObject.transform.LookAt(looker);
                cone.transform.GetComponent<MeshRenderer>().material = cone.VisionConeMaterialSighted;

                if (shootable)
                {
                    seenApriori = true;
                    returned = false;
                    StartCoroutine(Wait());
                    Shoot(obj);
                }
            }
        }

        if(Objects.Count == 0)
        {
            gun.Stop();
            cone.transform.GetComponent<MeshRenderer>().material = cone.VisionConeMaterial;

            if (seenApriori)
            {
                textUI.text = "?";
                leftPoint = transform.rotation.eulerAngles.y + 30;
                rightPoint = transform.rotation.eulerAngles.y - 30;
                StartCoroutine(Forget());
            }
        }
    }

    IEnumerator Forget()
    {
        yield return new WaitForSeconds(1f);
        seenApriori = false;

        //Declare a yield instruction.
        WaitForSeconds wait = new WaitForSeconds(0.00000002f);

        while(transform.rotation.eulerAngles.y < leftPoint)
        {
            transform.rotation *= Quaternion.Euler(Vector3.up * Time.deltaTime * 5f);
            yield return wait;
        }

        yield return new WaitForSeconds(1f);

        while (transform.rotation.eulerAngles.y > rightPoint)
        {
            transform.rotation *= Quaternion.Euler(Vector3.down * Time.deltaTime * 5f);
            yield return wait;
        }

        yield return new WaitForSeconds(1f);

        back = true;
    }

    private void Shoot(GameObject obj)
    {
        GameObject created = Instantiate(bullet, transform.position, Quaternion.identity);

        created.GetComponent<bullet>().Shoot(obj, gameObject);

        shootable = false;
    }

    Mesh CreateWedgeMesh()
    {
        Mesh mesh = new Mesh();

        int segments = 10;
        int numTriangles = (segments * 4) + 2 + 2;
        int numVertices = numTriangles * 3;

        Vector3[] vertices = new Vector3[numVertices];
        int[] triangles = new int[numVertices];

        Vector3 bottomCenter = Vector3.zero;
        Vector3 bottomLeft = Quaternion.Euler(0f, -angle, 0f) * Vector3.forward * distance;
        Vector3 bottomRight = Quaternion.Euler(0f, angle, 0f) * Vector3.forward * distance;

        Vector3 topCenter = bottomCenter + Vector3.up * height;
        Vector3 topRight = bottomRight + Vector3.up * height;
        Vector3 topLeft = bottomLeft + Vector3.up * height;

        int vert = 0;

        //left side
        vertices[vert++] = bottomCenter;
        vertices[vert++] = bottomLeft;
        vertices[vert++] = topLeft;


        vertices[vert++] = topLeft;
        vertices[vert++] = topCenter;
        vertices[vert++] = bottomCenter;

        //right side
        vertices[vert++] = bottomCenter;
        vertices[vert++] = topCenter;
        vertices[vert++] = topRight;

        vertices[vert++] = topRight;
        vertices[vert++] = bottomRight;
        vertices[vert++] = bottomCenter;

        float currentAngle = -angle;
        float deltaAngle = (angle * 2) / segments;

        for(int i = 0; i < segments; ++i)
        {
            bottomLeft = Quaternion.Euler(0f, currentAngle, 0f) * Vector3.forward * distance;
            bottomRight = Quaternion.Euler(0f, currentAngle + deltaAngle, 0f) * Vector3.forward * distance;

            topRight = bottomRight + Vector3.up * height;
            topLeft = bottomLeft + Vector3.up * height;

            //far side
            vertices[vert++] = bottomLeft;
            vertices[vert++] = bottomRight;
            vertices[vert++] = topRight;

            vertices[vert++] = topRight;
            vertices[vert++] = topLeft;
            vertices[vert++] = bottomLeft;

            //top
            vertices[vert++] = topCenter;
            vertices[vert++] = topLeft;
            vertices[vert++] = topRight;

            //bottom
            vertices[vert++] = bottomCenter;
            vertices[vert++] = bottomRight;
            vertices[vert++] = bottomLeft;

            currentAngle += deltaAngle;
        }

        for(int i = 0; i < numVertices; i++)
        {
            triangles[i] = i;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        return mesh;
    }

    private void OnValidate()
    {
        mesh = CreateWedgeMesh();
        scanInterval = 1.0f / scanFrequency;
    }

    private void OnDrawGizmos()
    {
        if(mesh)
        {
            Gizmos.color = meshColor;
            Gizmos.DrawMesh(mesh, transform.position, transform.rotation);
            LineRenderer line = new LineRenderer();
            
        }

        //Gizmos.DrawWireSphere(transform.position, distance);
        for(int i = 0; i < count; ++i)
        {
            Gizmos.DrawSphere(colliders[i].transform.position, 0.2f);
        }

        Gizmos.color = Color.green;
        foreach(var obj in Objects)
        {
            Gizmos.DrawSphere(obj.transform.position, 0.2f);
        }
    }
}
