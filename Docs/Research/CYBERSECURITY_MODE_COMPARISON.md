# Controles de ciberseguridad: ATAQUE y DEFENSA

Auditoría del código y los recursos locales, 11/11/2026. Escena `BancoCorresponsal`, Unity 6000.3.24f1. Este documento describe **la simulación**, no controles de un banco real ni una reconstrucción técnica del incidente histórico. Los seis expedientes son ficticios. No se modificó `BankingDomain.cs` ni `TrainingScenario.asset` para elaborar la matriz.

## Regla que realmente separa los modos

El jugador es el analista en ambos modos. Registro, alertas, expediente, impresora y decisiones están disponibles en ATAQUE y DEFENSA. La diferencia programada es que `DefenseManager.ApplyDefense` solo activa `Defense.Active` en DEFENSA. Con la defensa activa, si el analista elige **Aprobar** una orden con cualquier anomalía, `TransactionManager.Decide` sustituye el estado efectivo por `Recommended(data)` y marca `DefenseOverride`. Sin activar el control, DEFENSA tampoco interviene. El control no usa la etiqueta oculta `SimulatedFraud` para decidir; esa etiqueta solo alimenta la métrica de pérdida ficticia.

`Recommended`: falta de intermediario → Devolver; sin anomalías → Aprobar; las demás anomalías → Retener. `Correct` puntúa la **elección original** del analista; `SimulatedLoss` suma los montos con etiqueta interna de fraude simulado cuyo estado **efectivo** quedó Aprobado. El control puede reducir la pérdida sin mejorar la puntuación del jugador. La última decisión cierra el turno automáticamente; «Cerrar turno» antes de resolver todo explica el requisito.

El teléfono actual **simula** una conversación con una mesa de registros. No hay directorio de números confiables, comprobación técnica del destino, autenticación de interlocutor ni comunicación fuera del juego; por tanto no se puede afirmar que implementa una verificación independiente real. Su valor actual es practicar el contraste de una fuente ficticia y registrar el alcance limitado de su respuesta. Para enseñar un canal realmente independiente haría falta modelar selección de contacto verificado y aprobación editorial del guion, manteniendo esa capa separada de las reglas de pérdida.

## Matriz verificable

| Deficiencia / caso ficticio | ATAQUE | DEFENSA | Acción y evidencia del jugador | Consecuencia simulada y límite | Estado / prueba |
| --- | --- | --- | --- | --- | --- |
| SIM-001, sin discrepancia (`None`) | Consulta y decisión manual | Igual; la revisión reforzada no interviene | Leer expediente, contrastar registro y aprobar si procede | Aprobar no suma pérdida ficticia. No demuestra un control adicional. | Implementado. `TrainingScenario.asset`, `TransactionManager.Recommended`, DomainTests. |
| SIM-002, intermediario ausente | La observación de formato existe; sin corrección automática | Con control activo, un **Aprobar** se convierte en Devuelta | Revisar campo obligatorio, registrar observación opcional, decidir | Es defecto de formato, no prueba de fraude. La orden no lleva etiqueta de pérdida ficticia. | Implementado. `MissingIntermediary`, `DefenseOverride`, DomainTests/flow. |
| SIM-003, nombre de beneficiario distinto del registro | Registro y alerta muestran discrepancia; teléfono no está habilitado; decisión manual | Además del registro, teléfono contextual de prueba; con control activo, **Aprobar** queda Retenida | Comparar `Papell` frente a `Papel`, solicitar/leer llamada, **registrar** resultado si se elige, decidir | La llamada registra un dato de fuente ficticia; no confirma autorización, no activa defensa ni modifica pérdida. La revisión automática sí puede cambiar el estado. | Teléfono prototipo funcional solo DEFENSA/SIM-003, guion pendiente de aprobación. `VerificationConversation`, `DeskSessionJournal`, `TransactionManager`; DeskToolsTests y phone probe anteriores. |
| SIM-004, cuenta no hallada en registro ficticio | Consulta detectiva y decisión manual | Con control activo, **Aprobar** queda Retenida | Consultar REGISTRO y contrastar cuenta; evidencia opcional | «No encontrada» no demuestra fraude por sí sola. Pérdida ficticia solo si la orden con etiqueta interna queda Aprobada. | Implementado. `GameManager.InspectRegistry`, `UnknownAccount`, DomainTests. |
| SIM-005, monto inusual para el perfil simulado | Alerta y decisión manual | Con control activo, **Aprobar** queda Retenida | Leer monto y observación, decidir | Umbral/perfil detallado no está modelado; se usa un flag de anomalía predefinido. | Implementado como caso fijo, no como motor de perfiles. `UnusualAmount`, DomainTests. |
| SIM-006, coincidencia parcial de alerta | Alerta y decisión manual | Con control activo, **Aprobar** queda Retenida | Leer alerta, buscar contexto, decidir | Una coincidencia **no confirma fraude**. Este expediente no tiene etiqueta de pérdida ficticia, aunque la decisión recomendada de la simulación es Retener. | Implementado. `AlertMatch`, `FormatReferencePanel.ObservedAlerts`, DomainTests. |

### Tipos de control y evidencia

| Tipo | Implementado | Qué demuestra | Qué no demuestra |
| --- | --- | --- | --- |
| Detectivo | Datos de registro y observaciones de alertas en ambos modos | La información discrepante está disponible para el analista | Motor antifraude real, búsqueda externa o certeza de fraude |
| Procedimiento de verificación | Portapapeles de tres hojas y llamada SIM-003 en DEFENSA | Lectura, contraste, consulta contextual y registro explícito | Que abrir una fuente o marcar una hoja valida automáticamente sus datos |
| Evidencia de auditoría | `EvidenceManager` y diario auxiliar `DeskSessionJournal` | Entrada única por texto/expediente; fuentes abiertas, marcas manuales, elección y estado final | Persistencia entre cierres de app, firma, trazabilidad legal o auditoría bancaria real |
| Corrección automática simulada | Revisión reforzada activable solo en DEFENSA | Intercepta una aprobación de orden con anomalía y cambia el estado efectivo | Que el analista acertó o que una coincidencia prueba fraude |
| Preventivo propuesto, **no implementado** | Ninguno adicional | Una futura simulación podría enseñar doble autorización, verificación de identidad o límites | MFA, cifrado, segregación de funciones, control de credenciales o bloqueo real no existen en este proyecto |

La acción «EVIDENCIA» de la impresora registra la observación actual; el teléfono crea una entrada propia solo después de `REGISTRAR`. El diario distingue `Sources` (fuente **abierta**), `ManualChecks` (marca manual), `PhoneEvidence` (consulta registrada), `ChosenDecision` y `EffectiveStatus`. Los tres primeros no son equivalentes entre sí.

## Portapapeles: evaluación y propuesta **antes de cambiar hojas**

Las hojas actuales son: (1) formato y procedimiento en cuatro pasos, (2) datos reales del expediente y (3) observaciones y fuentes abiertas. Cada una permite `MARCAR REVISIÓN`; la nota dice explícitamente que es manual y no valida datos ni confirma fraude. Se muestran campos obligatorios, contraste y observaciones, pero las tres hojas son iguales en ambos modos. No hay una lista visual específica de controles reforzados; `Sources` se llena al abrir una fuente, aunque el jugador no la haya leído. El estado de la consulta telefónica se distingue por `PhoneEvidence`, no por abrir el panel.

**Propuesta para revisión editorial, aún no implementada:** conservar las tres hojas y su tamaño. En ATAQUE, añadir una frase corta en la primera/tercera hoja: «Revisión manual; no hay corrección automática disponible». En DEFENSA, mostrar «Control reforzado disponible / activo» a partir de `Defense.Active`, y en la tercera hoja separar visualmente `FUENTES ABIERTAS`, `MARCAS DEL ANALISTA` y `CONSULTA REGISTRADA`. No ocultar información básica en ATAQUE; el experimento compara la intervención del control, no la desaparición arbitraria del registro. No escribir «validado» cuando solo haya una marca. No poner `Recommended`, `SimulatedFraud` ni la respuesta correcta en una hoja. Antes de incorporar estas frases, revisar con docencia si revelan demasiado en la partida.

## Resultado educativo: estado actual y mejora viable

La pantalla de resultados muestra aciertos, pérdida ficticia, evidencias, valores de ATAQUE/DEFENSA cuando existen y seis filas con elección original, estado efectivo e intervención. La revisión posterior pagina fuentes y consulta telefónica. Esto es una base válida. Falta un **resumen causal breve** que indique si el control estuvo activo durante el turno y evite atribuir toda diferencia de pérdidas a la defensa cuando las elecciones de dos partidas difieren. La comparación numérica solo es causal si se repiten las mismas seis decisiones y se cambia únicamente la activación del control. El teléfono de prueba no altera el algoritmo.

Prioridad P1/P2: se añadió a la línea superior de resultados un estado breve leído de `session.Mode` y `session.Defense.Active`: «ATAQUE / Sin revisión automática», «DEFENSA / Control sin activar» o «DEFENSA / Control activo». No cambia cálculos ni decisiones. [Captura real del estado activo](../Previews/CybersecurityReview/README.md). Falta medir legibilidad binocular en HONOR y capturar los otros dos estados. Prioridad P3: piloto del guion telefónico y de las hojas con 3–5 observadores; preguntar «¿qué dato vino de qué fuente?», «¿qué acción hizo el analista?» y «¿qué cambió el sistema?». Prioridad P4: ampliar la explicación posterior solo si el texto cabe y la distinción mejora en el piloto.

## Pruebas y estado de validación

- **PASS .NET anterior:** DomainTests 27/27 y DeskToolsTests 37/37, documentados en `Docs/Context/IMMERSIVE_AUTONOMOUS_PROGRESS.md`. Los tests de dominio incluían recomendación, intervención y puntuación.
- **PASS Unity Editor anterior:** `flow`, `office`, `smoke`, `phone`, `clipboard`, `objects`, `regression` según sus recibos y el informe citado. La última pasada visual repitió `flow/office/smoke/objects/stationpc` y verificó cierre por dwell; consultar `Docs/Context/OFFICE_DETAIL_POLISH_REPORT.md`.
- **NO PROBADO:** instalación y uso físico de esta revisión en HONOR, claridad binocular, tiempos de demo en salón, Quest, dos mandos, simulador XR, agarre real y persistencia entre sesiones.

Esta pasada: **PASS .NET** DomainTests **37/37** (diez checks nuevos de la demo con decisiones idénticas, aciertos 5/6 y pérdida 20 000 frente a 0) y DeskToolsTests **37/37**. **PASS Unity Editor** `regression`, `flow`, `office`, `smoke`, `phone` y `clipboard`; recibos fechados en `Artifacts/CybersecurityReview/`. La [galería nueva](../Previews/CybersecurityReview/README.md) muestra resultados, revisión educativa, teléfono y portapapeles; las galerías históricas sobrescritas por los probes se restauraron desde los bytes de `HEAD` tras guardar las nuevas copias. No se ejecutó simulador XR ni hardware físico.

## Reversión y riesgos

La matriz no cambia reglas. Para cualquier texto nuevo, respaldar el archivo local ya modificado, ejecutar DomainTests/sondas y restaurar **solo** ese archivo desde la copia si falla; no hacer reset/clean/checkout del árbol con trabajo ajeno. Mantener `TrainingScenario.asset`, URP, Cardboard, loaders y el segundo monitor. La principal limitación académica actual es explicar la diferencia entre **detectar**, **registrar**, **decidir** e **intervenir**; agregar mandos no corrige esa confusión.
