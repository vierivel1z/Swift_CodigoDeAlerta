# SWIFT: Código de Alerta — explicación maestra de ciberseguridad

**Para estudiar y presentar, 11 de noviembre de 2026.** El juego es una simulación educativa: los seis expedientes, bancos, cuentas y pérdidas jugables son ficticios. El incidente de Bangladesh Bank sirve de contexto, no de libreto de esas seis transferencias. Los detalles verificables del juego proceden del código y de `TrainingScenario.asset`; los hechos históricos llevan fuentes externas. [Guion oral](SWIFT_GUION_ORAL.md) · [Demostración](SWIFT_DEMO_PRACTICA.md) · [Preguntas](SWIFT_PREGUNTAS_PROFESOR.md) · [Glosario](SWIFT_GLOSARIO.md) · [Diapositivas](SWIFT_PRESENTACION_ESTRUCTURA.md).

## 1. Historia y funcionamiento bancario desde cero

**SWIFT** es una red y un lenguaje de mensajería financiera entre instituciones. Un mensaje puede comunicar una instrucción de pago; el movimiento de dinero ocurre mediante cuentas y sistemas de los bancos participantes. SWIFT no es, por sí mismo, la cuenta donde está depositado el dinero. Esta distinción evita decir que el juego «transfiere por SWIFT» o que una alerta de nuestra pantalla es una validación real de la red. [Qué es SWIFT, explicación de la propia organización](https://www.swift.com/about-us/who-we-are/what-swift).

Un **banco corresponsal** ofrece cuentas y servicios de pago a otro banco, a menudo para que este pueda operar en otro país o moneda. Por ejemplo, el banco de un ordenante puede necesitar que otro banco con cuenta en el país de destino participe en la cadena. Los mensajes comunican instrucciones; los bancos debitan, acreditan, retienen o solicitan aclaraciones según sus responsabilidades y relaciones. La cadena concreta varía de una transferencia a otra. En nuestro juego el jugador trabaja en un banco corresponsal **ficticio** y examina órdenes que llegan para su revisión. Esto es una simplificación pedagógica, no el organigrama ni el procedimiento exacto de ningún banco de 2016. [Definición del Banco de Pagos Internacionales](https://www.bis.org/committees/cpmi/payment), [explicación de la cadena de pagos](https://www.bis.org/publications/innovations-payments).

En febrero de 2016, atacantes obtuvieron acceso no autorizado al entorno informático de **Bangladesh Bank** relacionado con la emisión de mensajes. Según la documentación judicial estadounidense, generaron instrucciones que parecían salir de los sistemas del banco y solicitaron transferencias desde su cuenta en la Reserva Federal de Nueva York. El intento rondó **USD 951 millones**; unos **USD 81 millones** llegaron a cuentas en Filipinas y una transferencia de **USD 20 millones** dirigida a Sri Lanka fue detenida por el banco receptor, sin llegar al destinatario previsto. Son cifras históricas aproximadas de las fuentes citadas, **no** las pérdidas calculadas por el videojuego. [Departamento de Justicia de EE. UU., acusación](https://www.justice.gov/usao-cdca/press-release/file/1367721/dl?inline=), [denuncia judicial, párrafos 144–146](https://www.justice.gov/d9/press-releases/attachments/2018/09/06/2018_09_06_park_complaint_unsealed_0.pdf).

SWIFT declaró en 2016 que sus servicios centrales de mensajería y su red no habían sido comprometidos; situó el compromiso en el entorno local de los clientes donde se originaban o recibían las instrucciones. Después anunció un programa de seguridad de clientes para reforzar ese ecosistema. Esto **no** significa que nuestra revisión simulada reproduzca los controles históricos de SWIFT o del banco receptor. [Declaración de SWIFT de mayo de 2016](https://www.swift.com/news-events/press-releases/gottfried-leibbrandt-cyber-security-and-innovation), [programa anunciado en 2016](https://www.swift.com/news-events/press-releases/swift-launches-customer-security-programme-reinforce-security-global-banking-system).

La lección aplicable al juego es limitada y concreta: si una instrucción aparentemente válida contiene un dato incompatible o inusual, la revisión y una barrera adicional pueden impedir que una mala aprobación se convierta en un resultado efectivo. El proyecto **no simula la intrusión**, las credenciales robadas, los mensajes auténticos de SWIFT ni el procedimiento histórico de los bancos. El resumen interno del material docente menciona 36 órdenes y un desglose particular; como aquí no se localizó la fuente primaria de ese recuento exacto, conviene citarlo solo como «resumen del material docente» y usar para los hechos confirmados las fuentes anteriores.

## 2. Qué problema educativo plantea el juego

El jugador es un **analista** en ambos modos. Recibe seis órdenes ficticias, lee cada expediente, puede contrastar un registro y alertas, y elige **Aprobar**, **Retener** o **Devolver**. «ATAQUE» nombra el **escenario vulnerable** en el que una orden problemática puede pasar si el analista se equivoca; no convierte al jugador en hacker. «DEFENSA» repite el mismo trabajo con la posibilidad de activar una corrección reforzada. El aprendizaje principal es separar cuatro hechos: **una señal aparece**, **el analista la consulta**, **el analista decide** y **el sistema puede intervenir**. Son cosas distintas.

La vulnerabilidad simulada es una **aprobación manual de una orden con anomalía sin intervención automática reforzada**. No es un exploit de SWIFT. Las alertas ya existen en ATAQUE; el fallo posible consiste en que ver una alerta no modifica por sí solo el estado de la orden. El juego ilustra cómo una barrera adicional puede reducir el daño ficticio de un error humano. También enseña que bloquear todo indiscriminadamente crea revisiones innecesarias: `SIM-006` tiene una coincidencia parcial, pero no está etiquetada como fraude simulado.

El recorrido implementado es portal → prólogo omitible → seis expedientes → resultados. El tutorial de mirada y el portapapeles ayudan a practicar el procedimiento. No hay cuatro niveles independientes ni una infraestructura bancaria conectada. El prototipo ha pasado pruebas del dominio y del Editor, pero la lectura y la interacción en el HONOR con Cardboard aún requieren una prueba física de la versión actual.

## 3. ATAQUE explicado como una jornada del analista

En ATAQUE, el jugador no pierde el acceso al expediente, REGISTRO, ALERTAS, portapapeles o evidencia. Estas herramientas son **detectivas y documentales**: revelan datos para que una persona actúe. La diferencia programada es que el botón de CONTROLES no activa la revisión reforzada en este modo. Por eso una alerta puede verse en pantalla y, aun así, el jugador puede elegir Aprobar. Una señal no es una orden automática de bloqueo ni una demostración de fraude.

**Aprobar** deja la transferencia ficticia como aprobada. Si el expediente también lleva una etiqueta interna de fraude simulado, su monto se suma a la pérdida del ejercicio. **Retener** deja la orden detenida para revisión; no equivale a demostrar un delito. **Devolver** la rechaza por un problema de formato y la envía de vuelta en la lógica didáctica; el ejemplo claro es la falta de banco intermediario en `SIM-002`. En ATAQUE, la decisión elegida y el estado final coinciden porque no existe la corrección reforzada. La puntuación mide si la elección coincide con la regla educativa; no mide destreza técnica de intrusión.

Ejemplo: `SIM-003` solicita USD 20 000 para «Fundacion Horizonte de Papell», mientras el titular del registro ficticio figura como «Fundacion Horizonte de Papel». El analista puede consultar ambos nombres y decidir **Retener**, que es la opción coherente. Si elige **Aprobar** en ATAQUE, la orden queda aprobada y esos USD 20 000 se suman a la pérdida ficticia porque el caso está marcado internamente para ese cálculo. La discrepancia visible justifica revisar; por sí sola no prueba fraude real.

## 4. DEFENSA y el control que sí cambia el resultado

DEFENSA usa los mismos expedientes y decisiones. El jugador debe activar **CONTROLES** una vez durante el turno. La elección de diseño hace visible la diferencia entre «tener una función disponible» y «ponerla en operación». Si no la activa, DEFENSA se comporta como ATAQUE respecto a la corrección automática. El código no agrega un motor de inteligencia artificial: lee las anomalías precargadas de cada caso.

Con CONTROLES activos, si el analista pulsa **Aprobar** en una orden con cualquier anomalía, el sistema sustituye el estado final: **Devuelta** cuando falta el intermediario; **Retenida** para las otras anomalías. Marca `DefenseOverride`, cuyo significado simple es «la decisión del analista fue reemplazada por el control». Si la orden no tiene anomalías, Aprobar sigue siendo Aprobar. Si el analista ya elige Retener o Devolver, ese control no sustituye esa elección, incluso si otra acción habría sido la recomendada. La regla usa `Anomalies` —datos visibles precargados— y **no** la etiqueta interna `SimulatedFraud`, el teléfono, una llamada externa o una inferencia de IA.

Una **alerta detectiva** informa; una **retención** detiene temporalmente una orden para revisar; una **devolución** responde a un defecto formal en esta simulación; una **intervención correctiva** transforma una aprobación errónea en uno de esos estados. Es razonable llamar «preventivo respecto de la aprobación efectiva» al efecto de esta barrera, pero no llamarlo autenticación, doble aprobación o prevención de intrusión: esas funciones no existen aquí.

En el resultado `SIM-003` puede figurar «Elegiste: APROBAR», «Estado final: RETENIDA» y «Control: intervino». El contador de decisiones coherentes sigue evaluando **APROBAR**, así que el jugador pierde ese acierto aunque el sistema evite la pérdida ficticia. Esto es intencional: el sistema y la persona tienen responsabilidades distintas.

## 5. Comparación precisa de los modos

| Función | ATAQUE | DEFENSA | Debilidad / control y consecuencia | Evidencia en el juego |
| --- | --- | --- | --- | --- |
| Expediente, REGISTRO y ALERTAS | Disponibles | Disponibles | **Iguales:** información para detectar discrepancias; consultar no bloquea por sí solo | Pestañas, titular y alertas de `SIM-003`. |
| Portapapeles, evidencia, impresora y decisiones | Disponibles | Disponibles | **Iguales:** procedimiento y constancia; la marca manual no valida | Tres hojas, `MARCAR REVISIÓN`, estado de evidencia. |
| CONTROLES | No activa la revisión reforzada | Se puede activar una vez por turno; inicia inactivo | Diferencia central: una barrera adicional ante aprobaciones con anomalía | Mensaje «Controles activos» y estado de defensa en resultados. |
| Aprobar sin anomalías | Aprobada | Aprobada aun con control activo | Evita bloquear sistemáticamente el caso limpio | `SIM-001` en ambas partidas. |
| Aprobar con intermediario faltante | Aprobada | Devuelta **si el control estaba activo** | Defecto de formato; el sistema corrige una aprobación | `SIM-002`, estado y marca de intervención. |
| Aprobar con otra anomalía | Aprobada | Retenida **si el control estaba activo** | Riesgo observable que requiere revisión | `SIM-003` a `SIM-006`, elección frente a estado final. |
| Teléfono de `SIM-003` | No disponible | Consulta ficticia contextual | Fuente adicional para contraste y registro; **no** bloquea ni activa controles | Transcripción `GUION DE PRUEBA` y «Consulta registrada». |
| Puntuación y pérdida | Puntúa la elección; suma montos etiquetados que quedaron aprobados | Puntúa igual; la intervención puede bajar la pérdida efectiva | Distingue error humano de resultado con barrera | Tarjetas 5/6, pérdida y fila con `Control: intervino`. |

La tabla muestra que DEFENSA incorpora **una sola diferencia bancaria decisiva: la corrección activable de aprobaciones anómalas**. El teléfono es una herramienta adicional de consulta en un caso, pero no es la causa del cambio de estado.

## 6. Teléfono y portapapeles: qué hacen exactamente

En un procedimiento real, una verificación por canal independiente sería útil porque una instrucción o un número incluidos en una comunicación comprometida podrían llevar al mismo actor que originó la solicitud. Habría que escoger un contacto ya conocido y verificar a la persona y la autorización por un proceso aprobado. **El prototipo no implementa esas garantías.** Su teléfono está disponible solo en DEFENSA y `SIM-003`, usa un guion ficticio en «Mesa de registros» y muestra el nombre que figura en el registro. El jugador solicita, responde, lee y pulsa **REGISTRAR**. La entrada queda asociada a la orden en la evidencia y el diario de la sesión. El texto advierte que no confirma autorización; no entrega la orden «Aprueba/Retén/Devuelve». No modifica el estado, los aciertos, la pérdida ni `Defense.Active`. El guion está etiquetado como prueba y requiere aprobación editorial.

**Frase para presentarlo:** «Esta llamada es una consulta simulada a otra fuente para que el jugador compare nombres y registre qué escuchó. Hoy no verificamos técnicamente el número ni la identidad; el bloqueo que verán después se debe al botón CONTROLES, no a la llamada».

El portapapeles de madera contiene **tres hojas**. La primera resume cuatro pasos: leer emisor, beneficiario y cuenta; comprobar monto e intermediario; contrastar registro y alertas; documentar y decidir. La segunda reproduce datos del expediente, incluida la fecha si existe. La tercera reúne las observaciones, las fuentes abiertas y si se registró una llamada. En cada hoja se puede **MARCAR REVISIÓN**, una marca manual que puede quitarse. Sirve para practicar un procedimiento y recordar qué se consultó, pero **no certifica** que el dato sea correcto, que la fuente sea auténtica ni que haya fraude. Tampoco aplica la regla de DEFENSA. El monitor secundario ofrece apoyo resumido; el portapapeles permite una lectura cercana y secuencial. La interfaz de revisión posterior separa fuentes abiertas, marcas, consulta, elección y estado final.

## 7. Los seis expedientes, uno por uno

Todos proceden del mismo banco emisor ficticio, llevan fecha simulada `05/02/2016`, cuenta `FIC-SIM-00X` y, salvo `SIM-002`, intermediario ficticio. El dato particular importante se describe abajo. «Si se aprueba incorrectamente» se refiere a la regla del ejercicio; no es una sentencia de fraude.

### SIM-001 — Taller Nube Azul, USD 1 200

La cuenta `FIC-SIM-001`, el titular registrado y el beneficiario coinciden; no hay anomalías precargadas. El analista comprueba esa correspondencia y **Aprobar** es la elección coherente. En ATAQUE queda aprobada. En DEFENSA activa también: no hay motivo programado para sustituirla. Si el jugador la retiene o devuelve, pierde un acierto sin reducir ninguna pérdida ficticia. Enseña que un control proporcionado debe permitir casos sin señales. En la exposición muestre expediente, REGISTRO y el resultado Aprobada en ambos modos.

### SIM-002 — Cooperativa Loma Clara, USD 8 200

La cuenta `FIC-SIM-002` presenta el **banco intermediario vacío**. El analista comprueba ese campo y **Devolver** es la elección coherente por formato. Si pulsa Aprobar en ATAQUE, el estado queda Aprobada y pierde un acierto; no aumenta la pérdida ficticia porque este caso no tiene esa etiqueta interna. En DEFENSA con CONTROLES activos, el mismo Aprobar termina **Devuelta** con intervención marcada. Enseña que falta de dato obligatorio y fraude son conceptos diferentes. En la exposición señale el campo vacío y el resultado devuelto por el control.

### SIM-003 — Fundacion Horizonte de Papell, USD 20 000

La cuenta `FIC-SIM-003` tiene en el expediente «Papell» y en el registro ficticio «Papel». El analista contrasta nombres; en DEFENSA puede consultar el teléfono de prueba y registrar lo que comunica. **Retener** es coherente para revisar. Si elige Aprobar en ATAQUE, queda Aprobada, pierde un acierto y suma USD 20 000 de pérdida ficticia. En DEFENSA activa, la misma elección termina **Retenida**, el acierto sigue perdido y la pérdida de este caso es cero. Es la mejor evidencia de que una fuente consultada, la decisión humana y la barrera automática son tres etapas distintas. Muestre REGISTRO, transcripción y fila «Elegiste APROBAR / RETENIDA / Control intervino».

### SIM-004 — Servicios Isla Inventada, USD 45 000

La cuenta `FIC-SIM-004` **no consta en el registro ficticio**. El analista consulta REGISTRO y **Retener** es coherente. Aprobar en ATAQUE la deja Aprobada y suma USD 45 000 de pérdida ficticia. Con defensa activa, un Aprobar acaba Retenida y el jugador no recibe el acierto. La lección es contrastar referencias y no aprobar una orden que requiere averiguación. «No encontrada» no basta para declarar fraude en el mundo real. Muestre el mensaje del registro y la intervención en resultados.

### SIM-005 — Comercio Brisa Ficticia, USD 900 000

La cuenta `FIC-SIM-005` lleva una señal precargada de **monto inusual**; no existe un algoritmo estadístico de perfil. El analista lee importe y alerta; **Retener** es coherente. Aprobar en ATAQUE suma USD 900 000 ficticios y pierde un acierto. Con defensa activa, esa aprobación termina Retenida. Enseña revisión proporcional ante una operación fuera del perfil simulado y el impacto potencial de ignorar una señal de alto monto. Muestre monto, alerta y pérdida acumulada; no lo describa como IA antifraude.

### SIM-006 — Asociacion Faro de Tinta, USD 7 000

La cuenta `FIC-SIM-006` tiene una **coincidencia parcial de alerta**. La recomendación didáctica es **Retener para revisar**, sin afirmar que sea fraude. Aprobar en ATAQUE pierde un acierto, pero no suma pérdida ficticia porque el caso no tiene esa etiqueta interna. La defensa activa retiene ese Aprobar de todos modos. Enseña el riesgo de **falsos positivos**: una señal puede exigir atención sin representar un fraude confirmado. Muestre la advertencia «no confirma fraude» y la diferencia entre alerta y etiqueta interna de pérdida.

## 8. Cómo se calculan puntuación y resultados

Para cada expediente, el sistema guarda si la **elección original** coincidió con la recomendación educativa (`CorrectDecision`). Esa recomendación es Aprobar si no hay anomalía; Devolver si falta intermediario; Retener para las demás señales. Luego asigna el **estado efectivo**: la elección del jugador, salvo que DEFENSA activa sustituya un Aprobar anómalo. `DefenseOverride` registra precisamente esa sustitución. `Correct` cuenta las elecciones coherentes, no las órdenes salvadas por el control. `SimulatedLoss` suma los montos de los expedientes con etiqueta interna de fraude simulado cuyo **estado efectivo** terminó Aprobado. Son variables del ejercicio, no valores bancarios reales.

Ejemplo comprobado con las mismas seis elecciones en los dos modos: `Aprobar, Devolver, Aprobar, Retener, Retener, Retener`. En ATAQUE el resultado es **5/6** y **USD 20 000 ficticios**; en DEFENSA con CONTROLES activos es **5/6** y **USD 0**. El analista se equivocó en `SIM-003` en ambos; solo el segundo turno corrigió su aprobación. Una prueba del dominio en `Tools/DomainTests/Program.cs` comprueba ese caso. Otro caso verificado: **Aprobar las seis** produce **1/6** y **USD 965 000** en ATAQUE o DEFENSA sin activar controles; en DEFENSA activa produce **1/6** y **USD 0**. La defensa no convierte malas elecciones en buenas respuestas.

La comparación que aparece en la pantalla muestra el último resultado de ATAQUE y el último de DEFENSA durante la ejecución actual. Si las decisiones fueron distintas, la diferencia de pérdidas **no puede atribuirse solo al control**. Para explicarlo causalmente se repiten las mismas seis elecciones y se cambia únicamente la activación de CONTROLES. Tras reiniciar la aplicación no hay garantía de conservar esa comparación.

## 9. Qué enseñar al profesor y qué falta

La demostración ideal usa `SIM-003`: «La discrepancia existía en ambos modos; yo decidí Aprobar en ambos; solo cuando activé CONTROLES el estado final pasó a Retenida. La llamada aportó información, pero no tomó la decisión». El recorrido detallado, incluida una opción completa y otra abreviada con video real previamente grabado, está en [SWIFT_DEMO_PRACTICA.md](SWIFT_DEMO_PRACTICA.md).

**Implementado y comprobado en código/Editor:** seis casos, tres decisiones, consulta, alertas, evidencia, control activable, elección frente a estado final y pérdidas; pruebas de dominio y sondas de Unity documentadas. **Simulado:** bancos, cuentas, anomalías, pérdidas, llamada y procedimiento. **Parcial:** guion telefónico por aprobar, audio con tonos temporales, explicación causal que aún debe probarse con alumnos. **Pendiente:** APK con todos los cambios locales actuales, instalación y recorrido en HONOR, lectura binocular y rendimiento, fuentes sonoras finales, ensayo cronometrado y requisitos particulares del profesor (`Funcional`, `Técnico`, `Roles`, `Defensa`, `Vulnerabilidad`), que no constan completos en el árbol consultado. No hay integración con SWIFT, autenticación telefónica, motor antifraude dinámico, MFA, doble aprobación ni manos/mandos Quest verificados. Estas últimas son posibles mejoras de alcance, no componentes ya entregados.

La VR tiene sentido si hace tangible el trabajo: mirar un expediente, consultar otra fuente y observar en un puesto el cambio entre decisión e intervención. Para defender su valor académico no basta que la oficina se vea bien: un estudiante debe poder explicar **qué señal vio, qué decidió, qué hizo el control y por qué cambió el resultado**. Esa comprensión necesita una prueba breve con usuarios nuevos y una validación física en el visor.

## Fuentes y trazabilidad

**Fuentes históricas primarias:** [SWIFT: qué es su red](https://www.swift.com/about-us/who-we-are/what-swift); [BIS: banco corresponsal](https://www.bis.org/committees/cpmi/payment); [Departamento de Justicia: intento y transferencias de 2016](https://www.justice.gov/usao-cdca/press-release/file/1367721/dl?inline=); [denuncia judicial: intrusión, mensajes, Filipinas y Sri Lanka](https://www.justice.gov/d9/press-releases/attachments/2018/09/06/2018_09_06_park_complaint_unsealed_0.pdf); [SWIFT: alcance del compromiso y respuesta](https://www.swift.com/news-events/press-releases/gottfried-leibbrandt-cyber-security-and-innovation).

**Fuentes del proyecto:** `Assets/_Project/Resources/TrainingScenario.asset`; `Assets/_Project/Scripts/Banking/BankingDomain.cs`; `Core/GameManager.cs`; `DeskTools/DeskReviewState.cs` y `DeskInteractionCoordinator.cs`; `UI/BankDocumentUI.cs`; `Tools/DomainTests/Program.cs`. Contraste documental: [funcionalidades auditadas](../Context/ATAQUE_DEFENSA_FUNCIONALIDADES.md), [matriz de controles](../Research/CYBERSECURITY_MODE_COMPARISON.md), [demo académica](../Research/ACADEMIC_DEMO_FLOW.md), [progreso y pruebas](../Context/IMMERSIVE_AUTONOMOUS_PROGRESS.md), [Kanban](../Context/KANBAN.md). Este paquete de presentación solo creó documentación; no repitió las pruebas ni validó hardware.
