// @trace MOD-002  |  03 Off-Mesh Link：抛物线跳跃
// 本场景自包含：几何、材质、导航组件均由本脚本在 Awake 中搭建，不引用其他 demo。
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;
using System.Collections;

namespace MOD002.OffMeshLink
{
    public class OffMeshLinkDemo : MonoBehaviour
    {
        string status = "";
        GameObject Prim(PrimitiveType t, string name, Vector3 pos, Vector3 scale, Color color)
        {
            var go = GameObject.CreatePrimitive(t);
            go.name = name;
            go.transform.SetParent(transform, false);
            go.transform.position = pos;
            go.transform.localScale = scale;
            var mr = go.GetComponent<Renderer>();
            if (mr != null) mr.material = MakeMat(color);
            return go;
        }

        Material MakeMat(Color c)
        {
            var sh = Shader.Find("Universal Render Pipeline/Lit");
            if (sh == null) sh = Shader.Find("Standard");
            var m = new Material(sh);
            m.color = c;
            return m;
        }

        void Aim(float x, float y, float z)
        {
            var cam = Camera.main;
            if (cam == null) return;
            cam.transform.position = new Vector3(x, y, z);
            cam.transform.LookAt(new Vector3(0, 0, 1));
        }

        void TryMove(NavMeshAgent a, Vector3 target)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(target, out hit, 3f, NavMesh.AllAreas))
                a.SetDestination(hit.position);
            else
                status = "目标点不在 NavMesh 上：换个位置点";
        }

        void Pick(NavMeshAgent a)
        {
            if (!Input.GetMouseButtonDown(0) || Camera.main == null) return;
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 200f))
                TryMove(a, hit.point);
        }

        NavMeshAgent agent;
        NavMeshLink link;
        bool animated = true;
        const string hints = "左键点对面平台 ｜ J：切换 自动瞬移 / 抛物线跳跃";

        void Awake()
        {
            Aim(0, 14, -19);
            Prim(PrimitiveType.Cube, "Platform_L", new Vector3(-6, -0.5f, 0), new Vector3(8, 1, 8), new Color(0.28f, 0.48f, 0.32f));
            Prim(PrimitiveType.Cube, "Platform_R", new Vector3(6, -0.5f, 0), new Vector3(8, 1, 8), new Color(0.32f, 0.42f, 0.58f));
            Prim(PrimitiveType.Sphere, "Target", new Vector3(7, 1, 0), new Vector3(0.6f, 0.6f, 0.6f), new Color(0.9f, 0.25f, 0.2f));

            var surfGo = new GameObject("NavMeshSurface");
            surfGo.transform.SetParent(transform, false);
            var surface = surfGo.AddComponent<NavMeshSurface>();
            surface.collectObjects = CollectObjects.All;
            surface.BuildNavMesh();

            var linkGo = new GameObject("Gap_Link");
            linkGo.transform.SetParent(transform, false);
            link = linkGo.AddComponent<NavMeshLink>();
            link.startPoint = new Vector3(-2, 0, 0);
            link.endPoint = new Vector3(2, 0, 0);
            link.width = 2f;
            link.bidirectional = true;

            var aGo = Prim(PrimitiveType.Capsule, "Agent", new Vector3(-8, 1, 0), new Vector3(0.8f, 1, 0.8f), new Color(0.9f, 0.75f, 0.2f));
            agent = aGo.AddComponent<NavMeshAgent>();
            agent.speed = 3;
            agent.autoTraverseOffMeshLink = !animated;
            StartCoroutine(LinkMover());
            TryMove(agent, new Vector3(7, 1, 0));
            status = "跳跃动画=开（抛物线）；关掉后 Unity 会瞬移";
        }

        IEnumerator LinkMover()
        {
            while (true)
            {
                if (agent != null && agent.isOnOffMeshLink && animated)
                {
                    OffMeshLinkData data = agent.currentOffMeshLinkData;
                    Vector3 from = agent.transform.position;
                    Vector3 to = data.endPos + Vector3.up * agent.baseOffset;
                    float t = 0f;
                    while (t < 1f)
                    {
                        float arc = 2.5f * 4f * (t - t * t);
                        agent.transform.position = Vector3.Lerp(from, to, t) + Vector3.up * arc;
                        t += Time.deltaTime / 0.6f;
                        yield return null;
                    }
                    agent.CompleteOffMeshLink();
                }
                yield return null;
            }
        }

        void Update()
        {
            Pick(agent);
            if (Input.GetKeyDown(KeyCode.J))
            {
                animated = !animated;
                agent.autoTraverseOffMeshLink = !animated;
                status = animated ? "跳跃动画=开（抛物线）" : "跳跃动画=关（Unity 瞬移）";
            }
        }
        void OnGUI()
        {
            GUI.Box(new Rect(10, 10, 560, 96),
                "03 Off-Mesh Link：抛物线跳跃\n" + status + "\n" + hints);
        }
    }
}
