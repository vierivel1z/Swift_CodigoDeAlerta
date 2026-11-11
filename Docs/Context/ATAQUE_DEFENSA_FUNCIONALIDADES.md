# Modos ATAQUE y DEFENSA: funciones, límites y aprendizaje de ciberseguridad

**Estado auditado:** 11 de noviembre de 2026. Proyecto `SWIFT_CodigoDeAlerta`, escena `BancoCorresponsal`, Unity 6000.3.24f1. Este inventario contrasta el código local con la documentación y los recibos de pruebas existentes. No se modificó la lógica del juego ni se ejecutó una prueba nueva para redactarlo. Los seis expedientes, cuentas, importes y pérdidas del juego son **ficticios**; el jugador representa a un **analista bancario en ambos modos**, no a un atacante que controla un sistema real.

**Lectura rápida:** en ATAQUE el analista decide con la información disponible, sin corrección automática reforzada. En DEFENSA puede **activar** ese control; una aprobación de una orden con anomalía se convierte en retención o devolución según la regla existente. Entrar en DEFENSA por sí solo no activa el control. La comparación de pérdidas solo aísla el efecto de la defensa si se repiten las mismas decisiones en ambos turnos.

## 1. Flujo común ya implementado

Portal ATAQUE/DEFENSA por mirada → prólogo breve que se puede omitir → estación y seis expedientes → resultados → posibilidad de iniciar el otro modo. La selección usa el dwell existente de 1,5 s; también hay control de prueba con ratón en PC. Un expediente se resuelve con **Aprobar, Retener o Devolver**. La sexta decisión cierra el turno automáticamente; `CERRAR TURNO` no permite finalizar con órdenes pendientes.

| Función común | Para qué sirve en el juego | Qué demuestra en ciberseguridad | Estado y límite |
| --- | --- | --- | --- |
| Expediente y documento físico consultable | Leer emisor, beneficiario, cuenta, monto, intermediario, fecha y observaciones disponibles | Revisar datos de una transacción antes de autorizarla | Implementado; los datos son precargados y ficticios. El documento físico es de solo lectura. |
| REGISTRO | Comparar cuenta y titular con el expediente; `SIM-004` indica cuenta no encontrada | Contrastar una solicitud con otra fuente interna | Implementado como registro simulado; no consulta una base externa ni verifica identidad real. |
| ALERTAS | Mostrar anomalías de formato, nombre, cuenta, monto o coincidencia | Distinguir indicio de confirmación: una alerta pide revisión | Implementado sobre flags de caso, no mediante un detector antifraude en vivo. Una coincidencia parcial no prueba fraude. |
| Portapapeles de tres hojas | Revisar formato, datos y observaciones; marcar manualmente cada hoja | Practicar un procedimiento y distinguir lectura de constancia de revisión | Implementado; `MARCAR REVISIÓN` no valida datos ni cambia la decisión bancaria. |
| EVIDENCIA e impresora | Registrar una observación del expediente y dar feedback de interacción | Dejar constancia de lo observado | Implementado por turno; evita repetir la misma entrada textual, pero no es una auditoría legal ni persistente. |
| Tres decisiones | Resolver cada orden de forma explícita | Relacionar evidencia, decisión humana y consecuencia | Implementado; la recomendación educativa es: sin anomalías, Aprobar; falta de intermediario, Devolver; otras anomalías, Retener. |
| Resultados y revisión posterior | Ver aciertos, pérdida ficticia, elección original, estado efectivo, intervención del control, fuentes y llamada registrada | Separar lo que eligió el analista de lo que hizo el sistema | Implementado en Editor; la legibilidad binocular en el teléfono está pendiente. |
| Teletransporte, asiento, retícula y tutorial | Llegar a la estación y usar sus controles por mirada | Facilitar una práctica accesible de revisión en VR | Implementado y probado en Editor; falta comprobar comodidad y raycasts en Cardboard físico. |

## 2. Modo ATAQUE

**Propósito:** exponer qué ocurre cuando una decisión manual equivocada no recibe la corrección reforzada. El modo no simula penetración, robo de credenciales ni manipulación técnica de SWIFT. La vulnerabilidad didáctica es **aprobar una orden con anomalías pese a señales disponibles**.

| Función | Comportamiento actual | Qué demuestra |
| --- | --- | --- |
| Selección de ATAQUE | Abre una sesión nueva con los mismos seis casos de entrenamiento | Permite establecer la línea de comparación. |
| Consulta de fuentes | Expediente, REGISTRO, ALERTAS, portapapeles y evidencia siguen disponibles | La información detectiva puede existir y aun así no impedir una mala autorización. |
| CONTROLES | `ApplyDefense` no se activa en este modo | La ausencia de intervención automática es la diferencia programada respecto a DEFENSA. |
| Aprobar una orden con anomalía | La elección queda como estado efectivo Aprobada | Muestra el riesgo de depender solo de la decisión manual. La pérdida ficticia aparece únicamente si ese caso tiene una etiqueta interna de fraude simulado. |
| Puntuación y resultado | Se puntúa la elección del jugador y se suma la pérdida ficticia de casos etiquetados que terminaron Aprobados | Permite discutir error humano y resultado sin presentar la cifra como daño bancario real. |

**Caso demostrable:** con las seis decisiones `Aprobar, Devolver, Aprobar, Retener, Retener, Retener`, el expediente `SIM-003` queda aprobado pese al nombre discrepante; el resultado documentado es **5/6 decisiones coherentes y USD 20 000 de pérdida ficticia**. Aprobar las seis órdenes produce **USD 965 000 ficticios** con los datos actuales. Estas cifras dependen de esas elecciones y del conjunto de casos vigente.

## 3. Modo DEFENSA

**Propósito:** repetir el ejercicio y mostrar el efecto de un control reforzado activado explícitamente. El control actúa sobre **anomalías observables**, no sobre la etiqueta oculta `SimulatedFraud`.

| Función | Comportamiento actual | Qué demuestra y qué no demuestra |
| --- | --- | --- |
| Activar CONTROLES | Disponible una vez por turno solo en DEFENSA; deja `Defense.Active = true` | Una protección debe habilitarse para intervenir. Entrar al modo sin activarla deja la decisión manual sin corrección. |
| Interceptar Aprobar con anomalía | Si falta intermediario, el estado efectivo pasa a **Devuelta**; con otras anomalías, a **Retenida**. Se marca `DefenseOverride`. | Simula una barrera ante aprobaciones riesgosas. No prueba que la orden sea fraudulenta ni equivale a un control bancario real. |
| Mantener elección original | La puntuación `Correct` evalúa la elección del analista aunque el sistema la cambie | Un buen resultado económico no significa que el jugador haya decidido bien. |
| Teléfono contextual `SIM-003` | Secuencia ficticia Solicitar → Responder → leer → Registrar, solo en DEFENSA y ese expediente | Aporta una fuente de contraste y una entrada de evidencia. El guion sigue pendiente de aprobación; no realiza una llamada real, no verifica un número independiente y **no activa ni gobierna** el control reforzado. |
| Resultados comparables | Muestra el estado del control, elección, estado efectivo y pérdida de cada modo si ambos se jugaron sin cerrar la app | Permite explicar una intervención concreta. `LastAttackLoss` y `LastDefenseLoss` viven en memoria; no hay persistencia entre reinicios. |

**Caso demostrable:** repetir exactamente `Aprobar, Devolver, Aprobar, Retener, Retener, Retener` y activar CONTROLES antes de `SIM-003` mantiene **5/6** aciertos, pero cambia el estado efectivo de esa orden a Retenida y deja **USD 0 de pérdida ficticia**. La llamada opcional no causa ese cambio. Si el jugador ya retiene correctamente los casos riesgosos, la pérdida puede ser cero en ambos modos; no hay que prometer siempre una diferencia.

## 4. Qué enseña cada expediente

| Caso | Señal existente | Acción recomendada por la simulación | Lección | Límite importante |
| --- | --- | --- | --- | --- |
| `SIM-001` | Ninguna anomalía precargada | Aprobar | No todo expediente necesita bloqueo; un control debe permitir operaciones normales. | La ausencia de flags no certifica legitimidad real. |
| `SIM-002` | Falta banco intermediario | Devolver | Un defecto de formato se trata de manera diferente a una sospecha de fraude. | No tiene etiqueta interna de pérdida ficticia. |
| `SIM-003` | Beneficiario distinto del titular registrado | Retener | Contrastar nombres y separar consulta, registro, decisión e intervención. | El teléfono es un guion ficticio; no confirma autorización. |
| `SIM-004` | Cuenta no encontrada en el registro ficticio | Retener | Verificar referencias antes de aprobar. | “No encontrada” no demuestra fraude por sí sola. |
| `SIM-005` | Monto inusual según flag precargado | Retener | Revisar operaciones fuera de perfil. | No existe un modelo estadístico ni umbral dinámico de perfil. |
| `SIM-006` | Coincidencia parcial de alerta | Retener para revisar | Una alerta puede afectar una orden sin etiqueta de fraude; importa el contexto y el posible falso positivo. | Retener es revisión, no declaración de culpabilidad. |

En el código, `SIM-003`, `SIM-004` y `SIM-005` tienen una etiqueta interna de fraude **solo para calcular la pérdida ficticia** cuando terminan aprobados. La defensa no lee esa etiqueta para decidir. `SIM-006` muestra por qué no se debe presentar toda alerta como fraude confirmado.

## 5. Funcionalidades que faltan o necesitan mejora

| Prioridad | Estado | Mejora viable | Para qué sirve / qué demostraría | Criterio de aceptación propuesto |
| --- | --- | --- | --- | --- |
| P0 | **No probado en dispositivo** | Recorrer ambos modos en HONOR/Cardboard con la APK que incluya los cambios actuales | Demostrar que la experiencia funciona en el medio de entrega, no solo en Editor | Entrada por portal, prólogo, seis decisiones, consulta, evidencia, control, teléfono, resultados, retícula y teletransportes legibles y alcanzables con ambos ojos; registrar fallos y rendimiento. |
| P0 | **Pendiente** | Generar e identificar una APK nueva después de los cambios locales posteriores al build de desarrollo del 10/11 | Evitar probar una versión anterior al estado actual | Log de Unity, hash, contenido de escena y versión registrados; conservar APK previa. |
| P1 | **Parcial** | Revisar la explicación causal de resultados y las hojas del portapapeles con estudiantes | Que puedan decir qué dato provino de cada fuente, qué eligió el analista y qué cambió el control | Piloto de 3–5 personas; distinguir elección y estado efectivo sin ayuda oral. Mantener ocultas las respuestas durante la partida. |
| P1 | **Parcial** | Aprobar o ajustar el guion telefónico `SIM-003` | Enseñar una consulta a otra fuente sin sugerir que el prototipo autentica al interlocutor | Redacción revisada; interfaz indica su alcance ficticio. Si se quiere enseñar un canal independiente, modelar selección de contacto verificado como mecánica separada. |
| P1 | **Pendiente de especificación docente** | Incorporar los requisitos particulares Funcional/Técnico/Roles/Defensa/Vulnerabilidad cuando se reciban | Alinear la demostración académica con los criterios reales | Matriz de requisito → función → prueba, sin inventar controles ausentes. |
| P2 | **Parcial** | Refinar la devolución posterior por expediente con una frase causal breve | Explicar por qué se retuvo/devolvió y evitar que el jugador confunda alerta con fraude | Una explicación legible para cada caso, validada en visor; ningún cálculo nuevo ni respuesta anticipada. |
| P2 | **Parcial** | Integrar audio final y medir volumen en el visor | Distinguir confirmación, acción no disponible, alerta, consulta, defensa y final de turno | Clips con licencia, mezcla cómoda, sin repetición por cada refresco; prueba auditiva Android. Actualmente hay cues y tonos temporales, no una banda sonora final. |
| P2 | **No probado en dispositivo** | Medir FPS, temperatura, lectura binocular y fatiga durante un turno completo de cada modo | Garantizar que la simulación puede usarse en una clase | Mediciones en HONOR y corrección de cuellos observados, sin cambiar Cardboard/URP a ciegas. |
| P3 | **Propuesta, fuera del alcance actual** | Simular un control preventivo concreto, como segunda aprobación o contacto por canal previamente validado | Comparar un control preventivo con las alertas detectivas y la corrección actual | Diseño docente aprobado, regla explícita y pruebas nuevas. No presentarlo como ya implementado. |
| P3 | **Propuesta, fuera del alcance actual** | Variar casos o añadir más ramas solo después de validar la comprensión de los seis actuales | Evitar memorizar respuestas y explorar falsos positivos | Casos escritos y revisados, con medición de comprensión; preservar comparabilidad Ataque/Defensa. |

## 6. Límites que conviene declarar al presentar el juego

- Es una **simulación educativa**, no una implementación del protocolo SWIFT, un sistema bancario, una auditoría de seguridad ni una reconstrucción de órdenes históricas individuales.
- El modo ATAQUE muestra una **situación vulnerable**; el jugador no realiza phishing, explotación, intrusión ni robo de credenciales.
- No están implementados MFA, cifrado, segregación de funciones, verificación técnica de identidad, motor antifraude dinámico ni llamadas externas. Si se incorporan en el futuro, deben diseñarse y probarse como funciones nuevas.
- El diario de fuentes y marcas manuales existe durante la sesión actual. Abrir una fuente, marcar una hoja, registrar una llamada y validar un dato son hechos distintos; la aplicación no certifica que el usuario haya leído o comprobado realmente el contenido.
- Los resultados de Unity Editor y las capturas monoscópicas no certifican comodidad, legibilidad ni interacción en el HONOR. Tampoco hay una variante Quest con manos o mandos verificada.

## 7. Evidencia consultada

- Código: `Assets/_Project/Scripts/Banking/BankingDomain.cs`, `Assets/_Project/Scripts/Core/GameManager.cs`, `Assets/_Project/Scripts/UI/BankDocumentUI.cs`, `Assets/_Project/Scripts/DeskTools/DeskInteractionCoordinator.cs`, `Assets/_Project/Scripts/Audio/FeedbackAudio.cs`.
- Comparación y guion anteriores: [CYBERSECURITY_MODE_COMPARISON.md](../Research/CYBERSECURITY_MODE_COMPARISON.md) y [ACADEMIC_DEMO_FLOW.md](../Research/ACADEMIC_DEMO_FLOW.md).
- Pruebas previas: DomainTests 37/37 y DeskToolsTests 37/37 registrados en `Artifacts/CybersecurityReview/`; sondas `flow`, `office`, `smoke`, `phone`, `clipboard` y `regression` en Unity Editor documentadas en la comparación. Tras el último cambio de tipografía de la pantalla de espera, `welcome-after`, `flow`, `office`, `smoke`, `tutorial` y `closeui` pasaron en el Editor, con recibos en `Artifacts/ImmersiveGameplay/`.
- Estado Android: existe una APK **Development anterior**, documentada en [IMMERSIVE_AUTONOMOUS_PROGRESS.md](IMMERSIVE_AUTONOMOUS_PROGRESS.md). No representa necesariamente todos los cambios locales posteriores y no hay prueba física HONOR registrada.
