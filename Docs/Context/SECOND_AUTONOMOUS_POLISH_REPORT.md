# Segunda ronda autónoma: calidad, inmersión y optimización

**Proyecto:** SWIFT: Código de Alerta · BancoCorresponsal · Unity 6000.3.24f1 · URP 17.3.0 · Android/Google Cardboard.  
**Fecha local:** 2026-11-10. **Rama:** `review/portal-dwell-feedback`. **Estado:** cambios locales sin commit ni push.

## Alcance y estado de partida

Se revisaron el Kanban, la auditoría de estado, el avance inmersivo, el informe de dirección artística y `git status` antes de intervenir. Ya había cambios locales importantes en la escena, iluminación, galería y herramientas de Editor. Esta ronda **no editó BancoCorresponsal, URP, Cardboard, paquetes ni los seis expedientes serializados**. Las copias de los tres scripts modificados antes de editar están en `Artifacts/SecondAutonomousPolish/Backup-20261110/`. `Assets/_Recovery/` y los cambios artísticos previos permanecen intactos.

Prioridades que surgieron de la evidencia: (1) reconstrucción innecesaria del documento ante avisos idénticos; (2) resultados que mezclaban decisión elegida, efecto del control y estado final en una línea pequeña; (3) desbordes con importes e identificadores largos; (4) un objeto equivocado podía recibir la respuesta visual de la impresora; (5) verificar tutorial, ratón, objetos y flujo sin atribuir pruebas del Editor al visor.

## Mejoras implementadas

| Zona | Cambio | Evidencia |
| --- | --- | --- |
| UI documental | `BankDocumentUI.Refresh` evita reconstruir sus tarjetas cuando sesión, expediente, página, mensaje, evidencias y cifras siguen iguales. La actualización explícita invalida el caché. `UIManager` conserva las tres referencias de decisión y cambia su visibilidad solo cuando corresponde. | 20 avisos iguales: 780 `RectTransform` adicionales antes, 0 después, medidos inmediatamente en Play Mode. |
| Expediente | El monto admite 13 dígitos en el caso de estrés. Los nombres y cuentas que no caben muestran puntos suspensivos sin bajar de 28 px; el detalle paginado conserva el dato íntegro. El acceso se llama **VER DETALLE**. | Cuatro desbordes TMP antes; cero en la repetición final. Capturas 03–05. |
| Resultados | Seis tarjetas de dos líneas distinguen acción elegida, estado efectivo, intervención del control, cantidad de fuentes y llamada. La nota explica que la defensa es contrafactual. Las tarjetas superiores conservan puntuación, pérdida, evidencias y comparación Ataque/Defensa. Se leen datos ya calculados; no se recalcula el resultado bancario. | Capturas 06 y 08, pruebas de ambos modos y evidencia registrada. |
| Revisión educativa | Etiquetas más breves agrupan fuentes y llamada y evitan una segunda página casi vacía en el caso normal; las observaciones extensas siguen paginadas. | Captura 07 y caso de texto largo. |
| Impresora | La respuesta visual ahora se adjunta al objeto **Interactive Printer**, no al primer objeto de tipo `Evidence` que Unity enumere. | El primer `objects` falló por esta causa; tras la corrección, `objects` pasó. |
| Sonda de tutorial | La prueba guarda el nombre del objetivo antes de activarlo, pues el tutorial destruye objetivos completados. El fallo era de lectura de un objeto destruido en la propia sonda. | `tutorial` pasó en la repetición; 41 comprobaciones previas ya habían pasado antes del fallo de la sonda. |

**Archivos de código de esta ronda:** `Assets/_Project/Scripts/UI/BankDocumentUI.cs`, `Assets/_Project/Scripts/UI/UIManager.cs`, `Assets/_Project/Scripts/DeskTools/DeskInteractionCoordinator.cs`, `Assets/_Project/Scripts/Testing/ImmersivePlayProbe.cs` y la sonda nueva `Assets/_Project/Editor/SecondPolishReview.cs` con su `.meta`. El puente de Editor, la escena y sus lightmaps figuran modificados en Git por la ronda artística anterior; esta ronda no los editó.

## Pruebas y mediciones

La sonda nueva abrió la escena principal guardada y limpia, entró en Play Mode, ejercitó la estación y generó pares de capturas desde las mismas poses. Para estrés sustituyó **solo datos en memoria durante Play**: banco y beneficiario extensos, cuenta larga, monto de 13 dígitos, fecha opcional ausente y observaciones de varias páginas. Comprobó regreso al expediente tras cambios rápidos de pestaña, apertura del portapapeles, salida de la silla con panel abierto, reentrada, fuentes y resultados. El estado serializado de los seis expedientes no cambió. Recibos y comprobaciones completas: `Artifacts/SecondAutonomousPolish/Before/` y `After/`.

| Medida en Editor | Antes | Después | Interpretación |
| --- | ---: | ---: | --- |
| 20 avisos idénticos, tiempo síncrono de llamada | 131,151 ms | 0,118 ms | Una muestra por estado; indica eliminación de reconstrucción repetida, no FPS. |
| `RectTransform` inmediatamente después de 20 avisos | 409 → 1189 | 409 → 409 | Antes había 780 objetos UI transitorios; después ninguno. |
| Desbordes TMP en expediente de estrés | 4 | 0 | Se mantienen previews abreviadas y detalle completo. |
| Memoria total asignada informada por Unity | 447 772 172 B | 448 230 865 B | No demuestra reducción de memoria; snapshots de estados distintos. |

La lectura `ManagedAllocatedBytesCurrentThread=0` no se usa como prueba de ausencia de GC: no cubre necesariamente el trabajo de Unity ni las destrucciones diferidas. Las estadísticas de escena de la revisión artística previa fueron 285 `MeshRenderer`, 53 materiales distintos usados, 32 632 triángulos, 17 luces horneadas, 2 en tiempo real, 1 lightmap y 1 reflection probe. Esta ronda no cambió mallas, luces, sombras, transparencia, texturas ni calidad global. **No se midieron** draw calls, overdraw con Frame Debugger, tiempos de GPU por ojo, GC de una sesión larga, memoria de GPU o FPS en HONOR. El perfil Android y la visualización binocular siguen pendientes.

**Regresión final:** DomainTests 27/27; `flow`, `office`, `smoke`, `regression`, `clipboard`, `phone`, `objects`, `tutorial` y `stationpc` pasaron en el Editor. `flow` incluye prólogo completo y omisión, ambos modos y decisiones. `office` cubre raycasts y teletransportes. `stationpc` comprobó que el ratón no acumula dwell oculto, que soltar reinicia y que mantenerlo no activa dos veces. `tutorial` comprobó completar, repetir, omitir y no crear una sesión bancaria. No se observaron errores nuevos de compilación o ejecución en la sonda final. Recibos en `Artifacts/SecondAutonomousPolish/*-result.txt` y `domain-tests.txt`.

Se conservaron los fallos intermedios: `objects-initial-failure.txt` detectó la impresora mal asociada y quedó corregido; la primera ejecución de `tutorial` falló por acceso de la sonda a un objetivo destruido después de 40 comprobaciones superadas y se repitió con PASS. La vieja `UXGameplayReview.review` busca `TutorialStart` mientras la estación está en reposo, una suposición desactualizada; no se contó como prueba superada. La sonda actual del tutorial verifica el recorrido real desde la estación.

## Inspección visual y límites

La [galería de esta ronda](../Previews/SecondAutonomousPolish/README.md) contiene imágenes reales de Unity Play Mode, monoscópicas. Se revisaron el portal, prólogo, expediente, datos largos, observaciones, resultados, revisión educativa y resultado con evidencia; las iteraciones de corrección permanecen en `Artifacts/SecondAutonomousPolish/AfterIteration*/`. La vista de datos largos muestra el importe completo y previews con elipsis; la vista de resultados distingue la elección original del estado resultante del control. La revisión educativa normal cabe en una página.

El portal y el prólogo conservan su composición y funcionamiento existentes. La sonda confirmó arranque, omisión y duración completa del prólogo. **En la captura frontal el modal oculta casi todo el panel del portal**, aunque su pedestal permanece visible: sigue pendiente conciliar la solicitud de mantener el panel reconocible detrás con la legibilidad del prólogo, y comprobar el resultado en visión binocular. El exterior nocturno ya fue trabajado en la [ronda artística](ART_DIRECTION_AUTONOMOUS_REPORT.md); no se añadió otra ciudad porque no había evidencia de mejora suficiente que justificara más geometría. Los objetos existentes mantienen transiciones breves sin mover artificialmente la cámara; esta ronda corrigió la identificación de la impresora, no introdujo animaciones nuevas sin un problema reproducido.

## Pendientes y orden propuesto

1. **HONOR con visor Cardboard:** comprobar lectura binocular de texto y elipsis, tamaño aparente de botones, asiento, giro al teléfono/portapapeles, retícula, cancelación de dwell, comodidad de teletransportes, prólogo y resultados. Probar una sesión completa en Ataque y Defensa. El Editor no sustituye estas pruebas.
2. **Perfilado móvil:** Development Build + Profiler conectado o captura equivalente; registrar FPS por ojo, CPU/GPU, draw calls, overdraw y GC durante portal, estación, resultados y 10 minutos de juego. Optimizar solo el cuello medido; evitar bajar calidad global por conjetura.
3. **Prólogo y aceptación visual:** mantener un fragmento reconocible del portal detrás del modal sin reducir los textos narrativos; validar ambas capas en el visor. Si las filas de resultados resultan pequeñas en HONOR, ampliar el panel o paginar resultados sin reducir fuente.
4. **Audio y tacto de interacción:** probar en dispositivo volumen y localización de UI, objetos y resultados; incorporar clips finales con licencia cuando estén disponibles. Cardboard no aporta por sí mismo mandos o seguimiento de manos.
5. **Pruebas antiguas:** actualizar `UXGameplayReview.review` para el flujo actual de reposo de estación si se quiere volver a usar como gate; conservar sus capturas y fallo previo como histórico.

No se generó APK en esta ronda y no se hicieron pruebas Android. No hubo commit, push, merge ni despliegue.
