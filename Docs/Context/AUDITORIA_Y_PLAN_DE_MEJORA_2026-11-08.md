# SWIFT: Código de Alerta — auditoría y plan viable de mejora

**Fecha:** 8 de noviembre de 2026, America/Lima. **Proyecto:** `D:\UnityGames\SWIFT_CodigoDeAlerta`. **Destino principal:** Android / Google Cardboard. **Alcance de esta sesión:** inspección del proyecto, revisión de evidencias y búsqueda de referencias; únicamente documentación. No se implementaron funciones, instalaron paquetes, modificaron escenas, ejecutaron builds ni hicieron operaciones de publicación.

## 1. Conclusión ejecutiva

El proyecto es un **prototipo funcional avanzado con presentación en proceso de cierre**, todavía sin evidencia suficiente para declararlo listo para una demostración física en el salón. Ya tiene una oficina, portal de modos, seis expedientes ficticios, consulta de registros, alertas, decisiones, evidencia, defensa, resultados, tutorial y selección por mirada. El trabajo pendiente principal es consolidar y comprobar esta experiencia en el dispositivo real, completar el sonido y mejorar la orientación y explicación educativa.

No conviene rehacer la oficina ni acumular más modelos. La mejora más rentable consiste en **hacer más evidente qué se puede hacer, por qué una decisión tiene sentido y qué acaba de ocurrir**. Gráficamente conviene trabajar jerarquía de iluminación, coherencia de materiales y lectura a distancia. La UI documental ya recibió el tratamiento gris papel y petróleo; repetir ese rediseño tendría menos impacto que verificarlo en el visor.

**Recomendación de entrega:** mantener una versión Cardboard completa por mirada y preparar, si el equipo del salón lo permite, una variante de interacción con mandos o manos. La segunda variante debe compartir las reglas bancarias y demostrar primero una sola estación. Añadir manos no es un requisito técnico para que la versión Cardboard sea válida y no se consigue simplemente importando un modelo 3D.

Las tres carencias más urgentes son:

1. **Versión instalable actual y validación física:** la APK localizada más reciente es del 2 de noviembre y precede a los últimos cambios visuales y de estación.
2. **Audio final:** existen categorías y disparadores, pero las referencias de clips están vacías y no hay archivos de audio importados en Assets.
3. **Preparación de la demostración:** modelo exacto de HONOR, visor y herramientas de manos sin confirmar; faltan resultados documentados de lectura, estabilidad, temperatura y uso por personas nuevas.

## 2. Cómo se auditó y qué significa “verificado”

Se revisaron versiones, paquetes, lista de compilación, código de interacción, dominio bancario, UI, audio, recursos, materiales, perfiles móviles, Git, informes y recibos de pruebas. Se abrieron capturas existentes de oficina, puesto, portal, prólogo y expediente refinado. Se consultaron fuentes oficiales de otros juegos y documentación técnica de Google, Unity y Ultraleap.

**Esta auditoría no ejecutó nuevamente Unity, Play Mode, DomainTests ni Android.** Los PASS mencionados abajo son evidencias anteriores localizadas y fechadas, no pruebas nuevas del 8 de noviembre. No se escuchó audio en un dispositivo. Las opiniones visuales se basan en imágenes reales anteriores del Editor, no en una captura binocular nueva.

La documentación antigua conserva problemas que ya fueron resueltos. Este informe contrasta sus afirmaciones con el estado local y no suma pendientes históricos como si todos siguieran abiertos. Tampoco asigna un porcentaje de avance arbitrario.

### Evidencia local principal

| Evidencia | Qué permite afirmar | Límite |
|---|---|---|
| `ProjectSettings/ProjectVersion.txt`, `Packages/manifest.json` y lock | Unity 6000.3.24f1, URP 17.3.0, Input System 1.20.0 y XR Management 4.5.4 | No certifican compatibilidad de un SDK nuevo |
| `ProjectSettings/EditorBuildSettings.asset` | BancoCorresponsal primera y habilitada; SampleScene y HelloCardboard conservadas y deshabilitadas | Comprobar cualquier perfil que sobrescriba la lista al generar la próxima APK |
| `Artifacts/DossierTreatment/review-result.txt` | PASS del ciclo UI, 4/11/2026, 16:51:08 UTC | Editor; hubo un intento previo fallido por espera, conservado en el historial |
| `Artifacts/DossierTreatment/stationpc-result.txt` | PASS de entrada/salida y selección PC, 4/11, 16:51:22 UTC | No equivale a sensores o manos reales |
| `Artifacts/StationRefinement/{flow,office,smoke}-result.txt` | PASS de sondas el 4/11, alrededor de 13:21 UTC | No ejecutadas de nuevo hoy |
| [Validación funcional](../Archive/2026-11-10/Context/FUNCTIONAL_PROTOTYPE_VALIDATION.md) | Build Android y verificaciones de noviembre 2; registros de pruebas y corrección del raycast | APK anterior al refinamiento posterior |
| [Progreso UX](../Archive/2026-11-10/Context/UX_GAMEPLAY_PROGRESS.md) | Tutorial, buses de audio, pruebas históricas y límites | Parte de los pendientes se actualiza aquí |
| [Acabado del expediente](../Archive/2026-11-10/Context/DOSSIER_VISUAL_TREATMENT.md) | Cambio visual final del documento, capturas y respaldo | Aceptación visual humana y HONOR pendientes |

**Estado de Git:** rama `review/portal-dwell-feedback`, HEAD `c4a1278`. Hay numerosos cambios locales y archivos sin seguimiento: escena, iluminación, scripts, UI y documentación. El HEAD por sí solo no reproduce lo que se ve actualmente. Una futura entrega requiere revisar y versionar explícitamente esos cambios, con autorización; en esta sesión no se hizo commit ni push.

Cardboard está declarado por URL Git sin revisión en el manifest; el lock conserva el hash `7fb2c0e438d2ef686f51cd59b53da16cc07b1b68`. Conviene fijar explícitamente la revisión antes de una entrega reproducible, después de comprobarla, sin actualizar el plugin por el solo hecho de auditar.

## 3. Estado real por sistema

| Área | Estado encontrado | Qué falta |
|---|---|---|
| Reglas y expedientes | Dominio separado, seis órdenes, Aprobar/Retener/Devolver, evidencias y comparación | Repetir regresión al preparar la versión de entrega; mejorar explicación sin alterar cálculos |
| Ataque y defensa | Implementados; los controles reforzados pueden corregir el resultado efectivo | Explicar claramente decisión del jugador frente a intervención del control |
| Portal y retícula | Dwell de 1,5 s, barras ligadas al progreso, rearme y activación única | Validación física del seguimiento, brillo y facilidad de selección |
| Prólogo | Secuencia existente, omisión, final automático y panel cercano; portal visible detrás en la revisión final | Lectura real, comodidad al girar y audio específico |
| Estación | Reposo, entrada/salida, expediente, registro, alertas, procedimiento y apoyo secundario | Comprobar tamaño angular, giros al secundario y acceso con diferentes usuarios |
| Tutorial | Práctica breve y omitible, separada de puntuación bancaria | Comprobar que alguien nuevo lo descubre y termina sin ayuda |
| Oficina | Modelos importados, cubículos, zonas y materiales diferenciados | Priorizar foco y señalética; reducir ruido visual y comprobar contacto/luz |
| Materiales | Biblioteca aislada con 12 materiales reutilizados de seis familias | Aplicación coherente donde haga falta, no otra biblioteca de duplicados |
| Audio | Cinco categorías y ocho eventos; tonos sintéticos temporales | Clips finales, ambiente, mezcla, escucha y prueba de pausa/reanudación |
| Manos | No se encontró integración de XR Hands, OpenXR/XRI ni Ultraleap en el manifest | Identificar hardware y desarrollar un proveedor separado si se necesita |
| Android | Existe APK anterior, Cardboard y configuración Android | Nueva APK, instalación, recorrido completo y rendimiento real |
| Entrega académica | Contexto, créditos y documentación amplia | Requisitos particulares completos, contribuciones del equipo, video y ficha final |

### Precauciones de interpretación

- La auditoría de noviembre 2 detectaba BancoCorresponsal deshabilitada. **Ese problema ya no aparece en la lista actual.**
- No debe presentarse el audio como inexistente: hay lógica y señales temporales. Lo que falta es la producción sonora final.
- Las capturas de GraphicsPolishV2 muestran interfaces anteriores. Sirven para analizar arquitectura y materiales, pero la UI vigente se evalúa con las capturas posteriores de estación/expediente.
- Documentos antiguos hablan de cuatro teletransportes y posteriores de ocho. La escena actual contiene diez componentes serializados con acción Teleport, incluyendo posibles objetos conservados/inactivos. Contarlos en YAML no demuestra cuántos destinos están activos y alcanzables: inventariarlos en Play y actualizar la lista de pruebas antes de la entrega.
- Hay 27 comprobaciones de dominio reportadas como PASS en sesiones anteriores. No se anuncian como repetidas en esta auditoría.

### APK localizada

`Artifacts/Functional-20261102-083259/SWIFT-BancoCorresponsal-current.apk`, 48.851.914 bytes. El informe de generación registra SHA-256 `F59EA07AE5019C2C67A1B6025E9B8DB993DB7E67B7B2B4DD39012DD6C793FB6E`, firma de desarrollo y BancoCorresponsal incluida. El hash se cita del recibo anterior; no se recalculó hoy.

El nombre “current” ya no significa que incluya los cambios del 3 y 4 de noviembre. Es necesario generar otro archivo, identificado por fecha y hash, y actualizar [HONOR_TEST_CHECKLIST.md](../Archive/2026-11-10/Context/HONOR_TEST_CHECKLIST.md), que todavía apunta a esa APK.

## 4. Diagnóstico visual y mejoras gráficas viables

La dirección artística adecuada sigue siendo **oficina bancaria institucional estilizada**: azul noche/petróleo, papel gris o marfil frío, madera desaturada y acentos funcionales. La referencia artística puede orientar composición y contraste; su iluminación idealizada no debe convertirse en una obligación de reproducir efectos caros en un teléfono.

### Lo que todavía reduce el acabado

| Prioridad | Hallazgo o riesgo | Mejora propuesta y comprobación |
|---|---|---|
| CRÍTICO | Falta confirmar lectura binocular y estabilidad de imagen | Probar expediente, monto, botones y secundario desde la silla en HONOR. No reducir paneles por buscar proporciones de monitor fotográficas |
| IMPORTANTE | En la vista general hay mucha superficie y detalle repetido de techo/suelo; compiten con el puesto | Bajar contraste de juntas y reservar el contraste más fuerte para portal, puesto y destinos útiles. Comparar desde entrada y asiento |
| IMPORTANTE | La iluminación ambiental hace que zonas diferentes tengan una lectura parecida | Repartir luz horneada por zonas: puesto claro, circulación suficiente, cubículos secundarios menos dominantes. No oscurecer globalmente |
| IMPORTANTE | La UI tiene un acabado más definido que varios muebles y rótulos | Compartir paleta y familias de materiales; sustituir rótulos largos por nombre corto e icono contenidos |
| IMPORTANTE | Muchos objetos parecen igualmente interactivos o igualmente decorativos | Estado de foco inequívoco en objetos útiles; decoración sin halos permanentes. Texto contextual solo cuando ayuda |
| IMPORTANTE | La escena puede sentirse estática y sin actividad institucional | Sonido ambiental y unas pocas respuestas locales a acciones: pantalla que cambia, luz de estado o documento que se confirma; sin animar toda la sala |
| IMPORTANTE | Líneas finas, teclas y patrones muestran dentado en los PNG | Simplificar detalle de alta frecuencia; comprobar filtrado y densidad de textura; comparar AA solo en un perfil de prueba medido |
| OPCIONAL | Madera y tela admiten mayor variedad superficial | Una textura tenue por familia si mejora la vista real; evitar normal maps y materiales adicionales sin beneficio visible |
| OPCIONAL | Exterior nocturno sencillo | Mantener siluetas opacas; mejorar composición de ventanas si ayuda a profundidad, sin añadir ciudad 3D |

**Perfil móvil observado:** render scale 0,8; MSAA 1; sombras principales 1024; una cascada y distancia 50; sombras de luces adicionales desactivadas; sin Renderer Features en Mobile_Renderer. Esto no demuestra un fallo, pero explica por qué conviene investigar el dentado. Android tiene asignado el nivel Mobile por defecto; el índice de calidad del Editor no prueba qué se ejecutará en el teléfono.

Propuesta de ensayo posterior: medir el perfil actual y después una sola variación, por ejemplo MSAA 2. Registrar tiempo CPU/GPU y legibilidad durante un recorrido idéntico. Si no hay margen, simplificar geometría y bordes finos antes de subir resolución. No se recomienda activar bloom, SSAO, sombras adicionales ni reflejos en tiempo real para “hacerlo profesional”.

### Diez mejoras visuales en orden

1. Validar y ajustar la lectura del expediente y botones en el visor real.
2. Dar al puesto principal mayor jerarquía de luz y contraste que a los cubículos.
3. Hacer inequívoca la ruta portal → puesto → consulta → decisión → resultados.
4. Unificar la señalética corta de las zonas con la UI institucional.
5. Suavizar juntas, tramas y detalles que produzcan ruido o parpadeo.
6. Revisar contacto de muebles, papeles y equipos; corregir flotación/intersecciones observadas durante el recorrido.
7. Consolidar los materiales existentes por función y escala de textura.
8. Mantener confirmaciones breves, consistentes y ligadas a acciones reales.
9. Refinar resultados para separar decisión, corrección de defensa y explicación.
10. Añadir únicamente el arte de superficie que siga aportando después de lo anterior.

## 5. Mejoras educativas y de experiencia

El bucle revisar–consultar–decidir es adecuado. Para que se sienta más terminado hace falta explicar mejor sus consecuencias, no multiplicar las reglas.

| Propuesta | Cómo debe ser | Viabilidad |
|---|---|---|
| Explicación por decisión | Una frase vinculada a la anomalía existente: “Falta el intermediario: devolver por formato”. Para coincidencias: revisar contexto, nunca afirmar fraude automáticamente | Alta; capa de presentación sobre datos existentes, revisión docente |
| Resumen más educativo | Por expediente: acción elegida, estado efectivo, intervención del control y motivo. Mantener puntuación y pérdidas actuales | Alta; el dominio ya conserva estado, `CorrectDecision` y `DefenseOverride`; guardar la acción original requeriría un registro adicional si no se puede reconstruir |
| Próximo paso contextual | Una instrucción breve en el monitor de apoyo; desaparece al dejar de ser relevante. No sugerir automáticamente la decisión correcta | Alta; reutiliza estados de sesión |
| Ritmo entre expedientes | Confirmación visual breve con acceso a explicación, sin impedir continuar ni añadir más esperas obligatorias | Media; validar que no interfiere con el rearme de mirada |
| Práctica accesible | Mantener tutorial omitible y sin puntuación; comprobar su descubrimiento con usuarios nuevos | Alta; ya existe, necesita ensayo de uso |
| Cierre para el salón | Tres preguntas: diferencia entre formato y riesgo; qué cambió la defensa; qué parte es histórica y cuál simulada | Alta; puede empezar como ficha externa sin tocar el juego |
| Evaluación de aprendizaje | Comparar respuestas antes/después con una rúbrica corta, registrar errores y ayuda requerida | Alta; no necesita cuentas, nube ni datos personales |

**Aclaración importante:** si el jugador aprueba todo con defensa activa puede evitar pérdidas y obtener una puntuación baja, porque el sistema distingue su decisión de la corrección realizada por los controles. No es necesariamente un bug. La pantalla debe hacer esa distinción comprensible.

No se propone introducir multas, nuevas reglas antifraude, información bancaria real ni un sistema económico adicional. El informe tampoco valida por investigación histórica independiente las cifras del material docente; deben revisarse contra las fuentes académicas finales antes de presentarlas como hechos.

## 6. Audio: qué existe, qué producir y cómo debe sonar

`FeedbackAudio.cs` y `AudioCueLibrary.cs` ya separan **UI, Objetos, Alertas, Ambiente y Música**. Los eventos incluyen confirmación, acción no disponible, nueva alerta, consulta, evidencia, defensa, teletransporte y fin. El sistema observa transiciones y limita repetición; no debe sustituirse por sonidos en cada actualización de UI.

En `Assets/_Project/Resources/Audio/SWIFT_AudioCues.asset`, los ocho clips y dos bucles están vacíos, `UseTemporaryTones` está activo y no hay AudioMixerGroup asignados. El inventario encontró **cero archivos WAV/MP3/OGG/AIFF y cero AudioMixer** bajo Assets. Los pitidos se generan en memoria; ambiente y música quedan sin contenido. El mixer no es imprescindible para empezar porque ya existen ganancias por categoría.

| Asset propuesto | Prioridad | Carácter y duración orientativa | Obtención recomendada |
|---|---|---|---|
| `SFX_UI_Confirm` | P1 | Clic electrónico suave, 0,08–0,20 s; inequívoco, sin estridencia | Síntesis/edición manual; IA opcional |
| `SFX_UI_Unavailable` | P1 | Dos tonos apagados o clic amortiguado, 0,15–0,30 s; sin castigar al usuario | Síntesis propia |
| `SFX_System_NewAlert` | P1 | Aviso sobrio, 0,25–0,60 s; atención, no sirena ni “fraude confirmado” | Síntesis o IA con edición |
| `SFX_Object_Dossier` | P1 | Papel o clic de carpeta, 0,2–0,5 s | Foley grabado o recurso con licencia |
| `SFX_Object_Evidence` | P1 | Sello suave/papel más confirmación, 0,3–0,6 s | Foley; IA viable |
| `SFX_System_Defense` | P1 | Confirmación ascendente breve, 0,4–0,8 s | Síntesis propia/IA |
| `SFX_UI_Teleport` | P1 | Transición tenue, 0,15–0,4 s; evitar barrido intenso o largo | Síntesis propia |
| `SFX_Result_TurnEnd` | P1 | Cierre neutral, 0,6–1,2 s; no victoria automática por haber terminado | Composición breve/IA |
| `AMB_Office_Night_Loop` | P1 | Aire acondicionado y actividad lejana, 30–60 s, sin conversación inteligible | Grabación licenciada o IA, corregir bucle |
| `SFX_Prologue_Printer` | P2 | Rodillos/motor/papel, 1–3 s; sincronizado con el momento narrativo pertinente | Foley/licenciado; IA con revisión |
| `MUS_Operations_Loop` | P2 | Instrumental discreto, sin voz, pulso lento, 60–90 s; fácil de silenciar | Composición propia o IA con derechos claros |
| `VO_Prologue_ES` | P3 | Narración española breve y tranquila, con subtítulos y pronunciación revisada | Voz humana autorizada o voz sintética licenciada |

P1 significa cerrar el prototipo; P2 aporta acabado; P3 es opcional. Los nombres son propuestas, no recursos ya existentes.

**Entrega técnica recomendada:** masters WAV PCM 48 kHz, 16 o 24 bits; efectos espaciales mono, música/ambiente estéreo cuando corresponda. Sin clipping, silencios innecesarios ni cambios bruscos de volumen. Recortar y comprobar las uniones de los bucles. Unity debe realizar la compresión final: clips breves precargados y bucles largos evaluados para streaming según memoria/dispositivo. No es necesario importar archivos enormes.

UI y señales de sistema deben ser consistentes y comprensibles; objetos pueden usar posición 3D cuando ayude a localizarlos. Audio espacial básico no equivale a un spatializer binaural. Mantener siempre señal visual/textual equivalente. Escuchar con el equipo real del salón y comprobar pausa/reanudación, volumen por categoría y ausencia de avisos repetidos.

**Mezcla propuesta:** ambiente por debajo de los eventos útiles, música aún más discreta durante lectura, sin alarma continua. Un AudioMixer tendría sentido al incorporar controles de volumen, silenciamiento o reducción de música durante voz; no es un requisito pendiente que bloquee asignar clips ahora.

## 7. Arte y recursos: qué generar con IA y qué reutilizar

Ya existen las cinco imágenes de documentos en `Assets/_Project/Resources/DocsImgs`, arte de prólogo y referencias institucionales. La UI principal se construye con TMP y formas nativas; esas referencias no deben convertirse en pantallas completas con texto rasterizado.

| Recurso | Estado / necesidad | Especificación propuesta | IA, Unity o recurso externo |
|---|---|---|---|
| Paneles, pestañas, retícula e iconos funcionales | Ya existen; refinamiento puntual | Bordes visibles, estados claros, geometría ligera; texto TMP | Unity/edición vectorial; IA aporta poco |
| Papel de expediente | Ya refinado | Conservar gris frío, agrupaciones y contraste; sin textura fuerte | Reutilizar |
| Materiales de seis familias | Biblioteca existente | URP/Lit opaco, brillo moderado, materiales compartidos | Reutilizar, no generar duplicados |
| `T_Oak_Subtle_512` | Opcional | 512², albedo seamless, veta amplia de bajo contraste | IA viable; revisar tiling y UV |
| `T_Fabric_Subtle_256` | Opcional | 256², trama poco marcada; comprobar moiré | Procedural preferible; IA posible |
| `ART_Wayfinding_Atlas` | Mejora posible | Atlas 512–1024² con símbolos institucionales; rótulos separados en TMP | Diseñar en Unity/vector; IA solo referencia |
| `ART_Office_Poster` | Solo si falta variedad útil | 1024² máximo inicial, ilustración limpia sin texto ni marcas | IA viable; no reemplazar póster existente sin necesidad |
| Prólogo adicional/corregido | No prioritario; ya hay imágenes | Mantener personajes, composición y estilo entre escenas; texto externo | IA viable, revisión de coherencia |
| `T_Exterior_Silhouette` | Opcional | Panorama estilizado de contraste bajo; materiales opacos | IA posible o geometría simple existente |
| Manos visibles | Dependencia de hardware, no de ilustración | Malla riggeada compatible con esqueleto del proveedor; material mate | Preferir sample oficial/licenciado; IA solo concepto de estilo |
| Audio de la tabla anterior | Falta producción final | Variantes coherentes y edición final | Síntesis, foley, recursos licenciados o IA |

No recomiendo generar con IA textos de cuenta, cifras, decisiones, iconos pequeños con precisión obligatoria ni una malla de mano como sustituto del seguimiento. Una imagen de una mano no proporciona articulaciones, poses, pinch ni interacción.

### Prompts listos para producción posterior

**Madera:** “Textura albedo seamless de roble de oficina desaturado, veta amplia y tenue, iluminación uniforme, sin sombras, sin reflejos, sin perspectiva, sin juntas, sin texto ni marcas; estilo realista simplificado compatible con oficina low-poly, contraste bajo”. Exportar y revisar a 512², incluyendo repetición 3×3.

**Póster institucional:** “Ilustración corporativa de red de transferencias y revisión documental, geometría sencilla, azul petróleo, cian suave y marfil frío; composición sobria con espacios libres; sin palabras, números, logotipos reales, neón ni elementos hacker”. Añadir después cualquier título mediante TMP.

**Ambiente:** “Ambiente de oficina bancaria nocturna tranquila: ventilación suave constante y actividad mecánica muy lejana, sin voces inteligibles, sin música, sin alarmas, sin golpes cercanos, perspectiva estable, preparado para bucle discreto de 45 segundos”. Revisar unión, ruido y volumen; no asumir que la generación ya entregó un bucle perfecto.

**Música:** “Instrumental corporativo sobrio para simulación educativa de análisis, pulso lento, texturas cálidas discretas y pocos instrumentos; sin voz, melodía protagonista, percusión agresiva, sobresaltos ni imitación de artistas; versión continua de 75 segundos con transición de bucle”. Mantenerla opcional.

**Confirmación:** “Efecto único de confirmación de terminal institucional, clic breve con tono suave, 150 milisegundos aproximados, seco, sin reverberación larga, sin ruido de fondo, sin voz”. Para señales tan simples suele ser más controlable la síntesis manual.

Por cada recurso final registrar herramienta/autor, fecha, prompt si aplica, fuente, términos de uso y edición realizada. “Generado con IA” no significa automáticamente CC0 ni elimina la necesidad de revisar derechos. No imitar artistas ni copiar recursos de los juegos analizados.

## 8. Referentes investigados y qué aprender de ellos

Búsqueda realizada el 8/11/2026. Se priorizaron páginas de desarrolladores y documentación oficial. **Las lecciones visuales de esta tabla son propuestas de diseño para SWIFT**, no pruebas de que una característica produzca mejores notas ni una evaluación basada en haber jugado todos los títulos durante esta sesión.

| Referente y naturaleza | Qué aporta | Qué trasladar a SWIFT | Qué evitar |
|---|---|---|---|
| **Papers, Please**: juego de inspección documental; no VR ni simulador bancario | Convierte documentos, herramientas y decisiones en el centro de la experiencia | Identidad documental consistente; comparación clara; feedback de consecuencias. La fuente oficial describe decisiones a partir de documentos y herramientas de inspección | Copiar pixel art, documentos pequeños, marcas, música o reglas migratorias. [Presskit oficial](https://papersplea.se/presskit/) |
| **HoloLAB Champions**: simulador educativo de química en VR | Práctica de procedimientos en un entorno diseñado para aprender mediante acciones | Objetivos concretos, práctica segura, revisión posterior y tarea de clase alrededor de la experiencia | Trasladar competición por velocidad a la lectura Cardboard. [Schell Games](https://schellgames.com/games/hololab-champions) |
| **Job Simulator / Vacation Simulator**: entretenimiento VR de interacción con objetos | Referencia de presencia manual y respuesta tangible; Owlchemy documenta versiones completas con manos en Quest | Siluetas claras, objetos con función comprensible y respuesta inmediata. Para SWIFT, una carpeta o control bien resuelto vale más que muchas decoraciones interactivas | Suponer que su soporte de manos funciona en Cardboard o copiar su tono caricaturesco. [Owlchemy](https://owlchemylabs.com/blog/its-time-to-job-with-human-hands) |
| **I Expect You To Die**: puzle de entretenimiento VR | Experiencia concebida para jugar sentado, con acceso a objetos distantes | Diseñar el puesto alrededor del alcance y la comodidad; usar entorno y sonido para orientar | Añadir peligro, movimiento forzado o copiar su estética de espías. [Schell Games](https://schellgames.com/games/i-expect-you-to-die) |
| **Labster**: simulaciones educativas 3D | Vincula tareas científicas, narrativa y evaluación | Explicar propósito, dar feedback contextual y conectar resultado con concepto | Presentarlo como solución VR actual: su soporte oficial indica laboratorios web y pausa de VR desde 2021. [Qué es Labster](https://help.labster.com/en/articles/3905261-what-is-labster), [plataformas](https://help.labster.com/en/articles/1077008-can-my-device-run-labster-simulations) |

Lo que puede hacer que estos referentes se perciban mejor acabados es la coherencia entre **lo que se ve, lo que se puede hacer y la respuesta que se recibe**. Para SWIFT propongo aplicar ese criterio: el acento visual señala una función, la función produce una respuesta comprensible y el resultado explica el aprendizaje. Más polígonos o reflejos no solucionan una orientación confusa.

La diferencia pendiente de SWIFT no es solamente “faltan manos”. También falta probar si el alumno comprende la tarea sin instrucciones del desarrollador, si distingue evidencia de conclusión y si puede explicar por qué la defensa cambia el resultado.

## 9. Manos en el salón: alternativas y límites

**Hardware aún no confirmado.** Se solicitó modelo de visor, mandos/sensor y disponibilidad de PC. Hasta identificarlo no corresponde prometer compatibilidad ni fijar versiones de paquetes. Las rutas siguientes son propuestas, no integraciones realizadas.

### 9.1 Elegir la ruta correcta

| Equipo disponible | Qué es viable | Recomendación |
|---|---|---|
| HONOR + Cardboard | Mirada, retícula y botón del visor según soporte; no aporta seguimiento nativo de dedos | Entrega principal actual. No prometer manos rastreadas |
| Visor con mandos 6DoF compatibles | Posición/orientación de mandos, rayos y selección; manos animadas aproximadas | Primera variante de aula si el equipo ya existe; distinguir animación de dedos de seguimiento real |
| Meta Quest con seguimiento de manos compatible | Articulaciones, gestos y selección según runtime/proveedor | Prototipo aislado OpenXR + XR Hands y una capa de interacción, o SDK del fabricante validado |
| Leap Motion / Ultraleap + PC | Seguimiento óptico de manos mediante sensor y software compatible | Probar primero sample del proveedor. En pantalla de PC es una demostración de manos, no automáticamente una experiencia VR |
| Otro visor, guantes o dispositivos hápticos | Depende de SDK, runtime, calibración y licencia concretos | Pedir modelo exacto y demostrar sample oficial antes de planificar integración |
| Cámara normal del teléfono + visión artificial | Investigación adicional; oclusión, latencia, encuadre y consumo complican VR | No recomendable para cerrar esta entrega |

La guía oficial de Cardboard describe render estéreo, orientación de cabeza y botón del visor, no un proveedor de articulaciones de manos. Añadir XR Hands tampoco crea esa capacidad: Unity exige hardware compatible y un proveedor que entregue datos. [Cardboard](https://developers.google.com/cardboard/develop/unity/quickstart), [XR Hands](https://docs.unity3d.com/Packages/com.unity.xr.hands@1.7/manual/index.html).

### 9.2 Integración propuesta para este código

La UI actual usa **colliders y `BankInteractable.Activate()`**, con foco/progreso ligado a `VRInteractionManager`. No basta con añadir un XR Ray Interactor y esperar que todos los controles se conviertan en botones XRI.

Ruta recomendada, por etapas:

1. **Inventario de aula:** visor, modelo de mandos/sensor, PC/GPU/SO, conexión, runtime, permisos de instalación y espacio disponible. Si son guantes, confirmar qué datos entregan: orientación, flexión de dedos, posición absoluta y/o háptica.
2. **Escena de laboratorio separada:** sample oficial de manos o mandos funcionando con Unity y URP. Ver dos manos, detectar selección y probar pérdida/recuperación de seguimiento. No empezar en la oficina completa.
3. **Proveedor de entrada:** adaptar rayo de mando o mano a los colliders existentes. El evento seleccionado llama a `BankInteractable.Activate()`; nunca modifica directamente `TransactionManager`.
4. **Estado visual compartido:** ofrecer a DocumentFocus/retícula un estado de objetivo, foco, selección y progreso. Para mirada, conservar el contador real actual. Para mando/pinch, mostrar foco y confirmación según la entrada real, sin simular una carga de 1,5 s que no existe.
5. **Un dueño de selección:** elegir explícitamente mirada, mando o mano. Impedir que mirada y pinch activen el mismo botón. Rearmar al liberar gatillo/pinch, cancelar al perder tracking y evitar acciones al recuperar una mano todavía cerrada.
6. **Probar una estación:** entrar, consultar, aprobar/retener/devolver, ampliar notas y salir. Después conectar portal y destinos de teletransporte existentes.
7. **Variante de compilación propia:** Cardboard y OpenXR no deben arrancar dos rigs/cámaras ni proveedores simultáneos. Separar configuración y condiciones de compilación; mantener idéntico contenido bancario. Una APK Android para Quest no es intercambiable con la APK Cardboard del HONOR.
8. **Regresión cruzada:** ejecutar las mismas decisiones y comprobar resultados equivalentes con las tres entradas disponibles. Conservar la mirada como alternativa explícita cuando corresponda.

Si en una fase posterior se migra a botones uGUI estándar, Unity ofrece XR UI Input Module y Tracked Device Graphic Raycaster. Eso requiere adaptar eventos y arbitraje; no es necesario migrar todo para la primera prueba con el sistema de colliders existente. [Documentación oficial XRI](https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.1/manual/ui-components.html).

### 9.3 Si el equipo es Ultraleap

La documentación requiere cámara de seguimiento, computadora compatible, software de tracking y plugin de Unity; para XR recomienda OpenXR cuando esté soportado. También advierte que algunos materiales de ejemplo requieren adaptación a URP y proporciona modelos de manos URP. Primero comprobar “Capsule Hands” o “Hello Hands”, después la interacción del banco. [Preparación oficial](https://docs.ultraleap.com/xr-and-tabletop/xr/unity/getting-started/index.html), [Hello Hands](https://docs.ultraleap.com/xr-and-tabletop/xr/unity/getting-started/your-first-project.html).

En nuestra propuesta hay que decidir montaje sobre mesa o en visor, alinear el origen del sensor con el espacio de juego y verificar alcance. Un sensor fijo sobre mesa no sigue la cabeza por sí mismo. No asumir que un cable USB al HONOR proporciona soporte Android: verificar explícitamente combinación de dispositivo, sistema y SDK antes de comprometer esa ruta.

### 9.4 Qué deberían hacer las manos

Primera versión: apuntar con rayo y confirmar mediante gatillo o pinch. Mantener botones grandes y no exigir precisión de dedo sobre el monitor lejano. Una segunda etapa puede permitir tocar una carpeta próxima, tomar la libreta o pulsar un control físico. No es necesario implementar escritura, teclado virtual, brazos completos, física compleja ni manipulación libre de cada documento.

Manos visuales mate, de silueta limpia y compatibles con el esqueleto del SDK. Ocultarlas o atenuarlas al perder seguimiento y dar aviso discreto. Evitar manos rígidas pegadas a la cámara que se presenten como rastreadas. El coste de dos mallas deformadas y sus interacciones debe medirse, especialmente si se añaden físicas.

**Prueba de aceptación propuesta:** diez selecciones consecutivas por mano con cero dobles activaciones; liberar/rearmar correctamente; retirar las manos del campo de seguimiento sin decisiones accidentales; recuperar tracking sin salto ni acción; completar ambos modos; verificar comodidad sentado y salida accesible. No afirmar que una prueba PC con ratón verifica dedos, oclusión o calibración.

## 10. Plan priorizado y criterios de cierre

Estimaciones orientativas de trabajo efectivo para alguien que conoce el proyecto: **S = hasta 1 jornada; M = 2–4 jornadas; L = 5 o más**. No son fechas de entrega; fallos de dispositivo, licencias y disponibilidad de equipo pueden ampliar el esfuerzo.

| Orden | Tarea | Esfuerzo | Se considera cerrada cuando… |
|---|---|---|---|
| P0-1 | Consolidar inventario actual y requisitos del docente | S | Se distingue implementado/validado/pendiente; destinos y checklist corresponden al banco actual |
| P0-2 | Generar APK nueva conservando las anteriores | S, si compila | Build correcto, banco inicial, versión identificable, logs y hash, archivo instalable |
| P0-3 | Recorrido HONOR/Cardboard | M | Ambos ojos, giro, QR/recentrado, prólogo, tutorial, decisiones, destinos activos y pausa funcionan; incidencias registradas |
| P1-1 | Producir e integrar ocho señales + ambiente | M | Clips con licencia y volumen revisados, sin spam, eventos correctos y pausa estable |
| P1-2 | Ensayo con alumnos nuevos y guía de clase | S–M | Se registra ayuda, errores de selección y comprensión; se corrigen los bloqueos repetidos |
| P1-3 | Jerarquía de luz, señalética y limpieza visual | M | Comparación en mismos puntos y aceptación en visor; sin pérdida de lectura ni rendimiento |
| P1-4 | Explicación de decisiones y defensa | M | El jugador distingue su acción del control y entiende formato frente a revisión, sin cambiar puntuación |
| P1-5 | Medición de rendimiento | M | Presupuesto de frame acorde al dispositivo, sin caídas sostenidas durante sesión representativa; CPU/GPU, memoria y calentamiento registrados |
| P2-1 | Sample de herramientas de manos del salón | S–M tras conocer equipo | Sample oficial funciona físicamente, incluida pérdida/recuperación de tracking |
| P2-2 | Adaptador de entrada y estación con manos/mandos | L | Mismo banco y resultados, sin doble entrada, selección fiable; regresión Cardboard aprobada |
| P2-3 | Música y sonido del prólogo | S–M | Apoyan lectura/narrativa, pueden silenciarse y no tapan feedback |
| P3 | Texturas y arte adicional opcional | S–M | Resuelve un problema visible comprobado; no aumenta ruido ni coste injustificado |

Si la demostración es cercana, cerrar P0 y P1 de audio/usabilidad antes de P2 de manos. Si el profesor exige manos, elevar primero el sample de hardware a P0 para detectar incompatibilidades pronto; no rehacer toda la aplicación antes de esa prueba.

## 11. Plan de prueba en el salón

**Preparación:** identificar versión/hash, cargar dispositivo, comprobar el equipo real, disponer de una silla estable y una persona de apoyo; preparar una copia local del video y una demostración de PC como respaldo. La prueba debe poder funcionar sin descargas de último minuto.

**Guion:** portal → prólogo u omisión → entrar al puesto → leer y consultar → resolver un expediente de formato y uno con alerta → resultados de ataque → defensa → comparación y reflexión. Para una exposición de siete minutos conviene ensayar tiempos reales; no prometer mostrar seis órdenes dos veces sin medir cuánto tarda la lectura por mirada.

**Registro mínimo por participante:** dispositivo, build, entrada usada, duración, pasos que requieren ayuda, activaciones accidentales, textos difíciles y comentarios de comodidad. Puede ser anónimo. Propuesta inicial: 3–5 usuarios nuevos como ensayo de usabilidad; ese tamaño no demuestra eficacia educativa general.

**Preguntas finales:** ¿por qué se devuelve un formato incompleto?, ¿por qué una coincidencia requiere revisión y no prueba fraude?, ¿qué cambió la defensa?, ¿qué cifras corresponden a las órdenes ficticias y cuáles al contexto histórico?

**Rendimiento:** medir una sesión representativa de 10–15 minutos y repetir después de calentamiento. El objetivo se fija según frecuencia real del visor/teléfono; no se declara un FPS universal ni se usa una captura PNG como benchmark.

**Entregables académicos pendientes de comprobar:** APK definitiva, repositorio reproducible con contribuciones del equipo, video de 2–3 minutos desde celular, ficha de máximo cuatro páginas, roles, créditos completos y material particular del escenario. Estos requisitos proceden del contexto local; confirmar cualquier cambio reciente con el docente.

## 12. Evidencia visual y límites finales

Imágenes abiertas durante esta auditoría:

- [Oficina general, GraphicsPolishV2](../Previews/GraphicsPolishV2/After/02-general.png): arquitectura y composición anterior a la última UI.
- [Puesto, GraphicsPolishV2](../Previews/GraphicsPolishV2/After/04-puesto.png): materiales y proporciones; su interfaz fue reemplazada posteriormente.
- [Portal refinado](../Previews/PortalFinalReview/20261103/01-front-improved.png).
- [Prólogo con portal visible](../Previews/PortalFinalReview/20261103/09-prologue-main-visible.png).
- [Expediente gris papel aplicado](../Previews/DossierTreatment/20261104/after/02-active-main.png).

No se generaron capturas nuevas en esta sesión. Para trazabilidad del estado inspeccionado: escena SHA-256 `89520893a03420fe75c5f9fc4dfbf1a93f21cb783f1f1f82042071caba0c4c2b`; `BankDocumentUI.cs` SHA-256 `994657d8f78accf6fcf4d3072da5d693ad65d257bd82511765a51468ad8af743`. Estos hashes no certifican que todas las dependencias coincidan con una galería anterior.

**Conclusión propuesta:** conservar el núcleo y la oficina actuales, cerrar la versión de dispositivo, sustituir los tonos temporales, ordenar la orientación y hacer más explícito el aprendizaje. Después, probar una estación con las herramientas de manos reales del salón mediante un proveedor separado. El objetivo viable es una experiencia corta, coherente y comprobada; la expansión de contenido queda por detrás de ese cierre.
