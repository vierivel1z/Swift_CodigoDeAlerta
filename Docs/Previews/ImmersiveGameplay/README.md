# Revisión de herramientas inmersivas — 10/11/2026

Capturas reales de Unity 6000.3.24f1 / URP 17.3.0. `20261110` contiene las capturas finales de BancoCorresponsal; `20261110-Trial` conserva la fase aislada. PC y tutorial se probaron en la copia aislada; los demás modos se repitieron sobre la principal. Los `capture-receipt.txt` registran escena, pose, hora y progreso; el historial incluye las iteraciones. No son pruebas Android, binocularidad ni rendimiento.

## Antes y después

- [Monitor anterior](20261110/baseline/before-main.png) y [monitor después, sin inspección](20261110/clipboard/after-main.png): misma cámara URP, ojo (0; 1,25; 0,05), dirección (0; 0,17; 0,99). La interfaz bancaria existente se conserva.
- [Portapapeles anterior](20261110/baseline/before-clipboard.png): objeto físico reutilizado. El PNG `before-gameview-gameview.png` de la captura inicial se excluye de la comparativa porque la toma diferida no conservó la orientación; se mantienen el archivo y su recibo por trazabilidad.
- [Inspección ampliada en Game View](20261110/clipboard/clipboard-gameview-gameview.png): cabecera completa, controles separados de la mesa y monitor principal visible al lado. El encuadre de inspección es distinto al del objeto físico y se identifica como tal.

## Mirada y herramientas

| Estado | Captura |
|---|---|
| Inicio de fijación real | [0 %](20261110/clipboard/clipboard-00.png) |
| Media fijación, alrededor de 0,75 s | [≈50 %](20261110/clipboard/clipboard-50.png) |
| Activación del dwell real | [100 % / apertura](20261110/clipboard/clipboard-100.png) |
| Datos existentes del expediente | [Hoja contextual](20261110/clipboard/clipboard-context.png) |
| Observaciones y fuentes | [Tercera hoja](20261110/clipboard/clipboard-sources.png) |
| Llamada pendiente | [Teléfono](20261110/phone/phone-ringing.png) |
| Subtítulos de prueba | [Game View de llamada](20261110/phone/phone-gameview-gameview.png) |
| Registro explícito | [Consulta registrada](20261110/phone/phone-recorded.png) |
| Documento físico de solo lectura | [Inspección](20261110/objects/document-inspection.png) |
| Impresora | [Vista desde asiento](20261110/objects/printer-feedback.png) |
| Reentrada conservando expediente | [Monitor y secundario](20261110/objects/station-reentered.png) |
| Tutorial existente completado | [Game View, fase aislada](20261110-Trial/tutorial/tutorial-gameview-gameview.png) |
| Elección original y control | [Resultados](20261110/regression/results-choice-and-control.png) |
| Revisión posterior por expediente | [Explicación](20261110/regression/educational-review.png) |

La captura de impresora es lejana desde el asiento: permite ubicarla, pero no certifica la lectura de su placa desde ese lugar. La prueba funcional sí usa raycast y dwell reales. La consulta telefónica no confirma fraude ni autorización. El texto de prueba se distingue de contenido final aprobado.

## Cuatro vistas de contexto

[Sesión real del Editor en edición](../20261110T083754509Z-0bfe48/README.md), con `capture.json` y hashes. Se inspeccionaron oficina, escritorio, computadora y vista cenital. Muestran textos serializados anteriores al arranque; la UI dinámica y el teléfono se crean en Play y se evalúan en las capturas de arriba. La cenital oculta temporalmente el techo y muestra el núcleo central, sin abarcar los extremos completos de la oficina. Ningún encuadre de edición se presenta como prueba de interfaz en ejecución.

## Hallazgos y límites visuales

Se corrigieron cabecera cortada, botón detrás del portapapeles, borde del control tapado por la mesa, apertura telefónica fuera del campo inicial y teléfono colocado en el rayo hacia los papeles. Se mantuvo el tamaño angular del texto al acercar la inspección. Los paneles utilizan gris papel, petróleo y cian, sin nuevas luces, bloom ni texturas. No se observaron materiales rosa en las imágenes inspeccionadas.

Pendiente: aceptación visual del usuario; lectura binocular y giro cómodo en Cardboard; medición de FPS/memoria/calor. La oficina conserva su iluminación, su apariencia low poly y su mobiliario; esta entrega mejora interacción local y no pretende igualar una referencia cinematográfica. Los renders no son mediciones de rendimiento.