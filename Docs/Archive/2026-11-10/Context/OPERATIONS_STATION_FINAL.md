> **Histórico archivado el 10/11/2026.** Conserva el estado de su sesión; no describe necesariamente la versión actual. Consulta el [índice vigente](../../../README.md).

# Estación de operaciones VR e interfaz final

Fecha: 2026-11-04, Lima. Unity 6000.3.24f1, URP 17.3.0, escena BancoCorresponsal, destino Android/Cardboard. Implementado y comprobado en Editor; aceptación visual del usuario y verificación HONOR pendientes. Rama `review/portal-dwell-feedback`, sin commit ni push.

## Implementación y estados

| Estado | Presentación y funcionamiento |
|---|---|
| De pie | Los dos monitores físicos siguen visibles y encendidos. Escritorio corporativo principal con cuatro accesos visuales, identidad SWIFT, widget de disponibilidad y barra de sistema. Consulta en espera en el secundario. UI bancaria detallada y carteles antiguos de silla ocultos durante Play. Teletransportes habilitados. |
| Monitor enfocado | El collider corresponde al área del monitor principal, delante del collider de la computadora. Aparece ENTRAR A ESTACIÓN, icono, borde cian y barra del dwell real. El escritorio sigue detrás. Cancelar oculta el indicador y limpia la barra sin mover al jugador. |
| Entrada | Una fijación de 1,5 s activa una vez. Confirmación de borde/barra, fade existente de la silla y traslado instantáneo del origen bajo negro, sin animar el descenso de la cámara ni imponer orientación de cabeza. |
| Sentado | Se habilitan ambos monitores detallados. Los teletransportes de suelo quedan bloqueados mediante el sistema de silla existente. |
| Salida | Un solo SALIR DE ESTACIÓN dentro del monitor, separado de CERRAR TURNO, usa el mismo dwell. El fade protege el traslado al punto de pie seguro. Se oculta la UI detallada, vuelven ambos escritorios de reposo y se restauran los teletransportes. |
| Reentrada | Conserva la misma GameSession, expediente, decisiones y pestaña seleccionada. No inicia un turno nuevo. |

Se reutiliza `AnalystSeat` sin modificar su fuente. Se conservan el seguimiento real de cabeza, posiciones de silla y pie, y `KeepPanelInFrontOfComputer`. La pantalla principal mide 2,08×1,17 m dentro del marco existente de 2,22×1,32 m. Pose de prueba sentada: (0, 1,25, 0,05); monitor principal en (0, 1,53, 1,66), a unos 1,63 m. No se desplazaron los modelos de los monitores.

## Cuatro pestañas y referencia artística

Se encontró y abrió el archivo exacto `Assets/_Project/Art/References/REF_Operations_Expediente_Final.png`. No se usó como fondo funcional. La interfaz es Unity UI, geometría de pocos vértices y TMP dinámico.

| Pantalla | Resultado |
|---|---|
| EXPEDIENTE | Marco azul noche/cian, cabecera institucional, modo/ID/estado reales, navegación, tarjeta marfil con iconos y separadores, emisor, beneficiario, cuenta, intermediario y fecha solo cuando existe. Monto destacado. Observaciones compactas, VER MÁS y tres decisiones grandes. |
| REGISTRO | Cuenta, beneficiario, titular, estado y resultado procedentes de la consulta existente. Cuenta ausente comprobada con SIM-004. Sin nueva base de datos. |
| ALERTAS | Tres columnas alineadas con etiquetas propias. Filas ajustadas al número de observaciones, con más espacio cuando son pocas. Se muestran anomalías existentes y la nota de que una coincidencia no confirma fraude. |
| PROCEDIMIENTO | Cuatro tarjetas amplias: leer, contrastar, documentar y decidir. Conserva los textos editables de UIManager y no introduce reglas nuevas. |
| VER MÁS | Datos completos y observaciones, VOLVER y paginación TMP para mensajes extensos. Los campos compactos usan autoajuste con tamaño mínimo y elipsis para no invadir otros bloques. |
| Resultados | Conserva los valores, comparación, historial y explicación existentes dentro del marco compartido. |

Comparación visual realizada abriendo los renders finales: se reproduce la distribución documental, bloque de monto, observaciones laterales, cuatro pestañas y decisiones verde/ámbar/azul. Los bordes son más finos, la iconografía más simple y la cabecera más compacta que en el arte; no se declara equivalencia literal ni aprobación artística. La barra de herramientas añade acceso a evidencia, controles, cierre y salida, manteniendo sus acciones originales.

El secundario conserva Formato/Consulta, campos obligatorios, datos observados y alertas existentes; letra ampliada a 36–40 unidades, sin mostrar la decisión recomendada. Su reposo usa una composición para su proporción nativa, sin estirar un escritorio horizontal.

## Mirada y PC

`VRInteractionManager` sigue siendo el único reloj. Todas las barras leen DwellProgress con Image.fillAmount; no se usa una animación independiente. DocumentFocus dibuja normal/foco/progreso/confirmación, y los controles conservan una activación por fijación. La confirmación usa LastActivationTime existente.

En PC los controles de esta interfaz y la entrada/salida requieren mantener el botón izquierdo 1,5 s; soltar reinicia desde cero. G sigue alternando la simulación de mirada existente y el botón derecho conserva el control de cámara. La entrada Cardboard no necesita pulsación. No se reemplazó el interactor ni se modificó XR.

## Reposo activo e iluminación

El principal muestra un escritorio ficticio sobrio, accesos no interactivos de Expedientes/Registro/Alertas/Procedimiento, terminal disponible y barra SWIFT OS. La única interacción es entrar por el área del monitor; no se creó otro sistema operativo jugable.

La UI usa el shader estándar sin iluminación, por lo que mantiene apariencia de pantalla encendida. Esto por sí solo no emite GI. Además se añadió una luz puntual REAL de corto alcance: intensidad 0,30, rango 1,3 m, cian moderado, sin sombras. No se reemplazaron materiales originales ni se añadió bloom. El perfil existente procesa luces adicionales por vértice y permanece intacto.

La comparación URP apagada/encendida desde la misma pose registra 57.502 píxeles afectados en la región de escritorio (400,600)–(1250,850), con aumento medio RGB de 2,085 sobre 255: efecto sutil, medible y no solamente un dibujo brillante. Datos: `Artifacts/OperationsStationFinal/light-comparison.json`. Los PNG 14/15 conservan ambos estados. No se midieron FPS, temperatura, GPU ni confort Android; los contadores UnityStats son agregados del Editor, no un perfil de dispositivo. Profiler/Frame Debugger de Android pendientes.

No hace falta arte externo para esta implementación. Como refinamiento opcional podría generarse un fondo corporativo abstracto discreto, sin texto ni botones, de 1024×512; no es una dependencia. Todos los iconos actuales se construyeron con geometría UI propia y se reutilizaron los sprites pequeños del portal. No se descargaron recursos ni dependencias.

## Archivos afectados

- `Assets/_Project/Scripts/UI/BankDocumentUI.cs`: composición compartida, cuatro vistas, datos, observaciones y paginación.
- `Assets/_Project/Scripts/UI/UIManager.cs`: recursos compartidos, tipografía y arranque optativo de estación.
- `Assets/_Project/Scripts/UI/DocumentFocus.cs`: borde y relleno real de controles.
- `Assets/_Project/Scripts/UI/OperationsStation.cs` + `.meta` (nuevo): estados, visibilidad, reposo y consulta secundaria.
- `Assets/_Project/Scripts/UI/StationIconGraphic.cs` + `.meta` (nuevo): iconos geométricos propios.
- `Assets/_Project/Scripts/Interaction/VRInteractionManager.cs`: utiliza su reloj existente también para mantener pulsado en controles UI de PC.
- `Assets/_Project/Scripts/Interaction/BankInteractable.cs`: nuevas acciones exclusivamente de presentación para ampliar/volver/paginar; enum ampliado al final para conservar los valores serializados anteriores.
- `Assets/_Project/Scripts/Testing/BankOfficeProbe.cs`: prepara la silla real antes de comprobar botones y sale antes de probar teletransportes. Conserva las verificaciones bancarias y de raycast.
- `Assets/_Project/Editor/OperationsStationReview.cs` + `.meta` (nuevo): migración inicial protegida, probes y capturas optativos por cola local.
- `Assets/_Project/Scenes/BancoCorresponsal.unity`: vínculos de sprites, activación de estación y una luz del monitor. No se sustituyó la oficina ni se sobrescribieron prefabs.
- Copia de recuperación en `Assets/_Recovery/OperationsStation_Before_20261104T060358810.unity` y `.meta`; documentos y galer?a de esta sesión.

Se compararon 45 archivos protegidos con el respaldo: fuentes de banca, GameManager, prólogo, silla, adaptador PC, portal, ajustes URP y ProjectSettings idénticos (`protected-files.json`). Los fuentes de interacción/UI cambiados se detallan en `change-manifest.json`. Las imágenes originales se conservaron.

## Pruebas

| Prueba | Resultado |
|---|---|
| Reposo, foco contextual, 50 % real, cancelación sin movimiento | PASS Editor |
| Entrada única, fade existente, pose sentada y orientación sin imponer | PASS Editor |
| Pestañas, consulta de cuenta ausente, decisiones, VER MÁS/VOLVER | PASS raycast/dwell real |
| Valores largos contenidos y TMP de los seis expedientes | PASS Editor |
| Observaciones extensas: SIGUIENTE/ANTERIOR y registros sin cambios | PASS con mensaje sintético de UI, no datos bancarios nuevos |
| Salir, restaurar teletransportes y reentrar con expediente/pestaña | PASS Editor |
| Cerrar turno independiente e incompleto bloqueado | PASS |
| Entrada/salida PC: hover cero, medio progreso, soltar, recargar | PASS final |
| Portal: mirada y ratón, latch, cancelación, ambos modos | PASS regresiones existentes |
| GameplayFlowProbe | PASS, incluyendo prólogo automático y comparación |
| BankOfficeProbe | PASS, incluyendo roles de interacción y teletransportes |
| PrototypeSmokeProbe | PASS |
| DomainTests | PASS 27/27 mediante .NET |
| Iluminación real frente a apariencia de pantalla | PASS comparación A/B de renders |
| Compilación y errores runtime | Sin nuevos errores en las ejecuciones finales |
| HONOR/Cardboard/APK | NO ejecutado / APK no generada |

Recibos y resultados: `Artifacts/OperationsStationFinal/*-result.txt`. El fallo intermedio del test de ratón se conservó: el dispositivo virtual no se hacía actual en cada frame. Se alineó el test con el patrón de la regresión existente y la ejecución final pasó. También se corrigieron tildes perdidas en una iteración y la proporción del reposo secundario antes de las capturas finales. El historial acumulado de Console no se borró; conserva avisos antiguos y errores intermedios resueltos. No se certifica que todo el historial esté vacío.

## Capturas y respaldo

[Galería completa](../../../Previews/OperationsStationFinal/20261104/README.md): diez vistas requeridas, consulta secundaria, resultados, cuenta ausente, A/B de luz y dos poses reales adicionales. Renders URP 1600×900 generados por el Editor, no capturas Android. Los encuadres de revisión copian la posición real del jugador y orientan una cámara temporal al monitor; las capturas 16/17 conservan también la orientación real. Recibos, hashes y resultados se incluyen en la galería.

Respaldo completo previo: `D:\UnityGames\SWIFT_Backups\OperationsStation-20261104-004925`. Incluye Assets, Docs, ProjectSettings y git status. `EditorBefore.unity` conserva el estado abierto inmediatamente antes de editar la escena. La copia en Assets tiene GUID propio: no utilizar su .meta para sustituir el GUID de BancoCorresponsal. No se hicieron reset, clean, commit, push ni merge.

## Qué comprobar en HONOR

1. De pie: ambos monitores encendidos, sin UI bancaria detallada ni carteles flotantes de silla. La pantalla no debe deslumbrar.
2. Mirar el monitor principal: indicador contextual, borde y progreso uniforme 1,5 s; cancelar a mitad sin desplazamiento.
3. Completar: fade a negro, silla cómoda, seguimiento de cabeza natural y monitores integrados en sus marcos.
4. Leer las cuatro pestañas, datos largos, consulta secundaria y botones; probar VER MÁS, páginas y VOLVER.
5. Resolver algunas órdenes, salir mediante SALIR DE ESTACIÓN, teletransportarse y regresar: mismo expediente y progreso. CERRAR TURNO debe seguir siendo otra acción.
6. Completar Ataque y Defensa, comprobar resultados y que no haya activaciones dobles. Medir rendimiento, calentamiento y comodidad antes de aceptar el diseño VR.
