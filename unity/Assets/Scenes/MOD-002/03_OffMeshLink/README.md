# 03 Off-Mesh Link：抛物线跳跃

**学完你能得到**：能做出角色跳过沟渠的弧线，并解释 Link 是「图上的一条边」、`autoTraverseOffMeshLink` 为什么是关键开关。

**操作**：打开 `OffMeshLinkDemo.unity` → Play → 点右侧平台红球。按 **J** 切换：开=自己用协程画抛物线；关=Unity 默认瞬移。

**验收**：角色过沟时有可见弧线、落地后继续寻路；能说清 `currentOffMeshLinkData` / `CompleteOffMeshLink()` 的职责。

**注意**：本脚本的抛物线实现与官方 `AgentLinkMover.cs` 的 Parabola 分支同构，但没有引用包内示例脚本——完全自包含。
