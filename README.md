# Unity 2022 学习 · game-programmer 验证工程

本项目有两个身份：

1. **Unity 2022 LTS 新功能学习**：`docs/Unity2022-你的待办清单.md`（MOD-001~010）
2. **game-programmer 书单验证**：`docs/GameProgrammer-Unity验证映射.md`（GP-01~11）

工程源码在 `unity/`（Assets / Packages / ProjectSettings），只含源码与配置，
Unity 的 Library/Temp/Logs 缓存未纳入版本控制。

自检脚本（只读）：

```bash
bash docs/verify_unity2022_project.sh unity/
```
