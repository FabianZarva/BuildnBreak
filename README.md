# Build & Break (Build N Break)

Build & Break is an educational physics-based construction and structural simulation game developed in Unity (C#). The project was created to help civil engineering students understand structural calculations, realistic load bearing, and material tolerances under severe environmental stressors.

Players design and assemble structural frameworks using diverse construction materials, subject them to dynamic natural disasters such as high-velocity wind loads and rising floodwaters, and evaluate structural integrity through real-time collapse mechanics.

* **Live Project on itch.io**: [fabianzarva.itch.io/build-n-break](https://fabianzarva.itch.io/build-n-break)

---

## Core Features

* **Data-Driven Material Configuration**:
  * Utilizes Unity `ScriptableObject` architecture to define physical and mechanical material properties (such as density, tensile/compressive strength, elasticity, and cost).
  * Enables straightforward tweaking and balancing of real-world materials (e.g., concrete, steel, timber) without modifying core physics scripts.

* **Dynamic Disaster Simulation**:
  * **Wind Load Events**: Simulates directional atmospheric forces and lateral pressure against building facades, testing lateral stiffness and shear resistance.
  * **Flood Inundation Events**: Simulates rising water levels, hydrostatic pressure, and buoyancy effects that undermine foundational stability and weaken submerged components.

* **Procedural Structural Destruction (Open Fracture)**:
  * Integrates the open-source `Open Fracture` package to compute mesh slicing, procedural fragmentation, and break points dynamically upon structural failure.
  * Discards predefined, canned destruction animations in favor of authentic real-time physics collapse dictated by force thresholds and joint strain.

* **Constraint-Based Construction & Rules Engine**:
  * Enforces construction limitations and engineering rules, including geometric connection logic, support boundaries, and resource limits.
  * Evaluates structures through win/lose logic based on survival thresholds, deflection limits, and safety tolerances under disaster conditions.

---

## Technical Architecture

* **Game Engine**: Unity (3D, Physics Engine).
* **Programming Language**: C#.
* **Destruction Pipeline**: Open Fracture (procedural mesh fracturing and convex hull collider generation).
* **Configuration Architecture**: Modular `ScriptableObjects` for material definitions and scenario stress profiles.
* **Target Platform**: PC / Standalone Windows.
