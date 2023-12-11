using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

//[ExecuteInEditMode]
public class enemyScript : MonoBehaviour
{
    [SerializeField] ParticleSystem gun;
    [SerializeField] GameObject bullet;
    [SerializeField] public int rotLeftAngle = 150;
    [SerializeField] public int rotRightAngle = 210;
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
        ogPos = gameObject.transform.position;
        ogRot = gameObject.transform.rotation.eulerAngles;
        shootable = true;
        scanInterval = 1.0f / scanFrequency;
    }

    // Update is called once per frame
    void Update()
    {
        scanTimer -= Time.deltaTime;
        if(scanTimer < 0 && !over)
        {
            scanTimer += scanInterval;
            Scan();
        }

        if(over)
        {
            gun.Stop();
        }

        if(seenApriori)
        {
            StartCoroutine(Chase());
        }

        if (back && gameObject.transform.position == ogPos)
        {
            back = false;
            StartCoroutine(returnToRot());
        }
        else if (gameObject.transform.position == ogPos)
        {
            StartCoroutine(Look());
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

    IEnumerator Look()
    {

        //Declare a yield instruction.
        WaitForSeconds wait = new WaitForSeconds(0.5f);
        
        if(cnt < 3)
        {
            yield return new WaitForSeconds(0.5f);

            while (transform.rotation.eulerAngles.y > rotLeftAngle && !left)
            {
                transform.rotation *= Quaternion.Euler(Vector3.down * Time.deltaTime * 100f);
                yield return wait;
            }
            left = true;

            yield return new WaitForSeconds(2f);

            while (transform.rotation.eulerAngles.y < rotRightAngle && !right)
            {
                transform.rotation *= Quaternion.Euler(Vector3.up * Time.deltaTime * 100f);
                yield return wait;
            }
            right = true;
        }

        if(left && right)
        {
            cnt++;
            left = false;
            right = false;
        }

        if(cnt == 3)
        {
            while (transform.rotation.eulerAngles.y != -transform.rotation.eulerAngles.y)
            {
                transform.rotation *= Quaternion.Euler(Vector3.down * Time.deltaTime * 100f);
                yield return wait;
            }

            cnt = 0;
        }

    }

    IEnumerator returnToRot()
    {
        seenApriori = false;

        //Declare a yield instruction.
        WaitForSeconds wait = new WaitForSeconds(0.00000002f);


        while(transform.rotation.eulerAngles.y != ogRot.y)
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

                gameObject.transform.LookAt(looker);

                if(shootable)
                {
                    seenApriori = true;
                    StartCoroutine(Wait());
                    Shoot(obj);
                }
            }
        }

        if(Objects.Count == 0)
        {
            gun.Stop();

            if(seenApriori)
            {
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

        for (int i = 0; i < 200; i++)
        {
            transform.rotation *= Quaternion.Euler(Vector3.up * Time.deltaTime * 5f);
            yield return wait;
        }

        for (int i = 0; i < 400; i++)
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
