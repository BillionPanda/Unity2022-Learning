# 02 多体型：两条不同的路

**学完你能得到**：能做出「小兵钻窄缝、坦克绕宽道」，并解释 Agent Type 决定烘焙体素化与半径。

**前提**：项目 `ProjectSettings/NavMeshAreas.asset` 里需有第二个 Agent Type（Tank，Radius 1.5）。生成脚本已写入；如果你改过导航设置，请到 Project Settings → Navigation → Agents 核对。若只有一个类型，场景会在左上角给出警告，两个角色都按 Humanoid 烘焙。

**操作**：打开 `AgentSizesDemo.unity` → Play → 点墙另一侧的红球。绿色小兵会从 1m 窄缝穿过，橙色坦克只能走右侧宽通道。

**验收**：同一点下达目标，两条路径肉眼可见不同；把 Tank 半径改大后窄缝彻底不可通行。
