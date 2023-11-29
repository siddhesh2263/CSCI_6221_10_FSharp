# A Game Development Project Based On Unity & F#
### Date - 11/27/2023

### What is Unity?
* Unity is a cross-platform game engine, used to create three-dimensional (3D) and two-dimensional (2D) games, as well as interactive simulations and other experiences.

### How did we import the character in Unity?
* The character is integrated into Unity as a FBX (Filmbox) file. The FBX format helps developers transfer data between different 3D software programs.

![alt text](https://https://github.com/siddhesh2263/CSCI_6221_10_FSharp/tree/master/images/Jammo_1.png?raw=true)

### How are animations integrated with the character?
* Character animations are downloaded as form of FBX files for Unity. The project consists of a Running, Walking, and Idle animation.

### Assigning keyboard controls to the character.
* The Unity Action Map Manager lets the developer assign the keyboard movements that would be used by the player.

### How is transition from one animation state to another handled?
* State machines are used to manage transitions, for instance from Idle state to Walk state, and from Walk state to Run state, and vica versa, based on the player input values.

### How is F# source code integrated in Unity?
* The F# solution file synchronizes all the DLL files between Unity and F# source code.
* The source code is compiled in such a way, that any changes made in the F# code would reflect directly in the Unity development environment.

### Use of Unity DLLs (Dynamic Linked Libraries) in F#.
* The MonoBehaviour class is the base class from which every Unity script derives, by default.
* It  provides the framework which allows you to attach your script to a GameObject in the editor, as well as providing hooks into useful Events such as Start and Update.

### Setting up Game Environment Details.
#### Gravity and Grounded Gravity:
* We defined a variable in the source code, which helps the character not to float in space, but be affected by gravity.
* Grounded gravity is defined in order to make the character stay on the ground.

### Rotation Management for Character Movement.
* Unity internally uses Quaternions to represent all rotations.
* Quaternions are Unity Engine objects which prevents characters suffering from gimbal lock.
* Gimbal lock is the loss of one degree of freedom in a three-dimensional space, that occurs when the axes of two of the three gimbals are driven into a parallel configuration, "locking" the system into rotation in a degenerate two-dimensional space.

### Challenges faced in F# Game Development.
* Game development with the Unity is primarily focused in C#. This makes game development using F# restricted in certain aspects.
* F# is not supported in script format in Unity. Due to this, F# development is done in assembly point of view, wherein the F# is loaded into a DLL, and consumed by Unity.
* F# has to wait for a compilation pass, which is slow compared to hot-reloading of C# scripts.

---

#### To recap, the project first handles animations, then we handle gravity, then we calculate the character's relative movement, then we handle rotation for that movement, and finally apply the final calculated movement to the character control.

---

### References:
* https://docs.unity3d.com/Manual/class-MonoBehaviour.html
* https://docs.unity3d.com/ScriptReference/Quaternion.html
* https://www.mixamo.com/#/
* https://assetstore.unity.com/
* https://learn.microsoft.com/en-us/archive/msdn-magazine/2014/august/unity-developing-your-first-game-with-unity-and-csharp