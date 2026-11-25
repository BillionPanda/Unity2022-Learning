# MOD-003 进阶 · URP 14 你可能还不知道的能力

> 依据：本机包 `com.unity.render-pipelines.universal@14.0.9`（Unity 2022.3.14f1c1）的**包内 `Documentation~` 一手文档**与 `CHANGELOG.md`，逐条核对，不用网上旧教程。
> 前提：你已经知道 URP 是什么、会改 URP Asset 的常规开关。下面全部是**超出"切个后处理、调个灯光"**的部分。
> 对应 GP-07（图形学）：这些是"引擎把图形学封装成什么、边界在哪"的具体样本。

---

## 先校正一个常见误解

URP 的"新功能"分两层，**很多人混着记**：

| 层 | 是什么 | 例子 |
|---|---|---|
| **URP 14（= Unity 2022.2）新增** | 这个版本才有的 | Full Screen Pass Renderer Feature、Forward+、Rendering Layers 扩展到 Decal、64-bit HDR、SSAO 改进 |
| **更早就有但常被忽略** | 老功能，教程少讲 | Camera Stacking、Decal 两种技术的取舍、LOD Cross-fade 的开关位置 |

本文件两层都收，但**逐条标注来源**，不把老功能冒充新功能。

---

## 你现在工程的实际状态（已核对）

- 生效档位：**URP-HighFidelity**（`GraphicsSettings.asset` 里 `m_CustomRenderPipeline` 的 GUID `7b7fd912…` 对应它）
- 三档并存：`URP-Performant` / `URP-Balanced` / `URP-HighFidelity`
- HighFidelity：`MSAA = 4`、HDR 开、HDR 精度 0（32-bit）、Depth/Opaque Texture 都关、SRP Batcher 开
- Balanced：`MSAA = 1`、HDR 开；Performant：`MSAA = 1`、HDR 关
- 三档都**没有配置任何 Renderer Feature**（`m_RendererFeatures` 空）
- ⚠️ 渲染路径字段在这版 Renderer 资产里不叫 `m_RenderingPath`，我未从 YAML 直接读到值 → **以 Inspector 里的 Rendering Path 下拉为准，标"待核验"**

---

## 能力 1：Forward+ —— 打破"每个物体最多 8 盏灯"

**这是 URP 14 最被低估的升级。** 网上大量教程还在讲"Forward 每物体 8 灯限制"，那是旧约束。

> **学完你能得到什么**
>
> - **能做出**：一个物体同时被十几盏点光照射，不再出现"多出来的灯对它无效"；无需为了绕开限制把大网格拆碎。
> - **能解释**：Forward+ 取消了**每物体**灯数上限，但**每相机**上限仍在——桌面/主机 **256**、移动 **32**、OpenGL ES 3.0 及以下 **16**。这是两件事，别混为一谈。顺带解锁：混合超过 2 个反射探针、ECS 下多灯、更灵活的 procedural draw。
> - **能迁移**：夜景街景、大厂房、火把多的地牢——这些场景过去必须上 Deferred 或拆网格，现在 Forward+ 可保留 Forward 的 MSAA 与透明排序优势。

**怎么做**：选中 `URP-HighFidelity-Renderer.asset`（或你用的那档 Renderer）→ Inspector 里把 **Rendering Path** 切到 **Forward+** → 场景里放 12+ 个点光源围绕同一个物体 → 对比切换前后的受光数量。

**验收**：同一个球体，Forward 下只吃到前几盏灯，Forward+ 下全部灯都生效；能说出两个层级的上限分别是多少。

**坑**：官方文档写明 14.0.7 起 Forward+ 才支持 **XR 渲染和正交投影相机**；本机 14.0.9 覆盖这个修复。但 CHANGELOG 也记录了 Forward+ 下反射探针更新、ShaderGraph "vector 隐式截断"警告等历史 bug（已修），升级补丁别停在太旧版本。

---

## 能力 2：Rendering Layers —— 它和 GameObject Layer 不是一回事

**这是最容易搞混、也最有用的一项。** 大多数人以为"灯光层 = Layer 掩码"，其实 URP 14 里它是独立的一套 **Rendering Layer Mask**，在 Mesh Renderer 的 Additional Settings 里，不在 GameObject 的 Layer 下拉里。

> **学完你能得到什么**
>
> - **能做出**：红灯只照 A 球、绿灯只照 B 球；更进一步——**Custom Shadow Layers** 能让"灯不影响这个物体，但这个物体照样投它的影子"（官方举的例子）。
> - **能解释**：开启路径 = URP Asset → Lighting → ⋮ → Show Additional Properties → **Use Rendering Layers**；层名在 Project Settings → Graphics → **URP Global Settings → Rendering Layers (3D)** 里改，不是 TagManager。
> - **能迁移**：手电筒只照亮特定物件、UI 3D 模型的独立打光、阴影投射与受光解耦的特殊效果。

**怎么做**：照包内 `Documentation~/features/rendering-layers.md` 的四步做——把 Rendering Layer 1 改名 Red、2 改名 Green；两盏 Point Light 分别设 Red/Green；两个球的 Mesh Renderer → Rendering Layer Mask 各自排除对方那层。

**验收**：两球颜色各归各灯；打开 Custom Shadow Layers 后，被排除的物体仍投影。

**性能边界（官方明确）**：层数**超过 8 的倍数**时性能影响会跳一档（8→9 比 9→10 影响更大）。给 Decal 用 Rendering Layers 会增加内存带宽。只在 Forward 下给灯用，开销可忽略。

---

## 能力 3：Full Screen Pass Renderer Feature —— 低代码自定义后处理

**URP 14 新增。** 以前想做自定义后处理得自己写 `ScriptableRenderPass` + 管理 RTHandle；现在可以只写一个 shader 塞进 Renderer Feature。

> **学完你能得到什么**
>
> - **能做出**：不写 C# 就实现灰度、自定义雾、暗角等全屏效果；能做到 Unity 默认雾**做不到的"影响天空盒"**（官方就是拿这个当示例）。
> - **能解释**：三个注入点——Before Rendering Transparents / Before Rendering Post Processing / After Rendering Post Processing（默认）；Requirements 决定要不要额外 pass（Depth / Normal / Color / Motion），**Color 是默认**，对应 shader 里的 `_BlitTexture`。
> - **能迁移**：风格化滤镜、屏幕水滴、受击闪白、自定义调色——都能走同一套低代码路径。

**怎么做**：Renderer Data → Add Renderer Feature → Full Screen Pass → Pass Material 挂一个全屏 shader → 选 Injection Point 和 Requirements。官方低代码教程在 `Documentation~/post-processing/post-processing-custom-effect-low-code.md`。

**验收**：同一个场景加/不加这个 Feature，画面差异可见；把注入点从 After Post Processing 改到 Before Transparents，能解释先后顺序变化。

**你可能不知道的坑**：Unity 14 起所有全屏绘制改用 **Blitter API** 替代 `cmd.Blit`——老教程里的 `cmd.Blit` 在 XR SPI 下会错误地改 shader 关键字。另外 `ScriptableRendererFeature` 新增 `SetupRenderPasses`，**在 `AddRenderPasses` 里访问 render target 会报错**，必须挪到 `SetupRenderPasses`。这两条是 2022.2 升级指南里写死的迁移要求。

---

## 能力 4：TAA 的"互斥清单" —— 开关前先看这个

TAA 在 14 里可用，但它有一组**硬性互斥**，很多人开了 TAA 才发现别的特效失效，然后以为是自己配错了。

> **学完你能得到什么**
>
> - **能做出**：在抖动/闪烁严重的细线、植被场景里，用 TAA 消除时域抖动；并知道什么时候**不该**用它。
> - **能解释**：TAA 用颜色历史缓冲跨帧平滑，所以快速移动的物体前会出现 **ghosting（拖影）**；它依赖 Motion Vectors 来抑制闪烁。
> - **能迁移**：静态建筑漫游适合 TAA；有大量快速移动物体的战斗场景要谨慎。

**官方明确的"不能和 TAA 同用"清单**：
1. **MSAA**（硬件抗锯齿，与后处理 TAA 冲突）
2. **Camera Stacking**
3. **Dynamic Resolution**

**验收**：把相机的 Anti-aliasing 依次切 None / FXAA / SMAA / TAA，对同一个含细线的画面截图对比；再试着同时开 TAA 和 MSAA，能说明为什么不行。

**坑**：你当前 HighFidelity 档 `m_MSAA = 4`——**要试 TAA 就得先把 MSAA 关掉**，否则配置自相矛盾。CHANGELOG 里还记录了 TAA 进出 Play 模式的资源泄漏、Very High 档闪烁等问题（14.0.8 已修），本机 14.0.9 覆盖。

---

## 能力 5：LOD Cross-fade —— 开关不在你以为的地方

> **学完你能得到什么**
>
> - **能做出**：相机远近移动时，两个 LOD 级别之间**平滑过渡**，而不是"啪"地跳一下（popping）。
> - **能解释**：它是基于物体到相机距离的混色，解决 LOD 突变；代价是过渡期同时绘制两级网格。
> - **能迁移**：大面积植被、远处建筑群等需要 LOD 又怕跳变的场景。

**怎么做**：开关在 **URP Asset**（不是 Camera、不是 Renderer）的 LOD Cross Fade 属性；场景里需要一个带 LODGroup 的模型，两级网格差别明显才看得出来。

**验收**：关掉时相机后退能看到明显跳变；打开后过渡平滑。

---

## 能力 6：Camera Stacking —— Base + Overlay 的叠加渲染

> **学完你能得到什么**
>
> - **能做出**：官方 Samples 里的三类样例——`3D Skybox`、`MixedFOV`、`SplitScreen`。典型用途是"2D UI 里嵌一个 3D 模型"或"载具座舱"。
> - **能解释**：一个 Base Camera + 若干 Overlay Camera，Overlay 按列表顺序盖在 Base 上；脚本里通过 `camera.GetUniversalAdditionalCameraData().cameraStack.Add(overlayCam)` 操作（官方给的写法）。
> - **能迁移**：分屏多人、小地图、武器视图（独立 FOV）——都知道该往哪套。

**怎么做**：Package Manager → URP → Samples → 导入 `CameraStacking`，直接跑三个示例场景看结构。

**验收**：能说清为什么 Stack 顺序会造成 overdraw（官方特意警告过）；能指出它和 TAA 互斥。

---

## 能力 7：Decal 的两种技术 —— 不是只有一种贴花

> **学完你能得到什么**
>
> - **能做出**：Official Samples 的 `BlobShadow` / `PaintSplat` / `ProxyLighting` 三套贴花效果；贴花能沿网格起伏、参与光照。
> - **能解释**：**DBuffer** vs **Screen Space** 的取舍——DBuffer 需要 DepthNormal prepass，在 tile-based GPU（移动）上更贵，还不支持粒子和地形细节；Screen Space 用深度重建法线，官方**推荐移动平台用**，但只支持法线混合，Deferred + Accurate G-buffer normals 下不支持法线混合。
> - **能迁移**：弹孔、血迹、地面标识、遮蔽接缝——知道该按平台选哪种技术。

**怎么做**：Renderer Data 加 **Decal Renderer Feature** → 选 Technique（Automatic / DBuffer / Screen Space）→ 建材质用 `Shader Graphs/Decal` → 加 Decal Projector 摆到墙面上。

**验收**：同一张贴花在 DBuffer 和 Screen Space 下分别渲染，能说出视觉/性能差异；能解释为什么透明表面不受支持。

**性能红利（官方写的）**：Decal 设计上**不支持 SRP Batcher**（因为用 Material property blocks），但可以靠 **GPU Instancing** 合批——同材质 + 勾 Enable GPU Instancing，多个贴花一次 instanced draw call。配合图集（atlas）+ projector 的 UV offset，能把材质数量压到最低。

---

## 能力 8：64-bit HDR 与 Shader Stripping —— 两个"改一个开关"的优化

> **学完你能得到什么**
>
> - **能做出**：把 HDR 精度从 32-bit 提到 64-bit，减少色彩 banding（尤其蓝—黄渐变）；再关掉 Light Cookies 把 shader variant 数量砍下来。
> - **能解释**：64-bit 内存占用是 32-bit 的**两倍**，移动端性能影响明显；它只改**内部 HDR 渲染精度**，不等于 HDR 输出。Light Cookies 开关生效的前提是**所有** URP Asset（Graphics 和 Quality 里挂的）都关掉，Unity 才会 strip 相关 variant，官方说**最多能减半**。
> - **能迁移**：渐变天空/影视感画面用 64-bit；移动端包体/首屏编译优化用 stripping。

**怎么做**：URP Asset → Quality → HDR → **HDR Precision**；Lighting → **Light Cookies** 关掉。你当前三档的 `m_HDRColorBufferPrecision: 0`（32-bit），Performant 档 HDR 直接关。

**验收**：一个明显 banding 的渐变面，切换精度前后对比；用 Shader 编译统计（或 Profiler）看 variant 数量变化。

---

## 能力 9：SSAO 改进 —— 老功能但参数全变了

> **学完你能得到什么**
>
> - **能做出**：用 **Falloff Distance** 让远处物体不吃 SSAO 计算，用 **Downsample** 同时影响 AO 与后续模糊 pass来提速。
> - **能解释**：新增 **Blur Quality**（High/Medium/Low）、**Method** 里新增 **Blue Noise** 算法；勾 After Opaque 时最后一个 blur pass 会和 After Opaque pass 合并。
> - **能迁移**：室内场景的角落阴影、角色与地面的接触阴影——知道质量/性能旋钮在哪。

**验收**：打开 SSAO，逐个调 Falloff Distance 和 Blur Quality，能指出画质与耗时的取舍方向。

---

## 能力 10：给自定义 Renderer Feature 的"正确姿势"

**这条是给你写代码时避坑的，不是画面效果。**

URP 14 因 RTHandle 系统改造，旧教程的写法会直接报错：

| 旧写法（错） | 新写法（对） |
|---|---|
| `RenderTargetHandle` | `RTHandle` |
| `renderer.cameraColorTarget` | `renderer.cameraColorTargetHandle` |
| 在 `AddRenderPasses` 里访问 render target | 挪到新增的 `SetupRenderPasses`；`EnqueuePass` 仍留在 `AddRenderPasses` |
| `cmd.GetTemporaryRT` | `RenderingUtils.ReAllocateIfNeeded` 或 `RTHandles.Alloc` |
| color/depth 合成一个 ID | **必须分开**两个 RTHandle；depth 的 `depthBufferBits` 设 0 用于纯 color |

另外 **Intermediate Texture** 属性：Renderer Feature 若没用 `ScriptableRenderPass.ConfigureInput` 声明输入，就得选 Always，否则选 Auto。官方明确：新建 Renderer 资产默认 Auto，老资产为了兼容默认 Always。

**验收**：写一个最小 Full Screen Pass Feature，能正确跑起来且不触发"target before allocation"错误。

---

## 优先级建议（如果只做三件）

| 顺序 | 能力 | 为什么先做 |
|---|---|---|
| 1 | **能力 4 TAA + 能力 1 Forward+** | 都是"一个开关 + 肉眼可见差异"，但注意 TAA 要先关你当前档的 MSAA |
| 2 | **能力 2 Rendering Layers** | 概念最反直觉（和 GameObject Layer 不同），一旦搞懂能迁移到很多打光场景 |
| 3 | **能力 3 Full Screen Pass** | URP 14 招牌新功能，低代码出效果，还顺带学会注入点概念 |

---

## 依据与边界

- 全部能力来自本机包 `14.0.9` 的 `Documentation~/`（含 `whats-new/urp-whats-new.md`、`features/rendering-layers.md`、`customize/custom-pass-injection-points.md`、`renderer-features/renderer-feature-full-screen-pass.md`、`renderer-feature-decal.md`、`anti-aliasing.md`、`upgrade-guide-2022-2.md`）与 `CHANGELOG.md`。
- **待核验**：当前生效 Renderer 的 Rendering Path 具体值——YAML 里未找到 `m_RenderingPath` 字段，以 Inspector 下拉为准，本文不替你断言当前是 Forward 还是 Deferred。
- 本机 URP 版本是 **14.0.9**，CHANGELOG 里只列到 14.0.8（"Version Updated due to related graphics package"），所以 14.0.9 的功能集按 14.0.8 + 版本更新理解。
- 你当前三档 Renderer **都没配任何 Renderer Feature** → 能力 3/7 需要你先手动 Add Renderer Feature，本文件只写方法，不改你工程设置。
- 本文件只讲"有什么能力/怎么验"，不是场景实现。每项"验收"是你判断学会的标尺。

---

银橙AI出品，官网 <https://yinchengai.cn/>，反馈邮箱：yincheng@yinchengai.cn，微信公众号：银橙AI。
