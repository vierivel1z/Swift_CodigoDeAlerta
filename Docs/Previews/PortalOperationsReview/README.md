# Portal de operaciones — revisión real de Unity

Unity 6000.3.24f1, URP móvil existente, Editor DX11. Capturas PNG de 1600 × 900 realizadas mediante solicitudes de render URP, sin imágenes generadas ni barras preparadas artificialmente. FOV de la cámara existente: 60°. **No es una prueba Android/Cardboard en dispositivo.**

| Captura | Estado |
|---|---|
| [Antes](before.png) | Presentación original: franjas inferiores de geometría, sin llenado UI ligado al dwell |
| [Después, normal](after-normal-0.png) | Dos tarjetas y barras vacías, Editor |
| [0 % en Play](play-0.png) | Mirada fuera de ambas tarjetas |
| [ATAQUE ≈ 50 %](attack-50.png) | 49,07 %; `Image.fillAmount == DwellProgress`; DEFENSA a cero |
| [DEFENSA ≈ 50 %](defense-50.png) | 49,38 %; `Image.fillAmount == DwellProgress`; ATAQUE a cero |
| [DEFENSA 100 %](defense-100-confirmation.png) | Confirmación real después del dwell y antes del prólogo |
| [ATAQUE 100 %](attack-100-confirmation.png) | Confirmación real de la segunda selección |
| [Regreso, 0 %](portal-reset-0.png) | Indicadores reiniciados después del teletransporte |
| [Posición inicial](initial-player.png) | Cámara en (0; 1,60; −6,60), orientada al portal, mismo FOV |

El encuadre de revisión usa (0; 2,35; −4,35). Es una cámara temporal de observación; no mueve la cámara que produce los raycasts. En la vista inicial se usa la posición real del jugador y se orienta la cámara de captura hacia la consola. No se amplió el FOV para encajar el panel.

La última ejecución específica terminó con PASS a las `2026-11-04T01:45:47Z`. Las muestras fueron 0,73598 s para ATAQUE y 0,74070 s para DEFENSA, aproximadamente 0,75 s dentro de la tolerancia de muestreo por frames. El render muestra ese progreso real, no un valor escrito por la prueba.

## Resultados automáticos

| Prueba | Resultado |
|---|---|
| Dwell real de ATAQUE/DEFENSA, imágenes ligadas al contador | PASS Play Mode |
| Apartar la mirada a mitad de selección | PASS; vuelve a cero, sin activar |
| Cambiar ATAQUE → DEFENSA | PASS; sin transferencia |
| Selección con dwell configurado en 1,5 s | PASS ambos modos |
| Confirmación antes del prólogo | PASS |
| Mantener mirada durante 2 s adicionales | PASS; una sola activación |
| Teletransportarse fuera y regresar | PASS; barras a cero |
| Cambio de entrada PC sin heredar tiempo de hover | PASS |
| Referencias del prefab, retícula y desbordes TMP | PASS |
| DomainTests | PASS 27/27 |
| GameplayFlowProbe | PASS; omisión y final automático del prólogo, ambos modos, 6/6 decisiones correctas |
| BankOfficeProbe | PASS; nueve roles alcanzables, defensa y cuatro teletransportes |
| PrototypeSmokeProbe | PASS; modos, resultados, teletransportes y latch/reset |
| Android / HONOR | NO EJECUTADO |

El probe de oficina necesitó esperar la nueva confirmación tanto al entrar en ATAQUE como al entrar en DEFENSA. Se mantuvieron sus comprobaciones de lógica e interacción; el fallo inicial de sincronización y las ejecuciones finales constan en los logs.

## Comparación con la referencia aprobada

Se revisaron estas capturas junto con `Assets/_Project/Art/References/REF_Portal_Operations.png`. Se conserva la composición, jerarquía y separación ámbar/cian. La implementación simplifica iconos, utiliza bordes rectos y evita los reflejos, biseles y bloom del concepto. Las barras de ambas tarjetas no avanzan a la vez: solo representa progreso el objetivo de mirada real.

Desde la posición inicial, los títulos y opciones son identificables; el texto descriptivo es considerablemente menor que en la vista cercana. Su legibilidad binocular y el tamaño angular cómodo siguen pendientes del HONOR. No se declara equivalencia literal con la ilustración ni aprobación artística final.

`receipt.txt` conserva fechas UTC de las capturas y `hashes.json` sus SHA-256. Evidencia automática y logs: `Artifacts/PortalOperations/`. Hubo un cierre nativo de Unity tras la primera ronda satisfactoria; se conservó el log y se repitieron las verificaciones tras reabrir. No volvió a impedir las pruebas finales.

Informe y archivos afectados: [PORTAL_OPERATIONS_REPORT.md](../../Archive/2026-11-10/Context/PORTAL_OPERATIONS_REPORT.md).
