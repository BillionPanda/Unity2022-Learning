# Unity 2022 模块化教学手册

> 基于你的项目：`D:\UnityProject\TestProjectAddAI` ｜ Unity **2022.3.14f1c1** ｜ URP **14.0.9**  
> 这份手册取代之前的「广泛待办清单」。编号按教学顺序重排为 `MOD-001`…`MOD-010`。  
> 每个模块统一结构：**为什么学 → 学完你能得到什么（预期）→ 核心机制 → 动手步骤 → 验收 → 迁移到真实项目 → 常见坑**。  
> 所有 API 名以本机包版本与官方文档为准；不臆造签名，抄包内 Samples 最稳。

**这个项目有两个身份，先看清楚再学**：

1. **引擎功能线**（本文件）：熟悉 Unity 2022 LTS 新功能，用 `// @trace MOD-00X` 标记。
2. **理论验证线**（`GameProgrammer-Unity验证映射.md`）：后续学 [miloyip/game-programmer](https://github.com/miloyip/game-programmer) 书单时，用本项目验证书单里的知识域，用 `// @verify GP-0X` 标记。

两条线共用同一工程，**标记不混用**：写 2022 新功能的脚本标 `MOD-`；验证书单理论的标 `GP-`（B/C 类不写脚本的，在 `Evidence/` 建 `GP-0X-笔记.md`）。自检脚本分开统计两条覆盖率。书单是 C++/底层引擎导向，并非所有域都能实机验证，映射文件已按 A 实机 / B 封装观察 / C 不在此项目 三类分好。

---

## 怎么用这份手册

1. **按顺序做**，不要跳。前一个模块的「验收」没过，不做下一个。
2. 每个模块结束做两件事：在 `Evidence/` 留一份证据（截图 / 日志 / 笔记），在脚本顶部写 `// @trace MOD-00X`。
3. **先看「预期」再动手**。预期不是客套话，是验收的标尺：做完后问自己「这三条我做到了吗」。
4. 模块 10 是阅读级，不写代码，但预期同样是硬的——要能做出技术选型判断。

## 模块地图：学完你到底得到什么

| 编号      | 模块                       | 学完你能得到什么（一句话）                               |
| ------- | ------------------------ | ------------------------------------------- |
| MOD-001 | 工程基线与排错                  | 能独立判断版本/管线/包状态，并能自己修复包缓存类编译错误               |
| MOD-002 | AI Navigation 智能导航       | 能做出「点击地面，角色绕开障碍走过去」，并解释烘焙参数为何改变结果           |
| MOD-003 | URP 渲染能力                 | 能判断什么时候必须切 Forward+，并用对比截图证明 TAA / LOD 的作用  |
| MOD-004 | Splines 程序化路径            | 能做出「一条弯曲路径 + 沿路生成物体 + 摄像机沿路巡游」的可复用系统        |
| MOD-005 | DOTS / ECS               | 能用 Entity 写批量物体运动，并说清它和 MonoBehaviour 的适用边界 |
| MOD-006 | Prefab 与 Override        | 能安全整批替换 Prefab 资产而不丢场景改动，并能清理无用 Override    |
| MOD-007 | Addressables + 内存分析      | 能把资源从「场景直接引用」改成「按需加载并释放」，用快照证明内存下降          |
| MOD-008 | UI Toolkit 与编辑器扩展        | 能做出可换肤的运行时 HUD，并给自定义组件写出可用的 PropertyDrawer  |
| MOD-009 | Netcode for Entities（选做） | 能说清服务端权威与客户端预测的区别，跑通两个客户端同步                 |
| MOD-010 | HDRP / VFX / 平台（阅读级）     | 能做技术选型：什么时候必须 HDRP、这台机器为什么只能读不能做            |

---

## MOD-001 · 工程基线与排错

**为什么学**：你现在遇到的 CS0006 就是这一课。不清楚版本/管线/缓存机制，后面每个模块都会卡在环境问题而不是知识本身。

> **学完你能得到什么（预期）**
>
> - **能做出**：独立读完一个 Unity 工程的"体检报告"——版本、渲染管线、关键包、编译状态。
> - **能解释**：为什么包缓存里的 DLL"明明存在却找不到"，以及为什么删 `Library/Bee` 能修好它。
> - **能迁移**：以后任何 Unity 项目报包/编译错误，你第一反应是查缓存和布局，而不是重装编辑器。

**核心机制**

- `ProjectVersion.txt` 是版本真相；`Packages/manifest.json` 是包依赖真相；`Library/PackageCache` 只是**可再生**的解压副本。
- `Library/Bee` 和 `Library/ScriptAssemblies` 是编译缓存，记录着旧的引用路径；包升级后路径变了，缓存没刷新就会报 CS0006。
- URP 与 HDRP 在同一项目互斥；你的是 URP，HDRP 只能阅读了解。

**动手步骤**

1. 打开 `ProjectSettings/ProjectVersion.txt`，记下 `2022.3.14f1c1`。
2. Edit → Project Settings → Graphics，确认当前渲染管线资产是 URP。
3. Package Manager 里记下 URP 版本 `14.0.9`；确认 `com.unity.collab-proxy` 已装且编译通过。
4. 复现一次诊断：打开 `%LOCALAPPDATA%\Unity\Editor\Editor.log`，搜索 `CS0006`，看 `-analyzer:` 指向的路径；再对比磁盘上 `Library/PackageCache/com.unity.collab-proxy@2.12.4/Lib/Editor/` 里 DLL 的真实位置。
5. 用自检脚本确认基线：`bash verify_unity2022_project.sh "D:/UnityProject/TestProjectAddAI"`。

**验收**：能不看提示说出「版本 / 管线 / URP 版本 / 包缓存是拷贝不是源」四项；Console 无红色编译错误。

**迁移到真实项目**：接手别人的工程先跑这套体检，再决定改代码还是先修环境。

**常见坑**：`com.unity.modules.ai` 是内置旧 AI 模块，不等于 `com.unity.ai.navigation` 包；别把两者搞混。

---

## MOD-002 · AI Navigation 智能导航

**为什么学**：你的项目名是 `TestProjectAddAI`，这是最贴合你目标的一课，也是 2022.2 起包化的重点功能。它把"让 NPC 会走路"从手写寻路变成配置 + 一行调用。

> **学完你能得到什么（预期）**
>
> - **能做出**：一个可点击移动的导航场景——鼠标点地面，Capsule 角色绕开 Cube 障碍走到目标点；改一个半径参数，窄通道从"能过"变"不能过"。
> - **能解释**：NavMesh 是可行走表面的**近似**；Agent Radius / Voxel Size / Collect Objects / Include Layers 分别控制烘焙的什么；为什么带 NavMeshAgent 的物体会被自动排除出烘焙。
> - **能迁移**：塔防敌人路径、RTS 点击移动、NPC 巡逻、动态障碍物——都是同一套机制。

**核心机制**

- `NavMeshSurface` 组件定义一个 NavMesh 的烘焙范围与代理类型；一个场景可有多个 Surface。
- `Collect Objects`：`All` / `Volume` / `Children`（新版本名可能为 `Current Object Hierarchy`），决定哪些物体参与烘焙；`Include Layers` 二次过滤。
- `Use Geometry`：`Render Meshes` vs `Physics Colliders`——后者让代理更贴近碰撞边界。
- 烘焙产出 `.asset`；运行时由 `NavMeshAgent` 消费。烘焙可在编辑器做，也可运行时做（后者吃内存/CPU）。

**动手步骤**

1. 装包 `com.unity.ai.navigation`（版本以本机 Package Manager 为准）。菜单 `GameObject > AI > NavMesh Surface`（1.x 手册路径）或 `Add Component > Navigation > NavMesh Surface`（2.x 手册路径）出现即装好。
2. 搭场景：Plane 当地面（放大到 10×10）；放 3 个 Cube 当墙，其中一个留 1.2 单位窄缝；放 Capsule 角色并加 `NavMeshAgent`。
3. 建空 GameObject 加 `NavMeshSurface`，`Collect Objects` 选 Children，把地面和墙作为它的子物体；点 `Bake`，观察蓝色可行走覆盖层——**注意窄缝处是否有 NavMesh**。
4. 写点击移动脚本（挂到 Capsule 上，场景里要有 Main Camera 标签）：

```csharp
using UnityEngine;
using UnityEngine.AI;

public class ClickToMove : MonoBehaviour
{
    private NavMeshAgent agent;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (!Input.GetMouseButtonDown(0)) return;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            agent.SetDestination(hit.point);
        }
    }
}
```

1. **教学关键实验**：把 `NavMeshAgent.radius` 从 0.5 调到 1.5，重新 Bake，再点窄缝另一侧——角色会绕远路或走不过去。记录前后两张截图，这就是"参数为什么影响结果"的直观证据。
2. 加一个 `NavMeshModifier` 把某块地面标成 `Not Walkable`，重新烘焙，看角色改道。

**验收**：角色能绕障到达；半径实验的前后行为差异可复现；你能说清 `Collect Objects` 三种模式的区别。

**迁移到真实项目**：把点击移动换成敌人 AI 的目标点，就是塔防的核心循环；把障碍物做成运行时可开关，就是动态断路。

**常见坑**：忘记 Bake / 角色没加 NavMeshAgent / 地面没 Collider 导致射线打不中 / 把角色也烘焙进 NavMesh。

---

## MOD-003 · URP 渲染能力

**为什么学**：这是 2022 LTS 最直观的升级，也是你在 URP 项目里每天会碰到的设置。它决定"场景能放多少灯、画面边缘干不干净、LOD 切换突不突兀"。

> **学完你能得到什么（预期）**
>
> - **能做出**：同一场景的"前后对比组"——12 个点光源在 Forward 与 Forward+ 下的表现差异、开/关 TAA 的边缘差异、LOD 切换的过渡差异。
> - **能解释**：Forward+ 为什么能放更多灯；Decal Layers 为什么能过滤贴花影响对象；Shader Variant Prefiltering 为什么影响构建时间和内存。
> - **能迁移**：做移动端/低端 PC 时，你能判断哪些效果该留、哪些该砍，而不是凭感觉调。

**核心机制**

- 2022.3 URP（14.x）提供 Forward+、TAA、LOD crossfade、Decal Layers、Shader Graph 全屏主节点、Shader Variant Prefiltering、内置管线转 URP 转换器。
- Forward+ 把灯光按屏幕分块处理，解除"每物体灯光数"限制；代价是tile 计算成本。
- TAA 用历史帧累积抗锯齿，对动态物体可能有拖影——要自己对比判断。

**动手步骤**

1. 在 SampleScene 放 12 个点光源，范围重叠、亮度调高；先按当前设置 Play 并截图。
2. 在 URP Asset / Renderer Data 的 Inspector 里找到 Rendering Path，切到 **Forward+**（具体名称以你本机 Inspector 为准），再 Play 截图，对比亮灯数量。
3. 打开 **TAA**，对着高对比边缘（如立方体轮廓）截前后两张图。
4. 加一个 LOD Group（三个不同细分的网格），打开 **LOD crossfade**，走近走远看过渡。
5. 加 Decal Projector 与 Decal Layers，让贴花只影响特定层。
6. 打开 Project Settings → Graphics 里的 Shader Variant 相关设置，做一次构建对比耗时（可选，耗时较长）。

**验收**：至少两组对比截图；能口头回答"什么时候该切 Forward+"。

**迁移到真实项目**：移动端优先保帧率，Forward+ 与 TAA 的开关要基于实测截图/Profiler，而不是默认开启。

**常见坑**：切换渲染管线后材质变粉，说明着色器不兼容；别把 HDRP 材质拖进 URP 项目。

---

## MOD-004 · Splines 程序化路径

**为什么学**：道路、河流、围栏、摄像机轨道——这些场景里到处都是路径。Splines 让路径成为**可编辑的数据**，改一个控制点，整条路径和沿路物体全部更新。

> **学完你能得到什么（预期）**
>
> - **能做出**：一条弯曲样条，沿它自动生成一排栅栏（间距可调），摄像机沿它自动巡游，并能做出一条分支路径。
> - **能解释**：`SplineContainer` / `Spline` / `BezierKnot` 的关系；`SplineInstantiate` 的三种实例化方法（Instance Count / Spline Distance / Linear Distance）分别适合什么场景。
> - **能迁移**：关卡路径生成、巡逻路线、树木/路灯沿路布置、过场动画摄像机轨迹。

**核心机制**

- `SplineContainer` 持有 `Spline`，`Spline` 是 `BezierKnot` 的集合（位置 + 进出切线）；命名空间 `UnityEngine.Splines`。
- 2022.1 是 Splines 1.0；2022.2 起是 2.1，加入分支、直接拖拽控制点、情境工作流。
- 三个现成组件：`SplineInstantiate`（沿样条生成物体）、`SplineAnimate`（沿样条移动）、`SplineExtrude`（沿样条生成管状网格）。

**动手步骤**

1. 装包 `com.unity.splines`（版本以本机为准）。
2. 建 GameObject 加 `SplineContainer`，用 Draw Splines 工具画一条 5 个控制点的弯曲路径。
3. 做一个 Cube 预制体当栅栏；加 `SplineInstantiate` 组件，Container 指向样条，Items 指向 Cube 预制体，Instantiate Method 先选 `Instance Count` 设为 10，再改 `Spline Distance` 观察差异。
4. 给摄像机加 `SplineAnimate`，Container 指向样条，Play 看巡游。
5. 试分支：用 Draw Splines 从原样条上再画一段，观察 `SplinePath` 如何实现多段拼接（概念级即可）。
6. 可选：加 `SplineExtrude` 生成一条管子，理解运行时网格生成。

**验收**：三件事都能演示——沿路生成、沿路移动、分支路径；你能解释三种实例化方法的区别。

**迁移到真实项目**：把"手工摆 50 个路灯"变成"画一条样条 + 一个组件"，改路线只改控制点。

**常见坑**：`SplineInstantiate` 的 Up Axis / Forward Axis 设错会导致物体朝向错乱；改样条后记得 Regenerate。

---

## MOD-005 · DOTS / ECS

**为什么学**：这是 2022 LTS 最大的架构变化——ECS 首次生产可用，并可与 GameObject 混用。它解决的是"大量同类物体的性能与代码组织"问题。

> **学完你能得到什么（预期）**
>
> - **能做出**：一个 SubScene 里成百上千个 Entity 按同一套规则运动，Console 能打印出 Entity 数据；对比同数量 GameObject 的表现。
> - **能解释**：Entity 是数据标识、Component 只存数据、System 只写行为；Baking 把 GameObject 转成 Entity；为什么数据连续布局有利于批处理。
> - **能迁移**：子弹/粒子/人群/建筑群等"数量大、逻辑同"的系统；也知道什么时候**不该**用 ECS（少量复杂交互对象）。

**核心机制**

- Entities 1.0 要求 Unity 2022.3.0f1+（你的版本满足）。
- 典型工作流：SubScene → 放 GameObject → Baker 生成 Entity → System 在运行时处理 Component 数据。
- ECS 与 GameObject 可并存：用 `Companion Components` / 引用互操作，不必全项目重写。

**动手步骤**

1. 装包 `com.unity.entities`（版本以本机为准）。
2. **先跑通包内 Samples**：Package Manager → Entities → Samples，导入并运行入门示例。这一步比你手写代码可靠。
3. 在场景新建 SubScene，把一批带相同组件的 GameObject 放进去；理解 Baking 后它们变成 Entity。
4. 照 Samples 写一个最小 Baker 与 System：给 Entity 一个速度分量，System 每帧更新位置；Console 打印第一个 Entity 的位置。
5. 把数量从 100 加到 5000，用 Profiler 看帧时间，和同数量 GameObject 对比。

**验收**：Console 有 Entity 数据输出；能说清"什么时候 ECS 比 MonoBehaviour 更合适"并给出两条理由。

**迁移到真实项目**：先把"最重复的那一类对象"迁到 ECS，而不是全盘重写——这是你以后做性能优化的起点。

**常见坑**：Entities Graphics 与渲染管线的兼容问题；SubScene 里的光照/UI 有已知限制，先用最小场景验证。

---

## MOD-006 · Prefab 与 Override 工作流

**为什么学**：Prefab 是团队协作的骨架，但大多数人只用最基础的功能。2022.2 补上了"换资产、排序、删子物体、清理无用 Override"，这些直接决定你改一套资源会不会炸掉场景。

> **学完你能得到什么（预期）**
>
> - **能做出**：两个共用同一 Prefab 的实例，替换其中一个的 Prefab 资产后，位置/旋转/缩放与命名匹配的 Override 仍在；并清理掉场景里所有 Unused Overrides。
> - **能解释**：Override 是"差异记录"；替换资产按名称匹配；无用 Override 为什么可能把不再使用的资源打进构建包。
> - **能迁移**：美术换新版模型/材质时，你能整批替换而不丢关卡里手工调整过的位置和参数。

**核心机制**

- 2022.2 新增：`PrefabUtility.ReplacePrefabAssetOfPrefabInstance`、`PrefabUtility.ConvertToPrefabInstance`（官方博客明确给出）。
- 新增复制能力：Inspector 里对象字段替换资产；Hierarchy 右键菜单替换；普通 GameObject 转 Prefab 实例；删除子 GameObject 作为 Override；Inspector 内重排新增组件（无公开 API）。
- Unused Overrides 会被标记并可批量移除，但 Unity **不会自动删除**（原因不可推断）。

**动手步骤**

1. 做一个 Prefab（例如一个带 3 个子物体的门），在场景放两个实例，分别改颜色和位置。
2. 做第二个 Prefab（结构相似、名字一致、颜色不同）。
3. 选中实例 1，用 Inspector 的新对象字段替换 Prefab 资产；观察哪些 Override 保留、哪些丢失。
4. 在实例 2 里删除一个子 GameObject，然后 Revert / Apply，观察删除作为 Override 的行为。
5. 在 Hierarchy 的 Overrides 下拉里找到 Unused Overrides 并清理；对比清理前后场景文件里是否还有失效引用。

**验收**：能演示"替换资产后保留位置与命名匹配的 Override"；能说清为什么不要自动删 Unused Overrides。

**迁移到真实项目**：版本迭代时换整套 UI/角色模型，这是最常用的场景。

**常见坑**：组件重排在 Inspector 可做但无公开 API，别指望写脚本自动化。

---

## MOD-007 · Addressables + 内存分析

**为什么学**：资源直接放进场景，会让包体变大、内存只增不减。Addressables 把"资源在哪、什么时候加载、什么时候释放"变成可控的流程；Memory Profiler 让你用数据证明优化有效。

> **学完你能得到什么（预期）**
>
> - **能做出**：一个 Prefab 不再被场景直接引用，而是通过地址按需加载、实例化、卸载；用 Memory Profiler 抓加载前/后快照并 Compare，指出新增的内存。
> - **能解释**：Addressables 的 key/address 与直接引用的区别；多进程 AssetBundle 构建为什么更快；Build Report 能告诉你什么。
> - **能迁移**：分包加载、DLC、按需下载、减少首包体积——都是这套流程的延伸。

**核心机制**

- Addressables 2022 LTS 能力：多进程 AssetBundle 构建管线、Build Report、Profiler Module、USD 互通。
- Memory Profiler 1.0：抓快照、检查、比较应用内存与编辑器内存。

**动手步骤**

1. 装 `com.unity.addressables` 与 `com.unity.memoryprofiler`。
2. 把一个 Prefab 标记为 Addressable，记住它的 address。
3. 写加载代码（API 名以本机包为准，常见入口是 `Addressables.LoadAssetAsync<T>(key)`），Play 时加载并实例化 Prefab，退出时释放。
4. 打一次 Build，打开 Build Report，记录构建耗时与产物。
5. 打开 Memory Profiler：抓"加载前"快照 → 加载并实例化 → 抓"加载后"快照 → Compare，看新增对象与内存。
6. 卸载后再抓一份，确认内存回落。

**验收**：三份快照 + 一段能说明差异的笔记；能解释为什么"直接引用"不利于按需加载。

**迁移到真实项目**：把最大的一类资源（贴图/音频/关卡）先迁到 Addressables，用快照验证收益。

**常见坑**：地址写错导致加载失败；加载后不释放导致内存持续上涨——这正是本模块要教会你的。

---

## MOD-008 · UI Toolkit 与编辑器扩展

**为什么学**：2022.1 起 UI Toolkit 同时用于运行时 UI 和编辑器工具；你学会一套，就能同时做游戏 HUD 和自定义 Inspector。


> **学完你能得到什么（预期）**
> - **能做出**：一个用 UI Builder 搭的 HUD（文本 + 按钮），改样式不用改 C#；再给自己的组件写 `PropertyDrawer`，不写自定义 Inspector 也能在面板里正常显示。
> - **能解释**：UXML/USS 与 IMGUI 的分工；默认 Inspector + Property Drawer 为什么能减少重复代码；Vector Drawing API 能做什么。
> - **能迁移**：团队自定义编辑器工具、数据配置面板、可换肤的游戏 UI。

**核心机制**
- UI Toolkit 提供 TreeView、Vector Drawing API / Painter2D、渐变、视觉搜索查询；2022 LTS 里 Property Drawers 与默认 Inspector 更完善。
- 关注点分离：结构（UXML）、样式（USS）、逻辑（C#）分开，和 IMGUI 的"代码即界面"不同。

**动手步骤**
1. 建一个 UI Document，用 UI Builder 放一个 Label 和一个 Button，存为 UXML；在 USS 里改颜色/字号，看运行时变化。
2. 写 C# 绑定按钮点击事件，改变 Label 文本。
3. 建一个自定义 Serializable 类字段，写 `PropertyDrawer` 让它按你的方式显示，确认不需要自定义 Inspector。
4. 试一次 Project 窗口的视觉搜索查询，找某个脚本。
5. 观察进出 Play 模式的响应速度（2022 LTS 有提速）。

**验收**：HUD 可交互；Drawer 生效；能说清"什么时候该用 UI Toolkit，什么时候 IMGUI 更省事"。

**迁移到真实项目**：把配置项做成可复用的编辑器面板，减少手改代码。

**常见坑**：运行时 UI 的分辨率缩放与面板布局要单独测，别照搬编辑器界面的写法。

---

## MOD-009 · Netcode for Entities（选做）

**为什么学**：2022 LTS 首次把基于 ECS 的 Netcode 作为生产支持包发布。它决定"多人游戏能不能扩展"，但学习成本高，所以放最后且选做。

> **学完你能得到什么（预期）**
> - **能做出**：本地两个客户端，一个 Cube 的位置在两边同步；或跑通官方 Megacity 示例的一部分。
> - **能解释**：服务端权威与客户端预测的区别；为什么基于 ECS 的 netcode 能支撑更多实体与玩家；Relay / Lobby / Matchmaker 各自解决什么问题。
> - **能迁移**：多人玩法的基础架构认知——即使你不做联网游戏，也能判断什么功能该放服务端。

**核心机制**
- `com.unity.netcode`（Netcode for Entities）基于 Entities，2022 LTS 起生产支持。
- UGS 服务：Relay、Lobby、Game Server Hosting、Matchmaker、Voice/Text Chat。

**动手步骤**
1. 装 `com.unity.netcode`（版本以本机为准）。
2. 优先跑包内 Samples；需要 GitHub 下载的官方示例（如 Megacity）**跳过**——你的网络条件下不划算。
3. 本地起两个 Client，同步一个 Cube 的变换。
4. 记录服务端权威与客户端预测在代码里的体现位置。

**验收**：同步成功；能口头解释服务端权威；做不到就写清卡在哪一步。

**迁移到真实项目**：先决定"哪些状态必须服务端说了算"，再选网络方案。

**常见坑**：需要账号/云服务的部分本学习项目不碰；别在本机环境上卡太久。

---

## MOD-010 · HDRP / VFX / 平台能力（阅读级）

**为什么学**：这些能力真实项目里很重要，但你的硬件与当前项目（URP）不适合实机做。这里的预期不是"做出画面"，而是**能做技术选型判断**——这同样是能力。

> **学完你能得到什么（预期）**
> - **能做出**：一份 6–8 条的选型笔记，每条写清功能名、适用场景、本机为何不能实机验证。
> - **能解释**：HDRP 的 Water System / Volumetric Materials / Volumetric Clouds 解决什么问题；VFX Graph 的 6-way lighting / Instancing / Boolean ports / Timeline 集成各解决什么问题；DX12、WebGL 原生多线程、增量构建分别带来什么。
> - **能迁移**：以后看到需求"要做水面/体积雾/大量特效"，你能立刻判断该选 URP 还是 HDRP，以及代价是什么。

**核心机制**
- HDRP 与 URP 互斥；HDRP 面向高保真平台，成本更高。
- 2022 LTS 平台能力：DX12（Windows/Xbox）、主机增量构建、WebGL 内存管理与原生 C++ 多线程、XR 的 XRI 2.3 / AR Foundation 5.1（含 Play 模式设备模拟）。

**动手步骤**
1. 打开 Unity 官方 2022 LTS 的 artists / 2022-lts 页面，逐条读上述功能。
2. 每条写三行：功能名 → 它解决什么问题 → 本机不能实机的具体原因（URP 项目 / 显存 / 无主机设备 / 无 XR 设备）。
3. 最后写一句总结：什么情况下你会考虑把项目转成 HDRP。

**验收**：`Evidence/reading-notes.md` 有 6–8 条，且最后一句选型结论是你自己的判断。

**迁移到真实项目**：技术选型报告，比会点工具更值钱。

**常见坑**：不要把"读过文档"写成"已掌握"；阅读级就是阅读级。

---

## 附录 · 你刚遇到的 CS0006 到底怎么回事

这不是玄学，是 MOD-001 的实战案例：

1. 磁盘上的 `com.unity.collab-proxy@2.12.4` 里，DLL 位于 `Lib/Editor/` **根目录**。
2. 但 Unity 的编译缓存（`Library/Bee`）里记录的 analyzer 路径仍是旧布局 `Lib/Editor/PlasticSCM/`。
3. 编译器按缓存里的路径找文件 → 找不到 → `error CS0006`。
4. 处理：删掉 `Library/Bee` 与 `Library/ScriptAssemblies`（都是可再生缓存），重启 Unity 或 `Assets → Refresh` 强制重编译。
5. 你本地还留着旧布局的 2.0.3 / 2.2.0 包缓存，所以升级后更容易触发这类残留。

**这个案例的教学价值**：以后遇到"文件明明在却找不到"，先怀疑缓存路径，而不是重装软件。

---

银橙AI出品，官网 https://yinchengai.cn/，反馈邮箱：yincheng@yinchengai.cn，微信公众号：银橙AI。
