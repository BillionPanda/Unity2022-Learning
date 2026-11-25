// @trace MOD-002  |  04 动态障碍：挖洞 vs 规避
// 本场景自包含：几何、材质、导航组件均由本脚本在 Awake 中搭建，不引用其他 demo。
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;
using System.Collections;

namespace MOD002.ObstacleCarving
{
    public class ObstacleCarvingDemo : MonoBehaviour
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
        NavMeshObstacle obstacle;
        Transform box;
        bool carve = true;
        const string hints = "空格：把箱子推到路口 / 推回 ｜ C：切换 Carve ｜ 左键改目标";

        void Awake()
        {
            Aim(0, 14, -18);
            Prim(PrimitiveType.Cube, "Ground", new Vector3(0, -0.5f, 0), new Vector3(20, 1, 20), new Color(0.32f, 0.45f, 0.30f));
            // 一堵带门洞的墙，门洞在 x=-2..2
            Prim(PrimitiveType.Cube, "Wall_L", new Vector3(-7, 1, 0), new Vector3(6, 2, 1), new Color(0.55f, 0.55f, 0.6f));
            Prim(PrimitiveType.Cube, "Wall_R", new Vector3(7, 1, 0), new Vector3(6, 2, 1), new Color(0.55f, 0.55f, 0.6f));

            var surfGo = new GameObject("NavMeshSurface");
            surfGo.transform.SetParent(transform, false);
            var surf = surfGo.AddComponent<NavMeshSurface>();
            surf.collectObjects = CollectObjects.All;
            surf.BuildNavMesh();

            var boxGo = Prim(PrimitiveType.Cube, "MovingBox", new Vector3(0, 0.6f, 4), new Vector3(2.6f, 1.2f, 1), new Color(0.75f, 0.4f, 0.2f));
            box = boxGo.transform;
            obstacle = boxGo.AddComponent<NavMeshObstacle>();
            obstacle.carving = carve;
            obstacle.carveOnlyStationary = true;
            obstacle.shape = NavMeshObstacleShape.Box;
            obstacle.size = new Vector3(2.6f, 1.2f, 1f);

            var aGo = Prim(PrimitiveType.Capsule, "Agent", new Vector3(-6, 1, -6), new Vector3(0.8f, 1, 0.8f), new Color(0.25f, 0.55f, 0.9f));
            agent = aGo.AddComponent<NavMeshAgent>();
            agent.speed = 3;
            TryMove(agent, new Vector3(0, 1, 6));
            status = "箱子在旁边，角色走门洞；空格把它推到门洞中央";
        }

        void Update()
        {
            Pick(agent);
            if (Input.GetKeyDown(KeyCode.Space))
            {
                bool inDoor = Mathf.Abs(box.position.z) < 1f;
                box.position = inDoor ? new Vector3(0, 0.6f, 4) : new Vector3(0, 0.6f, 0);
                status = inDoor ? "箱子已移开门洞" : "箱子堵住门洞，看角色如何改道";
            }
            if (Input.GetKeyDown(KeyCode.C))
            {
                carve = !carve;
                obstacle.carving = carve;
                status = carve ? "Carve=开：障碍在 NavMesh 上挖洞" : "Carve=关：只有短距碰撞规避";
            }
        }
        void OnGUI()
        {
            GUI.Box(new Rect(10, 10, 560, 96),
                "04 动态障碍：挖洞 vs 规避\n" + status + "\n" + hints);
        }
    }
}
