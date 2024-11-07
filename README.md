# 项目简介

本项目扩展了Unity游戏引擎中的UI组件:`Image` 和 `RawImage`，使其可以生成四角圆润的网格，每个角的圆润程度可以单独设置。

使用扩展后的 `RoundedImage` 和 `RoundedRawImage` 组件，可以有如下优势：

* 减少项目中 `Mask` 组件的使用，从而优化项目性能。

* 减少纹理中的Alpha通道，可优化纹理大小和运行时内存大小。

* 支持UI的默认材质，可以便于UI系统的合批操作，从而优化DrawCalls的数量。


# Introduction

This project extends the UI components Image and RawImage in the Unity game engine, enabling them to generate meshes with rounded corners, with the degree of roundness independently adjustable for each corner.

By using the extended RoundedImage and RoundedRawImage components, you can achieve the following advantages:

* Reduce the use of Mask components in the project, thereby optimizing project performance.

* Minimize the alpha channel in textures, which can optimize texture size and runtime memory usage.

* Support the default UI material, facilitating batching operations in the UI system, thereby optimizing the number of DrawCalls.

![image](https://github.com/user-attachments/assets/8ef0cc3e-7ea9-4659-8006-bca8cd942896)
