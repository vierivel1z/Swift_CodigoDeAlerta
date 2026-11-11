# Legibilidad de la pantalla de espera — Centro de operaciones

Fecha: 11 de noviembre de 2026. Plataforma objetivo: Android con Google Cardboard. La revisión visual y las pruebas de este documento se hicieron en Unity Editor 6000.3.24f1; falta comprobarlas en el teléfono.

## Diagnóstico

La vista anterior mostraba una tarjeta clara de gran tamaño con tres líneas de instrucciones en gris y cursiva. Desde el asiento del jugador, el texto secundario se percibía pequeño, las instrucciones de PC y VR competían entre sí y el área negra inferior quedaba vacía. También aparecían `EVIDENCIA`, `CONTROLES` y `CERRAR TURNO` sin un turno abierto. Esto dificultaba identificar la siguiente acción: escoger un modo en el portal.

Capturas reales desde el mismo punto del Editor: [antes](../Previews/WelcomeTypography/Before/welcome.png) y [después](../Previews/WelcomeTypography/After/welcome.png). Los archivos `capture-receipt.txt` de cada carpeta documentan la captura.

## Investigación aplicada

- La [guía de tipografía de Meta para VR](https://developers.meta.com/vr/design/styles_typography/) recomienda una jerarquía breve, fuentes sans legibles y evitar cursivas extensas en el entorno inmersivo. Se conservó TextMeshPro con la fuente SDF existente y se eliminó la cursiva del texto de instrucciones.
- Los [ejemplos de introducción de Meta](https://developers.meta.com/vr/design/hands-onboarding-best-practices/) muestran instrucciones cortas junto a la acción correspondiente. La pantalla de espera ahora presenta una sola decisión principal, una ayuda de control dependiente de la plataforma y acceso separado al tutorial.
- El equipo de [Demeo describe su proceso de UI](https://www.resolutiongames.com/blog/demeo-one-year-anniversary-a-look-at-the-evolving-uxui-of-demeo) empezando por la información que necesita el jugador en cada momento; una opción sin título se confundía con decoración hasta que la separaron en botones claros. Por eso aquí se separó la decisión principal de la ayuda y se ocultaron acciones aún no disponibles.
- Los [requisitos de diseño para apps de realidad virtual de Meta](https://developers.meta.com/vr/documentation/android-apps/design-requirements/) aconsejan evitar superficies de blanco puro y comprobar el contraste. Son una referencia de diseño, no una certificación de Cardboard. La tarjeta se cambió por azul petróleo oscuro con letras claras y acentos cian moderados.

## Cambio aplicado

En `Assets/_Project/Scripts/UI/BankDocumentUI.cs` la pantalla de espera ahora tiene estos niveles: `ESTACIÓN DISPONIBLE`, `ELIGE ATAQUE O DEFENSA`, ubicación del portal, ayuda de selección, botón `PRACTICAR MIRADA / TUTORIAL VR` y barra de estado. Se usa una sola superficie oscura y espaciado amplio. En PC se muestra el control de ratón; Android muestra la instrucción de mirada. Las acciones bancarias se ocultan hasta que exista una sesión y la antigua línea de estado redundante se oculta mientras no haya expediente. `SALIR DE ESTACIÓN` permanece disponible. No se alteraron las reglas bancarias ni el tiempo de selección por mirada.

`Assets/_Project/Scripts/Testing/ImmersivePlayProbe.cs` y `Assets/_Project/Editor/ImmersiveEditorBridge.cs` añaden un modo de captura y validación `welcome-before`/`welcome-after`. La sonda comprueba que la sesión aún no se ha abierto, que se alcanza el tutorial por raycast desde el asiento, que las etiquetas nuevas no desbordan y que las acciones del turno están ocultas mientras no corresponde.

## Validación y pendientes

Pasaron en Unity Editor las sondas `welcome-before`, `welcome-after`, `flow`, `office`, `smoke`, `tutorial` y `closeui`. Los resultados detallados están en `Artifacts/ImmersiveGameplay/*-result.txt`; la [captura adicional del tutorial](../Previews/WelcomeTypography/Validation/tutorial-complete.png) quedó separada para conservar la galería histórica. La comparación antes/después usa una cámara temporal en la misma posición y orientación. La escena y la configuración URP/Cardboard no se modificaron para esta mejora.

Queda una validación en el HONOR con Cardboard: leer título, ayuda y estado desde la posición habitual sin inclinarse; completar el tutorial; elegir un modo; comprobar que las acciones bancarias reaparecen dentro del turno y que el texto se mantiene cómodo con ambos ojos. Si el teléfono reduce perceptiblemente la nitidez, aumentar primero el tamaño físico del monitor o la proximidad de inspección y revisar la distancia angular antes de añadir detalles finos.
