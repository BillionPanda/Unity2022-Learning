// @trace MOD-002  |  01 HeightMesh：台阶贴合
// 本场景自包含：几何、材质、导航组件均由本脚本在 Awake 中搭建，不引用其他 demo。
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;
using System.Collections;

namespace MOD002.HeightMesh
{
    public class HeightMeshDemo : MonoBehaviour
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
        bool heightMesh = false;
        const string hints = "左键点地面移动 ｜ H：切换 HeightMesh 并重烘焙";

        void Awake()
        {
            Aim(0, 13, -17);
            Prim(PrimitiveType.Cube, "Ground", new Vector3(0, -0.5f, 0), new Vector3(20, 1, 22), new Color(0.28f, 0.48f, 0.30f));
            for (int i = 0; i < 5; i++)
            {
                float h = 0.3f * (i + 1);
                Prim(PrimitiveType.Cube, "Step_" + i, new Vector3(0, h * 0.5f, 1.5f + i * 0.8f),
                    new Vector3(4, h, 0.8f), new Color(0.62f, 0.42f, 0.24f));
            }
            Prim(PrimitiveType.Cube, "TopPlatform", new Vector3(0, 0.75f, 6.6f), new Vector3(4, 1.5f, 3), new Color(0.62f, 0.42f, 0.24f));
            Prim(PrimitiveType.Sphere, "Target", new Vector3(0, 1.7f, 7), new Vector3(0.6f, 0.6f, 0.6f), new Color(0.9f, 0.25f, 0.2f));

            var surfGo = new GameObject("NavMeshSurface");
            surfGo.transform.SetParent(transform, false);
            surface = surfGo.AddComponent<NavMeshSurface>();
            surface.collectObjects = CollectObjects.All;
            surface.buildHeightMesh = heightMesh;
            surface.BuildNavMesh();

            var agentGo = Prim(PrimitiveType.Capsule, "Agent", new Vector3(0, 1, -7), new Vector3(0.8f, 1, 0.8f), new Color(0.25f, 0.55f, 0.9f));
            agent = agentGo.AddComponent<NavMeshAgent>();
            agent.speed = 3;
            agent.SetDestination(new Vector3(0, 1.7f, 7));
            status = "HeightMesh=" + heightMesh + "（关：台阶被抹成斜坡）";
        }

        void Update()
        {
            Pick(agent);
            if (Input.GetKeyDown(KeyCode.H))
            {
                heightMesh = !heightMesh;
                surface.buildHeightMesh = heightMesh;
                surface.BuildNavMesh();
                status = "HeightMesh=" + heightMesh + (heightMesh ? "（开：精确台阶高度）" : "（关：斜坡近似）");
            }
            if (agent != null)
                status = "HeightMesh=" + heightMesh + " ｜ 角色脚部 Y=" + agent.transform.position.y.ToString("F2");
        }
        void OnGUI()
        {
            GUI.Box(new Rect(10, 10, 560, 96),
                "01 HeightMesh：台阶贴合\n" + status + "\n" + hints);
        }
    }
}
