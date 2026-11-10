# Oficina ampliada ? capturas reales Unity

Fecha: 2026-11-03, America/Lima. Unity 6000.3.24f1 / URP 17.3.0 / rama `review/environment-immersive`, HEAD 1074fdd, escena local sin commit. Hashes de escena, scripts, iluminaci?n y PNG en [capture.json](capture.json). No es una aprobaci?n visual ni una prueba Android.

**Arquitectura: 16 ? 14 ? 20 ? 18 m, 224 ? 360 m? (+136 m?). Circulaci?n conectada aproximada: 140,625 ? 260,4375 m?.** Cuatro cub?culos, seis zonas funcionales y ocho destinos. M?todo exacto: [informe](../../../Archive/2026-11-10/Context/EXPANDED_BANK_OFFICE_REPORT.md). Las m?tricas son mediciones Unity, no una certificaci?n arquitect?nica.

| Vista | Captura |
|---|---|
| Planta anterior, techo oculto temporalmente | [01](01-planta-antes.png) |
| Planta ampliada, misma c?mara/escala | [02](02-planta-despues.png) |
| Entrada anterior | [03a](03a-entrada-antes.png) |
| Entrada ampliada, c?mara anterior | [03b](03b-entrada-misma-camara.png) |
| Entrada ampliada desde nuevo acceso | [03](03-entrada-ampliada.png) |
| Cub?culos y corredor | [04](04-cubiculos.png) |
| Puesto principal, de pie, Play | [05](05-puesto-principal-de-pie.png) |
| Fondo hacia entrada | [06](06-fondo-hacia-entrada.png) |
| Consola inclinada | [07](07-consola-modos.png) |
| Escritorio sentado, Play | [08](08-escritorio-sentado.png) |
| Iluminaci?n general | [09](09-iluminacion-general.png) |
| Monitor principal, Play | [10](10-monitor-principal.png) |
| Consulta derecha, Play | [11](11-consulta-derecha.png) |
| Libreta y madera, detalle, Play | [12](12-libreta-y-materiales.png) |
| Levantarse, Play | [13](13-levantarse.png) |

C?maras temporales monosc?picas 1600 ? 900, FOV 60? solo de revisi?n; plantas ortogr?ficas de tama?o 11. No se modifica la c?mara Cardboard ni su FOV. Coordenadas/horas de cada captura en capture-receipt.txt. Techo restaurado despu?s de las plantas, ninguna c?mara de revisi?n persistida.

Se abrieron y revisaron im?genes reales de planta, entrada, fondo, cub?culos, consola, iluminaci?n, puesto de pie/sentado y detalles. Se aument? cobertura de sondas/iluminaci?n tras detectar oscuridad; primera iteraci?n conservada en Iteration01-DarkLighting. El detalle inicial de libreta quedaba oculto por el monitor: corregido el encuadre sin mover ni cambiar su l?gica.

Diferencias respecto a los artes: cuatro cub?culos, geometr?a simplificada, ventanas opacas sin reflejos, estilo m?s oscuro/plano y disposici?n adaptada a los cuatro destinos originales. El escritorio mantiene orientaci?n funcional; las perspectivas se ajustan a la sala real y no reproducen exactamente la composici?n del arte. La pantalla ancha exige giro de mirada; validar confort/lectura binocular en HONOR. No se declara coincidencia art?stica ni optimizaci?n Android.

PASS antes/despu?s y final: flow (pr?logo omitido/12 s autom?tico y ambos modos), office (interacciones/raycast), smoke (decisiones/evidencias/registro/alertas/defensa/resultados y dwell/latch/reset), revisi?n espec?fica de ocho rutas/silla/fade/TMP/consulta. Dominio 27 PASS. Recibos en BeforeTests, AfterTests y FinalTests. Play-captures verifica tambi?n los botones desde el asiento y monitor derecho. Los tiempos son UTC reales de ejecuci?n.

Un intento de bake encontr? escena sucia tras importaci?n y se detuvo de forma segura: bake-save-guard-result.txt conserva ese rechazo. Se cre? checkpoint expl?cito antes de continuar. Segundo bake final PASS; 12 luces baked, una realtime activa y 75 sondas. Una entrada lightmap activa final; las copias anteriores y sus mapas siguen en el respaldo externo. No hay errores nuevos de compilaci?n en Editor.log; Android, FPS, draw calls, memoria y temperatura NO MEDIDOS.

Respaldo inicial: `D:/UnityGames/SWIFT_Backups/ExpandedBankOffice-20261103-004324`. Copias por etapas registradas en Artifacts/ExpandedBankOffice/checkpoints.txt. Sin commit/push/merge, main intacta. No hay modelos imprescindibles pendientes; texturas y audio final opcionales/independientes.
