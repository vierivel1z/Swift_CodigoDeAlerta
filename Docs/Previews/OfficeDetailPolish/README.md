# Pulido de detalles de la oficina — comparación real de Unity

Capturas de **Unity 6000.3.24f1, Editor, URP, vista monoscópica**. `Before` y `After` usan las mismas ocho posiciones y direcciones de cámara. Cada carpeta incluye un `capture-receipt.txt` con la ruta y SHA-256 de la escena y de cada PNG. Estas vistas no sustituyen una prueba binocular en HONOR.

| Vista | Antes | Después | Qué comprobar |
| --- | --- | --- | --- |
| Oficina | [01](Before/01-office.png) | [01](After/01-office.png) | Coherencia del conjunto y carteles |
| Cubículos | [02](Before/02-cubicles.png) | [02](After/02-cubicles.png) | Libros apoyados sobre mobiliario |
| Documentación | [03](Before/03-documentation.png) | [03](After/03-documentation.png) | Rótulo oscuro, texto claro y estantes |
| Supervisión | [04](Before/04-supervision.png) | [04](After/04-supervision.png) | Identificación de la zona |
| Cierre, vista amplia | [05](Before/05-close-turn.png) | [05](After/05-close-turn.png) | Entrada y partición; esta vista queda parcialmente ocluida |
| Estantería | [06](Before/06-reference-shelf.png) | [06](After/06-reference-shelf.png) | Ocupación de niveles sin objetos flotantes |
| Evidencia | [07](Before/07-evidence.png) | [07](After/07-evidence.png) | Contraste de etiquetas |
| Cierre, cerca | [08](Before/08-close-turn-near.png) | [08](After/08-close-turn-near.png) | Tamaño y separación de título, acción y condición |

La fila inferior del monitor principal se ajustó después de esta serie de capturas de la escena. [Monitor activo](Runtime/close-turn-monitor.png) y [resultados](Runtime/close-turn-results.png) son capturas reales adicionales de Play Mode tomadas desde la silla. La sonda verificó el tamaño de `CERRAR TURNO`, su TMP sin desborde, el raycast y el dwell real. [Objetos](After/objects/capture-receipt.txt) conserva las capturas nuevas de la sonda de interacción. Aún requiere inspección en el dispositivo.
