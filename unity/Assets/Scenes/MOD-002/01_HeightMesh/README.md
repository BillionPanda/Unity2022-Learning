# 01 HeightMesh：台阶贴合

**学完你能得到**：能说清「NavMesh 管找路，HeightMesh 管落脚高度」，并用肉眼对比看到差异。

**操作**：
1. 打开本目录 `HeightMeshDemo.unity`，点 Play。角色会走向台阶顶端的红球。
2. 观察角色在台阶上是「滑上斜坡」还是「一级一级踩」——此时 H=关。
3. 按 **H** 切换 HeightMesh 并立即重烘焙，再看角色脚部高度读数。

**验收**：能解释为什么关掉 HeightMesh 时台阶在导航网格里变成斜坡；能指出烘焙耗时/内存代价。

**注意**：本场景所有几何由 `Scripts/HeightMeshDemo.cs` 在 Awake 中生成，场景文件本身只放相机、灯和 Bootstrap 对象。
