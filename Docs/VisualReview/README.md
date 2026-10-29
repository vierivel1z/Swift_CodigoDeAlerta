# Diseñar y revisar desde el celular

La nube puede preparar código, documentación y cambios revisables por Git. **Unity local instalado y activado es quien importa, compila, modifica la escena y produce las imágenes reales**. El teléfono sirve para revisar GitHub y enviar comentarios; no necesita Unity ni MCP. No se ha generado ninguna captura nueva durante esta implementación.

## Primera ejecución local

1. Trae la rama `review/unity-scene-designer` cuando sus cambios se hayan subido de forma autorizada, o transfiere el diff local. No hay push automático. Usa Unity Hub para abrir el checkout con **6000.3.24f1** y conserva **URP 17.3.0** y los paquetes existentes.
2. Espera resolución de paquetes/importación y confirma que Console no tiene errores. La herramienta no añade paquetes y está bajo `Editor/`, excluida del player Android.
3. Abre `Assets/_Project/Scenes/BancoCorresponsal.unity` como única escena. Sal de Play y guarda tus cambios. Antes de editar la escena principal, conserva un commit o copia recuperable con los assets relacionados; no la reemplaces por una regeneración sin respaldo.
4. Abre **SWIFT > Revision visual > Camaras y capturas PNG**. Hay cuatro cámaras configurables por posición, destino, FOV o tamaño ortográfico. Puedes encuadrar en Scene View y pulsar **Usar encuadre de Scene View** en la perspectiva correspondiente. Las cámaras solo existen durante la captura.
5. Pulsa **Generar cuatro PNG y reporte**. Solo considera éxito la salida `SWIFT_REVIEW_CAPTURE_PASS`, las cuatro imágenes válidas y `capture.json`. La herramienta utiliza `UniversalRenderPipeline.SingleCameraRequest`; comprueba que el renderer activo lo soporte. No cambia pipeline, rig, iluminación ni calidad. Desactiva postprocesado y AA para las cámaras de revisión; conserva sombras según el perfil activo, registrado en el reporte. No equivale a una captura del perfil Android si está activo el perfil PC.
6. Abre los PNG, verifica iluminación, orientación, encuadre y texto. La vista superior oculta solo los Renderer llamados `Ceiling` durante la captura y restaura su estado incluso al fallar. Si se renombra el techo, actualiza esa selección tras inspeccionar la jerarquía. No desactives el techo permanentemente para conseguir una vista.
7. Verifica que no quedaron cámaras temporales, que el techo volvió y que escena, prefab, pipeline y XR no cambiaron por capturar. Si el Editor marca la escena como modificada, inspecciona el diff antes de guardarla. La herramienta nunca llama SaveScene ni los constructores/regeneradores existentes.

No se ha compilado ni ejecutado esta herramienta en Unity en la nube. Su primera compilación y sus cuatro encuadres deben validarse localmente. Si hay error de compilación/render, conserva Console/log y corrige la herramienta; no desactives URP ni inventes PNG.

## Automatizar la captura sin MCP

Con Unity cerrado para ese checkout, Editor activado y GPU/dispositivo gráfico funcional, desde la raíz del repositorio en PowerShell:

```powershell
$editor = 'C:\Program Files\Unity\Hub\Editor\6000.3.24f1\Editor\Unity.exe'
& $editor -batchmode -quit -projectPath (Get-Location).Path `
  -executeMethod Swift.EditorTools.SceneReviewCapture.CaptureBatch `
  -logFile (Join-Path $env:TEMP 'swift-review-capture.log')
if ($LASTEXITCODE -ne 0) { throw 'Falló Unity: revisar el log.' }
Select-String -Path (Join-Path $env:TEMP 'swift-review-capture.log') -Pattern 'SWIFT_REVIEW_CAPTURE_PASS'
```

Usa la ruta real de tu instalación; Linux/macOS emplean el mismo `-executeMethod` con su ejecutable. **No uses `-nographics`**. No ejecutes dos editores sobre el mismo proyecto. El método batch abre la escena guardada y restaura el setup de escenas al terminar; no crea ni modifica la oficina. Al fallar borra su sesión parcial y conserva revisiones completas anteriores.

Cada galería contiene 1600 × 1000 píxeles por imagen, hashes PNG, hash del archivo de escena y hash de dependencias. Guarda escena/prefabs/materiales antes de capturar; cualquier cambio posterior exige repetir. Añade en el PR el commit que contiene el estado capturado (el hash de escena no sustituye al commit ni a un perfil de rendimiento).

## Revisión privada por GitHub

1. Para cada modificación importante usa `review/<tema>`; esta preparación usa `review/unity-scene-designer`. Conserva un estado recuperable antes de cambiar escenas y evita que dos personas editen la misma escena a la vez.
2. En Codex local invoca `$unity-scene-designer` y describe el cambio. Aplica, guarda, captura y compara las cuatro vistas. Registra resultados en [CHANGELOG.md](CHANGELOG.md). La skill también es utilizable sin MCP: el usuario ejecuta los menús del Editor.
3. Examina `git diff` y `git status`. Selecciona explícitamente los archivos del cambio: escenas, materiales, prefabs y sus `.meta`, código, documentos y una sesión completa de `Docs/Previews`. No añadas Library, Temp, credenciales, APK o logs.
4. Cuando autorices compartir la revisión, haz commit y push **a la rama de revisión**, no a main. Abre un PR en borrador. No se ha hecho ninguno de estos envíos automáticamente. Usa la [plantilla de revisión](../../.github/PULL_REQUEST_TEMPLATE/visual-review.md).
5. Incluye una galería en el cuerpo del PR con enlaces a la rama/sesión concreta. Ejemplo tras subir (sustituye `SESION_REAL`):

```markdown
[Galería de esta revisión](https://github.com/vierivel1z/Swift_CodigoDeAlerta/blob/review/unity-scene-designer/Docs/Previews/SESION_REAL/README.md)
![Oficina](https://github.com/vierivel1z/Swift_CodigoDeAlerta/blob/review/unity-scene-designer/Docs/Previews/SESION_REAL/01-oficina.png?raw=true)
```

La galería de la sesión ya contiene las cuatro imágenes con enlaces relativos. No hace falta GitHub Pages, bucket público, servicio MCP remoto ni GitHub Actions. Un enlace a un repositorio privado exige iniciar sesión con una cuenta que tenga acceso. No publiques el repositorio para ver las imágenes.

6. Desde GitHub Mobile o el navegador del teléfono abre **Pull requests → revisión → galería**, abre cada PNG y amplía computadora/expedientes. Si la app no renderiza Markdown o PNG, usa **Open in browser** con la misma cuenta. Compara sesiones concretas, no solo LATEST.
7. Comenta en el PR con la plantilla de abajo. Después, indica a Codex que procese ese PR/comentario o copia el pedido al chat: los comentarios no ejecutan Codex ni Unity por sí solos. Codex nube puede preparar trabajo y revisar PNG existentes; la siguiente captura requiere tu Editor local. El equipo debe mantenerlo abierto/conectado para usar MCP.
8. Se responde con otra sesión, comparación y pendientes. El PR continúa como borrador hasta tu revisión. Solo se hace merge tras aprobación explícita; no se publica automáticamente cada modificación.

## Cómo solicitar una corrección

```text
Usa $unity-scene-designer en review/<tema>.
Sesión/PR: <enlace>. Imagen: 02-escritorio.png.
Objeto o zona: escritorio principal, lado izquierdo.
Cambio: mover la pila de expedientes 30 cm hacia la izquierda.
Conservar: monitor y botones visibles desde Teleport Inspection.
Criterio: no tapar la pantalla ni invadir el acceso a la silla.
Genera las cuatro vistas reales y compara antes/después si Unity está disponible.
No hagas push ni merge todavía.
```

## Validación local pendiente

- Compilación con Unity 6000.3.24f1/URP 17.3.0 y salida válida de las cuatro vistas, sin renders negros/rosas ni PNG vacíos.
- Capturar dos veces: dos sesiones, hashes/reportes actuales y LATEST enlazando la última completa; ninguna revisión anterior sobrescrita.
- Fallo controlado (escena sin guardar o incorrecta): rechazar sin reemplazar LATEST ni crear una revisión parcial. Cerrar ventana/reabrir para confirmar configuración de encuadres.
- Verificar restauración de techo, RenderTexture activo y ausencia de cámaras temporales. Revisar Git antes/después de capturar.
- Comparar con requisitos visuales: general completa, escritorio despejado, monitor/expedientes legibles y planta útil. Confirmar UI dinámica en Play de forma separada: el capturador no entra en Play ni fabrica datos de UI.
- Probar Android/Cardboard en teléfono: cinco interacciones, cuatro teletransportes, legibilidad binocular y comodidad. Medir FPS/frame time, batches, triángulos y memoria con el perfil Android antes/después. Las capturas de escritorio no aprueban estos puntos.

Si se requiere capturar un estado de juego con UI dinámica, documenta y ejecuta un procedimiento local de Play aparte; no presentes el render estático como evidencia de ese estado.
