# SWIFT — Viabilidad técnica en HONOR y Google Cardboard

**Fecha:** 8 de noviembre de 2026. **Estado:** análisis, sin pruebas nuevas de Editor o Android. Las estimaciones de coste/riesgo no son mediciones de rendimiento.

Relacionados: [investigación](VR_GAMEPLAY_RESEARCH.md), [diseño](IMMERSIVE_GAMEPLAY_DESIGN.md), [roadmap](CODEX_IMPLEMENTATION_ROADMAP.md). **[P]** observado en proyecto; **[F]** fuente externa; **[I]** inferencia; **[R]** propuesta.

## 1. Veredicto

**La entrega principal es viable con mirada, herramientas en posiciones conocidas, ampliación de documentos, llamadas subtituladas y transición de puesto con fade.** No necesita manos ni nuevas dependencias. La viabilidad de concepto no certifica legibilidad, comodidad o rendimiento: esas tres condiciones deben comprobarse con la APK actualizada en el HONOR Magic7 Lite y el visor real.

No es viable prometer agarre espacial libre de manos/mandos utilizando únicamente el teléfono y el Cardboard actuales. Añadir una malla de manos o instalar XR Hands no crea datos de seguimiento.

## 2. Dispositivo y configuración observada

**[F]** La ficha de soporte del operador Three identifica Magic7 Lite con Snapdragon 6 Gen 1, pantalla AMOLED de 6,78 pulgadas y 2700 × 1224, giroscopio y acelerómetro. La configuración descrita allí tiene 8 GB de RAM y 512 GB. Esto identifica una variante comercial; no demuestra RAM, sistema actualizado ni comportamiento de la unidad del usuario. La ficha dinámica de HONOR consultada expone encabezados de sensores pero no todos sus valores en la extracción textual; no se usa un encabezado como prueba de soporte. [Ficha de soporte Three](https://devicesupport.three.co.uk/guides/device/HONOR/Magic7Lite5G), [HONOR España](https://www.honor.com/es/phones/honor-magic7-lite/spec/).

**[P]** El proyecto utiliza Unity 6000.3.24f1, URP 17.3.0 y Cardboard. `CardboardLifecycle` pide 60 fps; esto es un objetivo de la aplicación, no una medición. El perfil móvil documentado tiene render scale 0,8, MSAA 1, una cascada de sombras principales y ninguna Renderer Feature en `Mobile_Renderer`. No se propone cambiarlos en esta tarea.

**[I]** La resolución nominal de la pantalla no es la resolución legible de cada ojo: hay división estereoscópica, distorsión, render scale, lentes y ajuste del visor. Tampoco una pantalla de alta frecuencia implica que SWIFT pueda renderizar a esa frecuencia.

Comprobaciones pendientes de la unidad física:

- Modelo exacto, RAM y versión instalada de Android/MagicOS.
- Ajuste del teléfono en el visor y QR óptico correspondiente.
- Seguimiento de orientación, deriva y recentrado después de varios minutos.
- Alineación binocular, lectura de nombres largos y comodidad del peso.
- Funcionamiento del botón del visor si se utiliza; el dwell debe bastar.
- Audio real, reanudación después de pausar y calentamiento durante una ronda.

## 3. Qué entrada existe realmente

**[F]** Google documenta renderizado estéreo, seguimiento de cabeza, botón del visor, parámetros QR y recentrado para Cardboard en Unity. No proporciona en ese flujo un sistema de manos articuladas o mandos espaciales rastreados. [Quickstart oficial](https://developers.google.com/cardboard/develop/unity/quickstart).

**[P]** SWIFT usa `VRInteractionManager` para raycast y dwell; `HeadPoseAdapter` aplica orientación y una altura virtual. No hay en el manifest proveedor de manos ni una implementación de posicionamiento espacial de mandos. La versión objetivo debe tratarse como **3DoF de cabeza**: mirar en distintas direcciones, sin prometer que acercarse físicamente al papel acerque la cámara al papel.

| Nivel de interacción | Qué significa | Cardboard actual | Implementación prevista |
|---|---|---|---|
| 1. Seleccionar | El raycast enfoca un objeto | Sí | Foco existente, indicador visual |
| 2. Activar | Confirmar una acción tras fijación | Sí | Un único dwell de 1,5 s |
| 3. Inspeccionar | Abrir información ampliada | Sí | UI/TMP y estado modal |
| 4. Acercar animado | Objeto va a una pose predeterminada | Sí, simulado | Animación de Transform y retorno |
| 5. Agarrar con mando | Objeto sigue pose espacial del controlador | No con equipo actual | Visor/runtime/controladores compatibles y adaptador |
| 6. Agarrar con manos | Se usan articulaciones y gesto rastreado | No de forma nativa en este montaje | Sensor/visor compatible y proveedor de manos |
| 7. Manipular con físicas | Colisiones, masa, soltar y lanzar | Unity puede simularlo, pero falta entrada espacial natural | Opcional; complejidad y coste adicionales |

Un mando Bluetooth convencional puede aportar botones, pero por sí solo no proporciona posición 6DoF. Una animación de mano siguiendo el centro de la cámara tampoco es seguimiento real. Usar cámaras del móvil para inferir manos sería un proyecto de visión y calibración distinto, no una integración pequeña apropiada para la entrega.

## 4. Reglas para manipulación simulada convincente

**[R]** Mantener un ciclo consistente: objeto visible → foco → progreso → activación → pose/vista estable → cierre → retorno. El objeto siempre tiene origen y destino reconocibles. La animación no mueve ni gira la cabeza del jugador.

Una primera prueba puede usar desplazamientos de objeto de 0,2–0,35 s, pendientes de ajuste. El auricular no debe cruzar el centro de lectura, tapar los botones ni parecer una mano controlada. El papel no debe moverse continuamente mientras se lee. Las poses son autoradas y cancelables; no hay arrastre por mirada ni colisiones complejas.

Objetos importantes dentro de un arco frontal. Como punto de partida de composición, probar teléfono y portapapeles a unos 20–30 grados a cada lado del monitor, no como norma ergonómica universal. Si el puesto actual no admite esa distribución, reducir alternancia y presentar el extracto en la misma zona de inspección. No obligar a giros detrás de la silla.

## 5. Matriz de viabilidad

**[R]** Escala: B bajo, M medio, A alto. En educación/inmersión, A indica beneficio esperado; en dificultad/riesgo, A indica problema mayor. P0 = base que debe preservarse; P1 = candidato principal; P2 = ampliación; P3 = posterior. Ninguna fila nueva está validada en dispositivo.

### Beneficio, dificultad y riesgos

| ID / mecánica | Educación | Inmersión | Dificultad | Riesgo técnico | Mareo/incomodidad | Coste Android incremental | Prioridad |
|---|---|---|---|---|---|---|---|
| M1 Teléfono A fijo + subtítulos | A | A | M | M: contexto/evidencia | B: si panel estable | B: pocos objetos y TMP | P1 |
| M2 Teléfono B con pose animada | Igual que M1 | A | M | M: oclusión/retorno | B–M: cercanía | B con Transform | P2 |
| M3 Teléfono C con agarre real | M: gesto no enseña por sí solo | A | A | A: proveedor/hardware | M: alcance y pérdida de tracking | Depende del visor, no comparable con HONOR | P3 |
| M4 Portapapeles A, hoja pequeña | M | A | B | B | A: esfuerzo de lectura | B | No como lector principal |
| M5 Portapapeles B ampliado | A | A | M | M: foco/UI/contexto | B–M: tamaño y distancia | B–M: canvas/overdraw | P1, primero |
| M6 Monitor secundario + libreta | M | M | B | B: ya existente | M: giros y texto | Similar a actual | P0/fallback |
| M7 Fade de asiento + sonido | B | M | B | B–M: transición/colliders | B, sujeto a prueba | B | P1 después de herramientas |
| M8 Descenso continuo de cámara | B | M | M | M: seguimiento | A: movimiento impuesto | B, pero no compensa | Descartar por defecto |
| M9 Carpeta/hoja en pose de inspección | M si añade comparación | M | M | M: duplicación | B–M | B–M | P2 |
| M10 Impresión visual de evidencia | M | M | B–M | M: duplicación de registro | B | B | P2 |
| M11 Archivador/sello obligatorio | B | M | M | M: nuevas dependencias de flujo | M: repetición y giros | B–M | Posponer/eliminar requisito |
| M12 Objetos con físicas libres | B para el objetivo actual | A | A | A: escapes/colisiones | M | M–A, requiere medir | P3 |

### Compatibilidad y recursos, con los mismos identificadores

| ID | Cardboard | Dependencias nuevas | Modelos / gráficos | Audio |
|---|---|---|---|---|
| M1 | Sí | Ninguna; scripts propios | Teléfono simple nuevo + UI existente | Subtítulos bastan; timbre/clics después |
| M2 | Sí, simulación de pose | Ninguna | Auricular separado y anclaje | Contestar/colgar |
| M3 | No nativo | Runtime, SDK e input según equipo; XRI/manos si procede | Objeto con puntos de agarre; manos del proveedor | Igual M1; háptica si disponible |
| M4 | Sí como prop | Ninguna | Reutilizar tabla, clip y papel | Opcional |
| M5 | Sí | Ninguna | Reutilizar prop; canvas TMP | Papel/confirmación opcionales |
| M6 | Sí, existente | Ninguna | Pantalla y portapapeles actuales | Existente |
| M7 | Sí, base existente | Ninguna | Silla existente, sin nuevo modelo | Un sonido de silla |
| M8 | Técnicamente sí, desaconsejado | Ninguna | Ninguno | No resuelve incomodidad |
| M9 | Sí, simulada | Ninguna | Papeles/carpeta simple, materiales existentes | Papel |
| M10 | Sí | Ninguna | Impresora Bruno y papeles Jarlan; respetar CC BY | Impresora breve opcional |
| M11 | Sí, con activaciones discretas | Ninguna | Archivador disponible; sello simple nuevo si se usa | Clic/sello |
| M12 | Motor sí; agarre espacial no | Para agarre real, stack/hardware adicional | Colliders, rigidbodies, objetos recuperables | Impactos controlados |

## 6. Texto, colisiones y rendimiento

### Lectura y composición [R]

- No juzgar tamaño de letra por píxeles del PNG o por el número de `fontSize` de TMP. Medir tamaño del panel en mundo, distancia real y lectura binocular.
- Ángulo del panel: `2 × atan(ancho / (2 × distancia))`, convertido a grados. Es una ayuda geométrica; no garantiza que una tipografía sea legible.
- Probar inicialmente una vista a aproximadamente 1–1,5 m virtuales, ajustando tamaño para no ocupar todo el visor. Es un rango de ensayo, no una recomendación universal ni un cambio automático al monitor actual.
- Evitar mucha información a diferentes profundidades y párrafos diminutos. Mantener contraste, campos largos con saltos previstos y controles de cierre siempre visibles.
- Probar beneficiarios, cuentas, monto y guion más largos; no recurrir a reducir la letra automáticamente hasta que «quepa».
- No añadir bloom, scanlines, texto en 3D con relieve ni transparencia multicapa detrás de las letras.

### Entrada y colisiones [P → R]

`VRInteractionManager` utiliza objetivos `BankInteractable` y colisiones de física; un `Button` nuevo de Unity UI no queda conectado automáticamente al sistema. Debe conservarse la separación del panel respecto al collider de la computadora y añadirse un bloqueo modal compatible con ambos caminos de activación, incluidos métodos llamados directamente desde pruebas.

Una ventana puede verse delante pero tener colliders detrás. Debe verificarse que el raycast encuentre la acción esperada, no el escritorio o un botón bancario oculto. Los objetivos no deben reconstruirse cada frame ni desaparecer durante el dwell. Al cambiar de hoja, exigir nueva fijación para acciones sucesivas.

### Presupuesto de recursos [R]

Objetivos iniciales de ingeniería, no límites medidos: teléfono de silueta sencilla, alrededor de 1–3 mil triángulos si se modela, materiales compartidos y como máximo una textura pequeña si realmente mejora la lectura. Empezar sin textura exclusiva. Un solo panel de inspección activo y número reducido de capas transparentes. Sin luces en tiempo real nuevas, cámaras de monitor adicionales ni físicas de papel.

Actualizar textos cuando cambian expediente, hoja o estado; no por `Update`. Reutilizar elementos UI y buffers de contenido. No hacer búsquedas globales, recrear canvas o asignar cadenas largas cada frame. Una animación corta de Transform no justifica modificar URP.

Para los 60 fps solicitados actualmente, el presupuesto total de frame es aproximadamente 16,7 ms. **No se ha medido cuánto consume hoy el banco.** Primero medir base y luego variante, mismo recorrido y configuración. Registrar CPU/GPU cuando estén disponibles, frames lentos, memoria y evolución térmica. Si la base no sostiene el objetivo, el teléfono no debe declararse «barato» por intuición: corregir el cuello de botella antes de ampliar.

## 7. Audio viable

**[P]** Existen `AudioCategory.UI`, `Objects`, `Alerts`, `Ambience`, `Music`; biblioteca y buses con ganancias y grupos opcionales. No hace falta crear un AudioMixer solo para demostrar una llamada subtitulada.

**[R]** Timbre en el objeto, voz inteligible en primer plano o con espacialización moderada, ambiente bajo. Subtítulos permiten completar todo con audio silenciado. Cortar timbre al responder/cancelar y voz al cerrar, salir del puesto o cambiar de sesión; relectura no repite alertas automáticamente. Pausar/reanudar conserva un estado coherente.

No ampliar sin revisión el enum de eventos: `FeedbackAudio` tiene arrays de ocho posiciones y `AudioCueLibrary.Clip` usa una selección condicional. Un evento telefónico añadido sin adaptar todo puede usar índice o clip incorrectos. Preferir un controlador de audio de escritorio separado que use las ganancias/grupos existentes, con clips explícitos y eventos de transición. Detalle en el roadmap.

## 8. Ruta posterior para mandos o manos del salón

**No se conoce el equipo del salón.** Antes de elegir SDK registrar: marca/modelo del visor, mandos o sensor, modo autónomo o PC, sistema operativo, runtime, conexión, versión de Unity soportada y permisos de instalación. Esto no bloquea el MVP de mirada.

**[F]** XRI ofrece interactores y `XR Grab Interactable`; este último sigue al interactor seleccionado y permite estrategias de movimiento con Transform o Rigidbody. Eso es una herramienta para implementar agarre, no una fuente de posiciones por sí misma. [Unity XRI](https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.1/manual/xr-grab-interactable.html).

**[F]** XR Hands expone datos de articulaciones y necesita un proveedor compatible; instalarlo sin ese proveedor y sin dispositivo que entregue datos no produce seguimiento. [Unity XR Hands](https://docs.unity3d.com/Packages/com.unity.xr.hands@1.7/manual/index.html).

Rutas condicionadas [R]:

| Equipo confirmado después | Prueba inicial | Lo que no debe asumirse |
|---|---|---|
| Visor 6DoF con mandos y runtime compatible | Un cubo agarrable, luego portapapeles; perfil separado | Que el APK Cardboard actual se convierte automáticamente en esa versión |
| Visor con seguimiento de manos soportado | Datos de articulaciones, pérdida de seguimiento, gesto de selección; después un objeto | Que todos los visores OpenXR exponen manos o iguales gestos |
| Sensor externo tipo Ultraleap | Comprobar sensor, SDK, montaje, equipo anfitrión y calibración según documentación específica | Que un sensor USB funciona directamente en el HONOR o sustituye seguimiento de cabeza |
| Solo guantes, mando Bluetooth o modelos de manos | Identificar qué datos entregan realmente | Que proporcionan posición espacial y agarre fiable |

Secuencia técnica futura: proyecto/perfil de revisión aislado → escena mínima del proveedor → adaptador de entrada → portapapeles agarrable → botones por ray/interactor → integración del puesto → regresión del dominio. Conservar el origen de seguimiento correcto del nuevo runtime. Los mandos/manos invocan las mismas acciones de aplicación; no duplican reglas ni cálculos. Elegir un método de selección activo para evitar activación simultánea por mirada y mano.

No migrar el paquete Cardboard de la entrega principal. No instalar dependencias en esta investigación. El plazo de esa variante solo puede estimarse tras conocer hardware y validar la escena mínima; no se considera «un día para añadir manos».

## 9. Protocolo de aceptación física propuesto

1. Instalar una APK nueva identificada por fecha y hash, sin sustituir la antigua. Confirmar BancoCorresponsal y versiones reales.
2. Ajustar visor y recentrar. Probar orientación sin interacción y lectura del monitor desde el puesto.
3. Abrir/cerrar portapapeles, cambiar hojas, mirar fuera al 50 % y cambiar objetivo. Ninguna activación involuntaria.
4. Probar SIM-003: llamada, lectura, registro único, decisión. Repetir con sonido silenciado.
5. Cancelar llamada y salir del puesto; después reiniciar modo. No sonido colgado, información vieja ni evidencia transferida.
6. Hacer una ronda ATAQUE y otra DEFENSA, incluyendo prólogo, omisión y resultados. Probar controles automáticos activos e inactivos.
7. Repetir durante 10–15 minutos para observar calor, frames lentos, deriva y cansancio. Este intervalo es una propuesta de prueba inicial, no certificación térmica.
8. Realizar piloto con personas nuevas; registrar necesidad de ayuda, errores de lectura, pérdida de foco y molestias. Detener si aparece incomodidad.

Capturas PC muestran composición y estado; capturas Android muestran el render del dispositivo; ninguna reemplaza la evaluación de lentes, profundidad y comodidad. Registrar resultados por plataforma, versión y fecha, sin etiquetar Editor como Android.

**Criterio de salida:** si falla lectura, foco, resultados o comodidad, mantener la versión anterior de la función mediante configuración y recortar alcance. La preparación de la demostración tiene prioridad sobre añadir otro objeto interactivo.
