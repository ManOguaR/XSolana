# XSolana.Codegen

Generador de código C# fuertemente tipado a partir de archivos IDL de Anchor (Solana).

Este paquete incluye tareas de MSBuild (`buildTransitive`) que detectan archivos `.idl.json`, generan código fuente en tiempo de compilación, y los integran automáticamente al proyecto consumidor.

## Características

- Soporte completo para IDL de Anchor.
- Generación de clases `ProgramService`, `AccountLayout`, `InstructionBuilder`.
- Basado en MSBuild + T4.
- Integración automática en tiempo de compilación.
