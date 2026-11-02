# Auditoría del estado real — SWIFT: Código de Alerta

Fecha: **2 de noviembre de 2026 (America/Lima)**. Equipo 05 — A Toda Olla. Raíz auditada: `D:/UnityGames/SWIFT_CodigoDeAlerta`. Rama: `review/unity-scene-designer`, HEAD `3bef77b`.

## 1. Resumen ejecutivo

**Avance técnico estimado: 68 %. Preparación para cumplir la evaluación: 56 %. Ambas estimaciones son provisionales; no son una nota ni una certificación de funcionamiento en teléfono.** La metodología y sus límites aparecen en la sección 12.

Existe un prototipo sustancial: oficina con modelos importados, seis expedientes ficticios, decisiones, evidencias, cuentas, alertas, dos modos y resultados comparativos. La lógica bancaria volvió a superar **27 comprobaciones .NET** y la compilación C# actual de juego, samples y Editor terminó con **0 errores y 0 advertencias**. Hay evidencias anteriores de Unity y Play Mode; no hay evidencia suficiente de aceptación en Android/Cardboard real.

El mayor riesgo inmediato de entrega es concreto: `ProjectSettings/EditorBuildSettings.asset` deshabilita BancoCorresponsal y habilita únicamente HelloCardboard. Una compilación normal con esa lista puede entregar el ejemplo en lugar del banco. La herramienta `PrototypeBuilder.BuildAndroid()` sí pasa BancoCorresponsal explícitamente, pero también reconfigura Cardboard; no se ejecutó durante esta auditoría.

La APK existente del banco es del 27 de octubre y corresponde al prototipo anterior a la oficina importada, al prólogo y a la nueva interfaz/iluminación. Firma y metadatos siguen siendo verificables, pero no acreditan el juego actual. Faltan requisitos particulares del escenario 5, pruebas de teléfono, créditos completos consultables, contribuciones acreditadas de todo el equipo y entregables finales.

## 2. Alcance, documentos y límites de la comprobación

Se revisaron los documentos base de `Docs/Context/`: PROJECT_CONTEXT, GAME_DESIGN, TECHNICAL_PLAN, KANBAN, INITIAL_AUDIT, SESSION_REPORT, OFFICE_SCENE_REPORT, PROTOTYPE_GUIDE, ASSET_CREDITS, INTRO_AND_UI, TWO_MODES, README_INSTALACION e IDE_START_PROMPT; también README raíz, documentación visual/MCP, galerías, inventarios y prompts de arte. No hay versiones raíz de los documentos base ni AGENTS.md localizado en el proyecto principal o sus ancestros examinados. La skill de diseño de escenas existe en `.agents/skills/`; esta tarea es una auditoría sin edición de escenas y no necesitó aplicarla.

Se inspeccionaron scripts propios, herramientas de Editor, pruebas, escenas, prefabs, datos, materiales, modelos, UI, plugins Android, manifest/lock y ajustes de ProjectSettings. El inventario de archivos se usó para localizar evidencia, no para calcular avance.

**Único documento creado por la auditoría: este informe.** KANBAN no se modificó: su propuesta de actualización está en la sección 13. No se editaron escenas, lógica, modelos, URP, XR, paquetes ni configuraciones; no hubo merge, reset, rebase, commit, push o instalación de dependencias. Las comprobaciones .NET actualizaron salidas generadas e ignoradas en bin/obj y Temp; no se añadieron fuentes o pruebas nuevas.

Unity 6000.3.24f1 está instalado en `D:/Program Files/Unity/Installations/6000.3.24f1/Editor/Unity.exe`, junto con AndroidPlayer/SDK/NDK/OpenJDK. No se detectó Unity.exe activo al consultar procesos. No se inició una sesión nueva de Editor/batch sobre el original: reimportar puede escribir assets/configuración y varios validadores disponibles crean escenas, reconfiguran XR o escriben capturas. Tampoco se creó otra copia, respetando la autorización de crear solo el informe. Se usaron compilación contra bibliotecas locales de Unity, análisis estático y evidencias anteriores del Editor, identificadas expresamente como tales. **No se ejecutó Play Mode ni build Android nuevo en esta auditoría.**

### Registro de pruebas y evidencia

| Ref. | Comprobación | Resultado y alcance |
|---|---|---|
| T1 | `dotnet run --project Tools/DomainTests/DomainTests.csproj --no-restore` | **27 PASS**, código actual de BankingDomain enlazado por el csproj. Seis órdenes, recomendaciones, formato, alertas, evidencias sin duplicados, cierre, controles y pérdidas 965000/0. No verifica MonoBehaviour, YAML, UI ni hardware. |
| T2 | `dotnet build Assembly-CSharp-Editor.csproj --no-restore --nologo -v:minimal` | **0 errores, 0 advertencias**. Compila CardboardSamples, Assembly-CSharp y Assembly-CSharp-Editor. Proyectos generados con referencias locales Unity; no sustituye importación, compilación IL2CPP, shaders o ejecución Android. |
| T3 | `./Tools/Test-ProjectConfiguration.ps1 -EditorPath 'D:/Program Files/Unity/Installations/6000.3.24f1/Editor/Unity.exe'` | PASS: versiones directas manifest/lock coherentes, URP 17.3.0, XR Management 4.5.4, Input 1.20.0; SDK/NDK/JDK presentes; API 26/35 y ARM64. No valida rutas activas de External Tools ni build. |
| T4 | Escaneo de GUID por Python de 93 escenas/prefabs/assets/materiales/ajustes, contra 13368 GUID de Assets y PackageCache | **0 GUID externos sin resolver** en YAML examinado, excluidos GUID incorporados. No valida subasset/fileID, tipos, scripts en runtime ni binarios como LightingData. Coincidencias entre samples importados y `Samples~` son copias de ejemplo; no se confunden con conflictos del banco. |
| T5 | Hash de BancoCorresponsal y recibos de capturas | Escena actual SHA256 `41467b15703bff8b588c7f36c6a76c684483b1f44ca3a315074961b59c3c3d92`, coincidente con After-20261031T005431540Z y After-20261031T010216621Z. Se inspeccionaron imágenes de oficina, monitor y resultados. Coincidir escena no demuestra igualdad de todas sus dependencias ni de scripts desde esa fecha. |
| T6 | `aapt2 dump badging Artifacts/SWIFT-prototype.apk`; `java -jar .../apksigner.jar verify --verbose ...`; SHA256 | Firma v2 **Verifies**, paquete `com.atodaolla.swiftcodigoalerta`, versión 0.1.0/código 1, min API 26, target/compile 35, arm64-v8a. 66528319 bytes. SHA256 `85FABAB373359FF82610970EE43CBF8914692E18990C80E32CC4ABEF0BA45D03`. No instalación ni ejecución. |
| T7 | Inventario de extracción UI y SHA256 de los 12 PNG | Todos coinciden con `Assets/_Project/UI/OperationsCenter/inventory.json`. Los `.meta` tienen textureType 0/spriteMode 0: son texturas de referencia, no sprites funcionales ya conectados. |
| T8 | Git: status, branch -a, log -5, remote -v, diff main, autores y divergencia | Estado detallado en sección 11. Solo lectura; no fetch, por lo que las referencias remotas pueden estar desactualizadas. |
| H1 | `Artifacts/Office20261028/scene-validation.txt`, `playmode-office.txt`, logs build/play | Evidencia histórica PASS: 74 renderers activos, 10601 triángulos, 17 acciones, cinco funciones y cuatro teletransportes; una cámara, Canvas serializado, consulta/impresora/defensa/decisión. No es repetición actual ni prueba Android. |
| H2 | `Artifacts/LightingReview/validation.txt`, `play-validation.txt`, `bake.txt`, `last-result.txt` | Evidencia histórica PASS: shaders, cinco interacciones, cuatro teletransportes, siete prefabs y un lightmap; Play Mode con datos dinámicos, decisiones, resultados ocultar/restaurar y pérdidas 965000/0. El texto conservado de Play **no incluye verificación de raycast** aunque el código actual de PlayCheck sí la contiene; no atribuir al recibo esa prueba nueva. |
| H3 | `Artifacts/Gameplay20261029/compile.log`, `play.log`, `Docs/Context/INTRO_AND_UI.md` | Intentos de Unity aislado quedaron en Initial Refresh. No existe resultado PASS localizado de GameplayFlowProbe para omisión y final temporizado del prólogo. Compilación actual T2 sí incluye su código. |
| H4 | `Docs/Previews/LightingReview/Play/expediente-activo.png`, `resultados.png` | Imágenes anteriores con datos ficticios y comparación 965000/0, inspeccionadas ahora. Esa carpeta no tiene capture.json; trazabilidad menor que las galerías After. No son capturas nuevas ni de dispositivo. |

## 3. Estado físico del proyecto y separación de copias

| Ubicación | Hallazgo |
|---|---|
| `Assets/Scenes/` | Solo SampleScene de plantilla. La escena del juego está en `_Project/Scenes`. |
| `Assets/Scripts/`, `Assets/Prefabs/`, `Assets/Materials/` | No existen con esos nombres raíz; no implica ausencia de implementación. Scripts están en `_Project/Scripts`, materiales en `_Project/Materials` y `_Project/OfficeVisual`, prefabs en OfficeVisual/UI. |
| `Assets/Models/` | Seis familias de recursos; 141 FBX, cuatro OBJ y cuatro MTL, una textura PNG y atribuciones. Importación visual acreditada por H1 y galerías. No todos los modelos del kit están usados en escena. |
| `Assets/_Project/OfficeVisual/Prefabs/` | Quince prefabs de muebles/accesorios normalizados. Materiales URP propios separados de originales. |
| `Assets/Art/UI/OperationsCenter/` | No existe; ubicación real `_Project/UI/OperationsCenter`, referencia en `_Project/Styles/OperationsCenter_Original.png`. |
| `Assets/_Project/UI/OperationsCenter/` | Doce recortes opacos, inventario y siete prefabs: expediente completo, Header, Document_well, Feedback_well, APROBAR, RETENER y DEVOLVER. |
| `Assets/Plugins/Android/` | mainTemplate.gradle y gradleTemplate.properties. Dependencias Android presentes en plantilla; resolución Gradle actual no ejecutada. Biblioteca Cardboard procede del paquete UPM, no de un AAR propio en esta carpeta. |
| `Tools/` | Diagnóstico de configuración, DomainTests y extractor Python. No se ejecutó extract.py porque sobrescribe recursos/reporte de extracción. |
| `Docs/Previews/` | Galerías del principal, una galería expresamente de TrialProject y LightingReview. LATEST apunta a una sesión anterior a la iluminación actual. |
| `ValidationProject/` | Copia aislada antigua; no se tomó como estado actual del principal. |
| `Artifacts/Gameplay20261029/Validation/` | Otra copia de validación; importación fallida/inconclusa según H3. |
| `Artifacts/UnityMCPReview/20261029/TrialProject/` | Copia real de ensayo MCP. La galería `Docs/Previews/20261029T170749356Z-3a6e29/README.md` declara expresamente este origen. No se integraron sus fuentes, packages ni ajustes. La documentación de instalación describe una fase anterior; no demuestra el estado final de la prueba MCP. |
| `Swift_CodigoDeAlerta/` | Repositorio anidado conservado de GitHub Desktop, excluido por .gitignore; no es la raíz auditada. |
| `PruebaVR/Test_Cardboard.apk` | APK de prueba separada. Su existencia no acredita BancoCorresponsal ni una instalación exitosa. No se confundió con la APK del juego. |

Escenas/prototipos: BancoCorresponsal es la principal. CardboardSmokeTest tiene el mismo SHA256 que `Backups/BancoCorresponsal_BeforeOffice_20261028.unity`; ambos conservan el prototipo antiguo. Hay tres LightingRecovery; `LightingRecovery 2` coincide exactamente con la escena principal. SampleScene, HelloCardboard y VrMode son plantilla/ejemplos, no niveles bancarios. La geometría Legacy se conserva desactivada: no sumar sus interacciones o luces al contenido activo. No se eliminó ningún duplicado.

## 4. Matriz de funcionalidades

Categorías: **COMPLETADO** exige implementación y verificación del alcance que se indica; **IMPLEMENTADO SIN PROBAR** tiene código/recurso sin aceptación correspondiente; **EN PROGRESO** cubre implementación parcial; **BLOQUEADO** requiere una dependencia concreta ausente; **PENDIENTE** indica ausencia de implementación localizada. Completar lógica pura o una vista PC no completa automáticamente el requisito VR.

En la tabla, `S/` abrevia `Assets/_Project/Scripts/`; `SC` es `Assets/_Project/Scenes/BancoCorresponsal.unity`; `OV/` es `Assets/_Project/OfficeVisual/`; `OC/` es `Assets/_Project/UI/OperationsCenter/`. T/H remiten a pruebas y rutas de la sección 2. La prioridad indica qué hacer para cerrar la aceptación pendiente, no una orden de reconstruir lo ya validado.

| ID | Tarea | Estado | Evidencia | Qué falta | Prioridad | Dependencias |
|---|---|---|---|---|---|---|
| F01 | Oficina bancaria construida | COMPLETADO | SC, OV/Prefabs; H1/H2, imágenes T5 | Montaje PC verificado; aceptación móvil separada F42 | P3 | — |
| F02 | Modelos importados | COMPLETADO | Assets/Models, OV/Prefabs; H1, T4/T5 | Importación visual verificada; rendimiento por medir | P3 | F42 |
| F03 | Escritorio y computadora | COMPLETADO | Quaternius_Desk y Google_Computer en OV/Prefabs; T5 | Papel superpuesto al teclado visible: pulido, no ausencia del recurso | P3 | — |
| F04 | Impresora y archivadores | COMPLETADO | Bruno_Printer, Kenney_Archive; H1 y capturas | Revalidar selección física en visor | P3 | F12 |
| F05 | Iluminación URP horneada | COMPLETADO | OV/LightingReview, Scenes/BancoCorresponsal/LightingData.asset y lightmap; H2/T5 | Se valida el bake PC, no coste Android | P3 | F42 |
| F06 | Materiales y texturas | COMPLETADO | OV/Materials, Assets/Models/Computer_PolyGoogle; H1/H2/T4/T5 | Revisar Mobile shader/compresión al compilar | P3 | F39 |
| F07 | Distribución/ergonomía de oficina | EN PROGRESO | SC, zonas de entrada/inspección/evidencias/seguridad; T5 | Visibilidad, distancias y alcance desde cuatro puntos en teléfono | P3 | F40 |
| F08 | Integración paquete Google Cardboard | COMPLETADO | manifest/lock, XRGeneralSettings y cache 1.36.0; T3/T4 | Integración estática comprobada; runtime separado F09/F40 | P0 | F39 |
| F09 | Cámara/pose VR | IMPLEMENTADO SIN PROBAR | S/VR/HeadPoseAdapter.cs, CardboardLifecycle.cs y SC; T2/T4 | Sensores, estéreo, QR, recenter y ciclo de vida en teléfono | P0 | F39/F40 |
| F10 | Retícula | IMPLEMENTADO SIN PROBAR | S/Interaction/VRInteractionManager.cs y referencia Reticle en SC; T2/T4 | Visibilidad, escala y profundidad en visor | P1 | F09 |
| F11 | Raycast y dwell 1.5 s | IMPLEMENTADO SIN PROBAR | VRInteractionManager; H1 y pruebas antiguas prototype; T2 | Repetir raycasts actuales desde pads y con mirada real; comprobar rearme | P1 | F09 |
| F12 | Cuatro teletransportes por mirada | IMPLEMENTADO SIN PROBAR | SC, BankAction.Teleport, H1/H2 | Activación mirando, destinos sin oclusión y comodidad en Cardboard | P1 | F11 |
| F13 | Portal de entrada/selección de modo | EN PROGRESO | BankInteractable.EnterScenario; dos paneles portal en SC, T5 | Es un selector interactivo, no tránsito espacial visual. Confirmar si satisface al docente y verificar prólogo/mirada | P1 | F31, material docente |
| F14 | Cinco objetos con funciones distintas | IMPLEMENTADO SIN PROBAR | Monitor, registro, impresora/evidencia, alertas, defensa y cierre; H1/H2 | Verificar cinco acciones completas mediante retícula en APK actual | P1 | F11/F39 |
| F15 | Datos de transferencias ficticias | COMPLETADO | Resources/TrainingScenario.asset y S/Banking/BankingDomain.cs; T1/T4 | Seis órdenes verificadas, ninguna conexión bancaria real | P1 | — |
| F16 | Visualización de expediente | COMPLETADO | S/UI/UIManager.cs, Canvas en SC, H2/H4 | Función PC verificada; legibilidad VR en F30 | P1 | F30 |
| F17 | Validación de campos | EN PROGRESO | TransactionManager.Recommended y TrainingData.Alerts; T1 | Solo anomalías precargadas, sobre todo MissingIntermediary. No validador general de cuenta, importe o nombre vacío en datos editados | P1 | Confirmar alcance docente |
| F18 | Aprobar transferencias | COMPLETADO | TransactionManager.Decide/Decision.Approve; T1 y H2 | Aceptación VR separada | P1 | F14 |
| F19 | Retener transferencias | COMPLETADO | Decision.Hold/TransactionStatus.Held; T1 | Aceptación VR separada | P1 | F14 |
| F20 | Devolver transferencias | COMPLETADO | Decision.Return, formato y override; T1 | Aceptación VR separada | P1 | F14 |
| F21 | Registro de evidencias | COMPLETADO | EvidenceManager y GameSession.RegisterEvidence; T1/H2 | Registro en memoria y deduplicación verificados. Visor de entradas/persistencia no implementados ni confirmados como requisito | P1 | — |
| F22 | Consulta de cuentas | COMPLETADO | S/Core/GameManager.InspectRegistry; H1/H2 | Consulta ficticia basada en expediente actual; no buscador de cuentas general | P1 | F14 |
| F23 | Alertas sospechosas | COMPLETADO | TrainingData.Alerts e InspectAlerts; T1/H2 | No es detección real; SIM-006 no confirma fraude. Aceptación VR pendiente | P1 | F14 |
| F24 | GameManager y ScenarioData | COMPLETADO | S/Core/*.cs, TrainingScenario y enlaces SC; T1/T2/T4/H1 | Arquitectura presente. Estados son Session/Mode/Ended, no enum formal Menu/Attack/Defense/Results | P1 | — |
| F25 | TransactionManager/EvidenceManager/DefenseManager | COMPLETADO | Tres clases reales dentro de BankingDomain.cs; T1/T2 | No requieren archivos .cs separados para existir | P1 | — |
| F26 | RegisterEvidence/ApplyDefense/EndScenario | COMPLETADO | GameManager delega a GameSession/servicios; T1/T2/H2 | API exacta presente; no hay firma docente adicional recibida | P1 | — |
| F27 | UI Propuesta C | EN PROGRESO | S/UI/OperationsCenterStyle.cs, flag en SC; T5/H4 | Estilo adaptado funcional, no todas las pantallas/estados propuestos. Verificar visor | P2 | F30 |
| F28 | Extracción de sprites/controles | EN PROGRESO | Tools/OperationsCenter/extract.py, OC/Extracted e inventory; T7 | 12 recortes opacos correctos, importados como texturas, sin sprites conectados. Decidir si se necesitan; UI usa geometría nativa | P2 | Decisión de diseño |
| F29 | Prefabs de pantallas y TMP | COMPLETADO | OC/Prefabs, fuente LiberationSans SDF; T2/T4/H2 | Un expediente completo + seis piezas, no siete pantallas distintas. Reuso independiente pendiente de probar | P2 | F30 |
| F30 | Botones VR, estados y legibilidad | EN PROGRESO | UIManager/OperationsCenterStyle y colliders; H2/H4 | Hide/restore probado antes; faltan estados Focus/Pressed/Disabled y progreso dwell. Retícula solo cambia tamaño. Lectura real en Cardboard pendiente | P1 | F09/F40 |
| F31 | Prólogo de impresora | IMPLEMENTADO SIN PROBAR | S/Core/IntroSequence.cs, GameManager.EnterScenario, H3/T2 | Storyboard de 3×4 s, omisión, papel animado. Falta PASS del flujo temporizado/omisión y vista VR; sin voz/personaje/cinemática definitiva | P2 | F13 |
| F32 | Nivel 1: El turno nocturno | PENDIENTE | Sin escena/controlador/progresión localizado en S/ o escenas | No consta aceptación definitiva; acordar primero si se implementa | P2 | Material docente y alcance |
| F33 | Nivel 2: La alerta Jupiter | PENDIENTE | Sin implementación específica; las alertas genéricas no son este nivel | Acordar alcance; no inventar una obligación | P2 | F32/alcance |
| F34 | Nivel 3: La llamada de emergencia | PENDIENTE | Sin implementación específica ni audio de llamada | Acordar alcance; no es requisito confirmado | P2 | Alcance |
| F35 | Nivel 4: Ataque contra defensa | PENDIENTE | Dos modos existen, pero no un nivel 4/secuencia propia | Decidir si basta la comparación actual o se aprueba otro nivel | P2 | Alcance |
| F36 | Transiciones/tutorial | EN PROGRESO | Intro callback, StartScenario, resultados y texto de controles UIManager | Flujo en una escena existe; no hay tutorial guiado por pasos ni transiciones entre niveles | P2 | F31/alcance |
| F37 | Pantalla de resultados | COMPLETADO | UIManager, LastAttackLoss/LastDefenseLoss; T1/H2/H4 | Resultado PC verificado; claridad/legibilidad y mirada en teléfono pendientes | P2 | F30 |
| F38 | Sonido UI/impresora/alertas/música | EN PROGRESO | S/Audio/FeedbackAudio.cs, AudioSource en SC; T2/T4 | Un tono sintetizado de 660 Hz/0.1 s para Changed. Sin SFX específicos ni música; escucha PC/Android no verificada | P2 | F40 |
| F39 | APK con contenido actual | PENDIENTE | Solo Artifacts/SWIFT-prototype.apk antiguo y PruebaVR/Test_Cardboard.apk; T6 | Compilar banco actual, resolver escena seleccionada y validar reporte IL2CPP/Gradle | P0 | F43, Editor/SDK |
| F40 | Prueba real Android/Cardboard | PENDIENTE | No registro de teléfono/Android/visor/resultado localizado | Instalar y registrar sensores, estéreo, portales, cinco objetos, modos, audio y final | P0 | F39 y dispositivo |
| F41 | Créditos/licencias dentro de app | EN PROGRESO | Resources/OfficeAssetCredits.txt, ThirdPartyNotices, cartel Model credits y final; T4/T5 | Textos completos incluidos, pero no hay UI de lectura de esos TextAsset localizada. Mostrar autores, fuentes, licencias y adaptaciones de forma accesible y declarar procedencia de imagen UI/IA | P4 | F30 |
| F42 | Optimización Android | PENDIENTE | No mediciones de FPS/GPU/temperatura/memoria localizadas; H1 es solo geometría | Medir perfil Mobile y sesión sostenida; fijar umbral objetivo para teléfono | P3 | F40 |
| F43 | Lista de escenas de build del banco | EN PROGRESO | EditorBuildSettings.asset: banco 0, sample 1; T3 | Preparar perfil del juego con BancoCorresponsal habilitada y verificar la primera escena; no se cambió aquí | P0 | — |
| F44 | Versionado/reproducibilidad | EN PROGRESO | T8; cambios de escena/código y assets nuevos sin commit | Revisar conjunto coherente sin perder trabajo, incluir .meta/lightmaps/scripts necesarios y luego integrar por flujo del equipo | P0 | Revisión humana de cambios |
| F45 | Especificación particular escenario 5 | BLOQUEADO | PROJECT_CONTEXT/TWO_MODES: faltan pestañas Funcional/Técnico/Roles/Defensa/Vulnerabilidad | Obtener fuente docente e incorporar trazabilidad de requisitos; no impide pruebas técnicas independientes | P0 | Profesor/material |
| F46 | GitHub con contribuciones de todos | EN PROGRESO | T8: remoto y cuatro commits, dos identidades de autor | Acreditar integrantes/roles y aportes reales; dos identidades no demuestran todo el equipo | P4 | Equipo |
| F47 | Video de 2–3 min desde celular | PENDIENTE | Sin video de entrega localizado en Docs/PruebaVR; video de dependencia MCP no es evidencia del juego | Grabar ambos modos y acciones VR, revisar duración | P4 | F40 |
| F48 | Ficha máximo 4 páginas | PENDIENTE | Sin ficha final PDF/DOCX localizada | Crear y revisar escenario, vulnerabilidad, defensa, conceptos, capturas, roles y créditos | P4 | F45/F40 |
| F49 | Presentación/hipótesis | PENDIENTE | PROJECT_CONTEXT: 10 min, 7 demo + 3 análisis, hipótesis argumentada | Preparar guion y ensayo con evidencia; confirmar material particular | P4 | F45/F47/F48 |

## 5. Qué está comprobado y qué no

**Comprobado ahora:** lógica C# de seis órdenes, los tres tipos de decisión, defensa limitada a su modo, protección por anomalías observables, evidencia sin duplicados, cierre inválido rechazado, puntuación y separación de pérdidas ficticias/historia (T1); compilación administrada de scripts actuales (T2); coherencia estática de paquetes/ajustes y referencias GUID (T3/T4); extracción UI por hashes (T7); firma y metadatos de APK antigua (T6).

**Comprobado anteriormente con evidencia local, sin repetir runtime ahora:** oficina importada, cinco funciones, cuatro destinos, Canvas/TMP, resultados comparativos y bake PC (H1/H2). Los PNG y el hash permiten corroborar el montaje visual guardado; no probar todas las interacciones actuales a través del raycast.

**No probado:** ejecución del código actual en Unity recién abierto, IL2CPP/Gradle actuales, clic humano completo, omisión/final temporizado del nuevo prólogo, selección por pose real, estéreo, QR, pausa/reanudación, sonido audible, legibilidad, confort, FPS y temperatura. Ninguna de esas pruebas se sustituye por el número de archivos o por un HECHO del Kanban.

## 6. Errores, riesgos y bloqueos concretos

1. **P0, selección de contenido de build:** lista predeterminada apunta al sample. El método BuildAndroid evita ese problema al pasar `scenes = new[] { ScenePath }`, pero no corrige por sí solo la lista para compilaciones normales si la escena ya existe. Riesgo confirmado por código y ajustes; no es error C#.
2. **P0, falta APK vigente y aceptación física:** no se puede afirmar que la combinación actual de banco, UI, prólogo y bake funciona en el Android destino. SDK/NDK/JDK presentes no garantizan build exitoso ni dispositivo compatible.
3. **P0 de especificación:** faltan pestañas particulares. Bloquean aceptar el alcance del escenario 5; no bloquean compilar ni probar el MVP disponible.
4. **P0 de reproducibilidad:** assets no versionados son dependencias de escena y scripts modificados. Un clon de HEAD no contiene IntroSequence, OperationsCenterStyle, UI, iluminación y lightmaps actuales. No se atribuye autoría a estos cambios ni se los sobrescribe.
5. **Paquete Cardboard:** manifest usa URL sin `#tag` o `#commit`; lock y caché sí fijan actualmente `7fb2c0e438d2ef686f51cd59b53da16cc07b1b68`, paquete 1.36.0. No hay dependencia ausente detectada; sí una discrepancia con la documentación y riesgo al regenerar/actualizar lock. Fijarlo sería una tarea posterior autorizada.
6. **XR: no concluir avería por flags aislados.** XRGeneralSettings tiene Android loader y InitManagerOnStart=1; automaticLoading/Running=0. El código local de XR Management inicializa desde XRGeneralSettings y pone esos flags en false al asumir el control. `preloadedAssets: []` difiere de HEAD, pero XRGeneralBuildProcessor añade settings durante prebuild y los limpia después. Es un cambio a revisar, **no prueba de XR roto**. Se requiere build/dispositivo para resolverlo.
7. **Pruebas incompletas:** GameplayFlowProbe está preparado, pero no hay PASS de su ejecución localizado. PlayCheck de iluminación inicia con StartScenario, por lo que no certifica el portal ni el prólogo. La versión actual de PlayCheck contiene un raycast adicional ausente del recibo conservado.
8. **Mecánica educativa limitada:** las anomalías están etiquetadas en datos; no hay editor ni validador general de formatos bancarios. Evidencias viven en memoria y UI muestra el contador; no existe visor completo de entradas. El expediente ya revela recomendaciones de alertas, lo que reduce necesidad de consultar otros objetos. Son decisiones de diseño a revisar, no nuevos requisitos docentes.
9. **Portales:** actualmente son paneles con BankAction.Attack/DefenseMode y selección; no hay implementación de atravesar un portal a otra sala. Falta confirmar expectativa docente y probarlos por mirada.
10. **UI/audio/créditos:** estados visuales incompletos, sonido único sin escucha verificada, créditos completos guardados como recursos sin lector visible localizado. No afirmar cumplimiento final por empaquetar un TXT.

No se encontró error de compilación C# vigente en T2 ni GUID externo perdido en T4. El log global del Editor contiene rutas ajenas al banco (`Assets/Class_UI/...`); no es evidencia limpia del estado actual de este checkout. Los fallos de licencia/refresh de H3 son antecedentes, no demostración de un bloqueo vigente de la instalación.

## 7. Documentación frente a implementación

| Afirmación/documento | Contraste y tratamiento |
|---|---|
| INITIAL_AUDIT: sin juego, sin Cardboard ni Git | Describe el 27/10 antes de implementar. No usarlo como estado actual. |
| KANBAN conserva K03 sin remoto y ausencia de juego en secciones antiguas | Hay remoto/historial/código; son registros cronológicos. Proponer una cabecera de estado vigente sin borrar bitácora. |
| KANBAN/README_INSTALACION/PROTOTYPE_GUIDE: solo planes, primitivas o prefabs futuros | Superado por OfficeVisual y UI actuales. Las guías no están totalmente sincronizadas. |
| PROTOTYPE_GUIDE: APK usa banco exclusivamente | Es cierto para el método BuildAndroid explícito y APK antigua; no para la lista general actual de EditorBuildSettings. |
| Contexto/guía: Cardboard fijado al commit | Solo lock/caché tienen hash; manifest no fija revisión en URL. |
| INTRO_AND_UI: integración Play pendiente | Sigue sin PASS del prólogo, pero H2 posterior sí verifica partes de UI/resultados. No extender ese PASS a lo que no ejecuta. |
| Propuesta C: sprites/estados y pantallas | Hay 12 controles opacos de referencia y UI nativa adaptada. Siete prefabs son una pantalla y seis piezas. No existen todos los estados ni sprites conectados. |
| OFFICE_SCENE_REPORT: una luz sin sombras | Describe estado anterior; escena actual serializa nueve Light y contiene bake/lightmap. No usar 10601 triángulos del informe antiguo como métrica actual completa. |
| Previews/README y CHANGELOG: capturas pendientes en nube | Hoy existen capturas locales; la limitación original sigue siendo histórica. LATEST no señala la revisión más reciente. |
| UNITY_MCP_INSTALL_PLAN: TrialProject sin abrir | Galería posterior declara capturas realizadas en TrialProject. Documento de preparación no registra el cierre de ese ensayo; no inferir integración MCP en el principal. |
| TECHNICAL_PLAN nombra VRGazeInteractor/GazeTeleport/paneles separados | Funciones consolidadas en VRInteractionManager, BankInteractable y UIManager. Los nombres propuestos no son APIs académicas obligatorias. |
| Niveles 1–4 de la solicitud | No constan aceptados como niveles definitivos en documentos recibidos. Ataque/defensa sí son obligatorios; cuatro niveles no. |

## 8. Cumplimiento de requisitos académicos

Fuente de requisitos: declaración del usuario y `Docs/Context/PROJECT_CONTEXT.md`/TWO_MODES. No se dispone de rúbrica original completa para cotejo independiente. La arquitectura exigida es GameManager, ScenarioData y métodos exactos; TransactionManager/EvidenceManager/DefenseManager además existen, pero no se inventa una obligación docente de separarlos en archivos.

| Requisito confirmado | Estado de aceptación | Evidencia / falta |
|---|---|---|
| Unity 6 + APK instalable Android | EN PROGRESO | Versión y APK antigua verificadas; falta APK actual instalada y ejecutada (T2/T3/T6, F39/F40). |
| Ataque y defensa en misma app | EN PROGRESO | Clases/modos probados en T1/H2; falta demostrar ambos en la APK actual. |
| Resultado diferente | COMPLETADO en dominio/PC | Aprobar todo: ataque 965000; defensa activa 0, T1/H2/H4. Debe grabarse en teléfono; no se promete diferencia si el jugador ya decide correctamente sin defensa. |
| Portal VR | EN PROGRESO | Selectores existen; falta aceptación de forma/uso real. |
| Teletransporte por mirada | IMPLEMENTADO SIN PROBAR en dispositivo | Cuatro puntos PC; mirar y comodidad no certificados. |
| Cinco objetos mediante retícula | IMPLEMENTADO SIN PROBAR en dispositivo | Funciones presentes y H1/H2; falta recorrido físico con cinco acciones. |
| Sonido | EN PROGRESO | Tono conectado; audibilidad no probada. Música/SFX específicos son propuesta de experiencia, no obligación adicional confirmada. |
| Pantalla final | EN PROGRESO para entrega VR | Función PC con comparación y textos; lectura/selección final en visor pendiente. |
| Arquitectura común | COMPLETADO en implementación verificable | GameManager/ScenarioData/APIs, T1/T2/T4/H1/H2. Firmas adicionales no recibidas. |
| GitHub y commits de todos | EN PROGRESO | Remoto/historial locales presentes; no se confirma aporte de cada integrante ni estado remoto actual. |
| Recursos permitidos y créditos dentro de app, IA declarada | EN PROGRESO | Autor/licencia/fuente documentados, cartel y final; falta lector accesible de atribuciones completas, procedencia de referencia UI y revisión en APK. |
| Video 2–3 minutos desde celular | PENDIENTE | No localizado entregable propio. |
| Ficha ≤4 páginas | PENDIENTE | No localizada. |
| Exposición 10 min e hipótesis argumentada | PENDIENTE | Confirmado por contexto; sin guion/ensayo final localizado. |
| Pestañas Funcional/Técnico específicas escenario 5 | BLOQUEADO | Faltan junto a Roles/Defensa/Vulnerabilidad completas. No se inventaron exigencias. |

AR, Image Targets y Vuforia no aplican al equipo VR según contexto. No se pide implementar intrusión real, malware, manos o las 36 órdenes como expedientes individuales. El resumen histórico se audita por concordancia interna con el material recibido, no como nueva investigación histórica.

## 9. Historia, niveles, arte y audio

El flujo real implementado es **portal selector → storyboard opcional/omitible → seis expedientes → resultados → otro modo** en una misma escena. No hay controlador de progresión de cuatro niveles ni escenas Nivel1–4. No consta que esos títulos fueran aprobados. La comparación entre dos modos no equivale a tener cuatro niveles terminados. Tutorial actual: instrucciones y recomendaciones textuales; falta enseñanza interactiva si el equipo la aprueba.

Prólogo: representación UI provisional de impresora/hoja y tres textos, sin movimiento forzado de cámara, sin personaje o voz. La distinción banco de origen/puesto del jugador está escrita; falta comprobar geometría y lectura real desde la orientación del portal y que omitir/finalizar siempre inicie una sola sesión.

Arte: oficina low-poly real, escritorio, computadora, impresora, archivadores, libros, silla, plantas y mobiliario secundario. Se mantienen paneles, puerta/reloj y partes geométricas provisionales. La imagen de monitor ocupa buena parte de la vista; los papeles siguen sobre el teclado en la captura más reciente inspeccionada. No se declara comodidad por ver una imagen 1600×1000.

Iluminación: se serializan nueve Light en el YAML (incluye contenido que puede estar inactivo); no equivalen a nueve luces dinámicas activas. Hay luminarias decorativas, luces horneadas, probes y un lightmap. H2 acredita bake completo y perfil PC; falta medir perfil Mobile_RPAsset y sombras/contraste/lectura en estéreo. La cuenta antigua de 74 renderers activos/10601 triángulos no incluye necesariamente todos los cambios actuales.

Audio: solo FeedbackAudio genera un tono y lo dispara al evento Changed. No hay WAV/MP3/OGG localizados en Assets, pero eso no implica silencio porque el clip se crea en runtime. No hay pistas específicas de impresora, alertas diferenciadas, ambiente o música localizadas. Son mejoras pendientes; primero verificar que el sonido mínimo se oye en Android.

Licencias: Kenney/Quaternius documentados CC0; computadora/grapadora Poly, impresora Bruno Oliveira y papeles Jarlan Perez documentados CC BY 3.0. Se conservaron créditos/fuentes en Models, ASSET_CREDITS y Resources. Se auditó la evidencia local, sin revalidar jurídicamente cada fuente externa. Office Pack sin autor/licencia no se usa según inventario. El cartel de escena nombra a esos autores/licencias, pero no sustituye por sí solo la lectura de todas las fuentes/adaptaciones incluidas en OfficeAssetCredits. Confirmar procedencia y condiciones de OperationsCenter_Original antes de entrega.

## 10. Estado de Cardboard, Android y APK

Paquete 1.36.0 resuelto en caché con fingerprint igual al hash de lock; XR Management 4.5.4 y loader Android asignados. Camera tiene HeadPoseAdapter, CardboardLifecycle, VRInteractionManager y EditorInputAdapter. La retícula/cámara/rig están referenciados. Input PC se evita en Android; Android usa pose XR y dwell; no hay seguimiento de manos ni red bancaria.

PlayerSettings: min 26, target 35, ARM64, IL2CPP, LandscapeLeft, GLES3 configurado; conservar URP/Graphics y perfil Mobile. Son ajustes actuales comprobados, no recomendaciones de versión nueva ni prueba de compatibilidad del teléfono.

La APK `Artifacts/SWIFT-prototype.apk` **existe y conserva firma v2 válida** (T6). Fue construida antes de la nueva oficina. Log `Artifacts/unity-android-final.log` y `android-build.txt` acreditan build anterior exitoso, no actual. `PruebaVR/Test_Cardboard.apk` queda separado: el nombre y archivo no demuestran qué experiencia se instaló ni que alguien la probó con visor.

Falta: nueva APK de BancoCorresponsal, instalación limpia/actualización, arranque, sensores/estéreo, QR/recenter, pausa/reanudación, cinco objetos, teletransporte, audio, ambos modos, final, créditos y mediciones sostenidas. Registrar modelo, Android, visor, versión/commit/hash del build, pasos, resultados y evidencias. No se consultó ni intervino un dispositivo; no se afirma que no exista uno disponible.

## 11. Git y trabajo pendiente de integración

Se ejecutaron los cuatro comandos solicitados. Rama `review/unity-scene-designer`; upstream `origin/review/unity-scene-designer`; `main` y `origin/main` existen. Las referencias locales muestran HEAD alineado con su upstream y dos commits por delante de main (`git rev-list --left-right --count main...HEAD`: `0 2`). Sin fetch no se certifica sincronización actual con GitHub. Remoto origin: `https://github.com/vierivel1z/Swift_CodigoDeAlerta.git` (fetch/push).

Solo hay cuatro commits disponibles en el historial, por eso log -5 devuelve cuatro:

| Commit | Autor declarado | Mensaje |
|---|---|---|
| 3bef77b | Vieri Veliz | Add TrialProject visual capture evidence |
| 2b4f670 | Samantha Claire Donovan Reese | Add Unity scene design skill and visual review workflow |
| e8b46fb | Vieri Veliz | Initial commit: Unity 6 VR banking simulator |
| 6ba7e8d | Vieri Veliz | Initial commit |

Son dos identidades de autor en Git; no se atribuyen automáticamente a dos integrantes diferentes ni se acredita el aporte de todo el equipo. No hay otras ramas de arte/pruebas visibles en `branch -a`; la única rama review sirve para revisión visual. TrialProject/ValidationProject son copias, no ramas de este repositorio.

**23 archivos rastreados ya modificados antes de crear este informe**: quince `.meta` de modelos, BancoCorresponsal, GameManager, BankInteractable, BankOfficeProbe, PrototypeSmokeProbe, UIManager, KANBAN y ProjectSettings.asset. En modelos el diff observado cambia generación UV2 para iluminación. PlayerSettings elimina dos preloadedAssets respecto de HEAD: revisar con el flujo XR/build, sin asumir fallo.

También hay archivos no rastreados previos: IntroSequence, GameplayFlowProbe/Validation, OperationsCenterStyle y OperationsLightingReview con sus .meta; materiales/lighting settings/lightmaps; tres backups; referencia visual, 12 recortes, siete prefabs UI e inventario; prompts, TWO_MODES/INTRO_AND_UI, galerías y extractor. **El juego local depende de parte de ese contenido; main y HEAD no lo reproducen todavía.**

La diferencia **entre commits** main..HEAD añade principalmente skill, capturador, galería de TrialProject y documentación visual/README. Las modificaciones de gameplay/arte/iluminación actuales están en el **working tree**, no en esos dos commits. Un `git diff --stat main` mezcla ambas capas y no enumera archivos nuevos; revisarlas por separado al integrar. No hay evidencia de conflicto de merge porque no se intentó integración. No se creó PR ni se cambió el historial.

## 12. Porcentajes: método explícito y provisional

Estos indicadores miden **madurez respaldada por evidencia**, no horas invertidas, archivos o tarjetas HECHO. Pesos asignados por importancia para un MVP VR jugable. Coeficiente 0 = ausente; valores intermedios reflejan alcance y pruebas; 1 = aceptación del alcance evaluado. Una función puede estar completa en dominio y recibir crédito parcial como experiencia móvil. Los pesos son juicio de auditoría, no una rúbrica publicada por el profesor.

### Progreso técnico: 67.75 % → 68 %

| Capacidad | Peso | Madurez | Aporte | Justificación |
|---|---:|---:|---:|---|
| Dominio bancario y modos | 25 | 0.95 | 23.75 | T1/H2 sólidos; validación genérica y profundidad de consulta limitadas. |
| Arquitectura e integración administrada | 10 | 0.90 | 9.00 | T2/T4; falta nueva ejecución integrada y reproducibilidad de cambios locales. |
| Oficina, modelos y luz | 15 | 0.80 | 12.00 | Montaje/bake y T5; Mobile y ergonomía pendientes. |
| Interacción/pose VR | 20 | 0.50 | 10.00 | Código y pruebas PC previas; ningún cierre físico. |
| UI funcional/estados/lectura | 10 | 0.65 | 6.50 | H2/H4, faltan estados/lector créditos y visor. |
| Flujo, introducción, tutorial y resultados | 10 | 0.50 | 5.00 | Resultados hechos; intro sin PASS y tutorial básico. Cuatro niveles opcionales no se penalizan como obligación. |
| Audio/presentación | 5 | 0.20 | 1.00 | Tono mínimo; sin escucha certificada ni diseño de sonido. |
| Build vigente y rendimiento Android | 5 | 0.10 | 0.50 | Herramientas y build histórico; ninguna APK actual/medición. |
| **Total** | **100** | | **67.75** | |

### Preparación para requisitos de evaluación: 56.25 % → 56 %

Coeficientes de preparación usados: 0 sin entregable; 0.25 antecedente/parte menor; 0.50 implementación con prueba en destino ausente; 0.75 evidencia PC sustancial faltando teléfono; 1 comprobación suficiente para el alcance indicado. No equivalen a categoría COMPLETADO ni a aprobación docente.

| Requisito | Peso | Coeficiente | Aporte |
|---|---:|---:|---:|
| APK instalable actual | 15 | 0.25 | 3.75 |
| Ataque en app | 10 | 0.75 | 7.50 |
| Defensa en app | 10 | 0.75 | 7.50 |
| Resultado distinto demostrado en dominio/PC | 5 | 1.00 | 5.00 |
| Portal VR | 5 | 0.50 | 2.50 |
| Teletransporte mirada | 10 | 0.50 | 5.00 |
| Cinco objetos con retícula | 10 | 0.50 | 5.00 |
| Audio | 5 | 0.50 | 2.50 |
| Pantalla final | 5 | 0.75 | 3.75 |
| Arquitectura común | 10 | 1.00 | 10.00 |
| GitHub con contribuciones del equipo | 5 | 0.25 | 1.25 |
| Recursos y créditos | 5 | 0.50 | 2.50 |
| Video | 3 | 0 | 0 |
| Ficha | 2 | 0 | 0 |
| **Total** | **100** | | **56.25** |

La exposición/hipótesis se registra como condición de entrega adicional sin peso inventado. Las pestañas particulares desconocidas no entran al denominador: al recibirlas habrá que recalcular. Por eso **56 % no significa 56 % de la rúbrica real satisfecha**; es preparación provisional frente a la lista general conocida. Ni 68 % ni 56 % deben publicarse como aprobación final.

## 13. Propuesta del siguiente Kanban, sin modificar el existente

Conservar K01–K20 y su bitácora. Añadir posteriormente una sección «Estado vigente según PROJECT_STATUS_AUDIT, 2026-11-02» y enlazar subacciones verificables de esta tabla. No reemplazar log histórico ni cerrar K04/K19 por compilación PC. Responsables: por acordar con el equipo; no inventarlos.

| ID | Tarea | Estado | Evidencia | Qué falta / aceptación verificable | Prioridad | Dependencias |
|---|---|---|---|---|---|---|
| N01 (K02/K19) | Preparar perfil de build del banco | PENDIENTE | F43; lista actual solo sample | BancoCorresponsal primera habilitada; reporte/lista de escenas del build confirma contenido | P0 | Revisión de ajustes |
| N02 (K02/K10) | Repetir flujo actual en Editor sin regenerar oficina | PENDIENTE | T2/H2/H3 | Console sin errores, GameplayFlowProbe PASS para intro omitida/temporizada y ambos modos; prueba de raycast actual registrada | P0 | Preservar cambios locales |
| N03 (K19) | Compilar APK actual identificable | PENDIENTE | T6 solo APK antigua | Build Android exitoso, 0 errores; hash, firma, escena y versión registrados sin sobrescribir evidencia antigua | P0 | N01/N02 |
| N04 (K04/K19) | Validar arranque VR en teléfono | PENDIENTE | F09/F40 | Instala/abre banco, estéreo, sensores, QR/recenter y pausa/reanudación con registro del dispositivo | P0 | N03/teléfono/visor |
| N05 (K01) | Incorporar pestañas particulares docente | BLOQUEADO | F45 | Fuente Funcional/Técnico/Roles/Defensa/Vulnerabilidad obtenida y matriz obligatoria/propuesta revisada | P0 | Profesor/material |
| N06 (K03) | Revisar integridad de cambios locales | PENDIENTE | F44 y sección Git | Identificar scripts/assets/metas/lightmaps que deben viajar juntos; checklist de integración sin perder trabajo; commit/push en sesión posterior autorizada | P0 | Equipo |
| N07 (K07/K09) | Probar dwell y cuatro pads en dispositivo | PENDIENTE | Código/H1, sin visor | Cada destino alcanzable por mirada, una activación por foco, rearme y comodidad documentados | P1 | N04 |
| N08 (K08/K14) | Aceptar portal y cinco funciones VR | PENDIENTE | H1/H2 | Iniciar por portal, consultar cuenta/alertas, registrar evidencia, activar defensa/cerrar; video corto con retícula | P1 | N04/N05 |
| N09 (K13/K15/K16) | Ensayar decisiones y ambos resultados desde retícula | PENDIENTE | T1/H2 | Aprobar/retener/devolver; ataque 965000 frente a defensa activa 0 en caso reproducible; rechazo de cierre incompleto | P1 | N08 |
| N10 (K12) | Revisar contrato de validación de campos | PENDIENTE | F17 | Acordar campos exigidos; probar faltantes/formatos o documentar límite de anomalías precargadas según especificación | P1 | N05 |
| N11 (K12/K14) | Añadir estados visuales de interacción | PENDIENTE | F30 | Normal/foco/progreso/pulsación/deshabilitado distinguibles por mirada; comprobar sin doble decisión | P2 | N07/N09 |
| N12 (K12/K18) | Ajustar lectura de expediente/resultados | PENDIENTE | T5/H4 | Texto completo legible en visor desde pad; sin clipping/solapado, créditos accesibles | P2 | N04/N09 |
| N13 (K08/K18) | Validar y cerrar prólogo mínimo | PENDIENTE | F31/H3 | Omisión y 12 s terminan una sola vez; orientación/tamaño/audio revisados en visor | P2 | N02/N04 |
| N14 (K18) | Tutorial mínimo de mirada y controles de defensa | PENDIENTE | F36 | Jugador nuevo completa primera inspección y activa defensa con instrucciones verificables | P2 | N08/N09 |
| N15 (K18) | Verificar sonido mínimo y diferenciar eventos aprobados | PENDIENTE | F38 | Escucha Android; confirmación/alerta distinguibles y volumen cómodo. Música/impresora solo si equipo aprueba alcance | P2 | N04 |
| N16 | Decidir alcance de niveles 1–4 | PENDIENTE | F32–F35 | Acta/decisión de inclusión o descarte; dividir por nivel solo si se aprueban | P2 | N05 |
| N17 (K17) | Corregir superposiciones y lectura de señalética | PENDIENTE | T5: papeles/teclado, panel grande | Capturas antes/después desde posición real y aprobación en visor | P3 | N12 |
| N18 (K17/K19) | Medir y optimizar perfil Mobile | PENDIENTE | F42 | FPS/frame time, memoria, temperatura y comodidad en sesión sostenida; comparar cambios con baseline medido | P3 | N04 |
| N19 (K18/K20) | Publicar créditos completos dentro del juego | PENDIENTE | F41 | Autores, URLs, licencias/adaptaciones e IA legibles; origen de UI aclarado; recursos omitidos identificados | P4 | N12 |
| N20 (K03/K20) | Acreditar contribuciones de integrantes | PENDIENTE | F46 | Roles y commits reales de cada integrante revisados en repo compartido | P4 | N06/equipo |
| N21 (K19/K20) | Cerrar APK de entrega y regresión | PENDIENTE | F39/F40 | APK identificada, instalada, ambos modos/VR/audio/final/créditos PASS; guardar log y hash | P4 | N07–N15/N18/N19 |
| N22 (K20) | Grabar video 2–3 minutos | PENDIENTE | F47 | Video de celular con ambos modos y diferencia de resultado, duración comprobada | P4 | N21 |
| N23 (K20) | Crear ficha de hasta cuatro páginas | PENDIENTE | F48 | Contenido docente, roles, capturas y créditos; exportación/páginas revisadas | P4 | N05/N19/N21 |
| N24 (K20) | Ensayar presentación y argumento final | PENDIENTE | F49 | 7 min demo + 3 análisis; hipótesis argumentada apoyada en material y ficha | P4 | N22/N23 |
| N25 | Sincronizar documentación de estado | PENDIENTE | Sección 7 | Cabecera Kanban vigente, README/guía/build y LATEST coherentes; conservar históricos | P4 | Cierre de pruebas |

## 14. Próximas diez tareas y primera acción

Orden recomendado: **N01, N02, N03, N04, N05, N06, N07, N08, N09 y N10**. N05 y N06 pueden avanzar en paralelo al circuito de pruebas cuando el equipo esté disponible. No esperar documentos para probar lo ya implementado.

**Primera tarea inmediata: N01, revisar y preparar el perfil de compilación para que incluya BancoCorresponsal como escena inicial.** Antes de generar otra APK, comprobar la lista de escenas del perfil y preservar los ajustes locales. Esta auditoría solo lo propone: no cambió la configuración. A continuación ejecutar N02 y construir una APK vigente para el teléfono. Crear niveles o pulir decoración antes de cerrar ese recorrido retrasaría la validación del riesgo principal.
