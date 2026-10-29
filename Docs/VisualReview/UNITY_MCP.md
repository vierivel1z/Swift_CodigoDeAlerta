# Unity MCP y Codex local

Investigación de fuentes primarias: **2026-10-29**. Se descargó código fuente en una carpeta temporal para inspección estática; **no se instaló, importó ni ejecutó ninguna integración**. Esto no constituye una auditoría completa ni certifica ausencia de vulnerabilidades.

Unity MCP no es un producto oficial de Unity u OpenAI: las dos opciones siguientes son comunitarias. Un servidor MCP aporta herramientas; no instala ni activa Unity por sí mismo. Para controlar este proyecto necesitas Codex local, el Editor local activado y el puente conectado al mismo proyecto. El Codex de la nube no alcanza automáticamente el localhost de tu PC.

## Opciones evaluadas

| Opción | Control y capturas | Dependencias observadas | Compatibilidad / decisión |
| --- | --- | --- | --- |
| [CoplayDev/unity-mcp](https://github.com/CoplayDev/unity-mcp) | Assets, escenas, scripts, tests; `manage_camera` con `screenshot`/`screenshot_multiview`, imagen incluida opcionalmente; configurador Codex TOML | UPM mínimo 2021.3; Newtonsoft 3.0.2, Test Framework 1.1.31 y módulos Unity. Servidor Python ≥3.10/uv, FastMCP ≥3,<4, MCP ≥1.16, Pydantic, HTTPX, FastAPI/Uvicorn, Click/Tomli. Editor referencia DLL de Roslyn | README declara 2021.3–6.x: **candidato para prueba local** con 6000.3.24f1. No hay certificación observada de URP 17.3.0 exacta. Preferible evaluar primero esta opción por el flujo Editor y configuración Codex explícita; no está aprobada para instalar sin resolver la revisión de versión/dependencias |
| [IvanMurzak/Unity-MCP](https://github.com/IvanMurzak/Unity-MCP) / AI Game Developer | Edición de objetos/escenas, invocación de código; `screenshot-camera`, `screenshot-game-view`, `screenshot-scene-view`, `screenshot-isolated` | UPM mínimo 2022.3; Test Framework 1.1.33 y PlayerPrefsEx 2.1.5 desde OpenUPM. Resolver propio NuGet; McpPlugin, ReflectorNet, SignalR, System.Text.Json, R3, Microsoft.Extensions y Roslyn. Servidor independiente; CLI npm opcional | Mínimo declarado compatible nominalmente con Unity 6; **6000.3.24f1/URP 17.3.0 sin probar**. Mayor superficie de dependencias y funciones runtime/cloud: mantener como alternativa hasta auditar y excluir integración de la APK |
| Herramienta nativa incluida en este proyecto | Cuatro PNG y galería mediante menú o `-executeMethod`; sin servidor | UnityEditor + URP ya presentes, sin nuevos paquetes | Preparada para las APIs Unity 6/URP; código contrastado con la rama oficial 6000.3. Primera compilación, render y encuadres aún pendientes en Editor local |

No hay motivo para sustituir URP ni actualizar sus paquetes para probar MCP. El proyecto fija Test Framework **1.6.0**; las dependencias antiguas declaradas por los puentes exigen revisar qué versión resuelve UPM, sin degradar el proyecto. Captura de Game View, Scene View y cámaras tiene comportamientos distintos en SRP; comprobar PNG real con el renderer de este proyecto es obligatorio.

### Versiones inspeccionadas y trazabilidad

- Coplay: commit [`ed4e205d31a545bac3686d5e19210e67959aeddd`](https://github.com/CoplayDev/unity-mcp/tree/ed4e205d31a545bac3686d5e19210e67959aeddd). `MCPForUnity/package.json`: **10.3.1-beta.11**; `Server/pyproject.toml`: **10.3.0**. El README aún propone `v10.0.0`: no asumir que release, main y servidor son equivalentes. Alinear plugin/servidor revisados y probar protocolo; no instalar `#main` ni `uvx ...@latest` sin fijar versiones.
- IvanMurzak: commit [`0c5b78de724dbccbf48631e63f6db805708ac5c9`](https://github.com/IvanMurzak/Unity-MCP/tree/0c5b78de724dbccbf48631e63f6db805708ac5c9). Paquete **1.5.0**, mínimo Unity 2022.3. El resolver incluye versiones de desarrollo de McpPlugin/RQL: revisar sus propios artefactos transitivos, no solo este package.json.
- Licencias declaradas en los repositorios: MIT; verificar licencias de dependencias y artefactos por separado antes de redistribuir.

## Dependencias, permisos y seguridad

Hallazgos concretos de la inspección:

- Coplay permite `execute_code`, edición de scripts/assets y lanzamiento de procesos. Sus comprobaciones de patrones no constituyen aislamiento de seguridad; no uses `--no-safety-checks`. En el servidor, telemetría activada por defecto con destino `https://api-prod.coplay.dev/telemetry/events`; existe opt-out `UNITY_MCP_DISABLE_TELEMETRY=1` (también `DISABLE_TELEMETRY`/`MCP_DISABLE_TELEMETRY`). Configura el opt-out **antes** de arrancar y revisa por separado la opción `TelemetryDisabled` del Editor. No habilites generación de assets/servicios externos para capturar escenas reales.
- IvanMurzak restaura NuGet en `Assets/Plugins/NuGet` y caché en Library; sus pins incluyen dependencias marcadas `includeInBuild: true` (SignalR, System.Text.Json, R3, McpPlugin, entre otras). No asumir Editor-only por usarlo para diseñar. Revisa los asmdef/import settings y el reporte de build antes de aceptar cualquier impacto en Android/Cardboard. Evita el instalador opaco, el CLI global y el modo runtime mientras no estén revisados.
- IvanMurzak documenta autenticación local `none` por defecto y opciones `token`/`oauth`, además de servicios cloud en `ai-game.dev`. No elegir automáticamente cloud ni reenviar capturas/código a esos endpoints. Usa una configuración local revisada. Coplay también admite servidor remoto: no se necesita para este flujo.
- Ambos pueden leer/escribir el proyecto y ejecutar acciones con permisos del usuario del Editor/servidor. No son visores de solo lectura. Respaldos y límites de instrucciones no sustituyen controles de acceso.

Antes de instalar localmente:

1. Elige una revisión concreta y revisa su diff, dependencias transitivas/lock, scripts de instalación, DLL descargadas, fuentes de paquetes y licencias. Para otra release hay que repetir la revisión; esta inspección no cubre futuras versiones.
2. Usa una copia descartable del proyecto con control Git. Guarda inventario y hashes de los artefactos obtenidos por TLS/fuentes oficiales del proveedor, manteniendo firmas/verificación. No desactives TLS ni antivirus para importar DLL.
3. Examina cambios propuestos a `Packages/manifest.json`, `packages-lock.json`, scoped registries, Assets/Plugins, asmdef y preferencias de Codex. No aceptes actualizaciones/degradaciones de URP, XR, Input System ni Test Framework a ciegas. Comprueba exclusión del player Android.
4. Revisa listeners, autenticación, permisos del proceso, destinos de red y telemetría. Vincula el servidor a loopback **127.0.0.1**; no uses `0.0.0.0`, port forwarding ni un túnel público. No abras puertos del Editor para revisar desde el celular: GitHub resuelve esa necesidad.
5. No concedas acceso a secretos, publicación, terminal arbitraria ni modificación de paquetes por defecto. Habilita solo herramientas necesarias cuando el servidor/cliente permita seleccionarlas. La primera prueba debe leer escena/Console y capturar; prueba mutaciones reversibles en la copia antes del proyecto principal.
6. Conserva el diff y resultados. Solo después de esa revisión y decisión local instala el paquete fijado. **Esta tarea no cambió ningún manifiesto ni configuración MCP.**

## Conectar Coplay a Codex local después de la revisión

Esta es una ruta propuesta, no una instalación ya realizada. No se necesita MCP para usar nuestro capturador.

1. Instala Codex local y las herramientas Python/uv requeridas por la revisión elegida usando sus distribuidores oficiales. Ejecuta `codex --version`, `codex mcp --help` y comprueba Python/uv. No instales integraciones en la nube ni copies credenciales a este repositorio.
2. En la copia de prueba con Unity 6000.3.24f1 abierto, usa Package Manager → **Add package from git URL**, con el SHA ya revisado. La estructura comprobada es:

   ```text
   https://github.com/CoplayDev/unity-mcp.git?path=/MCPForUnity#<SHA_REVISADO>
   ```

   Sustituye el marcador; no es un comando listo para ejecutar. Si eliges el commit beta inspeccionado, acepta explícitamente que es una prueba beta y alinea el servidor con esa revisión. Para producción conviene revisar una release estable antes de adoptarla.
3. Espera compilación; abre **Window > MCP for Unity**. Configura servidor local HTTP/streamable HTTP y el endpoint loopback que indique esa versión, habitualmente `http://127.0.0.1:8080/mcp`. Desactiva telemetría en Editor y servidor antes del inicio. Fija servidor/dependencias desde la revisión revisada; inspecciona el comando que propone la ventana, en lugar de ejecutarlo ciegamente si descarga `latest`.
4. Arranca servidor y conexión del Editor según el panel de esa revisión. Copia solo su bloque **Codex** y revísalo antes de mezclarlo en tu `~/.codex/config.toml` (Windows: `%USERPROFILE%\.codex\config.toml`). No uses **Configure All Detected Clients**, porque podría modificar otros clientes y skills innecesariamente.
5. Con el servidor ya iniciado, registra la URL **real** en Codex local. Si coincide con la habitual:

   ```sh
   codex mcp add unity-local --url http://127.0.0.1:8080/mcp
   codex mcp list
   ```

   Equivalente TOML para un servidor HTTP ya iniciado:

   ```toml
   [mcp_servers.unity-local]
   url = "http://127.0.0.1:8080/mcp"
   ```

   No dupliques la entrada si el panel ya la creó. HTTP necesita el proceso servidor arrancado; esta entrada no lo inicia. Si configuras stdio, usa el comando exacto revisado de esa versión con rutas y versiones fijadas, y opt-out de telemetría en su entorno. No transfieras sintaxis JSON de otros clientes a TOML. La CLI instalada en esta nube confirma `codex mcp add <nombre> --url <URL>`; verifica tu versión local antes de usarlo.
6. Reinicia/recarga Codex, abre el proyecto local y lista las herramientas MCP. Identifica **la instancia correcta de Unity** si hay varias. Pide leer nombre/ruta de escena, jerarquía y Console sin cambiar nada. Confirma versión exacta del Editor y URP y que se ve BancoCorresponsal.
7. Pide una captura de cámara usando las herramientas disponibles en esa revisión. Verifica que retorna/guarda un PNG auténtico de la escena. Después usa el menú SWIFT o `SceneReviewCapture.CaptureBatch` mediante una acción Editor revisada; no asumas un nombre de herramienta para ejecutar menús si el servidor no lo expone. El método batch requiere salir de Play. Revisa las cuatro imágenes y el reporte.
8. Valida una edición reversible en la copia: mover un objeto con Undo, guardar, capturar, comparar, restaurar y volver a capturar. Comprueba que no cambió pipeline/XR y que los paquetes MCP no contaminan Android. Solo entonces adopta el flujo en tu rama de trabajo.

Para desconectar: detén el puente/servidor local y usa `codex mcp remove unity-local` si creaste esa entrada. Para quitar el paquete, revierte únicamente su incorporación y dependencias introducidas en el diff de la copia, preservando cambios de usuario; no borres indiscriminadamente Assets/Plugins.

## Fuentes primarias consultadas

- [Coplay README y requisitos](https://github.com/CoplayDev/unity-mcp/blob/ed4e205d31a545bac3686d5e19210e67959aeddd/README.md).
- [Coplay manifiesto Unity](https://github.com/CoplayDev/unity-mcp/blob/ed4e205d31a545bac3686d5e19210e67959aeddd/MCPForUnity/package.json), [dependencias Python](https://github.com/CoplayDev/unity-mcp/blob/ed4e205d31a545bac3686d5e19210e67959aeddd/Server/pyproject.toml), [herramientas de cámara](https://github.com/CoplayDev/unity-mcp/blob/ed4e205d31a545bac3686d5e19210e67959aeddd/Server/src/services/tools/manage_camera.py), [telemetría](https://github.com/CoplayDev/unity-mcp/blob/ed4e205d31a545bac3686d5e19210e67959aeddd/Server/src/core/telemetry.py), [configurador Codex](https://github.com/CoplayDev/unity-mcp/blob/ed4e205d31a545bac3686d5e19210e67959aeddd/MCPForUnity/Editor/Helpers/CodexConfigHelper.cs).
- [IvanMurzak README: capturas, instalación y transportes](https://github.com/IvanMurzak/Unity-MCP/blob/0c5b78de724dbccbf48631e63f6db805708ac5c9/README.md), [manifiesto UPM](https://github.com/IvanMurzak/Unity-MCP/blob/0c5b78de724dbccbf48631e63f6db805708ac5c9/Unity-MCP-Plugin/Packages/com.ivanmurzak.unity.mcp/package.json), [resolver/pins NuGet](https://github.com/IvanMurzak/Unity-MCP/blob/0c5b78de724dbccbf48631e63f6db805708ac5c9/Unity-MCP-Plugin/Packages/com.ivanmurzak.unity.mcp/Editor/DependencyResolver/NuGetConfig.cs).
- [Configuración oficial Codex](https://developers.openai.com/codex/config-basic), [fuente oficial de configuración](https://github.com/openai/codex/blob/main/docs/config.md); sintaxis CLI contrastada con `codex mcp add --help`, sin registrar servidores.
- [API URP de la rama oficial 6000.3](https://github.com/Unity-Technologies/Graphics/blob/6000.3/staging/Packages/com.unity.render-pipelines.universal/Runtime/UniversalRenderPipeline.cs). La rama puede evolucionar; consultar también el paquete **17.3.0 instalado localmente**. No equivale a una prueba de ejecución.
