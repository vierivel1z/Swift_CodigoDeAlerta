> **Histórico archivado el 10/11/2026.** Conserva el estado de su sesión; no describe necesariamente la versión actual. Consulta el [índice vigente](../../../README.md).

# Informe de sesión autónoma — 2026-10-27

## Resultado técnico
Implementado un prototipo de oficina bancaria con seis órdenes ficticias y modos ataque/
defensa, manteniendo Unity 6000.3.24f1 y URP 17.3.0. La validación se ejecutó con el editor
real en una copia aislada (ValidationProject), sin cerrar las ventanas del usuario.
Las escenas y assets generados por Unity se trasladaron al proyecto principal junto con
sus .meta; se comprobó coincidencia del GUID de GameManager. No se escribió YAML de escenas.

## Bloques A–I
| Bloque | Archivos / implementación | Evidencia y pendiente |
|---|---|---|
| A | .gitignore, estructura Assets/_Project, Git local, README, diagnóstico | Git instalado; carpetas generadas ignoradas; sin staging/commits/remotos. GitHub/equipo pendientes. |
| B | Packages/manifest.json y lock, Editor/CardboardSetup.cs, Assets/_Project/XR, Assets/XR, Assets/Plugins/Android, Scripts/VR | UPM resolvió Cardboard 1.36.0, XR Management 4.5.4, XR Core Utils 2.6.0 y Legacy Input Helpers 2.1.13. Loader Android asignado. Teléfono/HelloCardboard pendientes. |
| C | Scripts/Core y Scripts/Banking | GameManager y ScenarioData conectados en escena; servicios C# separados de entrada/UI. RegisterEvidence, ApplyDefense y EndScenario en uso real. |
| D | Editor/PrototypeBuilder.cs, Scripts/Testing/OfficeFactory.cs, Scenes, Materials | BancoCorresponsal y CardboardSmokeTest generadas/reabiertas. Suelo, cuatro paredes, techo, escritorio, monitor, silla, archivador y paneles con primitivas. |
| E | BankingDomain.cs y Resources/TrainingScenario.asset | Seis órdenes; formato devuelve, riesgo retiene, coincidencia no confirma fraude. Estados separados de datos del asset. 27 comprobaciones de dominio aprobadas. |
| F | Scripts/Interaction y Scripts/VR | Clic/raycast PC, mouse/WASD, mirada 1.5 s con latch/rearme; dos teletransportes. Dwell y ambos destinos comprobados en Play Mode simulado; entrada física pendiente. |
| G | Scripts/UI/UIManager.cs, Assets/TextMesh Pro | Canvas World Space/TMP, seis datos de expediente, alertas, botones y estado. Fuente Liberation Sans SDF con licencia OFL 1.1 incluida. Revisión visual de captura PC; visor pendiente. |
| H | GameSession/GameManager/UIManager | Ataque con resumen histórico fijo del contexto docente. Defensa aplica controles sobre anomalías visibles; comparación ficticia persistente entre turnos de la misma ejecución. |
| I | Tools/DomainTests, PrototypeSmokeProbe, métodos batch y Artifacts | Compilación C# y generación: PASS. Play Mode: PASS; configuración: PASS. Estado Android se registra al final del informe. |

## Pruebas verificadas
- `dotnet run --project Tools/DomainTests/DomainTests.csproj`: **27 PASS**.
  Incluye los tres tipos de decisión, evidencias sin duplicados, cierre incompleto rechazado,
  cierre definitivo, defensa solo en su modo, falso positivo de alertas y pérdida ficticia.
- `PrototypeBuilder.ValidateBatch`: **PASS**, salida Unity 0. Compiló scripts, configuró XR,
  generó/reabrió escena y comprobó GameManager, cámara y al menos diez interacciones.
- `PrototypeBuilder.StartPlaySmoke`: **PASS** tras corregir fuente e interfaz. Verifica TMP
  y etiquetas, seis decisiones por modo, evidencias, consultas, controles, resultados,
  dos destinos y mirada que activa una vez y se rearma al retirar/volver a enfocar.
- Captura de cámara renderizada revisada: `Artifacts/prototype-preview.png`.
- `Test-ProjectConfiguration.ps1`: **PASS** en el proyecto principal después del traslado.
  Manifest y lock coinciden; Android mínimo 26, target 35, ARM64, Input System nuevo.
- Hashes de SampleScene, GraphicsSettings, QualitySettings, Mobile_RPAsset y PC_RPAsset:
  idénticos entre el proyecto original y la copia. Pipeline conservado.
- `git check-ignore`: Library, Temp, Logs, UserSettings, Artifacts y ValidationProject excluidos.
  Sin archivos rastreados todavía; el primer commit lo realiza el equipo cuando autorice.

## Fallos encontrados y corregidos
1. Git sin red en sandbox: acceso oficial verificado fuera del sandbox; tag confirmado.
2. Diferencia de propietario del .git creado por sandbox: Git del usuario necesitó confiar
   únicamente en la ruta exacta del proyecto (safe.directory). Cambio de propietario fue
   denegado por Windows; se conservó el repositorio y se aplicó la excepción puntual.
3. Generación batch intentaba escena aditiva sobre escena sin título: corregida para usar
   Single exclusivamente en batch. El menú conserva escenas abiertas y requiere guardarlas.
4. Fuente LegacyRuntime no generaba atlas TMP: sustituida por Liberation Sans SDF del
   paquete esencial oficial. Importación asíncrona esperada antes de salir del editor.
5. Botones sin etiquetas y texto recortado/solapado: corregidos tras inspección visual.
6. La primera ejecución gráfica registró una excepción interna del índice UnityEditor.Search;
   no reapareció en las ejecuciones gráficas posteriores. También hubo diagnósticos iniciales
   de licencia/token, pero Unity resolvió la licencia y ejecutó las pruebas. No se modificó
   configuración de licencia ni el índice del editor del usuario.

## Limitaciones y aceptación humana
- No afirmar funcionamiento en Android ni visor por pasar las pruebas del editor.
- Falta probar APK, giroscopio, estéreo, QR, recenter, pausa/reanudación, audio y rendimiento
  en un teléfono real. Ajustar tamaño/distancias de UI tras probar el visor.
- El modo PC tiene desplazamiento de depuración acotado, sin simulación de colisiones del
  jugador. Android usa teletransporte y pose de cabeza; sin manos ni controladores Quest.
- Sonido sintetizado conectado a eventos, pendiente escuchar/verificar en dispositivo.
- No hay modelos definitivos, cinematografía, menú artístico, guardado entre ejecuciones ni
  entregables finales. No se descargaron modelos externos.
- K01 sigue dependiendo de las pestañas particulares del profesor. No se inventaron.
- No hay push ni commits automáticos. K03 completo requiere GitHub y aportes del equipo.

## Cómo continuar
Abrir BancoCorresponsal y seguir PROTOTYPE_GUIDE.md. Probar primero el flujo PC y luego
el teléfono. K04/K19 no se cierran hasta aportar evidencia del dispositivo. Mantener datos
jugables ficticios y separar el resultado contrafactual del resumen histórico.

Comprobación adicional del traslado: seis assets de escena/datos/XR sin GUID externos sin resolver (Artifacts/reference-check.txt). La pose XR prioriza centerEye, como el ejemplo oficial, y se actualiza también antes del render; la validación física sigue pendiente.

## Resultado final de compilación Android
- Primer intento completo: Succeeded, 0 errores. Se mantuvo como evidencia inicial.
- Versión final del prototipo: **Succeeded, 0 errores; Unity salió con código 0**.
  Log: Artifacts/unity-android-final.log. APK: Artifacts/SWIFT-prototype.apk.
- Tamaño real de APK: **66,528,319 bytes** (~66.5 MB decimal). Build de desarrollo.
- Paquete `com.atodaolla.swiftcodigoalerta`, versión `0.1.0`, código `1`.
- Manifiesto comprobado con aapt2: mínimo API 26, target/compile API 35,
  UnityPlayerActivity y únicamente código nativo arm64-v8a.
- Contiene libGfxPluginCardboard.so y UnitySubsystemsManifest.json de Cardboard.
- apksigner: **Verifies**, firma APK v2 válida, un firmante.
- SHA256: `85FABAB373359FF82610970EE43CBF8914692E18990C80E32CC4ABEF0BA45D03`.
  También guardado en Artifacts/SWIFT-prototype.apk.sha256.
- La última prueba gráfica, con destino Android en el editor y scripts finales, volvió
  a pasar: Artifacts/unity-playmode-releasecheck.log. Se comprobó carga de los avisos
  de licencia desde Resources, además de UI, lógica y mirada/teletransporte simulados.
- 16 archivos C#/Gradle del proyecto principal coinciden por hash con la copia validada.
- Las licencias completas se distribuyen como ThirdPartyNotices en Resources; no se
  interpreta la estructura comprimida del contenedor de datos Unity del APK.
- **No se instaló ni ejecutó esta APK en un teléfono**. No se comprobó seguimiento real,
  estéreo, QR, audio físico, temperatura/FPS ni comodidad en Cardboard. K04/K19 siguen abiertos.

## Próximo paso operativo
Abrir BancoCorresponsal, revisar el flujo PC y después instalar la APK en el teléfono de
prueba del equipo. Registrar dispositivo, Android, visor, errores y evidencia en KANBAN.
No crear commits ni publicar hasta autorización; el repositorio local queda preparado.

El reporte binario LastBuild.buildreport también registra Assets/_Project/Resources/ThirdPartyNotices.txt entre los recursos del jugador compilado (Artifacts/apk-resource-report.txt).
