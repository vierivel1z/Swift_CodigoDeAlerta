> **Histórico archivado el 10/11/2026.** Conserva el estado de su sesión; no describe necesariamente la versión actual. Consulta el [índice vigente](../../../README.md).

# Prólogo cercano con seguimiento de cámara

2026-11-03, Lima. Rama `review/portal-dwell-feedback`. Sin commit ni push.

## Problema y corrección

El prólogo se colocaba una sola vez a 2,5 metros de la cámara y quedaba fijo en el mundo. Al seleccionar un modo cerca de la consola, la geometría podía quedar delante del storyboard y ocultarlo.

Ahora la presentación:

- Aparece aproximadamente a **1,1 metros**. Comprueba el centro y las cuatro esquinas contra colliders y acerca el panel si la consola ocupa ese espacio.
- Sigue la posición de la cámara y se recentra suavemente al girar más de 14°. Un pequeño margen estable permite mirar el botón inferior «Omitir» sin que este escape de la mirada.
- Actualiza la posición también antes del render para acompañar el seguimiento de cabeza, sin mover ni reparentar la cámara del jugador.
- Mantiene un tamaño angular consistente al ajustar la distancia y adapta el ancho cuando el aspecto de la cámara es estrecho. No se aumentó el FOV.
- Oculta únicamente el Canvas de ATAQUE/DEFENSA durante el prólogo y lo restaura después, conservando el marco, colliders y controlador de modos.
- Conserva las tres partes, ilustraciones, tiempos de 4 segundos por parte, omisión y llamada al modo seleccionado.

## Archivos y respaldo

- `Assets/_Project/Scripts/Core/IntroSequence.cs`: seguimiento, cercanía, adaptación y protección frente a la consola.
- `Assets/_Project/Scripts/UI/ModeStationVisual.cs`: ocultación y restauración temporal del Canvas del portal.
- `Assets/_Project/Editor/IntroCameraFollowReview.cs` y `.meta`: validación en Play Mode y capturas desde la cámara real.

Respaldo previo: `D:\UnityGames\SWIFT_Backups\IntroCameraFollow-20261103-213513`. No se guardó ni modificó la escena abierta, sus prefabs o los cambios pendientes del usuario. El helper permite probar la escena bancaria abierta sin descartar sus cambios sin guardar.

No se modificaron URP, Google Cardboard, dependencias ni reglas bancarias.

## Verificación en Unity

| Comprobación | Resultado |
|---|---|
| Prólogo delante del portal tras seleccionar ATAQUE | PASS |
| Giro de 55° y desplazamiento de la cámara | PASS: panel vuelve a centrarse, a menos de 2° del eje |
| Posición muy próxima a la consola | PASS: distancia reducida por debajo de 1 m; storyboard visible |
| Cuatro esquinas dentro del encuadre y textos TMP sin desbordes | PASS en las posiciones probadas |
| «Omitir» por raycast y fijación real de 1,5 s | PASS; no persigue la mirada durante la selección |
| Limpieza del prólogo y restauración del portal | PASS |
| GameplayFlowProbe | PASS: omisión, final automático con duración de producción, ambos modos y decisiones |
| BankOfficeProbe | PASS |
| PrototypeSmokeProbe | PASS |

Resultados y logs: `Artifacts/IntroCameraFollow/`. Capturas reales revisadas: [IntroCameraFollow](../../../Previews/IntroCameraFollow/README.md).

**Pendiente:** validación binocular, comodidad y rendimiento en HONOR/Android. No se generó APK en esta intervención. El margen de seguimiento evita un HUD rígidamente pegado a la cabeza, que impediría seleccionar por mirada un botón situado debajo del centro.
