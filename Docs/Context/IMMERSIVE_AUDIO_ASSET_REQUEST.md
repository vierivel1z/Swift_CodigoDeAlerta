# Recursos de audio para herramientas inmersivas

10/11/2026. Disparadores implementados en el gestor existente `FeedbackAudio`; referencias editables en `AudioCueLibrary`. No se han añadido clips finales ni descargado recursos. Los ocho tonos TEMP anteriores siguen identificados como temporales; los seis cues nuevos quedan silenciosos si no tienen clip. Esta lista es producción pendiente, no inventario de archivos existentes.

| Campo / categoría | Recurso requerido | Duración orientativa / tratamiento | Obtención viable |
|---|---|---|---|
| PhoneRing / Objects | Timbre de terminal institucional | 0,8–1,5 s, uno o dos pulsos discretos, mono; sin cola larga, sin alarma urgente | Grabación propia, biblioteca con licencia documentada o IA |
| PhoneAnswer / Objects | Descolgar / conexión | 0,15–0,4 s, clic mecánico suave, mono | Grabación propia o IA |
| PhoneEnd / Objects | Finalizar llamada | 0,15–0,4 s, clic corto, diferenciable del rechazo UI | Grabación propia o IA |
| PageTurn / Objects | Pasar hoja | 0,25–0,6 s, papel limpio cercano, sin golpes de mesa | Grabación propia preferida o IA |
| Chair / Objects | Movimiento breve de silla | 0,3–0,7 s, tejido/rueda sutil, sin chirrido estridente | Grabación propia preferida o biblioteca licenciada |
| Printer / Objects | Respuesta mecánica de impresora | 0,6–1,2 s, arranque y parada cortos, mono; no bucle continuo | Grabación propia, biblioteca o IA |
| Evidence / UI | Evidencia registrada | 0,15–0,35 s, confirmación cálida y discreta | Diseño en editor de audio o IA |
| NewAlert / Alerts | Atención a nueva observación | 0,2–0,5 s, neutra, no equiparar alerta a fraude | Síntesis/diseño o IA |
| AmbienceLoop / Ambience | Oficina tranquila | 30–60 s, estéreo sin voces inteligibles, loop sin costura; baja intensidad | Grabación propia o biblioteca licenciada; IA requiere editar loop |
| MusicLoop / Music | Capa institucional opcional | 45–90 s, muy ligera, sin percusión insistente; no competir con lectura | Composición o IA con derechos de uso claros |
| Voz opcional / futuro | Tres intervenciones de SIM-003 | Español claro, ritmo tranquilo, voz ficticia; siempre subtítulos | Primero aprobar guion; luego locución propia o IA con licencia |

Entregar masters WAV PCM a 48 kHz, sin clipping y con margen de pico. Mantener los efectos mono para localización; ambiente/música estéreo. Importar y ajustar compresión/carga en Unity después de medir memoria y latencia en HONOR; no cargar audio de minutos descomprimido para una interacción de un segundo. La señal visual sigue siendo suficiente cuando falta clip. La llamada detiene PhoneRing al responder, cerrar o pausar; no se repite desde GameManager.Changed.

Prompts sugeridos de IA (sin marcas comerciales ni voces reales):

- «Efecto mono de un teléfono de oficina sobrio: dos pulsos suaves, un segundo, limpio, sin voz, sin música, sin alarma estridente, sin reverberación larga. Uso en simulación educativa bancaria VR.»
- «Una hoja gruesa pasando sobre portapapeles de madera, medio segundo, grabación cercana limpia, sin ambiente, sin golpes ni voz.»
- «Arranque y parada breve de una impresora pequeña de oficina, un segundo, mecánica suave, sin pitidos agudos, sin ambiente ni voz.»
- «Ambiente de oficina tranquila nocturna, ventilación suave distante, sin conversación ni teléfonos, estéreo, sesenta segundos, apto para editar como loop y para lectura VR.»

Guardar para cada recurso origen, autor/herramienta, fecha, licencia y permiso de redistribución. Revisar el resultado y nivelar en un editor de audio; una generación no garantiza un loop limpio ni una licencia adecuada. No se solicita arte nuevo para que funcionen portapapeles, teléfono o documento: se reutilizan papel/madera/plástico/metal y TMP existentes.