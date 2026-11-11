# Propuesta de puntuación y resultados

**Diseño para aprobación; no está implementado.** Los resultados actuales siguen siendo `Correct` (elecciones que coinciden con `Recommended`) y `SimulatedLoss` (suma de montos con etiqueta interna de fraude simulado que terminaron Aprobados). El nuevo desglose se añadiría **en paralelo**, sin cambiar esos cálculos hasta aprobar el diseño y actualizar pruebas. Véanse [bucle propuesto](GAMEPLAY_REDESIGN_PROPOSAL.md) y [casos](CASE_BY_CASE_INTERACTIONS.md).

## 1. Qué debe medir cada columna

| Métrica propuesta | Definición precisa | Fuente de datos / advertencia |
| --- | --- | --- |
| **Expedientes** | 6 en el catálogo actual | `Transactions.Records.Count`. |
| **Decisiones correctas / incorrectas** | Correcta si la elección **original** coincide con la recomendación educativa; incorrecta en caso contrario | Guardar la elección en un registro de turno estable. `CorrectDecision` ya existe, pero no almacena el tipo de error. |
| **Aprobaciones indebidas** | Elección original Aprobar cuando la recomendación no era Aprobar | Contar aunque DEFENSA haya corregido el estado; distingue error humano de rescate. |
| **Aprobaciones indebidas efectivas** | Caso anterior cuyo estado terminó Aprobado | Permite mostrar qué aprobaciones pasaron realmente. No equivale siempre a pérdida. |
| **Retenciones innecesarias** | Elección Retener cuando la recomendación era Aprobar (`SIM-001` hoy) | Métrica de fricción operativa, **no** dinero perdido. |
| **Retención en lugar de devolución** | Elección Retener cuando la recomendación era Devolver (`SIM-002`) | Error de clasificación de formato, distinto de retener una orden normal. |
| **Devoluciones incorrectas** | Elección Devolver cuando la recomendación era Aprobar o Retener | Muestra rechazo erróneo sin llamarlo fraude. |
| **Pérdida ficticia** | `SimulatedLoss` existente: montos etiquetados internamente para el ejercicio y Aprobados al final | No sumar retenciones ni devoluciones como pérdidas. No es ingreso ni pérdida de un banco real. |
| **Monto aprobado / retenido / devuelto** | Suma de `Amount` por **estado efectivo final** `Approved` / `Held` / `Returned` | Son volúmenes de órdenes ficticias. Su suma debe igualar el total de los seis montos si el turno está completo. |
| **Monto procesado correctamente** | Suma de montos cuyo estado final es Aprobado **y** cuya elección Aprobar era coherente | No es «ganancia bancaria». No sumar una aprobación incorrecta rescatada ni una orden retenida. |
| **Hallazgos sustentados** | Por caso, observación elegida por el jugador que coincide con un dato consultado de una fuente identificada | Abrir una fuente, marcar una hoja o pulsar EVIDENCIA no bastan. Evaluar **después** de la decisión, sin filtrar respuesta. |

No usar `SimulatedFraud` para evaluar hallazgos ni para decidir si una coincidencia observada es verdadera. Esa etiqueta oculta solo alimenta la pérdida ficticia vigente. La clasificación de decisiones se basa en `Recommended(data)` por compatibilidad inicial; si se aprueba una resolución posterior de casos, habrá que rediseñar la rúbrica.

## 2. Puntuación mínima viable propuesta

**Máximo: 90 puntos**, mostrado como `X/90`, sin convertirlo todavía en una nota académica. Cada uno de los seis expedientes aporta:

- **10 puntos por decisión coherente** según la regla educativa actual: máximo 60. La elección original cuenta incluso cuando la defensa la sustituye.
- **5 puntos por un hallazgo sustentado** del caso: máximo 30. Debe existir una fuente consultada pertinente y una afirmación explícita correcta elegida **antes** de decidir. Solo el primer hallazgo pertinente cuenta para evitar farmear puntos; los demás pueden quedar en el diario sin puntuación. Una afirmación inventada o contradicha obtiene 0, pero no impide decidir.

**Cálculo:** `Puntos = 10 × decisiones_coherentes + 5 × casos_con_hallazgo_sustentado`. Los subtipos de error, montos y pérdida son **métricas paralelas**, no penalizaciones duplicadas. El jugador puede investigar y aun así equivocarse; el control puede evitar la pérdida y aun así dejarlo con baja puntuación.

Para que la rúbrica sea justa en los seis casos actuales, el hallazgo evaluable debe ser un dato disponible en el juego:

| Caso | Hallazgo sustentado de ejemplo | Fuente necesaria |
| --- | --- | --- |
| `SIM-001` | Titular y beneficiario coinciden; campos obligatorios presentes | REGISTRO + expediente/requisitos. |
| `SIM-002` | Intermediario no informado | Expediente + hoja de requisitos. |
| `SIM-003` | Titular «Papel» difiere de beneficiario «Papell» | REGISTRO + expediente. La llamada es opcional y no puede ser requisito de puntos porque ATAQUE no la tiene. |
| `SIM-004` | Cuenta no hallada en registro | Búsqueda explícita en REGISTRO. |
| `SIM-005` | Existe alerta de monto, pero **no hay umbral numérico consultable todavía** | Expediente + ALERTAS; reconocer la limitación es mejor que inventar una política. Tras autorar un umbral, revisar esta rúbrica. |
| `SIM-006` | Coincidencia parcial no concluyente | Detalle de ALERTAS; no registrar «fraude confirmado». |

Los controles de registro deben ofrecer opciones neutrales grandes en VR (`coincide`, `difiere`, `no consta`, `sin comprobar`), no una casilla «respuesta correcta». La validación se hace al cerrar el caso y compara observación, fuente y dato. La apertura de una pestaña se guarda como **consulta**, no como hallazgo. Si una fuente no estuvo disponible por un fallo del sistema, la prueba debe bloquear la calificación de ese caso o marcarlo «no evaluable», no otorgar cero al jugador.

### Estrategias ilustrativas, todavía no probadas como puntuación nueva

| Estrategia | Decisiones coherentes actuales | Con cero hallazgos | Con seis hallazgos sustentados | Pérdida ficticia actual |
| --- | --- | --- | --- | --- |
| Elegir la recomendación en los seis | 6/6 | 60/90 | 90/90 | USD 0. |
| Retener las seis órdenes | 4/6 | 40/90 | 70/90 | USD 0; registra una retención innecesaria de `SIM-001` y una retención donde `SIM-002` requería devolución. |
| Aprobar las seis en ATAQUE | 1/6 | 10/90 | 40/90 | USD 965 000. |
| Aprobar las seis en DEFENSA con CONTROLES activos | 1/6 | 10/90 | 40/90 | USD 0 por intervención; la mala puntuación permanece. |
| Demo `Aprobar, Devolver, Aprobar, Retener, Retener, Retener` | 5/6 | 50/90 | 80/90 | ATAQUE USD 20 000; DEFENSA activa USD 0. |

Las columnas de **nueva puntuación son proyecciones de la fórmula propuesta**, no resultados ejecutados. Los valores 6/6, 4/6, 1/6, 5/6 y pérdidas citadas se desprenden de la regla y del catálogo actuales; el caso demo y aprobar todo cuentan con pruebas de dominio existentes. No presentar el nuevo `X/90` como implementado.

## 3. Montos y conservación de estados

El total nominal de los seis expedientes actuales es **USD 981 400 ficticios** (`1 200 + 8 200 + 20 000 + 45 000 + 900 000 + 7 000`). Al cerrar un turno **MVP**, debe cumplirse:

`Monto aprobado efectivo + monto retenido efectivo + monto devuelto efectivo = USD 981 400`.

Con la secuencia de la demo en **ATAQUE**, el reparto calculado de los datos actuales sería: **USD 21 200 aprobados**, **USD 952 000 retenidos**, **USD 8 200 devueltos**. Dentro de lo aprobado, solo **USD 1 200** son «procesados correctamente» según la regla del ejercicio; los USD 20 000 de `SIM-003` son aprobación indebida y pérdida ficticia. En **DEFENSA activa** con las mismas elecciones, el estado efectivo esperado sería **USD 1 200 aprobados**, **USD 972 000 retenidos** y **USD 8 200 devueltos**. Estos agregados son **cálculos de diseño a partir del catálogo**, no métricas ya mostradas ni verificadas en Unity. Antes de implementación, convertirlos en pruebas automáticas.

`Held` en el MVP no es «pérdida»: es monto inmovilizado para una revisión externa al turno jugable. Si la versión ampliada añade `ReviewPending`, la identidad de conservación necesita una cuarta columna `monto pendiente de revisión` durante el turno, y al cerrar deben existir reglas explícitas para su resolución. Nunca contar simultáneamente el mismo monto como retenido final y pendiente.

## 4. Pantalla final propuesta en el monitor principal

**Resumen superior legible en VR:** `6 EXPEDIENTES`, `5 CORRECTOS / 1 INCORRECTO`, `PÉRDIDA FICTICIA USD …`, `MONTO APROBADO CORRECTAMENTE USD …`. Una segunda línea muestra aprobados, retenidos y devueltos **por estado efectivo**, con rótulo «volumen de órdenes ficticias», no «ganancias».

**Detalle paginado, una o dos órdenes por vista:** identificador; datos mínimos de la señal; fuente y hallazgo del jugador; **decisión original**; **estado efectivo**; `Control intervino / no intervino`; explicación de 1–2 frases. En `SIM-006` la explicación distingue retención prudente y posible falso positivo. Evitar seis tarjetas diminutas en un solo monitor. Si no hay hallazgo, mostrar «sin hallazgo registrado» sin borrar la decisión.

**Comparación ATAQUE/DEFENSA:** conservar las pérdidas de ambos turnos y añadir una etiqueta: «mismas decisiones» solo si se guardan y comparan los seis vectores de elecciones; en cualquier otro caso «diferencia combinada de decisiones y control». Los resultados previos viven hoy solo en memoria. Si se decide persistencia, diseñarla por separado.

La pantalla debe revelar la recomendación educativa y la etiqueta de evaluación **solo tras la decisión**, nunca en la fase INVESTIGAR. Mantener el contexto histórico en una página separada para no mezclar USD ficticios con 2016.

## 5. Implementación futura y pruebas exigidas

**Datos nuevos necesarios:** `ChosenDecision` estable por expediente, `Finding` con `Source`, `Field`, `Claim`, `ObservedValue` y momento de registro; estado de fuente consultada. El diario actual `DeskSessionJournal` guarda `ChosenDecision` y fuentes en memoria, pero es de presentación y no puede ser la única base de una puntuación robusta. Proponer un registro de turno de dominio o un snapshot inmutable al decidir, sin modificar el catálogo original.

**Código a tocar solo después de aprobación:** calculador nuevo `InvestigationScore` y pruebas en `Tools/DomainTests/Program.cs`; `GameManager` para captura de hallazgos; `BankDocumentUI.Results()` para métricas y paginación; `DeskInteractionCoordinator` para fuente vs hallazgo; `UIManager`/monitor secundario para evitar duplicados. `BankingDomain.cs` puede permanecer intacto en MVP si el nuevo calculador lee decisiones y estados de un registro confiable; la versión ampliada de revisión sí requiere cambio de dominio. No alterar `SimulatedLoss` ni `Correct` para hacer que una nueva pantalla parezca aprobada.

**Pruebas nuevas:** suma de montos por estado, seis retenciones, seis aprobaciones con/sin control, devolución equivocada, aprobación rescatada con mala puntuación, evidencia repetida sin puntos extra, hallazgo falso sin puntos, teléfono sin efecto en estado, comparación con decisiones iguales/desiguales y ausencia de fugas de respuesta antes de decidir. Luego probar lectura, paginación y raycasts en Editor y HONOR. La fórmula se acepta solo si un usuario nuevo entiende por qué obtuvo sus puntos.
