#!/usr/bin/env python3
"""Bake fruit PNGs: fruit fills the circle, thin rim only, no white padding."""

from PIL import Image, ImageDraw
import os

NAMES = [
    "cherry", "strawberry", "grape", "orange", "apple",
    "pear", "peach", "pineapple", "melon", "watermelon",
]

SRC_DIR = os.path.join(
    os.path.dirname(__file__), "..", "..", "..", "..",
    ".cursor", "projects", "Users-cody-Desktop-tencent-gongfeng-unity-learn", "assets"
)
# Fallback: use round assets relative to script when run from repo
ALT_SRC = "/Users/cody/.cursor/projects/Users-cody-Desktop-tencent-gongfeng-unity-learn/assets"
DST_DIR = os.path.join(os.path.dirname(__file__), "..", "Assets", "Resources", "Fruits")


def strip_white(img, threshold=220):
    pixels = img.load()
    for y in range(img.height):
        for x in range(img.width):
            r, g, b, a = pixels[x, y]
            if r >= threshold and g >= threshold and b >= threshold:
                pixels[x, y] = (r, g, b, 0)


def crop_to_content(img):
    bbox = img.getbbox()
    if not bbox:
        return img
    return img.crop(bbox)


def bake(path_in, path_out, size=512):
    src = Image.open(path_in).convert("RGBA")
    strip_white(src)
    src = crop_to_content(src)

    side = max(src.width, src.height)
    square = Image.new("RGBA", (side, side), (0, 0, 0, 0))
    square.paste(src, ((side - src.width) // 2, (side - src.height) // 2), src)
    src = square

    canvas = Image.new("RGBA", (size, size), (0, 0, 0, 0))
    cx = cy = size // 2
    outer_r = size // 2 - 2
    rim_w = 7
    fruit_r = outer_r - rim_w // 2

    fruit_d = int(fruit_r * 2 * 1.06)
    fruit = src.resize((fruit_d, fruit_d), Image.Resampling.LANCZOS)

    mask = Image.new("L", (size, size), 0)
    ImageDraw.Draw(mask).ellipse(
        (cx - fruit_r, cy - fruit_r, cx + fruit_r, cy + fruit_r), fill=255
    )

    layer = Image.new("RGBA", (size, size), (0, 0, 0, 0))
    layer.paste(fruit, (cx - fruit_d // 2, cy - fruit_d // 2), fruit)
    r, g, b, a = layer.split()
    a = Image.composite(a, Image.new("L", (size, size), 0), mask)
    layer = Image.merge("RGBA", (r, g, b, a))
    canvas = Image.alpha_composite(canvas, layer)

    ImageDraw.Draw(canvas).ellipse(
        (cx - outer_r, cy - outer_r, cx + outer_r, cy + outer_r),
        outline=(68, 40, 22, 255),
        width=rim_w,
    )
    canvas.save(path_out)


if __name__ == "__main__":
    dst = os.path.abspath(DST_DIR)
    src_root = ALT_SRC if os.path.isdir(ALT_SRC) else SRC_DIR
    for name in NAMES:
        src = os.path.join(src_root, f"{name}_round.png")
        if not os.path.exists(src):
            src = os.path.join(dst, f"{name}.png")
        bake(src, os.path.join(dst, f"{name}.png"))
        print("baked", name)
