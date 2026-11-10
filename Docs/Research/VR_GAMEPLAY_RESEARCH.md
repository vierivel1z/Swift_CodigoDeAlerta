# SWIFT — Investigación de jugabilidad VR

**Fecha:** 8 de noviembre de 2026. **Destino:** Unity 6000.3.24f1 / URP 17.3.0 / Android / Google Cardboard / HONOR Magic7 Lite.

**Resultado:** diseño documentado, sin implementar funcionalidades, instalar paquetes, abrir una nueva sesión de pruebas, compilar ni publicar. Los cuatro documentos de esta carpeta constituyen la entrega; las funciones propuestas todavía no existen.

Documentos complementarios: [Diseño de experiencia](IMMERSIVE_GAMEPLAY_DESIGN.md), [Viabilidad Cardboard](CARDBOARD_FEASIBILITY.md), [Plan ejecutable para Codex](CODEX_IMPLEMENTATION_ROADMAP.md).

## 1. Respuesta a la pregunta de investigación

SWIFT puede ganar presencia trasladando **acciones con significado** a dos herramientas reconocibles: un portapapeles contextual y un teléfono de contraste. El monitor conserva el expediente y las decisiones. El espacio deja de ser solo un decorado cuando la ubicación de cada objeto expresa su función, su estado responde a una acción y la información obtenida cambia lo que el jugador sabe, aunque no cambie las reglas del dominio.

La nueva mecánica con mayor valor educativo potencial es la **verificación telefónica parcial**: agrega una fuente que debe compararse, no una respuesta correcta. La intervención de menor riesgo es **ampliar el portapapeles existente**, con una vista legible y reversible. Esa infraestructura puede servir después al teléfono. No hace falta convertir toda la oficina en un simulador de físicas.

Esta es una recomendación de diseño, no un resultado experimental sobre SWIFT. Debe contrastarse con estudiantes en el visor.

## 2. Método y estado observado

Convenciones utilizadas en toda la entrega:

- **[P] Proyecto:** observación del código, recursos o informes locales.
- **[F] Fuente:** hecho respaldado por una publicación externa enlazada.
- **[I] Inferencia:** interpretación razonada; no demuestra un efecto en este juego.
- **[R] Recomendación:** diseño nuevo pendiente de implementación y prueba.

Se leyó la [auditoría del 8 de noviembre](../Context/AUDITORIA_Y_PLAN_DE_MEJORA_2026-11-08.md) y se contrastaron scripts de dominio, interacción, asiento, UI y audio; la escena serializada; modelos y materiales. La investigación externa utiliza páginas de desarrolladores, diarios de desarrollo, una entrevista a un diseñador, documentación técnica y una publicación de investigación. Se revisaron textos y diapositivas disponibles; no se afirma haber jugado estos títulos ni visto íntegramente sus vídeos o charlas.

### Estado que condiciona las propuestas

| Sistema | Estado [P] | Pendiente o nuevo |
|---|---|---|
| Oficina, portal, seis expedientes, registro, alertas y decisiones | Implementados | Validación física actualizada y claridad educativa |
| Mirada | `VRInteractionManager`: fijación de 1,5 s, progreso y bloqueo de repetición | Reutilizar; probar cada nueva superficie interactiva |
| Estación | `OperationsStation`, `UIManager`, `BankDocumentUI` y monitor de apoyo | Evitar duplicar sus datos en una herramienta innecesaria |
| Portapapeles | Objeto `Analyst review clipboard`: madera, clip, papel y texto; `BankAction.Notebook` abre Procedimiento | Hojas contextuales, lista manual y ampliación propia son nuevas |
| Teléfono | No se encontró controlador, estado, catálogo de llamadas ni acción telefónica en los scripts propios inspeccionados | Mecánica nueva; tampoco se identificó un modelo reutilizable de teléfono por el inventario de nombres |
| Asiento | `AnalystSeat`: fade, traslado del origen, verificación de destino, estado y bloqueo de telepads | Sonido y movimiento discreto de silla son propuestas |
| Audio | Cinco categorías, ocho eventos y tonos temporales; sin clips finales importados en la auditoría | Producción sonora y escucha en dispositivo |
| Manos | Sin integración de XRI, XR Hands, OpenXR o Ultraleap en el manifest | Depende de identificar hardware del salón |
| Pruebas y APK | Evidencias históricas de Editor y APK del 2 de noviembre | No certifican los cambios posteriores en HONOR |

El teléfono objetivo ahora está confirmado como **Magic7 Lite**; esto actualiza la incertidumbre del informe anterior. Siguen sin confirmarse el visor concreto, su ajuste óptico, los periféricos del salón y el rendimiento de esta escena en ese teléfono. No se repitieron pruebas de Unity en esta investigación.

## 3. Referentes: qué hacen y qué transferir

Los costes siguientes estiman **adaptaciones pequeñas a SWIFT**, no reproducir los juegos. Unidad: jornada de unas seis horas de desarrollo concentrado, una persona que conoce el proyecto, sin incluir producción externa, compras ni disponibilidad de hardware. Las tareas comparten infraestructura y los rangos no deben sumarse como presupuestos independientes.

### 3.1 Job Simulator — objetos que contestan a la acción

**Mecánica y evidencia [F].** Owlchemy presenta trabajos organizados en estaciones, incluido un cubículo de oficina. Su página muestra una experiencia de objetos cotidianos y acciones de trabajo. La sesión de Daniel Perry en GDC 2017 trata específicamente el sonido de interacción con objetos y entornos VR con mandos rastreados; se consultó la descripción pública, no el contenido completo de la charla. [Juego oficial](https://jobsimulatorgame.com/), [The Sound of Job Simulator](https://gdcvault.com/play/1023946/The-Sound-of-Job-Simulator).

**Qué aporta [I].** Un objeto familiar sugiere una acción antes de leer instrucciones: levantar un auricular, consultar una hoja o recoger una impresión. La respuesta localizada hace perceptible que el entorno cambió por la acción del usuario. Esto puede dar presencia incluso con una dirección artística simplificada.

**Cómo comunicar interacción en SWIFT [R].** Reutilizar el foco y el progreso reales; añadir un único verbo junto al objeto enfocado —«Consultar», «Responder», «Registrar»— y una respuesta breve de imagen/sonido. No se atribuye a Job Simulator un algoritmo concreto de contornos o dwell: esa solución corresponde al sistema propio del proyecto.

**Adaptación técnica [R].** Un objeto con collider estable y `BankInteractable`, presentación de foco mediante `DocumentFocus`, evento de acción aceptada y estado visual persistente. Impresora: una hoja visible después de registrar evidencia, sin exigir recogerla para avanzar. Teléfono: luz y tono al quedar una llamada pendiente.

**No copiar.** Manipulación libre de todos los adornos, lanzamiento de objetos, caos cómico y físicas numerosas. El tono educativo institucional y la selección con cabeza necesitan menos acciones, con más significado.

**Coste orientativo.** 1–2 jornadas para una respuesta de objeto existente; 3–5 para una llamada completa de un caso, excluyendo voces. Hacer «todo agarrable» es otro alcance.

### 3.2 I Expect You To Die — interés desde una posición sentada

**Mecánica y evidencia [F].** Schell Games describe un juego diseñado para estar sentado, con telequinesis para objetos lejanos, pistas en el entorno y comunicación del responsable mediante auricular. La entrevista a Shawn Patton documenta comodidad, presupuesto, objetivos y posibilidades de uso claras como criterios de diseño. Esto demuestra comunicación narrativa y alcance remoto; no demuestra un sistema de verificación bancaria ni que sus llamadas funcionen como la propuesta de SWIFT. [Schell Games](https://schellgames.com/games/i-expect-you-to-die), [entrevista de 2016](https://voicesofvr.com/482-designing-an-escape-room-puzzle-game-with-i-expect-you-to-die/).

**Qué aporta [I].** La riqueza procede de relaciones entre objetos y pistas, no del número de metros recorridos. Un escritorio puede ser un espacio completo de decisiones si cada herramienta cumple una función y está a una distancia accesible.

**Adaptación [R].** Mantener la ronda bancaria sentada; portapapeles y teléfono dentro de un arco de mirada cómodo. El teléfono aporta una declaración parcial; el expediente contiene otra fuente; el jugador encuentra la diferencia. Un objeto seleccionado puede ir a una pose de inspección, pero se explicará como animación asistida por mirada, no como manos rastreadas.

**Técnica.** Anclajes del puesto, cámara libre para seguir la cabeza, estado modal de inspección, retorno explícito y cancelación segura. Las fichas importantes deben poder inspeccionarse sin inclinarse físicamente ni caminar.

**No copiar.** Muerte y repetición como enseñanza principal, presión temporal, telequinesis con combinaciones de botones o enigmas que oculten información bancaria necesaria. No trasladar continuamente la cámara para imitar el movimiento de un cuerpo.

**Coste.** 2–4 jornadas para inspección legible compartida; 1–2 para pulir el asiento existente. Sus diapositivas de GDC recomiendan prototipos físicos y observar acciones inesperadas; ese método sí es útil antes de modelar más mobiliario. [Playtesting VR, Shawn Patton, GDC 2018](https://media.gdcvault.com/gdc2018/presentations/Patton_Shawn_Playtesting_VR.pdf).

### 3.3 Papers, Please — comparar es la actividad

**Mecánica y evidencia [F].** El juego combina documentación, inspección y decisión de admisión. Lucas Pope explica que en PC se organizan documentos, se señalan discrepancias y se interroga; al adaptar a móvil tuvo que preservar comparaciones y herramientas de referencia pese al espacio reducido. Su diario de febrero de 2013 explicita la intención de introducir conceptos gradualmente y equilibrar personas con y sin errores. Son testimonios del diseño y su evolución, no una enumeración validada de reglas de cada día de la versión final. [Press kit](https://papersplea.se/presskit/), [devlog de adaptación móvil, 2022](https://dukope.com/devlogs/papers-please/mobile/), [diario de desarrollo, febrero de 2013](https://dukope.com/devlogs/papers-please/tig-03/).

**Qué aporta [I].** El interés reside en descubrir la diferencia y asumir la consecuencia. Una respuesta automática puede hacer más rápida la interfaz, pero también quitar el trabajo intelectual que da sentido a la simulación.

**Adaptación [R].** Mostrar «Orden» y «Fuente consultada» con procedencia visible. El portapapeles conserva el procedimiento, y la llamada añade un dato parcial. Las marcas «revisado» son del alumno: no significan «correcto». La explicación completa aparece después de decidir o en la revisión final.

**Progresión viable [R].** Conservar el orden y las anomalías actuales. SIM-001 enseña lectura; SIM-002 exige detectar un campo ausente; SIM-003 introduce contraste de nombre; SIM-004 permite discutir información insuficiente; SIM-005 y SIM-006 consolidan cautela ante monto y coincidencias. Es una secuencia pedagógica propuesta sobre los seis casos existentes, no nuevas reglas ni una afirmación de dificultad medida.

**Técnica.** Proyección de `TransactionData`, hojas por ID, almacenamiento de marcas separado del dominio y registro de procedencia. No llamar a `TransactionManager.Recommended` ni mostrar `SimulatedFraud` durante la revisión.

**No copiar.** Texto diminuto, cronómetro laboral, castigos económicos ajenos a la simulación, estampado obligatorio para cada subpaso y documentos amontonados que exijan arrastrar con la cabeza.

**Coste.** 2–4 jornadas para hojas contextuales y comparación; 1–2 adicionales para una revisión educativa por expediente, sin recalcular puntuación.

### 3.4 Vacation Simulator — actividades y atención legible

**Mecánica y evidencia [F].** El sitio oficial distribuye actividades en playa, bosque, montaña y resort. Owlchemy documenta subtítulos que identifican al hablante, orientan hacia él y adaptan presentación al espacio; esta información proviene de su explicación de implementación de 2019. [Juego oficial](https://vacationsimulatorgame.com/), [subtítulos y soporte de idiomas](https://owlchemylabs.com/blog/vacation-simulator-subtitle-update-language-support).

**Qué aporta [I].** Una zona con una actividad reconocible permite descubrir qué hacer sin convertir el entorno en una lista de botones. Una respuesta visible a cada paso reduce incertidumbre. Aquí no se afirma que todo objeto del juego use el mismo resaltado o que su tecnología de subtítulos sea reutilizable directamente.

**Adaptación [R].** Tres focos de trabajo: monitor/decisión, portapapeles/procedimiento y teléfono/contraste. El teléfono señala su estado con luz, icono y texto; los subtítulos tienen remitente y una línea de estado. Las demás zonas del banco sirven para orientación y contexto, no para obligar a desplazarse por cada expediente.

**Técnica.** Un solo panel de inspección activo, TMP grande, foco identificable, subtítulos a ritmo del lector y retorno al puesto. Se conserva visible el monitor cuando sea posible, sin ventanas flotantes múltiples ni texto siguiendo rígidamente cada microgiro de cabeza.

**No copiar.** Mundo abierto, gran número de actividades, bots animados ni sistemas completos de conversación. La lección es organizar acciones, no multiplicar zonas.

**Coste.** 1–2 jornadas de orientación visual y ayuda contextual reutilizando componentes; 1–2 para pulir subtítulos sobre el flujo de llamada ya construido.

### 3.5 HoloLAB Champions — procedimiento y retroalimentación formativa

**Por qué es comparable.** Es un laboratorio educativo de acciones técnicas: el estudiante observa, usa herramientas, sigue un procedimiento y recibe evaluación. Ese ciclo es más cercano a SWIFT que una aventura exclusivamente narrativa.

**Mecánica y evidencia [F].** Su guía docente describe práctica de laboratorios individuales, consulta de instrucciones en un cuaderno y repetición. En práctica se omiten música, público e instrucciones habladas; al enviar el trabajo se obtiene evaluación de precisión, rapidez y seguridad, con información detallada del resultado. La guía contempla apoyo de compañeros fuera del visor. [Producto oficial](https://schellgames.com/games/hololab-champions), [guía docente, especialmente páginas impresas 8–12](https://schellgames.nyc3.cdn.digitaloceanspaces.com/images/Microsites/hololab-champions/resources/HoloLABChampions_TeacherGuide_ip.pdf).

**Qué aporta [I].** La herramienta física y la devolución posterior vinculan procedimiento con resultado. El cuaderno funciona porque acompaña una actividad; copiarlo como otro bloque de texto sin utilidad no mejora la experiencia.

**Adaptación [R].** Hoja de revisión contextual, llamada breve cuando añade información y explicación posterior que distinga decisión humana de intervención automática de DEFENSA. Un compañero puede observar y anotar dudas durante el piloto, sin dirigir cada acción.

**Técnica.** Registro auxiliar de consultas y marcas, lectura de resultados ya calculados y una ficha educativa por caso. Las consultas no necesitan sumar puntos ni alterar pérdidas.

**No copiar.** Puntuación por velocidad, equipamiento de laboratorio, precisión motriz con manos ni competición que anime a leer apresuradamente.

**Coste.** 1–3 jornadas para registro y explicación sobre la infraestructura propuesta, más preparación y realización del piloto.

## 4. Presencia no equivale a aprendizaje

**[F]** Makransky, Terkildsen y Mayer estudiaron una simulación de laboratorio con 52 universitarios. En ese estudio, la condición inmersiva aumentó presencia, pero obtuvo menor aprendizaje y mayor carga cognitiva que la alternativa de escritorio. Es evidencia de un contexto específico, no una condena general de VR. [Artículo de 2019 y resumen institucional](https://researchprofiles.ku.dk/en/publications/adding-immersive-virtual-reality-to-a-science-lab-simulation-caus/).

**[R]** Cada interacción nueva de SWIFT debe contestar: «¿Qué aprende o contrasta el alumno que antes no podía?». Si solo añade segundos de espera, un traslado o un gesto obligatorio, debe simplificarse. Medir descubrimiento, errores y comprensión, además de agrado.

Piloto formativo propuesto: 3–5 participantes nuevos, sin pretensión de significación estadística. Observar si pueden abrir una referencia, identificar una diferencia, registrar su fuente y justificar una decisión. Preguntar después por qué una coincidencia no confirma fraude y por qué una llamada fallida no prueba irregularidad. Comparar interfaz actual y propuesta con tareas equivalentes y orden alternado; repetir exactamente el mismo caso puede introducir memoria y sesgar la comparación.

## 5. Qué hace que estos referentes se perciban acabados

Las siguientes son **inferencias de diseño**, no mediciones comparativas de calidad gráfica:

| Principio | Aplicación viable a SWIFT |
|---|---|
| Objetos reconocibles y función clara | Auricular, clip y hoja con silueta legible; evitar detalles minúsculos |
| Respuestas coherentes | Foco, progreso real, confirmación y estado posterior persistente |
| Composición dirigida a la tarea | Mayor contraste en el puesto y herramientas útiles; decoración secundaria |
| Materiales diferenciados | Plástico mate, papel marfil, madera desaturada y metal pintado existentes |
| Sonido con causa | Una llamada suena por una transición concreta; una hoja responde al pasarla |
| Lectura ajustada al medio | Información corta y ampliable; campos dinámicos TMP, sin imagen vertical con letra impresa |
| Consecuencias comprensibles | Separar lo que decidió el alumno de lo que corrigió el control automático |

No se recomienda perseguir iluminación cinematográfica de una referencia 2D en Android. Antes de nuevos modelos: foco lumínico, reducción de líneas finas, contraste de papel y texto, y sonidos discretos. El inventario y las posibilidades de IA están en [el diseño](IMMERSIVE_GAMEPLAY_DESIGN.md).

## 6. Decisiones recomendadas

1. Portapapeles físico con ampliación legible como primera prueba reversible.
2. Teléfono por mirada, una conversación subtitulada de SIM-003; solo DEFENSA.
3. Mantener segundo monitor como alternativa mientras se evalúa el portapapeles.
4. Mantener fade y seguimiento de cabeza al entrar/salir del puesto.
5. No convertir todas las transferencias en un circuito obligatorio de llamadas, impresiones y sellos.
6. Probar una ronda completa en HONOR antes de ampliar casos y producir voces.
7. Separar una futura versión con mandos/manos de la entrega Cardboard.

## 7. Fuentes técnicas y límites de actualidad

Fuentes consultadas el 8 de noviembre de 2026; las publicaciones históricas se identifican por fecha en el texto. Sus páginas pueden cambiar. La documentación de paquetes describe capacidades, no compatibilidad ya probada con este proyecto.

- [Google Cardboard para Unity](https://developers.google.com/cardboard/develop/unity/quickstart): render estéreo, entrada y configuración del visor.
- [Unity XRI 3.1 — XR Grab Interactable](https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.1/manual/xr-grab-interactable.html): referencia para una variante futura de agarre.
- [Unity XRI 3.1 — locomoción](https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.1/manual/locomotion.html): arquitectura de locomoción; no propuesta de reemplazar `AnalystSeat`.
- [Unity XR Hands 1.7](https://docs.unity3d.com/Packages/com.unity.xr.hands@1.7/manual/index.html): manos requieren proveedor compatible.
- [Meta — comodidad](https://developers.meta.com/vr/design/comfort/): principios de atención, alcance y movimiento; sus funciones de passthrough no se atribuyen a Cardboard.
- [HONOR Magic7 Lite — especificaciones](https://www.honor.com/es/phones/honor-magic7-lite/spec/): referencia del dispositivo, no medición de SWIFT.
- [Owlchemy — uso de manos, 2024](https://owlchemylabs.com/blog/its-time-to-job-with-human-hands): evolución de Job/Vacation en hardware compatible; no posibilidad nativa del HONOR.

**Límite central:** ni el éxito de los referentes ni la existencia de un SDK demuestran que estas propuestas mejoren el aprendizaje o funcionen cómodamente en el Magic7 Lite. Esa aceptación corresponde a las pruebas futuras del [plan](CODEX_IMPLEMENTATION_ROADMAP.md).

## 8. Respuestas finales para decidir el alcance

| Pregunta | Conclusión propuesta |
|---|---|
| 1. ¿Qué mecánica nueva mejoraría más SWIFT? | Teléfono de contraste parcial: añade procedencia e incertidumbre, con mayor potencial educativo que otra animación o prop. Ese beneficio todavía debe medirse. |
| 2. ¿Vale la pena incorporar el teléfono? | Sí, como una llamada corta en DEFENSA y un solo caso primero; no como obligación para seis transferencias ni solución automática. |
| 3. ¿Sustituir el segundo monitor? | Probar antes el portapapeles ampliado. Mantener el secundario como alternativa; sustituir su función redundante solo si el ensayo confirma mejor lectura y uso. |
| 4. ¿Cómo sentarse? | Conservar fade y recolocación del rig; añadir sonido/señal de silla sin imponer movimiento continuo de cámara. |
| 5. ¿Qué objetos hacer interactivos? | Monitor, portapapeles y teléfono como herramientas centrales; impresora como respuesta de evidencia. Carpeta, archivador y sello solo si evitan duplicación o añaden comprensión. |
| 6. ¿Qué permite Cardboard? | En esta implementación: mirada, dwell, lectura ampliada, teletransporte y movimiento de objetos a poses prefijadas. No son agarres rastreados. |
| 7. ¿Qué exige mandos/manos? | Agarre espacial libre, posición articulada y manipulación natural necesitan dispositivo, runtime y proveedor compatibles; revisar equipo del salón antes de elegir SDK. |
| 8. ¿Imprescindible frente a opcional? | Imprescindibles: lectura, foco fiable, cancelación, evidencia contextual y resultados intactos. Opcionales: voz, animación de auricular, sellos, físicas y manos. |
| 9. ¿Qué añadir con menor riesgo? | Portapapeles existente con ampliación reversible, bajo flag y en escena de revisión; después una llamada acotada. Ninguna función nueva está libre de riesgo sin prueba. |
| 10. ¿Primer pedido a Codex? | Ejecutar fases 0 y 1 del roadmap: respaldo, regresión de base y portapapeles ampliado; mantener teléfono y manos fuera de esa primera implementación. El prompt completo está en el plan. |
