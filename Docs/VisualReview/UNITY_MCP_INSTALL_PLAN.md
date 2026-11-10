# Propuesta revisable de instalación local — 2026-10-29

Estado: preparación completada; integración NO instalada, importada ni ejecutada. No se modificó la configuración MCP de Codex. Revisado junto con UNITY_MCP.md mediante inspección estática de la release oficial, no una auditoría completa.

## Versiones verificadas

- GitHub API declara última release estable `v10.3.0`, publicada 2026-10-04. Tag anotado `ba6881930eadb2a9006d28bff3325ce03b4bfd8a`, commit resuelto **aa5fc638d623f56178d50329d9ad8c541a57fe66**. Plugin y servidor declaran 10.3.0 en esa revisión. Main observado: `81542a7e86292b5c4c21b1782c5af23b567f45ce`; no se propone instalarlo.
- Proyecto: Unity **6000.3.24f1**, URP **17.3.0**, XR Management **4.5.4**, Input System **1.20.0**, Test Framework **1.6.0**. Cardboard conserva su URL y resolución actuales en packages-lock.json.
- Herramientas locales disponibles: Codex CLI **0.164.0-alpha.11.1**, uv **0.12.16**, Python **3.12.10**. No hace falta instalarlas globalmente. `codex mcp add --help` confirmó HTTP y variables de entorno para stdio.
- Requisito declarado plugin Unity >=2021.3 y servidor Python >=3.10. Compatibilidad nominal; compilación real con este proyecto pendiente.

## Copia ya preparada

`Artifacts/UnityMCPReview/20261029/TrialProject` contiene copias de Assets, Packages y ProjectSettings, con repositorio Git independiente sin remoto ni commits. No contiene Library ni credenciales ni ZIP originales. No se abrió en Unity. Se conservaron también los cambios locales actuales del usuario. La copia reside en Artifacts, ignorado por el repositorio principal.

Junto a ella: `manifest.proposed.json`, `codex.proposed.toml` (deshabilitado), `server.uv.lock`, `python-dependencies.json`, `source-project-hashes.json` (714 archivos), `upstream.json` y fuente descargada. ZIP SHA256: `80918DB53CBD08F4CE8B03E6D89C5D7DDEA6393F0F34D6055BAA6F577CA6FBFF`. Guardar hashes individuales del código antes de instalar. El SHA del ZIP identifica esta descarga; no es firma del proveedor.

## Cambios concretos propuestos, aún sin aplicar

1. Añadir SOLO en el manifiesto de la copia:

   `com.coplaydev.unity-mcp = https://github.com/CoplayDev/unity-mcp.git?path=/MCPForUnity#aa5fc638d623f56178d50329d9ad8c541a57fe66`

2. UPM regenerará el lock de la copia y descargará el paquete en Library/PackageCache. El plugin declara Newtonsoft JSON **3.0.2**, Test Framework mínimo **1.1.31** y módulos Unity ya presentes. Mantener Test Framework **1.6.0** explícito; comprobar la resolución de Newtonsoft y fijarla en el manifiesto antes de aceptar el lock final. No agregar scoped registries. Comparar TODAS las entradas URP/XR/Input y abortar ante cambios. No copiar el lock propuesto al proyecto principal.

3. Crear `.venv` local dentro del Server descargado usando Python **3.12.10** y uv **0.12.16**, con `uv sync --frozen --no-dev --python <ruta-python-local>`. El `uv.lock` oficial fija **85 entradas** incluyendo variantes/paquetes de desarrollo; esto no significa 85 instalaciones para Windows. Dependencias principales fijadas: FastMCP **3.0.2**, MCP **1.25.0**, Pydantic **2.12.5**, HTTPX **0.28.1**, FastAPI **0.128.0**, Uvicorn **0.40.0**, Click **8.3.1**, Tomli **2.3.0**. Usar el lock, no resolver rangos ni `uvx latest`. La construcción del propio servidor usa setuptools/wheel con rangos en pyproject: fijar también esas herramientas o instalar el proyecto con entorno de construcción controlado antes de declarar reproducibilidad completa. Aún no se descargaron ni validaron wheels/DLL transitivos ni sus firmas/licencias.

4. Iniciar servidor con el ejecutable local `.venv/Scripts/mcp-for-unity.exe --transport http --http-host 127.0.0.1 --http-port 8080`. Confirmar primero puerto libre; consulta actual sin listener. Mantener `UNITY_MCP_DISABLE_TELEMETRY=1` en el entorno **del servidor y del proceso Unity antes de abrir la copia**. No iniciar con un Editor existente que no heredó la variable. También marcar `MCPForUnity.TelemetryDisabled=true` en EditorPrefs; esta preferencia afecta al perfil del usuario, no solo a la copia.

5. Registrar únicamente `unity-trial` en Codex tras revisión del bloque propuesto; no usar Configure All Detected Clients ni sincronización de skills. No se tocaron ~/.codex/config.toml ni otros clientes. HTTP no arranca el servidor ni transmite variables de entorno al proceso. URL `http://127.0.0.1:8080/mcp`. Empezar con `find_gameobjects` y recursos de estado; habilitar herramientas adicionales durante prueba supervisada. `manage_scene`, `manage_camera` y `read_console` tienen acciones que modifican estado (p. ej. limpiar Console): la lista de herramientas NO filtra acciones. Para cámara/jerarquía permitir solo las llamadas concretas supervisadas, retirar después.

## Telemetría, red y permisos

Telemetría por defecto hacia `https://api-prod.coplay.dev/telemetry/events`. El Editor envía UUID, Unity/plataforma y datos de eventos al servidor; el servidor tiene lógica propia de eventos/errores. Ambos reconocen el opt-out citado. Desactivarlo no garantiza ausencia de otra red: el comprobador de actualizaciones consulta GitHub/raw GitHub y un recurso de Supabase; el paquete también contiene integración opcional de generación de assets. No configurar claves, generación cloud, actualización automática ni sincronización de skills. Durante instalación se requieren GitHub, registro Unity y PyPI/files.pythonhosted.org; NuGet solo si se decide usar el instalador opcional. Registrar conexiones reales en la prueba, no afirmar aquí ausencia de egreso.

Modo HTTP local no ofrece por sí solo autenticación equivalente al modo remoto con validación de API key. Loopback evita acceso desde LAN, pero otros procesos del mismo equipo pueden alcanzarlo. No abrir firewall, túneles ni puertos públicos. Los procesos actúan con permisos del usuario; herramientas MCP no quedan aisladas por el sandbox de comandos de Codex. Una copia evita cambios accidentales en el juego, pero NO es una barrera del sistema operativo. No conceder acceso general a secretos, paquetes, shell, `execute_code`, runtime compilation o gestión de clientes.

Hallazgo de impacto Android: `MCPForUnity.Editor.asmdef` es Editor-only, pero **MCPForUnity.Runtime.asmdef no limita plataformas**. No prometer exclusión completa del player sin inspeccionar el build. El Editor referencia DLL de Roslyn; instalador opcional escribe cinco DLL en `Assets/Plugins/Roslyn`: Microsoft.CodeAnalysis.Common/CSharp **4.12.0**, System.Collections.Immutable **8.0.0**, System.Reflection.Metadata **8.0.0**, System.Runtime.CompilerServices.Unsafe **6.0.0**, desde api.nuget.org. No activar ese instalador en la primera prueba. Si la compilación exige estas DLL, detener y revisar hashes, licencias e import settings Editor-only antes de añadirlas; no instalar silenciosamente.

## Validación tras autorizar instalación

Guardar baseline Git local de la copia y hashes de fuentes; aplicar manifiesto, importar y compilar. Comparar manifest/lock y ProjectSettings con baseline, conservar URP/XR exactos y revisar inicializadores y preferencias modificadas. Verificar listener loopback y opt-out efectivo. Conectar solo a la instancia TrialProject, leer escena BancoCorresponsal/Console sin limpiar y capturar PNG real. Después probar mover/restaurar un objeto con Undo y comparar capturas. Revisar reporte Android para código/DLL añadidos; no trasladar MCP al principal antes de resolver Runtime/Roslyn. No se ejecutó ninguna de estas pruebas todavía.

Reversión: cerrar Unity de prueba y servidor; retirar únicamente entrada unity-trial si se creó; conservar o descartar la copia por separado. Las preferencias globales que se cambien deben registrarse para restauración selectiva. No revertir cambios ajenos del proyecto principal.

## Fuentes primarias

- https://github.com/CoplayDev/unity-mcp/releases/tag/v10.3.0
- https://github.com/CoplayDev/unity-mcp/tree/aa5fc638d623f56178d50329d9ad8c541a57fe66
- En esa revisión: MCPForUnity/package.json, Server/pyproject.toml, Server/uv.lock, Server/src/core/telemetry.py, Editor/Helpers/TelemetryHelper.cs, Editor/Setup/RoslynInstaller.cs, ambos asmdef y Editor/Services/PackageUpdateService.cs.
- https://developers.openai.com/codex/mcp (redirige actualmente a documentación oficial ChatGPT Learn; configuración TOML, filtros de herramientas y transportes).

La pausa antes de importar responde a la instrucción explícita del usuario: explicar primero los cambios concretos. No es un bloqueo de aprobación automática.
