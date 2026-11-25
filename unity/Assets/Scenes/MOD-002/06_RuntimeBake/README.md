# 06 运行时烘焙与增量更新

**学完你能得到**：能做出「运行时铺板后角色立刻能走」，并区分 `BuildNavMesh()`（全量）与 `UpdateNavMesh(data)`（增量）。

**操作**：打开 `RuntimeBakeDemo.unity` → Play。两岸分离、木板初始禁用，角色过不去。按 **空格** 启用木板并重建导航，角色立刻过桥；再按一次移除。

**验收**：能说清程序化地图为什么不能完全依赖编辑器预烘焙；在 Profiler 里对比全量与增量的帧时间（可把脚本里的 `BuildNavMesh()` 改成 `UpdateNavMesh(surface.navMeshData)` 自行实验）。
