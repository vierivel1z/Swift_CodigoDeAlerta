# Prueba del prototipo en HONOR

Sesión de desarrollo: 2026-11-02. Esta lista describe pruebas **pendientes en dispositivo**. No se considera aprobado Cardboard por superar Play Mode.

## Identificar e instalar la versión

APK de esta sesión: build **Succeeded, 0 errores**, firma v2 verificada; todavía no probada en teléfono.

`D:/UnityGames/SWIFT_CodigoDeAlerta/Artifacts/Functional-20261102-083259/SWIFT-BancoCorresponsal-current.apk`

SHA256: `F59EA07AE5019C2C67A1B6025E9B8DB993DB7E67B7B2B4DD39012DD6C793FB6E`. Tamaño: 48851914 bytes.

Evidencia de compilación, firma y SHA256 en la misma carpeta. No usar `Artifacts/SWIFT-prototype.apk` ni `PruebaVR/Test_Cardboard.apk` para validar la oficina actual. El paquete mantiene `com.atodaolla.swiftcodigoalerta`, versión 0.1.0/código 1; identificar esta compilación por nombre de archivo y hash, no solo por versión.

Copiar la APK al HONOR e instalarla desde Archivos. Confirmar que se abre la oficina bancaria con los portales ATAQUE/DEFENSA y el Centro de Operaciones, no el ejemplo HelloCardboard. Si Android muestra un error de instalación, registrar el mensaje exacto; no desinstalar versiones o borrar datos para ocultar el problema sin revisarlo.

Registrar antes de probar: modelo exacto del HONOR, versión Android/MagicOS, visor usado, SHA256 de APK, fecha y persona que prueba. El build requiere Android API 26 o posterior y ARM64; el seguimiento de cabeza todavía debe comprobarse en este teléfono.

## Controles y arranque VR

1. Abrir en horizontal y comprobar imagen estereoscópica en el visor, sin pantalla negra, rosa o un ojo ausente.
2. Girar la cabeza suavemente: la orientación debe seguirla; la retícula debe permanecer visible. Comprobar parámetros/QR del visor cuando Cardboard los solicite y recentrado.
3. Mirar ATAQUE durante aproximadamente **1.5 segundos**. El primer acceso debe mostrar el prólogo.
4. Dejar terminar los tres momentos del prólogo (unos **12 segundos**) sin pulsar omitir. Debe aparecer un único turno, SIM-001.
5. Después de una decisión por mirada, **mirar fuera del botón y volver a enfocarlo** para otra activación. Mantener la mirada fija no debe resolver varios expedientes seguidos. Este rearme es intencional.

## Recorrido de interacciones

| Acción | Resultado esperado |
|---|---|
| Punto de entrada | Teletransporta a la zona inicial, sin desplazamiento continuo forzado. |
| Punto de inspección | Sitúa frente al monitor; botones completos visibles y seleccionables. |
| Punto de evidencias | Acerca a impresora/archivadores. |
| Punto de seguridad | Permite consultar alertas y activar controles. |
| Computadora/monitor | Muestra o mantiene el expediente y feedback de inspección. |
| Archivador | Muestra cuenta/titular ficticios; en SIM-004 informa cuenta no encontrada. |
| Panel de alertas | Presenta la observación del expediente actual, sin declarar automáticamente fraude. |
| Impresora/evidencia | Incrementa evidencias del expediente; repetir la misma evidencia no debe duplicarla. |
| Consola de defensa | Solo activa controles en DEFENSA y una vez por turno. |
| Cierre de turno | Rechaza cerrar con expedientes pendientes; al resolver seis muestra resultados. |

Probar los cuatro puntos mediante mirada, no solo comprobar que existen. Registrar oclusiones, destinos incómodos y objetos que no responden. Las pruebas automatizadas comprueban posiciones y raycasts PC; este recorrido físico sigue pendiente.

## Dos modos y tres decisiones

**Turno A — ATAQUE:** aprobar los seis expedientes, retirando y devolviendo la mirada entre activaciones. Resultado esperado: pérdida **ficticia USD 965 000**. El panel de defensa no debe habilitar controles en este modo. Los resultados deben ocultar los botones de decisión.

**Turno B — DEFENSA:** entrar por el otro portal; no debe repetirse el prólogo en esa ejecución. Activar controles en seguridad y aprobar los seis. Resultado esperado: pérdida **ficticia USD 0**, con retenciones/devolución por control reforzado y comparación ataque 965000 / defensa 0. La puntuación puede ser **1/6**, porque se evalúa la decisión original del jugador aunque la defensa la corrija; no confundirla con pérdida.

**Turno C — decisiones correctas:** iniciar otro turno y resolver SIM-001 Aprobar; SIM-002 Devolver; SIM-003, SIM-004, SIM-005 y SIM-006 Retener. Esperado: **6/6** decisiones coherentes y pérdida ficticia cero. Verificar que cada botón responde por mirada.

**Omisión del prólogo:** cerrar y volver a abrir la app para una ejecución nueva; entrar por un portal y seleccionar OMITIR INTRODUCCIÓN. Debe iniciarse el modo elegido una sola vez. Los últimos resultados comparativos viven en memoria: al reiniciar la app pueden volver a figurar como pendientes.

## Lectura, sonido y estabilidad

- Leer expediente completo, cuenta, monto, botones y resultados desde el punto de inspección con ambos ojos. Registrar recortes, doble imagen o texto demasiado pequeño.
- Verificar tono audible al cambiar mensajes/decisiones, con volumen cómodo. No esperar música ni sonidos distintos de impresora: aún no están implementados.
- Pasar a otra app o bloquear/desbloquear el teléfono y regresar. Comprobar que pose, imagen, audio y selección siguen operativos.
- Jugar ambos modos durante 10–15 minutos. Registrar tirones, cierres, calentamiento y molestias. Detener la prueba si hay mareo; no declarar un FPS sin medición.
- Verificar separación entre cifras históricas y pérdidas ficticias. Los créditos actuales son breves; su lector completo sigue pendiente.

## Registro de resultado

Para cada caso anotar PASS/FAIL/NO PROBADO, pasos, resultado esperado/observado y captura o video. En un fallo indicar modo, expediente, objeto, punto de teletransporte y si retirar/volver a enfocar cambia el resultado. Entregar este registro antes de cerrar N04, N07, N08, N09 y K19.
