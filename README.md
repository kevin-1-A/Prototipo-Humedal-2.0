# Prototipo humedal
--
## Integrantes  y Roles del grupo
**Compilador:** Juan Camilo Bernal, Jhon Alejandro Arriaga, Kevin Alejandro Espitia Aranzales - Consolidación del proyecto y empaquetado.
* **Revisor:** Kevin Alejandro Espitia - Revisión de código, arquitectura y comentarios.
* **Evaluador:** Juan Camilo Bernal - Pruebas de funcionamiento y cumplimiento de la guía.
* **Entregas:** Jhon Alejandro Arriaga - Preparación del documento final e informes.
* **Alertas:** Jhon Alejandro Arriaga - Coordinación y tiempos de entrega.
*  **Sonidos:** Jhon Alejandro Arriaga - Sonidos de fondo, animales, herramientas, etc.
  
##  Estructura del Proyecto de Unity (`Assets/Scripts`)
* **`Player/`**: Movimiento del personaje (`PlayerController`) y selección de herramientas (`ToolManager`).
* **`Environment/`**: Lógica de interacción con basura, plantas y animales (`InteractableItem`).
* **`Managers/`**: Lógica central del juego (`GameManager`), interfaz (`UIManager`), atajo de reinicio con tecla **R** (`LevelManager`) y audio completo (`SoundManager`).
  
##  Controles
* **W, A, S, D / Flechas:** Mover al personaje.
* **Teclas 1, 2, 3:** Seleccionar herramienta.
* **Tecla E:** Limpiar / Sembrar / Curar.
* **Tecla R:** Reiniciar el nivel.
