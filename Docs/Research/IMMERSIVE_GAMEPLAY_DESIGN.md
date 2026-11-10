# SWIFT — Diseño de una ronda bancaria inmersiva

**Fecha:** 8 de noviembre de 2026. **Estado:** propuesta; ninguna mecánica de este documento se implementó durante la investigación.

Base: [investigación y fuentes](VR_GAMEPLAY_RESEARCH.md), [viabilidad](CARDBOARD_FEASIBILITY.md) y [plan técnico](CODEX_IMPLEMENTATION_ROADMAP.md). Etiquetas: **[P]** observado en proyecto, **[F]** fuente externa, **[I]** inferencia, **[R]** recomendación nueva. Salvo donde se indique otra cosa, el diseño de las secciones siguientes es **[R]**.

## 1. Objetivo y límites

El jugador debe sentirse un analista que utiliza herramientas para **leer, contrastar, documentar y decidir**. El monitor sigue siendo la fuente del expediente y la superficie de Aprobar, Retener y Devolver. El portapapeles organiza la revisión; el teléfono añade una declaración parcial. Ninguno decide por el estudiante.

Objetivos observables del piloto:

- Encontrar y usar una referencia sin indicación verbal del facilitador.
- Explicar qué dos datos se compararon y de dónde proceden.
- Diferenciar discrepancia, falta de información y fraude confirmado.
- Completar las mismas decisiones y resultados actuales sin regresiones.
- Consultar herramientas desde el asiento, con mirada, sin inclinarse ni desplazarse físicamente.

No se añaden reglas bancarias, niveles, llamadas reales, reconocimiento de voz, servicios online ni seguimiento de manos a la versión principal. Los casos y diálogos son una **recreación educativa ficticia**, no un protocolo histórico documentado del incidente de 2016.

## 2. Teléfono de verificación

### 2.1 Qué existe y qué aporta

**[P]** No se encontró verificación telefónica en `GameManager`, `BankInteractable`, `ScenarioData` ni en los scripts propios de la búsqueda. Los seis expedientes tienen datos y anomalías, pero no interlocutores, diálogos o estados de llamada. No se identificó un teléfono por los nombres del inventario de modelos. Por tanto, no debe presentarse como una función que solo necesita activarse.

Su valor educativo es enseñar a contrastar una orden con **otra fuente identificada y limitada**. El interlocutor puede confirmar un nombre registrado, declarar que no dispone de información o no responder. Nunca dice «la decisión correcta es Retener» ni conoce el indicador interno `SimulatedFraud`.

### 2.2 Disponibilidad

Disponible únicamente con sesión DEFENSA activa, expediente pendiente con diálogo autorizado, jugador en el puesto, sin prólogo, tutorial ni transición de asiento. Está disponible **independientemente de `Defense.Active`**: consultar no activa el control automático existente. La UI distingue «Consulta telefónica» de «Controles de defensa activados».

No es obligatorio llamar para decidir, terminar un turno o mantener la puntuación vigente. Los casos sin diálogo muestran «Sin consulta telefónica prevista para este expediente», sin un botón que parezca averiado. No penalizar el hecho de no responder a una señal sonora.

**MVP:** una llamada para SIM-003. **Ampliación:** SIM-004 para información inconclusa. SIM-005 puede servir después para enseñar ausencia de respuesta, pero no incluir todos los casos por completar una lista. Las versiones de un caso son deterministas: volver a llamar no debe producir otra verdad.

### 2.3 Flujo recomendado

1. La orden indica que existe una fuente de contraste disponible, sin marcarla como fraudulenta.
2. Mirar el teléfono durante el dwell real abre su ficha; seleccionar «Solicitar contraste» inicia una devolución de llamada simulada.
3. Tras una espera breve, el aparato suena y muestra «Llamada pendiente». El sonido se limita a dos ciclos; el estado visual permanece. Esa espera no es un segundo temporizador de selección.
4. Una fijación completa sobre «Responder» acepta la llamada una sola vez. No se exige continuar mirando el auricular durante la lectura.
5. La ficha presenta interlocutor ficticio, ID del expediente y dos o tres intervenciones. El usuario avanza a su ritmo; voz opcional, subtítulos siempre disponibles.
6. El jugador compara el dato comunicado con el expediente. Puede releer la transcripción sin repetir el sonido de llamada.
7. «Registrar consulta» guarda el resumen factual una vez. «Cerrar» permite volver al monitor sin registrar ni decidir.
8. Aprobar, Retener o Devolver se ejecutan desde los controles actuales, después de cerrar la inspección.

No se debe colocar «Responder», «Siguiente» y «Registrar» sucesivamente bajo una mirada sostenida que active todo por inercia. La nueva etapa requiere apartar la mirada y volver a enfocar, o un objetivo distinto claramente separado. El progreso procede únicamente de `VRInteractionManager`.

### 2.4 Ejemplo de SIM-003: información y responsabilidad

**[P] Datos actuales:** beneficiario «Fundacion Horizonte de Papell»; nombre registrado «Fundacion Horizonte de Papel»; anomalía `NameMismatch`. No cambiar estas cadenas para producir el ejemplo.

**[R] Guion propuesto, todavía no existente:**

| Intervención | Texto sugerido |
|---|---|
| Encabezado | «SIM-003 / Mesa de registros — conversación ficticia» |
| Analista | «Solicito contrastar el nombre asociado a la cuenta del expediente.» |
| Interlocutor | «El nombre que figura en nuestro registro es: {RegisteredName}.» |
| Límite de la fuente | «Esta consulta no confirma la autorización de la transferencia.» |
| Cierre | «Compara el dato con la orden y registra la observación si corresponde.» |

Evidencia factual: «Consulta telefónica ficticia SIM-003; fuente: Mesa de registros; nombre comunicado: Fundacion Horizonte de Papel; la autorización no fue confirmada». La decisión no se inserta en la frase.

Para impedir que sea solo otra pestaña de Registro, la llamada debe enfatizar **procedencia y límites de confirmación**. Si el piloto revela que no cambia la comprensión y solo repite un dato, se mantiene como demostración opcional o se retira; no se justifica por el modelo de teléfono.

### 2.5 Resultados posibles

| Resultado de la consulta | Presentación | Evidencia permitida | Efecto bancario |
|---|---|---|---|
| Dato comunicado distinto de la orden | Dos valores y su procedencia; sin veredicto automático | Declaración exacta y limitación de alcance | Ninguno automático |
| Coincidencia parcial | «Coincide el dato consultado; no confirma toda la operación» | Qué campo y qué fuente coincidieron | No aprobar automáticamente |
| Inconclusa | «La fuente no puede confirmar este dato» | Consulta realizada e información no disponible | No inventar una discrepancia |
| Sin respuesta, resultado escrito del caso | «No se obtuvo respuesta»; relectura o reintento con el mismo resultado | Intento sin confirmación | No inferir fraude |
| Jugador no contesta o cancela | «Consulta pendiente/cancelada» | No registrar una conversación que no ocurrió | Ninguno |

La contradicción se contrasta con las anomalías ya autorizadas. No añadir por diálogo una falta que convierta en incorrecta la recomendación actual del dominio. Nuevas historias requieren una revisión pedagógica aparte.

### 2.6 Evidencia y consecuencias

**[P]** `EvidenceManager` almacena cadenas únicas. `GameManager.RegisterEvidence(string)` las asocia al expediente **actual**; una sesión nueva reutiliza los mismos IDs. El diseño necesita capturar identidad de sesión, expediente y llamada al comenzar, comprobarla al registrar y cancelar al cambiar de contexto. Nunca adjuntar tardíamente la llamada de SIM-003 a SIM-004.

El nuevo registro auxiliar conserva fuente, campo consultado, resultado, texto mostrado y estado de registro. Su deduplicación usa `(sesión, expediente, llamada)`, no un texto variable con la hora. «Revisado» o «consulta registrada» describen una actividad; no constituyen validación del dato.

**[P]** La defensa automática ya puede corregir la resolución efectiva y evitar pérdidas aunque el jugador decida mal. Por ello el teléfono no puede prometer una nueva ventaja económica conservando exactamente los cálculos. **[R]** Su impacto es informativo y educativo: facilita una decisión razonada y deja evidencia; el resultado final diferencia elección, intervención del control y consultas realizadas. La pérdida y los aciertos siguen calculándose igual.

### 2.7 Tres alternativas

| Versión | Ventaja | Coste/riesgo | Decisión |
|---|---|---|---|
| A: teléfono fijo + ficha TMP | Funciona con mirada y conserva referencias espaciales | Diseño del panel, foco y estados | **Entrega principal** |
| B: auricular a pose prefijada | Respuesta más tangible sin manos | Puede tapar texto; necesita retorno y cancelación | Opcional después de A; animar objeto, no cámara |
| C: agarre con mando/manos | Posición controlada por el usuario | Hardware, proveedor XR, oclusión, colisiones y más pruebas | Variante posterior, fuera del MVP Cardboard |

## 3. Portapapeles contextual

### 3.1 Aprovechar lo que existe

**[P]** `EnvironmentRedesign.cs` construye `Analyst review clipboard` con tabla de madera, clip metálico, papel y cuatro pasos TMP. La escena serializada conserva el objeto. Su `BankAction.Notebook` abre la página `Procedure` del monitor. La mejora consiste en darle una consulta propia, no crear otro prop idéntico.

| Diseño | Legibilidad | Inmersión | Complejidad/claridad | Recomendación |
|---|---|---|---|---|
| A: hojas pequeñas totalmente físicas | Baja sin acercamiento; Cardboard no permite inclinarse con 6DoF | Alta como objeto | Puede obligar a movimientos incómodos o texto minúsculo | Solo aspecto del objeto en reposo |
| B: objeto + vista ampliada | Ajustable y comprobable en visor | Conserva origen físico | Media; abrir/consultar/cerrar claros | **Primera implementación** |
| C: secundario + portapapeles de apoyo | Aprovecha panel existente | Menor cambio | Menor riesgo; posible duplicación | Alternativa durante pruebas y reversión |

No retirar el monitor secundario al empezar. Evaluar B frente a C. Si B es legible, fácil de descubrir y no alarga la revisión, convertirlo en referencia principal. El monitor puede quedar como estado breve de terminal, no como una segunda copia de las mismas reglas.

### 3.2 Hojas y contenido

Hasta tres hojas grandes, con pestañas discretas o «Anterior/Siguiente», sin scroll diminuto:

1. **Revisión del expediente:** ID y campos que se deben comprobar; datos disponibles de la orden y «No informado» para ausencia. Una marca manual por grupo, no por cada carácter.
2. **Fuentes consultadas:** registro, alertas y llamada registrada cuando exista. Diferenciar «fuente abierta» de «revisión marcada por el jugador»; abrir una pestaña no demuestra lectura.
3. **Procedimiento:** cuatro pasos actuales: leer, contrastar, consultar alertas, documentar y decidir. Ayuda general, no instrucciones calculadas para acertar el caso.

Los datos cambian por expediente; las marcas se guardan por sesión e ID y comienzan vacías. Al revisar el historial pueden mostrarse como actividad pasada, sin editar una operación resuelta. Un caso nuevo no hereda marcas del anterior.

No llenar la hoja con todos los datos del monitor. Para comparar un nombre, mostrar únicamente ID, campo de la orden y fuente consultada. Evitar tablas con muchas columnas, fondos con casillas impresas o tipografía dentro de una imagen generada.

**[P]** `FormatReferencePanel.ObservedAlerts` ya neutraliza frases que ordenan «retener/devolver». Reutilizar ese formato de observación; llamar directamente a `TrainingData.Alerts` puede volver a revelar la respuesta. Ni `Recommended` ni `SimulatedFraud` deben alimentar estas hojas.

### 3.3 Interacción y composición

Foco → indicador de 1,5 s → apertura → lectura estable → marca/cambio de hoja → cierre. Abrir solo una herramienta a la vez; el monitor queda visible como contexto, pero sus decisiones no reciben activaciones a través del panel.

La vista ampliada se sitúa en un anclaje cómodo del puesto, no en una tarjeta permanentemente pegada a la cámara. La ubicación exacta se ajusta con capturas y visor; no se fija una coordenada mundial sin medir la escena. Recentrar bajo petición si queda fuera de vista. El prop queda sobre la mesa y puede resaltar suavemente mientras su hoja está abierta.

Pasar página puede usar un desplazamiento muy corto o transición de opacidad. El texto debe detenerse antes de leer. No simular papel flexible, arrastre continuo con la cabeza ni físicas de hojas.

## 4. Sentarse y levantarse

**[P]** `AnalystSeat` ya comprueba destino, oscurece en aproximadamente 0,16 s, recoloca el origen del rig y recupera imagen en 0,20 s. Conserva seguimiento de cabeza, bloquea telepads mientras está sentado y actualiza objetivos de entrada/salida. `HeadPoseAdapter` añade una altura virtual de 1,6 m en Android; no está midiendo la altura física del estudiante.

| Alternativa | Presencia | Riesgo de incomodidad | Decisión |
|---|---|---|---|
| Salto instantáneo sin fade | Brusco, poca señal de transición | Desorientación posible | Mantener solo como alternativa técnica de prueba |
| Fade + traslado | Comprensible con estado claro | Menor exposición al desplazamiento visual | Conservar base actual |
| Descenso suave de cámara | Puede parecer una animación corporal | Movimiento visual sin movimiento físico correspondiente | No recomendado por defecto |
| Silla que se mueve, cámara quieta | Explica preparación del puesto | Bajo si no invade la vista o destino | Decoración opcional |
| Híbrido | Sonido, señal, fade y estación activada | Acotado y controlable | **Recomendado** |

Implementación conceptual: «Entrar al puesto» → breve confirmación → sonido suave de silla → fade actual → recolocación existente → monitor en estado activo. Salida mediante objetivo visible con dwell real, sin giro de cámara impuesto. Una silla puede desplazarse discretamente durante el negro si no cambia los colliders del destino.

La postura virtual y la física son distintas: el alumno puede permanecer sentado físicamente toda la demostración. No se le debe ordenar que busque una silla a ciegas o que se levante realmente porque el avatar sale del puesto. Si hace falta calibración, ajustar el anclaje virtual con la lógica del rig; no escribir continuamente sobre la cámara rastreada. [Principios de comodidad de Meta](https://developers.meta.com/vr/design/comfort/); aplicados como criterio, no como garantía clínica ni función propia de Cardboard.

## 5. Qué objetos merecen interacción

| Objeto | Función educativa | Acción Cardboard | Decisión de alcance |
|---|---|---|---|
| Teléfono | Fuente de contraste con límites | Responder, leer, registrar | Nuevo prioritario después de infraestructura |
| Portapapeles | Procedimiento y memoria de revisión | Ampliar, pasar hoja, marcar | Prioridad inmediata |
| Carpeta | Identificar caso activo | Abrir expediente actual | Opcional; evitar duplicar monitor |
| Hoja de transferencia | Comparar un campo fuera de pantalla | Inspección de extracto | Opcional si reduce alternancia; no otra copia completa |
| Impresora | Materializar evidencia o cierre | Respuesta visible después del registro | Reutilizar; no obligar a caminar por cada hoja |
| Archivador | Consultar casos resueltos | Abrir historial | Posponer; el historial actual ya cumple esa función |
| Sello | Confirmar una acción ya decidida | Animación de registro | Cosmético; no exigirlo además del botón ni sellar «sin fraude» |

La manipulación simulada consiste en seleccionar, abrir una vista, mover **el objeto** a una pose conocida y devolverlo. No representa agarre real, no usa posición de manos y no permite tomar un objeto desde cualquier ángulo. La variante con manos se describe separadamente en [viabilidad](CARDBOARD_FEASIBILITY.md).

## 6. Ronda mínima y ronda ampliada

### MVP

Portal y prólogo actuales → entrar al puesto con fade → leer expediente → consultar libremente registro/alertas/portapapeles → una llamada opcional en SIM-003 DEFENSA → registrar si procede → decisión actual → confirmación → siguiente expediente → cierre y comparación actuales.

No repetir portal, prólogo, traslado y asiento por cada caso. No obligar a recorrer las tres fuentes cuando no aportan nada. «Registrar» debe guardar una observación útil, no convertirse en peaje para habilitar Aprobar/Retener/Devolver.

Objetivo de duración para evaluar, no resultado medido: consulta telefónica de unos 10–25 segundos de lectura; referencia en una o dos hojas; ninguna espera artificial larga. El tiempo total se medirá en el piloto, sin penalización por leer despacio.

### Ampliada

Añadir un segundo caso con resultado inconcluso, hojas contextuales completas y resumen de fuentes al terminar. Eventualmente, una impresión física como respuesta y animación de auricular. Conservar la misma libertad de decidir y la misma contabilidad de pérdidas.

En resultados: «modo», «decisión del jugador», «estado efectivo», «intervención de defensa», «evidencias consultadas/registradas» y explicación educativa. **[P]** `TransactionRecord` no guarda la `Decision` original como campo independiente; no reconstruirla a partir de `Status` cuando `DefenseOverride` sea verdadero. Un registro auxiliar debe capturar la elección antes de `Decide`, si se añade ese detalle. Los totales existentes siguen siendo la fuente de verdad.

## 7. Arte, audio y animación

### Recursos localizados y reutilización [P]

- Portapapeles de la escena: soporte, papel y clip ya construidos; conservar el original recuperable.
- Materiales de `Assets/_Project/OfficeVisual/GraphicsPolishV2/Materials/`: `Wood_DesaturatedOak`, `Wood_WalnutInset`, `Plastic_EquipmentMatte`, `Plastic_Keycaps`, `Metal_GraphitePaint`, `Metal_ArchiveEnamel`, `Metal_AluminiumTrim`, `Paper_Ivory`, `Paper_StackEdges`, `Fabric_AcousticSlate` y `Fabric_SeatCharcoal` entre los recursos disponibles.
- Prefabs de `Assets/_Project/OfficeVisual/Prefabs/`: `Bruno_Printer`, `Jarlan_Papers`, `Kenney_Archive`, `Kenney_Books`, `Quaternius_Desk`.
- UI/TMP, iconos procedurales y componentes de foco del proyecto. No sustituir texto dinámico por PNG.
- Según [créditos locales](../Context/ASSET_CREDITS.md): recursos Kenney/Quaternius CC0; impresora Bruno y papeles Jarlan CC BY 3.0; conservar atribuciones. OfficePack tiene licencia pendiente de verificar: excluirlo de nuevas incorporaciones. Revisar la entrada concreta antes de redistribuir un recurso.

### Producción mínima propuesta [R]

| Recurso | Cómo debe ser | Origen recomendado / IA | Prioridad |
|---|---|---|---|
| Teléfono institucional genérico | Base, auricular y teclado reconocibles; plástico mate; sin marca ni afirmación de modelo histórico exacto | Primitivas Unity o modelado simple; IA útil como referencia, no malla final sin optimizar | Necesario para A |
| Estados del teléfono | Inactivo, disponible, pendiente, en llamada, registrada | UI/TMP y pequeña luz sin luz dinámica; iconos geométricos propios | Necesario |
| Hoja contextual | Papel marfil/gris, bloques amplios, títulos petróleo, cian discreto | Unity UI/TMP; no IA para letras ni datos | Necesario |
| Iconos teléfono/hoja/fuente | Silueta gruesa, un solo lenguaje, sin detalles pequeños | Geometría/sprites propios; IA solo exploración de estilo | Útil |
| Tono de llamada | Corto, institucional, distinguible del aviso bancario; no estridente | Grabación/síntesis propia o clip con licencia; IA viable con derechos revisados | Prioritario después de flujo |
| Contestar/colgar | Dos clics suaves distintos, sin ruido excesivo | Grabación propia/CC0; IA no imprescindible | Prioritario |
| Papel/clip/silla | Respuesta breve y baja, sin chirrido agresivo | Foley propio/licenciado; IA posible | Pulido |
| Diálogo | Dos o tres intervenciones, español claro, remitente ficticio | TMP primero; voz humana o sintética con permiso/licencia, sin imitar personas reales | Subtítulos esenciales; voz opcional |
| Ambiente | Aire acondicionado discreto y actividad distante no inteligible | Grabación/licencia o IA revisada; loop limpio | Después de inteligibilidad |
| Música | Instrumental sobria, poco densa, volumen bajo; reducir durante voz | IA posible si permite uso y distribución; silencio también válido | Opcional |
| Animaciones | Luz, auricular a pose, hoja, silla; breves y cancelables | Transform/Animator de Unity, sin simulación física de papel | Solo necesarias para feedback |

No hacen falta texturas 4K, normal maps de papel ni reflejos en tiempo real. Reutilizar materiales; el acabado depende de silueta, contraste, jerarquía y respuestas. Los estados no deben depender solo del color.

**Prompts de referencia, no recursos generados en esta sesión:**

- Arte: «Teléfono de escritorio institucional genérico, oficina bancaria latinoamericana de mediados de la década de 2010, plástico grafito mate, auricular claro, teclado grande, sin marcas, sin texto, vistas frontal lateral y tres cuartos, formas simples aptas para modelado low poly, iluminación neutra».
- Foley: «Timbre breve de teléfono de oficina, dos pulsos discretos, limpio y aislado, sin voz, sin música, sin reverberación grande ni saturación». Verificar licencia y editar principio/final.
- Voz: «Voz ficticia adulta en español latinoamericano, tono profesional tranquilo, lectura clara y natural, sin dramatismo ni imitación de personas; respetar literalmente el guion educativo aprobado».

Conservar por recurso: autor/proveedor, fecha, términos de uso, archivo original y transformaciones. Una salida IA no garantiza licencia, corrección factual, topología adecuada o coherencia estilística.

## 8. Condiciones para aprobar el diseño

El teléfono se conserva si el alumno comprende mejor procedencia e incertidumbre; el portapapeles se convierte en principal si mejora consulta y comodidad; el asiento se pule solo si no altera seguimiento ni salida segura. Ninguna de estas decisiones está validada aún en HONOR.

Si la entrega está próxima y no hay tiempo para pruebas físicas, limitarse a la vista ampliada reversible y a los recursos sonoros actuales. Mantener el teléfono tras una opción desactivada hasta que supere la regresión y el piloto del [roadmap](CODEX_IMPLEMENTATION_ROADMAP.md).
