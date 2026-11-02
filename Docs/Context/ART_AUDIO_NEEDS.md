# Auditoría visual y de audio — SWIFT: Código de Alerta

Fecha: **2 de noviembre de 2026, America/Lima**. Proyecto principal: `D:/UnityGames/SWIFT_CodigoDeAlerta`. Unity 6000.3.24f1 / URP 17.3.0. Rama `review/unity-scene-designer`, base `3bef77b`, con cambios locales del equipo conservados.

## 1. Diagnóstico ejecutivo

**El problema principal es la falta de una dirección visual uniforme y de jerarquía, más que la calidad de los modelos.** Ya hay una oficina, mobiliario suficiente, iluminación horneada y una interfaz funcional. La presentación todavía combina una sala amplia y poco articulada, muebles con colores y detalle dispares, carteles de prototipo, un panel flotante enorme y feedback audiovisual mínimo.

La mejora con mayor retorno es **ordenar composición, escala aparente, materiales, luz y tipografía con lo que ya existe**. No hace falta reconstruir la oficina, comprar un pack nuevo ni generar toda la interfaz con IA. La IA conviene para unas pocas ilustraciones o superficies 2D coherentes; botones, retícula, iconos básicos y estados deben seguir siendo recursos controlables en Unity.

En audio, el diagnóstico es más directo: **no se localizaron archivos de sonido, música o AudioMixer en Assets**. Existe un tono sintetizado en ejecución; no existe todavía un diseño de sonido por acción, espacio o momento narrativo.

Esta sesión es una auditoría: se crearon documento, evidencia de inspección y capturas. No se modificaron escenas, materiales, luces, scripts, URP, Cardboard ni lógica bancaria. No se generó arte o audio, no se descargaron recursos y no hubo commit/push. Las propuestas siguientes **no están implementadas ni aprobadas visualmente por el usuario**.

## 2. Evidencia real y límites

Se aplicó `.agents/skills/unity-scene-designer/SKILL.md`, se revisaron los documentos de contexto/diseño, créditos, informe de oficina, revisión visual y validación funcional posterior. Se inspeccionaron BancoCorresponsal, modelos/prefabs, materiales, luces, perfiles URP, UI/Propuesta C, scripts de sonido, prólogo y retícula.

### Capturas nuevas de esta auditoría

Galería: [Before-20261102T140647380Z](../Previews/LightingReview/Before-20261102T140647380Z/README.md).

| Evidencia | Qué se revisó |
|---|---|
| [01-oficina.png](../Previews/LightingReview/Before-20261102T140647380Z/01-oficina.png) | Entrada, jerarquía general, techo, contraste y foco. |
| [02-escritorio.png](../Previews/LightingReview/Before-20261102T140647380Z/02-escritorio.png) | Proporciones de computadora/panel, materiales y papeles sobre teclado. |
| [03-computadora-expedientes.png](../Previews/LightingReview/Before-20261102T140647380Z/03-computadora-expedientes.png) | UI en modo Edición, espacio vacío, señalética y legibilidad relativa. |
| [04-distribucion-superior.png](../Previews/LightingReview/Before-20261102T140647380Z/04-distribucion-superior.png) | Distribución y grandes superficies libres; techo y luminarias ocultos temporalmente solo para esta vista. |
| [05-iluminacion.png](../Previews/LightingReview/Before-20261102T140647380Z/05-iluminacion.png) | Luminarias, gradientes en paredes, kioscos, pads y relación entre zonas. |

Son renders nuevos del Editor principal, perfil **PC**, Direct3D11. La etiqueta Before es el nombre que usa el capturador existente, no implica que se haya aplicado un cambio después. Se utilizó su cola local solo con `inspect` y `before`; no se ejecutaron apply, refine, bake, checkpoint o regeneradores.

SHA256 de escena: `41467b15703bff8b588c7f36c6a76c684483b1f44ca3a315074961b59c3c3d92`, coincidente con la escena conservada en la sesión funcional. Inspección del Editor: una escena, `dirty=False`, `playing=False`; un lightmap, un grupo de light probes y cero reflection probes. Recibos/hashes en `capture.json` de la galería y `Artifacts/ArtAudioAudit-20261102/`.

También se abrieron las cuatro vistas de After-20261031T010216621Z, la referencia `Assets/_Project/Styles/OperationsCenter_Original.png`, la captura de resultados `Docs/Previews/LightingReview/Play/resultados.png` y la captura runtime **posterior a la corrección del panel**, `Artifacts/Functional-20261102-083259/office-inspection.png`.

**No confundir estados:** las capturas nuevas son de Edición y muestran el menú, no expedientes activos. La corrección de UIManager recoloca el panel al comenzar Play; la captura runtime anterior demuestra ese estado distinto. La galería que declara TrialProject no se tomó como evidencia del principal ni se copiaron sus recursos.

No se realizó audición del juego en HONOR, captura estéreo, prueba de confort, medición de FPS o comparación de brillo del teléfono. El diagnóstico de audio se basa en código y archivos; el juicio estético se basa en las imágenes abiertas. Las cámaras de revisión desactivan AA y postprocesado: los dientes de sierra de estos PNG **no prueban por sí solos un defecto equivalente en el dispositivo**.

## 3. Qué está haciendo que se vea «feito»

### Iluminación

La luz **sí está horneada**. El Editor confirma seis spots de techo baked con intensidad 9.5, un spot baked de inspección a 6.5 y una direccional activa a 0.44. La direccional antigua «Luz principal» está inactiva. No son nueve luces dinámicas activas.

Lo que falla visualmente es la jerarquía: las luminarias se repiten en una retícula regular, las paredes y suelo reciben un relleno amplio y el puesto de análisis no queda separado con suficiente claridad del resto. En varias paredes aparecen manchas/gradientes de luz sin un detalle arquitectónico que les dé contexto. Las luminarias se leen como placas blancas casi uniformes contra un techo muy oscuro. Hay sombras y contacto, pero no bastan para dar variedad material o dirigir la mirada.

Propuesta: trabajar el contraste **entre zonas**, manteniendo escritorio y recorrido legibles, con ambiente más discreto y un área de trabajo cálida coherente con las luminarias. Variar orientación, alcance y reparto de las luces existentes y volver a hornear después de aprobar composición. No oscurecer toda la sala para fabricar «tensión» ni aumentar todas las intensidades. Los números actuales no deben transformarse en una receta fija: el objetivo se evalúa en las mismas vistas y en el visor.

### Materiales y colores

Hay 35 materiales en OfficeVisual. Sus smoothness se concentran en **0.10, 0.12 y 0.16**. Ejemplos de madera, pared, suelo y supuesto latón tienen Metallic 0. En los PNG muchos objetos se perciben con una respuesta mate parecida. Esto es válido para low-poly, pero aquí reduce la distinción entre metal pintado, madera, plástico y papel.

Los archivadores naranja claro y el sofá rojo destacan más que su función secundaria. El escritorio marrón, los verdes de pads, el azul del banco y esos acentos no siguen todavía una pauta común. La computadora incluye una textura con detalle más fino que muebles y arquitectura: el contraste de estilos se nota cerca del teclado y torre.

Propuesta: conservar mallas y materiales URP compatibles, reducir saturación de los muebles secundarios, compartir una madera y dos familias de plástico/metal pintado, y reservar los acentos para acciones. No poner Metallic alto a todo: un archivador pintado no debe convertirse en cromo. No hace falta cubrir todas las superficies con texturas PBR grandes.

No se observó material rosa en las vistas inspeccionadas. La ausencia de reflection probes no se clasifica como error: con una dirección predominantemente mate puede seguir siendo opcional.

### Distribución y escala aparente

La sala ocupa aproximadamente 10 × 10 m según los bounds del suelo. El escritorio concentra casi toda la actividad y el resto de los objetos se coloca en el perímetro. La planta parece una exposición de piezas aisladas: grandes espacios vacíos, pads verdes sin integración gráfica y kioscos de selección delante del escritorio.

Al acercarse, la computadora/torre y el panel tienen mucho peso visual. La UI funciona como una pantalla de presentación flotante delante del monitor, no como una parte integrada del puesto. La pila de papeles invade el teclado. Los carteles se parecen a etiquetas de depuración; su tamaño, soporte y densidad no forman una familia.

Propuesta: ordenar el escritorio, agrupar visualmente archivo/impresora y seguridad, usar señalética coherente y dar estructura al suelo con recursos discretos. Reubicar mobiliario ya disponible antes de añadir más. Mantener circulación libre y los cuatro destinos; no llenar cada hueco con props ni mover pads sin volver a comprobar los raycasts.

### Interfaz y Propuesta C

La Propuesta C **sí está integrada como adaptación nativa**, mediante UIManager y OperationsCenterStyle: azul noche, bordes cian, tres colores de decisión, textos TMP, siete prefabs y una pantalla funcional. No es una integración de seis pantallas artísticas completas.

Los 12 PNG extraídos de la referencia existen, pero son **recortes RGB opacos de referencia**; sus importadores tienen textureType 0/spriteMode 0. No son sprites transparentes funcionales enlazados a estados. Volver a generar botones casi iguales con IA no arreglaría el problema actual.

Problemas observados: expediente en un bloque continuo de texto, gran espacio vacío en menú, cabecera extensa, instrucciones de PC mezcladas con VR y resultados con historial, lista de decisiones, comparación y créditos apretados en una misma región. El contraste principal blanco/azul noche es bastante claro en PC; la debilidad es la jerarquía, densidad y distancia de lectura, no una falta general de contraste.

Los botones tienen color y etiqueta, pero no muestran una familia completa de foco, progreso de mirada, pulsación y deshabilitado. La retícula es una pequeña esfera cian que aumenta de tamaño al enfocar; no comunica cuánto falta de los 1.5 segundos ni que hay que apartar la mirada para rearmar.

Propuesta: campos alineados y agrupados, monto/modo/estado claramente diferenciados, resultados por bloques y estados de interacción consistentes. Usar icono + texto + color. No hornear texto dinámico en imágenes ni añadir pestañas sin contenido.

**Preservar la corrección funcional:** el collider de la computadora bloqueaba los botones y UIManager ahora mantiene el panel por delante durante Start. Cualquier cambio de escala, posición o integración del monitor debe conservar ese despeje y repetir la prueba desde inspección. «Hacer el panel más pequeño» sin cuidar tamaño angular y collider puede empeorar la experiencia.

### URP y nitidez

Android tiene calidad Mobile asignada; el Editor inspeccionado usa PC. Mobile_RPAsset conserva renderScale **0.8**, MSAA **1** y HDR activado; Mobile_Renderer no tiene renderer features añadidas. PC tiene renderScale 1. Estos datos explican por qué no se puede aprobar la nitidez móvil viendo solo la galería PC.

No propongo cambiar URP ahora. Después de resolver composición y UI, hacer una comparación controlada en HONOR antes de modificar renderScale o AA. No añadir SSAO, bloom, volumétricos, reflejos en tiempo real o transparencias extensas como solución a una escena mal compuesta.

## 4. Clasificación de problemas

CRÍTICO significa prioritario para lectura/orientación/feedback de la experiencia, no que se haya demostrado un crash. Cuando depende del HONOR se marca como riesgo pendiente de validar.

| ID | Gravedad | Problema y evidencia | Corrección propuesta | Aceptación observable |
|---|---|---|---|---|
| V01 | CRÍTICO | Jerarquía/lectura de UI: texto corrido y resumen final denso; UIManager, captura runtime y resultados | Separar campos, métricas, comparación e historia; tipografía y espaciado uniformes | Se identifica modo, monto, estado y acción sin recorrer todo el párrafo; en visor no hay recortes ni necesidad de inclinarse |
| V02 | CRÍTICO | Foco de mirada poco informativo: retícula solo cambia de tamaño y botones no tienen estados completos | Contorno de foco, anillo de progreso y confirmación breve con señal de rearme | Se distingue mirar, esperar y haber activado; la señal sigue visible sobre fondos claros/oscuros |
| V03 | CRÍTICO — riesgo por validar | Escala/profundidad del panel e integración con monitor; panel muy dominante de cerca, señalética pequeña de lejos | Ajustar por tamaño angular y destino de inspección, preservando despeje del collider | Tres botones y datos esenciales visibles desde el pad sin oclusión; raycasts y lectura binocular aprobados |
| V04 | IMPORTANTE | Luz de oficina con poca jerarquía; retícula de luminarias y relleno parejo | Reequilibrar luces existentes y bake por zonas | El puesto se reconoce como foco y las zonas secundarias siguen legibles |
| V05 | IMPORTANTE | Paredes/suelo/techo con poca separación perceptual; algunas zonas oscuras y emisores blancos planos | Balance de valores y materiales; difusores con detalle geométrico simple | Contactos y siluetas distinguibles; sin convertir el techo en punto de atención principal |
| V06 | IMPORTANTE | Colores secundarios compiten: archivo naranja/sofá rojo/pads verdes | Paleta reducida, saturación controlada y acentos funcionales | Mobiliario de fondo no roba atención a UI/objeto enfocado |
| V07 | IMPORTANTE | Materiales de respuesta similar y mezcla de detalle | Familias de madera/plástico/metal/papel; ajustes locales de smoothness | Material reconocible por respuesta y color sin texturas pesadas |
| V08 | IMPORTANTE | Oficina amplia con objetos aislados y pads de prototipo | Agrupar zonas y reutilizar props disponibles; señalización de suelo discreta | Circulación clara y composición con profundidad, conservando cuatro destinos |
| V09 | IMPORTANTE | Papeles sobre teclado y proporciones visuales poco cuidadas | Ordenar objetos del escritorio y revisar escala relativa | Teclado despejado, documentos con apoyo visible, nada flotando o atravesado |
| V10 | IMPORTANTE | Señales y kioscos no forman una identidad consistente | Una familia de soportes, rótulos e iconos; títulos cortos | Entrada, archivo, inspección y seguridad reconocibles desde sus puntos de uso |
| V11 | IMPORTANTE | Prólogo sigue siendo un storyboard de rectángulos, según IntroSequence | Acabado con el modelo de impresora existente o 3 ilustraciones coherentes | Banco de origen claramente separado del puesto; sin tapar omitir/subtítulos |
| V12 | CRÍTICO audiovisual | El mismo tono para cualquier Changed; sin distinción de error/confirmación/riesgo | Feedback por intención y prioridad, con control de repetición | Una acción relevante produce una señal comprensible; cambios de texto no generan ruido gratuito |
| V13 | IMPORTANTE | Sin ambiente ni paisaje sonoro espacial localizado | Ambiente discreto y fuentes puntuales para objetos | La sala se siente habitada sin interferir con lectura o indicaciones |
| V14 | OPCIONAL | Falta de textura sutil y detalles de identidad | Papel, gráfica institucional y acabados discretos | Mejoran cercanía sin ruido visual ni nueva confusión de interacción |
| V15 | OPCIONAL | Música y firma sonora de resultados | Capa musical contenida y dos cierres breves | Refuerzan tono sin obligar a oír música para comprender el resultado |

## 5. Dirección artística recomendada

**«Operaciones bancarias nocturnas, sobrias y humanas»**: oficina institucional low-poly, madera moderada, metal pintado, papel claro y señalética funcional. Tensión por revisión y contraste, no por estética hacker, neón, virus o alarma permanente. Conservar separación entre hechos históricos y defensa educativa.

Paleta propuesta como guía de diseño, no como valores ya aplicados:

| Uso | Color orientativo |
|---|---|
| Base UI/institucional | Azul noche `#101B2D` |
| Superficies UI secundarias | Gris azulado `#243746` |
| Pared clara | Blanco cálido `#D5D2C9` |
| Suelo / metal pintado | Pizarra `#3B4851` |
| Madera | Marrón desaturado `#765A43` |
| Texto principal | Marfil claro `#E9EFF3` |
| Foco / orientación | Cian moderado `#2AA8B2`, con contorno claro y oscuro cuando corresponda |
| Atención / modo ataque | Ámbar `#C28B30` |
| Defensa / aprobación | Verde petróleo `#0D695E` |
| Devolución por formato | Azul `#2C6FA3` |

No usar el mismo color como único significado de «interactivo», «defensa» y «correcto». Mantener etiquetas e iconos. La devolución por formato no debe parecer automáticamente un delito o fallo grave. La cabecera del modo puede variar de acento sin recolorear toda la oficina.

Tipografía: continuar con TMP/fuente existente; jerarquía de título, etiqueta, valor y nota. Evitar texto fino, párrafos largos, bordes de un solo píxel e iconos llenos de detalle. Conservar contraste estático; no depender de bloom para leer un borde. No fijar un tamaño de fuente como garantía VR sin considerar distancia y escala del Canvas.

## 6. Inventario de assets: qué falta y cómo producirlo

**Todos los nombres siguientes son propuestas de producción, no archivos ya existentes.** Los recursos actuales reutilizables son: modelos de oficina, materiales URP, fuente TMP, referencia Propuesta C, 12 recortes y siete prefabs OC. «Crear en Unity» puede ser una mejora de un recurso actual, sin necesitar una nueva imagen externa.

Prioridades: P0 lectura/feedback, P1 coherencia y acabado principal, P2 enriquecimiento opcional.

### UI, iconos, retícula y efectos

| Nombre propuesto | Categoría / prioridad | Para qué sirve y estado real | Producción recomendada |
|---|---|---|---|
| UI_OC_FieldRow | UI / P0 | Etiqueta/valor consistentes para origen, beneficiario, cuenta, monto y fecha; hoy son un texto continuo | **Unity UI/TMP**, reutilizando OC_Expedientes; sin fondo IA nuevo |
| UI_OC_ModeStatus | UI / P0 | Modo, expediente actual y controles activos destacados | **Unity**, formas y textos dinámicos |
| UI_OC_ResultCards | UI / P0 | Separar pérdida ficticia, decisiones, comparación e historia; contenido ya existe | **Unity**, reorganizar presentación sin cambiar cálculo |
| UI_OC_ButtonStates | UI / P0 | Normal, foco, pulsado y no disponible; hoy faltan estados completos | **Unity**, materiales/imágenes simples y contornos; recortes actuales solo referencia |
| ICON_OperationsSet | Iconos / P1 | Check, revisión/lupa, devolver, archivo, evidencia, alerta, escudo, continuar, omitir y reiniciar | **Crear formas/vector controlado y rasterizar si hace falta**; 10 iconos coherentes. No conviene IA para la versión funcional pequeña |
| RET_GazeProgress | Retícula / P0 | Centro contrastado + anillo de espera + confirmación/rearme; hoy solo esfera cian | **Unity**, geometría/UI procedural; sin IA ni plugin nuevo |
| FX_ObjectFocus | Efectos interacción / P0 | Identificar objeto enfocado además de la retícula | **Unity**, indicador localizado/contorno simple; evitar efecto global caro |
| FX_ActionConfirm | Efectos interacción / P1 | Señal de pulsación/acción completada, breve y sin animar cámara | **Unity**, cambio visual suave de 100–200 ms como punto inicial a validar |
| FX_TeleportMarker | Efectos interacción / P1 | Sustituir lectura de «cuadrado verde» por destino reconocible conservando el pad | **Unity**, aro, símbolo y etiqueta corta; sin partículas ni transparencia extensa |
| UI_ZoneSigns | UI / P1 | Misma familia de títulos para entrada, inspección, archivo y seguridad | **Unity/TMP**; mantener texto editable, soportes reutilizados |
| UI_CreditsLayout | UI / P1 | Dar lectura a atribuciones existentes sin apretarlas en resultados | **Unity/TMP**, formato legible del contenido ya documentado |

### Props/arte 2D y prólogo

| Nombre propuesto | Categoría / prioridad | Para qué sirve y estado real | Producción recomendada |
|---|---|---|---|
| ART_FictionalPaperSurface | Props 2D / P1 | Papel/documento con estructura discreta; actualmente hay pilas 3D, no una familia gráfica propia de expediente | **IA opcional**, 1 superficie de 1024² sin texto; campos y sello SIMULACIÓN añadidos aparte con Unity |
| ART_FolderCover | Props 2D / P2 | Portada de carpeta para identidad documental | **Diseño simple o IA**, 1024²; misma paleta. No requiere un modelo nuevo si se reutiliza geometría disponible |
| ART_BankWallGraphic | Props 2D / P2 | Una gráfica institucional o diagrama abstracto para una pared secundaria | **IA opcional**, 1024×512 o 2048×1024 como fuente; exportar al tamaño útil. Sin texto incrustado ni marcas reales |
| ART_FloorWayfinding | Props 2D / P1 | Orientar entre zonas sin llenar la sala | **Unity**, tiras/placas opacas e iconos; IA innecesaria |
| ART_MaterialSwatches | Materiales / P1 | Familias de madera/plástico/metal/papel | **Unity**, ajustar materiales existentes. Textura nueva solo si una prueba cercana demuestra que hace falta |
| PROLOGUE_Frame01_Printer | Prólogo / P1 | Presentación de impresora en el banco de origen | Preferir **render del modelo existente en Unity**; IA como alternativa si se elige storyboard ilustrado |
| PROLOGUE_Frame02_NoPrint | Prólogo / P1 | Impresora detenida y bandeja sin comprobantes, misma composición | **Unity/render existente o IA consistente**, misma impresora y encuadre |
| PROLOGUE_Frame03_Correspondent | Prólogo / P1 | Puente visual al puesto del analista en otro banco | **Captura/render del banco actual**; IA solo si toda la secuencia usa el mismo estilo |
| PROLOGUE_PaperLayer | Prólogo / P2 | Capa de papel para animación ligera | **Unity** con papel existente o recorte derivado del mismo arte; PNG transparente solo si se necesita compositar |

No se recomienda generar nuevos escritorios, PCs, impresoras, archivadores o una oficina completa con IA. Esos recursos ya existen. No son necesarios personajes realistas ni una cinemática nueva para mejorar la presentación del prólogo.

### Audio y música

| Nombre propuesto | Categoría / prioridad | Uso | Producción recomendada |
|---|---|---|---|
| SFX_UI_Focus | UI / P1 | Entrada a foco, muy discreta y una sola vez; no un pitido por frame | **Síntesis/edición propia** o pack licenciado |
| SFX_UI_Confirm | UI / P0 | Confirmación de selección aceptada | **Síntesis propia**, corto y sobrio |
| SFX_UI_Unavailable | UI/sistema / P0 | Acción no disponible o cierre incompleto | **Síntesis propia**, distinguible sin ser alarma estridente |
| SFX_Dossier_Open | Objeto/UI / P1 | Consulta del expediente | **Grabación/descarga licenciada** de papel/terminal, editada; evitar duplicar confirmación innecesariamente |
| SFX_Paper_Evidence | Objeto / P1 | Inspección/registro de evidencia | **Grabar o descargar** roce de papel; no hacer sonar una impresión si no está ocurriendo |
| SFX_Archive_Open | Objeto / P1 | Consulta del archivador | **Grabar o descargar** cajón/carpeta; duración aproximada 0.3–0.8 s |
| SFX_System_Attention | Sistema / P0 | Alerta relevante del expediente | **Síntesis propia**, aviso corto; no sirena continua ni «fraude confirmado» por una coincidencia |
| SFX_Defense_Enable | Sistema / P1 | Controles activados | **Síntesis propia** de 0.3–0.6 s, confirmación tecnológica contenida |
| SFX_Teleport_Arrive | Sistema / P1 | Refuerzo suave al llegar al destino | **Síntesis/edición propia**, sin gran barrido estéreo ni sobresalto |
| AMB_Office_RoomTone | Ambiente / P1 | Aire/ventilación leve que une la sala | **Grabación o biblioteca licenciada**, bucle de 30–60 s sin conversaciones comprensibles |
| AMB_Equipment_Low | Ambiente / P2 | Presencia local muy suave de equipo electrónico | **Síntesis o grabación**; no zumbido tonal insistente |
| MUS_NightOperations | Música / P2 | Tensión ambiental discreta durante análisis | **Composición propia, descarga licenciada o IA** con condiciones de uso revisadas; bucle 60–90 s |
| MUS_Resolution | Música / P2 | Capa breve y más estable al cerrar | **Derivar del mismo tema**; no otra estética musical desconectada |
| SFX_Prologue_PrinterAttempt | Prólogo / P1 | Intento mecánico de alimentación sincronizado con papel | **Grabar/descargar** impresora; IA solo si no se obtiene un sonido limpio adecuado |
| SFX_Prologue_Stop | Prólogo / P1 | Corte del motor y breve silencio narrativo | **Edición del mismo sonido**, manteniendo continuidad |
| AMB_Prologue_OriginOffice | Prólogo / P2 | Distinguir discretamente el banco de origen | **Derivar del ambiente** con otra mezcla; no es imprescindible otro archivo |
| SFX_Result_Attack | Resultados / P1 | Cierre sobrio del ejercicio con exposición/pérdida | **Síntesis/composición breve**; no celebración del delito |
| SFX_Result_Defense | Resultados / P1 | Cierre contenido de controles aplicados | **Variación de la misma firma sonora**; no fanfarria de videojuego de combate |
| MIX_SWIFT_Master | Mezcla / P0 | Control separado de UI, objetos, ambiente y música | **AudioMixer en Unity**, no asset descargable |

Estas duraciones son especificaciones iniciales de producción, no límites ya probados. La IA es más útil en una cama musical o textura ambiental que en un click preciso o una impresora reconocible. Para SFX mecánicos prefiero grabación/biblioteca editable.

## 7. Diagnóstico y dirección de audio

### Qué existe

`Assets/_Project/Scripts/Audio/FeedbackAudio.cs` crea un clip mono de **4410 muestras a 44100 Hz**, unos **0.1 s**, con seno de **660 Hz** y envolvente decreciente. Configura volumen 0.12 y spatialBlend 0. Se reproduce con PlayOneShot cada vez que GameManager emite Changed.

No se localizaron archivos WAV, MP3, OGG, AIFF, FLAC o `.mixer` en Assets. Esto no significa silencio total: el tono se genera en runtime. Tampoco demuestra que se oiga bien en el teléfono. No se escuchó una sesión real para calificar el volumen o la distorsión.

### Qué falta por familia

- **UI:** foco, selección aceptada y acción no disponible diferenciados; confirmación limitada a una por acción.
- **Objetos:** papel/evidencia, archivador y terminal, con origen espacial cuando sea apropiado.
- **Sistema:** atención, defensa activada, llegada por teletransporte y cierre incompleto, con prioridades claras.
- **Ambiente:** room tone continuo y discreto. Evitar una oficina inexplicablemente vacía acústicamente o un zumbido que canse.
- **Música:** una capa opcional, sobria y repetible; el juego debe poder entenderse con ella apagada.
- **Prólogo:** intento de impresora, parada y silencio breve, sincronizados con la secuencia ya existente.
- **Resultados:** firma corta y diferencia emocional moderada entre exposición y defensa; las cifras siguen explicadas por texto.

### Cómo debería sonar

Diseño contenido: materiales reales, electrónica suave y pocos eventos simultáneos. No convertir cada consulta o cambio de mensaje en un pitido idéntico. Separar el disparo del sonido de Changed: ese evento sirve a UI y se emite también al consultar mensajes. Añadir una capa de presentación sonora por intención, conservando las APIs y cálculos bancarios.

Crear grupos Master, UI, Objects, Ambience y Music en AudioMixer. UI/música como 2D; impresora/archivo como fuentes 3D simples cuando el evento corresponda al objeto. **SpatialBlend 3D no equivale a audio binaural/HRTF**: no se propone instalar otro spatializer o plugin ahora. Con el altavoz del teléfono la localización será limitada; probar también con auriculares si se usarán en la demostración.

Durante lectura, música y ambiente deben quedar por debajo de los eventos de confirmación sin exigir subir el volumen. Evitar tonos de foco repetidos, alertas en bucle, grandes barridos y efectos sobre la cámara. Las señales de audio siempre se acompañan con texto/estado visual.

Entregar masters editables WAV, SFX sin silencios largos y bucles sin clics. Elegir compresión, carga y mono/estéreo en Unity según duración/uso y medir memoria; no forzar PCM a toda la música. Registrar autor, fuente, licencia/condiciones de uso y uso de IA por recurso. No se presupone que una generación IA o una descarga sea automáticamente reutilizable.

## 8. Prompts sugeridos para IA

Son briefs listos para la fase de producción; **no se ejecutaron**. Mantener una misma referencia aprobada y paleta entre variantes. Producir recursos por separado, no una hoja con seis pantallas aplastadas o un mockup con texto ilegible.

### P-A — Superficie de documento ficticio

> Superficie frontal ortográfica de una hoja bancaria ficticia para un videojuego educativo low-poly. Papel marfil mate, estructura editorial sobria con pocas líneas gris azulado y espacios amplios totalmente vacíos para campos que añadiremos en Unity. Ligera variación de papel visible solo de cerca. Iluminación neutra uniforme, sin sombras proyectadas, sin perspectiva, sin mesa alrededor. Imagen cuadrada 1024×1024, bordes completos. Sin texto, letras, números, firmas, sellos oficiales, logotipos, marcas de agua ni datos personales. Sin brillos ni textura sucia de documento antiguo.

Añadir etiquetas ficticias y «SIMULACIÓN» con TMP/diseño controlado, nunca confiar datos dinámicos a la imagen generada.

### P-B — Gráfica institucional de pared

> Ilustración horizontal 2:1 para la gráfica secundaria de una oficina bancaria ficticia nocturna, estilo geométrico low-poly limpio. Paleta azul noche #101B2D, gris pizarra, marfil y acentos muy contenidos cian/ámbar. Tema abstracto: trazabilidad y verificación de documentos mediante formas simples y conexiones escasas. Grandes masas visuales y espacio negativo. Vista frontal plana, sin marco, sin sombras ni iluminación horneada. Sin texto, cifras, mapas de ciberataques, binario, banderas, logos reales o símbolos de malware. Debe acompañar la sala sin convertirse en el foco principal.

### P-C — Storyboard de impresora, tres imágenes consistentes

Usar primero una imagen de referencia aprobada del modelo actual; si se decide ilustración, pedir las tres tomas como variantes coherentes:

> Ilustración 16:9 low-poly estilizada, oficina nocturna sobria de un banco de origen ficticio. Misma impresora gris, misma mesa, misma iluminación cálida de trabajo y ambiente azul pizarra en toda la secuencia. Encuadre estable con espacio limpio inferior para subtítulos y zona libre para un botón de omisión añadido después en Unity. Sin texto, marcas reales, rostros identificables, pantallas con datos legibles ni elementos futuristas. Mantener impresora y bandeja completamente dentro del encuadre.

- Imagen 1: impresora y hoja iniciando alimentación; no mostrar fallo con símbolos inventados.
- Imagen 2: idéntica composición, mecanismo detenido y bandeja sin comprobantes; tensión por pausa/ausencia, no por luces rojas por toda la sala.
- Imagen 3: puente al Banco Corresponsal ficticio, basado en la oficina real del juego; distinguir el cambio con el título dinámico, no fingir que es la misma ubicación.

Si el estilo no coincide con las mallas actuales, **preferir renders de los modelos existentes** en lugar de insertar tres ilustraciones inconexas.

### P-D — Concepto de identidad documental, opcional

> Familia visual mínima para carpetas y separadores documentales de una institución bancaria ficticia: azul noche, marfil cálido, madera desaturada y cian moderado. Formas geométricas amplias y una única banda de categoría. Vista plana ortográfica, sin mockup, sin perspectiva, sin letras, cifras, logos existentes o sellos oficiales. Evitar desgaste dramático y detalles finos. El resultado servirá como referencia; textos, iconos definitivos y estados serán construidos por separado.

### P-E — Música de operaciones nocturnas

> Pieza instrumental ambiental original para lectura y análisis en una oficina bancaria nocturna ficticia. Tensión contenida, pulso escaso de 65–75 BPM si se usa ritmo, texturas cálidas y discretas, armonía estable, sin melodía invasiva, percusión agresiva, voces, sirenas, sobresaltos o subgraves dominantes. Bucle de 60–90 segundos con inicio y final compatibles; ofrecer versión sin pulso y stems si la herramienta lo permite. Sin imitar artistas, bandas sonoras o juegos existentes. Debe poder mezclarse muy por debajo de las confirmaciones UI.

Revisar el bucle en edición: el prompt no garantiza continuidad ni licencia.

### P-F — Ambiente de sala, solo si no se graba/descarga

> Ambiente interior de una pequeña oficina de operaciones en turno nocturno: ventilación suave y presencia lejana de equipos, estable y discreta. Sin voces comprensibles, alarmas, melodías, golpes, pasos cercanos, pitidos o cambios bruscos. Bucle de 45 segundos, sin reverberación exagerada ni efecto binaural preprocesado. Entrega limpia con margen de nivel para mezclar.

### P-G — Impresora y firma de resultados

> SFX aislado de una impresora de oficina intentando alimentar papel: motor corto, rodillo y roce de hoja durante 1–2 segundos, parada mecánica natural, sin pitido electrónico añadido, música, voces o reverberación de sala. Una sola acción por archivo, con cola limpia.

> Dos firmas sonoras originales relacionadas, de 0.8–1.5 segundos: una de cierre sobrio con tensión no resuelta y otra de cierre estable y contenido. Timbre electrónico cálido, pocos elementos, sin fanfarria, golpes cinematográficos, voces o similitud con marcas/juegos. Mantener ambas dentro de la misma familia.

Para clicks/retícula/iconos funcionales no se propone prompt de generación: es más preciso crearlos con síntesis y geometría/UI.

## 9. Plan de mejora en orden

### Fase A — Sin descargas ni arte nuevo

1. **Ficha de estilo:** aprobar paleta, jerarquía tipográfica y familia de materiales. Definir un conjunto pequeño de capturas de comparación.
2. **UI y mirada:** ordenar campos/resultados, mejorar estados de botones y retícula, conservando tamaño angular y corrección del collider. Separar instrucciones PC/VR según presentación, sin añadir mecánicas.
3. **Escritorio y composición:** sacar papeles del teclado, revisar escala aparente de torre/panel y agrupar mobiliario disponible por zona. No cambiar a la vez posiciones de pads y de UI sin pruebas.
4. **Materiales y señalética:** bajar saturación de archivo/sofá, unificar madera y metal pintado, mejorar rótulos y pads con formas existentes.
5. **Luz:** ajustar reparto de las luminarias actuales y luz de trabajo, mantener bake y comparar desde los mismos puntos. Ningún cambio global de URP para compensar la composición.

### Fase B — Arte IA o renders reutilizados

6. Producir primero **una** muestra de documento y **una** imagen de prólogo. Aprobar estilo antes de solicitar variantes.
7. Completar las tres imágenes del prólogo solo si superan visualmente a un render de la impresora existente y mantienen continuidad.
8. Añadir una gráfica institucional secundaria únicamente donde mejore una composición concreta. No rellenar paredes por inercia.

### Fase C — Audio

9. Crear mezcla y feedback mínimo: confirmar/no disponible/atención. Ajustar los disparadores para no reproducir el mismo sonido por cada Changed.
10. Añadir papel/archivo/impresora y ambiente, con duración y posición coherentes.
11. Incorporar firma de resultados y música opcional después de comprobar que los eventos esenciales se distinguen a bajo volumen.

### Fase D — Validación en HONOR

12. Probar cada iteración importante, no esperar al final: modo/estado/monto legibles, retícula visible, botones alcanzables, cuatro pads y cinco funciones conservados.
13. Comparar nitidez/aliasing y coste con perfil Mobile; solo entonces evaluar variantes controladas de renderScale/AA. Registrar FPS/frame time, memoria, calentamiento y comodidad, sin inventar mediciones.
14. Escuchar en el altavoz y configuración real de la demostración; revisar equilibrio, repetición, pausa/reanudación, bucles y fatiga. Repetir raycasts después de cualquier cambio de layout.

## 10. Diez mejoras visuales más importantes, ordenadas

1. **Reorganizar el expediente y los resultados:** lectura por campos y bloques claros con TMP.
2. **Completar foco, progreso y confirmación de mirada:** retícula y botones como una sola familia.
3. **Integrar visualmente panel/monitor sin perder legibilidad ni despeje del collider.**
4. **Ordenar escritorio y proporciones de sus objetos:** corregir papeles sobre teclado y peso de la torre.
5. **Dar protagonismo lumínico al puesto de análisis** con las luces existentes y bake revisado.
6. **Unificar paleta:** desaturar muebles secundarios y reservar acentos para orientación/acciones.
7. **Diferenciar madera, papel, plástico y metal pintado** mediante materiales locales coherentes.
8. **Rediseñar señalética, kioscos y aspecto de pads** sin cambiar sus funciones o número.
9. **Agrupar las zonas con mobiliario existente y pocos apoyos gráficos**, conservando circulación.
10. **Dar acabado al prólogo** con el modelo existente o una familia de tres imágenes coherentes, sin rehacer la oficina.

El audio mínimo diferenciado debe acompañar las primeras mejoras de feedback, aunque no figure como tarea visual. Música y decoración adicional vienen después.

## 11. Primeras tareas de implementación propuestas

| Orden | Acción concreta | Archivos/recursos candidatos | Prueba de aceptación |
|---|---|---|---|
| A01 | Aprobar ficha visual y crear tokens de color/tipo de presentación | OperationsCenterStyle.cs y materiales propios, sin tocar URP | Paleta única documentada; capturas comparables |
| A02 | Maquetar una ficha activa y un resultado con la misma familia | UIManager.cs, OC_Expedientes.prefab | Datos/cálculos sin cambios; textos completos y tres acciones conservadas |
| A03 | Mostrar progreso y feedback de foco, activación y rearme | VRInteractionManager.cs, presentación de BankInteractable, retícula/UI | Una activación por foco, señal comprensible y ninguna regresión de raycast |
| A04 | Limpiar escritorio y ajustar la composición aprobada | BancoCorresponsal y prefabs existentes, mediante APIs Unity/Undo | Cuatro vistas antes/después, sin oclusión y escena recuperable |
| A05 | Ajustar materiales y luces por zonas, volver a hornear | OfficeVisual/Materials, LightingReview y escena | Capturas y visor comparados; sin nuevas luces dinámicas por defecto |
| A06 | Sustituir tono único por tres intenciones y mezcla básica | FeedbackAudio.cs y futuro MIX_SWIFT_Master | Confirmar/no disponible/atención distinguibles, sin ráfagas por mensajes |

Antes de editar escena/prefab en la fase de implementación, conservar copia fechada de escena+.meta y recursos afectados; no sobrescribir el trabajo local del equipo. Repetir GameplayFlowProbe, BankOfficeProbe y PrototypeSmokeProbe cuando se cambie UI/retícula. No hace falta ejecutar otra vez esas pruebas por esta auditoría documental.

## 12. Fuentes técnicas y decisiones de alcance

La crítica estética y el orden de trabajo son juicio de dirección de arte basado en la evidencia local, no afirmaciones de una documentación externa. Para los puntos técnicos se consultaron fuentes primarias:

- [Unity 6.3 — Anti-aliasing en URP](https://docs.unity.com/en-us/engine/6000.3/manual/render-pipelines/universal-render-pipeline/anti-aliasing): distintas técnicas tienen costes/compatibilidades diferentes; no se recomienda activar una a ciegas.
- [Unity 6.3 — Audio Mixer](https://docs.unity.com/en-us/engine/6000.3/manual/audio/mixer): mezcla de fuentes, grupos y efectos para organizar el audio propuesto.
- [Unity — Introducción a Audio Source](https://docs.unity.com/en-us/engine/6000.0/manual/audio/reference/class-audio-source/source-overview): reproducción 2D/3D y SpatialBlend. Esta última referencia corresponde a 6.0; se usa para el concepto básico, mientras la implementación actual se contrastó con el código local de 6.3.

No se propone cambiar Cardboard, migrar pipeline, instalar plugins de audio/render ni adoptar música/arte comercial sin revisar su procedencia. No se pidieron nuevos niveles ni nuevas mecánicas. El siguiente paso recomendado es **A01–A03, una pasada de interfaz y feedback con los recursos actuales**, seguida por escritorio/luz/materiales y una prueba en el HONOR.
