# SWIFT — Plan ejecutable para Codex local

**Fecha:** 8 de noviembre de 2026. **Estado:** plan, no implementación. **Repositorio:** `D:\UnityGames\SWIFT_CodigoDeAlerta`. **Base observada:** rama `review/portal-dwell-feedback`, HEAD `c4a1278` y numerosos cambios locales. El commit no representa por sí solo el estado auditado.

Leer conjuntamente [diseño](IMMERSIVE_GAMEPLAY_DESIGN.md), [viabilidad](CARDBOARD_FEASIBILITY.md) e [investigación](VR_GAMEPLAY_RESEARCH.md). **[P]** observado; **[R]** propuesta. Todos los scripts/prefabs nuevos nombrados aquí son propuestas, no archivos ya creados.

## 1. Orden y alcance recomendado

Cambiar el orden inicial sugerido por el usuario: **base verificable → portapapeles ampliado → teléfono de un caso → pulido de asiento → integración educativa → validación y sonido**.

Justificación: el portapapeles y sus materiales ya existen; permiten comprobar inspección, lectura y bloqueo de acciones detrás del panel con menos variables. El teléfono reutiliza esa base y tiene el mayor potencial educativo nuevo. Implementar primero una conversación completa sin resolver interacción y presentación multiplica los puntos de fallo.

Si la entrega es inminente, completar solo base y portapapeles reversible; no habilitar el teléfono sin prueba física. Ninguna fecha límite concreta fue facilitada.

Estimación orientativa: base 1–2 jornadas; portapapeles 2–4; teléfono 3–5; asiento 1–2; integración 2–3; pulido/prueba 2–4. Total secuencial aproximado **11–20 jornadas de seis horas**, con infraestructura compartida y sin producción externa ni esperas de hardware. Un corte reducido de portapapeles y una llamada puede ocupar 5–8 jornadas si la base ya pasa y hay dispositivo disponible. Son estimaciones, no un compromiso de calendario.

## 2. Mapa de arquitectura existente [P]

Rutas relativas a `Assets/_Project/Scripts/`, salvo indicación diferente.

| Archivo / tipo real | Responsabilidad observada | Integración propuesta |
|---|---|---|
| `Core/GameManager.cs` | Sesión, prólogo, decisiones, evidencia, defensa, mensajes, `Changed`, pérdidas comparadas | Observar cambios de sesión/expediente; no emitir mensajes por cada línea de llamada |
| `Core/ScenarioData.cs` | ScriptableObject con transacciones y contexto histórico | Conservar; catálogo telefónico independiente referenciado desde el nuevo coordinador |
| `Banking/BankingDomain.cs` | `TransactionData`, `TransactionRecord`, `TransactionManager`, `EvidenceManager`, `DefenseManager`, `GameSession`, `TrainingData` | Fuente de verdad; no cambiar reglas ni cálculos para estas funciones |
| `Interaction/BankInteractable.cs` | Enum `BankAction` y despacho de acciones; guardas de tutorial/prólogo/asiento | Añadir rutas al final del enum y guarda modal uniforme |
| `Interaction/VRInteractionManager.cs` | Raycast, foco, reloj único de dwell, bloqueo hasta apartar mirada | Reutilizar reloj; como máximo ampliar elegibilidad/bloqueo, sin otro temporizador |
| `UI/DocumentFocus.cs` | Presentación del foco y `fillAmount` a partir del manager | Utilizar en controles nuevos; conservar identidad del objetivo durante fijación |
| `UI/UIManager.cs`, `UI/BankDocumentUI.cs` | Construcción runtime y páginas de documentación/decisión | Mantener; abrir desde herramienta cuando corresponde, no duplicar dominio |
| `UI/FormatReferencePanel.cs` | Referencia de solo lectura; elimina órdenes de decisión de alertas presentadas | Reutilizar `ObservedAlerts` y datos disponibles |
| `UI/OperationsStation.cs` | Estados del puesto y visibilidad de escritorio, panel principal y secundario | Añadir política explícita de presentación del secundario |
| `VR/AnalystSeat.cs` | Fade, comprobación de destino, posición del rig, estado, telepads | Pulido opcional por transiciones aceptadas |
| `VR/HeadPoseAdapter.cs` | Pose de cabeza y altura virtual Android | No modificar para animar una postura corporal |
| `Audio/FeedbackAudio.cs`, `Audio/AudioCueLibrary.cs` | Cinco buses, ocho eventos, tonos temporales, ganancias y grupos | Controlador de escritorio separado para clips nuevos |
| `Testing/GameplayFlowProbe.cs`, `BankOfficeProbe.cs`, `PrototypeSmokeProbe.cs` | Sondas existentes de flujo y escena | Ejecutar como regresión y añadir sonda específica de herramientas |

**Hallazgos que cambian la implementación:**

1. `BankAction.Notebook` ya abre `BankDocumentUI.Page.Procedure`. Debe conservarse como fallback si la nueva función está deshabilitada.
2. `OperationsStation.RefreshVisibility` actualiza el secundario continuamente. Desactivarlo una vez con `SetActive(false)` no lo sustituye: su política de visibilidad lo reactivaría.
3. El dwell PC solo es sostenido para tarjetas de modo, Sit/Stand y objetivos con `DocumentFocus`; otros usan clic. Los controles nuevos deben entrar en la ruta existente de dwell, o la prueba PC no representará los 1,5 s.
4. `VRInteractionManager.Activated` se emite antes de `target.Activate()`. No sirve como prueba de que una acción bancaria o una evidencia fue aceptada.
5. `GameManager.RegisterEvidence(string)` usa el expediente actual; una respuesta tardía puede contaminar otro caso si no se valida contexto.
6. `TransactionRecord` no conserva la `Decision` original por separado. No inferirla de `Status` si hubo `DefenseOverride`.
7. `FeedbackAudio` tiene `temporary[8]` y `lastCue[8]`; no añadir un valor al enum sin revisar índices y selección de clips.
8. La UI se construye en runtime. Modificar solamente un prefab puede no afectar a lo que realmente aparece; revisar la ruta creadora y no relanzar builders que rehacen la oficina.

## 3. Componentes nuevos propuestos [R]

Mantenerlos en una carpeta aislada, por ejemplo `Assets/_Project/Scripts/DeskTools/`. No introducir frameworks, contenedores de dependencias ni un segundo GameManager.

| Tipo propuesto | Datos / responsabilidad |
|---|---|
| `DeskToolsProfile` — ScriptableObject | Flags `ClipboardEnabled`, `PhoneEnabled`, `SeatPolishEnabled`; política `ReferencePresentation` con `LegacySecondary`, `ClipboardTrial`, `ClipboardPrimary` |
| `DeskInteractionCoordinator` — MonoBehaviour | Referencias explícitas a manager, seat, gaze y vistas; dueño de la herramienta activa, identidad de sesión y cancelación; obtiene contexto en `Changed` |
| `DeskInspectionView` — MonoBehaviour | Un panel world-space reutilizable: título, ID, contenido, anterior/siguiente, registrar/cerrar; colliders y texto estables |
| `ClipboardController` — MonoBehaviour | Hoja, datos leídos, apertura/cierre y marcas manuales por contexto; no calcula recomendación |
| `VerificationCallCatalog` — ScriptableObject | Definiciones inmutables de llamadas por ID; interlocutor, campos visibles, líneas, resultado escrito y clips opcionales |
| `VerificationCallController` — MonoBehaviour | Máquina de estados de una llamada; elegibilidad, transcripción, cancelación, registro explícito |
| `DeskSessionJournal` — clase C# | Estado runtime: marcas, consultas, evidencia registrada y, en fase 4, decisiones aceptadas; claves de sesión/expediente |
| `DeskAudioController` — MonoBehaviour | AudioSources y clips explícitos; responde a cambios de estado, usa ganancias/grupos de `AudioCueLibrary` |
| `SeatPresentationFeedback` — MonoBehaviour opcional | Respuesta audiovisual por entrada/salida aceptada, no por cada `LateUpdate` |
| `DeskToolsProbe` / pruebas puras | Casos de regresión específicos y recibos por plataforma |

Prefabs propuestos en `Assets/_Project/Prefabs/DeskTools/`: `DeskInspectionPanel`, `VerificationPhone`, `VerificationClipboard` y raíz `DeskToolsRoot`. Para extraer el portapapeles, conservar original y crear referencias nuevas con GUID nuevo; no duplicar `.meta` entre assets activos. Configuración y catálogo en una carpeta propia bajo `Assets/_Project/Resources/DeskTools/` solo si la carga por Resources resulta necesaria; preferir referencias serializadas explícitas.

Flags desactivados por defecto en la configuración de entrega hasta superar su fase. La escena de revisión puede activarlos. El modelo de teléfono simple se genera solo durante implementación autorizada, no como requisito de esta investigación.

### Catálogo telefónico: esquema mínimo

```text
CallDefinition
  CallId                      identificador único
  TransactionId               ID exacto: "SIM-003", no "SIM003"
  SpeakerLabel                fuente ficticia identificada
  ResultKind                  FieldStatement | Inconclusive | NoAnswer
  ComparedField               campo permitido de los datos existentes
  Lines[]                     texto autorado con tokens permitidos
  EvidenceTemplate            resumen factual, sin decisión recomendada
  OptionalVoiceClips[]         vacío permitido; subtítulos siempre disponibles
  AvailabilityMode            Defense
```

Los resultados no usan `SimulatedFraud`. Los tokens se limitan a datos de presentación aprobados como `RegisteredName` y `Account`; un token desconocido falla validación, no imprime una cadena vacía silenciosamente. La definición referencia datos del expediente para evitar copiar nombres que después diverjan. Las frases narrativas nuevas son ficticias y deben revisarse.

Validar IDs únicos, existencia del expediente, líneas no vacías, resultado coherente, ausencia de recomendaciones y coincidencia con el caso autorizado. El asset permanece inmutable en Play; marcas y estados viven en `DeskSessionJournal`. No modificar `ScenarioData.Transactions`, anomalías, montos, etiquetas internas o contexto histórico para hacer encajar una llamada.

## 4. Estados, eventos y flujo

### Estados propuestos

```text
Herramienta: None → Clipboard | Phone → None
Llamada: Unavailable / Ready → Requested → Ringing → Connected
         → Reviewing → Recorded → Closed
         Requested → NoAnswer → Reviewing
         cualquier estado abierto → Cancelled
```

El controlador comprueba elegibilidad cada vez que recibe una acción. `Recorded` es idempotente. NoResponse escrito del caso y cancelación del usuario son resultados diferentes. Nueva sesión limpia todo; nuevo expediente cierra herramientas y conserva únicamente historial asociado al expediente anterior. Pausa detiene audio y acciones; al regresar se reanuda una vista coherente con nueva fijación, sin ejecutar comandos acumulados.

Eventos nuevos orientativos: `InspectionOpened`, `InspectionClosed`, `CallStateChanged`, `TranscriptAdvanced`, `ChecklistMarked`, `CallEvidenceCommitted`. Emitirlos en transiciones reales; suscriptores se registran y retiran de forma simétrica. `CallEvidenceCommitted` ocurre una sola vez después de guardar, nunca desde `Update` ni por escuchar `Changed` indiscriminadamente.

Flujo de datos:

```text
ScenarioData → GameSession → Current.Data ─┐
                                         ├→ vista de solo lectura
VerificationCallCatalog → CallController ┘
mirada → VRInteractionManager → BankInteractable → coordinador/controlador
Registrar → comprobar sesión + ID + llamada → evidencia existente + diario auxiliar
Decidir → GameManager.Decide → dominio actual → resultados actuales
                                       └→ registro educativo de decisión aceptada
```

La sesión se identifica por referencia del `GameSession` y un token runtime nuevo cada reinicio. No basta `mode + TransactionId`, porque un nuevo turno reutiliza los IDs. Una llamada conserva ese contexto al solicitarse; si cambia, cancela y no registra nada. Comparar antes de invocar el API que asocia evidencia al expediente actual; actualizar diario solo si el registro fue aceptado, comprobando el estado resultante si el API público no devuelve un booleano.

### Guarda modal sin romper mirada

Introducir una consulta de elegibilidad central, por ejemplo `CanActivate`, que haga bypass al estar desactivado el perfil nuevo. Debe ser usada tanto por el raycast como por `BankInteractable.Activate`; una comprobación solo visual no protege llamadas directas. Revisar las ramas tempranas Sit/Stand: salir puede cancelar la inspección antes de la transición, pero no debe saltarse la coordinación accidentalmente.

Durante inspección, permitir sus controles y salida/cierre; bloquear decisiones o teletransportes detrás. El manager conserva el único reloj de 1,5 s. Un objetivo bloqueado muestra progreso cero, no una carga oculta. No deshabilitar/recrear el manager para cambiar páginas. Mantener la fijación completada bloqueada hasta apartar mirada; si se reemplaza contenido bajo el mismo objetivo, exigir una nueva entrada de foco mediante versión de contenido, sin cronómetro nuevo.

No utilizar `Activated` para registrar decisiones aceptadas. En fase 4 puede añadirse a `GameManager` un evento descriptivo `DecisionCommitted` tras una decisión válida, capturando el registro y la elección antes de avanzar. Ese evento no modifica el dominio. Los caminos existentes deben seguir funcionando con el diario ausente.

## 5. Fases verificables

### Fase 0 — Base y protección, 1–2 jornadas

**Archivos afectados:** solo informe de base, recibos y escena de revisión nueva. **Scripts/prefabs/datos nuevos:** ninguno de producción; copia de revisión con metadatos propios.

1. Leer `git status`, diff y documentos actuales. No reset, clean, checkout destructivo, commit ni push automático.
2. Copiar archivos que se tocarán y sus `.meta` a respaldo fechado fuera de Assets, por ejemplo `Artifacts/Backups/DeskTools-<fecha>/`; registrar rutas, hashes y estado inicial. Respaldo de escena y prefabs antes de editarlos.
3. Crear una escena de revisión desde el estado local actual. No partir del HEAD ignorando cambios sin versionar, ni ejecutar builders que reemplacen BancoCorresponsal.
4. Ejecutar regresión existente con Unity 6000.3.24f1 y guardar logs. Capturar puesto y secundario desde cámara reproducible.

**Pruebas:** DomainTests, GameplayFlowProbe, BankOfficeProbe, PrototypeSmokeProbe, más entrada/salida y prólogo de la UI actual. **Riesgo:** una sonda puede cargar/reconstruir una escena; inspeccionar su runner antes de ejecutarlo y no guardar modificaciones transitorias sobre la escena principal.

**Aceptación:** baseline reproducible, fallos preexistentes identificados, respaldo recuperable. **Reversión:** no hay función nueva que retirar; eliminar solo artefactos propios si se requiere, preservando recibos e historial. Si la base falla, documentar y resolver antes de adjudicar fallos a las nuevas mecánicas.

### Fase 1 — Portapapeles ampliado, 2–4 jornadas

**Nuevos:** `DeskToolsProfile`, `DeskInteractionCoordinator`, `DeskInspectionView`, `ClipboardController`, `DeskSessionJournal`; prefabs de panel/portapapeles/raíz. Datos: una hoja contextual sobre `Current.Data`, sin nuevos expedientes.

**Existentes a tocar mínimamente:** `BankInteractable` para la ruta Notebook/fallback y acciones añadidas; `VRInteractionManager` solo para elegibilidad modal; `OperationsStation` para política del secundario. Reutilizar estilo y `FormatReferencePanel` sin alterar reglas. Escena de revisión; todavía no sustituir la principal.

**Pruebas:** apertura, cambio de hoja, cierre, nombres largos, cancelación a media fijación, no transferencia de progreso, no botones ocultos, marcas separadas por expediente y sesión. Cambiar `ReferencePresentation` verifica reversibilidad sin borrar el monitor.

**Riesgos:** canvas detrás del collider del PC; reactivación del secundario; UI reconstruida durante dwell; marca interpretada como validación.

**Aceptación:** consulta útil y legible, datos fieles, cero cambios de decisiones/pérdidas, regreso inequívoco al expediente; capturas reales antes/después desde mismo asiento. Validación física requerida antes de declararla mejora VR aceptada.

**Reversión:** apagar `ClipboardEnabled`, seleccionar `LegacySecondary` y volver a la ruta Notebook → Procedure. Mantener las guardas inertes con flags apagados. No restaurar una escena completa sobre cambios posteriores de terceros.

### Fase 2 — Teléfono A, un expediente, 3–5 jornadas

**Nuevos:** `VerificationCallCatalog`, `VerificationCallController`, prefab `VerificationPhone`; definición solo para `SIM-003`. Extender diario y vista existentes. Audio inicialmente opcional; no bloquear la fase por falta de voces.

**Existentes:** nuevas acciones al final de `BankAction` y rutas al coordinador, sin reordenar valores serializados; llamada al registro de evidencia existente con contexto validado. `ScenarioData` y `BankingDomain` quedan intactos.

**Pruebas:** solo DEFENSA, controles automáticos activos/inactivos, cancelar/repetir, sin respuesta e inconclusa con fixtures de prueba, cambio de expediente o sesión, registro único, audio ausente, subtítulos completos. Fixtures no agregan casos al catálogo de entrega.

**Riesgos:** evidencia del caso anterior, doble registro, revelación de respuesta, acciones encadenadas por mirada sostenida.

**Aceptación:** una llamada reproducible aporta dato y procedencia, no recomienda decisión; no altera `Defense.Active`; tres decisiones siguen usando reglas actuales; sin dependencia online; estado reiniciado al volver al portal. Capturas de teléfono pendiente, conversación y evidencia.

**Reversión:** `PhoneEnabled=false`; retirar visualmente solo la raíz propia si procede. Repetir escenario con teléfono deshabilitado y verificar equivalencia con base.

### Fase 3 — Asiento híbrido, 1–2 jornadas

**Nuevos:** `SeatPresentationFeedback`, configuración opcional de clip/animación; ningún modelo necesario. **Existentes:** integrar con `AnalystSeat` mediante evento de transición aceptada o detección de cambio de estado almacenado; no reaccionar cada frame. No cambiar `HeadPoseAdapter`.

**Pruebas:** entrar/salir repetidamente; destino ocupado; cancelación de herramienta; pausa durante fade; componente deshabilitado; telepads restaurados; distintas orientaciones de cabeza; objetivo de salida alcanzable.

**Riesgos:** repetir sonido, mover colliders antes de validar destino, dejar negro permanente o alterar origen al cancelar.

**Aceptación:** misma recolocación y seguimiento que la base, sin desplazamiento continuo de cámara; confirmación sonora opcional; cero doble transición. La comodidad necesita ensayo físico.

**Reversión:** `SeatPolishEnabled=false`, audio detenido y transform de silla restaurado; conservar el `AnalystSeat` anterior como camino funcional. No aumentar duración del fade sin justificación de prueba.

### Fase 4 — Integración y explicación, 2–3 jornadas

**Nuevos datos:** segundo diálogo opcional `SIM-004` inconcluso; fichas educativas escritas sobre casos existentes. **Existentes:** diario, controladores y presentación de resultados; evento `DecisionCommitted` mínimo en `GameManager` si se muestra elección original. Añadir una vista de consultas al resultado sin recalcular totales.

**Pruebas:** seis expedientes, ATAQUE/DEFENSA, todos los patrones de decisión; repetición de turno con mismos IDs; comparación de totales; decisión humana distinta del estado efectivo; datos no disponibles mostrados como tales; sin instrucciones automáticas durante revisión.

**Riesgos:** presentar estado efectivo como elección del jugador, nuevas contradicciones que invaliden casos, demasiados pasos obligatorios.

**Aceptación:** misma salida del dominio para idéntica secuencia de decisiones con herramientas encendidas/apagadas; consulta opcional; información educativa fiel. No extender llamadas a seis casos por defecto.

**Reversión:** catálogo vuelve a un caso, ocultar detalles auxiliares y conservar resultados originales. Los registros nuevos no son necesarios para completar una sesión.

### Fase 5 — Sonido, piloto y cierre, 2–4 jornadas

**Nuevos:** `DeskAudioController`, clips con licencia y documentación, `DeskToolsProbe`, informe de dispositivo. **Existentes:** asignaciones del perfil, integración probada en la escena principal tras respaldo; créditos. No nuevas dependencias, URP ni Cardboard.

**Pruebas:** suite completa, ciclo de foco, pausa/reanudación, audio silenciado, rondas en HONOR, 10–15 minutos de estabilidad inicial y piloto con estudiantes. Comparar base/variante con igual calidad y captura; grabar versión y configuración.

**Aceptación:** no hay bloqueos ni regresiones de resultados; lectura y controles utilizables en visor; sonidos emitidos solo una vez por evento; APK nueva con log/hash y sin sobrescribir la antigua. Si una condición falta, marcarla pendiente, no PASS.

**Reversión:** flags individuales; conservar una APK de base validada. Deshabilitar lo que no haya superado pruebas físicas para la demostración. El cierre visual no autoriza por sí solo commit o publicación.

## 6. Plan concreto de pruebas

### Regresión existente

`Tools/DomainTests/DomainTests.csproj` es un ejecutable .NET 10 que compila el dominio real, no un proyecto de NUnit que se ejecute automáticamente con `dotnet test`. Comando propuesto para la futura fase de implementación, **no ejecutado en esta investigación**:

```powershell
dotnet run --project Tools/DomainTests/DomainTests.csproj
```

Usar el SDK existente; si no está disponible, documentar la falta, sin instalar dependencias por sorpresa. Para las sondas Unity usar runners ya presentes después de inspeccionarlos. `Swift.EditorTools.GameplayFlowValidation.Run` abre BancoCorresponsal y entra a Play; por eso no debe invocarse sobre una escena con cambios sin guardar. Los runners de revisión también pueden agregar objetos transitorios. No iniciar una segunda instancia que compita por el mismo proyecto abierto.

Guardar por ejecución: comando/método real, versión, escena, configuración, fecha, exit code cuando corresponda, log y recibo. Las evidencias PASS históricas de noviembre 4 no sustituyen nuevas ejecuciones.

### Regresión numérica esperada del dominio actual [P, derivada del código]

Estas cifras pertenecen exclusivamente a los seis casos ficticios, no al incidente histórico. Se deben confirmar ejecutando pruebas:

| Secuencia | Aciertos esperados | Pérdida simulada esperada |
|---|---|---|
| ATAQUE: aprobar las seis | 1/6 | 965.000 |
| DEFENSA sin activar controles: aprobar las seis | 1/6 | 965.000 |
| DEFENSA con controles activos desde el inicio: aprobar las seis | 1/6 | 0; hay intervención del control |
| Aprobar, Devolver, Retener, Retener, Retener, Retener | 6/6 | 0 en ambos modos |

Añadir comparación de cada `Status`, `DefenseOverride` y `CorrectDecision`, no solo suma final. Ejecutar exactamente la misma secuencia con teléfono/portapapeles activados y desactivados. Abrir herramientas, pasar hojas y marcar revisión no debe modificar ninguna de esas propiedades.

### Nuevas pruebas puras y de integración propuestas

| ID | Comprobación | Resultado exigido |
|---|---|---|
| D01 | Catálogo con ID inexistente, token desconocido o duplicado | Validación falla con mensaje útil |
| D02 | Una llamada intenta acceder a etiqueta de fraude/recomendación | Campo no disponible para presentación; no filtración de solución |
| D03 | Registrar dos veces la misma llamada | Una evidencia y una confirmación |
| D04 | Cambiar de SIM-003 a SIM-004 antes de registrar | No se guarda evidencia antigua en SIM-004 |
| D05 | Reiniciar DEFENSA y volver a SIM-003 | Contexto nuevo; no llamadas/marcas heredadas |
| D06 | NoAnswer frente a cancelación del jugador | Estados y textos diferentes; ninguna decisión automática |
| D07 | Cambiar `Defense.Active` | No altera el contenido factual de la llamada ni la disponibilidad por modo |
| D08 | Registrar decisión corregida por defensa | Diario guarda elección original y resultado efectivo por separado |
| I01 | Foco 0,75 s con dwell 1,5 s | Progreso cercano a 0,5, tolerancia de uno o dos frames |
| I02 | Foco 1,5 s | Una activación al cruzar umbral, sin adelanto significativo |
| I03 | Apartar mirada o soltar ratón a 0,75 s en ruta PC sostenida | Progreso cero; no carga escondida |
| I04 | Cambiar objetivo | El segundo empieza en cero |
| I05 | Mantener mirada tras activar y tras cambiar contenido | Ninguna segunda acción sin rearme explícito |
| I06 | Botón bancario detrás de la inspección | No recibe acción; controles visibles del modal sí |
| I07 | Estado bloqueado, prólogo/tutorial/pausa | No acumula progreso ni dispara al desbloquear |
| I08 | Cambiar modo, salir del puesto y volver al portal | Paneles, audio y barras reiniciados; telepads coherentes |
| I09 | Datos largos y valores ausentes | Sin desbordes ni letra reducida; «No informado» cuando corresponde |
| I10 | Varias notificaciones `GameManager.Changed` sin transición | No repite timbre, página, voz ni confirmación |
| I11 | Prólogo completo y omitido, ambos modos | Flujo anterior preservado |
| I12 | Flags apagados | Comportamiento equivalente a la base, incluido Notebook y secundario |

No probar solo una función que asigne `fillAmount`: las pruebas de progreso deben enfocar colliders reales mediante el raycast del manager. Usar `SimulateGazeInEditor` para un ensayo determinista y probar también la ruta PC de ratón sostenido. El test puede controlar tiempos; la UI productiva no puede tener un reloj de dwell paralelo.

Para las pruebas puras nuevas, no acoplar el proyecto .NET del dominio a `UnityEngine`. Separar validación de datos/estado en C# puro o usar Unity Test Framework ya instalado para componentes Unity. No añadir paquetes por conveniencia.

### Capturas y Android

Propuesta de carpeta: `Docs/Previews/DeskToolsReview/<fecha>/`, con manifiesto de cámara, resolución, plataforma, sesión/ID y estado. Capturas necesarias: puesto anterior y posterior desde misma pose, portapapeles 0/50/100 % de progreso real, hoja contextual, teléfono pendiente/en llamada/registrado, vista sentada y resultados. Android debe tener su propia carpeta y no reciclar PNG del Editor como evidencia.

El checklist físico completo está en [viabilidad](CARDBOARD_FEASIBILITY.md). Registrar lectura, estabilidad del foco, comodidad, audio y desempeño térmico. Si la conexión o hardware no permite obtener una medida, indicarlo; no inventar FPS ni tiempos GPU.

## 7. Gestión del riesgo y reversión

Los cambios locales ajenos tienen prioridad. Antes de cada fase, volver a comprobar estado, fechas y hashes de los archivos objetivo. Un respaldo inicial no autoriza sobrescribir trabajo realizado después. Si se necesita recuperar, comparar y revertir únicamente el bloque propio; restaurar una copia completa solo si se demuestra que no contiene cambios ajenos posteriores.

No mover/reemplazar la oficina, no duplicar GUID de assets, no cambiar orden numérico de `BankAction`, no actualizar paquetes ni pipeline. Mantener los PNG y recursos originales. El interruptor de función es la primera reversión; la copia recuperable es el segundo recurso.

Riesgos decisivos para limitar alcance: falta de prueba física, información redundante, regresión de dwell, contaminación entre expedientes y rendimiento sin margen. Ante ellos se recorta la función nueva antes de alterar reglas bancarias o la configuración XR.

## 8. Primer encargo sugerido para Codex

```text
Lee Docs/Research/VR_GAMEPLAY_RESEARCH.md, IMMERSIVE_GAMEPLAY_DESIGN.md,
CARDBOARD_FEASIBILITY.md y CODEX_IMPLEMENTATION_ROADMAP.md.
Implementa únicamente las fases 0 y 1: base verificable y portapapeles ampliado.
Trabaja con Unity 6000.3.24f1, URP 17.3.0 y Cardboard existentes.

Comprueba git status y los cambios locales. Guarda respaldo fechado de cada
archivo, escena o prefab que vayas a editar, sin sobrescribir cambios ajenos.
Usa una escena de revisión del estado local y conserva BancoCorresponsal.
Reutiliza Analyst review clipboard, su madera/papel/clip y el estilo TMP.
Abre una vista legible de solo lectura con el ID y datos del expediente actual.
Mantén Notebook → Procedure y el monitor secundario como fallback configurable.
Reutiliza VRInteractionManager, DocumentFocus y el dwell real de 1,5 segundos.
Impide acciones a través del panel y activaciones encadenadas por mirada sostenida.
No cambies cálculos, anomalías, defensa, URP, Cardboard ni dependencias.
No implementes aún teléfono, manos ni físicas nuevas.

Ejecuta las regresiones existentes y las pruebas de foco/contexto de la fase 1.
Genera capturas reales antes/después desde la misma pose y documenta resultados.
Separa pruebas Editor de Android; lo que no puedas ejecutar queda pendiente.
No hagas commit ni push. Entrega archivos afectados, respaldo, evidencias,
problemas encontrados y pasos exactos de reversión.
```

## 9. Estado de esta entrega documental

Se inspeccionó y se diseñó; no se ejecutaron estas fases ni las pruebas enumeradas. Los entregables creados son los cuatro Markdown de `Docs/Research/`. El teléfono sigue ausente, el portapapeles sigue con su comportamiento actual y la validación física de las propuestas continúa pendiente.
