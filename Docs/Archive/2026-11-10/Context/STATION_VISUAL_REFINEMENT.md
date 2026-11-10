> **Histórico archivado el 10/11/2026.** Conserva el estado de su sesión; no describe necesariamente la versión actual. Consulta el [índice vigente](../../../README.md).

# Refinamiento visual de las pantallas de estación

2026-11-04, Lima. Unity 6000.3.24f1 / URP 17.3.0. Implementado y verificado en Editor. Sin commit ni push; aceptación artística del usuario y HONOR pendientes.

## Cambios realizados

- **Reposo principal:** cabecera centrada SWIFT / CENTRO DE OPERACIONES; ESTACIÓN DISPONIBLE como foco principal; una sola instrucción: Mira el monitor principal para entrar. Cuatro accesos grandes, alineados y dentro de tarjetas propias: Expediente, Registro, Alertas y Procedimiento. Barra discreta Terminal lista. Se eliminaron frases redundantes, iconos sueltos y mensajes pequeños que competían entre sí. Los accesos siguen siendo visuales; el monitor conserva la única entrada contextual existente.
- **Pantalla activa:** cabecera contenida, pestaña seleccionada con fondo persistente y separación más clara entre icono y texto. Documento marfil más ancho, con banco emisor e intermediario a todo el ancho y beneficiario/cuenta en dos columnas. El monto pasa a una cápsula compacta en la cabecera documental. Se elimina el bloque repetido de tres líneas educativas que restaba espacio a los datos. Fecha y todos los valores siguen siendo los existentes.
- **Observaciones y decisiones:** observaciones laterales más compactas con VER MÁS independiente; se conserva la ampliación/paginación. Aprobar, Retener y Devolver tienen alineación uniforme, mayor separación y acentos de foco/progreso coherentes con sus colores. El feedback queda dentro de un canal delimitado, que se oculta en resultados para no tapar la explicación.
- **Pantalla secundaria:** tres tarjetas de apoyo con títulos propios: Formato, Cuenta y beneficiario, Observaciones. Resume datos disponibles y observaciones ya redactadas; el detalle completo permanece en REGISTRO y ALERTAS. Si hay muchos avisos, el resumen indica cuántos más hay. No consulta otra base de datos ni clasifica fraude. El reposo secundario queda reducido a identidad, consulta en espera y estado.
- **Luz y estilo:** se conserva azul noche, cian, ámbar y marfil. La UI estándar mantiene apariencia luminosa y se conserva la luz sutil del monitor ya existente. No se aumentaron luces, bloom, emisión global ni calidad URP. No se modificaron materiales ni se descargaron assets.

## Archivos

- `Assets/_Project/Scripts/UI/OperationsStation.cs`: composiciones de reposo y tarjetas de apoyo; mismos estados y transiciones.
- `Assets/_Project/Scripts/UI/BankDocumentUI.cs`: layout principal, márgenes, monto, observaciones, pestañas y botones.
- `Assets/_Project/Scripts/UI/FormatReferencePanel.cs`: proyección resumida de datos existentes, optativa para el panel de apoyo. Mantiene la presentación anterior como alternativa cuando CompactSupport está desactivado.
- `Assets/_Project/Scripts/UI/DocumentFocus.cs`: color base de pestaña y acentos visuales; continúa leyendo el mismo DwellProgress y LastActivationTime.
- `Assets/_Project/Editor/StationRefinementReview.cs` y `.meta`: nuevo runner local de captura y regresión. No aplica migraciones ni guarda la escena.
- Este informe, galer?a y entradas añadidas al historial de Kanban/CHANGELOG.

BancoCorresponsal, UIManager, VRInteractionManager, BankInteractable, AnalystSeat, GameManager, prólogo, portal, fuentes bancarias, ajustes URP y ProjectSettings protegidos: **49/49 archivos idénticos al respaldo**. Evidencia: `Artifacts/StationRefinement/protected-files.json`. No se editaron escenas ni prefabs; los cambios son de presentación construida en ejecución.

Respaldo anterior a cambios: `D:\UnityGames\SWIFT_Backups\StationRefinement-20261104-080938` (Assets, Docs, ProjectSettings y git status). Permite recuperar selectivamente los cuatro fuentes UI y sus metas sin descartar avances ajenos.

## Capturas comparativas

[Antes/después desde los mismos ángulos](../../../Previews/StationRefinement/20261104/README.md).

Se generaron renders reales nuevos antes de cambiar los layouts y después del refinamiento: principal en reposo, principal activa, secundaria activa y secundaria en reposo. Los cuatro pares tienen exactamente la misma posición y dirección de cámara, FOV 60°, aspecto 16:9 y resolución 1600×900. La pose de pie y la silla son las existentes; las cámaras temporales de documentación miran al monitor elegido. No se alteró el rig de producción para mejorar los encuadres. `capture.json`, `receipt.txt` y `hashes.json` documentan las vistas finales.

Se abrieron y compararon los PNG: el reposo tiene menos mensajes y una jerarquía más evidente; la ficha deja más espacio a los campos; el secundario organiza su contenido en tarjetas. No se declara que la lectura binocular o la comodidad estén certificadas por estas imágenes.

## Verificación

- PASS capturas comparativas y comprobación de las cuatro poses coincidentes.
- PASS Play Mode: entrada contextual, barra real a mitad, cancelar, una activación, fade y silla existentes, cuatro pestañas, VER MÁS/VOLVER, decisiones, salida, teletransportes y reentrada conservando sesión y pestaña.
- PASS textos TMP sin desbordes en las vistas probadas, valores largos contenidos y textos de los seis expedientes. PROCEDIMIENTO conserva el nombre completo.
- PASS ratón: hover sin carga, mantener, soltar/reiniciar y entrar/salir con 1,5 s.
- PASS GameplayFlowProbe, BankOfficeProbe y PrototypeSmokeProbe. El repaso final de presentación y PC se repitió después de los últimos ajustes visuales.
- Sin nuevos errores runtime en las ejecuciones finales. Las pruebas y recibos están en `Artifacts/StationRefinement/`.

Durante la revisión se detectó y corrigió un fondo opaco antiguo que tapaba el nuevo secundario, un desborde del título PROCEDIMIENTO y la interferencia del soporte físico con el pie de pantalla. Los PNG finales se regeneraron y revisaron después. El historial intermedio de recibos y checks se conserva.

No se generó APK ni se realizaron pruebas en Android/Cardboard. Los contadores UnityStats de los recibos son agregados del Editor; no miden coste GPU aislado, FPS ni temperatura del HONOR.

## Recomendaciones pendientes

1. Comprobar en HONOR la lectura del secundario, la pestaña PROCEDIMIENTO y la barra inferior desde la silla real. Ajustar fuentes o distancia solo según esa comprobación binocular.
2. Verificar que el brillo y el contraste de marfil/cian resulten cómodos con el visor. Mantener la luz suave y evitar añadir bloom para compensar lectura.
3. Revisar con Profiler/Frame Debugger en dispositivo el overdraw UI y calentamiento antes de aumentar efectos. No hace falta arte externo adicional para estas composiciones.
