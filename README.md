# ImageText
Adds text to a given image

A simple command-line utility for creating test images with given text based on a given image. 

## Command Line options:
```
 -i, --InputFile       The input image file (if no input file is used a random 512x512 bitmap is generated)
 -o, --OutputFile      Required. The output image file
 -t, --Text            Required. The text to write on the image
 -c, --ColorName       (Default: HotPink) The named color for writing the text
 -f, --FontName        (Default: Arial) The font name to use when writing text
 -s, --FontSize        (Default: 30) The font size to use when writing text
 -a, --Angle           (Default: -45) The rotation angle to use when writing text
 -q, --UseQuantizer    (Default: false) Use Quantizer (to get smaller image file sizes)
 --help                Display this help screen.
 --version             Display version information.
 ```
