Cyotek.Drawing.Imaging.Farbfeld
===============================

![Sample Screenshot](screenshot.png)

About farbfeld
--------------

[farbfeld](http://tools.suckless.org/farbfeld/) by [suckless.org](http://suckless.org/) is a lossless image format which is easy to parse, pipe and compress. It has the following format:

| Bytes  | Description                                               |
| -----: | --------------------------------------------------------- |
| 8      | "farbfeld" magic value                                    |
| 4      | 32-Bit BE unsigned integer (width)                        |
| 4      | 32-Bit BE unsigned integer (height)                       |
| [2222] | 4x16-Bit BE unsigned integers [RGBA] / pixel, row-aligned |

The RGB-data should be sRGB for best interoperability and not alpha-premultiplied.

About Cyotek.Drawing.Imaging.Farbfeld
-------------------------------------

**Cyotek.Drawing.Imaging.Farbfeld** is a library for processing farbfeld files in .NET.

I wanted a simple image format for storing cross platform data without requiring complicated decoding. I had considered [PPM](https://en.wikipedia.org/wiki/Netpbm_format) but had decided on a binary format. I had actually sketched out my own simple indexed format when by sheer coincidence farbfeld (which I keep writing as farfeld and am forever having to correct!) popped up on [Hacker News](https://news.ycombinator.com/item?id=10890873). Not having anything better to do on a Friday night, I decided I would write an encoder/decoder in order to load and save .NET images in this format.

As it turns out, compression is pretty much a must with this format - a HD image creates a 16MB farbfeld file. Note that this library does not handle any of that, it only handles raw farbfeld data. When I sketched my own format, it was 8bit indexed, so should have been quite small without needing compression, but of course only 256 colours and no alpha.

I've only spent a couple of hours this evening working on the project so take the library with a pinch of salt. It might be (is) missing functionality, could have bugs, API could be better, etc. I'll be refining this over time when I actually use it in the project I intended it for in the next couple of weeks.

Using the library
-----------------

There are two classes, `FarbfeldDecoder` and `FarbfeldEncoder` for saving and loading .NET `Bitmap` objects in the farbfeld format.

### FarbfeldDecoder

* `Decode(string)` - reads a farbfeld image from a file and creates a `Bitmap` from the data
* `Decode(Stream)` - reads a farbfeld image from a Stream and creates a `Bitmap` from the data
* `IsFarbfeldImage(string)` - tests if the given file contains a farbfeld image
* `IsFarbfeldImage(Stream)` - tests if the given Stream contains a farbfeld image

### FarbfeldEncoder

* `Encode(string)` - saves a `Bitmap` into the specified file using the farbfeld format
* `Encode(Stream)` - saves a `Bitmap` into the specified Stream using the farbfeld format

> Note: Only 32bpp RGBA images are supported by the `FarbfeldEncoder` class at this time

Sample Application
------------------

This repository also includes a simple image viewer (using the [ImageBox](https://github.com/cyotek/Cyotek.Windows.Forms.ImageBox) control of course!) that lets you open farbfeld images, or convert other basic image formats to farbfeld.

Sample Images
-------------

The sample images which I used for reference to ensure the encoding/decoding actually worked came from the [here](https://github.com/mehlon/farbfeld)

Tests
-----

Naughty me, I didn't add any tests yet.

Nuget?
------

I haven't really spent a huge amount of time on this yet so what you see is what you get. Once I've expanded the encoder to get pixel data for formats other than 32bit and made sure the API is suitable, I'll create a package.

Acknowledgements
----------------

The icon used in the viewer program came from <https://www.iconfinder.com/icons/254234/image_icon#size=128>

The `polgyon.png`, `polygon.png.ff`, `yellow-1x1-semitransparent.png` and `yellow-1x1-semitransparent.png.ff` sample images came from <https://github.com/mehlon/farbfeld>