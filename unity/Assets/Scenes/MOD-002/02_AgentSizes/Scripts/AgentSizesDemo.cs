// @trace MOD-002  |  02 多体型：两条不同的路
// 本场景自包含：几何、材质、导航组件均由本脚本在 Awake 中搭建，不引用其他 demo。
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;
using System.Collections;

namespace MOD002.AgentSizes
{
    public class AgentSizesDemo : MonoBehaviour
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

        NavMeshAgent small, tank;
        const string hints = "左键点地面：两个角色同时寻路 ｜ 看谁走了窄缝";

        void Awake()
        {
            Aim(0, 15, -19);
            Prim(PrimitiveType.Cube, "Ground", new Vector3(0, -0.5f, 0), new Vector3(22, 1, 22), new Color(0.30f, 0.45f, 0.32f));
            // 一道墙，留 1m 窄缝（x=-3..-2）与 4m 宽通道（x=3..7）
            Prim(PrimitiveType.Cube, "Wall_A", new Vector3(-6, 1, 0), new Vector3(4, 2, 1), new Color(0.55f, 0.55f, 0.6f));
            Prim(PrimitiveType.Cube, "Wall_B", new Vector3(0.5f, 1, 0), new Vector3(5, 2, 1), new Color(0.55f, 0.55f, 0.6f));
            Prim(PrimitiveType.Cube, "Wall_C", new Vector3(7.5f, 1, 0), new Vector3(1, 2, 1), new Color(0.55f, 0.55f, 0.6f));
            Prim(PrimitiveType.Sphere, "Target", new Vector3(0, 1.2f, 7), new Vector3(0.6f, 0.6f, 0.6f), new Color(0.9f, 0.25f, 0.2f));

            int tankId = 0;
            if (NavMesh.GetSettingsCount() > 1)
                tankId = NavMesh.GetSettingsByIndex(1).agentTypeID;
            else
                status = "警告：未找到第二个 Agent Type，坦克会用 Humanoid 参数";

            var surfSmall = new GameObject("Surface_Small").AddComponent<NavMeshSurface>();
            surfSmall.transform.SetParent(transform, false);
            surfSmall.collectObjects = CollectObjects.All;
            surfSmall.agentTypeID = 0;
            surfSmall.BuildNavMesh();

            if (tankId != 0)
            {
                var surfTank = new GameObject("Surface_Tank").AddComponent<NavMeshSurface>();
                surfTank.transform.SetParent(transform, false);
                surfTank.collectObjects = CollectObjects.All;
                surfTank.agentTypeID = tankId;
                surfTank.BuildNavMesh();
            }

            var sGo = Prim(PrimitiveType.Capsule, "SmallAgent", new Vector3(-5, 1, -6), new Vector3(0.8f, 1, 0.8f), new Color(0.2f, 0.7f, 0.3f));
            small = sGo.AddComponent<NavMeshAgent>();
            small.speed = 3;
            small.agentTypeID = 0;

            var tGo = Prim(PrimitiveType.Cylinder, "TankAgent", new Vector3(-3, 1, -6), new Vector3(1.6f, 1, 1.6f), new Color(0.85f, 0.35f, 0.15f));
            tank = tGo.AddComponent<NavMeshAgent>();
            tank.speed = 2.2f;
            if (tankId != 0) tank.agentTypeID = tankId;

            TryMove(small, new Vector3(0, 1.2f, 7));
            TryMove(tank, new Vector3(0, 1.2f, 7));
            if (string.IsNullOrEmpty(status)) status = "小兵（绿）走窄缝，坦克（橙）走宽通道";
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit h;
                if (Physics.Raycast(ray, out h, 200f))
                {
                    TryMove(small, h.point);
                    TryMove(tank, h.point);
                    status = "目标已更新，观察两条路径";
                }
            }
        }
        void OnGUI()
        {
            GUI.Box(new Rect(10, 10, 560, 96),
                "02 多体型：两条不同的路\n" + status + "\n" + hints);
        }
    }
}
