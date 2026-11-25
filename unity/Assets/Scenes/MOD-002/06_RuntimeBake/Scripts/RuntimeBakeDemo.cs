// @trace MOD-002  |  06 运行时烘焙与增量更新
// 本场景自包含：几何、材质、导航组件均由本脚本在 Awake 中搭建，不引用其他 demo。
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;
using System.Collections;

namespace MOD002.RuntimeBake
{
    public class RuntimeBakeDemo : MonoBehaviour
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

        NavMeshSurface surface;
        NavMeshAgent agent;
        GameObject plank;
        const string hints = "空格：生成/移除木板并重建 NavMesh ｜ 左键改目标";

        void Awake()
        {
            Aim(0, 15, -19);
            Prim(PrimitiveType.Cube, "Ground_L", new Vector3(0, -0.5f, -5), new Vector3(16, 1, 8), new Color(0.30f, 0.46f, 0.32f));
            Prim(PrimitiveType.Cube, "Ground_R", new Vector3(0, -0.5f, 5), new Vector3(16, 1, 8), new Color(0.32f, 0.42f, 0.55f));
            plank = Prim(PrimitiveType.Cube, "Plank", new Vector3(0, -0.45f, 0), new Vector3(3, 0.1f, 4), new Color(0.75f, 0.45f, 0.2f));
            plank.SetActive(false); // 初始未铺板，两岸断开

            var surfGo = new GameObject("NavMeshSurface");
            surfGo.transform.SetParent(transform, false);
            surface = surfGo.AddComponent<NavMeshSurface>();
            surface.collectObjects = CollectObjects.All;
            surface.BuildNavMesh();

            var aGo = Prim(PrimitiveType.Capsule, "Agent", new Vector3(0, 1, -7), new Vector3(0.8f, 1, 0.8f), new Color(0.9f, 0.7f, 0.25f));
            agent = aGo.AddComponent<NavMeshAgent>();
            agent.speed = 3;
            TryMove(agent, new Vector3(0, 1, 7));
            status = "木板未连接两侧 → 角色停在左岸；按空格铺板";
        }

        void Update()
        {
            Pick(agent);
            if (Input.GetKeyDown(KeyCode.Space))
            {
                plank.SetActive(!plank.activeSelf);
                surface.BuildNavMesh(); // 全量重建；换成 UpdateNavMesh(data) 即增量更新
                TryMove(agent, new Vector3(0, 1, 7));
                status = plank.activeSelf ? "木板已铺：BuildNavMesh 重建，角色可过" : "木板移除：路径消失，角色停在左岸";
            }
        }
        void OnGUI()
        {
            GUI.Box(new Rect(10, 10, 560, 96),
                "06 运行时烘焙与增量更新\n" + status + "\n" + hints);
        }
    }
}
