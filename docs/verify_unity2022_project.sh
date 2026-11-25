#!/usr/bin/env bash
# Unity 2022 学习工程自检脚本（只读，不修改任何工程文件）
# 用法：bash verify_unity2022_project.sh <Unity工程根目录>
# 输出：编辑器版本 / 关键包清单 / @trace 模块覆盖率 / @verify 理论验证覆盖率 / 缺失项
# 两条线独立统计：MOD = Unity 2022 新功能线；GP = game-programmer 书单验证线
# 平台：Windows Git Bash、macOS、Linux 均可运行

set -u
ROOT="${1:-.}"
MODS=(001 002 003 004 005 006 007 008 009 010)
GPS=(01 02 03 04 05 06 07 08 09 10 11)
KEY_PKGS=(com.unity.entities com.unity.netcode com.unity.ai.navigation com.unity.splines \
          com.unity.render-pipelines.universal com.unity.render-pipelines.high-definition \
          com.unity.addressables com.unity.memoryprofiler com.unity.visualeffectgraph)

cd "$ROOT" 2>/dev/null || { echo "[FATAL] 目录不存在: $ROOT"; exit 2; }

echo "=== Unity 2022 学习工程自检 ==="
echo "工程路径: $(pwd)"
echo

# --- 1) 编辑器版本 ---
echo "--- [1/4] 编辑器版本 ---"
if [ -f ProjectSettings/ProjectVersion.txt ]; then
  VER=$(grep -m1 'm_EditorVersion:' ProjectSettings/ProjectVersion.txt | tr -d '\r' | awk '{print $2}')
  echo "m_EditorVersion = ${VER:-<未解析>}"
  case "${VER:-}" in
    2022.3*) echo "判定: 2022.3 LTS OK" ;;
    2022.*) echo "判定: 2022 周期但非 2022.3（Tech Stream），学习前请确认基线" ;;
    *) echo "警告: 非 2022 周期版本，请按规格 S6-Q1 核对后再学习" ;;
  esac
else
  echo "缺失: ProjectSettings/ProjectVersion.txt（尚无 Unity 工程或路径不对）"
fi
echo

# --- 2) 关键包清单 ---
echo "--- [2/4] Packages/manifest.json 关键包 ---"
if [ -f Packages/manifest.json ]; then
  for p in "${KEY_PKGS[@]}"; do
    line=$(grep -m1 "\"$p\"" Packages/manifest.json | tr -d '\r' | sed 's/^[[:space:]]*//')
    if [ -n "$line" ]; then echo "  $line"; else echo "  [未安装] $p"; fi
  done
  echo "  说明: 包版本随 2022.3.xf1 补丁变化，一律以本机解析结果为准，不臆造版本号。"
else
  echo "缺失: Packages/manifest.json"
fi
echo

# --- 3) @trace 覆盖率 ---
echo "--- [3/4] // @trace MOD- 覆盖率（Unity 2022 新功能线）---"
if [ -d Assets ]; then
  found=$(grep -rho 'MOD-[0-9][0-9][0-9]' Assets --include='*.cs' 2>/dev/null | sort -u)
  hit=0
  for m in "${MODS[@]}"; do
    if echo "$found" | grep -qx "MOD-$m"; then
      echo "  [OK]      MOD-$m"; hit=$((hit+1))
    else
      echo "  [MISSING] MOD-$m"
    fi
  done
  echo "覆盖率 C = $hit / ${#MODS[@]} = $((hit * 100 / ${#MODS[@]}))%"
  echo "提示: 覆盖率只统计标记；通过率 P 还需逐模块核对 evidence/ 与可运行 mini-demo。"
else
  echo "缺失: Assets/（阶段二工程尚未建立）"
  echo "覆盖率 C = 0 / ${#MODS[@]} = 0%"
fi
echo

# --- 4) @verify 覆盖率 ---
echo "--- [4/4] // @verify GP- 覆盖率（game-programmer 理论验证线）---"
if [ -d Assets ]; then
  gfound=$(grep -rho 'GP-[0-9][0-9]' Assets --include='*.cs' 2>/dev/null | sort -u)
  # B/C 类笔记也算完成：Evidence/ 下的 GP-xx 笔记文件
  notes=$(ls Evidence 2>/dev/null | grep -oE 'GP-[0-9][0-9]' | sort -u)
  ghit=0
  for g in "${GPS[@]}"; do
    if echo "$gfound $notes" | grep -qw "GP-$g"; then
      echo "  [OK]      GP-$g"; ghit=$((ghit+1))
    else
      echo "  [MISSING] GP-$g"
    fi
  done
  echo "覆盖率 G = $ghit / ${#GPS[@]} = $((ghit * 100 / ${#GPS[@]}))%"
  echo "提示: G 统计 @verify 标记或 Evidence/ 下的 GP-xx 笔记；B/C 类只写笔记也算完成。"
else
  echo "缺失: Assets/（阶段二工程尚未建立）"
  echo "覆盖率 G = 0 / ${#GPS[@]} = 0%"
fi
echo
echo "=== 自检结束（只读，未修改任何文件）==="
