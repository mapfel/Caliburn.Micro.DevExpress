# Caliburn.Micro.DevExpress

A library containing Caliburn.Micro conventions for DevExpress' visual controls.  Target platform: WPF.

This is a copy of [calmicrodevexpress](https://archive.codeplex.com/?p=calmicrodevexpress) from CodePlex (source code and description here)

## Project Description

A library containing Caliburn.Micro conventions for DevExpress' visual controls.  
Target platform: WPF.  
Inspired by Caliburn.Micro.Telerik package.

## How to enable conventions
In your app's bootstrapper, in Configure override or in a static constructor add next line:

```cs
Caliburn.Micro.DevExpress.DXConventions.Install();
```

Of course, you need a DevExpress WPF subscription to use this package. It's compiled with v14.2.3.

## Nuget package

Easiest way to use it is the Caliburn.Micro.DevExpress package, but it works only if you use exactly the same DevExpress version as the package was compiled with. If you use other version, just download and compile the source code!

## Currently supported DevExpress controls

I use this package in a project and develop them in parallel. I add conventions for DevExpress controls as I need them in my project. If you need conventions for other controls that I currently don't support, just create a new thread on the Discussions page and I will create them when I have some time. If you have other suggestions, they are also welcome on the Discussions page.
So, the currently supported DevExpress controls that have conventions:

- DataControlBase (GridControl, TreeListControl)
- PivotGridControl
- DataLayoutControl
- LookUpEditBase (LookUpEdit, GridLookUpEdit, SearchLookUpEdit)
- DocumentGroup (for tabbed documents)
- ChartControl
- BaseEdit (most editors)
- SpinEdit
- RangeBaseEdit (ProgressBarEdit and TrackBarEdit)

## Attaching messages to BarItem objects

If you want to use Caliburn.Micro's Message.Attach mechanism together with DXBars or DXRibbon, you can use my related project Caliburn.Micro.FrameworkContentElement. I personally use these two packages together, too.
