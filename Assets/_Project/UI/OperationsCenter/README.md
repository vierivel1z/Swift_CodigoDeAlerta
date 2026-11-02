# Centro de operaciones — recursos y prefabs

Referencia original localizada: `Assets/_Project/Styles/OperationsCenter_Original.png` (1586 × 992, RGB). Se conserva su SHA-256 en `inventory.json`. Las rutas alternativas indicadas en la tarea no existen en este checkout.

`python Tools/OperationsCenter/extract.py` utiliza Pillow y busca las tres ubicaciones conocidas. También acepta `--source RUTA`. Las coordenadas base corresponden a 1586 × 992 y se escalan por eje si cambia la resolución; otra composición exige revisar manualmente los recortes. No redimensiona los píxeles extraídos ni elimina fondos. Genera 12 controles opacos y un inventario de coordenadas, dimensiones y hashes. La hoja de control está en `Docs/Previews/LightingReview/extraction-contact-sheet.png` y no es una captura de Unity.

Se revisaron completos los biseles de los 12 recortes. El atlas tiene fondos opacos y símbolos incrustados: las pantallas completas, retículas e iconos no se usaron como fondos funcionales. No se inventó un canal alfa. Los controles extraídos quedan como referencia; la interfaz principal reconstruye paneles, bordes y botones con Unity UI/TMP para conservar texto dinámico, proporción y claridad.

## Siete prefabs

| Archivo en Prefabs/ | Uso |
|---|---|
| OC_Expedientes.prefab | Pantalla funcional completa: Canvas World Space, UIManager, textos TMP y tres acciones con BoxCollider. Busca GameManager al iniciar si no se asignó. |
| OC_Header.prefab | Franja de cabecera reutilizable. Añadir texto TMP al componer otra pantalla. |
| OC_Document_well.prefab | Fondo de lectura reutilizable. |
| OC_Feedback_well.prefab | Fondo de estado/feedback reutilizable. |
| OC_APROBAR.prefab | Texto TMP, collider y BankAction.Approve. |
| OC_RETENER.prefab | Texto TMP, collider y BankAction.Hold. |
| OC_DEVOLVER.prefab | Texto TMP, collider y BankAction.Return. |

Los seis componentes parciales se colocan dentro de un Canvas World Space existente. Asignar `BankInteractable.Manager` en los botones usados por separado; el prefab completo enlaza las acciones automáticamente. No duplicar el panel completo encima de la UI ya instalada en BancoCorresponsal. No se generaron siete pantallas ficticias sin lógica: se priorizó un expediente funcional y seis piezas para composición.

No existían `Tools/OperationsCenter/extract.py` ni el generador solicitado en este checkout; se añadieron las herramientas para esta tarea. `OperationsLightingReview.GeneratePrefabs` genera los siete recursos al aplicar o refinar la revisión. Las luminarias y materiales nuevos están separados de los prefabs/materiales anteriores, en `OfficeVisual/LightingReview`.

## Uso en Unity

Abrir `Assets/_Project/Scenes/BancoCorresponsal.unity`. La integración ya está guardada. Pulsar Play y elegir ATAQUE o DEFENSA: tras el prólogo aparecen los datos ficticios y los botones Aprobar/Retener/Devolver. Consultar registro, alertas y evidencias como antes. Los resultados ocultan las decisiones y conservan la separación entre historia de 2016 y defensa simulada.

Para nuevas modificaciones, guardar la escena y usar los menús `SWIFT > Centro de operaciones`. La aplicación inicial se niega a duplicar una instalación existente. El horneado exige una escena guardada. Si las actualizaciones de TMP/importadores dejan cambios pendientes, guardarlos primero y conservar una copia de recuperación. No usar los constructores antiguos para regenerar la oficina.
