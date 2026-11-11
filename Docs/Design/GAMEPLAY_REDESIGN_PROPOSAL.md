# Propuesta de rediseño: investigar → evaluar → decidir

**Para aprobación, 11/11/2026. Sin cambios de implementación.** Esta propuesta conserva `BancoCorresponsal`, los seis expedientes de `TrainingScenario.asset`, Google Cardboard, la selección por mirada y las tres decisiones actuales mientras se valida un bucle más activo. No modifica datos, reglas, scripts ni puntuación. Documentos complementarios: [matriz de controles](CYBERSECURITY_GAMEPLAY_MATRIX.md), [puntuación](SCORING_REDESIGN.md) y [seis casos](CASE_BY_CASE_INTERACTIONS.md).

## 1. Diagnóstico contrastado con el código

| Hallazgo actual | Evidencia concreta | Efecto en la experiencia |
| --- | --- | --- |
| El expediente entrega el resumen de la anomalía antes de consultar | `BankDocumentUI.Dossier()` coloca `FormatReferencePanel.ObservedAlerts(data)` en OBSERVACIONES; `FormatReferencePanel.Refresh()` también lo envía al monitor de apoyo | El jugador puede leer la conclusión y elegir sin investigar. |
| Parte del texto aún sugiere la acción | `TrainingData.Alerts()` contiene frases como «devolver por formato», «retener y verificar» y «revisar». `ObservationText()` quita algunas, pero no todas; `UIManager.Refresh()` todavía usa la cadena original | Una vista heredada o un aviso puede revelar la respuesta. La pista debe describir un hecho, no el botón correcto. |
| REGISTRO resuelve la comparación al abrir la pestaña | `BankDocumentUI.Registry()` muestra cuenta, beneficiario, titular y estado según el caso; `GameManager.InspectRegistry()` consulta directamente el expediente actual | No hay búsqueda manual ni posibilidad de elegir la cuenta equivocada y corregirse. |
| ALERTAS se calculan con datos precargados y ya incluyen «Formato» o «Revisar» | `BankDocumentUI.Alerts()`, `TrainingData.Alerts()` | No hay tarea de clasificar la señal. Debe quedar claro que una coincidencia parcial no certifica fraude. |
| El portapapeles repite expediente y alertas | `DeskInteractionCoordinator.RefreshView()`: hojas de procedimiento, datos y observaciones; `MARCAR REVISIÓN` solo alterna un booleano | Sirve como apoyo de lectura, pero todavía no obliga a contrastar ni demuestra que se verificó un dato. |
| EVIDENCIA registra texto generado por el sistema | `GameManager.RegisterEvidence()` toma `TrainingData.Alerts(current.Data)` sin exigir un hallazgo del jugador | Premia un clic potencialmente vacío; no es evidencia de investigación. El teléfono sí requiere una secuencia propia para registrar. |
| Teléfono de DEFENSA es contextual, pero no cambia la regla | Solo aparece en `SIM-003`; `DeskSessionJournal.RecordPhone()` guarda evidencia. `TransactionManager.Decide()` no lee esa evidencia | Es útil para contraste, pero puede confundirse con la causa de la retención automática. |
| RETENER es terminal en el turno | `TransactionManager.Decide()` cambia inmediatamente el estado a `Held`; `Current` avanza al siguiente `Pending`; `EndScenario()` solo exige que no queden `Pending` | El juego no representa investigación posterior a una retención. No debe insinuar que el jugador la resolvió. |
| El control de DEFENSA corrige toda aprobación anómala | Con `Defense.Active`, cualquier `Approve` con `Anomalies != None` se sustituye por la recomendación; también `SIM-006`, que no tiene etiqueta interna de fraude | La barrera es visible, pero puede parecer un detector infalible si no se explica el posible falso positivo. |
| La métrica económica es incompleta | `Correct` cuenta elecciones coherentes; `SimulatedLoss` suma tres casos etiquetados si acabaron aprobados. Resultados no muestran flujo por estado | No diferencia montos aprobados correctamente, retenidos o devueltos; cero pérdida puede ocultar exceso de retención. |

**Matiz:** retener los seis expedientes **no da 6/6** con la regla vigente: `SIM-001` requiere Aprobar y `SIM-002` Devolver, así que obtiene **4/6**. Aun así, arroja cero pérdida ficticia y puede parecer una estrategia cómoda. El nuevo diseño debe hacer visible su costo operativo y la falta de investigación sin convertir retenciones en «pérdidas».

## 2. Objetivo de diseño y principios

El jugador debe producir una explicación verificable: **«consulté esta fuente; encontré este dato; lo comparé con esta instrucción; por eso decidí así»**. El sistema nunca debe mostrar `Recommended(data)` ni `SimulatedFraud` antes de cerrar el caso. Las anomalías siguen siendo datos de autor para evaluar, pero el jugador ve primero campos y fuentes, no un veredicto.

**INVESTIGAR.** El expediente muestra la orden recibida, sin resumen automático de inconsistencias. REGISTRO exige seleccionar o introducir la cuenta en una interfaz VR de botones grandes; la consulta devuelve solo el registro asociado o «no encontrado». Una hoja de política muestra requisitos y rangos de referencia de la **simulación** sin decir qué caso incumple. La consulta de entidades reconocidas responde «consta/no consta» y muestra información descriptiva, **nunca «APROBADO» ni «transferencia legítima»**. En DEFENSA, `SIM-003` puede añadir la llamada ficticia.

Para el MVP en Cardboard, REGISTRO puede usar un **teclado de tres dígitos grandes** para el sufijo de `FIC-SIM-___`, con `BORRAR` y `BUSCAR`, sin escritura libre. El jugador copia los dígitos de la orden; una búsqueda errónea devuelve el resultado de esa otra clave y puede repetirse. No presentar una lista que marque de antemano qué cuentas existen. El catálogo de entidades y cualquier política numérica necesitan una fuente autorada aparte; si todavía no están aprobados, sus pestañas deben indicar «referencia no disponible en esta versión» y no fingir una comprobación.

**EVALUAR.** El jugador registra uno o dos hallazgos estructurados por caso: fuente, campo y observación (`coincide`, `difiere`, `no consta`, `dato ausente`, `fuera de política`, `coincidencia parcial`, `no pude confirmar`). El botón solo guarda lo que **el jugador eligió**; no llena el contenido desde `TrainingData.Alerts`. Abrir una fuente se registra por separado de formular un hallazgo. Las opciones son neutrales y grandes para Cardboard, sin teclado obligatorio. Una revisión posterior explica si la conclusión estaba respaldada por los datos.

**DECIDIR.** Aprobar, Retener y Devolver permanecen como acciones independientes. Una confirmación breve muestra la justificación registrada o «sin hallazgo registrado», sin revelar la respuesta antes de confirmar. En DEFENSA, CONTROLES sigue aplicando la intervención actual cuando corresponde; la pantalla posterior separa elección, intervención y resultado. La mirada de 1,5 s no cambia.

La recomendación de reducir texto y enseñar una acción a la vez se apoya en las [prácticas de onboarding VR de Meta](https://developers.meta.com/vr/design/hands-onboarding-best-practices/). Es una **adaptación de diseño** a Cardboard, no una prueba de que esa interfaz ya funciona en HONOR. La separación de observación, análisis y respuesta también es coherente con el tratamiento de anomalías y falsos positivos en el [marco CSF de NIST](https://csrc.nist.gov/projects/cybersecurity-framework/filters); el juego no afirma conformidad con ese marco.

## 3. Portapapeles propuesto

La versión mínima usa **cuatro vistas paginadas** con el mismo objeto físico y botones de mirada grandes, en vez de añadir cuatro objetos 3D:

1. **Entidades de referencia:** nombre y tipo de entidad presentes en un catálogo ficticio. «Consta en la referencia de entrenamiento» solo indica existencia, no aprobación de todas sus órdenes. La política de relación con entidades no se infiere del nombre del beneficiario.
2. **Requisitos obligatorios:** campos que una instrucción debe traer, incluida la presencia de intermediario en el ejercicio. Muestra la regla general; no resalta automáticamente `SIM-002`.
3. **Política de montos y alertas:** un umbral o rango **autoría explícita** para el caso de monto, acompañado de la advertencia «superar un rango requiere revisión, no demuestra fraude». Hoy solo existe un flag `UnusualAmount`, sin umbral; no inventar uno durante la implementación sin aprobarlo y agregar fixtures de prueba. Para el MVP puede mostrarse «consultar política: dato aún no modelado» y conservar `SIM-005` como alerta manual hasta aprobar el rango.
4. **Registro manual de comprobaciones:** muestra los hallazgos que eligió el jugador y la fuente; una marca «revisé» sigue siendo distinta de «coincide». Permite corregir antes de decidir.

El monitor secundario puede mostrar **estado de investigación** (fuentes consultadas y hallazgos pendientes), mientras el portapapeles contiene la referencia y el formulario. El monitor principal conserva expediente y decisiones. Así cada superficie tiene una función y no duplica el mismo párrafo. Una vista de entidad o política no debe revelar la recomendación calculada.

## 4. Semántica de Aprobar, Retener y Devolver

| Acción | Semántica para el MVP | Consecuencia actual que se conserva | Posible ampliación, aún no aprobada |
| --- | --- | --- | --- |
| **Aprobar** | Permitir que la orden avance dentro de la simulación | `Approved`, salvo intervención activa de DEFENSA | Confirmación explícita de hallazgos relevantes antes de autorizar. |
| **Retener** | Detenerla para investigación **fuera del turno jugable actual** | `Held` terminal para este ejercicio; el siguiente expediente aparece | Estado `ReviewPending` no terminal, nuevas consultas y resolución posterior. |
| **Devolver** | Retornar una instrucción que requiere corrección formal | `Returned` terminal | Modelar una versión corregida y nuevo envío solo si se escriben casos nuevos. |

No recomiendo introducir revisión posterior a Retener en el MVP: los seis casos actuales no contienen una segunda respuesta autorada ni datos para liberar una orden retenida. Hacerla no terminal exigiría decidir qué pasa con `Current`, `Complete`, `EndScenario`, el resultado y las métricas. En la versión ampliada, `ReviewPending` sería una cola separada de expedientes nuevos; cada caso requeriría evidencia de resolución y una acción final, con pruebas de que no se puede cerrar el turno mientras queden revisiones abiertas. `SIM-006` sería el primer piloto para enseñar que una coincidencia parcial puede terminar en aprobación **solo después de** una aclaración autorada. No se debe derivar esa aclaración automáticamente de la etiqueta oculta `SimulatedFraud`.

## 5. Diferencia de seguridad sin quitar trabajo al jugador

**ATAQUE:** mismas herramientas básicas para leer, buscar y registrar; el analista debe descubrir la discrepancia y decidir. No dispone de la corrección reforzada. Si aprueba una orden problemática, esa elección puede quedar como estado efectivo Aprobada.

**DEFENSA:** mantiene lo anterior y permite activar CONTROLES. El teléfono ficticio de `SIM-003` aporta otra fuente, pero no certifica identidad ni autorización. El control automático existente cambia **solo** una aprobación con anomalía; no puntúa los hallazgos, no escribe la justificación del jugador y no activa la llamada. La interfaz debe señalar cuándo el control intervino y cuándo solo se abrió una fuente. En `SIM-006`, la retención automática debe explicarse como revisión potencialmente innecesaria, no como fraude descubierto.

**Ampliación opcional:** controles con cobertura imperfecta, casos de falso negativo o aprobación dual. Requerirían nuevos datos de autor y evaluación docente; no es correcto afirmar que están presentes hoy. Mantener el mismo catálogo y la misma secuencia de decisiones entre modos para una comparación causal clara.

### Limpieza de rótulos sin ocultar la ficción

Mostrar una banda institucional persistente y legible en portal, estación y resultados: **«ENTRENAMIENTO / DATOS Y RESULTADOS FICTICIOS»**. En la capa de presentación, abreviar «REGISTRO FICTICIO» a «REGISTRO», quitar el sufijo repetido `(ficticio)` de los nombres de bancos y `(simulada)` de la fecha visible. **No editar `TrainingScenario.asset` ni `TrainingData.Create()` para este ajuste.** La página de contexto y los resultados deben repetir la aclaración completa antes de mostrar cifras. Eliminar solo esos sufijos conocidos mediante un formateador específico; no hacer reemplazos globales que cambien el contenido de una observación o atribuyan autenticidad a los datos.

## 6. Dos alcances para aprobar

### MVP propuesto — viable sobre los seis datos actuales

- Reordenar revelación: expediente con **hechos**, observaciones sin respuesta automática, REGISTRO por búsqueda manual y apoyo secundario sin copia de la alerta. Mantener una vía accesible para encontrar cada pista, incluida la coincidencia parcial.
- Registrar hallazgos estructurados antes de decidir. No exigir un hallazgo para permitir decidir; una decisión a ciegas sigue siendo posible y se evalúa después. El feedback de corrección llega **después** de la decisión.
- Portapapeles con entidades/requisitos, política explícita solo cuando exista un dato aprobado, y hoja de hallazgos. El texto de simulación se mueve a una barra institucional visible en portal, estación y resultados.
- Conservar `TransactionStatus`, `Recommended`, `SimulatedLoss` y `DefenseOverride` como base comparativa. Añadir métricas y desglose **en paralelo**, sin reemplazar la puntuación actual hasta que se apruebe su interpretación.
- Resultados paginados en el monitor principal: resumen de seis casos, decisión original, estado efectivo, fuentes/hallazgos y explicación posterior. Retenido significa retenido para revisión, sin simular una resolución inexistente.

### Versión ampliada — requiere diseño y datos nuevos

- Catálogo independiente de entidades, cuentas y políticas con distractores controlados; consultas manuales con errores recuperables. No copiar `RegisteredName` a una «fuente independiente» que solo repita el mismo campo sin justificar su procedencia.
- Resultado de llamada autenticada modelada: seleccionar contacto desde un directorio autorado, comprobar identidad y registrar respuesta; sin conexión real ni promesa de autenticación bancaria. Separa este control del `DefenseOverride` existente.
- Cola de revisión posterior a RETENER con estados no terminales, nuevas evidencias y resolución. Diseñar especialmente `SIM-006` para que se pueda aclarar un falso positivo; no resolver por leer la etiqueta interna de fraude.
- Balancear dificultad, puntuación y tutorial con 3–5 usuarios nuevos y un recorrido completo en HONOR antes de ampliar más casos.

## 7. Impacto técnico exacto cuando se autorice implementar

| Archivo o componente | MVP propuesto | Versión ampliada |
| --- | --- | --- |
| `Assets/_Project/Scripts/UI/BankDocumentUI.cs` | Separar hechos iniciales, búsqueda, hallazgos, confirmación y resultados paginados; conservar colliders/dwell | Cola de revisión y vista de resolución. |
| `Assets/_Project/Scripts/UI/FormatReferencePanel.cs` y `UIManager.cs` | Quitar pistas duplicadas y texto prescriptivo de **todas** las proyecciones, incluida la heredada | Ayuda contextual ligada a la etapa. |
| `Assets/_Project/Scripts/Core/GameManager.cs` | Exponer comandos `SearchAccount`, `RecordFinding`, `InspectPolicy` y observaciones de consulta sin adelantar respuesta | Orquestar revisiones abiertas y cierres. |
| **Nuevo** `InvestigationObservationFormatter` (nombre propuesto) | Proyectar hechos neutrales desde los flags a la UI. Sustituir en `GameManager.InspectAlerts()` y en `RegisterEvidence()` el uso directo de `TrainingData.Alerts()`, que hoy contiene recomendaciones; esta última acción debe registrar un hallazgo elegido o quedar deshabilitada sin él | Redactar nuevos tipos de observación solo después de aprobar los casos. |
| `Assets/_Project/Scripts/DeskTools/DeskInteractionCoordinator.cs`, `DeskReviewState.cs`, `DeskToolsProfile.cs` | Reorganizar cuatro páginas, registrar fuente frente a hallazgo y conservar teléfono actual separado | Directorio/contacto verificado ficticio y cola posterior. |
| **Nuevos** `InvestigationCatalog` y `InvestigationJournal` (nombres propuestos) | Referencias y hallazgos por expediente, sin modificar `TrainingScenario.asset`; documentar su fuente de verdad | Datos adicionales de entidades/políticas y resolución de revisiones. |
| `Assets/_Project/Scripts/Banking/BankingDomain.cs` | **Sin cambio en la primera fase**; solo tras aprobación, añadir métricas separadas si se necesita en dominio | Añadir `ReviewPending`, transición de resolución, `Current/Complete` y reglas de cierre. Requiere migración de pruebas. |
| `TrainingScenario.asset` | **Intacto** | Nuevos campos solo con diseño y autorización específicos; mejor asset aparte para referencias. |
| `BankInteractable.cs`, `DocumentFocus.cs`, `VRInteractionManager.cs` | Añadir acciones o targets sin nuevo temporizador; mantener una activación por mirada | Nuevas acciones de resolución con las mismas garantías. |

**Estados de flujo propuestos:** `Investigating` al abrir cada expediente; `Evaluating` cuando el jugador formula o edita un hallazgo; `DecisionConfirmation` tras seleccionar Aprobar/Retener/Devolver; `Decided` al confirmar; `Results` tras el sexto caso. Volver de `Evaluating` a `Investigating` no borra consultas, y cancelar `DecisionConfirmation` no avanza ni puntúa. Los estados describen la UI y el diario; no reemplazan todavía `TransactionStatus.Pending/Approved/Held/Returned`. En la ampliación, `ReviewPending` sería un estado **del dominio**, no solo una pantalla.

**Pruebas nuevas necesarias:** dominio (`Tools/DomainTests/Program.cs`) para decisiones y métricas; `Tools/DeskToolsTests/Program.cs` para búsqueda, fuente vs hallazgo, llamada y cambios de expediente; `GameplayFlowProbe`, `BankOfficeProbe`, `ImmersivePlayProbe` para seis casos, raycast, desbordes, resultados y regreso al portal. Añadir tests de **no filtración de respuestas** en UI inicial/secundario/portapapeles y de no acreditación por abrir una fuente. Repetir regresión de ATAQUE/DEFENSA con decisiones idénticas y pérdidas actuales (20 000 frente a 0 en el caso demostrado). Después, probar versión Android en HONOR: legibilidad binocular, tiempos de búsqueda y fatiga; el Editor no certifica eso.

## 8. Decisiones que requieren aprobación antes del código

1. **MVP:** ¿se acepta conservar RETENER como estado terminal del turno y añadir investigación manual, hallazgos y métricas paralelas?
2. **Política de monto:** ¿se autoriza un umbral ficticio explícito para `SIM-005`, o se mantiene la alerta precargada hasta redactar esa política?
3. **Puntuación:** ¿se aprueba el desglose y la fórmula propuesta en [SCORING_REDESIGN.md](SCORING_REDESIGN.md) como evaluación nueva, manteniendo `Correct` y `SimulatedLoss` visibles durante la transición?
4. **Ampliación futura:** ¿se quiere una revisión pendiente resoluble con datos de segunda etapa? Esto cambia el contrato del dominio y requiere autoría de nueva evidencia.

La propuesta usa como referencia de organización de información el proceso descrito por el equipo de [Demeo](https://www.resolutiongames.com/blog/demeo-one-year-anniversary-a-look-at-the-evolving-uxui-of-demeo): definir qué necesita saber el jugador en cada momento y probarlo. Su aplicación aquí es una inferencia de diseño, no una función copiada del juego.
