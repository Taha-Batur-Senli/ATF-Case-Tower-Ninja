using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[ExecuteInEditMode]
public class enemyScript : MonoBehaviour
{
    [SerializeField] ParticleSystem gun;
    [SerializeField] GameObject bullet;
    public float distance = 10;
    public float angle = 30;
    public float height = 1.0f;
    public Color meshColor = Color.red;
    public int scanFrequency = 30;
    public LayerMask layers;
    public LayerMask occlusionLayers;
    bool seenApriori = false;
    public float movementSpeed = 0.8f;

    public List<GameObject> Objects = new List<GameObject>();
    Collider[] colliders = new Collider[50];
    Mesh mesh;
    int count;
    float scanInterval;
    float scanTimer;
    bool shootable;
    public bool over = false;

    // Start is called before the first frame update
    void Start()
    {
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
                StartCoroutine(SomeCoroutine());
            }
        }
    }
    IEnumerator SomeCoroutine()
    {
        seenApriori = false;
        //Declare a yield instruction.
        WaitForSeconds wait = new WaitForSeconds(0.5f);

        for (int i = 0; i < 10; i++)
        {
            transform.rotation *= Quaternion.Euler(Vector3.up * Time.deltaTime * 10000f);
            yield return wait;
        }

        for (int i = 0; i < 20; i++)
        {
            transform.rotation *= Quaternion.Euler(Vector3.down * Time.deltaTime * 10000f);
            yield return wait;
        }

    }

    IEnumerator Forget()
    {
        seenApriori = false;

        float time = 0;

        yield return new WaitForSeconds(1.5f);

        while (time < 0.2f)
        {
            Debug.Log(time);

            transform.rotation *= Quaternion.Euler(Vector3.up * 0.5f);
            yield return new WaitForEndOfFrame();

            time += Time.deltaTime;
        }
    }
    IEnumerator Forget2()
    {
        yield return new WaitForSeconds(1f);

        float time = 0;

        while (time < 0.4f)
        {

            transform.rotation *= Quaternion.Euler(Vector3.down * 0.5f);
            yield return new WaitForEndOfFrame();

            time += Time.deltaTime;
        }

        yield return new WaitForSeconds(1f);
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
