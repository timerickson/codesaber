CodeSaber - A hand-crafted tool for code exploration

C# only

Two varieties:
=====================================================================
1) CodeSaber.exe
- Microsoft (R) Roslyn-based basic C# REPL
  (shout out to @gblock for the inspiration)
- Scripts (no class/global type declaration required) or assemblies
- Verrry basic initial implementation but *mucho* room for growth

2) CodeSaber.Ice.exe
- csc.exe-based C# compiler
- Assembly-based only (due to csc.exe)
- Basic gist export

Roadmap
  - Add gist export to REPL
  - Color/syntax highlighting for REPL?
  - Add script file save/import/execution support in REPL
  - Expand github support in general
  - Add file persistence in ICE
  - Add nuget support?
