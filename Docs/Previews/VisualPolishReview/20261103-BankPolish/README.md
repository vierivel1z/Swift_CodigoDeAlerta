# Pulido art?stico ? BancoCorresponsal

2026-11-03, America/Lima. Unity **6000.3.24f1**, URP **17.3.0**. Rama `review/environment-immersive`, base 1074fdd; escena sin commit. **Implementado y probado en Editor; aceptaci?n art?stica del usuario pendiente; Android NO PROBADO.** Planta conservada: 20 ? 18 m.

[Informe completo](../../../Archive/2026-11-10/Context/VISUAL_POLISH_REPORT.md) ? [Inventario de modelos/licencias](../../../Context/AVAILABLE_3D_ASSETS.md) ? [Hashes y fuentes](capture.json)

## Comparaciones reales

| Vista | Antes | Despu?s |
|---|---|---|
| General | [01](Before/01-general.png) | [01](After/01-general.png) |
| Cub?culos / silla y monitor | [02](Before/02-cubiculos.png) | [02](After/02-cubiculos.png) |
| Silla del jugador | [03](Before/03-silla-jugador.png) | [03](After/03-silla-jugador.png) |
| Documentaci?n | [04](Before/04-documentacion.png) | [04](After/04-documentacion.png) |
| Supervisi?n y cierre | [05](Before/05-supervision-cierre.png) | [05](After/05-supervision-cierre.png) |
| Puesto principal | [06](Before/06-puesto.png) | [06](After/06-puesto.png) |
| Materiales/luz | Inicial ocluida; NO comparable | [07: encuadre corregido](After/07-materiales-luz.png) |
| Nocturna desde entrada | [08](Before/08-nocturna.png) | [08](After/08-nocturna.png) |
| Planta, techo oculto | [09](Before/09-planta.png) | [09](After/09-planta.png) |

Ocho parejas mantienen posici?n, objetivo, FOV y resoluci?n. El detalle 07 inicial quedaba dentro del archivo y se conserva solo como incidencia; no se presenta como comparaci?n v?lida. Las tres im?genes enviadas en el chat se revisaron directamente como casos de silla invertida, bloque de cierre/planta y supervisi?n vac?a; las im?genes Before se generaron de nuevo en Unity, no son copias de esas capturas.

## Play y perfiles existentes

- [Cierre por mirada y bloqueo del turno incompleto](Play/01-cierre-por-mirada.png).
- [Terminal integrada en supervisi?n](Play/02-supervision-terminal.png).
- [Puesto de pie](Play/03-puesto-de-pie.png), [sentado](Play/04-puesto-sentado.png) y [libreta/teclado](Play/05-libreta-teclado.png).
- Misma c?mara y contenido, perfiles URP existentes: [PC](Profiles/PC-same-camera.png) / [Mobile](Profiles/Mobile-same-camera.png). Se restaura el asset original; no se cambian sus configuraciones.

Capturas monosc?picas 1600 ? 900; c?maras de revisi?n temporales a FOV 60?, planta ortogr?fica 11. No se cambia FOV ni rig Cardboard. El techo se oculta/restaura solo para planta. En c?maras externas de detalle se oculta/restaura ?nicamente la ret?cula del observador original para que su aviso de rearme no aparezca de canto; en la captura desde el ojo del jugador se conserva. No se deshabilita el gestor ni se altera su temporizador.

Las iteraciones Placement, Surfaces e Iteration01 se conservan. La subcarpeta FinalTests-functional-details corresponde a c?maras de las sondas previas, no sustituye las vistas art?sticas finales.

## Comprobaciones

PASS: DomainTests **27**, GameplayFlowProbe, BankOfficeProbe, PrototypeSmokeProbe y revisi?n de entorno. Pr?logo omitido y autom?tico (12 s), ambos modos, comparaci?n de resultados, decisiones 6/6, interacciones/raycasts, ocho destinos, retorno, clearance, silla/pose/fade y TMP de seis expedientes. Recibos de sondas en FinalTests.

PASS revisi?n espec?fica: seis sillas hacia su mesa, ruedas a suelo, proporciones/apoyos, cierre por mirada de 1,5 s una sola vez, turno incompleto bloqueado, raycasts sentados de APROBAR/RETENER/DEVOLVER/consulta/libreta/LEVANTARSE, textos sin desborde y transici?n a pie. ?ltimo estado: play-result.txt. Bake final renovado y reflexi?n 128? vinculada; bake-result.txt. Los resultados intermedios fallidos se conservan con nombre iteration/guard/race y est?n explicados en el informe; no se confunden con los finales.

Inventario final: **284 MeshRenderer / 49 materiales distintos / 31.360 tri?ngulos activos** (31.228 en Renderer habilitados), **17 baked + 1 realtime**, **81 sondas** y una reflexi?n baked 128?. Estos n?meros no son draw calls de un frame ni un benchmark Android. profile-comparison.txt conserva instant?neas globales del Editor con sus l?mites. Frame Debugger/Profiler de Android no ejecutados; FPS, confort, memoria y temperatura pendientes de HONOR.

El resultado sigue siendo low-poly y m?s simple que el arte aprobado. Las ventanas son opacas y el panel principal se mantiene grande para lectura; no se promete equivalencia literal ni confort VR sin hardware.

## Recuperaci?n e integridad

Respaldo inicial completo: `D:/UnityGames/SWIFT_Backups/VisualPolish-20261103-130232`; checkpoints por fase en Artifacts/VisualPolish/checkpoints.txt. Se compararon **488 archivos protegidos**, sin cambios en modelos/importadores, URP/XR/configuraci?n, n?cleo bancario/pr?logo/VR/interacci?n, artes y prefabs originales. GUID original conservado. No se hicieron commit, push, merge, reset ni clean. meta-check.json confirma metas nuevas completas y ausencia de GUID duplicados.

No se requiere un nuevo modelo para esta fase. Office Pack conserva 120 mallas pendientes de licencia y cinco duplicados acreditables por fuente individual. La APK, aceptaci?n visual y HONOR quedan pendientes.

Checkpoint final completo: `D:\UnityGames\SWIFT_Backups\VisualPolish-20261103-130232\FinalCheckpoint-20261103T185430Z`. Escena guardada; sin cambios pendientes de guardar en esta entrega. Historial de cambios locales sigue sin commit.
