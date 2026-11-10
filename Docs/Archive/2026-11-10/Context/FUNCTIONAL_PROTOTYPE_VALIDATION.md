> **Histórico archivado el 10/11/2026.** Conserva el estado de su sesión; no describe necesariamente la versión actual. Consulta el [índice vigente](../../../README.md).

# Cierre del prototipo funcional — N01, N02 y N03

Fecha: 2026-11-02, America/Lima. Proyecto principal: `D:/UnityGames/SWIFT_CodigoDeAlerta`. Unity **6000.3.24f1**, URP **17.3.0**. Rama `review/unity-scene-designer`; sin commits, merges o push durante esta sesión.

Evidencia de esta sesión: `Artifacts/Functional-20261102-083259/`. Los porcentajes y pendientes de PROJECT_STATUS_AUDIT describen el estado anterior a estas correcciones; este documento registra el seguimiento.

## Protección del trabajo previo

Antes de modificar configuración se guardaron `git-before.txt`, `baseline-hashes.json` y copia íntegra de Assets, Packages, ProjectSettings, Docs y Tools en `Artifacts/Functional-20261102-083259/Backup/`. Los resultados antiguos de pruebas se conservaron en `PreviousEvidence/`. No se usó reset, clean, checkout, merge, rebase, commit ni push.

La comparación inicial posterior a las pruebas confirma cero archivos anteriores eliminados y solo cuatro cambios sobre archivos de la copia: EditorBuildSettings, UIManager y las dos sondas BankOfficeProbe/GameplayFlowProbe. Se añadió PrototypeRelease como herramienta de Editor con su .meta. La escena bancaria, modelos, materiales, lightmaps, paquetes, PlayerSettings, GraphicsSettings, QualitySettings y XR no se editaron para estas correcciones. El panel se recoloca solo durante ejecución; no se guardó ni regeneró la oficina.

La APK antigua `Artifacts/SWIFT-prototype.apk` conserva SHA256 `85FABAB373359FF82610970EE43CBF8914692E18990C80E32CC4ABEF0BA45D03`.

## N01 — escenas de compilación

**COMPLETADO.** `ProjectSettings/EditorBuildSettings.asset` conserva todas sus entradas: BancoCorresponsal primera y habilitada; SampleScene y HelloCardboard deshabilitadas como escenas de prueba. No se eliminó ninguna escena. No hay un perfil personalizado activo localizado que desvíe la lista; la herramienta además contempla un perfil activo que sobrescriba escenas.

La primera aplicación mediante API dejó la lista correcta en memoria, pero el cierre del proceso de pruebas no la persistió en disco. Se corrigieron explícitamente los dos flags del archivo de ajustes con Unity cerrado y se verificó su persistencia en aperturas posteriores.

`Assets/_Project/Editor/PrototypeRelease.cs` conserva los ajustes Android existentes y compila exclusivamente la lista habilitada tras exigir que contenga solo BancoCorresponsal. No llama a CardboardSetup.Configure ni a constructores de escena. Se niega a sobrescribir una APK existente. El archivo `build-scenes.txt` registra la lista.

## N02 — pruebas de juego ejecutadas en Unity

**COMPLETADO para las verificaciones automatizadas PC/Editor descritas aquí. Android y aceptación humana permanecen pendientes.** Se abrió el Editor real sobre el proyecto principal y después se repitieron pruebas con Unity batch gráfico. No se usó TrialProject ni la copia antigua ValidationProject.

| Prueba | Resultado | Evidencia |
|---|---|---|
| DomainTests, código actual | 27 PASS | domain-tests.txt |
| GameplayFlowValidation / GameplayFlowProbe | PASS, salida Unity 0 | flow-final-editor.log y flow-final-result.txt |
| Omisión del prólogo | Entra al modo elegido y no deja intro activa | flow-final-result.txt |
| Final automático del prólogo | Tres momentos a duración real de 4 segundos y espera de prueba de 13 segundos; entra a turno nuevo | GameplayFlowProbe.cs, flow-final-result.txt |
| Ambos modos/comparación | Ataque 965000 frente a defensa activa 0, pérdidas ficticias | flow-final-result.txt y smoke-final-result.txt |
| Aprobar/Devolver/Retener | Acciones de los botones sobre seis expedientes; secuencia correcta obtiene 6/6 y pérdida 0 | flow-final-result.txt |
| Resultados/botones | Se ocultan al terminar y reaparecen en turno nuevo | flow-final-result.txt |
| BankOfficeProbe sobre escena guardada | PASS, salida Unity 0; cámara, Canvas, TMP, consultas, evidencia, defensa y cuatro destinos | office-fixed-editor.log, office-result.txt |
| Raycasts de interacciones | Nueve roles alcanzables desde puntos disponibles; tres botones alcanzables desde inspección | interaction-raycasts.txt |
| PrototypeBuilder.StartPlaySmoke / PrototypeSmokeProbe | PASS, salida Unity 0 | smoke-final-editor.log, smoke-final-result.txt |
| Dwell simulado | Una activación, sin repetición al mantener foco; rearme al apartar y regresar | smoke-final-result.txt |
| Captura runtime del monitor | Generada en Unity e inspeccionada; datos y tres botones visibles | office-inspection.png |

La sonda de oficina se ejecutó sin llamar al antiguo StartPlayValidation, porque este llama a RefineLayout y guarda cambios visuales. PrototypeRelease.ValidateOffice abre la escena guardada y agrega únicamente la sonda en memoria.

### Bloqueo de mirada encontrado y corregido

El collider global del modelo de computadora cubre la zona de los botones. Antes de la corrección, apuntar al botón APROBAR desde inspección impactaba `Interactive Computer` antes del botón: su centro estaba en z=1.12, dentro del collider. Las invocaciones directas de decisiones pasaban y ocultaban este problema.

`UIManager.KeepPanelInFrontOfComputer()` sitúa el anclaje de UI unos 3 cm por delante de la cara frontal del BoxCollider de la computadora durante Start. Usa los límites del collider y la normal del panel; no cambia la lógica bancaria ni la escena guardada. Los raycasts posteriores alcanzan los botones en z≈0.82 desde el punto de inspección.

Se conservaron los intentos fallidos: office-first-attempt.txt, office-second-attempt.txt, office-ray-diagnostic.log y blocked-interaction-raycasts.txt. El primer intento también detectó que el inventario de prueba excluía botones inicialmente desactivados; la sonda ahora refresca los objetos tras iniciar el turno. El bloqueo físico se confirmó después de corregir ese inventario y solo se dio por cerrado al pasar el test de raycast con la corrección runtime.

### Console y límites

Se revisaron los logs reales de compilación/Play Mode. Las sondas Office/Smoke vigilan LogType.Error/Exception/Assert durante la ejecución y terminaron PASS sin esos errores. La primera apertura registró un diagnóstico de token de licencia, posteriormente resuelto por Unity Personal, y un mensaje interno de CancellationTokenSource durante indexación. No se modificaron credenciales/licencia ni paquetes por esos mensajes.

Las capturas de la ventana de Windows mediante computer-use agotaron el tiempo de espera; no se afirma revisión visual interactiva de Console ni prueba humana de mouse/teclado. La captura de cámara de Unity sí se generó y revisó. Los raycasts PC no certifican giroscopio, render estéreo, comodidad o selección física en el HONOR. La prueba de dwell usa un blanco de prueba; las acciones/posiciones de teletransporte se comprobaron por activación programática, quedando el recorrido de mirada en dispositivo pendiente.

Al terminar se dejó el Editor abierto en BancoCorresponsal y se abrió Console mediante su atajo, confirmado por accesibilidad. La herramienta no expone sus mensajes individuales y las capturas continúan sin responder. El log de esa apertura (`editor-final.log`) registró además `GUILayout: Mismatched LayoutGroup.keyUp`, diagnóstico interno de GUI que no se investigó ni se ocultó. Por ello no se declara Console visualmente vacía o libre de todos los diagnósticos del Editor; sí están verificadas las salidas de las sondas y el build.

## N03 — APK de contenido actual

**COMPLETADO para generación y verificación estática de APK; instalación/ejecución en HONOR NO PROBADAS.** BuildReport: Succeeded, **0 errores y 1 advertencia**, salida Unity 0, duración 11 min 48 s aproximadamente. Se mantuvieron API 26/35, ARM64, IL2CPP, GLES3, Cardboard y URP.

APK generada: `D:/UnityGames/SWIFT_CodigoDeAlerta/Artifacts/Functional-20261102-083259/SWIFT-BancoCorresponsal-current.apk`.

Tamaño real de la APK: **48851914 bytes**, 48.85 MB decimales. El campo totalSize de BuildReport incluye archivos auxiliares/símbolos y no es el tamaño del APK.

SHA256: `F59EA07AE5019C2C67A1B6025E9B8DB993DB7E67B7B2B4DD39012DD6C793FB6E`.

Verificaciones posteriores:

- apksigner: **Verifies**, firma v2 válida, un firmante Android Debug. Es una APK de desarrollo.
- aapt2: paquete `com.atodaolla.swiftcodigoalerta`, versión 0.1.0/código 1, min API 26, target/compile 35, solo arm64-v8a.
- Dentro del APK, `assets/bin/Data/globalgamemanagers` contiene la ruta BancoCorresponsal y no contiene HelloCardboard.unity. La lista explícita de BuildPlayer incluye solo la escena bancaria.
- `lib/arm64-v8a/libGfxPluginCardboard.so` está incluida.
- El inventario de assets empaquetados incluye TrainingScenario, LightingData y lightmaps de la oficina actual.
- La APK antigua no fue sobrescrita y conserva su hash original.

La única advertencia de BuildReport indica que Diagnostics Data requiere Debug Symbols SymbolTable/Full para resolver stacktraces de fallos. No impidió compilar. No se cambiaron esas preferencias durante la sesión. Las pruebas físicas siguen siendo necesarias; firma válida y escena incluida no certifican sensores/estéreo ni estabilidad en HONOR.

Evidencia en la carpeta de sesión: `android-editor.log`, `android-build.txt`, `build-packed-assets.txt`, `apk-badging.txt`, `apk-signature.txt`, `apk-content-check.json`, `SWIFT-BancoCorresponsal-current.apk.sha256`, `release-source-hashes.json` y `preservation-check.json`.

Para repetir el build, cerrar otras instancias de Unity de este checkout, elegir una carpeta de salida nueva en la variable de entorno `SWIFT_SESSION_DIR` y ejecutar Unity 6000.3.24f1 con `-batchmode -quit -projectPath RUTA_PROYECTO -buildTarget Android -executeMethod Swift.EditorTools.PrototypeRelease.BuildAndroid -logFile RUTA_LOG`. No reutilizar la carpeta con una APK existente: la herramienta lo rechaza para protegerla.

## Validación pendiente en HONOR y siguientes tareas

Seguir `Docs/Context/HONOR_TEST_CHECKLIST.md`. Prioridades siguientes: N04 arranque real; N07 cuatro teletransportes y dwell; N08 portal/cinco funciones; N09 ambos modos y decisiones desde retícula. Registrar modelo HONOR, Android/MagicOS, visor, hash, pasos y PASS/FAIL/NO PROBADO.

Después: N05 material particular del escenario, N06 revisión del conjunto de cambios locales, N11 estados visuales, N12 lectura en visor, N15 escucha y N18 rendimiento/temperatura. K04 y K19 siguen abiertos hasta la prueba física. Créditos completos y entregables académicos no se cerraron en esta sesión.
