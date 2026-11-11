# Piloto jugable SIM-003: investigación y evidencia física

Estado: variante optativa de revisión en Unity Editor. La escena principal y las reglas bancarias oficiales no se sustituyeron. No se probó en un teléfono Android con Google Cardboard.

## Alcance y activación

- Escena de revisión: `Assets/_Project/Scenes/Review/BancoCorresponsal_SIM003Pilot_20261111.unity`.
- Perfil de revisión: `Assets/_Project/DeskTools/DeskTools_SIM003Pilot.asset`.
- `Sim003PilotEnabled`: activa el flujo de investigación SIM-003. Desactivarlo devuelve el flujo anterior.
- `PolishedClipboardVisual`: activa el acabado físico y la animación de hojas. Desactivarlo conserva el aspecto previo y la ventana ampliada sigue disponible.
- `Sim003PrinterEnabled`: activa la constancia física de consulta. Desactivarlo conserva la consulta digital.
- `Sim003PrinterFailsInAttack`: con valor verdadero, ATAQUE muestra un fallo controlado sin imprimir; DEFENSA imprime. Con valor falso, la impresora funciona en ambos modos. El perfil entregado deja esta opción verdadera.

Estos interruptores pertenecen a la copia del perfil; el perfil de la escena principal no los tiene activos. Se hizo copia recuperable de los archivos relevantes en `Artifacts/SIM003Pilot/Backups` antes de editarlos. No se modificaron `BankingDomain`, los seis expedientes originales, las reglas oficiales de decisión ni su puntuación.

## Experiencia del piloto

SIM-003 empieza con la información del expediente sin revelar el titular del registro ni una decisión recomendada. En REGISTRO el jugador elige la cuenta y ejecuta BUSCAR; entonces aparece el titular registrado. Puede anotar un hallazgo, editarlo antes de confirmar la decisión y consultar el portapapeles físico. La decisión muestra confirmación y permite cancelar. Tras decidir, el monitor presenta una revisión con decisión original, estado efectivo, fuentes usadas, control y pérdida ficticia. Una valoración experimental de investigación (10 + 5 puntos) se muestra aparte de la puntuación oficial.

La hoja del portapapeles contiene procedimientos generales y la anotación que haya hecho el jugador; no da la respuesta a SIM-003. El teléfono sigue disponible como comprobación adicional en DEFENSA. Los demás expedientes mantienen el flujo existente.

## Portapapeles físico

El objeto se levanta y vuelve mediante el sistema de mirada y el tiempo de fijación existentes. Conserva el clip, el papel, las páginas y las marcas de revisión. El aspecto nuevo usa una tablilla grafito sobria, pinza metálica visible, varias capas de papel marfil, acento lateral cian discreto y texto oscuro con jerarquía de título, contenido, nota y número de hoja. Los controles ANTERIOR, SIGUIENTE, MARCAR y DEJAR se ordenaron en dos filas con separación y contraste; AMPLIAR permanece como alternativa de accesibilidad.

Al cambiar de página, una lámina clara recorre brevemente la hoja durante aproximadamente 0,30 s. Es una transición visual; el contenido y las marcas siguen procediendo de la paginación existente. No se añadió un segundo temporizador de mirada ni un raycast nuevo. El pulido se crea en tiempo de ejecución en `PhysicalClipboardPresentation.cs` y `DeskInspectionView.cs`; el prefab previo se conserva sin sobrescribir.

## Impresora y constancia

La primera hoja implementada es una **constancia de consulta de registro** para SIM-003. Sólo puede solicitarse después de consultar REGISTRO. Toma los datos dinámicos del expediente y del resultado obtenido; no contiene la decisión recomendada. La impresora muestra actividad durante la salida de la hoja, deja una sola copia visible en la bandeja y permite acercarla por mirada, devolverla y abrir una lectura ampliada. Mantener la mirada no crea copias adicionales. La consulta digital sigue accesible incluso si falla la impresión.

En la variante entregada, ATAQUE presenta un fallo de impresión explícito y DEFENSA permite imprimir. El fallo representa una hipótesis didáctica reversible, no una reconstrucción histórica ni indicio de fraude. Se probó también la variante con impresión operativa en ambos modos. La arquitectura actual sólo implementa esta constancia; resumen de orden, registro de comprobación y comprobante de decisión siguen pendientes.

## Verificación realizada

| Comprobación | Resultado |
| --- | --- |
| Preparación de escena y perfil aislados en Unity 6000.3.24f1 | Pasó; la escena principal mantuvo su hash durante la copia. |
| Play Mode, ATAQUE con fallo controlado de impresora | Pasó; la consulta digital no se perdió y el flujo de seis expedientes continuó. |
| Play Mode, DEFENSA con constancia física | Pasó; impresión, una copia, acercamiento, retorno, ampliación, teléfono, control y teletransporte. `errors=0`. |
| Play Mode, ATAQUE con impresión habilitada | Pasó; el resultado oficial se mantuvo. |
| Portapapeles: levantar, tres hojas, transición, marcar, devolver | Pasó en Editor mediante selección por mirada. |
| Confirmación/cancelación, revisión posterior y seis expedientes | Pasó en ambas variantes; ATAQUE: 5/6 y pérdida ficticia de USD 20 000; DEFENSA: 5/6 y pérdida ficticia 0 en las rutas comprobadas. |
| Valoración experimental aislada | 15/15 en la ruta ensayada de SIM-003; no altera la puntuación oficial. |
| `Tools/DomainTests` y `Tools/DeskToolsTests` | 37/37 y 37/37, respectivamente. |
| Compilación / Console durante las sondas finales | Sin errores nuevos (`errors=0`). |

Las pruebas anteriores son de **Unity Editor con destino Android configurado**, no pruebas ejecutadas en Android. La comodidad de lectura, el tamaño angular, el rendimiento, el tacto de las transiciones y la selección en un HONOR con visor Cardboard siguen sin comprobarse físicamente.

## Capturas reales

Galería y secuencia: [`Docs/Previews/SIM003Pilot/README.md`](../Previews/SIM003Pilot/README.md). Incluye antes/después desde la misma pose, hojas 1–3, transición, consulta de registro, fallo y éxito de impresión, inspección de hoja, confirmación y resultados. Las capturas son del render y Game View de Unity Editor.

## Archivos principales

- Piloto y estado: `Assets/_Project/Scripts/DeskTools/Sim003Pilot.cs`.
- Papel e impresora: `Assets/_Project/Scripts/DeskTools/Sim003PrinterPresentation.cs`.
- Portapapeles físico: `Assets/_Project/Scripts/DeskTools/PhysicalClipboardPresentation.cs`.
- UI, controles y coordinación: `BankDocumentUI.cs`, `DeskInspectionView.cs`, `DeskInteractionCoordinator.cs`, `FormatReferencePanel.cs`, `UIManager.cs`, `BankInteractable.cs`, `DeskToolsProfile.cs`.
- Escena y perfil aislados, con sus `.meta`, en las rutas anteriores.
- Preparación y sonda Editor: `Sim003PilotEditorSetup.cs`, `ImmersiveEditorBridge.cs`, `ImmersivePlayProbe.cs`.

## Pendiente antes de integrar en la escena principal

1. Probar sentado con el teléfono y Google Cardboard: distancia del portapapeles, lectura de las tres hojas, acceso a controles y ausencia de fatiga visual.
2. Medir en Android el rendimiento y verificar que papel, impresora y controles no ocultan otros objetivos ni bloquean raycasts.
3. Decidir si la diferencia de impresión ATAQUE/DEFENSA aporta aprendizaje. El flujo no obliga a trasladarse a la zona de documentación para cada expediente; la impresión es opcional.
4. Revisar el texto y la valoración experimental con la propuesta aprobada de jugabilidad antes de extenderlos a los otros cinco casos.
5. Si la prueba física resulta satisfactoria, integrar la variante optativa; conservar la lectura ampliada como accesibilidad y el perfil previo como reversión.
