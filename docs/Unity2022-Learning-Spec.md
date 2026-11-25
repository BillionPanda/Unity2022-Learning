# Unity 2022 新功能熟悉项目 · 阶段一学习规格与路线图

> 版本：v0.1 ｜ 状态：**draft（未冻结）** ｜ 日期：2026-11-25 ｜ 角色：银橙AI（Unity 游戏开发专家）
> 本文件是阶段一产物：把「熟悉 Unity 2022 新功能」收敛成可复核、可冻结、可直接照做的学习规格。**本轮不建 Unity 工程**（阶段二门禁未过）。

---

## S1 结论摘要

### 需求收敛表

| 维度 | 收敛结果 | 来源 |
|---|---|---|
| 项目性质 | 学习/探索型项目，不是商业游戏产品 | 用户本次陈述 + 【假设 H2】 |
| 目标 | 系统掌握 Unity 2022 值得注意的新功能 | 用户本次陈述 |
| 交付形态 | 阶段一：学习规格 + 冻结清单；阶段二：可运行 Unity 工程雏形（多个 mini-demo） | 本角色工作流 |
| 目标平台 | 单机 Windows（编辑器内验证为主） | 【假设 H3】/【假设 H4】 |
| 单局时长 | 不适用，改为「单模块实操时长」，见 S4 | 本项目口径 |
| 变现方式 | 无 | 【假设 H2】 |
| 引擎基线 | Unity 2022.3 LTS（2022 周期的 LTS 版本） | 官方 2022 LTS 页 + QA LTS 页 |

### 五条判断（每条带可执行动作）

1. **「Unity 2022」不是单一版本**：2022.1 / 2022.2 是 Tech Stream，2022.3 才是 LTS；官方 QA 页明确写「Unity 2022.3 LTS release has the same feature set as the 2022.2 Tech Stream release」。→ **动作**：先读本机 `ProjectSettings/ProjectVersion.txt` 锁定版本，学习基线定 2022.3 LTS，2022.1/2022.2 只作为「功能首次出现」的时间轴注解。
2. **最值得投入的是三类变化**：DOTS/ECS 首次正式生产可用、URP/HDRP 渲染管线升级、编辑器与 UI Toolkit 迭代提速。→ **动作**：先做 P0 五个模块（MOD-001/002/005/007/008），不要平均用力。
3. **不要读清单式学习**，要「每模块一个可运行 mini-demo + 一条自检证据」。→ **动作**：工程文件写 `// @trace MOD-00X`，用本轮附带的自检脚本统计覆盖率，把「是否照规格做」变成机器可查的事实。
4. **主机 / XR / 光追 / HDRP 高负载效果在你这台机器上不可实机验证**。→ **动作**：降为 P2 阅读级，只记「能力边界」，不标记为「已掌握」。
5. **本轮交付的是规格（draft），不是工程**。→ **动作**：确认 S6 待确认清单后把冻结清单置为 frozen，再进阶段二搭骨架。

### 核心循环与机制骨架（学习循环）

`核对版本 → 选模块 → 跑 mini-demo → 留证据 → 统计覆盖率 → 下一模块`

| 环节 | 输入 | 状态/判定规则 | 输出 |
|---|---|---|---|
| 核对版本 | ProjectVersion.txt、manifest.json | 版本前缀为 `2022.` 才继续；否则警告并转「版本核对清单」 | 版本记录 1 条 |
| 选模块 | 冻结清单 MOD-001…010 | 按 P0 → P1 → P2 顺序；未完成前置模块的验收不得跳级 | 模块开工单 |
| 跑 mini-demo | 该模块的官方功能名 | 必须进入 Play 模式并观察到可描述的现象 | 场景/脚本 1 份 |
| 留证据 | 运行结果 | 截图/日志/自检输出任一，写入 `evidence/` | 证据 1 条 |
| 统计覆盖率 | `// @trace MOD-` 标记 | 覆盖率 = 有标记的模块数 ÷ 10 | 自检报告 |
| 边界情况 | 功能在本机不可用 | 不改规格，标 `BLOCKED` 并写明原因（硬件/平台/包未装） | 待核实项 +1 |

---

## S2 适用口径声明

### 依据清单（本轮实际查阅，标注核验状态）

| # | 依据 | 核验状态 | 用途 |
|---|---|---|---|
| D1 | `https://unity.com/releases/2022-lts`（官方 2022 LTS 产品页，正文已抓取） | 已核验 | 功能全景、LTS 支持口径、资源链接 |
| D2 | `https://unity.com/releases/lts/programmers`（正文已抓取） | 已核验 | 程序员向功能、XRI 2.3、AR Foundation 5.1 |
| D3 | `https://unity.com/cn/releases/lts/artists`（官方中文艺术功能页） | 已核验（官方页摘要） | 2D / VFX / Material Variants |
| D4 | `https://unity.com/en/blog/engine-platform/unity-20221-tech-stream-is-now-available`（正文已抓取，发布日期 2022-05-10） | 已核验 | 2022.1 功能与发布时间 |
| D5 | `https://unity.com/blog/engine-platform/prefabs-whats-new-2022-2` | 已核验（官方博客摘要） | Prefab 新 API 名 |
| D6 | `https://unity.com/en/blog/engine-platform/building-better-paths-with-splines-in-2022-2`、`https://unity.com/roadmap/495-splines-2-0-2022-lts` | 已核验（官方摘要） | Splines 1.0 / 2.1 版本归属 |
| D7 | `https://unity.com/releases/editor/qa/lts-releases`（LTS Releases 页） | 已核验（官方摘要） | 2022.3 与 2022.2 功能集相同；支持周期至 2025 年中 |
| D8 | `https://unity.com/blog/engine-platform/unity-2022-lts-is-coming-in-june`（Editor's note） | 已核验（官方摘要） | 2022 LTS 于 2023-06-01 可用 |
| D9 | `https://docs.unity3d.com/2022.3/Documentation/Manual/com.unity.entities.html`、`.../com.unity.netcode.html` | 已核验（官方手册摘要） | 包版本与 2022.3 的对应关系 |
| D10 | `https://unity.com/releases/editor/beta/2022.2.0b6` | 已核验（官方 release notes 摘要） | `com.unity.ai.navigation`、UI Toolkit API 名 |
| D11 | `https://docs.unity3d.com/2022.3/Documentation/Manual/WhatsNew2022LTS.html` | **待核验**（链接存在，正文抓取仅返回版本选择器） | 官方「New in Unity 2022 LTS」逐条清单，需在本机/浏览器复核 |
| D12 | 官方文档链接中的包版本目录：`com.unity.render-pipelines.high-definition@14.0`、`...universal@14.0`、`com.unity.visualeffectgraph@16.0`、`com.unity.memoryprofiler@1.0` | **仅名称级**（从 D1 链接推断，未抓正文） | 只作索引，不作版本结论 |

### 引擎、版本与阶段口径

| 项 | 口径 | 状态 |
|---|---|---|
| 引擎 | Unity Editor 2022 周期 | 已核验（D1/D7） |
| 版本流 | 2022.1 Tech（2022-05-10 起）→ 2022.2 Tech → 2022.3 LTS；2022.3 与 2022.2 功能集相同 | 已核验（D4/D7） |
| LTS 支持 | 发布后两年内仅修复、不加功能/不改 API；2024 年中前双周修复，之后月度更新至 2025 年中 | 已核验（D1/D7） |
| 本机已装版本 | — | **待核验**（工作区为空，无 ProjectVersion.txt 可读） |
| 本机包版本 | 不同文档快照给出不同值（Entities 手册中文页 1.0.10、英文页 1.4.7；Netcode 手册 1.9.2，另一文档列 1.0 对应 2022.3.0f1+） | **待核验**：包版本随 `2022.3.xf1` 补丁版本变化，一律以本机 Package Manager 解析结果为准，**不写死版本号** |
| 阶段 | 阶段一（策划/学习规格），未冻结；阶段二门禁未过 | 本轮状态 |
| C# / .NET 版本、具体 API 签名 | — | **待核验**，以官方文档当前版本为准，不臆造 |

### 显式假设（结论强度随假设成立与否调整）

- **【假设 H1】**：按「以 2022.3 LTS 为学习基线、系统覆盖 2022 周期主要新功能」处理；若你实际装的是 2022.1/2022.2 Tech Stream，S3 的版本流表仍成立，但 P0 模块需重新排序，结论强度下调为「仅供参考」。
- **【假设 H2】**：按「学习用途，无上线/变现/合规要求」处理；若有发布目标，需补平台规格与商店政策章节。
- **【假设 H3】**：按「能装 Unity Hub 与 2022.3 LTS，且愿意用可运行 mini-demo 逐个验证」处理；若只想读文档，S5 全部实操动作降级为阅读清单，S4 时长预算作废。
- **【假设 H4】**：按「单机 Windows、显存有限、GitHub 下载受限」处理（来自会话背景，未本机核验）；因此 HDRP 高负载/主机/XR/光追项列为 P2 阅读级，示例资源优先走 Package Manager 内置 Samples 与 Unity Hub 模板，不走 GitHub 直链。若实际硬件/网络不同，P2 可升 P1。
- **【假设 H5】**：按「本轮不建 Unity 工程」处理；确认后进入阶段二搭骨架。

---

## S3 依据与推导

### 版本流判定表（每条挂依据）

| 版本流 | 性质 | 代表性首次能力 | 依据 | 核验状态 |
|---|---|---|---|---|
| 2022.1 | Tech Stream（2022-05-10 上线，70+ 新功能、280+ 改进） | UI Toolkit 扩展、Splines 1.0 包、Material Variants（HDRP+URP）、Sprite Atlas v2 默认、Delaunay tessellation、Frame Timing Manager、Frame Stats Profiler、IL2CPP 全共享泛型、Play 模式/导入提速 | D4 | 已核验 |
| 2022.2 | Tech Stream | Prefab 资产替换与重排序、Splines 2.1（分支 / Instantiate / Animate / Extrude / 情境工作流）、AI Navigation 包化（`com.unity.ai.navigation`）、AR Foundation 5.1、XRI 2.3 | D5/D6/D10/D2 | 已核验 |
| 2022.3 | **LTS**（2023-06-01 可用），功能集与 2022.2 相同 | DOTS/ECS 生产可用、Netcode for Entities、URP Forward+ / TAA / LOD crossfade / Decal Layers / Shader Graph 全屏主节点、HDRP Water System / Volumetric Materials / Cloud Layers、Addressables 多进程构建、Memory Profiler 1.0、DX12、WebGL 原生多线程、增量构建 | D1/D2/D7/D8/D9 | 已核验 |

### 学习模块清单（冻结清单草案，编号即阶段二 `// @trace` 目标）

| 编号 | 模块 | 关键功能名（官方措辞） | 优先级 | 依据 | 核验状态 |
|---|---|---|---|---|---|
| MOD-001 | 版本与工程基线 | ProjectVersion.txt / manifest.json 核对；2022.1→2022.2→2022.3 划分 | P0 | D1/D7/D9 | 已核验 |
| MOD-002 | URP 渲染升级 | Forward+ Rendering、LOD crossfade、TAA、Decal Layers、Shader Graph Full Screen Master Node、Shader Variant Prefiltering、Built-in Converter | P0 | D1 | 已核验 |
| MOD-003 | HDRP 与自然环境 | Water System、Volumetric Materials、Cloud Layers / Volumetric Clouds、6-way lighting、VFX Graph Instancing / Timeline 集成 / Boolean ports | P2（阅读级） | D1/D3 | 已核验 |
| MOD-004 | 材质与样条 | Material Variants；Splines 1.0（2022.1）→ 2.1（2022.2）的 Instantiate / Animate / Extrude 与分支 | P1 | D4/D6 | 已核验 |
| MOD-005 | DOTS / ECS | ECS for Unity（Entities）、Burst、C# Jobs System、Entity 与 GameObject 混用、SubScene | P0 | D1/D2/D9 | 已核验（包版本待核验） |
| MOD-006 | 多人网络 | Netcode for Entities（`com.unity.netcode`）、Relay / Lobby / Matchmaker / Game Server Hosting、Megacity Multiplayer sample | P1 | D1/D2/D9 | 已核验（包版本待核验） |
| MOD-007 | 资源与内存 | Addressables 多进程 AssetBundle 构建、Build Report、Profiler Module、USD 集成；Memory Profiler 1.0 | P0 | D1/D2 | 已核验 |
| MOD-008 | 编辑器与 UI Toolkit | UI Toolkit 运行时/编辑器统一、默认 Inspector、Vector Drawing API / Painter2D、TreeView、视觉搜索查询、Burst 编译提速、async/await 编程、Play 模式进出提速 | P0 | D1/D2/D4/D10 | 已核验（API 签名待核验） |
| MOD-009 | Prefab 与 2D | `PrefabUtility.ReplacePrefabAssetOfPrefabInstance`、`PrefabUtility.ConvertToPrefabInstance`、删除 GameObject 作为 Override、Unused Overrides；Sprite Atlas v2 / PSD Importer / Aseprite Importer / Delaunay tessellation / 2D Layer Overrides / Physics 2D Profiler | P1 | D5/D3/D4 | 已核验 |
| MOD-010 | 导航、XR 与平台 | AI Navigation（`com.unity.ai.navigation`）、XRI 2.3、AR Foundation 5.1（Play 模式设备模拟）、DX12、WebGL 原生 C++ 多线程、主机增量构建、Frame Timing Manager | P2（阅读级） | D1/D2/D10/D4 | 已核验 |

> 说明：D1 明确列出 LTS 两年内「不加功能、不改 API」，因此上述清单是 2022 周期的**稳定功能集**，不会随 2022.3 补丁版本增删；变化的是**包补丁版本**，须以本机为准（D9 的两个文档快照即为例证）。

### 可直接运行的 C# 片段（不含 2022 新增 API，仅用于版本核对）

```csharp
// Assets/Editor/Unity2022Probe.cs
// 只使用长期稳定的 Application.unityVersion / Debug.Log / MenuItem，不涉及任何 2022 新增 API 签名。
using UnityEngine;
using UnityEditor;

public static class Unity2022Probe
{
    [MenuItem("Tools/Unity2022/Print Editor Version")]
    static void Print()
    {
        Debug.Log("[Unity2022] Editor version = " + Application.unityVersion);
    }
}
```

> 若此片段或后续任何 API 编译报错，**不要猜签名**：以官方文档当前版本为准，并把该项记入 S6 待核实表。此片段只完成 MOD-001 的一半，另一半由自检脚本完成。

### 工程自检命令（本轮已落成脚本 `verify_unity2022_project.sh`，可立即运行）

```bash
bash verify_unity2022_project.sh "C:/Users/27176/WorkBuddy/spaces/2026_11_25_10-29-16"
```

脚本只读，输出：编辑器版本、关键包清单、`// @trace MOD-` 覆盖率、缺失项；不修改工程、不写入场景或预制体。

---

## S4 判断或计算链条

### 优先级判定规则（可复算）

`优先级 = f(影响面, 本机可验证性, 前置依赖)`

| 输入 | 取值 | 判定 |
|---|---|---|
| 影响面 | 架构级=3 / 工作流级=2 / 单点=1 | MOD-005=3、MOD-002=3、MOD-009=1 |
| 本机可验证性 | 可实机=1 / 仅阅读=0 | 可实机才允许进 P0 |
| 前置依赖 | MOD-001 为所有模块前置 | 未完成 MOD-001 不计入任何模块验收 |

`P0 条件：影响面 ≥ 2 且 可实机 = 1`；`P2 条件：可实机 = 0`；其余为 P1。
验算：MOD-003（影响面 2、仅阅读）→ 不进 P0，落 P2；MOD-008（影响面 2、可实机）→ P0。规则自洽。

### 学习时长预算表（单位：分钟/模块，含口径）

| 档 | 模块数 | 单模块下限 | 单模块上限 | 小计下限 | 小计上限 |
|---|---|---|---|---|---|
| P0 | 5 | 45 | 90 | 225 | 450 |
| P1 | 3 | 30 | 60 | 90 | 180 |
| P2 | 2 | 15 | 30 | 30 | 60 |
| **合计** | **10** | — | — | **345** | **690** |

- 口径：含「看文档 + 建 mini-demo + 留证据」，不含环境安装与包下载等待；单次连续实操，不含中断。
- 复核缓冲系数 `b = 1.2`（每模块收尾自检 20%）。
- **总预算** `T = 合计 × b` → 下限 `345 × 1.2 = 414` 分钟（6.9 小时），上限 `690 × 1.2 = 828` 分钟（13.8 小时）。
- 极值验算：若全部模块取下限且缓冲为 0 → 345 分钟（5.75 小时），为理论最短；若全部取上限且缓冲生效 → 828 分钟，为理论最长。**本表不承诺排期**，只给区间与口径（按本角色边界，不代替用户做时间决策）。

### 验收通过率（可复算）

`覆盖率 C = 有 // @trace MOD-00X 标记的模块数 / 10 × 100%`
`通过率 P = 同时具备「标记 + 证据 + 可运行 mini-demo」的模块数 / 10 × 100%`

- 极值验算：0/10 → 0%；10/10 → 100%；若 10 个都有标记但只有 7 个有证据 → `C = 100%`，`P = 70%`，差值 30% 即「漏做或漏留证据」的量，必须逐项定性为「真没做」还是「漏标」。
- 口径：P2 阅读级模块以「能力边界记录 1 条」代替 mini-demo，在分母中仍计 1，不免除。

---

## S5 实操动作

1. **锁定本机版本**（MOD-001，先做）：装好 Unity 2022.3 LTS 后，在工作区执行 `bash verify_unity2022_project.sh <工程路径>`；若无工程，先在 Unity Hub 建一个空的 2022.3 3D (URP) 工程并把路径传给脚本。
2. **核对包清单**：按脚本输出比对 `com.unity.entities` / `com.unity.netcode` / `com.unity.render-pipelines.universal` / `com.unity.ai.navigation` / `com.unity.splines` 是否齐全；缺的包走 Unity Package Manager 安装，**不要写死版本号**。
3. **补读官方逐条清单**：打开 D11（`WhatsNew2022LTS.html`），把 D1 未覆盖的条目补进本文件 S3，并标注「已核验」；这是本轮唯一未闭合的官方文档源。
4. **按 P0 → P1 → P2 做 mini-demo**，每个模块：建场景 → 写脚本并加 `// @trace MOD-00X` → 进 Play 模式观察现象 → 在 `evidence/` 留 1 条输出 → 跑一次自检脚本看覆盖率变化。
5. **P2 只做阅读级**：记录「该功能存在、属于哪个包、本机为何不能实机验证」，不写「已掌握」。
6. **冻结门禁**：确认 S6 待确认清单后，把本文件状态改为 `frozen v1.0`，此后新增/删模块必须递增版本并同步改 S3 与自检脚本的模块表。**冻结之前不进阶段二、不建 Unity 工程**。
7. **阶段二预告（需先冻结）**：按 `frozen` 清单搭工程骨架（ProjectSettings / manifest / 最小场景 / 程序集与目录规范），逐模块实现并跑编译 + 自检，产出覆盖率报告与失败项清单。

---

## S6 边界与待核实

### 待确认清单（编号，含影响节）

| 编号 | 待确认事项 | 影响哪一节 | 冻结建议 |
|---|---|---|---|
| Q1 | 本机实际安装的 Unity 版本（`2022.3.xf1` 具体补丁号） | S2、S3 MOD-001、S5-1 | 必须回传 `ProjectVersion.txt` 内容后冻结 |
| Q2 | 学习基线是否确认为 2022.3 LTS（而非 2022.1/2022.2） | S1、S2、S3 优先级 | 默认按【假设 H1】冻结 |
| Q3 | 是否要真建可运行工程（还是只要文档/阅读清单） | S1、S4、S5 全节 | 默认按【假设 H3】冻结为「要建」 |
| Q4 | 是否覆盖主机/XR/多人联网（涉及账号、设备、UGS 服务） | S3 MOD-003/006/010、S4 时长表 | 默认按【假设 H4】冻结为「P2 阅读级」 |
| Q5 | 渲染管线用 URP 还是 HDRP 起手 | S3 MOD-002/003、S5-1 | 默认 URP（对硬件更友好），冻结前请确认 |
| Q6 | 官方 `WhatsNew2022LTS.html` 正文（D11）是否有本清单未列的功能 | S3 全表 | 补读后冻结；未补读则本文件保持 draft |

### 六字段边界表

| 事项 | 影响 | 提供方 | 闭合方式 | 不闭合后果 | 当前状态 |
|---|---|---|---|---|---|
| 本机 Unity 版本与包版本 | 所有模块能否实机验证 | 用户本机 / Unity Hub | 回传 ProjectVersion.txt 与 manifest.json | 版本口径错，学到别版功能 | **未闭合（Q1）** |
| 官方 New in Unity 2022 LTS 正文 | 功能清单完整性 | Unity 官方文档（D11） | 浏览器打开补读并回填 S3 | 可能漏 1–N 个功能 | **未闭合（Q6）** |
| Entities / Netcode 等包版本 | MOD-005/006 示例代码 | 本机 Package Manager | 装包后读实际解析版本 | 臆造版本号、代码编译失败 | **待核验** |
| 具体 C# / URP / UI Toolkit API 签名 | 阶段二代码 | Unity 官方文档当前版本 | 写码前逐条查文档 | 编译错误、误学 | **待核验** |
| HDRP / 光追 / 主机 / XR 实机条件 | MOD-003/010 验收口径 | 用户硬件与账号 | 明确为阅读级或补充设备 | 假「已完成」 | **按【假设 H4】暂定 P2** |
| 示例资源下载渠道 | 阶段二能否拿到 Samples | Unity Package Manager / Hub | 优先内置 Samples，避免 GitHub 直链 | 环境卡住、误判功能不可用 | **按【假设 H4】暂定** |
| 设计冻结清单状态 | 阶段二开工门禁 | 用户确认 Q1–Q6 | 状态改 `frozen v1.0` | 未冻结即改工程，需求不可追溯 | **draft，门禁未过** |

### 三段式小结

- **本轮产出**：① 本文件（阶段一学习规格 v0.1，含需求收敛表、版本流判定、10 个编号模块、时长/覆盖率可复算公式、待确认清单与边界表）；② `verify_unity2022_project.sh`（只读自检脚本，输出版本、包清单、`// @trace` 覆盖率）；③ 对话内的学习路径图。首轮四样交付（结论摘要 / 适用口径声明 / 依据与推导 / 待确认清单）已齐。
- **未完成**：未补读官方 `WhatsNew2022LTS.html` 正文；未核验本机版本与包版本；**未建 Unity 工程**（门禁未过）；未写任何阶段二 C# 系统代码；未提供任何美术资源。
- **建议下一步**：回复 Q1–Q6（至少给出 Q1 版本与 Q3 是否建工程），我据此把清单置为 `frozen v1.0`，再进入阶段二搭 2022.3 LTS 工程骨架并按 MOD 编号逐个落地。

---

银橙AI出品，官网 https://yinchengai.cn/，反馈邮箱：yincheng@yinchengai.cn，微信公众号：银橙AI。
