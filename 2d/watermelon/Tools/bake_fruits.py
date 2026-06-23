#!/usr/bin/env python3
"""Bake fruit PNGs: cover-fill circle, no white padding, thin rim."""

from PIL import Image, ImageDraw
import os

NAMES = [
    "cherry", "strawberry", "grape", "orange", "apple",
    "pear", "peach", "pineapple", "melon", "watermelon",
]

ALT_SRC = "/Users/cody/.cursor/projects/Users-cody-Desktop-tencent-gongfeng-unity-learn/assets"
DST_DIR = os.path.join(os.path.dirname(__file__), "..", "Assets", "Resources", "Fruits")


def strip_near_white(img, threshold=215):
    px = img.load()
    for y in range(img.height):
        for x in range(img.width):
            r, g, b, a = px[x, y]
            if a > 20 and r >= threshold and g >= threshold and b >= threshold:
                px[x, y] = (r, g, b, 0)
            if a > 20 and r > 200 and g > 200 and b > 170 and abs(r - g) < 30:
                px[x, y] = (r, g, b, 0)


def bake(path_in, path_out, size=512):
    src = Image.open(path_in).convert("RGBA")
    strip_near_white(src)
    bbox = src.getbbox()
    if not bbox:
        return

    src = src.crop(bbox)
    side = max(src.width, src.height)
    square = Image.new("RGBA", (side, side), (0, 0, 0, 0))
    square.paste(src, ((side - src.width) // 2, (side - src.height) // 2), src)
    src = square

    canvas = Image.new("RGBA", (size, size), (0, 0, 0, 0))
    cx = cy = size // 2
    outer_r = size // 2 - 2
    rim_w = 6
    inner_r = outer_r - rim_w

    cover = int(inner_r * 2 * 1.18)
    fruit = src.resize((cover, cover), Image.Resampling.LANCZOS)

    circle_mask = Image.new("L", (size, size), 0)
    ImageDraw.Draw(circle_mask).ellipse(
        (cx - inner_r, cy - inner_r, cx + inner_r, cy + inner_r), fill=255
    )

    layer = Image.new("RGBA", (size, size), (0, 0, 0, 0))
    layer.paste(fruit, (cx - cover // 2, cy - cover // 2), fruit)
    r, g, b, a = layer.split()
    a = Image.composite(a, Image.new("L", (size, size), 0), circle_mask)
    layer = Image.merge("RGBA", (r, g, b, a))

    px = layer.load()
    samples = []
    for y in range(size):
        for x in range(size):
            if circle_mask.getpixel((x, y)) == 0:
                continue
            pr, pg, pb, pa = px[x, y]
            if pa > 128:
                samples.append((pr, pg, pb))

    if samples:
        avg = tuple(sum(c[i] for c in samples) // len(samples) for i in range(3))
        for y in range(size):
            for x in range(size):
                if circle_mask.getpixel((x, y)) == 0:
                    continue
                if px[x, y][3] < 40:
                    px[x, y] = (*avg, 255)

    canvas = Image.alpha_composite(canvas, layer)
    ImageDraw.Draw(canvas).ellipse(
        (cx - outer_r, cy - outer_r, cx + outer_r, cy + outer_r),
        outline=(68, 40, 22, 255),
        width=rim_w,
    )
    canvas.save(path_out)


if __name__ == "__main__":
    dst = os.path.abspath(DST_DIR)
    src_root = ALT_SRC
    for name in NAMES:
        src = os.path.join(src_root, f"{name}_round.png")
        if not os.path.exists(src):
            src = os.path.join(dst, f"{name}.png")
        bake(src, os.path.join(dst, f"{name}.png"))
        print("baked", name)
