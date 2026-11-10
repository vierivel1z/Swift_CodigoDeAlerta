> **Histórico archivado el 10/11/2026.** Conserva el estado de su sesión; no describe necesariamente la versión actual. Consulta el [índice vigente](../../../README.md).

# Verificación de la biblioteca de materiales

Fecha local: 3 de noviembre de 2026, America/Lima. Los nombres de carpetas y recibos usan UTC; por eso llevan 20261104. Rama feat/material-library-catalog, commit comprobado c4a1278.

Se abrió la instalación existente de Unity 6000.3.24f1 con el mismo proyecto. Tras la carga/importación, se reabrió MaterialLibrary_Studio y se ejecutó una prueba breve en Play Mode. No se creó otra copia del proyecto ni se utilizó Unity batch.

## Resultado

- PASS: reapertura de la escena guardada y resolución de sus referencias.
- PASS: 12 materiales de muestras y un control neutro; URP/Lit compatible, opaco y smoothness controlado.
- PASS: texturas superficiales de 256 x 256 o menores y cubemap offline128 persistente en modo Custom.
- PASS: 12 placas planas, 12 esferas, 12 cubos y 12 cilindros.
- PASS: dos luces blancas neutras, sin niebla ni skybox que altere el color.
- PASS: etiquetas TMP sin overflow, cámara única, sin colliders ni componentes faltantes.
- PASS: renderizado mediante URP en Editor y Play Mode; 14 capturas nuevas.
- PASS: cero errores y cero advertencias capturados en la reapertura/renderizado y en la ventana de Play comprobada. El log de esta sesión no contiene errores C# ni de shader. Los mensajes iniciales de validación de licencia terminaron con resolución correcta y no bloquearon el Editor.
- PASS: 1073 archivos previos de Assets/Packages/ProjectSettings idénticos por SHA-256; incluye BancoCorresponsal, catálogo, materiales, URP, XR y paquetes.

El perfil activo fue Assets/Settings/Mobile_RPAsset.asset, con Direct3D11 y plataforma Android seleccionada. No se modificó el perfil. Se comprobó también que las referencias sobreviven a la recarga de escena y al paso por Play.

No se guardó ni reconstruyó ninguna escena/material durante la prueba. El nuevo helper solo Editor MaterialLibraryVerification.cs comprueba el catálogo y escribe evidencia, sin cambiar la lógica del juego. Los recibos brutos están en Artifacts/MaterialLibraryVerify/20261104T010447Z/.

## Capturas y límites

[Galería de esta verificación](../../../Previews/MaterialLibraryVerification/20261104T010447Z/README.md). Las siete vistas de Editor y la general de Play se inspeccionaron directamente. Las anteriores siguen conservadas en MaterialLibrary.

Estas pruebas validan la biblioteca aislada. No son una prueba de las decisiones bancarias, prólogo o teletransportes; no se ejecutaron GameplayFlowProbe/BankOfficeProbe/PrototypeSmokeProbe en esta sesión. Android/HONOR, sensores/QR, comodidad binocular, aliasing en movimiento y rendimiento real siguen pendientes.

No se hizo un nuevo commit, push o merge. El commit c4a1278 permanece intacto; el helper y la nueva evidencia quedan como cambios locales para revisión.
