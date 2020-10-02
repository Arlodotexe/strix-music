# Strix Music v2 documentation

Strix Music is a free and open source UWP app, supercharged by the [Uno Platform](https://platform.uno/) to run cross-platform.

---

### Cores
**A "Core" is how the app communicates with a service provider.**

Strix Music provides a common abstraction layer across all cores, which is consumed by a Shell. In theory, Strix supports any arbitrary music provider. All you need is a libary or SDK, and the know-how. Create a core, and the app does the rest!

---

### Shells
**A "Shell" is how the user sees and interacts with the app.**. It deals with the abstraction layer created by the Strix Music Sdk to display the various Cores the user has configured.

Creating a shell can be an arduious task, but thanks to the power of XAML's templating system, you can speed up development by using our [Default Controls]()

It's recommended that you understand how Templated Controls work before continuing.

---