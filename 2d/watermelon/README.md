# Watermelon Game (合成大西瓜)

A Suika-style fruit merging game built with Unity 6.

## Quick Start

1. Open `2d/watermelon` in Unity 6.
2. Menu: **Watermelon → Setup Game Scene** (first time)
3. Menu: **Watermelon → Apply Polish To Scene** (fruit art, FX, next-fruit icon)
4. Press **Play**

## Controls

- Move mouse / finger left-right to aim
- Release click / lift finger to drop the fruit
- Match two identical fruits to merge into the next tier
- Game over if fruits stay above the red line

## Fruit Chain

Cherry → Strawberry → Grape → Dekopon → Orange → Apple → Pear → Peach → Pineapple → Melon → Watermelon

## Project Structure

- `Assets/Scripts/` — game logic
- `Assets/Editor/WatermelonGameSetup.cs` — one-click scene builder
- `Assets/Data/` — fruit database and physics material (created by setup)
