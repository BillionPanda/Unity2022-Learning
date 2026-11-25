// @trace MOD-002  |  05 区域代价与 Area Mask
// 本场景自包含：几何、材质、导航组件均由本脚本在 Awake 中搭建，不引用其他 demo。
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;
using System.Collections;

namespace MOD002.AreaCost
{
    public class AreaCostDemo : MonoBehaviour
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

        NavMeshAgent walker, afraid;
        const int WATER = 3;
        const string hints = "C：切换水域代价(3↔1) ｜ 左键改目标 ｜ 观察绿/蓝两条路径";

        void Awake()
        {
            Aim(0, 16, -20);
            Prim(PrimitiveType.Cube, "Ground", new Vector3(0, -0.5f, 0), new Vector3(20, 1, 22), new Color(0.36f, 0.42f, 0.32f));
            Prim(PrimitiveType.Cube, "CenterWall", new Vector3(0, 1, 0), new Vector3(2, 2, 8), new Color(0.5f, 0.5f, 0.55f));
            var water = Prim(PrimitiveType.Cube, "WaterPatch", new Vector3(-4, 0.02f, 0), new Vector3(3, 0.04f, 8), new Color(0.2f, 0.5f, 0.85f));
            var mod = water.AddComponent<NavMeshModifierVolume>();
            mod.center = Vector3.zero;
            mod.size = new Vector3(3, 0.5f, 8);
            mod.area = WATER;

            var surfGo = new GameObject("NavMeshSurface");
            surfGo.transform.SetParent(transform, false);
            var surf = surfGo.AddComponent<NavMeshSurface>();
            surf.collectObjects = CollectObjects.All;
            surf.BuildNavMesh();
            NavMesh.SetAreaCost(WATER, 3f);

            var g = Prim(PrimitiveType.Capsule, "Walker", new Vector3(-1.5f, 1, -8), new Vector3(0.8f, 1, 0.8f), new Color(0.25f, 0.8f, 0.3f));
            walker = g.AddComponent<NavMeshAgent>();
            walker.speed = 3;
            walker.areaMask = NavMesh.AllAreas;

            var b = Prim(PrimitiveType.Capsule, "Afraid", new Vector3(1.5f, 1, -8), new Vector3(0.8f, 1, 0.8f), new Color(0.3f, 0.55f, 0.95f));
            afraid = b.AddComponent<NavMeshAgent>();
            afraid.speed = 3;
            afraid.areaMask = (1 << 0) | (1 << 2); // 排除 area 3

            TryMove(walker, new Vector3(0, 1, 8));
            TryMove(afraid, new Vector3(0, 1, 8));
            status = "绿=可蹚水（代价3）走短路；蓝=禁用水域，绕右侧长路";
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit h;
                if (Physics.Raycast(ray, out h, 200f))
                {
                    TryMove(walker, h.point);
                    TryMove(afraid, h.point);
                }
            }
            if (Input.GetKeyDown(KeyCode.C))
            {
                float now = NavMesh.GetAreaCost(WATER);
                NavMesh.SetAreaCost(WATER, now > 1.5f ? 1f : 3f);
                status = "水域代价=" + NavMesh.GetAreaCost(WATER).ToString("F1") + "，观察者路径会变化";
            }
        }
        void OnGUI()
        {
            GUI.Box(new Rect(10, 10, 560, 96),
                "05 区域代价与 Area Mask\n" + status + "\n" + hints);
        }
    }
}
