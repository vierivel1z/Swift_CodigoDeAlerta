# Avance de UX y jugabilidad VR

Fecha: 2026-11-02. Unity 6000.3.24f1 / URP 17.3.0 / BancoCorresponsal. Implementación comprobada en Editor; Android y aceptación visual en el visor pendientes.

## Cambios realizados

### Mirada

Se conserva el temporizador de 1,5 s y su activación única por fijación. VRInteractionManager expone el instante/evento de activación para la presentación; no se sustituyó su algoritmo de raycast, latch o rearme.

La retícula conserva su punto central, ahora blanco con contorno oscuro. Tiene base oscura y borde blanco, progreso cian, confirmación verde breve (0,28 s) y estado ámbar con texto **APARTA LA MIRADA** mientras el objeto siga seleccionado. La señal combina forma/texto/color. Las mallas y el Canvas del indicador no tienen colliders. El aviso se amplió después de revisar la primera captura; su tamaño percibido en HONOR aún no está aprobado.

### Interfaz

Se aprovecha la interfaz documental ya implementada: campos separados, beneficiario/cuenta/intermediario/fecha, observaciones y tarjetas comparativas con TMP. Se reforzó la jerarquía de etiquetas y monto; los tres controles originales y la corrección KeepPanelInFrontOfComputer permanecen. El resaltado diferencia foco, confirmación y rearme, también en controles creados dinámicamente.

La bienvenida ofrece un botón grande **PRACTICAR MIRADA / TUTORIAL VR**. Las instrucciones del menú se eligen por plataforma. En Android también se sustituye la línea PC del cartel Welcome en runtime; no se alteró el arte original ni el cartel guardado. La rama Android se comprobó como texto/código; no se ejecutó en teléfono.

### Tutorial opcional sin puntuación

Cinco pasos: mirar un objetivo, esperar el indicador, seleccionar un monitor de práctica, abrir una demostración de expediente y seleccionar Aprobar/Retener/Devolver de práctica. Se puede omitir en cualquier momento. No contiene una transferencia real ni inventa nuevas reglas; no crea GameSession ni llama a Decide/Notify.

El tutorial se abre únicamente fuera de los modos bancarios y oculta temporalmente el monitor, restaurando su estado al cerrar. Bloquea acciones bancarias mientras está abierto; siempre ofrece Omitir. Reutiliza el collider principal entre pasos para exigir apartar la mirada antes de otra activación. El panel permanece fijo en el mundo, sin animar/reparentar la cámara. Ajusta su distancia al obstáculo frontal y conserva tamaño angular proporcional, evitando quedar dentro del collider de computadora.

### Audio preparado

`FeedbackAudio` dejó de reproducir un tono por cada Changed. Observa transiciones de sesión, evidencia nueva, defensa, nueva operación con alerta y final; las consultas, teletransportes, confirmaciones y acciones no disponibles tienen disparadores explícitos desde BankInteractable.

Hay cinco buses AudioSource independientes: UI, Objetos, Alertas, Ambiente y Música, con ganancias por categoría. Objetos usan posición/atenuación 3D simple cuando la acción proporciona un origen; UI/alertas/teletransporte son 2D. No se añadió un spatializer. No fue necesario crear un AudioMixer: la biblioteca expone rutas AudioMixerGroup opcionales para incorporarlo después, manteniendo separación y control de ganancia desde ahora.

Biblioteca editable: `Assets/_Project/Resources/Audio/SWIFT_AudioCues.asset`. Sus ocho clips y los bucles de ambiente/música están preparados como referencias vacías. Con UseTemporaryTones activo, las señales puntuales usan clips generados **TEMP_SYNTH_SWIFT_...**. Ambiente y música permanecen silenciosos hasta asignar recursos; no se generan zumbidos ni música provisional. Se limita la repetición por cue a 0,12 s. Eventos de evidencia repetida o simples actualizaciones visuales no vuelven a anunciar evidencia/nueva alerta.

## Archivos afectados en esta tarea

Modificados: `Interaction/VRInteractionManager.cs`, `Interaction/BankInteractable.cs`, `UI/GazeProgressVisual.cs`, `UI/DocumentFocus.cs`, `UI/BankDocumentUI.cs`, `UI/UIManager.cs`, `Audio/FeedbackAudio.cs` bajo `Assets/_Project/Scripts/`.

Nuevos: `UI/VRQuickTutorial.cs`, `Audio/AudioCueLibrary.cs`, sus `.meta`, la biblioteca de cues en Resources/Audio y `Assets/_Project/Editor/UXGameplayReview.cs` con `.meta`.

La escena/prefabs no se regeneraron. GameManager, BankingDomain, ScenarioData, datos de entrenamiento, artes y configuración URP/Cardboard se conservaron durante el trabajo de UX. Para la solicitud posterior de publicación se guardó la escena mediante APIs Unity y se incluyeron las dependencias revisadas; esa autorización posterior permite commit/push a la nueva rama, sin modificar main.

Respaldo anterior a la intervención: `D:\UnityGames\SWIFT_Backups\UXGameplay-20261102-103219`. Las salidas anteriores de sondas se conservaron en `Artifacts/UXGameplayReview/PreviousEvidence/`.

## Pruebas y capturas

- **DomainTests: 27 PASS**. Datos, reglas, puntuación, evidencias, pérdidas y resumen histórico.
- **Revisión UX en Unity: PASS**. Mirada real de la cámara simulada en Editor a 1,5 s; sin activación temprana; cinco objetivos del tutorial alcanzables; pasos completados mediante dwell; activación única al mantener foco, confirmación y rearme; omisión y restauración del monitor.
- Tutorial completo y omitido con Session nula y **cero eventos Changed** del banco: no inicia ni puntúa modos. Se verificó que intentar usar un portal durante la práctica no inicia un modo.
- TMP sin desbordes en menú, pasos, retícula y pantallas comprobadas. Cuenta/monto/datos existentes se conservan.
- Diez Notify y refrescos de UI sin audio adicional; una señal de evidencia nueva frente a registro duplicado; nueva alerta no repetida al consultar varias veces; una señal de cierre por turno comprobado.
- Secuencia coherente: **6/6 y pérdida 0**; ataque aprobando todo **965.000**, defensa con controles **0**, sin cambiar cálculos.
- `GameplayFlowProbe`, `BankOfficeProbe` y `PrototypeSmokeProbe`: **PASS en el cierre final**; los recibos finales están en `Artifacts/UXGameplayReview/`. Incluyen prólogo omitido/12 s, ambos modos, botones, cinco funciones, nueve roles de raycast, cuatro pads y dwell/latch/rearme de la sonda.

Galería: `Docs/Previews/UXGameplayReview/20261102T205325756Z/README.md`: menú, progreso, confirmación, rearme, decisión de práctica, tutorial completo, expediente y comparación de resultados. Son renders reales de Play Mode a 1600 × 1000, con cámara temporal. No son capturas estereoscópicas ni pruebas Android. Hashes, fechas y recibos acompañan las imágenes.

## Problemas encontrados y pendientes

El aviso inicial de rearme era pequeño; se amplió tras ver la captura y la revisión se repitió. El panel del tutorial debe colocarse antes del obstáculo frontal para no quedar detrás de computadora; se añadió ese ajuste sin mover al jugador. El tutorial es opcional y no intercepta el flujo existente portal → prólogo → turno, preservando las pruebas y resultados.

Pendientes: prueba física HONOR/Cardboard, comodidad angular del tutorial, contraste de punto/anillo en ambos ojos, volumen/localización real, pausa/reanudación con audio, rendimiento/memoria/temperatura y APK nueva con estos cambios. No se realizó audición ni medición de rendimiento del dispositivo. No se da por cerrado ningún requisito de hardware por los PASS del Editor.

## Recursos que faltan y siguiente paso

No hace falta arte externo para estas funciones. Se necesitan clips finales con autor/fuente/licencia documentados: confirmación, no disponible, atención/nueva alerta, consulta/archivo, papel/evidencia, defensa, teletransporte y cierre; además ambiente en bucle, música opcional e impresora del prólogo. Entregar SFX WAV limpios; ambiente/música con bucle sin clics. Las referencias de la biblioteca permiten reemplazar los tonos sin modificar la lógica.

Siguiente: publicar la rama autorizada con dependencias, generar APK nueva, probar tutorial/omisión/retícula/portal/decisiones en HONOR y sustituir señales temporales tras revisar los audios. No se añaden niveles ni reglas bancarias.
