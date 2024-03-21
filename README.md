# AvaloniaKeyboard

![](/image.png)

## Use it!

1. add submodule
```
git submodule add https://github.com/Coloryr/AvaloniaKeyboard.git
git submodule update --init --recursive
```

and add project `AvaloniaKeyboard`

2. add style
```
<StyleInclude Source="avares://AvaloniaKeyboard/Views/KeyboardControl.axaml" />
```

3. use it 
```
xmlns:view="clr-namespace:AvaloniaKeyboard.Views;assembly=AvaloniaKeyboard"
```

```
<StackPanel>
    <TextBox Name="Text1" />
    <view:KeyboardControl Name="Input1" TextBox="{ResolveByName Text1}" />
</StackPanel>
```
**Keyboard must set a TextBox**
TextBox can set from code or xaml

## With [Rime](https://github.com/Coloryr/librime)

1. Download rime
In [Action](https://github.com/Coloryr/librime/action)
download zip and unzip it.
copy `rime.dll` to your `Run Path`

2. Download data
make a dir `data`

download all data from [librime](https://github.com/Coloryr/librime/tree/master/data/minimal)

```
- net8.0 \
  - data \
     - cangjie5.dict.yaml
     - cangjie5.schema.yaml
     - default.yaml
     - ...
  - rime.dll
  - {your program.exe}
  - ...
```

3. Init Rime
```
using AvaloniaKeyboard;

RimeUtils.Init();
```