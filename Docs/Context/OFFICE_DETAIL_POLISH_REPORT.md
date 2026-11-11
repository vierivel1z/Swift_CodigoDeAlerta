# Pulido de detalles de la oficina — 11 de noviembre de 2026

## Estado y alcance

Se continuó la tarea de corregir elementos visuales señalados en capturas del usuario: cierre de turno apretado, libros flotantes, rótulos casi blancos, estantes vacíos y feedback rígido. Se revisó el estado de Git antes de modificar y se conservaron los cambios locales previos. No se tocaron reglas bancarias, URP, Cardboard ni se añadieron dependencias o modelos externos. Sin commit ni push.

## Cambios aplicados

- **Cierre físico:** pantalla y carcasa ampliadas 1,34 veces, con collider alineado. Cabecera, acción «CERRAR TURNO» y condición de uso tienen espacios y tamaños diferenciados. El objetivo `BankAction.End` sigue siendo único. El rótulo de supervisión se llevó a la entrada y se redujo para que identifique el lugar sin ocupar toda la pared.
- **Monitor principal:** fila inferior más alta (50 a 60 unidades UI); «CERRAR TURNO» pasa de 330 a 390 de ancho y de fuente 23 a 28. Las cuatro acciones conservan sus `BankAction` y raycasts. Los márgenes permanecen dentro del canvas de 1600 × 900.
- **Libros y estantes:** se corrigieron las posiciones de grupos que flotaban sobre los cubículos, la mesa de visitantes y supervisión. La estantería reutiliza tres instancias adicionales del prefab Kenney ya presente, apoyadas en los niveles medidos del modelo. Esas copias decorativas carecen de collider y usan light probes para no heredar un lightmap incorrecto.
- **Rótulos:** nueve superficies claras de la oficina pasan a azul petróleo con texto crema o cian y dos reglas discretas. Se aumentó la lectura del rótulo de evidencia. No se agregaron texturas grandes ni luces nuevas.
- **Feedback de objetos:** la fuerza del color de foco y confirmación se interpola con `Time.unscaledDeltaTime` y `MaterialPropertyBlock`; la cuenta de confirmaciones y la interacción siguen iguales.

## Archivos y recuperación

- Escena integrada: `Assets/_Project/Scenes/BancoCorresponsal.unity`.
- Presentación y feedback: `Assets/_Project/Scripts/UI/BankDocumentUI.cs`, `Assets/_Project/Scripts/DeskTools/DeskObjectFeedback.cs`.
- Herramienta de revisión y sonda enfocada: `Assets/_Project/Editor/OfficeDetailPolish.cs` y su `.meta`, `Assets/_Project/Editor/ImmersiveEditorBridge.cs`, `Assets/_Project/Scripts/Testing/ImmersivePlayProbe.cs`.
- Copia aislada con GUID propio: `Assets/_Project/Scenes/Review/BancoCorresponsal_DetailPolish_v3_20261110.unity` y su `.meta`.
- Respaldo previo de escena, meta e iluminación: `Artifacts/OfficeDetailPolish/Backup-20261110-222125`. Copia del feedback anterior: `Artifacts/OfficeDetailPolish/DeskObjectFeedback.before.cs`. Versión reconstruida, con las cuatro líneas exactas anteriores de la fila inferior: `Artifacts/OfficeDetailPolish/BankDocumentUI.before-bottom-row.cs`.
- Dos variantes previas se descartaron tras inspección visual, con archivos y capturas conservados en `Artifacts/OfficeDetailPolish/DiscardedTrials`. La primera dejó libros fuera del estante al medir límites después de instanciarlos; la segunda dejó un cartel de supervisión demasiado grande.

## Evidencia y pruebas

La [galería comparativa](../Previews/OfficeDetailPolish/README.md) tiene ocho pares de capturas reales de la escena principal desde posiciones iguales. Se abrieron y revisaron las vistas de documentación, estantería, supervisión y cierre cercano. Además hay capturas Play Mode del monitor activo y resultados desde la silla. La escena final se guardó con Unity Editor; SHA-256 `8DAFB3EED14CDC545E33A42EBF036F6E474C9CBB42892E06B13DBB3B30616A53` al capturarla. Este hash identifica la escena, no el código C# editado después.

Después de integrar la escena pasaron en Editor `BankOfficeProbe` (`office`), `GameplayFlowProbe` (`flow`) y `PrototypeSmokeProbe` (`smoke`); recibos en `Artifacts/OfficeDetailPolish/`. La sonda de objetos volvió a pasar después de editar el feedback; su nueva galería se guardó en `OfficeDetailPolish/After/objects` y se restauró byte a byte la galería histórica del repositorio. `DomainTests` pasó 27/27.

La sonda nueva `closeui` pasó: objetivo 390 × 60, fuente TMP 28 sin desborde, raycast desde la silla, una activación por dwell real, requisito existente de resolver todos los expedientes y cierre automático al resolver el último. Su primer intento falló porque **la sonda** esperaba cierre manual después del último expediente; el código de juego ya hacía cierre automático. La expectativa se corrigió sin tocar la regla. `stationpc` falló una vez en el hold sintético del ratón y pasó al repetir en sesión limpia; ambos registros se conservan en `Artifacts/ImmersiveGameplay`. Tras el ajuste final del monitor, `office`, `flow` y `smoke` se repitieron y pasaron. Recibos finales: `Artifacts/OfficeDetailPolish/{objects,closeui,stationpc,office,flow,smoke}-result.txt`. Todas estas pruebas son solo de Editor. Las capturas de escena y Play Mode son monoscópicas.

## Pendientes de calidad

1. Verificar en HONOR la lectura binocular del cierre físico y del botón del monitor, alcance por mirada, separación angular y comodidad desde cada teletransporte. La prueba del Editor no certifica Cardboard físico.
2. Revisar con el usuario si la señalización de supervisión y la densidad de libros son suficientes; hay estantes deliberadamente libres para evitar decoración repetitiva.
3. Perfilar FPS, temperatura y memoria en Android. No se hizo una APK nueva en esta pasada.
4. Sustituir el feedback visual temporal por el audio final cuando haya clips con licencia; evaluar las animaciones en visor antes de aumentar intensidad.

No se declara coincidencia exacta con una referencia ni aceptación visual final. La vista amplia `05-close-turn` está parcialmente ocluida por la partición; usar `08-close-turn-near` para valorar el texto.
