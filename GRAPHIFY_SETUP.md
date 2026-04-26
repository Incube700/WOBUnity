# GRAPHIFY_SETUP.md — Unity 6 + Codex + Graphify workflow

Graphify is used to reduce AI context/token waste and help Codex understand the Unity project structure faster.

Official repository: `safishamsi/graphify`  
Official PyPI package: `graphifyy`  
CLI command: `graphify`

## What Graphify does

Graphify reads the project and creates:

```text
graphify-out/
  graph.html       interactive graph
  GRAPH_REPORT.md  high-level architecture/community report
  graph.json       persistent queryable graph
  cache/           local cache
```

For this Unity project, AI agents should read `graphify-out/GRAPH_REPORT.md` before searching raw files.

## Install on macOS

Recommended:

```bash
uv tool install graphifyy
```

Then install Graphify integration for Codex:

```bash
graphify install --platform codex
```

Alternative:

```bash
pipx install graphifyy
graphify install --platform codex
```

Plain pip is possible, but may require PATH fixes:

```bash
pip install graphifyy
python -m graphify install --platform codex
```

## Codex config requirement

Codex users need `multi_agent = true` under `[features]` in:

```text
~/.codex/config.toml
```

Example:

```toml
[features]
multi_agent = true
```

## Build the project graph

From the Unity repository root:

```bash
cd /path/to/WOBLearnUnity
graphify .
```

In Codex, the skill command is:

```text
$graphify .
```

Not `/graphify .` — Codex uses `$`.

## Keep Graphify focused

This repo includes `.graphifyignore`.

It excludes Unity generated/heavy folders:

```text
Library/
Temp/
Obj/
Build/
Builds/
Logs/
UserSettings/
```

It also excludes local graph cache/cost files and AI instruction files from semantic extraction.

## Commit policy

Recommended to commit:

```text
graphify-out/GRAPH_REPORT.md
graphify-out/graph.json
graphify-out/graph.html
```

Recommended to ignore/avoid committing:

```text
graphify-out/cache/
graphify-out/manifest.json
graphify-out/cost.json
```

## Update graph after changes

For code-only changes:

```bash
graphify update .
```

For big refactors or new docs:

```bash
graphify . --update
```

Optional git hook:

```bash
graphify hook install
```

This can rebuild/update the graph after commits and branch switches.

## Recommended Codex prompt

```text
Read AGENTS.md, AI_CONTEXT_GRAPH.md, and graphify-out/GRAPH_REPORT.md if it exists.
If graphify-out does not exist, run $graphify . first.
Then work on GitHub issue #9 in branch feature/ricochet-tanks-prototype.
Keep changes isolated under Assets/_Project/RicochetTanks/.
Do not scan the whole repository unless the graph/report is insufficient.
```

## Workflow for Серёжа

1. Open terminal in repo root.
2. Run Graphify once.
3. Commit graph output if useful.
4. Tell Codex to use the graph before editing.
5. Give Codex small tasks, not the whole game at once.
6. After Codex changes files, open Unity 6 and check compile/play mode.
7. Send PR/diff back for review before merging.
