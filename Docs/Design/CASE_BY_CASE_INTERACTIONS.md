# Investigación propuesta para SIM-001 a SIM-006

**Diseño para aprobación, sin cambios de datos.** Los nombres, cuentas, montos y flags proceden de `Assets/_Project/Resources/TrainingScenario.asset`. Cada propuesta distingue: **orden inicial** (lo que el jugador recibe), **fuente a consultar** (lo que descubre), **hallazgo** (lo que decide registrar) y **decisión**. El sistema evalúa después; no escribe la respuesta en el expediente. [Bucle y alcance](GAMEPLAY_REDESIGN_PROPOSAL.md) · [métricas](SCORING_REDESIGN.md) · [controles](CYBERSECURITY_GAMEPLAY_MATRIX.md).

**Regla general del MVP:** las tres decisiones permanecen seleccionables; «coherente» designa la respuesta que la rúbrica actual considera correcta, no un botón obligatorio. Una mala elección avanza el turno y se explica al final. RETENER deja el caso detenido para revisión **fuera** del turno jugable; no se inventa una segunda resolución. En DEFENSA, la corrección automática solo actúa si CONTROLES estaba activo y la elección original fue Aprobar sobre un caso con anomalía. La puntuación propuesta separa 10 puntos de decisión y 5 de hallazgo sustentado por caso; aún no está implementada.

## SIM-001 — Taller Nube Azul · USD 1 200

**Orden inicial:** banco emisor, beneficiario, cuenta `FIC-SIM-001`, monto, intermediario y fecha. No mostrar «operación segura», «sin fraude» ni la respuesta. Una ausencia de aviso no es una garantía.

**Investigar y evaluar:** buscar la cuenta en REGISTRO, leer el titular y cotejarlo con el beneficiario. Usar la hoja de requisitos para comprobar que los campos están presentes. El hallazgo defendible es «los datos consultables coinciden; no encontré discrepancia en estas fuentes». La consulta de entidades, si se incorpora, solo confirma que el nombre **consta**, no que todas sus operaciones sean legítimas.

**Decidir y observar:** Aprobar es coherente. En ATAQUE y DEFENSA activa queda Aprobada porque no hay anomalía precargada. Retener sería una retención innecesaria; Devolver, una devolución incorrecta. Ninguna de esas decisiones genera pérdida ficticia. La pantalla final debe explicar el costo de bloquear sin fundamento, sin convertir USD 1 200 retenidos en dinero perdido. **Puntuación propuesta:** 10 por Aprobar, hasta 5 por la comparación documentada. El monto aprobado correctamente sería USD 1 200. En la ampliación, este caso puede servir como control de referencia antes de introducir variaciones legítimas.

## SIM-002 — Cooperativa Loma Clara · USD 8 200

**Orden inicial:** todos los campos originales, con banco intermediario **sin informar**. El vacío puede verse; lo que no debe aparecer de entrada es «Devolver por formato». Ocultar el campo vacío sería engañoso: el trabajo consiste en detectar que es obligatorio según la referencia.

**Investigar y evaluar:** consultar la hoja de requisitos obligatorios, regresar al expediente y registrar «intermediario ausente». No hace falta afirmar fraude. Si existe un catálogo de entidades, la presencia de la cooperativa no repara el campo que falta.

**Decidir y observar:** Devolver es coherente. Retener deja detenido un caso que necesita corrección de instrucción, por lo que se contabiliza como **retención en lugar de devolución**. Aprobar en ATAQUE lo deja Aprobado, pero no suma pérdida ficticia porque el caso no tiene esa etiqueta. En DEFENSA activa, un Aprobar termina Devuelta y marca intervención; la elección original sigue siendo incorrecta. **Puntuación propuesta:** 10 por Devolver, hasta 5 por identificar ausencia con la política; un Aprobar rescatado recibe 0 de decisión. En la ampliación, solo una nueva orden corregida y autorada permitiría jugar un reenvío real; no inventarla desde el dato actual.

## SIM-003 — Fundacion Horizonte de Papell · USD 20 000

**Orden inicial:** «Papell», cuenta `FIC-SIM-003` y demás campos, sin aviso que diga «retener». El nombre del registro **no** debe mostrarse en el expediente inicial ni en el monitor secundario.

**Investigar y evaluar:** buscar la cuenta. REGISTRO devuelve «Fundacion Horizonte de Papel». Comparar visualmente una letra y registrar «beneficiario difiere del titular» con REGISTRO como fuente. En DEFENSA se puede usar el teléfono de prueba, leer que la fuente ficticia comunica el nombre registrado y pulsar REGISTRAR. Ese acto agrega otra constancia, pero no autentica el número o interlocutor y no debe otorgar la respuesta ni puntos extra automáticos.

**Decidir y observar:** Retener es coherente para revisión. Aprobar en ATAQUE deja la orden Aprobada y suma **USD 20 000 ficticios** a la pérdida. En DEFENSA con CONTROLES activos, el mismo Aprobar termina Retenida por intervención; la puntuación de decisión permanece incorrecta. Devolver sería incorrecto porque el problema modelado es discrepancia a investigar, no un campo de formato ausente. **Puntuación propuesta:** 10 por Retener y 5 por el contraste de nombres; la llamada es opcional para que ATAQUE y DEFENSA puedan alcanzar la misma puntuación investigativa básica. La demo causal repite Aprobar en ambos modos para mostrar el cambio de estado, dejando claro que es un error provocado.

## SIM-004 — Servicios Isla Inventada · USD 45 000

**Orden inicial:** cuenta `FIC-SIM-004` y demás datos; no escribir «cuenta no encontrada» antes de la consulta. La interfaz puede invitar a verificar la cuenta sin preseleccionarla en REGISTRO.

**Investigar y evaluar:** seleccionar/buscar `FIC-SIM-004` y recibir «no encontrada en este registro ficticio». Registrar el resultado con la cuenta consultada. La ausencia de entrada no identifica por sí sola una intención fraudulenta ni demuestra que la cuenta no exista en el mundo real.

**Decidir y observar:** Retener es coherente. Aprobar en ATAQUE queda Aprobada y suma **USD 45 000 ficticios**. Con DEFENSA activa, la aprobación termina Retenida; la elección original sigue siendo errónea. Devolver no corresponde a la regla vigente. **Puntuación propuesta:** 10 por Retener y 5 por búsqueda/resultados documentados. En la ampliación, el jugador podría consultar otra referencia autorada para resolver la ausencia; no se deduce un resultado de la etiqueta oculta.

## SIM-005 — Comercio Brisa Ficticia · USD 900 000

**Orden inicial:** el monto alto y los demás campos de `FIC-SIM-005`. No mostrar de entrada «monto fuera del perfil: revisar». Un número grande por sí solo no prueba que esté fuera de una política.

**Investigar y evaluar en el MVP:** abrir ALERTAS para descubrir que existe una señal de monto inusual. El catálogo actual solo tiene el flag `UnusualAmount`; **no tiene umbral, historial ni perfil numérico**. Por eso el hallazgo honesto es «la fuente de alertas marca el monto como inusual; no hay rango numérico para verificarlo». Una futura hoja de política deberá tener un valor ficticio explícito aprobado y probado antes de permitir una comparación aritmética. No fingir una política a partir del importe de esta única orden.

**Decidir y observar:** Retener es coherente para revisar. Aprobar en ATAQUE queda Aprobada y suma **USD 900 000 ficticios**; DEFENSA activa cambia esa aprobación a Retenida. Devolver no es la opción prevista por la regla. **Puntuación propuesta:** 10 por Retener, 5 por registrar correctamente la fuente y su limitación; ningún punto por repetir «900 000 es mucho» sin referencia. La ampliación puede incorporar una política autorada que obligue a comparar, manteniendo el caso original y revisando la rúbrica.

## SIM-006 — Asociacion Faro de Tinta · USD 7 000

**Orden inicial:** cuenta `FIC-SIM-006` y demás campos, sin rótulo de fraude ni de «seguro». El aviso de coincidencia parcial se consulta en ALERTAS; debe describir el grado de coincidencia sin ordenar un botón.

**Investigar y evaluar:** abrir el detalle de la señal y registrar «coincidencia parcial; no es concluyente». El jugador puede revisar REGISTRO y datos de la orden para añadir contexto. El diseño no debe afirmar que la alerta sea falsa solo porque el caso no lleva etiqueta interna de pérdida: esa etiqueta es un recurso de autor, no un informe investigativo accesible durante el turno.

**Decidir y observar:** Retener para revisar es la recomendación actual. Aprobar en ATAQUE queda Aprobada y baja la puntuación de decisión, pero **no suma pérdida ficticia**; DEFENSA activa cambia esa aprobación a Retenida. Esto permite discutir posible falso positivo y exceso de retención sin llamar fraudulenta a la orden. Devolver es incorrecto: no hay defecto formal modelado. **Puntuación propuesta:** 10 por Retener, 5 por registrar que la coincidencia es parcial/no concluyente, sin afirmar «fraude confirmado». **Versión ampliada:** solo si se crea una respuesta de segunda etapa autorada, RETENER podría abrir una revisión que concluya en aprobar o mantener la retención. No resolverlo automáticamente por `SimulatedFraud=false`.

## Qué debe ver el profesor en la prueba

1. Abrir `SIM-003` **no** revela el nombre del registro ni el botón sugerido.
2. Buscar la cuenta muestra un dato; el hallazgo aparece solo cuando el jugador lo formula.
3. El teléfono, si se usa, registra una consulta separada de la defensa automática.
4. Una aprobación errónea deja distinta huella en ATAQUE y DEFENSA activa, pero **la misma mala puntuación de decisión**.
5. Retener `SIM-006` no se traduce en pérdida monetaria ficticia ni en fraude confirmado.

Antes de implementar, validar los cuatro documentos como un conjunto. La primera entrega debería usar los datos actuales, una UI investigativa y métricas paralelas; la resolución posterior de retenidos y las nuevas políticas necesitan autoría y pruebas separadas.
