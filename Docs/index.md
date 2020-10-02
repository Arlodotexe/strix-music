# Strix Music v2 documentation

## Overview
Strix Music is a free and open source UWP app that is supercharged by the [Uno Platform](https://platform.uno/), which allows us to target non-Windows platforms such as Android and the Web.

---

## Cores

A "Core" is how Strix Music communicates with a service provider. 

The Sdk provides an abstraction layer between the Shells and the Cores. This means if you want to add support for an arbitrary service provider, all you need to do is use an libary or SDK for that provider to create a Core. The app does the rest!

---

## Shells

A "Shell" is how the user sees and interacts with the app. It deals with the abstraction layer created by the Strix Music Sdk to display the various Cores the user has configured.

Creating a shell can be an arduious task, but thanks to the power of XAML's templating system, you can speed up development by using our [Default Controls]()

It's recommended that you understand how Templated Controls work before continuing.

---