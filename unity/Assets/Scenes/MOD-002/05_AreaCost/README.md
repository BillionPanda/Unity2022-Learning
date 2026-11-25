# 05 区域代价与 Area Mask

**学完你能得到**：能演示「能走但不愿走」（cost）和「根本不许走」（mask）两种不同效果——这是从寻路跨到决策的一步，对应 GP-10。

**操作**：打开 `AreaCostDemo.unity` → Play。中央墙把路分成左右：左侧蓝色水域是 area 3，代价 3；右侧绕行更远。绿色角色 mask 全开，蓝色角色 mask 排除 area 3。按 **C** 把代价从 3 改到 1，看绿色角色路径变化。

**验收**：能解释 `距离 × 区域代价`；能说清 Area Mask 是位掩码、`NavMesh.GetAreaCost` / `SetAreaCost` 的作用域（全局对该 area）。
