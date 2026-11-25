// @trace MOD-002  |  07 多 Surface 显式连接
// 本场景自包含：几何、材质、导航组件均由本脚本在 Awake 中搭建，不引用其他 demo。
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;
using System.Collections;

namespace MOD002.MultipleSurfaces
{
    public class MultipleSurfacesDemo : MonoBehaviour
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
        const string hints = "L：断开/接上 Link ｜ 左键改目标 —— 两张 Surface 不会自动连通";

        void Awake()
        {
            Aim(0, 15, -19);
            var left = Prim(PrimitiveType.Cube, "Island_A", new Vector3(-6, -0.5f, 0), new Vector3(8, 1, 8), new Color(0.30f, 0.45f, 0.34f));
            var right = Prim(PrimitiveType.Cube, "Island_B", new Vector3(6, -0.5f, 0), new Vector3(8, 1, 8), new Color(0.34f, 0.42f, 0.58f));

            var saGo = new GameObject("Surface_A"); saGo.transform.SetParent(transform, false);
            saGo.transform.position = left.transform.position;
            left.transform.SetParent(saGo.transform, true); // Children 模式必须让地面成为子物体
            var sa = saGo.AddComponent<NavMeshSurface>();
            sa.collectObjects = CollectObjects.Children;
            sa.center = Vector3.zero;
            sa.size = new Vector3(8, 3, 8);
            sa.BuildNavMesh();

            var sbGo = new GameObject("Surface_B"); sbGo.transform.SetParent(transform, false);
            sbGo.transform.position = right.transform.position;
            right.transform.SetParent(sbGo.transform, true);
            var sb = sbGo.AddComponent<NavMeshSurface>();
            sb.collectObjects = CollectObjects.Children;
            sb.center = Vector3.zero;
            sb.size = new Vector3(8, 3, 8);
            sb.BuildNavMesh();

            // 注意：Surface 各自有位移，Link 坐标按本物体局部坐标填写
            var linkGo = new GameObject("Bridge_Link");
            linkGo.transform.SetParent(transform, false);
            link = linkGo.AddComponent<NavMeshLink>();
            link.startPoint = new Vector3(-2, 0, 0);
            link.endPoint = new Vector3(2, 0, 0);
            link.width = 2f;
            link.bidirectional = true;

            Prim(PrimitiveType.Sphere, "Target", new Vector3(7, 1, 0), new Vector3(0.6f, 0.6f, 0.6f), new Color(0.9f, 0.25f, 0.2f));
            var aGo = Prim(PrimitiveType.Capsule, "Agent", new Vector3(-8, 1, 0), new Vector3(0.8f, 1, 0.8f), new Color(0.9f, 0.72f, 0.25f));
            agent = aGo.AddComponent<NavMeshAgent>();
            agent.speed = 3;
            TryMove(agent, new Vector3(7, 1, 0));
            status = "两张 Surface + 一条 Link；按 L 断开对比";
        }

        void Update()
        {
            Pick(agent);
            if (Input.GetKeyDown(KeyCode.L))
            {
                link.enabled = !link.enabled;
                TryMove(agent, new Vector3(7, 1, 0));
                status = link.enabled ? "Link 接上：可跨岛" : "Link 断开：到不了对岸（这就是证据）";
            }
        }
        void OnGUI()
        {
            GUI.Box(new Rect(10, 10, 560, 96),
                "07 多 Surface 显式连接\n" + status + "\n" + hints);
        }
    }
}
