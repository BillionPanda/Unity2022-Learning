# 07 多 Surface 显式连接

**学完你能得到**：能做出「两块分开烘焙的地面用 Link 连起来」，并解释多 Surface 之间不会自动连通。

**操作**：打开 `MultipleSurfacesDemo.unity` → Play → 角色从 A 岛走到 B 岛。按 **L** 禁用 Link 组件，再点目标，角色到不了对岸。

**验收**：能说清叠加加载 / 分块地图为什么必须显式 Link；能解释 Link 的 `startPoint` / `endPoint` 是相对该 GameObject 的局部坐标。
