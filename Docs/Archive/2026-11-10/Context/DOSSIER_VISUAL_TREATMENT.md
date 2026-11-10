> **Histórico archivado el 10/11/2026.** Conserva el estado de su sesión; no describe necesariamente la versión actual. Consulta el [índice vigente](../../../README.md).

# Expediente — acabado institucional, 2026-11-04

Aplicado en la UI real generada por `BankDocumentUI.Dossier()`: papel gris frío azulado, cabecera petróleo con gradiente leve, campos agrupados en superficies interiores, títulos oscuros consistentes, monto sobre fondo petróleo con valor crema/ámbar y observaciones con el mismo papel y un acento lateral discreto. Marco recortado, bordes suaves y sombra de apoyo dan profundidad sin texturas ni efectos de posprocesado.

Se eligió gris papel porque conecta mejor el documento con las pestañas y el monitor secundario, conservando contraste. No se crearon variantes redundantes ni sprites nuevos: se reutiliza PortalPanelGraphic y el marco existente. Las formas nuevas no reciben raycast. La tipografía conserva tamaños, límites mínimos y espacios para datos largos; etiquetas 24, valores 31–33 con mínimo 28, monto 35 con mínimo 31, notas 28 con mínimo 26 y fecha 25. Los textos siguen siendo TMP y proceden de los mismos datos.

## Archivos y protección

- Modificado: `Assets/_Project/Scripts/UI/BankDocumentUI.cs`, únicamente el método de presentación del expediente.
- Añadido: `Assets/_Project/Editor/DossierTreatmentReview.cs` y su `.meta`, puente local de pruebas/capturas derivado del verificador anterior. No guarda ni migra escenas.
- Evidencias: `Docs/Previews/DossierTreatment/20261104/`, registros en `Artifacts/DossierTreatment/`.
- Historial agregado a Kanban y CHANGELOG.
- Respaldo externo: `D:\UnityGames\SWIFT_Backups\DossierTreatment-20261104-114515`. Contiene script previo y escena con sus `.meta`; restaurar el script fuera de Play recupera el tratamiento anterior. No sobrescribir la escena si hubiera trabajo posterior del usuario.

BancoCorresponsal, UIManager, VRInteractionManager, BankInteractable, GraphicsSettings y QualitySettings conservan exactamente sus SHA-256 previos a esta intervención. No se cambió URP, Cardboard, la lógica bancaria, los prefabs ni la escena. No se hizo commit/push ni se generó APK.

## Verificación real en Unity 6000.3.24f1

- PASS captura antes y después: cuatro pares con la misma pose, dirección, FOV 60°, 1600×900. Se inspeccionaron los PNG del expediente anterior y nuevo.
- PASS `review`, final 16:51:08 UTC: entrada por mirada, progreso real, cancelación, una activación, silla, pestañas, observaciones ampliadas y regreso, valores largos contenidos, TMP sin desbordes en las vistas probadas, Aprobar/Retener/Devolver, seis decisiones de ataque, defensa sin pérdida simulada, salida/reentrada conservando sesión y teletransportes restaurados.
- PASS `stationpc`, final 16:51:22 UTC: hover sin carga, mantener, soltar/reiniciar y entrar/salir mediante los controles existentes.
- Compilación actualizada antes de la captura final; sin nuevos errores runtime registrados en esas ejecuciones.

El primer `after` se ejecutó antes de recompilar y no representaba el cambio. Fue sustituido por capturas nuevas a las 16:50:09 UTC. El recibo conserva ambas ejecuciones para trazabilidad. Un primer `review` agotó la espera de activación de Aprobar; se conserva como `review-initial-failed.txt`. La repetición completa con el código compilado pasó, sin cambiar el dwell ni las reglas. Los checks acumulados conservan el fallo inicial; el resultado final está en `review-result.txt`.

No se repitieron DomainTests ni los tres probes generales: esta intervención se limitó al dibujo del expediente y se verificó mediante el ciclo real completo de UI. Los resultados de sesiones anteriores no se presentan como pruebas nuevas.

## Pendiente en HONOR

Comprobar lectura binocular y comodidad del papel gris, contraste del monto, observaciones y datos largos desde la silla; revisar brillo, aliasing de bordes y coste UI con Profiler en Android. Los renders del Editor no certifican comodidad, FPS, temperatura ni rendimiento Android. No se requieren recursos artísticos adicionales para este acabado.
