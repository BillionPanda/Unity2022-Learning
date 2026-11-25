# 08 动画与导航解耦

**学完你能得到**：能做出「角色转身有平滑过渡」，并解释 `updateRotation` 把哪部分控制权交还给动画层。

**操作**：打开 `AnimationDecoupleDemo.unity` → Play。角色前方的橙色小块指示朝向。按 **R** 切换：开=脚本用 `Quaternion.Slerp` 按速度方向平滑转；关=Unity 每帧硬拧向路径方向。

**验收**：能说清第三人称 / 受击 / 瞄准动画为什么必须关掉 `updateRotation`；能指出 `angularSpeed`、`autoTraverseOffMeshLink` 各自的控制边界。
