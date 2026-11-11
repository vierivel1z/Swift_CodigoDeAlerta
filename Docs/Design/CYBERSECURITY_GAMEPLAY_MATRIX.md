# Matriz de ciberseguridad y acciones del jugador

**Propuesta, sin implementación.** Compara lo que ocurre hoy con el bucle [INVESTIGAR → EVALUAR → DECIDIR](GAMEPLAY_REDESIGN_PROPOSAL.md). Los casos y montos son ficticios. «Anomalía», «fraude simulado» y «fraude confirmado» no son sinónimos. La clasificación de controles toma como referencia conceptual el análisis de eventos y falsos positivos del [NIST CSF](https://csrc.nist.gov/projects/cybersecurity-framework/filters); esta simulación no declara conformidad con un estándar bancario.

## 1. Matriz funcional

| Riesgo o pregunta | ATAQUE hoy | DEFENSA hoy | Acción investigativa propuesta | Control representado y evidencia visible | Límite didáctico |
| --- | --- | --- | --- | --- | --- |
| ¿La cuenta y el titular coinciden? | REGISTRO entrega el dato del expediente actual al abrirse | Igual | Buscar `FIC-SIM-00X`, leer titular, comparar con beneficiario y registrar «coincide/difiere/no consta» | **Detectivo:** fuente de registro; mostrar consulta y hallazgo elegido, no respuesta preescrita | Registro local ficticio, no base bancaria conectada. |
| ¿Falta un campo obligatorio? | `SIM-002` muestra intermediario vacío y alerta | Igual; Aprobar puede acabar Devuelta si CONTROLES está activo | Consultar requisitos en portapapeles, revisar campo y registrar «ausente» | **Detectivo + corrección simulada:** documento, checklist, elección frente a estado | Falta de dato no prueba fraude. |
| ¿El monto requiere revisión? | `SIM-005` tiene flag y texto de «fuera de perfil», sin umbral visible | Igual; control activo retiene un Aprobar | Comparar monto con una política ficticia **autorada**; si no existe, tratar el flag como alerta sin inventar umbral | **Detectivo:** referencia de política, valor consultado y hallazgo | No hay modelo de comportamiento ni IA. |
| ¿Una coincidencia de alerta confirma fraude? | `SIM-006` muestra coincidencia parcial | Igual; control activo también retiene un Aprobar | Consultar detalle, registrar «coincidencia parcial / requiere contexto» | **Detectivo:** señal y necesidad de revisión; mostrar posible falso positivo al final | La etiqueta interna de fraude es `false`; el sistema no dispone de una aclaración posterior jugable. |
| ¿Qué aporta una llamada? | No disponible para `SIM-003` | Guion ficticio disponible en `SIM-003`; se puede registrar | Comparar lo que dice la fuente con el expediente y anotar alcance de la respuesta | **Consulta adicional:** transcripción y registro explícito | No hay directorio validado, autenticación ni impacto automático en estado. |
| ¿Puede una aprobación errónea prosperar? | Sí, queda Aprobada | Con CONTROLES activos, Aprobar anómalo termina Retenida o Devuelta | Mantener decisión humana explícita y mostrar luego si hubo intervención | **Corrección reforzada simulada:** `DefenseOverride`, estado efectivo y puntuación original | No detecta fraude verdadero; actúa sobre flags precargados. |
| ¿La investigación se documentó? | EVIDENCIA copia texto de alertas; portapapeles marca hojas | Igual, más llamada en `SIM-003` | Hallazgo estructurado elegido por el jugador, ligado a fuente y campo, editable antes de decidir | **Trazabilidad educativa:** fuente abierta vs afirmación registrada vs elección | Abrir, mirar o marcar no certifica que se verificó la información. |
| ¿Retener todo evita el problema? | Da cero pérdida ficticia pero solo 4/6 decisiones coherentes | Igual; la corrección automática no mejora una elección de Retener | Mostrar retenciones innecesarias, monto inmovilizado y explicación posterior | **Control de calidad de decisión:** desgloses separados, sin llamar «pérdida» a la retención | No hay costo económico de retención modelado. |

## 2. Secuencia que se debe poder demostrar

| Fase | Acción del jugador | Registro propuesto | Qué puede hacer el sistema | Qué no debe hacer |
| --- | --- | --- | --- | --- |
| INVESTIGAR | Abrir expediente; buscar cuenta; consultar entidad, requisitos y política; en DEFENSA `SIM-003` llamar | Registrar **fuente abierta** y resultado de consulta por separado | Entregar datos específicos de la fuente solicitada | Mostrar «Retener», «fraude» o `Recommended` en la pista inicial. |
| EVALUAR | Elegir observación y asociarla con fuente/campo | Guardar **hallazgo del jugador**, no la alerta autogenerada | Validar después si la observación tiene respaldo; permitir rectificar antes de decidir | Conceder puntos solo por abrir panel o marcar hoja. |
| DECIDIR | Aprobar, Retener o Devolver y confirmar | Conservar elección y hallazgos al cerrar el caso | En DEFENSA activa, mantener la regla actual de sustitución de Aprobar anómalo; calcular resultado | Usar la llamada para bloquear o dar por correcta una decisión. |
| REVISAR | Abrir resultados y explicación | Mostrar elección, estado, intervención y hallazgos | Explicar causalmente cada caso, incluyendo `SIM-006` | Tratar retención como pérdida o alerta como fraude confirmado. |

## 3. Dos maneras de comparar los modos

**Comparación causal de seguridad:** jugar ambos turnos con las mismas seis decisiones y cambiar únicamente la activación de CONTROLES. Ejemplo ya comprobado: `Aprobar, Devolver, Aprobar, Retener, Retener, Retener` → 5/6 en ambos; USD 20 000 ficticios en ATAQUE y USD 0 en DEFENSA activa. La llamada puede enseñarse como fuente adicional, pero **no** es la causa de esa diferencia. Ver [prueba existente](../Research/ACADEMIC_DEMO_FLOW.md).

**Comparación de aprendizaje:** permitir que el jugador investigue mejor en el segundo turno y mejore sus elecciones. Si cambian las decisiones, la diferencia de pérdida refleja **decisiones + control**, no solo el control. La pantalla debe decirlo. Esta variante mide aprendizaje potencial, pero requiere un piloto con usuarios para demostrarlo; la app no puede inferir por sí misma que alguien aprendió.

## 4. Estados y riesgos por caso

| Caso | Dato que se busca | Hallazgo defendible | Elección coherente actual | Si se aprueba en ATAQUE | Si se aprueba con DEFENSA activa |
| --- | --- | --- | --- | --- | --- |
| `SIM-001` | Cuenta/titular y campos | Sin discrepancia observada en las fuentes simuladas | Aprobar | Aprobada; sin pérdida ficticia | Aprobada; sin intervención. |
| `SIM-002` | Requisito de intermediario | Campo ausente | Devolver | Aprobada; decisión incorrecta, sin pérdida ficticia | Devuelta por intervención. |
| `SIM-003` | Titular frente a beneficiario | `Papel` ≠ `Papell` | Retener | Aprobada; USD 20 000 ficticios | Retenida por intervención. |
| `SIM-004` | Cuenta `FIC-SIM-004` | No encontrada en registro ficticio | Retener | Aprobada; USD 45 000 ficticios | Retenida por intervención. |
| `SIM-005` | Monto y alerta de perfil | Flag de monto inusual; política numérica aún no autorada | Retener | Aprobada; USD 900 000 ficticios | Retenida por intervención. |
| `SIM-006` | Coincidencia parcial | Señal no concluyente | Retener para revisar | Aprobada; elección incoherente con regla, sin pérdida ficticia | Retenida por intervención; posible falso positivo operativo. |

**Importante:** en el MVP `Held` sigue siendo final **para el turno**, aunque semánticamente represente una revisión pendiente externa. Si se aprueba la versión ampliada de revisión jugable, agregar un estado diferente y nuevas evidencias; no reinterpretar silenciosamente el enum actual.

## 5. Qué cambia y qué permanece en el MVP propuesto

La presentación y el registro de investigación cambian; `TrainingScenario.asset`, la tabla de recomendaciones, las consecuencias de decisiones y la regla `DefenseOverride` permanecen como contrato de comparación inicial. La política de montos, el directorio de entidades y la búsqueda deben ser **fuentes autoradas y probadas**, no una copia del texto de alertas. En particular, una entidad «reconocida» no implica que todas sus órdenes sean seguras. Las mejoras ampliadas (cola posterior a Retener, autenticación telefónica simulada y controles adicionales) son proyectos separados que necesitan diseño, datos y aprobación.
