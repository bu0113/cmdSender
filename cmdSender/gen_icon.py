# -*- coding: utf-8 -*-
"""生成 cmdSender 应用程序图标：深色圆角背景 + 绿色命令提示符 + 橙色发送箭头"""
from PIL import Image, ImageDraw, ImageFont

SIZE = 256
img = Image.new("RGBA", (SIZE, SIZE), (0, 0, 0, 0))
draw = ImageDraw.Draw(img)

# 圆角深色背景
bg = (38, 41, 60, 255)
draw.rounded_rectangle([6, 6, SIZE - 6, SIZE - 6], radius=52, fill=bg)

# 内边框高光
draw.rounded_rectangle([6, 6, SIZE - 6, SIZE - 6], radius=52,
                       outline=(96, 100, 140, 255), width=3)

# 绿色命令提示符 ">"
try:
    font = ImageFont.truetype("C:/Windows/Fonts/consola.ttf", 120)
except Exception:
    font = ImageFont.load_default()

prompt = ">"
bbox = draw.textbbox((0, 0), prompt, font=font)
pw = bbox[2] - bbox[0]
ph = bbox[3] - bbox[1]
px = 44 - bbox[0]
py = 64 - bbox[1]
# 提示符阴影（轻微）
draw.text((px + 3, py + 3), prompt, font=font, fill=(0, 80, 64, 180))
draw.text((px, py), prompt, font=font, fill=(78, 205, 176, 255))

# 橙色发送箭头（右下，三角形 + 杆）
arrow = (242, 138, 49, 255)
# 箭头杆
draw.rectangle([128, 116, 168, 140], fill=arrow)
# 箭头三角
draw.polygon([(168, 92), (216, 128), (168, 164)], fill=arrow)

# 底部命令行光标（闪烁条）
draw.rectangle([44, 196, 92, 212], fill=(78, 205, 176, 255))

# 保存多尺寸 ICO
sizes = [(256, 256), (128, 128), (64, 64), (48, 48), (32, 32), (16, 16)]
img.save("F:/files/cmdsender/cmdSender/app.ico", format="ICO", sizes=sizes)
img.convert("RGB").save("F:/files/cmdsender/cmdSender/app_preview.png", format="PNG")
print("ICO + preview saved")
