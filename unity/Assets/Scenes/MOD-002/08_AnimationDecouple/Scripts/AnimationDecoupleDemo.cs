// @trace MOD-002  |  08 动画与导航解耦
// 本场景自包含：几何、材质、导航组件均由本脚本在 Awake 中搭建，不引用其他 demo。
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;
using System.Collections;

namespace MOD002.AnimationDecouple
{
    public class AnimationDecoupleDemo : MonoBehaviour
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
        Transform face;
        bool manualRotation = true;
        const string hints = "R：切换 updateRotation ｜ 左键改目标 —— 看橙色指示块怎么转";

        void Awake()
        {
            Aim(0, 13, -16);
            Prim(PrimitiveType.Cube, "Ground", new Vector3(0, -0.5f, 0), new Vector3(20, 1, 20), new Color(0.30f, 0.44f, 0.32f));
            var aGo = Prim(PrimitiveType.Capsule, "Agent", new Vector3(-6, 1, -6), new Vector3(0.8f, 1, 0.8f), new Color(0.28f, 0.56f, 0.9f));
            agent = aGo.AddComponent<NavMeshAgent>();
            agent.speed = 3;
            agent.updateRotation = !manualRotation;

            face = Prim(PrimitiveType.Cube, "FacingMarker", new Vector3(-6, 1, -5.2f), new Vector3(0.25f, 0.25f, 0.25f), new Color(0.95f, 0.45f, 0.15f)).transform;
            face.SetParent(aGo.transform, true);

            var surfGo = new GameObject("NavMeshSurface"); surfGo.transform.SetParent(transform, false);
            var surf = surfGo.AddComponent<NavMeshSurface>();
            surf.collectObjects = CollectObjects.All;
            surf.BuildNavMesh();

            TryMove(agent, new Vector3(6, 1, 6));
            status = "手动朝向=开：角色转身有平滑过渡";
        }

        void LateUpdate()
        {
            Pick(agent);
            if (Input.GetKeyDown(KeyCode.R))
            {
                manualRotation = !manualRotation;
                agent.updateRotation = !manualRotation;
                status = manualRotation ? "手动朝向=开（平滑转身）" : "手动朝向=关（Unity 硬拧向路径）";
            }
            if (manualRotation && agent != null && agent.velocity.sqrMagnitude > 0.01f)
            {
                var dir = agent.velocity.normalized;
                var targetRot = Quaternion.LookRotation(dir);
                agent.transform.rotation = Quaternion.Slerp(agent.transform.rotation, targetRot, Time.deltaTime * 4f);
            }
        }
        void OnGUI()
        {
            GUI.Box(new Rect(10, 10, 560, 96),
                "08 动画与导航解耦\n" + status + "\n" + hints);
        }
    }
}
