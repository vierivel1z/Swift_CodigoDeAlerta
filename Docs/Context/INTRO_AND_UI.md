# Base del prólogo y cambios funcionales — 2026-10-29

Abrir BancoCorresponsal y Play. Elegir ATAQUE o DEFENSA: la primera selección abre un storyboard de unos 12 s. Mirar OMITIR INTRODUCCION durante el tiempo de selección o hacer clic en PC para pasar al modo seleccionado. La segunda entrada de modo no repite el prólogo durante esa ejecución.

La base muestra impresora mediante formas UI, hoja animada y tres subtítulos. No incluye personaje 3D, voz, animación esquelética ni arte definitivo. Banco de origen y puesto del analista quedan separados narrativamente. Se mantiene libre el seguimiento de cabeza, sin recorrido forzado. ShotSeconds en IntroSequence permite ajustar duración. Sustituir formas por sprites de Docs/ArtPrompts manteniendo texto dinámico y botón de omisión. La clase se agrega en runtime desde GameManager.EnterScenario; no requiere guardar ni reconstruir la oficina.

Corregido: botones de decisiones visibles en menú/resultados, espacio insuficiente del resumen y créditos breves desactualizados. El resumen amplía su región y reduce el listado a su zona superior. Requiere revisión visual con visor; no se afirma que todo el texto final ya resulte legible en teléfono.

Prueba funcional en copia aislada: Unity -batchmode -nographics -projectPath RUTA_COPIA -executeMethod Swift.EditorTools.GameplayFlowValidation.Run -logFile RUTA_LOG (sin -quit). El método abre la escena guardada y agrega una sonda en memoria, no guarda cambios. Resultado relativo al proyecto de prueba: Artifacts/GameplayFlow/result.txt. No ejecutar una segunda instancia sobre el checkout abierto.

Para lectura de requisitos, ver TWO_MODES.md. Los textos específicos Funcional/Técnico/Roles/Defensa/Vulnerabilidad del docente siguen sin estar en la documentación recibida.

Validación de esta sesión: compilación C# con referencias Unity 0 errores/0 advertencias; 27 pruebas de dominio PASS. Los dos intentos de importar la copia en Unity quedaron detenidos en Initial Refresh y se finalizaron. No se confirmó Play Mode ni revisión visual; las pruebas de integración quedan preparadas para ejecutarse cuando el Editor complete la importación.
